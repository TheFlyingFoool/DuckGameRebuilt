using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace XnaToFna
{
    public static class XnaToFnaExt
    {
        public static byte[] ReadBytesUntil(this BinaryReader reader, long position) => reader.ReadBytes((int)(position - reader.BaseStream.Position));

        public static void KillIfAlive(this Process p)
        {
            try
            {
                if (p == null || p.HasExited)
                    return;
                p.Kill();
            }
            catch
            {
            }
        }

        public static Thread AsyncPipeErr(this Process p, bool nullify = false)
        {
            Thread thread;
            if (!nullify)
            {
                thread = new Thread(() =>
                {
                    try
                    {
                        StreamReader standardError = p.StandardError;
                        while (!p.HasExited)
                            Console.WriteLine(standardError.ReadLine());
                    }
                    catch
                    {
                    }
                })
                {
                    Name = string.Format("STDERR pipe thread for {0}", p.ProcessName),
                    IsBackground = true
                };
            }
            else
            {
                thread = new Thread(() =>
                {
                    try
                    {
                        StreamReader standardError = p.StandardError;
                        while (!p.HasExited)
                            standardError.ReadLine();
                    }
                    catch
                    {
                    }
                })
                {
                    Name = string.Format("STDERR pipe thread for {0}", p.ProcessName),
                    IsBackground = true
                };
            }
            thread.Start();
            return thread;
        }

        public static Thread AsyncPipeOut(this Process p, bool nullify = false)
        {
            Thread thread;
            if (!nullify)
            {
                thread = new Thread(() =>
                {
                    try
                    {
                        StreamReader standardOutput = p.StandardOutput;
                        while (!p.HasExited)
                            Console.WriteLine(standardOutput.ReadLine());
                    }
                    catch
                    {
                    }
                })
                {
                    Name = string.Format("STDOUT pipe thread for {0}", p.ProcessName),
                    IsBackground = true
                };
            }
            else
            {
                thread = new Thread(() =>
                {
                    try
                    {
                        StreamReader standardOutput = p.StandardOutput;
                        while (!p.HasExited)
                            standardOutput.ReadLine();
                    }
                    catch
                    {
                    }
                })
                {
                    Name = string.Format("STDOUT pipe thread for {0}", p.ProcessName),
                    IsBackground = true
                };
            }
            thread.Start();
            return thread;
        }

        public static T GetTarget<T>(this WeakReference<T> weak) where T : class
        {
            T target;
            return weak.TryGetTarget(out target) ? target : default(T);
        }

        public static void EmitDefault(
          this ILProcessor il,
          TypeReference t,
          bool stind = false,
          bool arrayEmpty = true)
        {
            if (t == null)
            {
                il.Emit(OpCodes.Ldnull);
                if (!stind)
                    return;
                il.Emit(OpCodes.Stind_Ref);
            }
            else
            {
                if (t.MetadataType == MetadataType.Void)
                    return;
                int num1 = t.IsArray & arrayEmpty ? 1 : 0;
                int num2 = 0;
                if (!stind)
                {
                    num2 = il.Body.Variables.Count;
                    il.Body.Variables.Add(new VariableDefinition(t));
                    il.Emit(OpCodes.Ldloca, num2);
                }
                il.Emit(OpCodes.Initobj, t);
                if (stind)
                    return;
                il.Emit(OpCodes.Ldloc, num2);
            }
        }

        public static T GetDefault<T>() => default(T);
    }
}
