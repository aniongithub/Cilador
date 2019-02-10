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

using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Cilador.Clone
{
    /// <summary>
    /// Extension methods used for cloning
    /// </summary>
    public static class CloningExtensions
    {
        /// <summary>
        /// Invokes clone on each item
        /// </summary>
        /// <param name="cloners"></param>
        public static void CloneAll(
            this IEnumerable<ICloner<object, object>> cloners)
        {
            Contract.Requires(cloners != null);
            Contract.Requires(!cloners.Any(cloner => cloner.IsCloned));
            Contract.Ensures(cloners.All(cloner => cloner.IsCloned));

            foreach(var cloner in cloners) { cloner.Clone(); }
        }

        /// <summary>
        /// Determines whether a type is nested within another type, even deeply.
        /// </summary>
        /// <remarks>
        /// Because <paramref name="possibleAncestorType"/> is a type definition, generic type arguments are ignored.
        /// Even if you begin with a closed or partially closed type reference, resolving it strips away all
        /// generic arguments.
        /// </remarks>
        /// <param name="type">Type that may be nested within <paramref name="possibleAncestorType"/></param>
        /// <param name="possibleAncestorType">Type that may contain <paramref name="type"/>.</param>
        /// <returns><c>true</c> if <paramref name="type"/> is nested within <paramref name="possibleAncestorType"/>, else <c>false</c>.</returns>
        [Pure]
        public static bool IsNestedWithin(this TypeReference type, TypeDefinition possibleAncestorType)
        {
            if (type == null) { return false; }

            while (type.DeclaringType != null)
            {
                if (type.DeclaringType.Resolve().FullName == possibleAncestorType.FullName) { return true; }
                type = type.DeclaringType;
            }

            return false;
        }

        /// <summary>
        /// Determines whether a target and source method reference have equivalent signatures within the context
        /// of cloning.
        /// </summary>
        /// <param name="target">Target method..</param>
        /// <param name="source">Source method</param>
        /// <param name="cloningContext">cloning context.</param>
        /// <returns><c>true</c> if the signatures are equivalent within the root target and source, else <c>false</c></returns>
        [Pure]
        public static bool SignatureEquals(this MethodReference target, MethodReference source, ICloningContext cloningContext)
        {
            Contract.Requires(cloningContext != null);

            if (target == null || source == null) { return target == null && source == null; }

            return source.FullName.Replace(cloningContext.RootSource.FullName, cloningContext.RootTarget.FullName) == target.FullName;
        }

        /// <summary>
        /// Determines whether in instruction opcode stores a variable.
        /// </summary>
        /// <param name="code">OpCode's CIL code</param>
        /// <returns><c>true</c> if the code is a variable store code, else <c>false</c>.</returns>
        public static bool IsStoreVariableOpCode(this Code code)
        {
            return
                code == Code.Stloc ||
                code == Code.Stloc_S ||
                code == Code.Stloc_0 ||
                code == Code.Stloc_1 ||
                code == Code.Stloc_2 ||
                code == Code.Stloc_3;
        }

        /// <summary>
        /// Determines whether in instruction opcode loads a variable.
        /// </summary>
        /// <param name="code">OpCode's CIL code</param>
        /// <returns><c>true</c> if the code is a variable load code, else <c>false</c>.</returns>
        public static bool IsLoadVariableOpCode(this Code code)
        {
            return
                code == Code.Ldloc ||
                code == Code.Ldloc_S ||
                code == Code.Ldloc_0 ||
                code == Code.Ldloc_1 ||
                code == Code.Ldloc_2 ||
                code == Code.Ldloc_3;
        }

        /// <summary>
        /// Determines whether in instruction opcode loads a variable address.
        /// </summary>
        /// <param name="code">OpCode's CIL code</param>
        /// <returns><c>true</c> if the code is a variable address load code, else <c>false</c>.</returns>
        public static bool IsLoadVariableAddressOpCode(this Code code)
        {
            return
                code == Code.Ldloca ||
                code == Code.Ldloca_S;
        }

        /// <summary>
        /// Tries to find the index of a variable referenced by an instruction, if any.
        /// </summary>
        /// <param name="instruction">Instruction to examine.</param>
        /// <param name="variableIndex">Index to populate, if found.</param>
        /// <returns><c>true</c> if a referenced variable was found, otherwise <c>false</c>.</returns>
        public static bool TryGetVariableIndex(this Instruction instruction, out int? variableIndex)
        {
            Contract.Requires(instruction != null);
            Contract.Ensures(Contract.ValueAtReturn<int?>(out variableIndex).HasValue || !Contract.Result<bool>());

            switch (instruction.OpCode.Code)
            {
                case Code.Ldloc_0:
                case Code.Stloc_0:
                    variableIndex = 0;
                    return true;

                case Code.Ldloc_1:
                case Code.Stloc_1:
                    variableIndex = 1;
                    return true;

                case Code.Ldloc_2:
                case Code.Stloc_2:
                    variableIndex = 2;
                    return true;

                case Code.Ldloc_3:
                case Code.Stloc_3:
                    variableIndex = 3;
                    return true;

                case Code.Ldloc:
                case Code.Ldloc_S:
                case Code.Ldloca:
                case Code.Ldloca_S:
                case Code.Stloc:
                case Code.Stloc_S:
                    variableIndex = ((VariableDefinition)instruction.Operand).Index;
                    if (variableIndex < 0)
                    {
                        throw new InvalidOperationException(
                            $"Did not expect a negative variable index {((VariableDefinition)instruction.Operand).Index}");
                    }
                    return true;

                default:
                    variableIndex = default(int?);
                    return false;
            }
        }

        /// <summary>
        /// Examines the instruction and, if it accesses a local variable, provides a new version
        /// that translates the access by the given count.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="instruction"/> does not reference a local variable or if the
        /// translation is zero, then the original instruction will be returned. Likewise, if the
        /// instruction operand is a <see cref="VariableDefinition"/> and the translation doesn't
        /// cause the opcode to change (e.g. from stloc_S to stloc), then the original instruction
        /// is returned.
        /// </remarks>
        /// <param name="instruction">Instruction to look at.</param>
        /// <param name="translate">How to translate the referenced parameter, if any is referenced. Negative is a left translation, and positive is right.</param>
        /// <param name="variables">Collection of variables that may be referenced.</param>
        public static Instruction ApplyLocalVariableTranslation(
            this Instruction instruction,
            int translate,
            IEnumerable<VariableDefinition> variables)
        {
            Contract.Requires(instruction != null);
            Contract.Requires(variables != null);

            int? initialVariableIndex;
            if (translate == 0 || !instruction.TryGetVariableIndex(out initialVariableIndex)) { return instruction; }
            Contract.Assert(initialVariableIndex.HasValue);

            var newIndex = initialVariableIndex.Value + translate;
            if (newIndex < 0)
            {
                throw new InvalidOperationException("A variable index less than 0 cannot be used.");
            }
            if (newIndex > ushort.MaxValue)
            {
                throw new InvalidOperationException($"A variable index greater than {ushort.MaxValue} cannot be used.");
            }

            var indexedVariable = new Lazy<VariableDefinition>(
                () =>
                {
                    var value = instruction.Operand as VariableDefinition ??
                                variables.FirstOrDefault(variable => variable.Index == initialVariableIndex.Value);

                    if (value == null)
                    {
                        throw new InvalidOperationException("An instruction references a variable that cannot be found.");
                    }

                    return value;
                });

            if (instruction.OpCode.Code.IsStoreVariableOpCode())
            {
                switch(newIndex)
                {
                    case 0:
                        return Instruction.Create(OpCodes.Stloc_0);

                    case 1:
                        return Instruction.Create(OpCodes.Stloc_1);

                    case 2:
                        return Instruction.Create(OpCodes.Stloc_2);

                    case 3:
                        return Instruction.Create(OpCodes.Stloc_3);

                    default:
                        // only make a new instruction if the opcode needs to change
                        if (newIndex <= byte.MaxValue)
                        {
                            return instruction.OpCode.Code == Code.Stloc_S ?
                                instruction :
                                Instruction.Create(OpCodes.Stloc_S, indexedVariable.Value);
                        }

                        return instruction.OpCode.Code == Code.Stloc ?
                            instruction :
                            Instruction.Create(OpCodes.Stloc, indexedVariable.Value);
                }
            }

            if (instruction.OpCode.Code.IsLoadVariableOpCode())
            {
                switch (newIndex)
                {
                    case 0:
                        return Instruction.Create(OpCodes.Ldloc_0);

                    case 1:
                        return Instruction.Create(OpCodes.Ldloc_1);

                    case 2:
                        return Instruction.Create(OpCodes.Ldloc_2);

                    case 3:
                        return Instruction.Create(OpCodes.Ldloc_3);

                    default:
                        // only make a new instruction if the opcode needs to change
                        if (newIndex <= byte.MaxValue)
                        {
                            return instruction.OpCode.Code == Code.Ldloc_S ?
                                instruction :
                                Instruction.Create(OpCodes.Ldloc_S, indexedVariable.Value);
                        }

                        return instruction.OpCode.Code == Code.Ldloc ?
                            instruction :
                            Instruction.Create(OpCodes.Ldloc, indexedVariable.Value);
                }
            }

            if (!instruction.OpCode.Code.IsLoadVariableAddressOpCode())
            {
                throw new InvalidOperationException("Expected a variable store or load opcode.");
            }

            // only make a new instruction if the opcode needs to change
            if (newIndex <= byte.MaxValue)
            {
                return instruction.OpCode.Code == Code.Ldloca_S ?
                    instruction :
                    Instruction.Create(OpCodes.Ldloca_S, indexedVariable.Value);
            }

            return instruction.OpCode.Code == Code.Ldloca ?
                instruction :
                Instruction.Create(OpCodes.Ldloca, indexedVariable.Value);
        }
    }
}
