﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 
namespace Bix.Mixers.Fody.Core {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Bix:Mixers:Fody:Core")]
    [System.Xml.Serialization.XmlRootAttribute("BixMixersConfig", Namespace="urn:Bix:Mixers:Fody:Core", IsNullable=false)]
    public partial class BixMixersConfigType {
        
        private MixCommandConfigTypeBase[] mixCommandConfigField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MixCommandConfig", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MixCommandConfigTypeBase[] MixCommandConfig {
            get {
                return this.mixCommandConfigField;
            }
            set {
                this.mixCommandConfigField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Bix:Mixers:Fody:Core")]
    public abstract partial class MixCommandConfigTypeBase {
    }
}