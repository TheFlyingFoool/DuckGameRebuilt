// Decompiled with JetBrains decompiler
// Type: DuckGame.InvalidSound
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using System;

namespace DuckGame
{
    public class InvalidSound : Sound
    {
        private float _pitch;
        private float _pan;
        private bool _isLooped;

        public override float Pitch
        {
            get => this._pitch;
            set => this._pitch = value;
        }

        public override float Pan
        {
            get => this._pan;
            set => this._pan = value;
        }

        public override bool IsLooped
        {
            get => this._isLooped;
            set => this._isLooped = value;
        }

        public override SoundState State => SoundState.Stopped;

        public override float Volume
        {
            get => Math.Min(1f, Math.Max(0f, this._volume));
            set => this._volume = Math.Min(1f, Math.Max(0f, value));
        }

        public override void Play()
        {
        }

        public override void Stop()
        {
        }

        public override void Unpooled()
        {
        }

        public override void Pause()
        {
        }

        public override bool IsDisposed => false;

        public InvalidSound(string sound, float vol, float pitch, float pan, bool looped)
        {
            this._name = sound;
            this._volume = vol;
            this._pitch = pitch;
            this._pan = pan;
            this._isLooped = looped;
        }
    }
}
