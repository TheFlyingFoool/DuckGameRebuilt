using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace src.XnaToFna
{
    public class TranspilerMapEntry
    {
        public MethodInfo methodInfo;
        public ParameterInfo[] parameters;
        public TranspilerMapEntry(MethodInfo _methodInfo)
        {
            if (_methodInfo == null) 
                throw new ArgumentNullException(nameof(_methodInfo));
            if (!_methodInfo.IsStatic)
                throw new ArgumentException("the method must be static");
            parameters = _methodInfo.GetParameters();
            if (parameters.Length > 1)
                throw new ArgumentException("the method cant have more than one parameters");
            if (parameters.Length == 1 && parameters[0].ParameterType != typeof(List<>).MakeGenericType(typeof(Instruction)))
                throw new ArgumentException("the method parameter must be List<Instruction>");
            methodInfo = _methodInfo;

        }

        public InstructionCollection ProcessILCode(List<Instruction> instructions, MethodDefinition method)
        {
            if (parameters.Length > 0)
                instructions = (List<Instruction>)methodInfo.Invoke(null, new object[] { instructions });
            else
                instructions = (List<Instruction>)methodInfo.Invoke(null, null);
            InstructionCollection returninstructions = new InstructionCollection(method);
            foreach(Instruction instruction in instructions)
            {
                returninstructions.Add(instruction);
            }
            return returninstructions;
        }
    }
}