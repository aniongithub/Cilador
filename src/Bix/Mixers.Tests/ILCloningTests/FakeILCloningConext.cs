﻿/***************************************************************************/
// Copyright 2013-2015 Riley White
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

using Bix.Mixers.ILCloning;
using Mono.Cecil;
using System;

namespace Bix.Mixers.Tests.ILCloningTests
{
    /// <summary>
    /// Fake IL cloning context.
    /// </summary>
    internal class FakeILCloningConext : IILCloningContext
    {
        public FakeILCloningConext()
        {
            this.RootImportObjectDelegate = new Func<object, object>(item => item);
        }

        public TypeDefinition RootSource { get; set; }
        public TypeDefinition RootTarget { get; set; }

        public TItem DynamicRootImport<TItem>(TItem item)
        {
            return (TItem)this.RootImport((dynamic)item);
        }

        public Func<object, object> RootImportObjectDelegate { get; set; }
        private object RootImport(object item)
        {
            var handler = this.RootImportObjectDelegate;
            if (handler != null) { return handler(item); }
            return item;
        }

        public Func<TypeReference, TypeReference> RootImportTypeDelegate { get; set; }
        public TypeReference RootImport(TypeReference type)
        {
            var handler = this.RootImportTypeDelegate;
            if (handler != null) { return handler(type); }
            return type;
        }

        public Func<MethodReference, MethodReference> RootImportMethodDelegate { get; set; }
        public MethodReference RootImport(MethodReference method)
        {
            var handler = this.RootImportMethodDelegate;
            if (handler != null) { return handler(method); }
            return method;
        }

        public Func<FieldReference, FieldReference> RootImportFieldDelegate { get; set; }
        public FieldReference RootImport(FieldReference field)
        {
            var handler = this.RootImportFieldDelegate;
            if (handler != null) { return handler(field); }
            return field;
        }
    }
}
