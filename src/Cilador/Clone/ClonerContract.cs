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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Cilador.Clone
{
    /// <summary>
    /// Contracts for <see cref="ICloner{TSource, TTarget}"/>.
    /// </summary>
    [ContractClassFor(typeof(ICloner<,>))]
    internal abstract class ClonerContract<TSource, TTarget> : ICloner<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        /// <summary>
        /// Gets the context for cloning.
        /// </summary>
        public ICloningContext CloningContext
        {
            get
            {
                Contract.Ensures(Contract.Result<ICloningContext>() != null);
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Contracts for <see cref="CreateTarget()"/>
        /// </summary>
        /// <returns></returns>
        protected TTarget CreateTarget()
        {
            Contract.Ensures(Contract.Result<TTarget>() != null);
            throw new NotSupportedException();
        }

        /// <summary>
        /// Contracts for <see cref="ICloner{TSource, TTarget}.IsCloned"/>.
        /// </summary>
        public bool IsCloned
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Contracts for <see cref="ICloner{TSource,TTarget}.Clone"/>.
        /// </summary>
        public void Clone()
        {
            Contract.Requires(!this.IsCloned);
            Contract.Ensures(this.IsCloned);
            throw new NotSupportedException();
        }

        /// <summary>
        /// Contracts for <see cref="ICloner{TSource, TTarget}.Source"/>
        /// </summary>
        public TSource Source
        {
            get
            {
                Contract.Ensures(Contract.Result<TSource>() != null);
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Contracts for <see cref="ICloner{TSource, TTarget}.Target"/>
        /// </summary>
        public TTarget Target
        {
            get
            {
                Contract.Ensures(Contract.Result<TTarget>() != null);
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// No contracts for target transform.
        /// </summary>
        public IList<Action<ICloner<object, object>>> TargetTransforms
        {
            get => throw new NotImplementedException();
        }
    }
}
