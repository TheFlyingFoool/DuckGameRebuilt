using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public static class DanExtensions
    {
        public static Type[] SaferGetTypes(Assembly assembly)
        {
            Module[] modules = assembly.GetModules(false);
            int moduleLength = modules.Length;
            int typeLength = 0;
            Type[][] array = new Type[moduleLength][];
            for (int i = 0; i < moduleLength; i++)
            {
                try
                {
                    array[i] = modules[i].GetTypes();
                    typeLength += array[i].Length;
                }
                catch (Exception ex)
                {
                    array[i] = new Type[0];
                }
            }
            int LastIndex = 0;
            Type[] types = new Type[typeLength];
            for (int j = 0; j < moduleLength; j++)
            {
                int lengthCopyArray = array[j].Length;
                Array.Copy(array[j], 0, types, LastIndex, lengthCopyArray);
                LastIndex += lengthCopyArray;
            }
            return types;
        }
    }
}
