﻿/***************************************************************************/
// Copyright 2013-2017 Riley White
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
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cilador.Clone
{
    /// <summary>
    /// Clones a root source type's constructor into an existing root target type's constuctor.
    /// </summary>
    internal class ConstructorInitializationCloner :
        ClonerBase<MultiplexedConstructor, MethodBody>,
        ICloneToMethodBody<MultiplexedConstructor>
    {
        /// <summary>
        /// Creates a new <see cref="ConstructorInitializationCloner"/>.
        /// </summary>
        /// <param name="parent">Cloner for the constructor method definition being cloned.</param>
        /// <param name="logicSignatureCloner">Cloner for the signature of the logic portion of the constructor, if any.</param>
        /// <param name="source">Cloning source.</param>
        /// <param name="target">Cloning target.</param>
        public ConstructorInitializationCloner(
            ICloner<object, MethodDefinition> parent,
            ConstructorLogicSignatureCloner logicSignatureCloner,
            MultiplexedConstructor source,
            MethodBody target)
            : base(parent.CloningContext, source)
        {
            Contract.Requires(parent != null);
            Contract.Requires(parent.CloningContext != null);
            Contract.Requires(parent.Source != null);
            Contract.Requires(source != null);
            Contract.Requires(source.Constructor != null);
            Contract.Requires(source.Constructor.Body != null);
            Contract.Requires(target != null);
            Contract.Ensures(this.Parent != null);
            Contract.Ensures(this.ExistingTarget != null);

            this.Parent = parent;
            this.SourceThisParameter = source.Constructor.Body.ThisParameter;
            this.LogicSignatureCloner = logicSignatureCloner;
            this.ExistingTarget = target;
            this.CountOfTargetVariablesBeforeCloning = this.ExistingTarget.Variables == null ? 0 : this.ExistingTarget.Variables.Count;
        }

        /// <summary>
        /// Gets or sets the cloner for the parent root type for the constructor being cloned.
        /// </summary>
        private ICloner<object, MethodDefinition> Parent { get; set; }

        /// <summary>
        /// Gets or sets the cloner for the signature of the logic portion of the constructor, if any.
        /// </summary>
        public ConstructorLogicSignatureCloner LogicSignatureCloner { get; private set; }

        /// <summary>
        /// Gets or sets the This parameter of the source constructor.
        /// </summary>
        public ParameterDefinition SourceThisParameter { get; private set; }

        /// <summary>
        /// Gets the offset for  use in instruction cloning so that referenced variables can
        /// be translated. Normally zero, but in cases where a method is split up, such as for
        /// some constructors, variables may also be split up. This may be set to a non-zero
        /// value for cloners that are cloning only a subset of instructions and variables.
        /// </summary>
        public int GetVariableTranslation(Instruction sourceInstruction)
        {
            int? originalIndex;
            if (!sourceInstruction.TryGetVariableIndex(out originalIndex)) { return 0; }
            Contract.Assert(originalIndex.HasValue);

            int newIndex;
            if (!this.Source.TryGetInitializationVariableIndex(sourceInstruction, out newIndex)) { return 0;}
            return newIndex + this.CountOfTargetVariablesBeforeCloning - originalIndex.Value;
        }

        /// <summary>
        /// Collection of source variables that may be referenced by source instructions
        /// that will be cloned to the target. This may or may not be all variables
        /// as method cloning may split methods into parts, as is the case for some
        /// constructors.
        /// </summary>
        public IEnumerable<VariableDefinition> PossiblyReferencedVariables
        {
            get { return this.Source.InitializationVariables; }
        }

        /// <summary>
        /// Gets the action that should be used for inserting instructions for cloning instructions contained in the method.
        /// </summary>
        public Action<ILProcessor, ICloneToMethodBody<object>, InstructionCloner, Instruction, Instruction> InstructionInsertAction
        {
            get { return InstructionCloner.InsertBeforeExistingInstructionInsertAction; }
        }

        /// <summary>
        /// Determines whether the given instruction is a valid source instruction for the cloner
        /// that should be cloned to a target instruction.
        /// </summary>
        /// <param name="instruction">Instruction to examine.</param>
        /// <returns><c>true</c> if <paramref name="instruction"/> is a valid source instruction that should be cloned, else <c>false</c>.</returns>
        public bool IsValidSourceInstruction(Instruction instruction)
        {
            return instruction != null && this.Source.InitializationInstructions.Contains(instruction);
        }

        /// <summary>
        /// Gets or sets the pre-existing target method body.
        /// </summary>
        private MethodBody ExistingTarget { get; set; }

        /// <summary>
        /// Gets the number of variables that are in the existing target before
        /// the cloning operation begins.
        /// </summary>
        /// <returns>Target constructor method body.</returns>
        public int CountOfTargetVariablesBeforeCloning { get; private set; }

        /// <summary>
        /// Retrieves the existing constructor into which the source's data will be added.
        /// </summary>
        /// <returns>Target constructor method body.</returns>
        protected override MethodBody GetTarget()
        {
            return this.ExistingTarget;
        }

        /// <summary>
        /// Clones the source constructor instructions into the target constructor's method body.
        /// </summary>
        protected override void DoClone()
        {
            Contract.Assert(this.Source.HasInitializationItems);

            var source = this.Source;
            var target = this.Target;

            target.InitLocals = target.InitLocals || source.InitializationVariables.Any();

            if (this.LogicSignatureCloner == null) { return; }

            // we can't re-use multiplexed target constructors from initialization because they may have changed
            var targetMultiplexedConstructor = MultiplexedConstructor.Get(this.CloningContext, this.Target.Method);

            var boundaryInstruction =
                this.Target.Instructions[targetMultiplexedConstructor.BoundaryLastInstructionIndex];
            var targetILProcessor = this.Target.GetILProcessor();

            // insert in reverse order
            targetILProcessor.InsertAfter(
                boundaryInstruction,
                targetILProcessor.Create(OpCodes.Call, this.LogicSignatureCloner.Target));
            targetILProcessor.InsertAfter(boundaryInstruction, targetILProcessor.Create(OpCodes.Ldarg_0));
        }
    }
}
