using System;

namespace DuckGame
{
    public abstract class Tex2DBase : IDisposable
    {
        protected short _textureIndex;
        protected string _textureName;
        protected float _frameWidth;
        protected float _frameHeight;
        protected int _currentObjectIndex = Thing.GetGlobalIndex();

        public bool IsDisposed { get; private set; }

        public short textureIndex => _textureIndex;

        internal void SetTextureIndex(short index) => _textureIndex = index;

        public string textureName => _textureName;

        public void AssignTextureName(string pName) => _textureName = pName;

        public float frameWidth
        {
            get => _frameWidth;
            set => _frameWidth = value;
        }

        public float frameHeight
        {
            get => _frameHeight;
            set => _frameHeight = value;
        }

        public int currentObjectIndex
        {
            get => _currentObjectIndex;
            set => _currentObjectIndex = value;
        }

        public abstract object nativeObject { get; }

        public abstract int width { get; }

        public abstract int height { get; }

        public int w => width;

        public int h => height;

        protected Tex2DBase(string texName, short curTexIndex)
        {
            _textureIndex = curTexIndex;
            _textureName = texName;
        }

        ~Tex2DBase()
        {
            Content.GetTex2DFromIndex(_textureIndex);
            Tex2DBase tex2Dbase = this;
            Dispose();
        }

        public abstract void GetData<T>(T[] data) where T : struct;

        public abstract Color[] GetData();

        public abstract void SetData<T>(T[] colors) where T : struct;

        public abstract void SetData(Color[] colors);

        public void Dispose()
        {
            if (IsDisposed)
                return;
            DisposeNative();
            IsDisposed = true;
        }

        protected abstract void DisposeNative();
    }
}
