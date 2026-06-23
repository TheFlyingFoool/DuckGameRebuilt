using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DuckGame
{
    public class MarshalHelper
    {
        public unsafe static List<string> StringUTF8PointersToStringList(IntPtr filelistPtr)
        {
            var fileNameList = new List<string>();
            if (filelistPtr == IntPtr.Zero) {
                return fileNameList;
            }
            byte** filelist = (byte**)filelistPtr;
            int i = 0;
            while (filelist[i] != null)
            {
                // "Marshal.PtrToStringUTF8" is only available in dotnet Core 2.1 or newer.
                // string fileName = Marshal.PtrToStringUTF8((IntPtr)filelist[i]); 
                string fileName = PtrToStringUTF8((IntPtr)filelist[i]);
                fileNameList.Add(fileName);
                i++;
            }
            return fileNameList;
        }

        public unsafe static byte* ToUTF8(string str)
        {
            if (str == null) {
                return null;
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(str + '\0');
            var ptr = FNAPlatform.Malloc(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return (byte*)ptr;
        }

        private static string PtrToStringUTF8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) {
                return null;
            }
            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0) {
                len++;
            }
            byte[] buffer = new byte[len];
            Marshal.Copy(ptr, buffer, 0, len);
            return System.Text.Encoding.UTF8.GetString(buffer);
        }
    }
}
