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

using Cilador.Graph.Core;
using Cilador.Graph.Factory;
using System;
using System.Linq;

namespace Cilador.Graph.Operations
{
    public static class MergeExtension
    {
        public static ICilGraph Merge(this ICilGraph original, ICilGraph addition)
        {
            return new CilGraph(
                original.Vertices.Concat(addition.Vertices),
                original.ParentChildEdges.Concat(addition.ParentChildEdges),
                original.SiblingEdges.Concat(addition.SiblingEdges),
                original.DependencyEdges.Concat(addition.DependencyEdges));
        }
    }
}
