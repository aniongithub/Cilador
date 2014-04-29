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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bix.Mixers.Fody.Core
{
    /// <summary>
    /// Resolves assemblies based on the project path and the solution path of a weaving context.
    /// Looks for assemblies in the output path of the project and within a directory named "Tools"
    /// in the solution path of the project.
    /// </summary>
    internal class WeavingContextAssemblyResolver : DefaultAssemblyResolver
    {
        /// <summary>
        /// Creates a new <see cref="WeavingContextAssemblyResolver"/>
        /// </summary>
        /// <param name="weavingContext">Weaving context to pull path data from</param>
        public WeavingContextAssemblyResolver(IWeavingContext weavingContext)
        {
            Contract.Requires(weavingContext != null);

            Contract.Assert(weavingContext.DefineConstants != null);
            Contract.Assert(Directory.Exists(weavingContext.ProjectDirectoryPath));
            Contract.Assert(Directory.Exists(weavingContext.SolutionDirectoryPath));

            var projectRelativePath = Path.Combine("bin", weavingContext.DefineConstants.Contains("DEBUG") ? "Debug" : "Release");
            this.AddSearchDirectory(Path.Combine(weavingContext.ProjectDirectoryPath, projectRelativePath));

            this.AddSearchDirectory(Path.Combine(weavingContext.SolutionDirectoryPath, "Tools"));
        }
    }
}
