// Decompiled with JetBrains decompiler
// Type: DuckGame.MultiSound
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;

namespace DuckGame
{
    public class MultiSound : Sound
    {
        private MultiSoundUpdater _controller;
        private SoundState _state = SoundState.Stopped;

        public override bool IsDisposed => this._controller.IsDisposed;

        public override float Pitch
        {
            get => this._controller.Pitch;
            set => this._controller.Pitch = value;
        }

        public override float Pan
        {
            get => this._controller.Pan;
            set => this._controller.Pan = value;
        }

        public override bool IsLooped
        {
            get => this._controller.IsLooped;
            set => this._controller.IsLooped = value;
        }

        public override float Volume
        {
            set => this._volume = value;
        }

        public override SoundState State => this._state;

        public override void Play()
        {
            if (this._killed)
                return;
            this._controller.Play(this);
            this._state = SoundState.Playing;
        }

        public override void Stop()
        {
            if (this._killed)
                return;
            this._controller.Stop(this);
            this._state = SoundState.Stopped;
        }

        public new void Unpooled()
        {
            if (this._state != SoundState.Stopped)
                this._controller.Stop(this);
            this._pooled = false;
        }

        public new void Pause()
        {
            int num = this._killed ? 1 : 0;
        }

        public MultiSound(MultiSoundUpdater updater)
        {
            this._controller = updater;
            this._volume = 0.0f;
        }
    }
}
