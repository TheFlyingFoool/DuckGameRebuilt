using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Builder.Metadata;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Serialized;
using AsmResolver.DotNet.Signatures;
using AsmResolver.DotNet.Signatures.Types;
using AsmResolver.PE.DotNet.Cil;
using DuckGame.Proxies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DGShared.src.Proxies;
#if __ANDROID__
using HarmonyLib;
#endif
namespace DuckGame
{
#if __ANDROID__
    [HarmonyPatch]
#endif
    internal static class ReMapper
    {
        static ModuleDefinition EngineDef;
        static ModuleDefinition GameDef;
        static ModuleDefinition Mscorlib;
        static ModuleDefinition SystemCore;
        static CorLibTypeFactory CorLibFactory;

        static ReMapper()
        {
            EngineDef = ModuleDefinition.FromBytes(File.ReadAllBytes(typeof(Game).Assembly.Location));
            GameDef = ModuleDefinition.FromBytes(File.ReadAllBytes(typeof(TitleScreen).Assembly.Location));
            Mscorlib = ModuleDefinition.FromBytes(File.ReadAllBytes(typeof(Array).Assembly.Location));
            CorLibFactory = new CorLibTypeFactory(Mscorlib);
            SystemCore = ModuleDefinition.FromBytes(File.ReadAllBytes(typeof(HashSet<>).Assembly.Location));
#if __ANDROID__
            //new Harmony("Test").PatchAll(typeof(ReMapper).Assembly);
#endif
        }
#if __ANDROID__
        [HarmonyPatch(typeof(CilMethodBodySerializer), "SerializeMethodBody")]
        public static void Prefix(MethodDefinition method)
        {
            ownermethod = method;
        }

        [HarmonyPatch(typeof(CilAssembler), "WriteInstruction")]
        public static void Prefix(CilInstruction instruction)
        {
            ownerInst = instruction;
        }

        [HarmonyPatch(typeof(CilAssembler), "WriteInstruction")]
        public static void Postfix(CilInstruction instruction)
        {
            ownerInst = null;
        }

        [HarmonyPatch(typeof(CilMethodBodySerializer), "SerializeMethodBody")]
        public static void Postfix()
        {
            ownermethod = null;
        }

        static MethodDefinition ownermethod;
        static CilInstruction ownerInst;

        [HarmonyPatch(typeof(MemberNotImportedException), MethodType.Constructor, new Type[] {typeof(IMetadataMember) })]
        public static void Prefix(IMetadataMember member)
        {
            var imodule = (member as IModuleProvider);
            if(imodule != null)
            {

            }
            StackTrace st = new StackTrace();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("Start");
            foreach(var f in st.GetFrames())
            {
                var m = f.GetMethod();
                sb.AppendLine($"{m.GetFullName()}");
            }
            sb.AppendLine("End");
            File.AppendAllText(MainActivity.DocPath + "Error.txt", sb.ToString());
        }
#endif

        internal static List<(string name, ModuleDefinition Def, bool useContains)> replace;
        internal static ReferenceImporter importer;
        static TypeSignature MapType(TypeSignature t)
        {
            if(t is CorLibTypeSignature corsig)
            {
                var retclt = GameDef.CorLibTypeFactory.FromElementType(corsig.ElementType).ToTypeDefOrRef().ImportWith(importer);
                if (!retclt.IsImportedInModule(importer.TargetModule))
                {

                }
                return retclt.ToTypeSignature();
            }
            return MapType(t.ToTypeDefOrRef()).ToTypeSignature();
        }

        static string GetScope(TypeSignature type)
        {
            return type.Scope is TypeReference tr ? GetScope(tr.ToTypeSignature()) : type.Scope.Name;
        }
        static TypeDefinition MapTypeNP(ITypeDefOrRef t)
        {
            var t2 = t.ToTypeSignature();
            bool isNested = t2.Scope is TypeReference;
            string scopename = GetScope(t2);
            string name = t2.Name;
            if (t2 is CorLibTypeSignature corsig)
            {
                var ret = CorLibFactory.FromElementType(corsig.ElementType).ToTypeDefOrRef();
                if (ret is TypeDefinition td)
                {
                    return td;
                }
            }
            if (t2 is GenericInstanceTypeSignature gits)
            {
                MapTypeList(gits.TypeArguments);
                var i = name.IndexOf('`');
                if (i != -1)
                {
                    name = name.Substring(0, i + 2);
                }
            }
            var r = replace.FirstOrDefault(x => x.useContains ? scopename.Contains(x.name) : scopename.StartsWith(x.name));
            if (r.name != null)
            {
                Stack<string> stack = new Stack<string>();
                var scope = t2.Scope;
                while(scope is TypeReference tr)
                {
                    stack.Push(tr.Name);
                    scope = tr.Scope;
                }
                if (stack.Count == 0)
                {
                    var ts1 = r.Def.TopLevelTypes.FirstOrDefault(x => x.Name == name);
                    if (ts1 == null)
                    {
                        Console.WriteLine($"Type missing: {t2.FullName}");
                        return null;
                    }
                    return ts1;
                }
                string tname = stack.Pop();
                var ts2 = r.Def.TopLevelTypes.FirstOrDefault(x => x.Name == tname);
                while(stack.Count > 0 && ts2 != null)
                {
                    tname = stack.Pop();
                    ts2 = ts2.NestedTypes.FirstOrDefault(x => x.Name == tname);
                }
                var i2 = name.IndexOf('<');
                if (i2 != -1)
                {
                    name = name.Substring(0, i2);
                }
                var nt = ts2.NestedTypes.FirstOrDefault(x => x.Name == name);
                if (nt != null)
                {
                    return nt;
                }
            }
            return null;
        }

        static ITypeDefOrRef MapType(ITypeDefOrRef t)
        {
            var t2 = t.ToTypeSignature();
            ITypeDefOrRef ret = null;
            if (t2 is SzArrayTypeSignature ats)
            {
                if (ats.BaseType == null)
                    return t;
                var t3 = MapTypeNP(ats.BaseType.ToTypeDefOrRef());
                if (t3 == null)
                    return t;
                ret = t3.MakeSzArrayType().ToTypeDefOrRef().ImportWith(importer);
                return ret;
            }
            else if (t2 is ByReferenceTypeSignature brts && brts.BaseType != null)
            {
                var t3 = MapTypeNP(brts.BaseType.ToTypeDefOrRef());
                if (t3 == null)
                    return t;
                ret = t3.MakeByReferenceType().ImportWith(importer).ToTypeDefOrRef();
            }
            else if (t2 is CorLibTypeSignature corsig)
            {
                ret = GameDef.CorLibTypeFactory.FromElementType(corsig.ElementType).ToTypeDefOrRef().ImportWith(importer);
            }
            else
            {
                var ret2 = MapTypeNP(t);
                if (ret2 == null)
                    return t;
                ret = ret2.ImportWith(importer);
            }
            return ret;
        }

        static void MapTypeList(IList<TypeSignature> types)
        {
            for (int i = 0; i < types.Count; i++)
            {
                types[i] = MapType(types[i]);
            }
        }

        static PropertyInfo modDefFactorySetter = typeof(ModuleDefinition).GetProperty("CorLibTypeFactory");

        static void MapCustomAttributes(IList<CustomAttribute> attributes)
        {
            foreach (var a in attributes)
            {
                var ts = MapTypeNP(a.Constructor.DeclaringType);
                if (ts == null)
                    continue;
                var member = ts.Methods.FirstOrDefault(x => x.FullName == a.Constructor.FullName || (x.Name == a.Constructor.Name && Match(x.Signature, a.Constructor.Signature)));
                if (member == null)
                {
                    //Console.WriteLine($"MMR Member missing: {mr.FullName}");
                    continue;
                }
                a.Constructor = (ICustomAttributeType)member.ImportWith(importer);
                foreach (var n in a.Signature.NamedArguments)
                {
                    n.ArgumentType = MapType(n.ArgumentType);
                    n.Argument.ArgumentType = MapType(n.Argument.ArgumentType);
                }
            }
        }

        public static Assembly Remap(string path)
        {
            return Remap(File.ReadAllBytes(path), path);
        }


        public static Assembly Remap(byte[] buffer, string path = "")
        {
            //if (path != "" && File.Exists(path.Replace(".dll", " Remap.dll")))
            //{
            //    return Assembly.Load(File.ReadAllBytes(path.Replace(".dll", " Remap.dll")));
            //}
            var modParams = new ModuleReaderParameters();
            modParams.PEReaderParameters.ErrorListener = EmptyErrorListener.Instance;
            var mod = (SerializedModuleDefinition)ModuleDefinition.FromBytes(buffer, modParams);
            //var newmod = new ModuleDefinition(mod.Name, new AssemblyReference("mscorlib", new Version("2.0.5.0")));
            modDefFactorySetter.SetValue(mod, CorLibFactory);
            importer = mod.DefaultImporter;
            //importer = newmod.DefaultImporter;
            var refs = mod.AssemblyReferences;
            replace = new List<(string name, ModuleDefinition Def, bool useContains)> {
                ("Microsoft.Xna.Framework", EngineDef, true),
            };
#if __ANDROID__
            replace.Add(("DuckGame", GameDef, false));
            replace.Add(("mscorlib", Mscorlib, false));
            replace.Add(("System.Core", SystemCore, false));
#endif
            string[] toRemove = new string[] { "Microsoft.Xna.Framework", "Microsoft.Xna.Framework.Game",
            "Microsoft.Xna.Framework.Graphics", "Microsoft.Xna.Framework.Xact"};
            //newmod.TopLevelTypes.Clear();
            MapCustomAttributes(mod.Assembly.CustomAttributes);
            foreach (var t in mod.GetAllTypes())
            {
                MapCustomAttributes(t.CustomAttributes);
                for (int i1 = 0; i1 < t.Interfaces.Count; i1++)
                {
                    InterfaceImplementation i = t.Interfaces[i1];
                    var r = replace.FirstOrDefault(x => x.useContains ? i.Interface.Scope.Name.Contains(x.name) : i.Interface.Scope.Name == x.name);
                    if (r.name != null)
                    {
                        var ts1 = r.Def.TopLevelTypes.FirstOrDefault(x => x.IsInterface && x.Name == i.Interface.Name);
                        if (ts1 == null)
                        {
                            Console.WriteLine($"Type missing: {t.BaseType.FullName}");
                        }
                        else
                        {
                            var ts2 = mod.DefaultImporter.ImportType(ts1);
                            t.Interfaces[i1] = new InterfaceImplementation(ts2);
                        }
                    }
                }
                if (t.BaseType != null)
                {
                    t.BaseType = MapType(t.BaseType);
                }
                for (int i = 0; i < t.Fields.Count; i++)
                {
                    FieldDefinition f = t.Fields[i];
                    var ts = MapTypeNP(f.Signature.FieldType.ToTypeDefOrRef());
                    if (ts == null)
                        continue;
                    //var fd = new FieldDefinition(f.Name, f.Attributes, ts.ImportWith(importer).ToTypeSignature());

                    //f.Signature = fd.Signature.ImportWith(importer);
                    f.Signature = new FieldSignature(ts.ToTypeSignature().ImportWith(importer));
                    MapCustomAttributes(f.CustomAttributes);
                    //t.Fields[i] = fd;
                }
                //newmod.TopLevelTypes.Add(new TypeDefinition());
            }
            foreach (var t in mod.GetAllTypes())
            {
                //MapCustomAttributes(t.CustomAttributes);
                //for (int i1 = 0; i1 < t.Interfaces.Count; i1++)
                //{
                //    InterfaceImplementation i = t.Interfaces[i1];
                //    var r = replace.FirstOrDefault(x => x.useContains ? i.Interface.Scope.Name.Contains(x.name) : i.Interface.Scope.Name == x.name);
                //    if (r.name != null)
                //    {
                //        var ts1 = r.Def.TopLevelTypes.FirstOrDefault(x => x.IsInterface && x.Name == i.Interface.Name);
                //        if (ts1 == null)
                //        {
                //            Console.WriteLine($"Type missing: {t.BaseType.FullName}");
                //        }
                //        else
                //        {
                //            var ts2 = mod.DefaultImporter.ImportType(ts1);
                //            t.Interfaces[i1] = new InterfaceImplementation(ts2);
                //        }
                //    }
                //}
                //if (t.BaseType != null)
                //{
                //    t.BaseType = MapType(t.BaseType);
                //}
                //var fields = t.Fields;
                //for (int i = 0; i < fields.Count; i++)
                //{
                //    FieldDefinition f = fields[i];
                //    var ts = MapTypeNP(f.Signature.FieldType.ToTypeDefOrRef());
                //    if (ts == null)
                //        continue;
                //    var fd = new FieldDefinition(f.Name, f.Attributes, ts.ImportWith(importer).ToTypeSignature());
                //    f.Signature = fd.Signature.ImportWith(importer);
                //    MapCustomAttributes(f.CustomAttributes);
                //}
                foreach (var p in t.Properties)
                {
                    if(p.Name == "priority")
                    {

                    }
                    p.Signature.ReturnType = MapType(p.Signature.ReturnType);
                }
                foreach (var m in t.Methods)
                {
                    if (m.Name.Contains("get_priority"))
                    {

                    }
                    MapTypeList(m.Signature.ParameterTypes);
                    m.Signature.ReturnType = MapType(m.Signature.ReturnType);
                    if (m.CilMethodBody == null) continue;
                    foreach (var v in m.CilMethodBody.LocalVariables)
                    {
                        v.VariableType = MapType(v.VariableType);
                    }
                    foreach (var exh in m.CilMethodBody.ExceptionHandlers)
                    {
                        if (exh.ExceptionType == null)
                            continue;
                        exh.ExceptionType = MapType(exh.ExceptionType);
                    }
                    MapCustomAttributes(m.CustomAttributes);
                    foreach (var il in m.CilMethodBody.Instructions)
                    {
                        string name = il.Operand?.ToString();
                        if (name != null && il.Operand is not string && il.Operand is not CilLocalVariable && il.Operand is not CilInstructionLabel && !il.Operand.GetType().IsValueType)
                        {
                            if (name == "System.Collections.Generic.List`1+Enumerator<Microsoft.Xna.Framework.Graphics.EffectPass> Microsoft.Xna.Framework.Graphics.EffectPassCollection::GetEnumerator()")
                            {

                            }
                            if (il.Operand is TypeSpecification s)
                            {
                                s.Signature = MapType(s.Signature);
                                s.IsImportedInModule(mod);
                                continue;
                            }
                            if (il.Operand is TypeReference tr)
                            {
                                il.Operand = MapType(tr);
                                continue;
                            }
                            if (il.Operand is MemberReference mr)
                            {
                                var ms = mr.Signature as MethodSignature;
                                if (ms != null)
                                {
                                    ms.ReturnType = MapType(ms.ReturnType);
                                    MapTypeList(ms.ParameterTypes);
                                }
                                var ts = MapTypeNP(mr.DeclaringType);
                                if (ts == null)
                                    continue;
                                if (mr.IsMethod)
                                {
                                    if (name == "System.Reflection.Assembly System.Reflection.Assembly::Load(System.Byte[])")
                                    {
                                        il.Operand = importer.ImportMethod(typeof(AssemblyProxy).GetMethod("Load"));
                                        continue;
                                    }
                                    var members = ts.Methods.Where(x => x.FullName == mr.FullName || (x.Name == mr.Name));
                                    var member = members.FirstOrDefault(x => Match(x.Signature, ms));
                                    if (member == null)
                                    {
                                        //Console.WriteLine($"MMR Member missing: {mr.FullName}");
                                        continue;
                                    }
                                    //var ms2 = member.Signature;
                                    //if (ms2 != null)
                                    //{
                                    //    ms2.ReturnType = MapType(ms.ReturnType);
                                    //    MapTypeList(ms2.ParameterTypes);
                                    //}

                                    if (member.Module.TryLookupMember(member.MetadataToken, out IMetadataMember mem) && mem is SerializedMethodDefinition smr)
                                    {
                                        var ms2 = smr.Signature;
                                        ms2.ReturnType = MapType(ms2.ReturnType);
                                        MapTypeList(ms2.ParameterTypes);
                                        il.Operand = smr.ImportWith(importer);
                                    }
                                }
                                else
                                {
                                    var member2 = ts.Fields.FirstOrDefault(x => x.Name == mr.Name);
                                    if (member2 == null)
                                    {
                                        //Console.WriteLine($"FMR Member missing: {mr.FullName}");
                                        continue;
                                    }
                                    il.Operand = member2.ImportWith(importer);
                                }
                                continue;
                            }
                            if (il.Operand is SerializedMethodSpecification smd)
                            {
                                MapTypeList(smd.Signature.TypeArguments);
                                smd.Method.Signature.ReturnType = MapType(smd.Method.Signature.ReturnType);
                                var ts = MapTypeNP(smd.DeclaringType);
                                
                                if (ts == null)
                                {
                                    ts = smd.DeclaringType.Resolve();
                                }
                                if (ts == null)
                                    continue;
                                var sm = ts.Methods.Where(x => x.Name == smd.Name && Match(x.Signature, smd.Method.Signature));
                                var sm2 = sm.SingleOrDefault(x => Match(x.Signature, smd.Method.Signature));
                                if (sm2 == null)
                                {
                                    //Console.WriteLine($"SMD Member missing: {smd.FullName} [{il.Operand.GetType()}]");
                                    continue;
                                }
                                var ret = sm2.ImportWith(mod.DefaultImporter);
                                var ret2 = new MethodSpecification(ret, new GenericInstanceMethodSignature(smd.Signature.TypeArguments));
                                il.Operand = ret2;
                                continue;
                            }
                            if (il.Operand is SerializedFieldDefinition sfd)
                            {
                                //var r = replace.FirstOrDefault(x => x.useContains ? sfd.Signature.FieldType.Scope.Name.Contains(x.name) : sfd.Signature.FieldType.Scope.Name == x.name);
                                //if (r.name == null) continue;
                                var member = t.Fields.FirstOrDefault(x => x.Name == sfd.Name);
                                if (member != null)
                                {
                                    il.Operand = member;
                                    continue;
                                }
                            }
                            if (il.Operand is MethodDefinition md)
                            {
                                var t2 = MapType(md.DeclaringType).Resolve();
                                var member = t2.Methods.FirstOrDefault(x => x.Name == md.Name && Match(x.Signature, md.Signature));
                                if (member == null)
                                {
                                    //Console.WriteLine($"MD Member missing: {md.FullName}");
                                    continue;
                                }
                                var ret = member.ImportWith(mod.DefaultImporter);
                                MapTypeList(ret.Signature.ParameterTypes);
                                ret.Signature.ReturnType = MapType(ret.Signature.ReturnType);
                                var ret2 = ret.ImportWith(mod.DefaultImporter);
                                il.Operand = ret2;
                                if (!ret2.IsImportedInModule(mod))
                                {

                                }
                            }
                            //Console.WriteLine($"Operand {il.Operand} [{il.Operand.GetType()}] not handled");
                        }
                    }
                    //m.CilMethodBody.ComputeMaxStack();
                }
            }

            foreach (string r in toRemove)
            {
                var aref = refs.FirstOrDefault(x => x.Name == r);
                if (aref != null)
                {
                    refs.Remove(aref);
                }
            }
#if __ANDROID__
            var dg = refs.FirstOrDefault(x => x.Name == "DuckGame" && x.Version.Major != 0);
            if (dg != null)
            {
                refs.Remove(dg);
            }
            var corlib = refs.FirstOrDefault(x => x.Name == "mscorlib" && x.Version.ToString() != "2.0.5.0");
            if (corlib != null)
            {
                refs.Remove(corlib);
            }
            var systemCore = refs.FirstOrDefault(x => x.Name == "System.Core" && x.Version.ToString() != "2.0.5.0");
            if (systemCore != null)
            {
                refs.Remove(systemCore);
            }
#endif
            var stream = new MemoryStream();
            FileStream fstream = null;
            try
            {
                mod.Write(stream);
                //mod.Write(fstream);
                if (path != "")
                {
                    fstream = File.Create(path.Replace(".dll", " Remap.dll"));
                    stream.WriteTo(fstream);
                }
                return Assembly.Load(stream.ToArray());
            }
            catch (AggregateException aex)
            {
                throw aex;
            }
        }

        public static bool Match(this MethodSignature sig, MethodSignature sigr)
        {
            if (sig.IsGeneric != sigr.IsGeneric)
                return false;
            var left = sig.ParameterTypes;
            var right = sigr.ParameterTypes;
            if (left.Count - sig.GenericParameterCount != right.Count - sigr.GenericParameterCount)
                return false;
            for (int i = 0; i < left.Count; i++)
            {
                TypeSignature l = left[i];
                TypeSignature r = right[i];
                string lname = l.Name;
                if(l is GenericInstanceTypeSignature gits)
                {
                    lname = gits.GenericType.Name;
                }
                //int li = lname.IndexOf("<!");
                //if (li != -1)
                //{
                //    lname = lname.Substring(0, li);
                //}
                if (l.Name != r.Name && lname != r.Name)
                    return false;
                //if (l.Scope.Name.Replace(".dll", "") != r.Scope.Name.Replace(".dll", "") || l.Name != r.Name)
                //    return false;
            }
            return true;
        }
    }
}

public class CustomNetModuleResolver : INetModuleResolver
{
    public ModuleDefinition Resolve(string name)
    {
        //ModuleDefinition.
        return null;
    }
}