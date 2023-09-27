// Decompiled with JetBrains decompiler
// Type: DuckGame.Input
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DuckGame
{
    public class Input
    {
        public static InputCode konamiCode = new InputCode()
        {
              triggers = new List<string>()
              {
                Triggers.Up,
                Triggers.Up,
                Triggers.Down,
                Triggers.Down,
                Triggers.Left,
                Triggers.Right,
                Triggers.Left,
                Triggers.Right,
                Triggers.Quack,
                Triggers.Jump
              }
        };
        public static InputCode konamiCodeAlternate = new InputCode()
        {
            triggers = new List<string>()
            {
                 Triggers.Up + "|" + Triggers.Jump,
                Triggers.Up + "|" + Triggers.Jump,
                Triggers.Down,
                Triggers.Down,
                Triggers.Left,
                Triggers.Right,
                Triggers.Left,
                Triggers.Right,
                Triggers.Quack,
                Triggers.Up + "|" + Triggers.Jump,
            }
        };
        public static InputCode hookCode = new InputCode()
        {
            triggers = new List<string>()
            {
                Triggers.Jump,
                Triggers.Quack,
                Triggers.Ragdoll,
                Triggers.Ragdoll,
                Triggers.Grab
            },
            breakSpeed = 0.06f
        };
        private static bool _ignoreInput;
        public static bool debuggerInputOverride = false;
        public static InputProfile lastActiveProfile = new InputProfile();
        private static List<Sprite> _buttonStyles = new List<Sprite>();
        public static Dictionary<Keys, char> keyToChar = new Dictionary<Keys, char>();
        private static Dictionary<string, InputProfile> _profiles = new Dictionary<string, InputProfile>();
        private static List<InputDevice> _devices = new List<InputDevice>();
        public static Dictionary<string, Sprite> _triggerImageMap = new Dictionary<string, Sprite>();
        private static List<GenericController> _gamePads = new List<GenericController>();
        private static Array _keys = Enum.GetValues(typeof(Keys));
        private static List<DeviceInputMapping> _defaultInputMapping = new List<DeviceInputMapping>();
        private static List<DeviceInputMapping> _oldInputDefaults = new List<DeviceInputMapping>()
        {
            new DeviceInputMapping()
            {
                deviceName = "KEYBOARD P1",
                deviceGUID = "",
                map = new Dictionary<string,int>()
                {
                    {Triggers.Left, (int)Keys.A},
                    {Triggers.Right, (int)Keys.D},
                    {Triggers.Up, (int)Keys.W},
                    {Triggers.Down, (int)Keys.S},
                    {Triggers.Jump, (int)Keys.W},
                    {Triggers.Shoot, (int)Keys.V},
                    {Triggers.Grab, (int)Keys.C},
                    {Triggers.Start, (int)Keys.Escape},
                    {Triggers.Ragdoll, (int)Keys.Q},
                    {Triggers.Strafe, (int)Keys.B},
                    {Triggers.Quack, (int)Keys.E},
                    {Triggers.Select, (int)Keys.Space},
                    {Triggers.Chat, (int)Keys.Enter}
                }
            },
            new DeviceInputMapping()
            {
                deviceName = "KEYBOARD P2",
                deviceGUID = "",
                map = new Dictionary<string,int>()
                {
                    {Triggers.Left, (int)Keys.Left},
                    {Triggers.Right, (int)Keys.Right},
                    {Triggers.Up, (int)Keys.Up},
                    {Triggers.Down, (int)Keys.Down},
                    {Triggers.Jump, (int)Keys.Up},
                    {Triggers.Shoot, (int)Keys.OemSemicolon},
                    {Triggers.Grab, (int)Keys.L},
                    {Triggers.Start, (int)Keys.OemPlus},
                    {Triggers.Ragdoll, (int)Keys.I},
                    {Triggers.Strafe, (int)Keys.K},
                    {Triggers.Quack, (int)Keys.O},
                    {Triggers.Select, (int)Keys.RightShift},
                }
            }
        };




        public static List<DeviceInputMapping> _defaultInputMappingPresets = new List<DeviceInputMapping>()
        {
            new DeviceInputMapping() 
            {
                deviceName = "KEYBOARD P1",
                deviceGUID = "",
                map = new Dictionary<string,int>()
                {
                    {Triggers.Left, (int)Keys.A},
                    {Triggers.Right, (int)Keys.D},
                    {Triggers.Up, (int)Keys.W},
                    {Triggers.Down, (int)Keys.S},
                    {Triggers.Jump, (int)Keys.Space},
                    {Triggers.Shoot, (int)Keys.H},
                    {Triggers.Grab, (int)Keys.G},
                    {Triggers.Start, (int)Keys.Escape},
                    {Triggers.Ragdoll, (int)Keys.F},          
                    {Triggers.Strafe, (int)Keys.LeftShift},
                    {Triggers.Quack, (int)Keys.E},
                    {Triggers.Select, (int)Keys.Space},           
                    {Triggers.Chat, (int)Keys.Enter},
                    {Triggers.Cancel, (int)Keys.E},
                    {Triggers.Menu1, (int)Keys.H},
                    {Triggers.Menu2, (int)Keys.Q},
                    {Triggers.MenuLeft, (int)Keys.A},
                    {Triggers.MenuRight, (int)Keys.D},
                    {Triggers.MenuUp, (int)Keys.W},
                    {Triggers.MenuDown, (int)Keys.S},
                    {Triggers.RightStick, (int)Keys.Tab},
                    {Triggers.VoiceRegister, (int)Keys.Home},
                    {Triggers.KeyboardF, (int)Keys.F},
                }
            },
            new DeviceInputMapping() 
            {
                deviceName = "KEYBOARD P2",
                deviceGUID = "",
                map = new Dictionary<string,int>()
                {
                    {Triggers.Left, (int)Keys.Left},
                    {Triggers.Right, (int)Keys.Right},
                    {Triggers.Up, (int)Keys.Up},
                    {Triggers.Down, (int)Keys.Down},
                    {Triggers.Jump, (int)Keys.RightControl},
                    {Triggers.Shoot, (int)Keys.OemQuotes},
                    {Triggers.Grab, (int)Keys.OemSemicolon},
                    {Triggers.Start, (int)Keys.OemPlus},
                    {Triggers.Ragdoll, (int)Keys.O},
                    {Triggers.Strafe, (int)Keys.L},
                    {Triggers.Quack, (int)Keys.P},
                    {Triggers.Select, (int)Keys.RightShift},
                    {Triggers.Cancel, (int)Keys.P},
                    {Triggers.Menu1, (int)Keys.OemQuotes},
                    {Triggers.Menu2, (int)Keys.OemSemicolon},
                    {Triggers.MenuLeft, (int)Keys.Left},
                    {Triggers.MenuRight, (int)Keys.Right},
                    {Triggers.MenuUp, (int)Keys.Up},
                    {Triggers.MenuDown, (int)Keys.Down},
                    {Triggers.RightStick, (int)Keys.Tab}, 
                }
            },
            new DeviceInputMapping()
            {
                deviceName = "XBOX GAMEPAD",
                deviceGUID = "",
                map = new Dictionary<string,int>()
                {
                    {Triggers.Left, (int)PadButton.DPadLeft},
                    {Triggers.Right, (int)PadButton.DPadRight},
                    {Triggers.Up, (int)PadButton.DPadUp},
                    {Triggers.Down, (int)PadButton.DPadDown},
                    {Triggers.Jump, (int)PadButton.A},
                    {Triggers.Shoot, (int)PadButton.X},
                    {Triggers.Grab, (int)PadButton.Y},
                    {Triggers.Start, (int)PadButton.Start},
                    {Triggers.Ragdoll, (int)PadButton.RightShoulder},
                    {Triggers.Strafe, (int)PadButton.LeftShoulder},
                    {Triggers.Quack, (int)PadButton.B},
                    {Triggers.Select, (int)PadButton.A},

                    {Triggers.LeftTrigger, (int)PadButton.LeftTrigger},
                    {Triggers.RightTrigger, (int)PadButton.RightTrigger},
                    {Triggers.LeftBumper, (int)PadButton.LeftShoulder},
                    {Triggers.RightBumper, (int)PadButton.RightShoulder},
                    {Triggers.LeftStick, (int)PadButton.LeftStick},
                    {Triggers.RightStick, (int)PadButton.RightStick},
                    {Triggers.Cancel, (int)PadButton.B},
                    {Triggers.LeftOptionButton,       (int)PadButton.Back },
                    {Triggers.Menu1,       (int)PadButton.X },
                    {Triggers.Menu2,       (int)PadButton.Y },
                    {Triggers.MenuLeft, (int)PadButton.DPadLeft},
                    {Triggers.MenuRight, (int)PadButton.DPadRight},
                    {Triggers.MenuUp, (int)PadButton.DPadUp},
                    {Triggers.MenuDown, (int)PadButton.DPadDown},
                }
            },
            new DeviceInputMapping()
            {
                deviceName = "GENERIC GAMEPAD",
                deviceGUID = "",
                map = new Dictionary<string,int>()
                {
                    {Triggers.Left, (int)PadButton.DPadLeft},
                    {Triggers.Right, (int)PadButton.DPadRight},
                    {Triggers.Up, (int)(int)PadButton.DPadUp},
                    {Triggers.Down, (int)PadButton.DPadDown},
                    {Triggers.Jump, (int)PadButton.A},
                    {Triggers.Shoot, (int)PadButton.X},
                    {Triggers.Grab, (int)PadButton.Y},
                    {Triggers.Start, (int)PadButton.Start},
                    {Triggers.Ragdoll, (int)PadButton.RightShoulder},
                    {Triggers.Strafe, (int)PadButton.LeftShoulder},
                    {Triggers.Quack, (int)PadButton.B},
                    {Triggers.Select, (int)PadButton.A},

                    {Triggers.LeftTrigger, (int)PadButton.LeftTrigger},
                    {Triggers.RightTrigger, (int)PadButton.RightTrigger},
                    {Triggers.LeftBumper, (int)PadButton.LeftShoulder},
                    {Triggers.RightBumper, (int)PadButton.RightShoulder},
                    {Triggers.LeftStick, (int)PadButton.LeftStick},
                    {Triggers.RightStick, (int)PadButton.RightStick},
                    {Triggers.Cancel, (int)PadButton.B},
                    {Triggers.LeftOptionButton, (int)PadButton.Back},
                    {Triggers.Menu1, (int)PadButton.X},
                    {Triggers.Menu2, (int)PadButton.Y},

                    {Triggers.MenuLeft, (int)PadButton.DPadLeft},
                    {Triggers.MenuRight, (int)PadButton.DPadRight},
                    {Triggers.MenuUp, (int)PadButton.DPadUp},
                    {Triggers.MenuDown, (int)PadButton.DPadDown},
                }
            },
        };
        public static bool _imeAllowed = false;
        public static bool _prevImeAllowed = false;
        public static bool _dinputEnabled = false;
        public static int _suppressInputChangeMessages = 0;
        private static volatile int _dinputInitializeStatus = int.MinValue;
        private static volatile Exception _dinputInitException;
        private static int _dinputInitTimesCalled = 0;
        private static int _updateWaitFrames = 0;
        private static Thread _gamepadThread;
        private static volatile bool _gamepadsChanged;
        private static volatile bool _padConnectionChange = false;
        private static volatile string _changeName = "";
        private static volatile bool _changePluggedIn = false;
        public static bool enumeratingGamepads = false;
        public static int timesToEnumerateGamepads = 0;
        public static bool mightHavePlaystationController = false;
        public static volatile bool devicesChanged = false;
        private static int _deviceUpdateWait = 60;
        public static bool uiDevicesHaveChanged = false;
        private static bool _ignoreFirstInputChange = true;
        private static bool _initializedIME = false;
        private static bool _initializedMessageHook = false;
        private static bool _prevForceMode = false;

        public static bool ignoreInput
        {
            get
            {
                return !debuggerInputOverride && (!Graphics.inFocus || _ignoreInput);
            }
            set
            {
                _ignoreInput = value;
            }
        }

        public static List<Sprite> buttonStyles => _buttonStyles;

        public static Sprite GetTriggerSprite(string trigger)
        {
            Sprite triggerSprite;
            _triggerImageMap.TryGetValue(trigger, out triggerSprite);
            return triggerSprite;
        }

        public static List<InputDevice> GetInputDevices() => _devices;

        public static void Save()
        {
            DevConsole.Log(DCSection.General, "Input.Save()...");
            DuckXML doc = new DuckXML();
            DXMLNode node = new DXMLNode("Mappings");
            foreach (DeviceInputMapping deviceInputMapping in _defaultInputMapping)
                node.Add(deviceInputMapping.Serialize());
            doc.Add(node);
            string path = DuckFile.optionsDirectory + "/input.dat";
            DuckFile.SaveDuckXML(doc, path);
        }

        public static List<DeviceInputMapping> defaultInputMappingPresets
        {
            get
            {
                List<DeviceInputMapping> inputMappingPresets = new List<DeviceInputMapping>();
                foreach (DeviceInputMapping inputMappingPreset in _defaultInputMappingPresets)
                    inputMappingPresets.Add(inputMappingPreset.Clone());
                return inputMappingPresets;
            }
        }

        public static DeviceInputMapping GetDefaultMapping(
          string productName,
          string productGUID,
          bool presets = false,
          bool makeClone = true,
          Profile p = null)
        {
            if (p != null && p.linkedProfile != null)
                return GetDefaultMapping(productName, productGUID, presets, makeClone, p.linkedProfile);
            List<DeviceInputMapping> source = _defaultInputMapping;
            if (p != null && p.inputMappingOverrides.FirstOrDefault(x => x.deviceGUID == productGUID && x.deviceName == productName) == null)
                p = null;
            if (presets)
                source = defaultInputMappingPresets;
            if (p != null)
                source = p.inputMappingOverrides;
            foreach (DeviceInputMapping defaultMapping in source)
            {
                if (defaultMapping.deviceName == productName && defaultMapping.deviceGUID == productGUID)
                    return defaultMapping;
            }
            if (p != null)
                return null;
            DeviceInputMapping defaultMapping1 = source.FirstOrDefault(x => x.deviceName == "GENERIC GAMEPAD");
            if (!makeClone)
                return defaultMapping1;
            if (defaultMapping1 == null)
                return new DeviceInputMapping();
            DeviceInputMapping defaultMapping2 = defaultMapping1.Clone();
            defaultMapping2.deviceName = productName;
            defaultMapping2.deviceGUID = productGUID;
            return defaultMapping2;
        }

        public static void TryFallback(string pTrigger, string pFallback, DeviceInputMapping pMap)
        {
            if (pMap.map.ContainsKey(pTrigger) || !pMap.map.ContainsKey(pFallback))
                return;
            pMap.map[pTrigger] = pMap.map[pFallback];
        }

        public static bool MappingIsDefault(DeviceInputMapping pMapping)
        {
            DeviceInputMapping defaultMapping = GetDefaultMapping(pMapping.deviceName, pMapping.deviceGUID);
            return defaultMapping != null && defaultMapping.IsEqual(pMapping);
        }

        public static void SetDefaultMapping(DeviceInputMapping mapping, Profile overrideProfile = null)
        {
            if (overrideProfile != null && MappingIsDefault(mapping))
                return;
            TryFallback(Triggers.MenuLeft, Triggers.Left, mapping);
            TryFallback(Triggers.MenuRight, Triggers.Right, mapping);
            TryFallback(Triggers.MenuUp, Triggers.Up, mapping);
            TryFallback(Triggers.MenuDown, Triggers.Down, mapping);
            TryFallback(Triggers.Menu1, Triggers.Shoot, mapping);
            TryFallback(Triggers.Menu2, Triggers.Grab, mapping);
            TryFallback(Triggers.Cancel, Triggers.Quack, mapping);
            DeviceInputMapping defaultMapping = GetDefaultMapping(mapping.deviceName, mapping.deviceGUID);
            if (defaultMapping != null)
            {
                int num = 0;
                bool flag;
                do
                {
                    flag = true;
                    foreach (KeyValuePair<string, int> keyValuePair1 in mapping.map)
                    {
                        foreach (KeyValuePair<string, int> keyValuePair2 in mapping.map)
                        {
                            if (keyValuePair1.Key != keyValuePair2.Key && keyValuePair1.Value == keyValuePair2.Value && Triggers.IsUITrigger(keyValuePair1.Key) && Triggers.IsUITrigger(keyValuePair2.Key))
                            {
                                if (defaultMapping.map.ContainsKey(keyValuePair1.Key))
                                {
                                    mapping.map[keyValuePair1.Key] = defaultMapping.map[keyValuePair1.Key];
                                    flag = false;
                                }
                                if (defaultMapping.map.ContainsKey(keyValuePair2.Key))
                                {
                                    mapping.map[keyValuePair2.Key] = defaultMapping.map[keyValuePair2.Key];
                                    flag = false;
                                }
                                if (!flag)
                                    break;
                            }
                        }
                        if (!flag)
                            break;
                    }
                    ++num;
                }
                while (!flag && num <= 100);
            }
            List<DeviceInputMapping> source = _defaultInputMapping;
            if (overrideProfile != null)
                source = overrideProfile.inputMappingOverrides;
            DeviceInputMapping deviceInputMapping1 = source.FirstOrDefault(x => x.deviceName == mapping.deviceName && x.deviceGUID == mapping.deviceGUID);
            DeviceInputMapping deviceInputMapping2 = defaultInputMappingPresets.FirstOrDefault(x => x.deviceName == mapping.deviceName && x.deviceGUID == mapping.deviceGUID);
            if (deviceInputMapping1 != null)
            {
                DevConsole.Log(DCSection.General, "SetDefaultMapping() Found existing map for (" + mapping.deviceName + ")...");
                deviceInputMapping1.map = mapping.map;
                deviceInputMapping1.graphicMap = mapping.graphicMap;
                if (deviceInputMapping2 == null)
                    return;
                foreach (KeyValuePair<string, int> keyValuePair in deviceInputMapping2.map)
                {
                    if (!deviceInputMapping1.map.ContainsKey(keyValuePair.Key))
                        deviceInputMapping1.MapInput(keyValuePair.Key, keyValuePair.Value);
                }
            }
            else
            {
                DeviceInputMapping compare = _defaultInputMapping.FirstOrDefault(x => x.deviceName == mapping.deviceName && x.deviceGUID == mapping.deviceGUID);
                if (compare != null)
                {
                    if (!mapping.IsEqual(compare))
                    {
                        source.Add(mapping);
                        DevConsole.Log(DCSection.General, "Added input mapping for (" + mapping.deviceName + ")...");
                    }
                    else
                        DevConsole.Log(DCSection.General, "Skipped duplicate mapping for (" + mapping.deviceName + ")...");
                }
                else
                {
                    DevConsole.Log(DCSection.General, "Found default settings for (" + mapping.deviceName + ")...");
                    if (_defaultInputMapping.FirstOrDefault(x => x.deviceName == "GENERIC GAMEPAD").IsEqual(mapping))
                        return;
                    source.Add(mapping);
                }
            }
        }

        public static List<DeviceInputMapping> CloneDefaultMappings()
        {
            List<DeviceInputMapping> deviceInputMappingList = new List<DeviceInputMapping>();
            foreach (DeviceInputMapping deviceInputMapping in _defaultInputMapping)
                deviceInputMappingList.Add(deviceInputMapping.Clone());
            return deviceInputMappingList;
        }

        public static void SetDefaultMappings(List<DeviceInputMapping> mappings) => _defaultInputMapping = mappings;

        public static void ApplyDefaultMappings()
        {
            foreach (InputProfile defaultProfile in InputProfile.defaultProfiles)
            {
                bool flag = false;
                if (Profiles.all != null)
                {
                    foreach (Profile duckProfile in Profiles.active)
                    {
                        if (duckProfile.inputProfile == defaultProfile)
                        {
                            ApplyDefaultMapping(defaultProfile, duckProfile);
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                    ApplyDefaultMapping(defaultProfile);
            }
        }

        /// <summary>
        /// This function resets an InputProfile to it's default control settings. If duckProfile is defined, it will use the profile's mapping.
        /// Otherwise, it will use the global mapping. If none exist, it will use the built in defaults.
        /// </summary>
        /// <param name="p">InputProfile to reset</param>
        /// <param name="duckProfile">Optional duck profile to take controls from</param>
        public static void ApplyDefaultMapping(InputProfile p = null, Profile duckProfile = null)
        {
            if (p == null)
            {
                DevConsole.Log(DCSection.General, "ApplyDefaultMapping() had a null argument, for some reason?");
            }
            else
            {
                GenericController device = p.GetDevice(typeof(GenericController)) as GenericController;
                p.ClearMappings();
                if (device != null)
                {
                    if (device.device != null)
                    {
                        Profile p1 = duckProfile;
                        if (p1 == null && Profiles.all != null)
                        {
                            foreach (Profile profile in Profiles.active)
                            {
                                if (profile.inputProfile == p)
                                {
                                    p1 = profile;
                                    break;
                                }
                            }
                        }
                        DeviceInputMapping deviceInputMapping = GetDefaultMapping(device.device.productName, device.device.productGUID, p: p1) ?? GetDefaultMapping(device.device.productName, device.device.productGUID);
                        if (deviceInputMapping != null)
                        {
                            foreach (KeyValuePair<string, int> keyValuePair in deviceInputMapping.map)
                                p.Map(device, keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    else
                        p.Map(device, "", 0);
                }
                if (p == InputProfile.defaultProfiles[Options.Data.keyboard1PlayerIndex])
                {
                    DeviceInputMapping deviceInputMapping = GetDefaultMapping("KEYBOARD P1", "", p: duckProfile) ?? GetDefaultMapping("KEYBOARD P1", "");
                    if (deviceInputMapping == null)
                        return;
                    foreach (KeyValuePair<string, int> keyValuePair in deviceInputMapping.map)
                        p.Map(GetDevice<Keyboard>(), keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    if (p != InputProfile.defaultProfiles[Options.Data.keyboard2PlayerIndex])
                        return;
                    DeviceInputMapping deviceInputMapping = GetDefaultMapping("KEYBOARD P2", "", p: duckProfile) ?? GetDefaultMapping("KEYBOARD P2", "");
                    if (deviceInputMapping == null)
                        return;
                    foreach (KeyValuePair<string, int> keyValuePair in deviceInputMapping.map)
                        p.Map(GetDevice<Keyboard>(1), keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public static void InitializeGraphics()
        {
            MonoMain.NloadMessage = "Loading Input";
            foreach (Keys key in _keys)
            {
                char ch = KeyHelper.KeyToChar(key);
                if (ch > ' ' && ch < '\u007F')
                    keyToChar[key] = ch;
            }
            _triggerImageMap.Add("MOUSEWHEEL", new Sprite("buttons/mousewheel"));
            _triggerImageMap.Add("PLANET", new Sprite("smallEarth"));
            _triggerImageMap.Add("ARENA", new Sprite("smallArena"));
            _triggerImageMap.Add("MOON", new Sprite("smallMoon"));
            _triggerImageMap.Add("PLUG", new Sprite("plugRect"));
            _triggerImageMap.Add("UNPLUG", new Sprite("unplugRect"));
            _triggerImageMap.Add("CLIPCOPY", new Sprite("clipcopy"));
            _triggerImageMap.Add("SKIPICON", new Sprite("skipIcon"));
            _triggerImageMap.Add("SETTINGSCHANGED", new Sprite("wrenchRect"));
            _triggerImageMap.Add("NORMALICON", new Sprite("normalIcon"));
            _triggerImageMap.Add("RAINBOWICON", new Sprite("rainbowIcon"));
            _triggerImageMap.Add("CUSTOMICON", new Sprite("customIcon"));
            _triggerImageMap.Add("RANDOMICON", new Sprite("randomIcons"));
            _triggerImageMap.Add("ESCAPE", new Sprite("buttons/keyboard/escape"));
            _triggerImageMap.Add("CONSOLE", new Sprite("buttons/keyboard/tilde"));
            _triggerImageMap.Add("TINYLOCK", new Sprite("tinyLock"));
            _triggerImageMap.Add("RETICULE", new Sprite("challenge/reticule"));
            _triggerImageMap.Add("TICKET", new Sprite("arcade/ticket"));
            _triggerImageMap.Add("CHECK", new Sprite("checkIcon"));
            _triggerImageMap.Add("F1", new Sprite("buttons/keyboard/f1"));
            _triggerImageMap.Add("ALT", new Sprite("buttons/keyboard/alt"));
            _triggerImageMap.Add("COMMA", new KeyImage(','));
            _triggerImageMap.Add("DGR", new Sprite("DGR") { center = new Vec2(0, 0.75f) });
            _triggerImageMap.Add("DGRBIG", new Sprite("DGRBIG"));
            _triggerImageMap.Add("DGRBIGDIM", new Sprite("DGRBIG") { color = new Color(75, 75, 75) });
            Dictionary<string, Sprite> triggerImageMap = _triggerImageMap;
            Sprite sprite1 = new Sprite("checkIcon")
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -1f
            };
            triggerImageMap.Add("CHECKSMALL", sprite1);
            _triggerImageMap.Add("SPEEDCLOCK", new Sprite("speedrunClock"));
            _triggerImageMap.Add("STARGOODY", new Sprite("challenge/star"));
            _triggerImageMap.Add("SUITCASEGOODY", new Sprite("challenge/suitcase"));
            _triggerImageMap.Add("LAPGOODY", new Sprite("challenge/goal"));
            _triggerImageMap.Add("EDITORCURRENCY", new Sprite("editorCurrency"));
            _triggerImageMap.Add("LWING", new Sprite("arcade/titleWing"));
            Sprite sprite2 = new Sprite("arcade/titleWing")
            {
                flipH = true
            };
            sprite2.centerx = sprite2.width;
            _triggerImageMap.Add("RWING", sprite2);
            Sprite sprite3 = new Sprite("arcade/titleWing")
            {
                color = new Color(96, 119, 124)
            };
            ++sprite3.centery;
            _triggerImageMap.Add("LWINGGRAY", sprite3);
            Sprite sprite4 = new Sprite("arcade/titleWing")
            {
                flipH = true
            };
            sprite4.centerx = sprite4.width;
            ++sprite4.centery;
            sprite4.color = new Color(96, 119, 124);
            _triggerImageMap.Add("RWINGGRAY", sprite4);
            _triggerImageMap.Add("WRENCH", new Sprite("titleWrench"));
            _triggerImageMap.Add("SCREWDRIVER", new Sprite("titleScrewdriver"));
            _triggerImageMap.Add("BASELINE", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 0
            });
            _triggerImageMap.Add("BRONZE", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 1
            });
            _triggerImageMap.Add("SILVER", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 2
            });
            _triggerImageMap.Add("GOLD", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 3
            });
            _triggerImageMap.Add("PLATINUM", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 4
            });
            _triggerImageMap.Add("DEVELOPER", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 5
            });
            _triggerImageMap.Add("ONLINEBAD", new SpriteMap("onlineStatusIcons", 7, 7)
            {
                frame = 0
            });
            _triggerImageMap.Add("ONLINENEUTRAL", new SpriteMap("onlineStatusIcons", 7, 7)
            {
                frame = 1
            });
            _triggerImageMap.Add("ONLINEGOOD", new SpriteMap("onlineStatusIcons", 7, 7)
            {
                frame = 2
            });
            Sprite sprite5 = new Sprite("crownIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite5.centery -= 6f;
            _triggerImageMap.Add("HOSTCROWN", sprite5);
            Sprite sprite6 = new Sprite("subPlus");
            _triggerImageMap.Add("SUBPLUS", sprite6);
            Sprite sprite7 = new Sprite("steamIcon")
            {
                scale = new Vec2(0.25f, 0.25f)
            };
            sprite7.centery -= 48f;
            _triggerImageMap.Add("STEAMICON", sprite7);
            Sprite sprite8 = new Sprite("steamIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite8.centery -= 16f;
            _triggerImageMap.Add("STEAMICONMED", sprite8);
            Sprite sprite9 = new Sprite("accessIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite9.centery -= 8f;
            _triggerImageMap.Add("ACCESSICON", sprite9);
            Sprite sprite10 = new Sprite("vanillaIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite10.centery -= 8f;
            _triggerImageMap.Add("VANILLAICON", sprite10);
            Sprite sprite11 = new Sprite("spectatorIcon");
            _triggerImageMap.Add("SPECTATOR", sprite11);
            Sprite sprite12 = new Sprite("spectatorIcon")
            {
                scale = new Vec2(2f)
            };
            _triggerImageMap.Add("SPECTATORBIG", sprite12);
            Sprite sprite13 = new Sprite("discordIcon")
            {
                scale = new Vec2(0.25f, 0.25f)
            };
            sprite13.centery -= 48f;
            _triggerImageMap.Add("DISCORDICON", sprite13);
            Sprite sprite14 = new Sprite("singleDuck")
            {
                scale = new Vec2(1f, 1f)
            };
            _triggerImageMap.Add("_!DUCKSPAWN", sprite14);
            Sprite sprite15 = new Sprite("skipSpin");
            _triggerImageMap.Add("SKIPSPIN", sprite15);
            Sprite sprite16 = new Sprite("exclamationMoji");
            _triggerImageMap.Add("error", sprite16);
            Sprite sprite17 = new Sprite("logEvent");
            _triggerImageMap.Add("LOGEVENT", sprite17);
            Sprite sprite18 = new Sprite("networkSent");
            _triggerImageMap.Add("sent", sprite18);
            Sprite sprite19 = new Sprite("networkReceived");
            _triggerImageMap.Add("received", sprite19);
            Sprite sprite20 = new Sprite("networkDisconnect");
            _triggerImageMap.Add("disconnect", sprite20);
            Sprite sprite21 = new Sprite("networkDrop");
            _triggerImageMap.Add("netdrop", sprite21);
            Sprite sprite22 = new Sprite("blacklistX");
            _triggerImageMap.Add("blacklist", sprite22);
            _triggerImageMap.Add("SIGNALDEAD", new SpriteMap("signal", 8, 5)
            {
                frame = 0
            });
            _triggerImageMap.Add("SIGNALBAD", new SpriteMap("signal", 8, 5)
            {
                frame = 1
            });
            _triggerImageMap.Add("SIGNALNORMAL", new SpriteMap("signal", 8, 5)
            {
                frame = 2
            });
            _triggerImageMap.Add("SIGNALGOOD", new SpriteMap("signal", 8, 5)
            {
                frame = 3
            });
            _triggerImageMap.Add("PLUSKEY", new KeyImage('+'));
            _triggerImageMap.Add("ENTERKEY", new Sprite("buttons/keyboard/enter"));
            _triggerImageMap.Add("ESCAPEKEY", new Sprite("buttons/keyboard/escape"));
            _triggerImageMap.Add("ICONGRADIENT", new Sprite("iconGradient"));
            SpriteMap spriteMap1 = new SpriteMap("chanceIcon", 10, 9)
            {
                frame = 0,
                centery = 1f
            };
            _triggerImageMap.Add("CHANCEICON", spriteMap1);
            SpriteMap spriteMap2 = new SpriteMap("iconEight", 8, 8);
            _triggerImageMap.Add("ICONEIGHT", spriteMap2);
            _triggerImageMap.Add("LEFTMOUSE", new SpriteMap("buttons/mouse", 12, 15)
            {
                frame = 0
            });
            _triggerImageMap.Add("MIDDLEMOUSE", new SpriteMap("buttons/mouse", 12, 15)
            {
                frame = 1
            });
            _triggerImageMap.Add("RIGHTMOUSE", new SpriteMap("buttons/mouse", 12, 15)
            {
                frame = 2
            });
            _triggerImageMap.Add("LOADICON", new SpriteMap("iconSheet", 16, 16)
            {
                frame = 1
            });
            _triggerImageMap.Add("SAVEICON", new SpriteMap("iconSheet", 16, 16)
            {
                frame = 2
            });
            SpriteMap spriteMap3 = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                frame = 1
            };
            _triggerImageMap.Add("LOADICONTINY", spriteMap3);
            _triggerImageMap.Add("LOCKEDFOLDERICON", new SpriteMap("iconSheet", 16, 16)
            {
                frame = 14
            });
            _triggerImageMap.Add("FOLDERICON", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 2
            });
            _triggerImageMap.Add("FOLDERDELETEICON", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 8
            });
            _triggerImageMap.Add("SELECTICON", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 3
            });
            _triggerImageMap.Add("DELETEFLAG_OFF", new SpriteMap("deleteFlag", 8, 8)
            {
                frame = 0
            });
            _triggerImageMap.Add("DELETEFLAG_ON", new SpriteMap("deleteFlag", 8, 8)
            {
                frame = 1
            });
            Sprite sprite23 = new Sprite("muteIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite23.centery -= 6f;
            _triggerImageMap.Add("MUTEICON", sprite23);
            Sprite sprite24 = new Sprite("blockIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite24.centery -= 6f;
            _triggerImageMap.Add("BLOCKICON", sprite24);
            Sprite sprite25 = new Sprite("blockIcon")
            {
                scale = new Vec2(0.25f, 0.25f)
            };
            sprite25.centery -= 9f;
            _triggerImageMap.Add("BLOCKICONSMALL", sprite25);
            SpriteMap spriteMap4 = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                frame = 2
            };
            _triggerImageMap.Add("SAVEICONTINY", spriteMap4);
            SpriteMap spriteMap5 = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                frame = 0
            };
            _triggerImageMap.Add("NEWICONTINY", spriteMap5);
            _triggerImageMap.Add("RAINBOWTINY", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 6
            });
            SpriteMap spriteMap6 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 0
            };
            _triggerImageMap.Add("happyface", spriteMap6);
            SpriteMap spriteMap7 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 1
            };
            _triggerImageMap.Add("sadface", spriteMap7);
            SpriteMap spriteMap8 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 2
            };
            _triggerImageMap.Add("puffyface", spriteMap8);
            SpriteMap spriteMap9 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 3
            };
            _triggerImageMap.Add("angryface", spriteMap9);
            SpriteMap spriteMap10 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 4
            };
            _triggerImageMap.Add("yayface", spriteMap10);
            SpriteMap spriteMap11 = new SpriteMap("shrug", 78, 24)
            {
                scale = new Vec2(1f, 1f),
                centery = -1f
            };
            _triggerImageMap.Add("shrug", spriteMap11);
            SpriteMap spriteMap12 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 5
            };
            _triggerImageMap.Add("wowface", spriteMap12);
            SpriteMap spriteMap13 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 6
            };
            _triggerImageMap.Add("wtfface", spriteMap13);
            SpriteMap spriteMap14 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 7
            };
            _triggerImageMap.Add("straightface", spriteMap14);
            SpriteMap spriteMap15 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 8
            };
            _triggerImageMap.Add("oiface", spriteMap15);
            SpriteMap spriteMap16 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 9
            };
            _triggerImageMap.Add("blankface", spriteMap16);
            SpriteMap spriteMap17 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 10
            };
            _triggerImageMap.Add("sweatface", spriteMap17);
            SpriteMap spriteMap18 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 11
            };
            _triggerImageMap.Add("cryface", spriteMap18);
            Sprite sprite26 = new Sprite("cookedDuck")
            {
                scale = new Vec2(2f, 2f),
                centery = 2f
            };
            _triggerImageMap.Add("cooked", sprite26);
            Sprite sprite27 = new Sprite("grave")
            {
                scale = new Vec2(2f, 2f),
                centery = 0f
            };
            _triggerImageMap.Add("rip", sprite27);
            SpriteMap spriteMap19 = new SpriteMap("searchicon", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                centerx = -4f
            };
            _triggerImageMap.Add("searchicon", spriteMap19);
            Sprite sprite28 = new Sprite("filterOn", 16f, 16f)
            {
                scale = new Vec2(0.5f, 0.5f),
                center = new Vec2(0f, -9f)
            };
            _triggerImageMap.Add("languageFilterOn", sprite28);
            Sprite sprite29 = new Sprite("filterOff", 16f, 16f)
            {
                scale = new Vec2(0.5f, 0.5f),
                center = new Vec2(0f, -9f)
            };
            _triggerImageMap.Add("languageFilterOff", sprite29);
            SpriteMap spriteMap20 = new SpriteMap("searchiconwhite", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                centerx = -4f
            };
            _triggerImageMap.Add("searchiconwhite", spriteMap20);
            SpriteMap spriteMap21 = new SpriteMap("searchiconwhite", 16, 16)
            {
                scale = new Vec2(1f, 1f),
                centery = -6f,
                centerx = -4f
            };
            _triggerImageMap.Add("searchiconwhitebig", spriteMap21);
            SpriteMap spriteMap22 = new SpriteMap("cloudIcon", 16, 16)
            {
                scale = new Vec2(1f, 1f),
                centery = 0f,
                centerx = 0f
            };
            _triggerImageMap.Add("cloudicon", spriteMap22);
            SpriteMap spriteMap23 = new SpriteMap("exBox", 10, 10)
            {
                frame = 0,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap23.centery -= 6f;
            _triggerImageMap.Add("ITEMBOX", spriteMap23);
            SpriteMap spriteMap24 = new SpriteMap("exBox", 10, 10)
            {
                frame = 1,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap24.centery -= 6f;
            _triggerImageMap.Add("USERONLINE", spriteMap24);
            SpriteMap spriteMap25 = new SpriteMap("exBox", 10, 10)
            {
                frame = 2,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap25.centery -= 6f;
            _triggerImageMap.Add("USERAWAY", spriteMap25);
            SpriteMap spriteMap26 = new SpriteMap("exBox", 10, 10)
            {
                frame = 3,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap26.centery -= 6f;
            _triggerImageMap.Add("USERBUSY", spriteMap26);
            SpriteMap spriteMap27 = new SpriteMap("exBox", 10, 10)
            {
                frame = 4,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap27.centery -= 6f;
            _triggerImageMap.Add("USEROFFLINE", spriteMap27);
            _triggerImageMap.Add("KBDSHIFT", new Sprite("buttons/keyboard/shift"));
            _triggerImageMap.Add("KBDARROWS", new Sprite("buttons/keyboard/arrows"));
            _buttonStyles.Add(new Sprite("buttons/xbox/oButton"));
            _buttonStyles.Add(new Sprite("buttons/xbox/aButton"));
            _buttonStyles.Add(new Sprite("buttons/xbox/uButton"));
            _buttonStyles.Add(new Sprite("buttons/xbox/yButton"));
            _buttonStyles.Add(new Sprite("buttons/xbox/startButton"));
            _buttonStyles.Add(new Sprite("buttons/xbox/selectButton"));
            _buttonStyles.Add(new Sprite("buttons/xbox/dPadLeft"));
            _buttonStyles.Add(new Sprite("buttons/xbox/dPadRight"));
            _buttonStyles.Add(new Sprite("buttons/xbox/dPadUp"));
            _buttonStyles.Add(new Sprite("buttons/xbox/dPadDown"));
            _buttonStyles.Add(new Sprite("buttons/xbox/leftBumper"));
            _buttonStyles.Add(new Sprite("buttons/xbox/rightBumper"));
            _buttonStyles.Add(new Sprite("buttons/xbox/leftTrigger"));
            _buttonStyles.Add(new Sprite("buttons/xbox/rightTrigger"));
            _buttonStyles.Add(new Sprite("buttons/xbox/leftStick"));
            _buttonStyles.Add(new Sprite("buttons/xbox/rightStick"));
            _buttonStyles.Add(new Sprite("buttons/playstation/o"));
            _buttonStyles.Add(new Sprite("buttons/playstation/square"));
            _buttonStyles.Add(new Sprite("buttons/playstation/triangle"));
            _buttonStyles.Add(new Sprite("buttons/playstation/x"));
            _buttonStyles.Add(new Sprite("buttons/playstation/startButton"));
            _buttonStyles.Add(new Sprite("buttons/playstation/selectButton"));
            _buttonStyles.Add(new Sprite("buttons/playstation/leftBumper"));
            _buttonStyles.Add(new Sprite("buttons/playstation/rightBumper"));
            _buttonStyles.Add(new Sprite("buttons/playstation/leftTrigger"));
            _buttonStyles.Add(new Sprite("buttons/playstation/rightTrigger"));
            _buttonStyles.Add(new Sprite("buttons/SNES/a"));
            _buttonStyles.Add(new Sprite("buttons/SNES/b"));
            _buttonStyles.Add(new Sprite("buttons/SNES/x"));
            _buttonStyles.Add(new Sprite("buttons/SNES/y"));
            _buttonStyles.Add(new Sprite("buttons/SNES/aFami"));
            _buttonStyles.Add(new Sprite("buttons/SNES/bFami"));
            _buttonStyles.Add(new Sprite("buttons/SNES/xFami"));
            _buttonStyles.Add(new Sprite("buttons/SNES/yFami"));
            _buttonStyles.Add(new Sprite("buttons/SNES/startButton"));
            _buttonStyles.Add(new Sprite("buttons/SNES/selectButton"));
            _buttonStyles.Add(new Sprite("buttons/SNES/leftTrigger"));
            _buttonStyles.Add(new Sprite("buttons/SNES/rightTrigger"));
            _buttonStyles.Add(new Sprite("buttons/genesis/a"));
            _buttonStyles.Add(new Sprite("buttons/genesis/b"));
            _buttonStyles.Add(new Sprite("buttons/genesis/c"));
            _buttonStyles.Add(new Sprite("buttons/genesis/start"));
            _buttonStyles.Add(new Sprite("buttons/playstation/blank"));
            _buttonStyles.Add(new Sprite("buttons/genericButton"));
        }

        public static void InitDefaultProfiles()
        {
            for (int index = 0; index < DG.MaxPlayers; ++index)
            {
                InputProfile inputProfile = InputProfile.Add("MPPlayer" + (index + 1).ToString());
                inputProfile.mpIndex = index;
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Left, 4);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Right, 8);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Up, 1);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Down, 2);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Jump, 4096);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Shoot, 16384);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Grab, 32768);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Quack, 8192);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Start, 16);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Strafe, 256);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Ragdoll, 512);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.LeftTrigger, 8388608);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.RightTrigger, 4194304);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Select, 4096);
                inputProfile.Map(GetDevice<GenericController>(index), Triggers.Cancel, 8192);
                if (index == 0)
                    InputProfile.active = inputProfile;
            }
            ApplyDefaultMappings();
            InputProfile.Add("Blank");
        }
        public static void Initialize()
        {
            FNAPlatform.DeviceChangeEvent += OnDeviceChange;
            MonoMain.NloadMessage = "Initializing Input System...";
            DevConsole.Log(DCSection.General, "Initializing Input...");
            foreach (DeviceInputMapping inputMappingPreset in _defaultInputMappingPresets)
                _defaultInputMapping.Add(inputMappingPreset.Clone());
            InputDevice device = new Keyboard("KEYBOARD P1", 0);
            _devices.Add(device);
            InputDevice inputDevice1 = new Keyboard("KEYBOARD P2", 1);
            _devices.Add(inputDevice1);
            InputDevice inputDevice2 = new Mouse();
            _devices.Add(inputDevice2);

            for (int index = 0; index < MonoMain.MaximumGamepadCount; index++)
            {
                XInputPad XInputDevice = new XInputPad(index);
                _devices.Add(XInputDevice);
                XInputDevice.InitializeState();
            }
            GenericController genericController1 = new GenericController(0);
            _gamePads.Add(genericController1);
            GenericController genericController2 = new GenericController(1);
            _gamePads.Add(genericController2);
            GenericController genericController3 = new GenericController(2);
            _gamePads.Add(genericController3);
            GenericController genericController4 = new GenericController(3);
            _gamePads.Add(genericController4);
            GenericController genericController5 = new GenericController(4);
            _gamePads.Add(genericController5);
            GenericController genericController6 = new GenericController(5);
            _gamePads.Add(genericController6);
            GenericController genericController7 = new GenericController(6);
            _gamePads.Add(genericController7);
            GenericController genericController8 = new GenericController(7);
            _gamePads.Add(genericController8);
            InputProfile.Default = new InputProfile("Default");
            //DuckGame.Input.InitializeDInputAsync();
            _devices.Add(genericController1);
            _devices.Add(genericController2);
            _devices.Add(genericController3);
            _devices.Add(genericController4);
            _devices.Add(genericController5);
            _devices.Add(genericController6);
            _devices.Add(genericController7);
            _devices.Add(genericController8);
            InputProfile.Default.Map(device, Triggers.Left, 37);
            InputProfile.Default.Map(device, Triggers.Right, 39);
            InputProfile.Default.Map(device, Triggers.Up, 38);
            InputProfile.Default.Map(device, Triggers.Down, 40);
            InputProfile.Default.Map(GetDevice<XInputPad>(), Triggers.Left, 4);
            InputProfile.Default.Map(GetDevice<XInputPad>(), Triggers.Right, 8);
            InputProfile.Default.Map(GetDevice<XInputPad>(), Triggers.Up, 1);
            InputProfile.Default.Map(GetDevice<XInputPad>(), Triggers.Down, 2);
            _profiles[InputProfile.Default.name] = InputProfile.Default;
            EnumerateGamepads();
            InitDefaultProfiles();
            string str = DuckFile.optionsDirectory + "/input.dat";
            if (MonoMain.defaultControls)
            {
                DevConsole.Log(DCSection.General, "Clearing input settings (MonoMain.defaultControls == true)");
                DuckFile.Delete(str);
            }
            else
            {
                if (!DuckFile.FileExists(str) && DGSave.upgradingFromVanilla || MonoMain.oldDefaultControls)
                {
                    DevConsole.Log(DCSection.General, "Saving old input defaults...");
                    foreach (DeviceInputMapping oldInputDefault in _oldInputDefaults)
                        SetDefaultMapping(oldInputDefault);
                    Save();
                }
                DuckXML duckXml = DuckFile.LoadDuckXML(str);
                if (duckXml == null)
                    return;
                IEnumerable<DXMLNode> source = duckXml.Elements("Mappings");
                if (source == null)
                    return;
                foreach (DXMLNode element in source.Elements())
                {
                    if (element.Name == "InputMapping")
                    {
                        DeviceInputMapping mapping = new DeviceInputMapping();
                        mapping.Deserialize(element);
                        SetDefaultMapping(mapping);
                    }
                }
            }
        }

        private static void InitializeDInputAsync()
        {
            //++DuckGame.Input._dinputInitTimesCalled;
            //if (DuckGame.Input._dinputInitException != null)
            //{
            //    DevConsole.Log(DCSection.General, "DInput Initialization failed with exception: " + DuckGame.Input._dinputInitException.Message);
            //    DevConsole.Log(DCSection.General, "DInput has been disabled.");
            //    DuckGame.Input._dinputEnabled = false;
            //    DuckGame.Input._dinputInitException = null;
            //}
            //else
            //{
            //    switch (DuckGame.Input._dinputInitializeStatus)
            //    {
            //        case int.MinValue:
            //            DuckGame.Input._dinputInitializeStatus = int.MaxValue;
            //            if (MonoMain.disableDirectInput)
            //            {
            //                DuckGame.Input._dinputEnabled = false;
            //                DevConsole.Log(DCSection.General, "MonoMain.disableDirectInput is true, skipping DInput initialize.");
            //                return;
            //            }
            //            DevConsole.Log(DCSection.General, "Starting DInput Async Init...");
            //            Task.Run(() =>
            //           {
            //               try
            //               {
            //                   DuckGame.Input._dinputInitializeStatus = DInput.Initialize();
            //               }
            //               catch (Exception ex)
            //               {
            //                   DuckGame.Input._dinputInitException = ex;
            //               }
            //           });
            //            return;
            //        case 0:
            //            InputDevice inputDevice1 = new DInputPad(0);
            //            DuckGame.Input._devices.Add(inputDevice1);
            //            InputDevice inputDevice2 = new DInputPad(1);
            //            DuckGame.Input._devices.Add(inputDevice2);
            //            InputDevice inputDevice3 = new DInputPad(2);
            //            DuckGame.Input._devices.Add(inputDevice3);
            //            InputDevice inputDevice4 = new DInputPad(3);
            //            DuckGame.Input._devices.Add(inputDevice4);
            //            InputDevice inputDevice5 = new DInputPad(4);
            //            DuckGame.Input._devices.Add(inputDevice5);
            //            InputDevice inputDevice6 = new DInputPad(5);
            //            DuckGame.Input._devices.Add(inputDevice6);
            //            InputDevice inputDevice7 = new DInputPad(6);
            //            DuckGame.Input._devices.Add(inputDevice7);
            //            InputDevice inputDevice8 = new DInputPad(7);
            //            DuckGame.Input._devices.Add(inputDevice8);
            //            if (DuckGame.Input._dinputInitTimesCalled < 60)
            //                DuckGame.Input._suppressInputChangeMessages = 300;
            //            DuckGame.Input._dinputEnabled = true;
            //            DuckGame.Input.devicesChanged = true;
            //            break;
            //        case int.MaxValue:
            //            return;
            //        default:
            //            if (MonoMain.disableDirectInput)
            //                DevConsole.Log(DCSection.General, "MonoMain.disableDirectInput was true, DInput has been disabled.");
            //            else
            //                DevConsole.Log(DCSection.General, "DInput.Initialize() failed with code " + DuckGame.Input._dinputInitializeStatus.ToString() + ". DInput has been disabled.");
            //            DuckGame.Input._dinputEnabled = false;
            //            break;
            //    }
            //    DuckGame.Input._dinputInitializeStatus = int.MaxValue;
            //}
        }

        public static T GetDevice<T>(int index = 0) where T : InputDevice
        {
            Type type = typeof(T);
            foreach (InputDevice device in _devices)
            {
                if (type.IsAssignableFrom(device.GetType()) && device.index == index)
                    return device as T;
            }
            return default(T);
        }

        public static InputDevice GetDevice(string name)
        {
            foreach (InputDevice device in _devices)
            {
                if (device.name == name)
                    return device;
            }
            return null;
        }
        public static void OnDeviceChange(int dev, bool removed)
        {
            devicesChanged = true;
        }
        private static void CheckDInputChanges()
        {
            //foreach (GenericController gamePad in DuckGame.Input._gamePads)
            //{
            //    InputDevice device = gamePad.device;
            //    if (device is DInputPad)
            //    {
            //        DInputPad dinputPad = device as DInputPad;
            //        if (dinputPad.isConnected && !dinputPad.prevIsConnected)
            //        {
            //            DuckGame.Input._gamepadsChanged = true;
            //            DuckGame.Input._changePluggedIn = true;
            //            DuckGame.Input._changeName = gamePad.device.productName;
            //            DuckGame.Input._padConnectionChange = true;
            //        }
            //        if (!dinputPad.isConnected && dinputPad.prevIsConnected)
            //        {
            //            DuckGame.Input._gamepadsChanged = true;
            //            DuckGame.Input._changePluggedIn = false;
            //            DuckGame.Input._changeName = gamePad.device.productName;
            //            DuckGame.Input._padConnectionChange = true;
            //            gamePad.device = null;
            //        }
            //        dinputPad.prevIsConnected = dinputPad.isConnected;
            //    }
            //}
        }

        public static void InvalidateDirectInputDeviceIndex(int pIndex)
        {
        }

        private static void RunGamepadEnumerationThread()
        {
            //if (DuckGame.Input.enumeratingGamepads)
            //    return;
            //DuckGame.Input.enumeratingGamepads = true;
            //--DuckGame.Input.timesToEnumerateGamepads;
            //Task.Run(() => DInput.Thread_EnumGamepads());
        }
        public static bool ForceDirectInputMode()
        {
            if (MonoMain.disableDirectInput)
            {
                return false;
            }
            //return DInput.ForceDirectInputMode();
            return false;
        }
        public static bool DInputUpdate()
        {
            if (MonoMain.disableDirectInput)
            {
                return false;
            }
            //return DInput.Update();
            return false;
        }
        public static void EnumerateGamepads()
        {
            //if (Program.IsLinuxD)//FIX ME LATER DAN PLLZ
            //{
            //    return;
            //}
            foreach (GenericController gamePad in _gamePads)
            {
                InputDevice device1 = gamePad.device;
                //if (device1 is XInputPad && Input.ForceDirectInputMode())
                //{
                //    continue;
                //}
                //if (device1 is DInputPad && DuckGame.Input._dinputEnabled)
                //{
                //    DInputPad dinputPad = device1 as DInputPad;
                //    if (!dinputPad.isXInput || Input.ForceDirectInputMode())
                //    {
                //        if (dinputPad.isConnected != dinputPad.prevIsConnected)
                //        {
                //            DuckGame.Input._gamepadsChanged = true;
                //            DuckGame.Input._changePluggedIn = dinputPad.isConnected;
                //            DuckGame.Input._changeName = gamePad.device.productName;
                //            DuckGame.Input._padConnectionChange = true;
                //            if (!dinputPad.isConnected)
                //                gamePad.device = null;
                //        }
                //        dinputPad.prevIsConnected = dinputPad.isConnected;
                //    }
                //    else
                //        continue;
                //}
                //else
                if (device1 != null && !device1.isConnected)
                {
                    _changePluggedIn = false;
                    _changeName = device1.productName;
                    _padConnectionChange = true;
                    _gamepadsChanged = true;
                    gamePad.device = null;
                }
                if (gamePad.device == null)
                {
                    foreach (InputDevice device2 in _devices)
                    {
                        if (!(device2 is GenericController) && device2.isConnected && device2.genericController == null)
                        {
                            if (device2 is XInputPad) // && !Input.ForceDirectInputMode()
                            {
                                gamePad.device = device2 as AnalogGamePad;
                                _gamepadsChanged = true;
                                _changePluggedIn = true;
                                _changeName = gamePad.device.productName;
                                _padConnectionChange = true;
                                break;
                            }
                            //if (device2 is DInputPad && DuckGame.Input._dinputEnabled)
                            //{
                            //    gamePad.device = device2 as AnalogGamePad;
                            //    DuckGame.Input._gamepadsChanged = true;
                            //    DuckGame.Input._changePluggedIn = true;
                            //    DuckGame.Input._changeName = gamePad.device.productName;
                            //    if (device2.productName == "Wireless Controller")
                            //    {
                            //        DuckGame.Input.mightHavePlaystationController = true;
                            //        break;
                            //    }
                            //    break;
                            //}
                        }
                    }
                }
            }
        }

        public static void Update()
        {
            try
            {
                bool notlinux = !(Program.IsLinuxD || Program.isLinux);
                if (notlinux && !_initializedMessageHook)
                {
                    InputSystem.Initialize(MonoMain.instance.Window);
                    _initializedMessageHook = true;
                }
                if (notlinux && Options.Data.imeSupport && !_initializedIME)
                {
                    InputSystem.InitializeIme(MonoMain.instance.Window);
                    InputSystem.IMECharEntered += new CharEnteredHandler(Keyboard.IMECharEnteredHandler);
                    _initializedIME = true;
                }
                //InputSystem.CharEntered += new CharEnteredHandler(Keyboard.ALTCharEnteredHandler); removed because it does nothing on target platforms
                bool flag = Options.Data.imeSupport && _imeAllowed;
                if (notlinux && flag != _prevImeAllowed)
                {
                    if (flag)
                        InputSystem.StartIME();
                    else
                        InputSystem.EndIME();
                }
                //if (!MonoMain.disableDirectInput && !DuckGame.Input._dinputEnabled)
                //DuckGame.Input.InitializeDInputAsync();
                _prevImeAllowed = flag;
                _imeAllowed = false;
                //if (DuckGame.Input._prevForceMode != Input.ForceDirectInputMode())
                //{
                //    foreach (InputDevice device in DuckGame.Input._devices)
                //    {
                //        if (device is GenericController)
                //            (device as GenericController).device = null;
                //        DuckGame.Input.devicesChanged = true;
                //    }
                //    DuckGame.Input._prevForceMode = Input.ForceDirectInputMode();
                //}
                if (devicesChanged)
                {
                    ++_deviceUpdateWait;
                    if (_deviceUpdateWait > 90) // 1.5 second instead of the old 2 seconds
                    {
                        _deviceUpdateWait = 0;
                        devicesChanged = false;
                        ++timesToEnumerateGamepads;
                        //if (!DuckGame.Input._dinputEnabled)
                        EnumerateGamepads();
                    }
                }
                //if (DuckGame.Input._dinputEnabled && DuckGame.Input.timesToEnumerateGamepads > 0 && !DuckGame.Input.enumeratingGamepads)
                //    DuckGame.Input.RunGamepadEnumerationThread();
                if (_gamepadsChanged)
                {
                    ApplyDefaultMappings();
                    TeamSelect2.ControllerLayoutsChanged();
                    _gamepadsChanged = false;
                    uiDevicesHaveChanged = true;
                }
                if (_updateWaitFrames > 0)
                {
                    --_updateWaitFrames;
                }
                else
                {
                    //if (DuckGame.Input._dinputEnabled)
                    //{
                    //    if (DInputUpdate())
                    //    {
                    //        DuckGame.Input.enumeratingGamepads = false;
                    //        DuckGame.Input.EnumerateGamepads();
                    //    }
                    //    DuckGame.Input.CheckDInputChanges();
                    //}
                    if (_padConnectionChange)
                    {
                        _padConnectionChange = false;
                        if (MonoMain.started && !_ignoreFirstInputChange && _suppressInputChangeMessages <= 0)
                        {
                            _changeName = _changeName.Trim();
                            if (_changeName.Length > 25)
                                _changeName = _changeName.Substring(0, 25) + "...";
                            string str = "@PLUG@|LIME|";
                            if (!_changePluggedIn)
                                str = "@UNPLUG@|RED|";
                            HUD.AddInputChangeDisplay(str + _changeName);
                        }
                    }
                    foreach (InputDevice device in _devices)
                        device.Update();
                }
                if (MonoMain.started)
                    _ignoreFirstInputChange = false;
                if (_suppressInputChangeMessages <= 0)
                    return;
                --_suppressInputChangeMessages;
            }
            catch (Exception e)
            {
                DevConsole.Log(e.Message);
            }
        }

        public static void Terminate()
        {
            if (_gamepadThread != null)
                _gamepadThread.Abort();
            _gamepadThread = null;
            InputSystem.Terminate();
            for (int index = 0; index < MonoMain.MaximumGamepadCount; index++)
            {
                GamePadState state = FNAPlatform.GetGamePadState(index, GamePadDeadZone.IndependentAxes);
                if (state.IsConnected)
                    FNAPlatform.SetGamePadVibration(index, 0f, 0f);
            }
        }

        public static bool CheckCode(InputCode code)
        {
            foreach (KeyValuePair<string, InputProfile> profile in InputProfile.profiles)
            {
                if (profile.Value.virtualDevice == null && profile.Value.CheckCode(code))
                    return true;
            }
            return false;
        }

        public static bool Pressed(string trigger, string profile = "Any")
        {
            if (profile == "Any")
            {
                foreach (KeyValuePair<string, InputProfile> profile1 in InputProfile.profiles)
                {
                    if (profile1.Value.virtualDevice == null && profile1.Value.Pressed(trigger))
                        return true;
                }
                return false;
            }
            InputProfile inputProfile;
            return _profiles.TryGetValue(profile, out inputProfile) && inputProfile.Pressed(trigger);
        }

        public static bool Released(string trigger, string profile = "Any")
        {
            if (profile == "Any")
            {
                foreach (KeyValuePair<string, InputProfile> profile1 in InputProfile.profiles)
                {
                    if (profile1.Value.Released(trigger))
                        return true;
                }
                return false;
            }
            InputProfile inputProfile;
            return _profiles.TryGetValue(profile, out inputProfile) && inputProfile.Released(trigger);
        }

        public static bool Down(string trigger, string profile = "Any")
        {
            if (profile == "Any")
            {
                foreach (KeyValuePair<string, InputProfile> profile1 in InputProfile.profiles)
                {
                    if (profile1.Value.Down(trigger))
                        return true;
                }
                return false;
            }
            InputProfile inputProfile;
            return _profiles.TryGetValue(profile, out inputProfile) && inputProfile.Down(trigger);
        }
    }
}
