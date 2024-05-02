//using Mono.Cecil;
//using MonoMod;
//using MonoMod.Utils;
//using System;
//using System.IO;
//using System.Reflection;

//namespace XnaToFna
//{
//  public class ExampleshtProgram
//  {
//    public static void Main434(string[] args)
//    {
//      XnaToFnaUtil xnaToFnaUtil = new XnaToFnaUtil();
//      xnaToFnaUtil.Log(string.Format("[Version] {0}", (object) MonoModder.Version));
//      xnaToFnaUtil.ScanPath("C:\\Users\\daniel\\Desktop\\Release\\FNA.dll");
//      xnaToFnaUtil.ScanPath("C:\\Users\\daniel\\Desktop\\Release\\XnaToFna.exe");
//      xnaToFnaUtil.RelinkAll();
//      Assembly assembly = FixLoadAssembly(xnaToFnaUtil, "C:\\Users\\daniel\\Desktop\\Release\\demo\\QolMod.dll");
//      File.WriteAllBytes("C:\\Users\\daniel\\Desktop\\Release\\demo\\QolMod2.dll", (byte[]) assembly.GetType().GetMethod("GetRawBytes", BindingFlags.Instance | BindingFlags.NonPublic).Invoke((object) assembly, (object[]) null));
//      Console.WriteLine(assembly.Location);
//      Console.WriteLine(assembly.FullName);
//      xnaToFnaUtil.Log("[Main] Done!");
//      Console.ReadKey();
//    }

//    public static Assembly FixLoadAssembly(XnaToFnaUtil xnaToFnaUtil, string path)
//    {
//      really really = new really();
//      really.name = AssemblyName.GetAssemblyName(path);
//      ReaderParameters rp = xnaToFnaUtil.Modder.GenReaderParameters(false);
//      rp.ReadWrite = path != XnaToFnaUtil.ThisAssembly.Location && !xnaToFnaUtil.Mappings.Exists(new Predicate<XnaToFnaMapping>(really.ScanPath));
//      rp.ReadSymbols = false;
//      ModuleDefinition mod = MonoModExt.ReadModule(path, rp);
//      return xnaToFnaUtil.RelinkToAssembly(mod);
//    }
//  }
//}
