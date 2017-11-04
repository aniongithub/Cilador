﻿/***************************************************************************/
// Copyright 2013-2017 Riley White
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
using Cilador.Fody.Core;
using Cilador.Fody.TestMixinInterfaces;
using Cilador.Fody.TestMixins;
using Cilador.Fody.TestMixinTargets;
using Cilador.Fody.Tests.Common;
using NUnit.Framework;
using System;

namespace Cilador.Fody.Tests.InterfaceMixinTests
{
    [TestFixture]
    public class UnsupportedFunctionalityFixture
    {
        [Test]
        public void CannotUseAbstractMixin()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(AbstractMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning root source type cannot be abstract: [{0}]",
                    typeof(AbstractMixin).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotUseOpenGenericMixinWithoutProvidingTypeArguments()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(GenericMixinImplementation<>).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning root source type cannot be an open generic type: [{0}]",
                    typeof(GenericMixinImplementation<>).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotUseInterfaceTypeMixin()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(InterfaceTypeMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.AssignableTo<Exception>(),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotUseValueTypeMixin()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(ValueTypeMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.AssignableTo<Exception>(),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void MustImplementInterfaceForMixin()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(InterfacelessMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Configured mixin implementation type [{0}] must implement the interface specified mixin interface definition [{1}]",
                    typeof(InterfacelessMixin).FullName,
                    typeof(IEmptyInterface).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void MixinInterfaceMustBeAnInterface()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(NotAValidMixinInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(IncorrectMixinSpecifyingClassInsteadOfInterface).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Configured mixin definition interface type is not an interface: [{0}]",
                    typeof(NotAValidMixinInterface).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void MixinImplementationMustInheritDirectlyFromObject()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(InheritingMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning root source type cannot have a base type other than System.Object: [{0}]",
                    typeof(InheritingMixin).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotImplementOtherInterfaces()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(ExtraInterfaceMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Configured mixin implementation type [{0}] may implement only the mixin definition interface [{1}]",
                    typeof(ExtraInterfaceMixin).FullName,
                    typeof(IEmptyInterface).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotMakeUnmanagedCall()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(UnmanagedCallMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning source may not contain extern methods: [{0}]",
                    typeof(UnmanagedCallMixin).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotHaveSecurityAttributeOnMixinImplementation()
        {
            var config = new CiladorConfigType();
            ;
            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(SecurityDeclarationMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning root source type may not be annotated with security attributes: [{0}]",
                    typeof(SecurityDeclarationMixin).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotHaveSecurityAttributeOnNestedType()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(SecurityDeclarationOnNestedTypeMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning source type may not contain nested types annotated with security attributes: [{0}]",
                    typeof(SecurityDeclarationOnNestedTypeMixin).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void CannotHaveSecurityAttributeOnMethod()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(SecurityDeclarationOnMethodMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning source type may not contain methods annotated with security attributes: [{0}]",
                    typeof(SecurityDeclarationOnMethodMixin).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void TargetCannotAlreadyImplmentMixinDefinitionInterface()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IIsAlreadyImplementedTester).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(InterfaceIsAlreadyImplementedMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Target type [{0}] already implements interface to be mixed [{1}]",
                    typeof(InterfaceIsAlreadyImplementedTarget).FullName,
                    typeof(IIsAlreadyImplementedTester).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }

        [Test]
        public void ConstructorsCannotHaveParameters()
        {
            var config = new CiladorConfigType();

            config.WeaveConfig = new WeaveConfigTypeBase[]
            {
                new InterfaceMixinConfigType
                {
                    InterfaceMixinMap = new InterfaceMixinMapType[]
                    {
                        new InterfaceMixinMapType
                        {
                            Interface = typeof(IEmptyInterface).GetShortAssemblyQualifiedName(),
                            Mixin = typeof(ConstructorWithParametersMixin).GetShortAssemblyQualifiedName()
                        }
                    }
                },
            };

            Assert.Throws(
                Is.TypeOf((typeof(WeavingException)))
                .And.Message.EqualTo(string.Format(
                    "Cloning root source type cannot have constructors with parameters: [{0}]",
                    typeof(ConstructorWithParametersMixin).FullName)),
                () => ModuleWeaverHelper.WeaveAndLoadTestTarget("Cilador.Fody.TestMixinTargets", config));
        }
    }
}
