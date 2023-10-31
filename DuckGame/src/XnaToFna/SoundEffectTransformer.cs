//// Decompiled with JetBrains decompiler
//// Type: XnaToFna.ContentTransformers.SoundEffectTransformer
//// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
//// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
//// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Reflection;
//using System.Runtime.Serialization;
//using System.Text;

//namespace XnaToFna.ContentTransformers
//{
//  public class SoundEffectTransformer : ContentTypeReader<SoundEffect>
//  {
//    private static readonly Type t_SoundEffect = typeof (SoundEffect);
//    private static readonly FieldInfo f_Instances = typeof (SoundEffect).GetField("Instances", BindingFlags.Instance | BindingFlags.NonPublic);
//    private static readonly List<WeakReference> DummyReferences = new List<WeakReference>();

//    protected override SoundEffect Read(ContentReader input, SoundEffect existing)
//    {
//      CopyingStream baseStream1 = (CopyingStream) input.BaseStream;
//      baseStream1.Copy = false;
//      long position1 = input.BaseStream.Position;
//      input.BaseStream.Seek(3L, SeekOrigin.Begin);
//      bool x360 = input.ReadChar() == 'x';
//      input.BaseStream.Seek(position1, SeekOrigin.Begin);
//      using (BinaryWriter binaryWriter = new BinaryWriter(baseStream1.Output, Encoding.UTF8, true))
//      {
//        uint fmtLength = input.ReadUInt32();
//        ushort num1 = ContentHelper.SwapEndian(x360, input.ReadUInt16());
//        ushort num2 = ContentHelper.SwapEndian(x360, input.ReadUInt16());
//        uint num3 = ContentHelper.SwapEndian(x360, input.ReadUInt32());
//        if (num1 == (ushort) 353 || num1 == (ushort) 358)
//        {
//          input.BaseStream.Seek((long) (uint) ((int) fmtLength - 2 - 2 - 4), SeekOrigin.Current);
//          int num4 = input.ReadInt32();
//          binaryWriter.Write(18U);
//          binaryWriter.Write((ushort) 1);
//          binaryWriter.Write((ushort) 1);
//          binaryWriter.Write(num3);
//          binaryWriter.Write(0U);
//          binaryWriter.Write((ushort) 0);
//          binaryWriter.Write((ushort) 8);
//          binaryWriter.Write((ushort) 0);
//          long position2 = binaryWriter.BaseStream.Position;
//          binaryWriter.Write(0U);
//          input.BaseStream.Seek(position1, SeekOrigin.Begin);
//          Stream baseStream2 = input.BaseStream;
//          Stream output = baseStream1.Output;
//          ContentReader reader = input;
//          string format = num1 == (ushort) 353 ? "XWMA" : "WAVE";
//          int fmtLength1 = num1 == (ushort) 358 ? 52 : (int) fmtLength;
//          int dataLength = num4;
//          int num5 = x360 ? 1 : 0;
//          Action<BinaryWriter> fmtExtraWriter;
//          switch (num1)
//          {
//            case 353:
//              fmtExtraWriter = (Action<BinaryWriter>) null;
//              break;
//            case 358:
//              fmtExtraWriter = (Action<BinaryWriter>) (ffmpegWriter =>
//              {
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//                ffmpegWriter.Write(input.ReadByte());
//                ffmpegWriter.Write(input.ReadByte());
//                ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
//                input.BaseStream.Seek((long) (uint) ((int) fmtLength - 18 - 34), SeekOrigin.Current);
//              });
//              break;
//            default:
//              fmtExtraWriter = (Action<BinaryWriter>) null;
//              break;
//          }
//          Action<Process> soundEffectFeeder = ContentHelper.GenerateSoundEffectFeeder((BinaryReader) reader, format, (uint) fmtLength1, (uint) dataLength, 0U, num5 != 0, fmtExtraWriter);
//          ContentHelper.ConvertAudio(baseStream2, output, soundEffectFeeder, 0L);
//          long position3 = binaryWriter.BaseStream.Position;
//          binaryWriter.BaseStream.Seek(position2, SeekOrigin.Begin);
//          binaryWriter.Write((uint) ((ulong) (position3 - position2) - 4UL));
//          binaryWriter.BaseStream.Seek(position3, SeekOrigin.Begin);
//        }
//        else
//        {
//          binaryWriter.Write(fmtLength);
//          binaryWriter.Write(num1);
//          binaryWriter.Write(num2);
//          binaryWriter.Write(num3);
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
//          binaryWriter.Write(input.ReadBytes((int) fmtLength - 18));
//          int count = input.ReadInt32();
//          binaryWriter.Write(count);
//          binaryWriter.Write(input.ReadBytes(count));
//        }
//        binaryWriter.Write(input.ReadUInt32());
//        binaryWriter.Write(input.ReadUInt32());
//        binaryWriter.Write(input.ReadUInt32());
//        baseStream1.Copy = true;
//        if (existing != null)
//          return existing;
//        existing = (SoundEffect) FormatterServices.GetUninitializedObject(SoundEffectTransformer.t_SoundEffect);
//        SoundEffectTransformer.f_Instances.SetValue((object) existing, (object) SoundEffectTransformer.DummyReferences);
//        return existing;
//      }
//    }
//  }
//}
