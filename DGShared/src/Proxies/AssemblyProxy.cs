using DuckGame;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DGShared.src.Proxies
{
    public class AssemblyProxy
    {
        public static Assembly Load(byte[] rawAssembly)
        {
            return ReMapper.Remap(rawAssembly);
        }
    }
}
