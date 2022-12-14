// Decompiled with JetBrains decompiler
// Type: DuckGame.ModCircularDependencyException
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DuckGame
{
    [Serializable]
    internal class ModCircularDependencyException : Exception
    {
        public ModCircularDependencyException()
        {
        }

        public ModCircularDependencyException(string message)
          : base(message)
        {
        }

        public ModCircularDependencyException(string message, Exception inner)
          : base(message, inner)
        {
        }

        protected ModCircularDependencyException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }

        private static string CompileStack(Stack<string> stack)
        {
            string str1 = "A circular dependency was detected in the list. Mod load order:\r\n";
            foreach (string str2 in stack)
                str1 = str1 + " " + str2 + "\r\n";
            return str1;
        }

        public ModCircularDependencyException(Stack<string> stack)
          : base(CompileStack(stack))
        {
        }
    }
}
