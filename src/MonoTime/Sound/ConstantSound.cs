// Decompiled with JetBrains decompiler
// Type: DuckGame.ConstantSound
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;

namespace DuckGame
{
    public class ConstantSound : IAutoUpdate
    {
        private Sound _effect;
        private float _lerpVolume;
        private float _lerpSpeed = 0.1f;
        private bool _killSound;
        private Level _startLevel;

        public float volume
        {
            get => this._effect == null ? 1f : this._effect.Volume;
            set
            {
                if (this._effect != null)
                    this._effect.Volume = value;
                this._lerpVolume = value;
            }
        }

        public float lerpVolume
        {
            get => this._lerpVolume;
            set => this._lerpVolume = value;
        }

        public float lerpSpeed
        {
            get => this._lerpSpeed;
            set => this._lerpSpeed = value;
        }

        public float pitch
        {
            get => this._effect == null ? 0.0f : this._effect.Pitch;
            set
            {
                if (this._effect == null)
                    return;
                this._effect.Pitch = value;
            }
        }

        public ConstantSound(string sound, float startVolume = 0.0f, float startPitch = 0.0f, string multiSound = null)
        {
            AutoUpdatables.Add(this);
            this._effect = (double)startVolume <= 0.0 ? (multiSound == null ? SFX.Get(sound, startVolume * SFX.volume, startPitch, looped: true) : SFX.GetMultiSound(sound, multiSound)) : SFX.Play(sound, startVolume * SFX.volume, startPitch, looped: true);
            if (this._effect != null)
                return;
            DevConsole.Log("ConstantSound not found! (" + sound + ")");
            this._effect = new InvalidSound(sound, startVolume, startPitch, 0.0f, true);
        }

        ~ConstantSound()
        {
            this._lerpSpeed = 0.0f;
            this._lerpVolume = 0.0f;
        }

        public void Kill() => this._killSound = true;

        public void Mute()
        {
            if (this._effect == null || this._effect.IsDisposed)
                return;
            this._effect.Volume = 0.0f;
        }

        public void Update()
        {
            if (this._effect != null && this._effect.IsDisposed)
                this._effect = null;
            else if (this._effect == null || this._startLevel != null && Level.current != this._startLevel)
            {
                if (this._effect == null)
                    return;
                this._effect.Kill();
            }
            else if (this._killSound)
            {
                this._effect.Stop();
            }
            else
            {
                if ((double)this._effect.Volume > 0.00999999977648258 && this._effect.State != SoundState.Playing)
                {
                    this._effect.Play();
                    this._startLevel = Level.current;
                }
                else if ((double)this._effect.Volume < 0.00999999977648258 && this._effect.State == SoundState.Playing)
                    this._effect.Stop();
                this._effect.Volume = Maths.LerpTowards(this._effect.Volume, this._lerpVolume, this._lerpSpeed);
            }
        }
    }
}
