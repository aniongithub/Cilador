﻿using Mono.Cecil;
using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Bix.Mixers.Fody.ILCloning
{
    internal class PropertyCloner : MemberClonerBase<PropertyDefinition, PropertySourceWithRoot>
    {
        public PropertyCloner(PropertyDefinition target, PropertySourceWithRoot source)
            : base(target, source)
        {
            Contract.Requires(target != null);
            Contract.Requires(source != null);
        }

        public override void CloneStructure()
        {
            Contract.Assert(this.Target.DeclaringType != null);
            Contract.Assert(this.Target.Name == this.SourceWithRoot.Source.Name);

            this.Target.Attributes = this.SourceWithRoot.Source.Attributes;
            this.Target.Constant = this.SourceWithRoot.Source.Constant;
            this.Target.HasConstant = this.SourceWithRoot.Source.HasConstant;
            this.Target.HasDefault = this.SourceWithRoot.Source.HasDefault;
            this.Target.HasThis = this.SourceWithRoot.Source.HasThis;
            this.Target.IsRuntimeSpecialName = this.SourceWithRoot.Source.IsRuntimeSpecialName;
            this.Target.IsSpecialName = this.SourceWithRoot.Source.IsSpecialName;

            this.Target.PropertyType = this.SourceWithRoot.RootImport(this.SourceWithRoot.Source.PropertyType);

            for (int i = 0; i < this.SourceWithRoot.Source.OtherMethods.Count; i++)
            {
                this.Target.OtherMethods.Add(null);
            }

            foreach(var method in this.Target.DeclaringType.Methods)
            {
                if (this.SourceWithRoot.Source.GetMethod != null &&
                    this.Target.GetMethod == null &&
                    method.SignatureEquals(this.SourceWithRoot.Source.GetMethod))
                {
                    this.Target.GetMethod = this.SourceWithRoot.RootImport(method).Resolve();
                }

                if (this.SourceWithRoot.Source.SetMethod != null &&
                    this.Target.SetMethod == null &&
                    method.SignatureEquals(this.SourceWithRoot.Source.SetMethod))
                {
                    this.Target.SetMethod = this.SourceWithRoot.RootImport(method).Resolve();
                }

                for (int i = 0; i < this.SourceWithRoot.Source.OtherMethods.Count; i++)
                {
                    if (this.Target.OtherMethods[i] != null &&
                        this.Target.OtherMethods[i] == null &&
                        method.SignatureEquals(this.SourceWithRoot.Source.OtherMethods[i]))
                    {
                        this.Target.OtherMethods[i] = this.SourceWithRoot.RootImport(method).Resolve();
                    }
                }
            }
            this.Target.MetadataToken = this.SourceWithRoot.Source.MetadataToken;

            // I get a similar issue here as with the duplication in the FieldCloner...adding a clear line to work around
            this.Target.CustomAttributes.Clear();
            this.Target.RootImportAllCustomAttributes(this.SourceWithRoot, this.SourceWithRoot.Source.CustomAttributes);

            if (this.SourceWithRoot.Source.HasParameters)
            {
                // TODO property parameter cloning
                throw new NotImplementedException("Implement property parameters when needed");
            }

            this.IsStructureCloned = true;

            Contract.Assert(this.Target.SignatureEquals(this.SourceWithRoot.Source));
            Contract.Assert((this.Target.GetMethod == null) == (this.SourceWithRoot.Source.GetMethod == null));
            Contract.Assert((this.Target.SetMethod == null) == (this.SourceWithRoot.Source.SetMethod == null));
            for (int i = 0; i < this.SourceWithRoot.Source.OtherMethods.Count; i++)
            {
                Contract.Assert((this.Target.OtherMethods[i] == null) == (this.SourceWithRoot.Source.OtherMethods[i] == null));
            }
        }
    }
}
