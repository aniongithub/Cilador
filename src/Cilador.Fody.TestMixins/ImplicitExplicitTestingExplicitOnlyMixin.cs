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

using Cilador.Fody.TestMixinInterfaces;
using System;

namespace Cilador.Fody.TestMixins
{
    public class ImplicitExplicitTestingExplicitOnlyMixin : IInterfaceForImplicitExplicitTesting
    {
        string IInterfaceForImplicitExplicitTesting.Method1() { return "Explicit Method 1"; }
        string IInterfaceForImplicitExplicitTesting.Method2() { return "Explicit Method 2"; }
        string IInterfaceForImplicitExplicitTesting.Method3() { return "Explicit Method 3"; }


        string IInterfaceForImplicitExplicitTesting.Property1 { get { return "Explicit Property 1"; } }
        string IInterfaceForImplicitExplicitTesting.Property2 { get { return "Explicit Property 2"; } }
        string IInterfaceForImplicitExplicitTesting.Property3 { get { return "Explicit Property 3"; } }

        private EventHandler explicitEventHandler1;
        event EventHandler IInterfaceForImplicitExplicitTesting.Event1 { add { this.explicitEventHandler1 += value; } remove { this.explicitEventHandler1 -= value; } }
        private EventHandler explicitEventHandler2;
        event EventHandler IInterfaceForImplicitExplicitTesting.Event2 { add { this.explicitEventHandler2 += value; } remove { this.explicitEventHandler2 -= value; } }
        private EventHandler explicitEventHandler3;
        event EventHandler IInterfaceForImplicitExplicitTesting.Event3 { add { this.explicitEventHandler3 += value; } remove { this.explicitEventHandler3 -= value; } }
    }
}
