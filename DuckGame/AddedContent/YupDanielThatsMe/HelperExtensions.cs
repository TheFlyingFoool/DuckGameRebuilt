using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    public static class MyExtensions
    {
        public static unsafe int FixedGetHashCode(this string str)
        {

            fixed (char* ptr = str)
            {
                int num = 352654597;
                int num2 = num;
                int* ptr2 = (int*)ptr;
                int num3;
                for (num3 = str.Length; num3 > 2; num3 -= 4)
                {
                    num = ((num << 5) + num + (num >> 27)) ^ *ptr2;
                    num2 = ((num2 << 5) + num2 + (num2 >> 27)) ^ ptr2[1];
                    ptr2 += 2;
                }

                if (num3 > 0)
                {
                    num = ((num << 5) + num + (num >> 27)) ^ *ptr2;
                }

                return num + num2 * 1566083941;
            }
        }
        public static Type[] SaferGetTypes(this Assembly assembly)
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
