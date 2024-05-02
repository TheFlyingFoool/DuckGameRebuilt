//using MonoMod.Utils;
//using System;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Compression;
//using System.Text;
//using System.Threading;
//using XnaToFna.ContentTransformers;

//namespace XnaToFna
//{
//  public static class ContentHelper
//  {
//    public static bool XNBCompressGZip = true;
//    private static Type t_GZipContentReader = typeof (GZipContentReader<>);
//    public const uint XSBHeader = 1262634067;
//    public const uint XSBHeaderX360 = 1396982347;
//    public const uint XGSHeader = 1179862872;
//    public const uint XGSHeaderX360 = 1481069382;
//    public static ContentHelperGame Game;
//    public const uint XWBHeader = 1145979479;
//    public const uint XWBHeaderX360 = 1463963204;

//    public static void TransformContent(string path)
//    {
//      if (ContentHelper.Game == null || !File.Exists(path))
//        return;
//      ContentHelper.Log(string.Format("[TransformContent] Transforming {0}", (object) path));
//      object obj = ContentHelper.Game.Content.Load<object>(path);
//      try
//      {
//        if (obj is IDisposable disposable)
//          disposable.Dispose();
//        ContentHelper.Game.Content.Unload();
//      }
//      catch
//      {
//      }
//      ContentHelper.UpdateXNBSize(path + ".tmp");
//      File.Delete(path);
//      if (!ContentHelper.XNBCompressGZip)
//      {
//        File.Move(path + ".tmp", path);
//      }
//      else
//      {
//        using (Stream input = (Stream) File.Open(path + ".tmp", FileMode.Open, FileAccess.Read))
//        {
//          using (Stream stream = (Stream) File.Open(path, FileMode.Create, FileAccess.Write))
//          {
//            using (BinaryReader binaryReader = new BinaryReader(input, Encoding.ASCII, true))
//            {
//              using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.ASCII, true))
//              {
//                binaryWriter.Write(binaryReader.ReadBytes(6));
//                binaryWriter.Write(0U);
//                binaryWriter.Write((byte) 1);
//                binaryWriter.Write(ContentHelper.t_GZipContentReader.MakeGenericType(obj.GetType()).AssemblyQualifiedName);
//                binaryWriter.Write(0U);
//                binaryWriter.Write((byte) 0);
//                binaryWriter.Write((byte) 1);
//              }
//            }
//            input.Seek(0L, SeekOrigin.Begin);
//            using (GZipStream destination = new GZipStream(stream, CompressionMode.Compress, true))
//              input.CopyTo((Stream) destination);
//          }
//        }
//        File.Delete(path + ".tmp");
//        ContentHelper.UpdateXNBSize(path);
//      }
//    }

//    public static void UpdateXNBSize(string path, uint size = 0)
//    {
//      using (Stream output = (Stream) File.Open(path, FileMode.Open, FileAccess.ReadWrite))
//      {
//        using (BinaryWriter binaryWriter = new BinaryWriter(output))
//        {
//          if (size == 0U)
//            size = (uint) output.Length;
//          output.Position = 6L;
//          binaryWriter.Write(size);
//        }
//      }
//    }

//    public static void UpdateVideo(string path, BinaryReader reader = null, BinaryWriter writer = null)
//    {
//      if (!ContentHelper.IsFFMPEGAvailable)
//      {
//        ContentHelper.Log("[UpdateVideo] FFMPEG is missing - won't convert unsupported video files");
//        if (reader == null || writer == null)
//          return;
//        reader.BaseStream.CopyTo(writer.BaseStream);
//      }
//      else
//      {
//        string str = Path.ChangeExtension(path, "xnb");
//        if (File.Exists(str + "_"))
//          File.Delete(str + "_");
//        if (File.Exists(str))
//          File.Move(str, str + "_");
//        string path1 = Path.ChangeExtension(path, "ogv");
//        if (writer == null && !string.IsNullOrEmpty(path) && File.Exists(path1))
//          return;
//        ContentHelper.Log(string.Format("[UpdateVideo] Updating video {0}", (object) path));
//        if (writer == null)
//          ContentHelper.RunFFMPEG(string.Format("-i {0} -acodec libvorbis -vcodec libtheora \"{1}\"", reader == null ? (object) string.Format("\"{0}\"", (object) path) : (object) "-", (object) path1), reader?.BaseStream, (Stream) null);
//        else
//          ContentHelper.RunFFMPEG("-y -i - -acodec libvorbis -vcodec libtheora -", reader.BaseStream, writer.BaseStream);
//      }
//    }

//    public static void UpdateAudio(string path, BinaryReader reader = null, BinaryWriter writer = null)
//    {
//      if (!ContentHelper.IsFFMPEGAvailable)
//      {
//        ContentHelper.Log("[UpdateAudio] FFMPEG is missing - won't convert unsupported audio files");
//        if (reader == null || writer == null)
//          return;
//        reader.BaseStream.CopyTo(writer.BaseStream);
//      }
//      else
//      {
//        string str = Path.ChangeExtension(path, "xnb");
//        if (File.Exists(str + "_"))
//          File.Delete(str + "_");
//        if (File.Exists(str))
//          File.Move(str, str + "_");
//        string path1 = Path.ChangeExtension(path, "ogg");
//        if (writer == null && !string.IsNullOrEmpty(path) && File.Exists(path1))
//          return;
//        ContentHelper.Log(string.Format("[UpdateAudio] Updating audio {0}", (object) path));
//        if (writer == null)
//          ContentHelper.RunFFMPEG(string.Format("-i {0} -acodec libvorbis \"{1}\"", reader == null ? (object) string.Format("\"{0}\"", (object) path) : (object) "-", (object) path1), reader?.BaseStream, (Stream) null);
//        else
//          ContentHelper.RunFFMPEG("-y -i - -acodec libvorbis -", reader.BaseStream, writer.BaseStream);
//      }
//    }

//    public static void UpdateSoundBank(string path, BinaryReader reader, BinaryWriter writer)
//    {
//      ContentHelper.Log(string.Format("[UpdateSoundBank] Updating sound bank {0}", (object) path));
//      bool swap = reader.ReadUInt32() == 1396982347U;
//      writer.Write(1262634067U);
//      if (!swap)
//      {
//        reader.BaseStream.CopyTo(writer.BaseStream);
//      }
//      else
//      {
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt64()));
//        byte num1 = reader.ReadByte();
//        writer.Write((byte) 1);
//        if (swap && num1 != (byte) 3 || !swap && num1 != (byte) 1)
//          ContentHelper.Log(string.Format("[UpdateSoundBank] Possible platform mismatch! Platform: 0x{0}; Big endian (X360): {1}", (object) num1.ToString("X2"), (object) swap));
//        ushort num2 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num2);
//        ushort num3 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num3);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        writer.Write(reader.ReadByte());
//        ushort num4 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num4);
//        long position1 = reader.BaseStream.Position;
//        ushort num5 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num5);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        long position2 = reader.BaseStream.Position;
//        uint position3 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position3);
//        long position4 = reader.BaseStream.Position;
//        uint position5 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position5);
//        long position6 = reader.BaseStream.Position;
//        uint position7 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position7);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        long position8 = reader.BaseStream.Position;
//        uint position9 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position9);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        long position10 = reader.BaseStream.Position;
//        uint num6 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(num6);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        long position11 = reader.BaseStream.Position;
//        uint offset = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(offset);
//        uint position12 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position12);
//        writer.Write(reader.ReadBytes(64));
//        writer.Write(reader.ReadBytesUntil((long) position12));
//        for (ushort index1 = 0; (int) index1 < (int) num4; ++index1)
//        {
//          long position13 = reader.BaseStream.Position;
//          byte num7 = reader.ReadByte();
//          writer.Write(num7);
//          byte num8 = 1;
//          writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//          writer.Write(reader.ReadByte());
//          writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//          writer.Write(reader.ReadByte());
//          ushort num9 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//          writer.Write(num9);
//          if (((int) num7 & 1) != 0)
//          {
//            writer.Write(num8 = reader.ReadByte());
//          }
//          else
//          {
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            writer.Write(reader.ReadByte());
//          }
//          if (((int) num7 & 14) != 0)
//          {
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            ushort num10 = 0;
//            if (((int) num7 & 2) != 0)
//              ++num10;
//            if (((int) num7 & 4) != 0)
//              num10 += (ushort) num8;
//            for (ushort index2 = 0; (int) index2 < (int) num10; ++index2)
//            {
//              byte num11 = reader.ReadByte();
//              writer.Write(num11);
//              for (byte index3 = 0; (int) index3 < (int) num11; ++index3)
//                writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            }
//          }
//          if (((int) num7 & 16) != 0)
//          {
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            byte num12 = reader.ReadByte();
//            writer.Write(num12);
//            for (byte index4 = 0; (int) index4 < (int) num12; ++index4)
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//          }
//          if (((int) num7 & 1) != 0)
//          {
//            for (byte index5 = 0; (int) index5 < (int) num8; ++index5)
//            {
//              writer.Write(reader.ReadByte());
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//              writer.Write(reader.ReadByte());
//              writer.Write(reader.ReadByte());
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            }
//            for (byte index6 = 0; (int) index6 < (int) num8; ++index6)
//            {
//              byte num13 = reader.ReadByte();
//              writer.Write(num13);
//              for (byte index7 = 0; (int) index7 < (int) num13; ++index7)
//              {
//                uint num14 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//                writer.Write(num14);
//                writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                ContentHelper.SoundBankEventType soundBankEventType = (ContentHelper.SoundBankEventType) ((int) num14 & 31);
//                byte num15 = reader.ReadByte();
//                if (num15 != byte.MaxValue)
//                  ContentHelper.Log(string.Format("[UpdateSoundBank] Expected 0xFF between event info and data, got 0x{0} instead! ({1}, {2}, {3})", (object) num15.ToString("X2"), (object) index1, (object) index6, (object) index7));
//                writer.Write(num15);
//                switch (soundBankEventType)
//                {
//                  case ContentHelper.SoundBankEventType.Stop:
//                    writer.Write(reader.ReadByte());
//                    break;
//                  case ContentHelper.SoundBankEventType.PlayWave:
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    break;
//                  case ContentHelper.SoundBankEventType.PlayWaveTrackVariation:
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    ushort num16;
//                    ushort num17;
//                    if (num1 == (byte) 1)
//                    {
//                      num16 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                      num17 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                    }
//                    else
//                    {
//                      num17 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                      num16 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                    }
//                    writer.Write(num16);
//                    writer.Write(num17);
//                    writer.Write(reader.ReadUInt32());
//                    for (ushort index8 = 0; (int) index8 < (int) num16; ++index8)
//                    {
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                      writer.Write(reader.ReadByte());
//                      writer.Write(reader.ReadByte());
//                      writer.Write(reader.ReadByte());
//                    }
//                    break;
//                  case ContentHelper.SoundBankEventType.PlayWaveEffectVariation:
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    break;
//                  case ContentHelper.SoundBankEventType.PlayWaveTrackEffectVariation:
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    ushort num18;
//                    ushort num19;
//                    if (num1 == (byte) 1)
//                    {
//                      num18 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                      num19 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                    }
//                    else
//                    {
//                      num19 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                      num18 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                    }
//                    writer.Write(num18);
//                    writer.Write(num19);
//                    writer.Write(reader.ReadUInt32());
//                    for (ushort index9 = 0; (int) index9 < (int) num18; ++index9)
//                    {
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                      writer.Write(reader.ReadByte());
//                      writer.Write(reader.ReadByte());
//                      writer.Write(reader.ReadByte());
//                    }
//                    break;
//                  case ContentHelper.SoundBankEventType.Pitch:
//                  case ContentHelper.SoundBankEventType.Volume:
//                  case ContentHelper.SoundBankEventType.PitchRepeating:
//                  case ContentHelper.SoundBankEventType.VolumeRepeating:
//                    byte num20 = reader.ReadByte();
//                    writer.Write(num20);
//                    if (((int) num20 & 1) != 0)
//                    {
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                      break;
//                    }
//                    writer.Write(reader.ReadByte());
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(reader.ReadUInt32());
//                    writer.Write(reader.ReadByte());
//                    if (soundBankEventType == ContentHelper.SoundBankEventType.PitchRepeating || soundBankEventType == ContentHelper.SoundBankEventType.VolumeRepeating)
//                    {
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                      break;
//                    }
//                    break;
//                  case ContentHelper.SoundBankEventType.Marker:
//                  case ContentHelper.SoundBankEventType.MarkerRepeating:
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    break;
//                }
//              }
//            }
//          }
//          if (reader.BaseStream.Position < position13 + (long) num9)
//            reader.BaseStream.Seek(position13 + (long) num9, SeekOrigin.Begin);
//          else if (reader.BaseStream.Position > position13 + (long) num9)
//            ContentHelper.Log(string.Format("[UpdateSoundBank] Warning: Length of sound data didn't match read data! Expect further errors with this soundbank. ({0})", (object) index1));
//        }
//        if (num2 != (ushort) 0)
//        {
//          writer.Write(reader.ReadBytesUntil((long) position3));
//          writer.Flush();
//          position3 = (uint) writer.BaseStream.Position;
//          for (ushort index = 0; (int) index < (int) num2; ++index)
//          {
//            writer.Write(reader.ReadByte());
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//          }
//        }
//        if (num3 != (ushort) 0)
//        {
//          ushort num21 = 0;
//          writer.Write(reader.ReadBytesUntil((long) position5));
//          writer.Flush();
//          position5 = (uint) writer.BaseStream.Position;
//          for (ushort index = 0; (int) index < (int) num3; ++index)
//          {
//            byte num22 = reader.ReadByte();
//            if (((int) num22 & 4) == 0)
//              ++num21;
//            writer.Write(num22);
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            writer.Write(reader.ReadByte());
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            writer.Write(reader.ReadByte());
//          }
//          if (num21 != (ushort) 0)
//          {
//            writer.Write(reader.ReadBytesUntil((long) position9));
//            writer.Flush();
//            position9 = (uint) writer.BaseStream.Position;
//            for (ushort index10 = 0; (int) index10 < (int) num21; ++index10)
//            {
//              ushort num23;
//              ushort num24;
//              if (num1 == (byte) 1)
//              {
//                num23 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                num24 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//              }
//              else
//              {
//                num24 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//                num23 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//              }
//              writer.Write(num23);
//              writer.Write(num24);
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//              switch ((int) num24 >> 3 & 7)
//              {
//                case 0:
//                  for (ushort index11 = 0; (int) index11 < (int) num23; ++index11)
//                  {
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                  }
//                  break;
//                case 1:
//                  for (ushort index12 = 0; (int) index12 < (int) num23; ++index12)
//                  {
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(reader.ReadByte());
//                    writer.Write(reader.ReadByte());
//                  }
//                  break;
//                case 3:
//                  for (ushort index13 = 0; (int) index13 < (int) num23; ++index13)
//                  {
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//                  }
//                  break;
//                case 4:
//                  for (ushort index14 = 0; (int) index14 < (int) num23; ++index14)
//                  {
//                    writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//                    writer.Write(reader.ReadByte());
//                  }
//                  break;
//              }
//            }
//          }
//        }
//        uint[] numArray = new uint[(int) num2 + (int) num3];
//        writer.Write(reader.ReadBytesUntil((long) position7));
//        writer.Flush();
//        uint position14 = (uint) writer.BaseStream.Position;
//        ushort num25 = 0;
//        for (int index = 0; index < numArray.Length; ++index)
//        {
//          writer.Flush();
//          numArray[index] = (uint) writer.BaseStream.Position;
//          if (reader.PeekChar() != 0)
//          {
//            while (reader.PeekChar() != 0)
//            {
//              writer.Write(reader.ReadByte());
//              ++num25;
//            }
//            writer.Write(reader.ReadByte());
//            ++num25;
//          }
//          else
//          {
//            string str = string.Format("Nameless Cue #{0}", (object) index);
//            num25 += (ushort) str.Length;
//            writer.Write(str.ToCharArray());
//            writer.Write(reader.ReadByte());
//          }
//        }
//        writer.Flush();
//        long position15 = writer.BaseStream.Position;
//        writer.BaseStream.Seek(position1, SeekOrigin.Begin);
//        writer.Write(num25);
//        writer.Flush();
//        writer.BaseStream.Seek((long) offset, SeekOrigin.Begin);
//        for (int index = 0; index < numArray.Length; ++index)
//          writer.Write(numArray[index]);
//        writer.Flush();
//        writer.BaseStream.Seek(position2, SeekOrigin.Begin);
//        writer.Write(position3);
//        writer.Flush();
//        writer.BaseStream.Seek(position4, SeekOrigin.Begin);
//        writer.Write(position5);
//        writer.Flush();
//        writer.BaseStream.Seek(position6, SeekOrigin.Begin);
//        writer.Write(position14);
//        writer.Flush();
//        writer.BaseStream.Seek(position8, SeekOrigin.Begin);
//        writer.Write(position9);
//        writer.Flush();
//        writer.BaseStream.Seek(position10, SeekOrigin.Begin);
//        writer.Write(num6);
//        writer.Flush();
//        writer.BaseStream.Seek(position11, SeekOrigin.Begin);
//        writer.Write(offset);
//        writer.Flush();
//        writer.BaseStream.Seek(position15, SeekOrigin.Begin);
//        reader.BaseStream.CopyTo(writer.BaseStream);
//      }
//    }

//    public static void UpdateXACTSettings(string path, BinaryReader reader, BinaryWriter writer)
//    {
//      ContentHelper.Log(string.Format("[UpdateXACTSettings] Updating XACT global settings {0}", (object) path));
//      bool swap = reader.ReadUInt32() == 1481069382U;
//      writer.Write(1179862872U);
//      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//      writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt64()));
//      writer.Write(reader.ReadByte());
//      ushort num1 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//      writer.Write(num1);
//      if (!swap)
//      {
//        writer.Write(reader.ReadBytes(12));
//        uint position = reader.ReadUInt32();
//        writer.Write(position);
//        writer.Write(reader.ReadBytesUntil((long) position));
//        for (int index = 0; index < (int) num1; ++index)
//        {
//          writer.Write(reader.ReadBytes(5));
//          byte num2 = reader.ReadByte();
//          ContentHelper.CrossfadeType crossfadeType = (ContentHelper.CrossfadeType) ((uint) num2 & 7U);
//          if (crossfadeType != ContentHelper.CrossfadeType.Linear)
//            ContentHelper.Log(string.Format("[UpdateXACTSettings] Category #{0} uses unsupported crossfade type {1} ({2}) - replacing with Linear", (object) (index + 1), (object) Enum.GetName(typeof (ContentHelper.CrossfadeType), (object) crossfadeType), (object) (byte) crossfadeType));
//          writer.Write((byte) ((int) num2 & -8 | 0));
//          writer.Write(reader.ReadBytes(4));
//        }
//      }
//      else
//      {
//        ushort num3 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num3);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//        ushort num4 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num4);
//        ushort num5 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num5);
//        ushort num6 = ContentHelper.SwapEndian(swap, reader.ReadUInt16());
//        writer.Write(num6);
//        uint position1 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position1);
//        uint position2 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position2);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        uint position3 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position3);
//        uint position4 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position4);
//        uint position5 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(position5);
//        writer.Write(reader.ReadBytesUntil((long) position1));
//        for (int index = 0; index < (int) num1; ++index)
//        {
//          writer.Write(reader.ReadByte());
//          writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//          writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//          byte num7 = reader.ReadByte();
//          ContentHelper.CrossfadeType crossfadeType = (ContentHelper.CrossfadeType) ((uint) num7 & 7U);
//          if (crossfadeType != ContentHelper.CrossfadeType.Linear)
//            ContentHelper.Log(string.Format("[UpdateXACTSettings] Category #{0} uses unsupported crossfade type {1} ({2}) - replacing with Linear", (object) (index + 1), (object) Enum.GetName(typeof (ContentHelper.CrossfadeType), (object) crossfadeType), (object) (byte) crossfadeType));
//          writer.Write((byte) ((int) num7 & -8 | 0));
//          writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//          writer.Write(reader.ReadBytes(2));
//        }
//        if (position2 != uint.MaxValue)
//        {
//          writer.Write(reader.ReadBytesUntil((long) position2));
//          for (int index = 0; index < (int) num3; ++index)
//          {
//            writer.Write(reader.ReadByte());
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//          }
//        }
//        if (position3 != uint.MaxValue)
//        {
//          writer.Write(reader.ReadBytesUntil((long) position3));
//          for (int index1 = 0; index1 < (int) num4; ++index1)
//          {
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            byte num8 = reader.ReadByte();
//            writer.Write(num8);
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            for (int index2 = 0; index2 < (int) num8; ++index2)
//            {
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//              writer.Write(reader.ReadByte());
//            }
//          }
//        }
//        if (position4 < position5)
//        {
//          writer.Write(reader.ReadBytesUntil((long) position4));
//          for (int index = 0; index < (int) num5; ++index)
//          {
//            writer.Write(reader.ReadByte());
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//          }
//        }
//        if (position5 != uint.MaxValue)
//        {
//          writer.Write(reader.ReadBytesUntil((long) position5));
//          for (int index = 0; index < (int) num6; ++index)
//          {
//            writer.Write(reader.ReadByte());
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt16()));
//          }
//        }
//        if (position4 > position5 && position4 != uint.MaxValue)
//        {
//          writer.Write(reader.ReadBytesUntil((long) position4));
//          for (int index = 0; index < (int) num5; ++index)
//          {
//            writer.Write(reader.ReadByte());
//            writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//          }
//        }
//      }
//      reader.BaseStream.CopyTo(writer.BaseStream);
//    }

//    public static bool IsFFMPEGAvailable
//    {
//      get
//      {
//        try
//        {
//          Process process = new Process();
//          process.StartInfo = new ProcessStartInfo()
//          {
//            FileName = (MonoMod.Utils.PlatformHelper.Current & Platform.Windows) == Platform.Windows ? "where" : "which",
//            Arguments = "ffmpeg",
//            CreateNoWindow = true,
//            UseShellExecute = false
//          };
//          process.Start();
//          process.WaitForExit();
//          return process.ExitCode == 0;
//        }
//        catch (Exception ex)
//        {
//          ContentHelper.Log("Could not determine if FFMPEG available: " + (object) ex);
//          return false;
//        }
//      }
//    }

//    public static void Log(string txt)
//    {
//      Console.Write("[XnaToFna] [ContentHelper] ");
//      Console.WriteLine(txt);
//    }

//    public static void UpdateContent(
//      string path,
//      bool patchXNB = true,
//      bool patchXACT = true,
//      bool patchWindowsMedia = true)
//    {
//      if (patchXNB && path.EndsWith(".xnb"))
//        ContentHelper.TransformContent(path);
//      else if (patchXACT && path.EndsWith(".xwb"))
//        ContentHelper.PatchContent(path, new Action<string, BinaryReader, BinaryWriter>(ContentHelper.UpdateWaveBank));
//      else if (patchXACT && path.EndsWith(".xsb"))
//        ContentHelper.PatchContent(path, new Action<string, BinaryReader, BinaryWriter>(ContentHelper.UpdateSoundBank));
//      else if (patchXACT && path.EndsWith(".xgs"))
//        ContentHelper.PatchContent(path, new Action<string, BinaryReader, BinaryWriter>(ContentHelper.UpdateXACTSettings));
//      else if (patchWindowsMedia && path.EndsWith(".wmv"))
//      {
//        ContentHelper.UpdateVideo(path);
//      }
//      else
//      {
//        if (!patchWindowsMedia || !path.EndsWith(".wma"))
//          return;
//        ContentHelper.UpdateAudio(path);
//      }
//    }

//    public static void PatchContent(
//      string path,
//      Action<string, BinaryReader, BinaryWriter> patcher,
//      bool writeToTmp = true,
//      string pathOutput = null)
//    {
//      pathOutput = pathOutput ?? path;
//      if (writeToTmp)
//        File.Delete(path + ".tmp");
//      if (pathOutput != path)
//        File.Delete(pathOutput);
//      using (Stream input = (Stream) File.OpenRead(path))
//      {
//        using (BinaryReader binaryReader = new BinaryReader(input))
//        {
//          if (writeToTmp)
//          {
//            using (Stream output = (Stream) File.OpenWrite(path + ".tmp"))
//            {
//              using (BinaryWriter binaryWriter = new BinaryWriter(output))
//                patcher(path, binaryReader, binaryWriter);
//            }
//          }
//          else
//            patcher(path, binaryReader, (BinaryWriter) null);
//        }
//      }
//      if (!writeToTmp)
//        return;
//      if (pathOutput == path)
//        File.Delete(path);
//      File.Move(path + ".tmp", pathOutput);
//    }

//    public static byte[] SwapEndian(bool swap, byte[] data)
//    {
//      if (!swap)
//        return data;
//      for (int index1 = data.Length / 2; index1 > -1; --index1)
//      {
//        int index2 = data.Length - 1 - index1;
//        byte num = data[index1];
//        data[index1] = data[index2];
//        data[index2] = num;
//      }
//      return data;
//    }

//    public static ushort SwapEndian(bool swap, ushort data) => !swap ? data : (ushort) ((uint) (ushort) (((int) data & (int) byte.MaxValue) << 8) | (uint) (ushort) ((int) data >> 8 & (int) byte.MaxValue));

//    public static uint SwapEndian(bool swap, uint data) => !swap ? data : (uint) (((int) data & (int) byte.MaxValue) << 24 | ((int) (data >> 8) & (int) byte.MaxValue) << 16 | ((int) (data >> 16) & (int) byte.MaxValue) << 8 | (int) (data >> 24) & (int) byte.MaxValue);

//    public static ulong SwapEndian(bool swap, ulong data) => !swap ? data : (ulong) (((long) data & (long) byte.MaxValue) << 56 | ((long) (data >> 8) & (long) byte.MaxValue) << 48 | ((long) (data >> 16) & (long) byte.MaxValue) << 40 | ((long) (data >> 24) & (long) byte.MaxValue) << 32 | ((long) (data >> 32) & (long) byte.MaxValue) << 24 | ((long) (data >> 40) & (long) byte.MaxValue) << 16 | ((long) (data >> 48) & (long) byte.MaxValue) << 8 | (long) (data >> 56) & (long) byte.MaxValue);

//    public static void RunFFMPEG(
//      string args,
//      Stream input,
//      Stream output,
//      Action<Process> feeder = null,
//      long inputLength = 0)
//    {
//      Process ffmpeg = new Process();
//      ffmpeg.StartInfo = new ProcessStartInfo()
//      {
//        FileName = "ffmpeg",
//        Arguments = args,
//        UseShellExecute = false,
//        RedirectStandardOutput = true,
//        RedirectStandardInput = true,
//        RedirectStandardError = true
//      };
//      ffmpeg.Start();
//      ffmpeg.AsyncPipeErr();
//      Thread thread;
//      if (input != null)
//        thread = new Thread(feeder != null ? (ThreadStart) (() => feeder(ffmpeg)) : (inputLength == 0L ? (ThreadStart) (() =>
//        {
//          input.CopyTo(ffmpeg.StandardInput.BaseStream);
//          ffmpeg.StandardInput.BaseStream.Flush();
//          ffmpeg.StandardInput.BaseStream.Close();
//        }) : (ThreadStart) (() =>
//        {
//          byte[] buffer = new byte[4096];
//          Stream baseStream = ffmpeg.StandardInput.BaseStream;
//          long num = 0;
//          while (!ffmpeg.HasExited && num < inputLength)
//          {
//            int count;
//            num += (long) (count = input.Read(buffer, 0, Math.Min(buffer.Length, (int) (inputLength - num))));
//            baseStream.Write(buffer, 0, count);
//            baseStream.Flush();
//          }
//          baseStream.Close();
//        })))
//        {
//          IsBackground = true
//        };
//      else
//        thread = (Thread) null;
//      thread?.Start();
//      if (output == null)
//      {
//        ffmpeg.AsyncPipeOut();
//        ffmpeg.WaitForExit();
//      }
//      else
//      {
//        Stream baseStream = ffmpeg.StandardOutput.BaseStream;
//        byte[] buffer = new byte[1024];
//        int count;
//        while ((count = baseStream.Read(buffer, 0, buffer.Length)) > 0)
//          output.Write(buffer, 0, count);
//      }
//    }

//    public static void UpdateWaveBank(string path, BinaryReader reader, BinaryWriter writer)
//    {
//      if (!ContentHelper.IsFFMPEGAvailable)
//      {
//        ContentHelper.Log("[UpdateWaveBank] FFMPEG is missing - won't convert unsupported WaveBanks");
//        reader.BaseStream.CopyTo(writer.BaseStream);
//      }
//      else
//      {
//        ContentHelper.Log(string.Format("[UpdateWaveBank] Updating wave bank {0}", (object) path));
//        bool swap = reader.ReadUInt32() == 1463963204U;
//        writer.Write(1145979479U);
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//        uint[] numArray1 = new uint[5];
//        uint[] numArray2 = new uint[5];
//        long position1 = reader.BaseStream.Position;
//        for (int index = 0; index < 5; ++index)
//        {
//          numArray1[index] = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//          writer.Write(numArray1[index]);
//          numArray2[index] = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//          writer.Write(numArray2[index]);
//        }
//        writer.Write(reader.ReadBytesUntil((long) numArray1[0]));
//        uint num1 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//        writer.Write(num1);
//        if (((int) num1 & 2) == 2)
//        {
//          if (swap)
//            throw new InvalidDataException("Can't handle compact mode Xbox 360 wave banks - Content directory left in unstable state");
//          reader.BaseStream.CopyTo(writer.BaseStream);
//        }
//        else
//        {
//          uint length1 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//          writer.Write(length1);
//          writer.Write(reader.ReadBytes(64));
//          uint num2 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//          writer.Write(num2);
//          writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//          writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//          uint num3 = numArray1[4];
//          if (num3 == 0U)
//            num3 = numArray1[1] + length1 * num2;
//          uint[] numArray3 = new uint[(int) length1];
//          long[] numArray4 = new long[(int) length1];
//          uint[] numArray5 = new uint[(int) length1];
//          uint[] numArray6 = new uint[(int) length1];
//          long[] numArray7 = new long[(int) length1];
//          int[] numArray8 = new int[(int) length1];
//          int[] numArray9 = new int[(int) length1];
//          long[] numArray10 = new long[(int) length1];
//          uint[] numArray11 = new uint[(int) length1];
//          uint[] numArray12 = new uint[(int) length1];
//          uint[] numArray13 = new uint[(int) length1];
//          uint[] numArray14 = new uint[(int) length1];
//          uint[] numArray15 = new uint[(int) length1];
//          uint position2 = numArray1[1];
//          uint num4 = 0;
//          for (int index = 0; (long) index < (long) length1; ++index)
//          {
//            writer.Write(reader.ReadBytesUntil((long) position2));
//            if (num2 >= 4U)
//            {
//              uint num5 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//              writer.Write(num5);
//              numArray3[index] = num5 >> 4;
//            }
//            if (num2 >= 8U)
//            {
//              numArray10[index] = reader.BaseStream.Position;
//              writer.Write(num4 = ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            }
//            if (num2 >= 12U)
//            {
//              numArray4[index] = reader.BaseStream.Position;
//              writer.Write(numArray5[index] = numArray6[index] = ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            }
//            if (num2 >= 16U)
//            {
//              numArray7[index] = reader.BaseStream.Position;
//              writer.Write((uint) (numArray8[index] = numArray9[index] = (int) ContentHelper.SwapEndian(swap, reader.ReadUInt32())));
//            }
//            if (num2 >= 20U)
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            if (num2 >= 24U)
//              writer.Write(ContentHelper.SwapEndian(swap, reader.ReadUInt32()));
//            else if (numArray8[index] != 0)
//              numArray8[index] = (int) numArray2[4];
//            position2 += num2;
//            numArray5[index] += num3;
//            numArray11[index] = num4 & 3U;
//            numArray12[index] = num4 >> 2 & 7U;
//            numArray13[index] = num4 >> 5 & 262143U;
//            numArray14[index] = num4 >> 23 & (uint) byte.MaxValue;
//            numArray15[index] = num4 >> 31;
//          }
//          uint[][] numArray16 = new uint[(int) length1][];
//          if (((int) num1 & 524288) == 524288)
//          {
//            writer.Write(reader.ReadBytesUntil((long) numArray1[2]));
//            uint[] numArray17 = new uint[(int) length1];
//            for (int index = 0; (long) index < (long) length1; ++index)
//            {
//              numArray17[index] = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//              writer.Write(numArray17[index]);
//            }
//            writer.Flush();
//            uint position3 = (uint) writer.BaseStream.Position;
//            for (int index1 = 0; (long) index1 < (long) length1; ++index1)
//            {
//              writer.Write(reader.ReadBytesUntil((long) (position3 + numArray17[index1])));
//              uint length2 = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//              writer.Write(length2);
//              uint[] numArray18 = numArray16[index1] = new uint[(int) length2];
//              for (int index2 = 0; (long) index2 < (long) length2; ++index2)
//              {
//                numArray18[index2] = ContentHelper.SwapEndian(swap, reader.ReadUInt32());
//                writer.Write(numArray18[index2]);
//              }
//            }
//          }
//          for (int index3 = 0; (long) index3 < (long) length1; ++index3)
//          {
//            writer.Write(reader.ReadBytesUntil((long) numArray5[index3]));
//            if (numArray11[index3] != 1U && numArray11[index3] != 3U)
//            {
//              writer.Write(reader.ReadBytes(numArray8[index3]));
//            }
//            else
//            {
//              writer.Flush();
//              uint position4 = (uint) writer.BaseStream.Position;
//              Action<Process> feeder = (Action<Process>) null;
//              if (numArray11[index3] == 3U)
//                feeder = ContentHelper.GenerateXWMAFeeder(reader, numArray14[index3], numArray8[index3], numArray3[index3], numArray12[index3], numArray13[index3]);
//              else if (numArray11[index3] == 1U)
//                feeder = ContentHelper.GenerateXMA2Feeder(reader, numArray14[index3], numArray8[index3], numArray3[index3], numArray12[index3], numArray13[index3], numArray16[index3]);
//              ContentHelper.Log(string.Format("[UpdateWaveBank] Converting #{0}", (object) index3));
//              ContentHelper.ConvertAudio(reader.BaseStream, writer.BaseStream, feeder, (long) numArray8[index3]);
//              numArray12[index3] = 1U;
//              writer.Flush();
//              uint num6 = (uint) writer.BaseStream.Position - position4;
//              uint position5 = (uint) writer.BaseStream.Position;
//              uint num7 = num6 - (uint) numArray8[index3];
//              numArray11[index3] = 0U;
//              numArray15[index3] = 0U;
//              numArray14[index3] = 0U;
//              if (numArray10[index3] != 0L)
//              {
//                writer.Flush();
//                writer.BaseStream.Seek(numArray10[index3], SeekOrigin.Begin);
//                writer.Write((uint) ((int) numArray11[index3] & 3 | ((int) numArray12[index3] & 7) << 2 | ((int) numArray13[index3] & 262143) << 5 | ((int) numArray14[index3] & (int) byte.MaxValue) << 23 | (int) numArray15[index3] << 31));
//              }
//              if (numArray7[index3] != 0L)
//              {
//                writer.Flush();
//                writer.BaseStream.Seek(numArray7[index3], SeekOrigin.Begin);
//                writer.Write(numArray9[index3] = (int) num6);
//              }
//              for (int index4 = index3 + 1; (long) index4 < (long) length1; ++index4)
//              {
//                if (numArray4[index4] != 0L)
//                {
//                  writer.Flush();
//                  writer.BaseStream.Seek(numArray4[index4], SeekOrigin.Begin);
//                  writer.Write(numArray6[index4] += num7);
//                }
//              }
//              writer.Flush();
//              writer.BaseStream.Seek((long) position5, SeekOrigin.Begin);
//            }
//          }
//          writer.Flush();
//          uint position6 = (uint) writer.BaseStream.Position;
//          numArray2[4] = position6 - numArray1[4];
//          writer.Flush();
//          writer.BaseStream.Seek(position1, SeekOrigin.Begin);
//          for (int index = 0; index < 5; ++index)
//          {
//            writer.Write(numArray1[index]);
//            writer.Write(numArray2[index]);
//          }
//          writer.Flush();
//          writer.BaseStream.Seek((long) position6, SeekOrigin.Begin);
//          reader.BaseStream.CopyTo(writer.BaseStream);
//        }
//      }
//    }

//    public static void ConvertAudio(
//      Stream input,
//      Stream output,
//      Action<Process> feeder,
//      long length)
//    {
//      ContentHelper.RunFFMPEG("-y -i - -f u8 -ac 1 -", input, output, feeder, length);
//    }

//    public static Action<Process> GenerateSoundEffectFeeder(
//      BinaryReader reader,
//      string format,
//      uint fmtLength,
//      uint dataLength,
//      uint extraLength,
//      bool x360 = false,
//      Action<BinaryWriter> fmtExtraWriter = null)
//    {
//      return (Action<Process>) (ffmpeg =>
//      {
//        Stream baseStream = ffmpeg.StandardInput.BaseStream;
//        using (BinaryWriter binaryWriter = new BinaryWriter(baseStream, Encoding.ASCII, true))
//        {
//          binaryWriter.Write("RIFF".ToCharArray());
//          binaryWriter.Write((uint) ((int) dataLength + 4 + 4 + 8 + 4 + (int) fmtLength + (int) extraLength + 4 + 4 - 8));
//          binaryWriter.Write(format.ToCharArray());
//          binaryWriter.Write("fmt ".ToCharArray());
//          int num1 = (int) reader.ReadUInt32();
//          binaryWriter.Write(fmtLength);
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, reader.ReadUInt16()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, reader.ReadUInt16()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, reader.ReadUInt32()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, reader.ReadUInt32()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, reader.ReadUInt16()));
//          binaryWriter.Write(ContentHelper.SwapEndian(x360, reader.ReadUInt16()));
//          if (fmtExtraWriter == null)
//          {
//            ushort count = ContentHelper.SwapEndian(x360, reader.ReadUInt16());
//            binaryWriter.Write(count);
//            binaryWriter.Write(reader.ReadBytes((int) count));
//          }
//          else
//          {
//            Action<BinaryWriter> action = fmtExtraWriter;
//            if (action != null)
//              action(binaryWriter);
//          }
//          binaryWriter.Write("data".ToCharArray());
//          int num2 = (int) reader.ReadUInt32();
//          binaryWriter.Write(dataLength);
//          binaryWriter.Flush();
//        }
//        byte[] buffer = new byte[4096];
//        long num = reader.BaseStream.Position + (long) dataLength;
//        while (!ffmpeg.HasExited && reader.BaseStream.Position < num)
//        {
//          int count = reader.BaseStream.Read(buffer, 0, Math.Min(buffer.Length, (int) (num - reader.BaseStream.Position)));
//          baseStream.Write(buffer, 0, count);
//          baseStream.Flush();
//        }
//        baseStream.Close();
//      });
//    }

//    public static Action<Process> GenerateXWMAFeeder(
//      BinaryReader reader,
//      uint align,
//      int playLength,
//      uint duration,
//      uint channels,
//      uint rate)
//    {
//      return (Action<Process>) (ffmpeg =>
//      {
//        Stream baseStream = ffmpeg.StandardInput.BaseStream;
//        using (BinaryWriter binaryWriter = new BinaryWriter(baseStream, Encoding.ASCII, true))
//        {
//          short num1 = (long) align >= (long) ContentHelper.XWMAInfo.BlockAlign.Length ? ContentHelper.XWMAInfo.BlockAlign[(int) align & 15] : ContentHelper.XWMAInfo.BlockAlign[(int) align];
//          int num2 = playLength / (int) num1;
//          int num3 = (int) Math.Ceiling((double) duration / 2048);
//          int num4 = num3 / num2;
//          int num5 = num3 - num4 * num2;
//          binaryWriter.Write("RIFF".ToCharArray());
//          binaryWriter.Write(playLength + 4 + 4 + 8 + 4 + 2 + 2 + 4 + 4 + 2 + 2 + 2 + 4 + 4 + num2 * 4 + 4 + 4 - 8);
//          binaryWriter.Write("XWMAfmt ".ToCharArray());
//          binaryWriter.Write(18);
//          binaryWriter.Write((short) 353);
//          binaryWriter.Write((short) channels);
//          binaryWriter.Write(rate);
//          binaryWriter.Write((long) align >= (long) ContentHelper.XWMAInfo.BytesPerSecond.Length ? ContentHelper.XWMAInfo.BytesPerSecond[(int) (align >> 5)] : ContentHelper.XWMAInfo.BytesPerSecond[(int) align]);
//          binaryWriter.Write(num1);
//          binaryWriter.Write((short) 15);
//          binaryWriter.Write((short) 0);
//          binaryWriter.Write("dpds".ToCharArray());
//          binaryWriter.Write(num2 * 4);
//          int num6 = 0;
//          int num7 = 0;
//          for (; num6 < num2; ++num6)
//          {
//            num7 += num4 * 4096;
//            if (num5 > 0)
//            {
//              num7 += 4096;
//              --num5;
//            }
//            binaryWriter.Write(num7);
//          }
//          binaryWriter.Write("data".ToCharArray());
//          binaryWriter.Write(playLength);
//          binaryWriter.Flush();
//        }
//        byte[] buffer = new byte[4096];
//        long num = reader.BaseStream.Position + (long) playLength;
//        while (!ffmpeg.HasExited && reader.BaseStream.Position < num)
//        {
//          int count = reader.BaseStream.Read(buffer, 0, Math.Min(buffer.Length, (int) (num - reader.BaseStream.Position)));
//          baseStream.Write(buffer, 0, count);
//          baseStream.Flush();
//        }
//        baseStream.Close();
//      });
//    }

//    public static Action<Process> GenerateXMA2Feeder(
//      BinaryReader reader,
//      uint align,
//      int playLength,
//      uint duration,
//      uint channels,
//      uint rate,
//      uint[] seekData)
//    {
//      return (Action<Process>) (ffmpeg =>
//      {
//        Stream baseStream = ffmpeg.StandardInput.BaseStream;
//        using (BinaryWriter binaryWriter = new BinaryWriter(baseStream, Encoding.ASCII, true))
//        {
//          binaryWriter.Write("RIFF".ToCharArray());
//          binaryWriter.Write(playLength + 4 + 4 + 8 + 4 + 2 + 2 + 4 + 4 + 2 + 2 + 2 + 2 + 4 + 24 + 1 + 1 + 2 + 4 + 4 + seekData.Length * 4 + 4 + 4 - 8);
//          binaryWriter.Write("WAVEfmt ".ToCharArray());
//          binaryWriter.Write(52);
//          binaryWriter.Write((short) 358);
//          binaryWriter.Write((short) channels);
//          binaryWriter.Write(rate);
//          binaryWriter.Write((long) align >= (long) ContentHelper.XMAInfo.BytesPerSecond.Length ? ContentHelper.XMAInfo.BytesPerSecond[(int) (align >> 5)] : ContentHelper.XMAInfo.BytesPerSecond[(int) align]);
//          binaryWriter.Write((long) align >= (long) ContentHelper.XMAInfo.BlockAlign.Length ? ContentHelper.XMAInfo.BlockAlign[(int) align & 15] : ContentHelper.XMAInfo.BlockAlign[(int) align]);
//          binaryWriter.Write((short) 15);
//          binaryWriter.Write((short) 34);
//          binaryWriter.Write((short) 1);
//          binaryWriter.Write(channels == 2U ? 3U : 0U);
//          binaryWriter.Write(0U);
//          binaryWriter.Write(0U);
//          binaryWriter.Write(0U);
//          binaryWriter.Write(0U);
//          binaryWriter.Write(0U);
//          binaryWriter.Write(0U);
//          binaryWriter.Write((byte) 0);
//          binaryWriter.Write((byte) 4);
//          binaryWriter.Write((short) 1);
//          binaryWriter.Write("seek".ToCharArray());
//          binaryWriter.Write(seekData.Length * 4);
//          for (int index = 0; index < seekData.Length; ++index)
//            binaryWriter.Write(seekData[index]);
//          binaryWriter.Write("data".ToCharArray());
//          binaryWriter.Write(playLength);
//          binaryWriter.Flush();
//        }
//        byte[] buffer = new byte[4096];
//        long num = reader.BaseStream.Position + (long) playLength;
//        while (!ffmpeg.HasExited && reader.BaseStream.Position < num)
//        {
//          int count = reader.BaseStream.Read(buffer, 0, Math.Min(buffer.Length, (int) (num - reader.BaseStream.Position)));
//          baseStream.Write(buffer, 0, count);
//          baseStream.Flush();
//        }
//        baseStream.Close();
//      });
//    }

//    public enum SoundBankEventType : uint
//    {
//      Stop = 0,
//      PlayWave = 1,
//      PlayWaveTrackVariation = 3,
//      PlayWaveEffectVariation = 4,
//      PlayWaveTrackEffectVariation = 6,
//      Pitch = 7,
//      Volume = 8,
//      Marker = 9,
//      PitchRepeating = 16, // 0x00000010
//      VolumeRepeating = 17, // 0x00000011
//      MarkerRepeating = 18, // 0x00000012
//    }

//    public enum CrossfadeType : byte
//    {
//      Linear,
//      Logarithmic,
//      EqualPower,
//    }

//    public static class XWMAInfo
//    {
//      public static readonly int[] BytesPerSecond = new int[6]
//      {
//        12000,
//        24000,
//        4000,
//        6000,
//        8000,
//        20000
//      };
//      public static readonly short[] BlockAlign = new short[16]
//      {
//        (short) 929,
//        (short) 1487,
//        (short) 1280,
//        (short) 2230,
//        (short) 8917,
//        (short) 8192,
//        (short) 4459,
//        (short) 5945,
//        (short) 2304,
//        (short) 1536,
//        (short) 1485,
//        (short) 1008,
//        (short) 2731,
//        (short) 4096,
//        (short) 6827,
//        (short) 5462
//      };
//    }

//    public static class XMAInfo
//    {
//      public static readonly int[] BytesPerSecond = new int[6]
//      {
//        12000,
//        24000,
//        4000,
//        6000,
//        8000,
//        20000
//      };
//      public static readonly short[] BlockAlign = new short[16]
//      {
//        (short) 929,
//        (short) 1487,
//        (short) 1280,
//        (short) 2230,
//        (short) 8917,
//        (short) 8192,
//        (short) 4459,
//        (short) 5945,
//        (short) 2304,
//        (short) 1536,
//        (short) 1485,
//        (short) 1008,
//        (short) 2731,
//        (short) 4096,
//        (short) 6827,
//        (short) 5462
//      };
//    }
//  }
//}
