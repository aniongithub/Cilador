﻿using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Bix.Mixers.Fody.Core
{
    [ContractClass(typeof(IMixCommandContract))]
    public interface IMixCommand
    {
        bool IsInitialized { get; }
        void Initialize(XElement config);
        void Mix(TypeDefinition target, IMixCommandAttribute mixCommandAttribute);
    }
}
