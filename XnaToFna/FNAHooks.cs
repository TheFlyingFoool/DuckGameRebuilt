// Decompiled with JetBrains decompiler
// Type: XnaToFna.FNAHooks
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace XnaToFna
{
    public static class FNAHooks
    {
        public static bool Enabled = true;
        private static bool Hooked = false;
        public static FNAHooks.d_GetContentReaderFromXnb orig_GetContentReaderFromXnb;
        //public static Detour h_ctor_ContentReader;
        public static FNAHooks.d_ctor_ContentReader orig_ctor_ContentReader;

        //public static void Hook()
        //{
        //  if (!FNAHooks.Hooked)
        //    return;
        //  FNAHooks.Hooked = true;
        //  Dictionary<string, Func<ContentTypeReader>> dictionary = (Dictionary<string, Func<ContentTypeReader>>) typeof (ContentTypeReaderManager).GetField("typeCreators", BindingFlags.Static | BindingFlags.NonPublic).GetValue((object) null);
        //  dictionary["Microsoft.Xna.Framework.Content.EffectReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553"] = (Func<ContentTypeReader>) (() => (ContentTypeReader) new EffectTransformer());
        //  //dictionary["Microsoft.Xna.Framework.Content.SoundEffectReader"] = (Func<ContentTypeReader>) (() => (ContentTypeReader) new SoundEffectTransformer());
        //  FNAHooks.Hook<FNAHooks.d_GetContentReaderFromXnb>(typeof (ContentManager), out FNAHooks.orig_GetContentReaderFromXnb);
        //  FNAHooks.Hook<FNAHooks.d_ctor_ContentReader>(typeof (ContentReader), out FNAHooks.orig_ctor_ContentReader);
        //}

        internal static MethodBase Find(
          Type type,
          string name,
          List<Type> argTypes,
          bool hasSelf = true)
        {
            Type[] array = argTypes.ToArray();
            MethodBase method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, array, null);
            if (method != null)
                return method;
            if (name.StartsWith("ctor_"))
            {
                MethodBase constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, array, null);
                if (constructor != null)
                    return constructor;
            }
            if (name.StartsWith("get_") || name.StartsWith("set_"))
            {
                PropertyInfo property = type.GetProperty(name.Substring(4), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                MethodBase methodBase = name[0] != 'g' ? property.GetSetMethod(true) : (MethodBase)property.GetGetMethod(true);
                if (methodBase != null)
                    return methodBase;
            }
            argTypes.RemoveAt(0);
            return FNAHooks.Find(type, name, argTypes, false);
        }

        //internal static Detour Hook<T>(Type type, out T trampoline) where T : class
        //{
        //  string name = typeof (T).Name.Substring(2);
        //  List<Type> list = ((IEnumerable<ParameterInfo>) typeof (T).GetMethod("Invoke").GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (arg => arg.ParameterType)).ToList<Type>();
        //  MethodBase to = FNAHooks.Find(typeof (T).DeclaringType, name, list);
        //  Detour detour = new Detour(FNAHooks.Find(type, name, list), to);
        //  trampoline = detour.GenerateTrampoline<T>();
        //  return detour;
        //}

        public static ContentReader GetContentReaderFromXnb(
          ContentManager self,
          string originalAssetName,
          ref Stream stream,
          BinaryReader xnbReader,
          char platform,
          Action<IDisposable> recordDisposableObject)
        {
            Stream output = File.OpenWrite(originalAssetName + ".tmp");
            long position = xnbReader.BaseStream.Position;
            xnbReader.BaseStream.Seek(0L, SeekOrigin.Begin);
            using (BinaryWriter binaryWriter = new BinaryWriter(output, Encoding.ASCII, true))
            {
                binaryWriter.Write(xnbReader.ReadBytes(5));
                byte num = (byte)(xnbReader.ReadByte() & 4294967167U);
                binaryWriter.Write(num);
                binaryWriter.Write(0);
            }
            xnbReader.BaseStream.Seek(position, SeekOrigin.Begin);
            ContentReader contentReaderFromXnb = FNAHooks.orig_GetContentReaderFromXnb(self, originalAssetName, ref stream, xnbReader, platform, recordDisposableObject);
            ((CopyingStream)contentReaderFromXnb.BaseStream).Output = output;
            return contentReaderFromXnb;
        }

        public static void ctor_ContentReader(
          ContentReader self,
          ContentManager manager,
          Stream stream,
          GraphicsDevice graphicsDevice,
          string assetName,
          int version,
          char platform,
          Action<IDisposable> recordDisposableObject)
        {
            stream = new CopyingStream(stream, null);
            FNAHooks.orig_ctor_ContentReader(self, manager, stream, graphicsDevice, assetName, version, platform, recordDisposableObject);
        }

        public delegate ContentReader d_GetContentReaderFromXnb(
          ContentManager self,
          string originalAssetName,
          ref Stream stream,
          BinaryReader xnbReader,
          char platform,
          Action<IDisposable> recordDisposableObject);

        public delegate void d_ctor_ContentReader(
          ContentReader self,
          ContentManager manager,
          Stream stream,
          GraphicsDevice graphicsDevice,
          string assetName,
          int version,
          char platform,
          Action<IDisposable> recordDisposableObject);
    }
}
