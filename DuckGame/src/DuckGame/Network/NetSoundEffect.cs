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
        public Function function;
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
            pEffect.sfxIndex = kCurrentSfxIndex;
            _sfxDictionary[pName] = pEffect;
            _sfxIndexDictionary[kCurrentSfxIndex] = pEffect;
            ++kCurrentSfxIndex;
        }

        public static void Initialize()
        {
            Register("duckJump", new NetSoundEffect(new string[1]
            {
        "jump"
            })
            {
                volume = 0.5f
            });
            Register("duckDisarm", new NetSoundEffect(new string[1]
            {
        "disarm"
            })
            {
                volume = 0.3f
            });
            Register("duckTinyMotion", new NetSoundEffect(new string[1]
            {
        "tinyMotion"
            }));
            Register("duckSwear", new NetSoundEffect(new List<string>()
      {
        "cutOffQuack",
        "cutOffQuack2"
      }, new List<string>() { "quackBleep" })
            {
                pitchVariationLow = -0.05f,
                pitchVariationHigh = 0.05f
            });
            Register("duckScream", new NetSoundEffect(new string[3]
            {
        "quackYell01",
        "quackYell02",
        "quackYell03"
            }));
            Register("itemBoxHit", new NetSoundEffect(new string[1]
            {
        "hitBox"
            })
            {
                volume = 1f
            });
            Register("bananaSplat", new NetSoundEffect(new string[1]
            {
        "smallSplat"
            })
            {
                pitchVariationLow = -0.2f,
                pitchVariationHigh = 0.2f
            });
            Register("bananaEat", new NetSoundEffect(new string[1]
            {
        "smallSplat"
            })
            {
                pitchVariationLow = -0.6f,
                pitchVariationHigh = 0.6f
            });
            Register("bananaSlip", new NetSoundEffect(new string[1]
            {
        "slip"
            })
            {
                pitchVariationLow = -0.2f,
                pitchVariationHigh = 0.2f
            });
            Register("mineDoubleBeep", new NetSoundEffect(new string[1]
            {
        "doubleBeep"
            }));
            Register("minePullPin", new NetSoundEffect(new string[1]
            {
        "pullPin"
            }));
            Register("oldPistolClick", new NetSoundEffect(new string[1]
            {
        "click"
            })
            {
                volume = 1f,
                pitch = 0.5f
            });
            Register("oldPistolSwipe", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.6f,
                pitch = -0.3f
            });
            Register("oldPistolSwipe2", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.7f
            });
            Register("oldPistolLoad", new NetSoundEffect(new string[1]
            {
        "shotgunLoad"
            }));
            Register("pelletGunClick", new NetSoundEffect(new string[1]
            {
        "click"
            })
            {
                volume = 1f,
                pitch = 0.5f
            });
            Register("pelletGunSwipe", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.4f,
                pitch = 0.3f
            });
            Register("pelletGunSwipe2", new NetSoundEffect(new string[1]
            {
        "swipe"
            })
            {
                volume = 0.5f,
                pitch = 0.4f
            });
            Register("pelletGunLoad", new NetSoundEffect(new string[1]
            {
        "loadLow"
            })
            {
                volume = 0.7f,
                pitchVariationLow = -0.05f,
                pitchVariationHigh = 0.05f
            });
            Register("sniperLoad", new NetSoundEffect(new string[1]
            {
        "loadSniper"
            }));
            Register("flowerHappyQuack", new NetSoundEffect(new string[1]
            {
        "happyQuack01"
            })
            {
                pitchVariationLow = -0.1f,
                pitchVariationHigh = 0.1f
            });
            Register("equipmentTing", new NetSoundEffect(new string[1]
            {
        "ting2"
            }));
        }

        public static void Update()
        {
            if (_soundsPlayedThisFrame.Count <= 0)
                return;
            Send.Message(new NMNetSoundEvents(_soundsPlayedThisFrame));
            _soundsPlayedThisFrame.Clear();
        }

        public static NetSoundEffect Get(string pSound)
        {
            NetSoundEffect netSoundEffect;
            _sfxDictionary.TryGetValue(pSound, out netSoundEffect);
            return netSoundEffect;
        }

        public static NetSoundEffect Get(ushort pSound)
        {
            NetSoundEffect netSoundEffect;
            _sfxIndexDictionary.TryGetValue(pSound, out netSoundEffect);
            return netSoundEffect;
        }

        public static void Play(string pSound)
        {
            NetSoundEffect pSound1;
            if (!_sfxDictionary.TryGetValue(pSound, out pSound1))
                return;
            PlayAndSynchronize(pSound1);
        }

        public static void Play(string pSound, float pPitchOffset)
        {
            NetSoundEffect pSound1;
            if (!_sfxDictionary.TryGetValue(pSound, out pSound1))
                return;
            PlayAndSynchronize(pSound1, pPitchOffset);
        }

        public static void Play(ushort pSound)
        {
            NetSoundEffect pSound1;
            if (!_sfxIndexDictionary.TryGetValue(pSound, out pSound1))
                return;
            PlayAndSynchronize(pSound1);
        }

        private static void PlayAndSynchronize(NetSoundEffect pSound, float pPitchOffset = 0f)
        {
            pSound.Play(pit: pPitchOffset);
            _soundsPlayedThisFrame.Add(pSound);
        }

        public NetSoundEffect()
        {
        }

        public NetSoundEffect(params string[] sounds) => _sounds = new List<string>(sounds);

        public NetSoundEffect(List<string> sounds, List<string> rareSounds)
        {
            _sounds = sounds;
            _rareSounds = rareSounds;
        }

        public int index
        {
            get => _index;
            set
            {
                _index = value;
                if (_localIndex == value)
                    return;
                _localIndex = value;
                PlaySound();
            }
        }

        public void Play(float vol = 1f, float pit = 0f)
        {
            PlaySound(vol, pit);
            ++_index;
            _localIndex = _index;
        }

        private void PlaySound(float vol = 1f, float pit = 0f)
        {
            if (function != null)
                function();
            vol *= volume;
            pit += pitch;
            pit += Rando.Float(pitchVariationLow, pitchVariationHigh);
            if (pit < -1f)
                pit = -1f;
            if (pit > 1f)
                pit = 1f;
            if (_sounds.Count <= 0)
                return;
            if (pitchBinding != null)
                pit = (byte)pitchBinding.value / (float)byte.MaxValue;
            string str = "";
            if (appendBinding != null)
                str = ((byte)appendBinding.value).ToString();
            if (_rareSounds.Count > 0 && Rando.Float(1f) > 0.9f)
                SFX.Play(_rareSounds[Rando.Int(_rareSounds.Count - 1)] + str, vol, pit);
            else
                SFX.Play(_sounds[Rando.Int(_sounds.Count - 1)] + str, vol, pit);
        }

        public delegate void Function();
    }
}
