// Decompiled with JetBrains decompiler
// Type: DuckGame.MultiSoundUpdater
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace DuckGame
{
    public class MultiSoundUpdater : Sound
    {
        private List<MultiSound> _instances = new List<MultiSound>();
        private SoundEffectInstance _single;
        private SoundEffectInstance _multi;
        private SoundState _state = SoundState.Stopped;
        private new string _name = "";
        private int _playCount;

        public override void Kill()
        {
            for (int index = 0; index < this._playCount; ++index)
                this.Stop();
            this._killed = true;
        }

        public MultiSound GetInstance() => new MultiSound(this);

        public override bool IsDisposed
        {
            get
            {
                if (this._single == null)
                    return false;
                return this._single.IsDisposed || this._multi.IsDisposed;
            }
        }

        public override float Pitch
        {
            get => this._single != null ? this._single.Pitch : 0f;
            set
            {
                if (this._single == null)
                    return;
                this._single.Pitch = value;
                this._multi.Pitch = value;
            }
        }

        public override float Pan
        {
            get => this._single != null ? this._single.Pan : 0f;
            set
            {
                if (this._single == null)
                    return;
                this._single.Pan = value;
                this._single.Pan = value;
            }
        }

        public override bool IsLooped
        {
            get => this._single != null && this._single.IsLooped;
            set
            {
                if (this._single == null)
                    return;
                this._single.IsLooped = value;
                this._multi.IsLooped = value;
            }
        }

        public override SoundState State => this._state;

        public new string name => this._name;

        public void Play(MultiSound who)
        {
            if (this._killed || this._single == null || this._instances.Contains(who))
                return;
            if (this._playCount == 0 && SFX.PoolSound(this))
            {
                this._single.Volume = this._volume * SFX.volume;
                this._single.Play();
            }
            ++this._playCount;
            this._state = SoundState.Playing;
            this._pooled = true;
            this._instances.Add(who);
        }

        public override void Stop()
        {
            while (this._instances.Count > 0)
                this._instances[0].Stop();
            this._volume = 0f;
        }

        public void Stop(MultiSound who)
        {
            if (this._killed || this._single == null || !this._instances.Contains(who))
                return;
            if (this._state == SoundState.Playing)
                --this._playCount;
            if (this._playCount == 0)
            {
                this._single.Volume = 0f;
                this._single.Stop();
                this._multi.Volume = 0f;
                this._multi.Stop();
                this._pooled = false;
                SFX.UnpoolSound(this);
                this._state = SoundState.Stopped;
            }
            this._instances.Remove(who);
        }

        public override void Unpooled()
        {
            if (this._single != null && this._state != SoundState.Stopped)
            {
                this._single.Volume = 0f;
                this._single.Stop();
                this._multi.Volume = 0f;
                this._multi.Stop();
            }
            this._pooled = false;
        }

        public void Update()
        {
            if (this._single == null)
                return;
            float num1 = 0f;
            foreach (MultiSound instance in this._instances)
                num1 += instance.Volume;
            int count = this._instances.Count;
            float num2 = num1 / (count > 0 ? count : 1f);
            this._volume = Lerp.Float(this._volume, (float)(num2 * 0.7f + Maths.Clamp(this._instances.Count, 0, 4) / 4f * num2 * 0.3f), 0.05f);
            if (this._state != SoundState.Playing)
                return;
            if (this._playCount > 1)
            {
                if (this._multi.State == SoundState.Stopped)
                    this._multi.Play();
                if (this._single.State != SoundState.Stopped)
                {
                    this._single.Volume = Lerp.Float(this._single.Volume, 0f, 0.05f);
                    if (this._single.Volume < 0.02f)
                    {
                        this._single.Volume = 0f;
                        this._single.Stop();
                    }
                }
                this._multi.Volume = Lerp.Float(this._multi.Volume, this._volume * SFX.volume, 0.05f);
            }
            else
            {
                if (this._playCount != 1)
                    return;
                if (this._single.State == SoundState.Stopped)
                    this._single.Play();
                if (this._multi.State != SoundState.Stopped)
                {
                    this._multi.Volume = Lerp.Float(this._multi.Volume, 0f, 0.05f);
                    if (this._multi.Volume < 0.02f)
                    {
                        this._multi.Volume = 0f;
                        this._multi.Stop();
                    }
                }
                this._single.Volume = Lerp.Float(this._single.Volume, this._volume * SFX.volume, 0.05f);
            }
        }

        public MultiSoundUpdater(string id, string single, string multi)
        {
            this._name = id;
            if (id != "")
            {
                this._single = SFX.GetInstance(single, 0f, looped: true);
                this._multi = SFX.GetInstance(multi, 0f, looped: true);
            }
            this._cannotBeCancelled = true;
            this._volume = 1f;
        }
    }
}
