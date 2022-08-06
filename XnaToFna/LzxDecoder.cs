//// Decompiled with JetBrains decompiler
//// Type: XnaToFna.LzxDecoder
//// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
//// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
//// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

//using Microsoft.Xna.Framework;
//using MonoMod.Utils;
//using System;
//using System.IO;
//using System.Reflection;

//namespace XnaToFna
//{
//  public class LzxDecoder
//  {
//    private static readonly Type t_orig = typeof (Game).Assembly.GetType("Microsoft.Xna.Framework.Content.LzxDecoder");
//    private static readonly Type t_proxy = typeof (LzxDecoder);
//    private static readonly ConstructorInfo ctor = LzxDecoder.t_orig.GetConstructor(new Type[1]
//    {
//      typeof (int)
//    });
//    private readonly object _;
//    private static FastReflectionDelegate _Decompress = LzxDecoder.t_orig.GetMethod("Decompress", BindingFlags.Instance | BindingFlags.Public).GetFastDelegate();

//    public LzxDecoder(int window) => this._ = LzxDecoder.ctor.Invoke(new object[1]
//    {
//      (object) window
//    });

//    public int Decompress(Stream inData, int inLen, Stream outData, int outLen) => (int) LzxDecoder._Decompress(this._, new object[4]
//    {
//      (object) inData,
//      (object) inLen,
//      (object) outData,
//      (object) outLen
//    });
//  }
//}
