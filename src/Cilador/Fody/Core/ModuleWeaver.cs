﻿/***************************************************************************/
// Copyright 2013-2019 Riley White
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
/***************************************************************************/

using Cilador.Fody.Config;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;

namespace Cilador.Fody.Core
{
    /// <summary>
    /// The <see cref="ModuleWeaver.Execute()"/> method is invoked by Fody as a Visual
    /// Studio post-build step. This type serves to start the weaving process and to manage
    /// the configuration of the Fody weaver. It is also passed around as needed to other types
    /// in the aspect of a <see cref="IWeavingContext"/>.
    /// </summary>
    public sealed class ModuleWeaver : IDisposable, IWeavingContext
    {
        #region Construction and Disposal

        /// <summary>
        /// Creates a new <see cref="ModuleWeaver"/>.
        /// </summary>
        public ModuleWeaver()
        {
            Contract.Ensures(this.AssemblyResolver != null);
            Contract.Ensures(this.MetadataResolver != null);

            this.assemblyResolver = new DualAssemblyResolver(this);
            this.MetadataResolver = new MetadataResolver(this.AssemblyResolver);
        }

        /// <summary>
        /// Finalizer to release resources used by the <see cref="ModuleWeaver"/>
        /// </summary>
        ~ModuleWeaver()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Releases resources used by the <see cref="ModuleWeaver"/> in a
        /// deterministic manner
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by the <see cref="ModuleWeaver"/>
        /// </summary>
        /// <param name="isDisposing"><c>true</c> if this is being called explicitly through <see cref="Dispose()"/>, else <c>false</c> if being called by the finalizer during garbage collection.</param>
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                lock (this.containerLock)
                {
                    if (this.compositionHost != null) { this.compositionHost.Dispose(); }
                }
            }
        }

        #endregion

        #region MEF Management

        /// <summary>
        /// Handles locking for MEF lookup
        /// </summary>
        private readonly object containerLock = new object();

        private CompositionHost compositionHost;
        /// <summary>
        /// Gets the Microsoft Extensibility Framework container used by this object
        /// </summary>
        private CompositionHost Container
        {
            get
            {
                if (this.compositionHost == null)
                {
                    lock (this.containerLock)
                    {
                        // Add the loaded assemblies to the container 
                        return new ContainerConfiguration().WithAssembly(this.GetType().Assembly).CreateContainer();
                    }
                }
                return this.compositionHost;
            }
        }

        #endregion

        #region Configuration Handling

        /// <summary>
        /// Called by MEF when a part's imports have been satisfied and it is safe to use.
        /// For this type, it rebuilds some configuration information based on the imported weaver.
        /// </summary>
        [OnImportsSatisfied]
        void OnImportsSatisfied()
        {
            Contract.Assert(this.CiladorConfig != null);
            Contract.Assert(this.Weaves != null);

            foreach (var weave in this.Weaves)
            {
                var weaveConfigType = weave.Metadata.ConfigType;
                WeaveConfigTypeBase weaveConfig;
                if (weaveConfigType == null || this.CiladorConfig.WeaveConfig == null) { weaveConfig = null; }
                else
                {
                    weaveConfig = this.CiladorConfig.WeaveConfig.FirstOrDefault(config => config.GetType() == weave.Metadata.ConfigType);
                    if (weaveConfig == null)
                    {
                        this.LogWarning?.Invoke($"Ignoring weave with no configuration: [{weave.GetType().AssemblyQualifiedName}]");
                        continue;
                    }
                }
                weave.Value.Initialize(this, weaveConfig);
            }
        }

        /// <summary>
        /// Gets or sets the collection of weaves found in the MEF container catalogs.
        /// </summary>
        [ImportMany(nameof(IWeave))]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private IEnumerable<Lazy<IWeave, WeaveMeta>> Weaves { get; set; }

        private XElement config;
        /// <summary>
        /// Gets or sets the configuration element from FodyWeavers.xml.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public XElement Config
        {
            get { return this.config; }
            set
            {
                Contract.Requires(this.Config == null);
                Contract.Requires(value != null);
                Contract.Ensures(this.Config != null);
                Contract.Ensures(this.CiladorConfig != null);
                Contract.Ensures(this.Container != null);

                this.config = value;
                this.CiladorConfig = ReadCiladorConfig(value);

                this.Container.SatisfyImports(this);
            }
        }

        /// <summary>
        /// Gets or sets the contants defined by the build
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public List<string> DefineConstants { get; set; }

        /// <summary>
        /// Gets the contants defined by the build
        /// </summary>
        IReadOnlyCollection<string> IWeavingContext.DefineConstants
        {
            get { return this.DefineConstants; }
        }

        /// <summary>
        /// Gets or sets the path of the target assembly file.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public string AssemblyFilePath { get; set; }

        /// <summary>
        /// Gets or sets the path for the target assembly's project file
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public string ProjectDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the Cilador.Fody addin assembly
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public string AddinDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the target assembly's solution file
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public string SolutionDirectoryPath { get; set; }

        /// <summary>
        /// Deserializes a <see cref="CiladorConfigType"/> from within a config.
        /// <see cref="XElement"/>
        /// </summary>
        /// <remarks>
        /// The configuration type, <see cref="CiladorConfigType"/>, is primarily generated
        /// from WeaveConfig.xsd.
        /// </remarks>
        /// <param name="config">Item that contains the serialized config element.</param>
        /// <returns>Deserialized configurtion object</returns>
        public static CiladorConfigType ReadCiladorConfig(XElement config)
        {
            Contract.Requires(config != null);
            Contract.Ensures(Contract.Result<CiladorConfigType>() != null);

            var childElements = config.Elements();
            var children = childElements as XElement[] ?? childElements.ToArray();
            if (children.Count() != 1)
            {
                throw new WeavingException("Cilador config in FodyWeavers.xml should have exactly one child");
            }

            var firstChild = children.First();
            if (firstChild.Name.NamespaceName != "urn:Cilador:Fody:Config" ||
                firstChild.Name.LocalName != "CiladorConfig")
            {
                throw new WeavingException("Child of Cilador config in FodyWeavers.xml should be CiladorConfig in namespace urn:Cilador:Fody:Config");
            }

            CiladorConfigType deserializedConfig;
            try
            {
                deserializedConfig = firstChild.FromXElement<CiladorConfigType>();
            }
            catch (Exception e)
            {
                throw new WeavingException(
                    "Element urn:Cilador:Fody:Config:CiladorConfig in FodyWeavers.xml could not be deserialized into type of CiladorConfigType",
                    e);
            }

            return deserializedConfig;
        }

        /// <summary>
        /// Gets or sets the strongly typed Cilador configuration object.
        /// </summary>
        public CiladorConfigType CiladorConfig { get; set; }

        #endregion

        #region Logging

        /// <summary>
        /// Gets or sets the logger that displays debug-level output.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public Action<string> LogDebug { get; set; }

        /// <summary>
        /// Gets or sets the logger that displays info-level output. In Visual
        /// Studio, logged items appear in the "Error List" as Message items.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public Action<string> LogInfo { get; set; }

        /// <summary>
        /// Gets or sets the logger that displays warning-level output. In Visual
        /// Studio, logged items appear in the "Error List" as Warning items.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public Action<string> LogWarning { get; set; }

        /// <summary>
        /// Gets or sets the logger that displays warning-level output for a given
        /// CIL instruction's sequence point. In Visual
        /// Studio, logged items appear in the "Error List" as Warning items.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public Action<string, SequencePoint> LogWarningPoint { get; set; }

        /// <summary>
        /// Gets or sets the logger that displays error-level output. In Visual
        /// Studio, logged items appear in the "Error List" as Error items.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public Action<string> LogError { get; set; }

        /// <summary>
        /// Gets or sets the logger that displays error-level output for a given
        /// CIL instruction's sequence point. In Visual
        /// Studio, logged items appear in the "Error List" as Error items.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public Action<string, SequencePoint> LogErrorPoint { get; set; }

        #endregion

        #region Target Assembly Data

        private readonly DualAssemblyResolver assemblyResolver;
        /// <summary>
        /// Gets or sets the object that can find and load assemblies.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public IAssemblyResolver AssemblyResolver
        {
            get { return this.assemblyResolver; }
            set
            {
                // only control the first resolver with this setter
                this.assemblyResolver.Resolver1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the object that can resolve type members.
        /// </summary>
        public IMetadataResolver MetadataResolver { get; }

        /// <summary>
        /// Gets or sets the <see cref="ModuleDefinition"/> for the target assembly.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody sets this value.</remarks>
        public ModuleDefinition ModuleDefinition { get; set; }

        #endregion

        #region Execution

        /// <summary>
        /// Executes the Cilador code weaving logic.
        /// </summary>
        /// <remarks>When run as a Fody addin withing Visual Studio, Fody invokes this method after setting all configuration data.</remarks>
        public void Execute()
        {
            this.assemblyResolver.Resolver2 = new WeavingContextAssemblyResolver(this);

            foreach (var weaveAttributesByTargetType in this.ExtractWeaveAttributesByTargetTypes())
            {
                Contract.Assert(weaveAttributesByTargetType.Key != null);
                Contract.Assert(weaveAttributesByTargetType.Value != null);
                Contract.Assert(weaveAttributesByTargetType.Value.All(customAttribute => customAttribute != null));

                this.FindAndInvokeWeavers(weaveAttributesByTargetType.Key, weaveAttributesByTargetType.Value);
            }
        }

        /// <summary>
        /// Looks at all types within the target assembly, and collects any that are annotated with weave attributes.
        /// Weave command attributes are removed from types as they are gathered.
        /// </summary>
        /// <returns>Collection keyed by types the are annotated with at least one weave attribute. Values are the collection of weaves on the key item.</returns>
        private Dictionary<TypeDefinition, List<CustomAttribute>> ExtractWeaveAttributesByTargetTypes()
        {
            Contract.Ensures(Contract.Result<Dictionary<TypeDefinition, List<CustomAttribute>>>().Keys.All(type => type != null));
            Contract.Ensures(!Contract.Result<Dictionary<TypeDefinition, List<CustomAttribute>>>().Values.Any(
                customAttributes => customAttributes == null || customAttributes.Any(customAttribute => customAttribute == null)));

            var weaveAttributeInterfaceType = this.ModuleDefinition.ImportReference(typeof(IWeaveAttribute)).Resolve();

            var weaveAttributesByTargetTypes = new Dictionary<TypeDefinition, List<CustomAttribute>>();
            foreach (var type in this.ModuleDefinition.Types)
            {
                Contract.Assert(type != null);
                Contract.Assert(type.CustomAttributes != null);

                for (var i = type.CustomAttributes.Count - 1; i >= 0; i--)
                {
                    var attribute = type.CustomAttributes[i];
                    Contract.Assert(attribute != null);
                    var attributeTypeDefinition = attribute.AttributeType.Resolve();
                    Contract.Assert(attributeTypeDefinition != null);

                    foreach (var attributeInterfaceImplementation in attributeTypeDefinition.Interfaces)
                    {
                        if (attributeInterfaceImplementation.InterfaceType.Resolve() == weaveAttributeInterfaceType)
                            // TODO custom attributes matter here?
                        {
                            List<CustomAttribute> weaveAttributesForType;
                            if (!weaveAttributesByTargetTypes.TryGetValue(type.Resolve(), out weaveAttributesForType))
                            {
                                weaveAttributesForType = new List<CustomAttribute>();
                                weaveAttributesByTargetTypes[type.Resolve()] = weaveAttributesForType;
                            }

                            weaveAttributesForType.Add(attribute);
                            type.CustomAttributes.RemoveAt(i);
                        }
                    }
                }
            }
            return weaveAttributesByTargetTypes;
        }

        /// <summary>
        /// Looks up commands indicated by weave attributes thorugh configuration, and then
        /// executes each command.
        /// </summary>
        /// <param name="targetType">Type which will be modified by weaves</param>
        /// <param name="weaveAttributes">Collection of weave attributes which indicate weaves that will be applied the <paramref name="targetType"/></param>
        private void FindAndInvokeWeavers(
            TypeDefinition targetType,
            List<CustomAttribute> weaveAttributes)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(weaveAttributes != null);
            Contract.Requires(weaveAttributes.All(customAttribute => customAttribute != null));

            foreach (var weaveAttribute in weaveAttributes)
            {
                this.GetWeaverFor(targetType, weaveAttribute).Weave(this, targetType, weaveAttribute);
            }
        }

        /// <summary>
        /// Finds the weave that corresponds to a given weave attribute for a type
        /// </summary>
        /// <param name="targetType">Type that is the target for the command</param>
        /// <param name="weaveAttribute">Attribute to find command for</param>
        /// <returns>Weave command that corresponds to the given attribute</returns>
        /// <exception cref="InvalidOperationException">No weave is found that corresponds to the <paramref name="weaveAttribute"/></exception>
        private IWeave GetWeaverFor(TypeDefinition targetType, CustomAttribute weaveAttribute)
        {
            Contract.Requires(targetType != null);
            Contract.Requires(weaveAttribute != null);
            Contract.Ensures(Contract.Result<IWeave>() != null);

            var weaveAttributeType = weaveAttribute.AttributeType.Resolve();
            try
            {
                return this.Weaves.First(command =>
                    this.ModuleDefinition.ImportReference(command.Metadata.AttributeType).Resolve() == weaveAttributeType &&
                    command.Value.IsInitialized).Value;
            }
            catch(Exception e)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot find a configured weave for type [{0}] and weave attribute [{1}]", targetType.FullName, weaveAttributeType.FullName),
                    e);
            }
        }

        #endregion
    }
}
