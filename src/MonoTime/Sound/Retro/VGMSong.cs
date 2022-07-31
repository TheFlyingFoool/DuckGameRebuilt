// Decompiled with JetBrains decompiler
// Type: DuckGame.VGMSong
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace DuckGame
{
    public class VGMSong
    {
        private DynamicSoundEffectInstance _instance;
        private byte[] _buffer;
        private int[] _intBuffer;
        private YM2612 _chip = new YM2612();
        private SN76489 _psg = new SN76489();
        private bool _iSaidStop;
        private float _volume = 1f;
        private bool _looped = true;
        private float _playbackSpeed = 1f;
        private const uint FCC_VGM = 544040790;
        private uint _VGMDataLen;
        private VGM_HEADER _VGMHead;
        private BinaryReader _vgmReader;
        private byte[] _DACData;
        private byte[] _VGMData;
        private int _DACOffset;
        private int _VGMDataOffset;
        private byte _lastCommand;
        private int _wait;
        private float _waitInc;

        public SoundState state => !this._iSaidStop ? this._instance.State : SoundState.Stopped;

        public float volume
        {
            get => this._volume;
            set
            {
                this._volume = MathHelper.Clamp(value, 0f, 1f);
                if (this._instance == null || this._instance.State != SoundState.Playing)
                    return;
                this._instance.Volume = this._volume;
            }
        }

        public bool looped
        {
            get => this._looped;
            set => this._looped = value;
        }

        public bool gameFroze { get; set; }

        public float playbackSpeed
        {
            get => this._playbackSpeed;
            set => this._playbackSpeed = value;
        }

        public VGMSong(string file)
        {
            this._instance = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
            this._buffer = new byte[this._instance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(150.0))];
            this._intBuffer = new int[this._buffer.Length / 2];
            this._instance.BufferNeeded += new EventHandler<EventArgs>(this.StreamVGM);
            this.OpenVGMFile(file);
            this._chip.Initialize((int)this._VGMHead.lngHzYM2612, 44100);
            this._psg.Initialize(_VGMHead.lngHzPSG);
        }

        public void Terminate() => this._instance.Dispose();

        public void Play()
        {
            this._instance.Stop();
            this._instance.Play();
            this._instance.Volume = this._volume;
            this._iSaidStop = false;
        }

        public void Pause()
        {
            this._instance.Pause();
            this._iSaidStop = true;
        }

        public void Resume()
        {
            this._instance.Resume();
            this._instance.Volume = this._volume;
            this._iSaidStop = false;
        }

        public void Stop()
        {
            this._instance.Stop();
            this._instance.Volume = 0f;
            this._iSaidStop = true;
            this._vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
        }

        private static VGM_HEADER ReadVGMHeader(BinaryReader hFile)
        {
            VGM_HEADER vgmHeader = new VGM_HEADER();
            foreach (FieldInfo field in typeof(VGM_HEADER).GetFields())
            {
                if (field.FieldType == typeof(uint))
                {
                    uint num = hFile.ReadUInt32();
                    field.SetValue(vgmHeader, num);
                }
                else if (field.FieldType == typeof(ushort))
                {
                    ushort num = hFile.ReadUInt16();
                    field.SetValue(vgmHeader, num);
                }
                else if (field.FieldType == typeof(char))
                {
                    char ch = hFile.ReadChar();
                    field.SetValue(vgmHeader, ch);
                }
                else if (field.FieldType == typeof(byte))
                {
                    byte num = hFile.ReadByte();
                    field.SetValue(vgmHeader, num);
                }
            }
            if (vgmHeader.lngVersion < 257U)
                vgmHeader.lngRate = 0U;
            if (vgmHeader.lngVersion < 272U)
            {
                vgmHeader.shtPSG_Feedback = 0;
                vgmHeader.bytPSG_SRWidth = 0;
                vgmHeader.lngHzYM2612 = vgmHeader.lngHzYM2413;
                vgmHeader.lngHzYM2151 = vgmHeader.lngHzYM2413;
            }
            if (vgmHeader.lngHzPSG != 0U)
            {
                if (vgmHeader.shtPSG_Feedback == 0)
                    vgmHeader.shtPSG_Feedback = 9;
                if (vgmHeader.bytPSG_SRWidth == 0)
                    vgmHeader.bytPSG_SRWidth = 16;
            }
            return vgmHeader;
        }

        private bool OpenVGMFile(string fileName)
        {
            bool flag = fileName.Contains(".vgz");
            FileStream input = System.IO.File.Open(fileName, FileMode.Open);
            uint num1;
            if (flag)
            {
                input.Position = input.Length - 4L;
                byte[] buffer = new byte[4];
                input.Read(buffer, 0, 4);
                num1 = BitConverter.ToUInt32(buffer, 0);
                input.Position = 0L;
                this._vgmReader = new BinaryReader(new GZipStream(input, CompressionMode.Decompress));
            }
            else
            {
                num1 = (uint)input.Length;
                this._vgmReader = new BinaryReader(input);
            }
            if (this._vgmReader.ReadUInt32() != 544040790U)
                return false;
            this._VGMDataLen = num1;
            this._VGMHead = VGMSong.ReadVGMHeader(this._vgmReader);
            if (flag)
            {
                this._vgmReader.Close();
                input = System.IO.File.Open(fileName, FileMode.Open);
                this._vgmReader = new BinaryReader(new GZipStream(input, CompressionMode.Decompress));
            }
            else
                this._vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
            int count = (int)this._VGMHead.lngDataOffset;
            switch (count)
            {
                case 0:
                case 12:
                    count = 64;
                    break;
            }
            this._VGMDataOffset = count;
            this._vgmReader.ReadBytes(count);
            this._VGMData = this._vgmReader.ReadBytes((int)(num1 - count));
            this._vgmReader = new BinaryReader(new MemoryStream(this._VGMData));
            if ((byte)this._vgmReader.PeekChar() == 103)
            {
                int num2 = this._vgmReader.ReadByte();
                if ((byte)this._vgmReader.PeekChar() == 102)
                {
                    int num3 = this._vgmReader.ReadByte();
                    int num4 = this._vgmReader.ReadByte();
                    this._DACData = this._vgmReader.ReadBytes((int)this._vgmReader.ReadUInt32());
                }
            }
            input.Close();
            return true;
        }

        private void StreamVGM(object sender, EventArgs e)
        {
            if (this._iSaidStop)
                return;
            if (this._lastCommand == 102 && !this._looped)
            {
                this._lastCommand = 0;
                this._instance.Volume = 0f;
                this._iSaidStop = true;
                this.Stop();
            }
            else
            {
                int[] buffer = new int[2];
                int num1 = 0;
                int num2 = this._intBuffer.Length / 2;
                bool flag1 = false;
                while (num1 != num2)
                {
                    bool flag2;
                    if (this._wait == 0 && !this.gameFroze)
                    {
                        flag2 = false;
                        byte num3 = this._vgmReader.ReadByte();
                        this._lastCommand = num3;
                        switch (num3)
                        {
                            case 79:
                                int num4 = this._vgmReader.ReadByte();
                                break;
                            case 80:
                                this._psg.Write(this._vgmReader.ReadByte());
                                break;
                            case 82:
                                this._chip.WritePort0(this._vgmReader.ReadByte(), this._vgmReader.ReadByte());
                                break;
                            case 83:
                                this._chip.WritePort1(this._vgmReader.ReadByte(), this._vgmReader.ReadByte());
                                break;
                            case 97:
                                this._wait = this._vgmReader.ReadUInt16();
                                if (this._wait != 0)
                                {
                                    flag2 = true;
                                    break;
                                }
                                break;
                            case 98:
                                this._wait = 735;
                                flag2 = true;
                                break;
                            case 99:
                                this._wait = 882;
                                flag2 = true;
                                break;
                            case 102:
                                if (!this._looped)
                                {
                                    this._vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
                                    flag1 = true;
                                    break;
                                }
                                if (this._VGMHead.lngLoopOffset != 0U)
                                {
                                    this._vgmReader.BaseStream.Seek(_VGMHead.lngLoopOffset - _VGMDataOffset, SeekOrigin.Begin);
                                    break;
                                }
                                this._vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
                                break;
                            case 103:
                                int num5 = this._vgmReader.ReadByte();
                                int num6 = this._vgmReader.ReadByte();
                                this._vgmReader.BaseStream.Position += this._vgmReader.ReadUInt32();
                                break;
                            case 224:
                                this._DACOffset = (int)this._vgmReader.ReadUInt32();
                                break;
                        }
                        if (num3 >= 112 && num3 <= 127)
                        {
                            this._wait = (num3 & 15) + 1;
                            if (this._wait != 0)
                                flag2 = true;
                        }
                        else if (num3 >= 128 && num3 <= 143)
                        {
                            this._wait = num3 & 15;
                            this._chip.WritePort0(42, this._DACData[this._DACOffset]);
                            ++this._DACOffset;
                            if (this._wait != 0)
                                flag2 = true;
                        }
                        if (this._wait != 0)
                            --this._wait;
                    }
                    else
                    {
                        flag2 = true;
                        if (this._wait > 0)
                        {
                            for (this._waitInc += this._playbackSpeed; this._wait > 0 && _waitInc >= 1.0; --this._wait)
                                --this._waitInc;
                        }
                    }
                    if (!flag1)
                    {
                        if (flag2)
                        {
                            this._chip.Update(buffer, 1);
                            short num7 = (short)buffer[0];
                            short num8 = (short)buffer[1];
                            this._psg.Update(buffer, 1);
                            short num9 = (short)buffer[0];
                            short num10 = (short)buffer[1];
                            this._intBuffer[num1 * 2] = Maths.Clamp((num7 + num9) * 2, short.MinValue, short.MaxValue);
                            this._intBuffer[num1 * 2 + 1] = Maths.Clamp((num8 + num10) * 2, short.MinValue, short.MaxValue);
                            ++num1;
                            if (num1 == num2)
                                break;
                        }
                    }
                    else
                        break;
                }
                for (int index = 0; index < this._intBuffer.Length; ++index)
                {
                    short num11 = (short)this._intBuffer[index];
                    this._buffer[index * 2] = (byte)((uint)num11 & byte.MaxValue);
                    this._buffer[index * 2 + 1] = (byte)(num11 >> 8 & byte.MaxValue);
                }
                int num12 = num1 * 2;
                if (num12 / 4.0 - (int)(num12 / 4.0) > 0.0)
                    num12 -= 2;
                this._instance.SubmitBuffer(this._buffer, 0, num12);
                this._instance.SubmitBuffer(this._buffer, num12, num12);
            }
        }
    }
}
