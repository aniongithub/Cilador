﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace Cilador.Fody.Config {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Cilador:Fody:Config")]
    [System.Xml.Serialization.XmlRootAttribute("CiladorConfig", Namespace="urn:Cilador:Fody:Config", IsNullable=false)]
    public partial class CiladorConfigType {
        
        private WeaveConfigTypeBase[] weaveConfigField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("WeaveConfig", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public WeaveConfigTypeBase[] WeaveConfig {
            get {
                return this.weaveConfigField;
            }
            set {
                this.weaveConfigField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(DtoProjectorConfigType))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(InterfaceMixinConfigType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Cilador:Fody:Config")]
    public abstract partial class WeaveConfigTypeBase {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Cilador:Fody:Config")]
    public partial class DtoProjectorMapType {
        
        private string targetAssemblyField;
        
        private string targetNamespaceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TargetAssembly {
            get {
                return this.targetAssemblyField;
            }
            set {
                this.targetAssemblyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TargetNamespace {
            get {
                return this.targetNamespaceField;
            }
            set {
                this.targetNamespaceField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Cilador:Fody:Config")]
    public partial class InterfaceMixinMapType {
        
        private string interfaceField;
        
        private string mixinField;
        
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
        public string Mixin {
            get {
                return this.mixinField;
            }
            set {
                this.mixinField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Cilador:Fody:Config")]
    [System.Xml.Serialization.XmlRootAttribute("InterfaceMixinConfig", Namespace="urn:Cilador:Fody:Config", IsNullable=false)]
    public partial class InterfaceMixinConfigType : WeaveConfigTypeBase {
        
        private InterfaceMixinMapType[] interfaceMixinMapField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("InterfaceMixinMap", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public InterfaceMixinMapType[] InterfaceMixinMap {
            get {
                return this.interfaceMixinMapField;
            }
            set {
                this.interfaceMixinMapField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:Cilador:Fody:Config")]
    [System.Xml.Serialization.XmlRootAttribute("DtoProjectorConfig", Namespace="urn:Cilador:Fody:Config", IsNullable=false)]
    public partial class DtoProjectorConfigType : WeaveConfigTypeBase {
        
        private DtoProjectorMapType[] dtoProjectorMapField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DtoProjectorMap", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DtoProjectorMapType[] DtoProjectorMap {
            get {
                return this.dtoProjectorMapField;
            }
            set {
                this.dtoProjectorMapField = value;
            }
        }
    }
}
