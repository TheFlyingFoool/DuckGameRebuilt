// Decompiled with JetBrains decompiler
// Type: DuckGame.NetSoundEffect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NetSoundEffect
    {
        public static Dictionary<string, NetSoundEffect> _sfxDictionary = new Dictionary<string, NetSoundEffect>();
        public static Dictionary<ushort, NetSoundEffect> _sfxIndexDictionary = new Dictionary<ushort, NetSoundEffect>();
        private static ushort kCurrentSfxIndex = 0;
        private static List<NetSoundEffect> _soundsPlayedThisFrame = new List<NetSoundEffect>();
        public ushort sfxIndex;
        public NetSoundEffect.Function function;
        public FieldBinding pitchBinding;
        public FieldBinding appendBinding;
        private List<string> _sounds = new List<string>();
        private List<string> _rareSounds = new List<string>();
        public float volume = 1f;
        public float pitch;
        public float pitchVariationHigh;
        public float pitchVariationLow;
        private int _localIndex;
        private int _index;

        public static void Register(string pName, NetSoundEffect pEffect)
        {
            pEffect.sfxIndex = NetSoundEffect.kCurrentSfxIndex;
            NetSoundEffect._sfxDictionary[pName] = pEffect;
            NetSoundEffect._sfxIndexDictionary[NetSoundEffect.kCurrentSfxIndex] = pEffect;
            ++NetSoundEffect.kCurrentSfxIndex;
        }

        public static void Initialize()
        {
            NetSoundEffect.Register("duckJump", new NetSoundEffect(new string[1]
            {
        "jump"
            })
            {
                volume = 0.5f
            });
            NetSoundEffect.Register("duckDisarm", new NetSoundEffect(new string[1]
            {
        "disarm"
            })
            {
                volume = 0.3f
            });
            NetSoundEffect.Register("duckTinyMotion", new NetSoundEffect(new string[1]
            {
        "tinyMotion"
            }));
            NetSoundEffect.Register("duckSwear", new NetSoundEffect(new List<string>()
      {
        "cutOffQuack",
        "cutOffQuack2"
      }, new List<string>() { "quackBleep" })
            {
                pitchVariationLow = -0.05f,
                pitchVariationHigh = 0.05f
            });
            NetSoundEffect.Register("duckScream", new NetSoundEffect(new string[3]
            {
        "quackYell01",
        "quackYell02",
        "quackYell03"
            }));
            NetSoundEffect.Register("itemBoxHit", new NetSoundEffect(new string[1]
            {
        "hitBox"
            })
            {
                volume = 1f
            });
            NetSoundEffect.Register("bananaSplat", new NetSoundEffect(new string[1]
            {
        "smallSplat"
            })
            {
                pitchVariationLow = -0.2f,
                pitchVariationHigh = 0.2f
            });
            NetSoundEffect.Register("bananaEat", new NetSoundEffect(new string[1]
            {
        "smallSplat"
            })
            {
                pitchVariationLow = -0.6f,
                pitchVariationHigh = 0.6f
            });
            NetSoundEffect.Register("bananaSlip", new NetSoundEffect(new string[1]
            {
        "slip"
            })
            {
                pitchVariationLow = -0.2f,
                pitchVariationHigh = 0.2f
            });
            NetSoundEffect.Register("mineDoubleBeep", new NetSoundEffect(new string[1]
            {
        "doubleBeep"
            }));
            NetSoundEffect.Register("minePullPin", new NetSoundEffect(new string[1]
            {
        "pullPin"
            }));
            NetSoundEffect.Register("oldPistolClick", new NetSoundEffect(new string[1]
            {
        "click"
            })
            {
                volume = 1f,
                pitch = 0.5f
            });
            NetSoundEffect.Register("oldPistolSwipe", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.6f,
                pitch = -0.3f
            });
            NetSoundEffect.Register("oldPistolSwipe2", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.7f
            });
            NetSoundEffect.Register("oldPistolLoad", new NetSoundEffect(new string[1]
            {
        "shotgunLoad"
            }));
            NetSoundEffect.Register("pelletGunClick", new NetSoundEffect(new string[1]
            {
        "click"
            })
            {
                volume = 1f,
                pitch = 0.5f
            });
            NetSoundEffect.Register("pelletGunSwipe", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.4f,
                pitch = 0.3f
            });
            NetSoundEffect.Register("pelletGunSwipe2", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.5f,
                pitch = 0.4f
            });
            NetSoundEffect.Register("pelletGunLoad", new NetSoundEffect(new string[1]
            {
        "loadLow"
            })
            {
                volume = 0.7f,
                pitchVariationLow = -0.05f,
                pitchVariationHigh = 0.05f
            });
            NetSoundEffect.Register("sniperLoad", new NetSoundEffect(new string[1]
            {
        "loadSniper"
            }));
            NetSoundEffect.Register("flowerHappyQuack", new NetSoundEffect(new string[1]
            {
        "happyQuack01"
            })
            {
                pitchVariationLow = -0.1f,
                pitchVariationHigh = 0.1f
            });
            NetSoundEffect.Register("equipmentTing", new NetSoundEffect(new string[1]
            {
        "ting2"
            }));
        }

        public static void Update()
        {
            if (NetSoundEffect._soundsPlayedThisFrame.Count <= 0)
                return;
            Send.Message(new NMNetSoundEvents(NetSoundEffect._soundsPlayedThisFrame));
            NetSoundEffect._soundsPlayedThisFrame.Clear();
        }

        public static NetSoundEffect Get(string pSound)
        {
            NetSoundEffect netSoundEffect;
            NetSoundEffect._sfxDictionary.TryGetValue(pSound, out netSoundEffect);
            return netSoundEffect;
        }

        public static NetSoundEffect Get(ushort pSound)
        {
            NetSoundEffect netSoundEffect;
            NetSoundEffect._sfxIndexDictionary.TryGetValue(pSound, out netSoundEffect);
            return netSoundEffect;
        }

        public static void Play(string pSound)
        {
            NetSoundEffect pSound1;
            if (!NetSoundEffect._sfxDictionary.TryGetValue(pSound, out pSound1))
                return;
            NetSoundEffect.PlayAndSynchronize(pSound1);
        }

        public static void Play(string pSound, float pPitchOffset)
        {
            NetSoundEffect pSound1;
            if (!NetSoundEffect._sfxDictionary.TryGetValue(pSound, out pSound1))
                return;
            NetSoundEffect.PlayAndSynchronize(pSound1, pPitchOffset);
        }

        public static void Play(ushort pSound)
        {
            NetSoundEffect pSound1;
            if (!NetSoundEffect._sfxIndexDictionary.TryGetValue(pSound, out pSound1))
                return;
            NetSoundEffect.PlayAndSynchronize(pSound1);
        }

        private static void PlayAndSynchronize(NetSoundEffect pSound, float pPitchOffset = 0.0f)
        {
            pSound.Play(pit: pPitchOffset);
            NetSoundEffect._soundsPlayedThisFrame.Add(pSound);
        }

        public NetSoundEffect()
        {
        }

        public NetSoundEffect(params string[] sounds) => this._sounds = new List<string>(sounds);

        public NetSoundEffect(List<string> sounds, List<string> rareSounds)
        {
            this._sounds = sounds;
            this._rareSounds = rareSounds;
        }

        public int index
        {
            get => this._index;
            set
            {
                this._index = value;
                if (this._localIndex == value)
                    return;
                this._localIndex = value;
                this.PlaySound();
            }
        }

        public void Play(float vol = 1f, float pit = 0.0f)
        {
            this.PlaySound(vol, pit);
            ++this._index;
            this._localIndex = this._index;
        }

        private void PlaySound(float vol = 1f, float pit = 0.0f)
        {
            if (this.function != null)
                this.function();
            vol *= this.volume;
            pit += this.pitch;
            pit += Rando.Float(this.pitchVariationLow, this.pitchVariationHigh);
            if ((double)pit < -1.0)
                pit = -1f;
            if ((double)pit > 1.0)
                pit = 1f;
            if (this._sounds.Count <= 0)
                return;
            if (this.pitchBinding != null)
                pit = (byte)this.pitchBinding.value / (float)byte.MaxValue;
            string str = "";
            if (this.appendBinding != null)
                str = ((byte)this.appendBinding.value).ToString();
            if (this._rareSounds.Count > 0 && (double)Rando.Float(1f) > 0.899999976158142)
                SFX.Play(this._rareSounds[Rando.Int(this._rareSounds.Count - 1)] + str, vol, pit);
            else
                SFX.Play(this._sounds[Rando.Int(this._sounds.Count - 1)] + str, vol, pit);
        }

        public delegate void Function();
    }
}
