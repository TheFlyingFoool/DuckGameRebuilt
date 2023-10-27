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
            get => _fileName;
            set
            {
                _fileName = value;
                _setFile = true;
            }
        }

        public FileRecording()
        {
            Initialize();
            _defaultRasterizerState = new RasterizerState
            {
                CullMode = CullMode.None
            };
        }

        public void StopWriting()
        {
            if (_writer != null)
                _writer.Close();
            if (_reader != null)
                _reader.Close();
            UpdateAtlasFile();
        }

        public void StartWriting(string name)
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;
            }
            _loadedNextFrame = false;
            if (!_setFile)
            {
                if (name != "" && name != null)
                {
                    _fileName = name;
                }
                else
                {
                    DateTime now = DateTime.Now;
                    string shortDateString = now.ToShortDateString();
                    now = DateTime.Now;
                    string shortTimeString = now.ToShortTimeString();
                    _fileName = "funstream-" + (shortDateString + "-" + shortTimeString).Replace("/", "_").Replace(":", "-").Replace(" ", "");
                }
                _writer = new BinaryWriter(new GZipStream(File.Open(_fileName + ".vid", FileMode.Create), CompressionMode.Compress));
            }
            else
            {
                if (_writer == null)
                    return;
                _writer.Close();
                _writer = null;
            }
        }

        public void LoadAtlasFile(string file = "")
        {
            if (_writer != null)
                UpdateAtlasFile();
            if (file == "")
                file = _fileName;
            _fileName = file;
            BinaryReader binaryReader = new BinaryReader(File.Open(file + ".dat", FileMode.Open));
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
                        tex.SetData(numArray);
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
                    Content.SetEffectAtIndex(index, name == "" ? (MTEffect)new BasicEffect(Graphics.device) : Content.Load<MTEffect>(name));
                }
            }
        }

        public void UpdateAtlasFile()
        {
            if (_writer == null)
                return;
            BinaryWriter binaryWriter = new BinaryWriter(File.Open(_fileName + ".dat", FileMode.OpenOrCreate));
            binaryWriter.Seek(0, SeekOrigin.End);
            for (int textureWrittenIndex = _lastTextureWrittenIndex; textureWrittenIndex < Content.textureList.Count; ++textureWrittenIndex)
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
                        texture.GetData(numArray);
                    binaryWriter.Write(numArray);
                }
                else
                {
                    binaryWriter.Write((byte)1);
                    binaryWriter.Write(texture.textureName);
                }
                ++_lastTextureWrittenIndex;
            }
            for (int effectWrittenIndex = _lastEffectWrittenIndex; effectWrittenIndex < Content.effectList.Count; ++effectWrittenIndex)
            {
                binaryWriter.Write((byte)1);
                MTEffect effect = Content.effectList[effectWrittenIndex];
                binaryWriter.Write(effect.effectIndex);
                binaryWriter.Write(effect.effectName);
                ++_lastEffectWrittenIndex;
            }
            binaryWriter.Close();
        }

        public override void IncrementFrame(float speed = 1f)
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }
            if (_reader == null)
                _reader = new BinaryReader(new GZipStream(File.Open(_fileName + ".vid", FileMode.Open), CompressionMode.Decompress));
            int num1 = 2;
            int num2 = 0;
            if (_loadedNextFrame)
            {
                _framePos += speed;
                if (_framePos < 1)
                    return;
                --_framePos;
                num1 = 1;
                num2 = _curFrame == 0 ? 0 : 1;
                _curFrame = _curFrame == 0 ? 1 : 0;
                _frames[_curFrame].Update();
            }
            _loadedNextFrame = true;
            for (int index1 = num2; index1 < num2 + num1; ++index1)
            {
                _frame = index1;
                _frames[_frame].Reset();
                Color color = new Color(_reader.ReadByte(), _reader.ReadByte(), _reader.ReadByte(), _reader.ReadByte());
                int num3 = _reader.ReadInt32();
                _frames[_frame].sounds.Clear();
                for (int index2 = 0; index2 < num3; ++index2)
                {
                    RecorderSoundItem recorderSoundItem;
                    recorderSoundItem.sound = _reader.ReadString();
                    recorderSoundItem.pan = _reader.ReadSingle();
                    recorderSoundItem.pitch = _reader.ReadSingle();
                    recorderSoundItem.volume = _reader.ReadSingle();
                    _frames[_frame].sounds.Add(recorderSoundItem);
                }
                int num4 = _reader.ReadInt32();
                _frames[_frame].currentObject = num4;
                _frames[_frame].backgroundColor = color;
                for (int key = 0; key < num4; ++key)
                {
                    if (_reader.ReadByte() == 0)
                        _frames[_frame]._states[key] = new RecorderFrameStateChange()
                        {
                            rasterizerState = _defaultRasterizerState,
                            samplerState = SamplerState.PointClamp,
                            blendState = BlendState.AlphaBlend,
                            sortMode = (SpriteSortMode)_reader.ReadInt32(),
                            depthStencilState = _reader.ReadByte() == 0 ? DepthStencilState.Default : DepthStencilState.DepthRead,
                            effectIndex = _reader.ReadInt16(),
                            stateIndex = _reader.ReadInt32(),
                            camera = new Matrix()
                            {
                                M11 = _reader.ReadSingle(),
                                M12 = _reader.ReadSingle(),
                                M13 = _reader.ReadSingle(),
                                M14 = _reader.ReadSingle(),
                                M21 = _reader.ReadSingle(),
                                M22 = _reader.ReadSingle(),
                                M23 = _reader.ReadSingle(),
                                M24 = _reader.ReadSingle(),
                                M31 = _reader.ReadSingle(),
                                M32 = _reader.ReadSingle(),
                                M33 = _reader.ReadSingle(),
                                M34 = _reader.ReadSingle(),
                                M41 = _reader.ReadSingle(),
                                M42 = _reader.ReadSingle(),
                                M43 = _reader.ReadSingle(),
                                M44 = _reader.ReadSingle()
                            },
                            scissor = new Rectangle(_reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32())
                        };
                    _frames[_frame].objects[key].texture = _reader.ReadInt16();
                    _frames[_frame].objects[key].topLeft.x = _reader.ReadSingle();
                    _frames[_frame].objects[key].topLeft.y = _reader.ReadSingle();
                    _frames[_frame].objects[key].bottomRight.x = _reader.ReadSingle();
                    _frames[_frame].objects[key].bottomRight.y = _reader.ReadSingle();
                    _frames[_frame].objects[key].rotation = _reader.ReadSingle();
                    _frames[_frame].objects[key].color = new Color(_reader.ReadByte(), _reader.ReadByte(), _reader.ReadByte(), _reader.ReadByte());
                    _frames[_frame].objects[key].texX = _reader.ReadInt16();
                    _frames[_frame].objects[key].texY = _reader.ReadInt16();
                    _frames[_frame].objects[key].texW = _reader.ReadInt16();
                    _frames[_frame].objects[key].texH = _reader.ReadInt16();
                    _frames[_frame].objects[key].depth = _reader.ReadSingle();
                }
            }
            _frame = _curFrame;
        }

        public override void NextFrame()
        {
            if (_writer == null)
                return;
            _writer.Write(_frames[_frame].backgroundColor.r);
            _writer.Write(_frames[_frame].backgroundColor.g);
            _writer.Write(_frames[_frame].backgroundColor.b);
            _writer.Write(_frames[_frame].backgroundColor.a);
            _writer.Write(_frames[_frame].sounds.Count);
            for (int index = 0; index < _frames[_frame].sounds.Count; ++index)
            {
                _writer.Write(_frames[_frame].sounds[index].sound);
                _writer.Write(_frames[_frame].sounds[index].pan);
                _writer.Write(_frames[_frame].sounds[index].pitch);
                _writer.Write(_frames[_frame].sounds[index].volume);
            }
            _writer.Write(_frames[_frame].currentObject);
            for (int key = 0; key < _frames[_frame].currentObject; ++key)
            {
                if (_frames[_frame]._states.ContainsKey(key))
                {
                    _writer.Write((byte)0);
                    RecorderFrameStateChange state = _frames[_frame]._states[key];
                    _writer.Write((int)state.sortMode);
                    _writer.Write(state.depthStencilState == DepthStencilState.Default ? (byte)0 : (byte)1);
                    _writer.Write(state.effectIndex);
                    _writer.Write(state.stateIndex);
                    _writer.Write(state.camera.M11);
                    _writer.Write(state.camera.M12);
                    _writer.Write(state.camera.M13);
                    _writer.Write(state.camera.M14);
                    _writer.Write(state.camera.M21);
                    _writer.Write(state.camera.M22);
                    _writer.Write(state.camera.M23);
                    _writer.Write(state.camera.M24);
                    _writer.Write(state.camera.M31);
                    _writer.Write(state.camera.M32);
                    _writer.Write(state.camera.M33);
                    _writer.Write(state.camera.M34);
                    _writer.Write(state.camera.M41);
                    _writer.Write(state.camera.M42);
                    _writer.Write(state.camera.M43);
                    _writer.Write(state.camera.M44);
                    _writer.Write(state.scissor.x);
                    _writer.Write(state.scissor.y);
                    _writer.Write(state.scissor.width);
                    _writer.Write(state.scissor.height);
                }
                else
                    _writer.Write((byte)1);
                _writer.Write(_frames[_frame].objects[key].texture);
                _writer.Write(_frames[_frame].objects[key].topLeft.x);
                _writer.Write(_frames[_frame].objects[key].topLeft.y);
                _writer.Write(_frames[_frame].objects[key].bottomRight.x);
                _writer.Write(_frames[_frame].objects[key].bottomRight.y);
                _writer.Write(_frames[_frame].objects[key].rotation);
                _writer.Write(_frames[_frame].objects[key].color.r);
                _writer.Write(_frames[_frame].objects[key].color.g);
                _writer.Write(_frames[_frame].objects[key].color.b);
                _writer.Write(_frames[_frame].objects[key].color.a);
                _writer.Write(_frames[_frame].objects[key].texX);
                _writer.Write(_frames[_frame].objects[key].texY);
                _writer.Write(_frames[_frame].objects[key].texW);
                _writer.Write(_frames[_frame].objects[key].texH);
                _writer.Write(_frames[_frame].objects[key].depth);
            }
            base.NextFrame();
        }
    }
}
