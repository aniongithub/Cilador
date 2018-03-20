﻿/***************************************************************************/
// Copyright 2013-2018 Riley White
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

using Cilador.Clone;
using Cilador.Fody.Core;
using Cilador.Graph.Factory;
using Mono.Cecil;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cilador.Fody.InterfaceMixins
{
    /// <summary>
    /// This is a method object for a single execution of <see cref="InterfaceMixinWeave"/>.
    /// </summary>
    internal class InterfaceMixinWeaver
    {
        /// <summary>
        /// Creates a new <see cref="InterfaceMixinWeaver"/>.
        /// </summary>
        /// <param name="interfaceType">Interface which will be added to the <paramref name="target"/>.</param>
        /// <param name="mixinType">Type whose implementation will be cloned into the <paramref name="target"/>.</param>
        /// <param name="target">Type which will be modified by this command invocation.</param>
        /// <exception cref="WeavingException">
        /// Thrown if <paramref name="interfaceType"/> is not an interface, if <paramref name="mixinType"/> implements
        /// does not implement <paramref name="interfaceType"/>, or if <paramref name="mixinType"/> implements any interface
        /// other than <paramref name="interfaceType"/>.
        /// </exception>
        public InterfaceMixinWeaver(TypeDefinition interfaceType, TypeDefinition mixinType, TypeDefinition target)
        {
            Contract.Requires(interfaceType != null);
            Contract.Requires(mixinType != null);
            Contract.Requires(target != null);
            Contract.Requires(target.Module != null);
            Contract.Requires(!target.IsValueType);
            Contract.Requires(!target.IsPrimitive);

            Contract.Ensures(this.InterfaceType != null);
            Contract.Ensures(this.Target != null);
            Contract.Ensures(this.Target.IsClass);
            Contract.Ensures(this.Source != null);

            if (!interfaceType.IsInterface)
            {
                throw new WeavingException(string.Format("Configured mixin definition interface type is not an interface: [{0}]", interfaceType.FullName));
            }

            if (mixinType.Interfaces.All(@interface => @interface.InterfaceType.FullName != interfaceType.FullName))
            {
                throw new WeavingException(string.Format(
                    "Configured mixin implementation type [{0}] must implement the interface specified mixin interface definition [{1}]",
                    mixinType.FullName,
                    interfaceType.FullName));
            }

            if (mixinType.Interfaces.Count != 1)
            {
                throw new WeavingException(string.Format(
                    "Configured mixin implementation type [{0}] may implement only the mixin definition interface [{1}]",
                    mixinType.FullName,
                    interfaceType.FullName));
            }

            if (target.Interfaces.Any(@interface => @interface.InterfaceType.Resolve().FullName == interfaceType.FullName))
            {
                throw new WeavingException(string.Format(
                    "Target type [{0}] already implements interface to be mixed [{1}]",
                    target.FullName,
                    interfaceType.FullName));
            }

            this.InterfaceType = interfaceType;
            this.Source = mixinType;
            this.Target = target;
        }

        /// <summary>
        /// Gets or sets the interface which will be added to the <see cref="Target"/>.
        /// </summary>
        public TypeDefinition InterfaceType { get; }

        /// <summary>
        /// Gets or sets the mixin type which is the source of mixin code that will be added to the <see cref="Target"/>
        /// </summary>
        private TypeDefinition Source { get; set; }

        /// <summary>
        /// Gets or sets the target type which will be modified by the command execution.
        /// </summary>
        public TypeDefinition Target { get; }

        /// <summary>
        /// Executes the interface mixin command using the arguments passed into the constuctor.
        /// </summary>
        public void Execute()
        {
            this.Target.Interfaces.Add(new InterfaceImplementation(this.Target.Module.ImportReference(this.InterfaceType)));
            try
            {
                var graph = new CilGraphGetter().Get(this.Source);
                new CloningContext(graph, this.Source, this.Target).Execute();
            }
            catch(InvalidOperationException e)
            {
                throw new WeavingException(e.Message, e);
            }
        }
    }
}
