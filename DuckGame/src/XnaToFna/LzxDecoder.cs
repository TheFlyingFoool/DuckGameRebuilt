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
