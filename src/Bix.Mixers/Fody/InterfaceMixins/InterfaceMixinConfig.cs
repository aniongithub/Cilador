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
namespace Bix.Mixers.Fody.InterfaceMixins {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Bix:Mixers:Fody:InterfaceMixins")]
    [System.Xml.Serialization.XmlRootAttribute("InterfaceMixinConfig", Namespace="urn:Bix:Mixers:Fody:InterfaceMixins", IsNullable=false)]
    public partial class InterfaceMixinConfigType {
        
        private InterfaceMapType[] interfaceMapField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("InterfaceMap", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public InterfaceMapType[] InterfaceMap {
            get {
                return this.interfaceMapField;
            }
            set {
                this.interfaceMapField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Bix:Mixers:Fody:InterfaceMixins")]
    public partial class InterfaceMapType {
        
        private string interfaceField;
        
        private string configGroupField;
        
        private string mixinField;
        
        public InterfaceMapType() {
            this.configGroupField = "";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Interface {
            get {
                return this.interfaceField;
            }
            set {
                this.interfaceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("")]
        public string ConfigGroup {
            get {
                return this.configGroupField;
            }
            set {
                this.configGroupField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Mixin {
            get {
                return this.mixinField;
            }
            set {
                this.mixinField = value;
            }
        }
    }
}