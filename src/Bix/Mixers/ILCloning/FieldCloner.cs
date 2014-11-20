﻿/***************************************************************************/
// Copyright 2013-2014 Riley White
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

using Mono.Cecil;
using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Bix.Mixers.ILCloning
{
    /// <summary>
    /// Clones <see cref="FieldDefinition"/> contents from a source to a target.
    /// </summary>
    internal class FieldCloner : ClonerBase<FieldDefinition>
    {
        /// <summary>
        /// Creates a new <see cref="FieldCloner"/>
        /// </summary>
        /// <param name="ilCloningContext">IL cloning context.</param>
        /// <param name="source">Cloning source.</param>
        /// <param name="target">Cloning target.</param>
        public FieldCloner(ILCloningContext ilCloningContext, FieldDefinition source, FieldDefinition target)
            : base(ilCloningContext, source, target)
        {
            Contract.Requires(ilCloningContext != null);
            Contract.Requires(source != null);
            Contract.Requires(target != null);
        }

        /// <summary>
        /// Clones the field in its entirety
        /// </summary>
        public override void Clone()
        {
            Contract.Requires(this.Target.Name == this.Source.Name);

            this.Target.Attributes = this.Source.Attributes;
            this.Target.Constant = this.Source.Constant;
            this.Target.HasConstant = this.Source.HasConstant;
            this.Target.Offset = this.Source.Offset;

            // TODO research correct usage of field MarshalInfo
            if (this.Source.MarshalInfo == null)
            {
                this.Target.MarshalInfo = null;
            }
            else
            {
                this.Target.MarshalInfo = new MarshalInfo(this.Source.MarshalInfo.NativeType);
            }

            // TODO research correct usage of field InitialValue
            if (this.Source.InitialValue != null)
            {
                var initialValue = new byte[this.Source.InitialValue.LongLength];
                this.Source.InitialValue.CopyTo(initialValue, 0);
                this.Target.InitialValue = initialValue;
            }

            this.Target.FieldType = this.ILCloningContext.RootImport(this.Source.FieldType);

            this.Target.CloneAllCustomAttributes(this.Source, this.ILCloningContext);

            this.IsCloned = true;
        }
    }
}