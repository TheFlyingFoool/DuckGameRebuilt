// Decompiled with JetBrains decompiler
// Type: DuckGame.FileRecording
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.IO.Compression;

namespace DuckGame
{
    public class FileRecording : Recording
    {
        private BinaryWriter _writer;
        private BinaryReader _reader;
        private string _fileName = "";
        private bool _setFile;
        private int _lastTextureWrittenIndex;
        private int _lastEffectWrittenIndex;
        private RasterizerState _defaultRasterizerState;
        private bool _loadedNextFrame;
        private int _curFrame;
        private float _framePos;

        public string fileName
        {
            get => this._fileName;
            set
            {
                this._fileName = value;
                this._setFile = true;
            }
        }

        public FileRecording()
        {
            this.Initialize();
            this._defaultRasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };
        }

        public void StopWriting()
        {
            if (this._writer != null)
                this._writer.Close();
            if (this._reader != null)
                this._reader.Close();
            this.UpdateAtlasFile();
        }

        public void StartWriting(string name)
        {
            if (this._reader != null)
            {
                this._reader.Close();
                this._reader = null;
            }
            this._loadedNextFrame = false;
            if (!this._setFile)
            {
                if (name != "" && name != null)
                {
                    this._fileName = name;
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string shortDateString = now.ToShortDateString();
                    now = DateTime.Now;
                    string shortTimeString = now.ToShortTimeString();
                    this._fileName = "funstream-" + (shortDateString + "-" + shortTimeString).Replace("/", "_").Replace(":", "-").Replace(" ", "");
                }
                this._writer = new BinaryWriter(new GZipStream(System.IO.File.Open(this._fileName + ".vid", FileMode.Create), CompressionMode.Compress));
            }
            else
            {
                if (this._writer == null)
                    return;
                this._writer.Close();
                this._writer = null;
            }
        }

        public void LoadAtlasFile(string file = "")
        {
            if (this._writer != null)
                this.UpdateAtlasFile();
            if (file == "")
                file = this._fileName;
            this._fileName = file;
            BinaryReader binaryReader = new BinaryReader(System.IO.File.Open(file + ".dat", FileMode.Open));
            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
            {
                int num = binaryReader.ReadByte();
                short index = binaryReader.ReadInt16();
                if (num == 0)
                {
                    if (binaryReader.ReadByte() == 0)
                    {
                        int width = binaryReader.ReadInt32();
                        int height = binaryReader.ReadInt32();
                        byte[] numArray = new byte[width * height * 4];
                        binaryReader.Read(numArray, 0, width * height * 4);
                        RenderTarget2D tex = new RenderTarget2D(width, height);
                        tex.SetData<byte>(numArray);
                        Content.SetTextureAtIndex(index, tex);
                    }
                    else
                    {
                        string name = binaryReader.ReadString();
                        Content.SetTextureAtIndex(index, Content.Load<Tex2D>(name));
                    }
                }
                else
                {
                    string name = binaryReader.ReadString();
                    Content.SetEffectAtIndex(index, name == "" ? (MTEffect)new BasicEffect(DuckGame.Graphics.device) : Content.Load<MTEffect>(name));
                }
            }
        }

        public void UpdateAtlasFile()
        {
            if (this._writer == null)
                return;
            BinaryWriter binaryWriter = new BinaryWriter(System.IO.File.Open(this._fileName + ".dat", FileMode.OpenOrCreate));
            binaryWriter.Seek(0, SeekOrigin.End);
            for (int textureWrittenIndex = this._lastTextureWrittenIndex; textureWrittenIndex < Content.textureList.Count; ++textureWrittenIndex)
            {
                binaryWriter.Write((byte)0);
                Tex2D texture = Content.textureList[textureWrittenIndex];
                binaryWriter.Write(texture.textureIndex);
                if (texture.textureName == "" || texture.textureName == "__renderTarget" || texture.textureName == "__internal")
                {
                    binaryWriter.Write((byte)0);
                    binaryWriter.Write(texture.width);
                    binaryWriter.Write(texture.height);
                    byte[] numArray = new byte[texture.width * texture.height * 4];
                    if (!texture.IsDisposed && !((GraphicsResource)texture.nativeObject).IsDisposed)
                        texture.GetData<byte>(numArray);
                    binaryWriter.Write(numArray);
                }
                else
                {
                    binaryWriter.Write((byte)1);
                    binaryWriter.Write(texture.textureName);
                }
                ++this._lastTextureWrittenIndex;
            }
            for (int effectWrittenIndex = this._lastEffectWrittenIndex; effectWrittenIndex < Content.effectList.Count; ++effectWrittenIndex)
            {
                binaryWriter.Write((byte)1);
                MTEffect effect = Content.effectList[effectWrittenIndex];
                binaryWriter.Write(effect.effectIndex);
                binaryWriter.Write(effect.effectName);
                ++this._lastEffectWrittenIndex;
            }
            binaryWriter.Close();
        }

        public override void IncrementFrame(float speed = 1f)
        {
            if (this._writer != null)
            {
                this._writer.Close();
                this._writer = null;
            }
            if (this._reader == null)
                this._reader = new BinaryReader(new GZipStream(System.IO.File.Open(this._fileName + ".vid", FileMode.Open), CompressionMode.Decompress));
            int num1 = 2;
            int num2 = 0;
            if (this._loadedNextFrame)
            {
                this._framePos += speed;
                if (_framePos < 1.0)
                    return;
                --this._framePos;
                num1 = 1;
                num2 = this._curFrame == 0 ? 0 : 1;
                this._curFrame = this._curFrame == 0 ? 1 : 0;
                this._frames[this._curFrame].Update();
            }
            this._loadedNextFrame = true;
            for (int index1 = num2; index1 < num2 + num1; ++index1)
            {
                this._frame = index1;
                this._frames[this._frame].Reset();
                Color color = new Color(this._reader.ReadByte(), this._reader.ReadByte(), this._reader.ReadByte(), this._reader.ReadByte());
                int num3 = this._reader.ReadInt32();
                this._frames[this._frame].sounds.Clear();
                for (int index2 = 0; index2 < num3; ++index2)
                {
                    RecorderSoundItem recorderSoundItem;
                    recorderSoundItem.sound = this._reader.ReadString();
                    recorderSoundItem.pan = this._reader.ReadSingle();
                    recorderSoundItem.pitch = this._reader.ReadSingle();
                    recorderSoundItem.volume = this._reader.ReadSingle();
                    this._frames[this._frame].sounds.Add(recorderSoundItem);
                }
                int num4 = this._reader.ReadInt32();
                this._frames[this._frame].currentObject = num4;
                this._frames[this._frame].backgroundColor = color;
                for (int key = 0; key < num4; ++key)
                {
                    if (this._reader.ReadByte() == 0)
                        this._frames[this._frame]._states[key] = new RecorderFrameStateChange()
                        {
                            rasterizerState = this._defaultRasterizerState,
                            samplerState = SamplerState.PointClamp,
                            blendState = BlendState.AlphaBlend,
                            sortMode = (SpriteSortMode)this._reader.ReadInt32(),
                            depthStencilState = this._reader.ReadByte() == 0 ? DepthStencilState.Default : DepthStencilState.DepthRead,
                            effectIndex = this._reader.ReadInt16(),
                            stateIndex = this._reader.ReadInt32(),
                            camera = new Matrix()
                            {
                                M11 = this._reader.ReadSingle(),
                                M12 = this._reader.ReadSingle(),
                                M13 = this._reader.ReadSingle(),
                                M14 = this._reader.ReadSingle(),
                                M21 = this._reader.ReadSingle(),
                                M22 = this._reader.ReadSingle(),
                                M23 = this._reader.ReadSingle(),
                                M24 = this._reader.ReadSingle(),
                                M31 = this._reader.ReadSingle(),
                                M32 = this._reader.ReadSingle(),
                                M33 = this._reader.ReadSingle(),
                                M34 = this._reader.ReadSingle(),
                                M41 = this._reader.ReadSingle(),
                                M42 = this._reader.ReadSingle(),
                                M43 = this._reader.ReadSingle(),
                                M44 = this._reader.ReadSingle()
                            },
                            scissor = new Rectangle(this._reader.ReadInt32(), this._reader.ReadInt32(), this._reader.ReadInt32(), this._reader.ReadInt32())
                        };
                    this._frames[this._frame].objects[key].texture = this._reader.ReadInt16();
                    this._frames[this._frame].objects[key].topLeft.x = this._reader.ReadSingle();
                    this._frames[this._frame].objects[key].topLeft.y = this._reader.ReadSingle();
                    this._frames[this._frame].objects[key].bottomRight.x = this._reader.ReadSingle();
                    this._frames[this._frame].objects[key].bottomRight.y = this._reader.ReadSingle();
                    this._frames[this._frame].objects[key].rotation = this._reader.ReadSingle();
                    this._frames[this._frame].objects[key].color = new Color(this._reader.ReadByte(), this._reader.ReadByte(), this._reader.ReadByte(), this._reader.ReadByte());
                    this._frames[this._frame].objects[key].texX = this._reader.ReadInt16();
                    this._frames[this._frame].objects[key].texY = this._reader.ReadInt16();
                    this._frames[this._frame].objects[key].texW = this._reader.ReadInt16();
                    this._frames[this._frame].objects[key].texH = this._reader.ReadInt16();
                    this._frames[this._frame].objects[key].depth = this._reader.ReadSingle();
                }
            }
            this._frame = this._curFrame;
        }

        public override void NextFrame()
        {
            if (this._writer == null)
                return;
            this._writer.Write(this._frames[this._frame].backgroundColor.r);
            this._writer.Write(this._frames[this._frame].backgroundColor.g);
            this._writer.Write(this._frames[this._frame].backgroundColor.b);
            this._writer.Write(this._frames[this._frame].backgroundColor.a);
            this._writer.Write(this._frames[this._frame].sounds.Count);
            for (int index = 0; index < this._frames[this._frame].sounds.Count; ++index)
            {
                this._writer.Write(this._frames[this._frame].sounds[index].sound);
                this._writer.Write(this._frames[this._frame].sounds[index].pan);
                this._writer.Write(this._frames[this._frame].sounds[index].pitch);
                this._writer.Write(this._frames[this._frame].sounds[index].volume);
            }
            this._writer.Write(this._frames[this._frame].currentObject);
            for (int key = 0; key < this._frames[this._frame].currentObject; ++key)
            {
                if (this._frames[this._frame]._states.ContainsKey(key))
                {
                    this._writer.Write((byte)0);
                    RecorderFrameStateChange state = this._frames[this._frame]._states[key];
                    this._writer.Write((int)state.sortMode);
                    this._writer.Write(state.depthStencilState == DepthStencilState.Default ? (byte)0 : (byte)1);
                    this._writer.Write(state.effectIndex);
                    this._writer.Write(state.stateIndex);
                    this._writer.Write(state.camera.M11);
                    this._writer.Write(state.camera.M12);
                    this._writer.Write(state.camera.M13);
                    this._writer.Write(state.camera.M14);
                    this._writer.Write(state.camera.M21);
                    this._writer.Write(state.camera.M22);
                    this._writer.Write(state.camera.M23);
                    this._writer.Write(state.camera.M24);
                    this._writer.Write(state.camera.M31);
                    this._writer.Write(state.camera.M32);
                    this._writer.Write(state.camera.M33);
                    this._writer.Write(state.camera.M34);
                    this._writer.Write(state.camera.M41);
                    this._writer.Write(state.camera.M42);
                    this._writer.Write(state.camera.M43);
                    this._writer.Write(state.camera.M44);
                    this._writer.Write(state.scissor.x);
                    this._writer.Write(state.scissor.y);
                    this._writer.Write(state.scissor.width);
                    this._writer.Write(state.scissor.height);
                }
                else
                    this._writer.Write((byte)1);
                this._writer.Write(this._frames[this._frame].objects[key].texture);
                this._writer.Write(this._frames[this._frame].objects[key].topLeft.x);
                this._writer.Write(this._frames[this._frame].objects[key].topLeft.y);
                this._writer.Write(this._frames[this._frame].objects[key].bottomRight.x);
                this._writer.Write(this._frames[this._frame].objects[key].bottomRight.y);
                this._writer.Write(this._frames[this._frame].objects[key].rotation);
                this._writer.Write(this._frames[this._frame].objects[key].color.r);
                this._writer.Write(this._frames[this._frame].objects[key].color.g);
                this._writer.Write(this._frames[this._frame].objects[key].color.b);
                this._writer.Write(this._frames[this._frame].objects[key].color.a);
                this._writer.Write(this._frames[this._frame].objects[key].texX);
                this._writer.Write(this._frames[this._frame].objects[key].texY);
                this._writer.Write(this._frames[this._frame].objects[key].texW);
                this._writer.Write(this._frames[this._frame].objects[key].texH);
                this._writer.Write(this._frames[this._frame].objects[key].depth);
            }
            base.NextFrame();
        }
    }
}
