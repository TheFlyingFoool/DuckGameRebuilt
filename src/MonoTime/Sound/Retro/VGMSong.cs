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

        public SoundState state => !_iSaidStop ? _instance.State : SoundState.Stopped;

        public float volume
        {
            get => _volume;
            set
            {
                _volume = MathHelper.Clamp(value, 0f, 1f);
                if (_instance == null || _instance.State != SoundState.Playing)
                    return;
                _instance.Volume = _volume;
            }
        }

        public bool looped
        {
            get => _looped;
            set => _looped = value;
        }

        public bool gameFroze { get; set; }

        public float playbackSpeed
        {
            get => _playbackSpeed;
            set => _playbackSpeed = value;
        }

        public VGMSong(string file)
        {
            _instance = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
            _buffer = new byte[_instance.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(150.0))];
            _intBuffer = new int[_buffer.Length / 2];
            _instance.BufferNeeded += new EventHandler<EventArgs>(StreamVGM);
            OpenVGMFile(file);
            _chip.Initialize((int)_VGMHead.lngHzYM2612, 44100);
            _psg.Initialize(_VGMHead.lngHzPSG);
        }

        public void Terminate() => _instance.Dispose();

        public void Play()
        {
            _instance.Stop();
            _instance.Play();
            _instance.Volume = _volume;
            _iSaidStop = false;
        }

        public void Pause()
        {
            _instance.Pause();
            _iSaidStop = true;
        }

        public void Resume()
        {
            _instance.Resume();
            _instance.Volume = _volume;
            _iSaidStop = false;
        }

        public void Stop()
        {
            _instance.Stop();
            _instance.Volume = 0f;
            _iSaidStop = true;
            _vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
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
                _vgmReader = new BinaryReader(new GZipStream(input, CompressionMode.Decompress));
            }
            else
            {
                num1 = (uint)input.Length;
                _vgmReader = new BinaryReader(input);
            }
            if (_vgmReader.ReadUInt32() != 544040790U)
                return false;
            _VGMDataLen = num1;
            _VGMHead = VGMSong.ReadVGMHeader(_vgmReader);
            if (flag)
            {
                _vgmReader.Close();
                input = System.IO.File.Open(fileName, FileMode.Open);
                _vgmReader = new BinaryReader(new GZipStream(input, CompressionMode.Decompress));
            }
            else
                _vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
            int count = (int)_VGMHead.lngDataOffset;
            switch (count)
            {
                case 0:
                case 12:
                    count = 64;
                    break;
            }
            _VGMDataOffset = count;
            _vgmReader.ReadBytes(count);
            _VGMData = _vgmReader.ReadBytes((int)(num1 - count));
            _vgmReader = new BinaryReader(new MemoryStream(_VGMData));
            if ((byte)_vgmReader.PeekChar() == 103)
            {
                int num2 = _vgmReader.ReadByte();
                if ((byte)_vgmReader.PeekChar() == 102)
                {
                    int num3 = _vgmReader.ReadByte();
                    int num4 = _vgmReader.ReadByte();
                    _DACData = _vgmReader.ReadBytes((int)_vgmReader.ReadUInt32());
                }
            }
            input.Close();
            return true;
        }

        private void StreamVGM(object sender, EventArgs e)
        {
            if (_iSaidStop)
                return;
            if (_lastCommand == 102 && !_looped)
            {
                _lastCommand = 0;
                _instance.Volume = 0f;
                _iSaidStop = true;
                Stop();
            }
            else
            {
                int[] buffer = new int[2];
                int num1 = 0;
                int num2 = _intBuffer.Length / 2;
                bool flag1 = false;
                while (num1 != num2)
                {
                    bool flag2;
                    if (_wait == 0 && !gameFroze)
                    {
                        flag2 = false;
                        byte num3 = _vgmReader.ReadByte();
                        _lastCommand = num3;
                        switch (num3)
                        {
                            case 79:
                                int num4 = _vgmReader.ReadByte();
                                break;
                            case 80:
                                _psg.Write(_vgmReader.ReadByte());
                                break;
                            case 82:
                                _chip.WritePort0(_vgmReader.ReadByte(), _vgmReader.ReadByte());
                                break;
                            case 83:
                                _chip.WritePort1(_vgmReader.ReadByte(), _vgmReader.ReadByte());
                                break;
                            case 97:
                                _wait = _vgmReader.ReadUInt16();
                                if (_wait != 0)
                                {
                                    flag2 = true;
                                    break;
                                }
                                break;
                            case 98:
                                _wait = 735;
                                flag2 = true;
                                break;
                            case 99:
                                _wait = 882;
                                flag2 = true;
                                break;
                            case 102:
                                if (!_looped)
                                {
                                    _vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
                                    flag1 = true;
                                    break;
                                }
                                if (_VGMHead.lngLoopOffset != 0U)
                                {
                                    _vgmReader.BaseStream.Seek(_VGMHead.lngLoopOffset - _VGMDataOffset, SeekOrigin.Begin);
                                    break;
                                }
                                _vgmReader.BaseStream.Seek(0L, SeekOrigin.Begin);
                                break;
                            case 103:
                                int num5 = _vgmReader.ReadByte();
                                int num6 = _vgmReader.ReadByte();
                                _vgmReader.BaseStream.Position += _vgmReader.ReadUInt32();
                                break;
                            case 224:
                                _DACOffset = (int)_vgmReader.ReadUInt32();
                                break;
                        }
                        if (num3 >= 112 && num3 <= 127)
                        {
                            _wait = (num3 & 15) + 1;
                            if (_wait != 0)
                                flag2 = true;
                        }
                        else if (num3 >= 128 && num3 <= 143)
                        {
                            _wait = num3 & 15;
                            _chip.WritePort0(42, _DACData[_DACOffset]);
                            ++_DACOffset;
                            if (_wait != 0)
                                flag2 = true;
                        }
                        if (_wait != 0)
                            --_wait;
                    }
                    else
                    {
                        flag2 = true;
                        if (_wait > 0)
                        {
                            for (_waitInc += _playbackSpeed; _wait > 0 && _waitInc >= 1.0; --_wait)
                                --_waitInc;
                        }
                    }
                    if (!flag1)
                    {
                        if (flag2)
                        {
                            _chip.Update(buffer, 1);
                            short num7 = (short)buffer[0];
                            short num8 = (short)buffer[1];
                            _psg.Update(buffer, 1);
                            short num9 = (short)buffer[0];
                            short num10 = (short)buffer[1];
                            _intBuffer[num1 * 2] = Maths.Clamp((num7 + num9) * 2, short.MinValue, short.MaxValue);
                            _intBuffer[num1 * 2 + 1] = Maths.Clamp((num8 + num10) * 2, short.MinValue, short.MaxValue);
                            ++num1;
                            if (num1 == num2)
                                break;
                        }
                    }
                    else
                        break;
                }
                for (int index = 0; index < _intBuffer.Length; ++index)
                {
                    short num11 = (short)_intBuffer[index];
                    _buffer[index * 2] = (byte)((uint)num11 & byte.MaxValue);
                    _buffer[index * 2 + 1] = (byte)(num11 >> 8 & byte.MaxValue);
                }
                int num12 = num1 * 2;
                if (num12 / 4.0 - (int)(num12 / 4.0) > 0.0)
                    num12 -= 2;
                _instance.SubmitBuffer(_buffer, 0, num12);
                _instance.SubmitBuffer(_buffer, num12, num12);
            }
        }
    }
}
