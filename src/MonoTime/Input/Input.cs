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
using System.Threading.Tasks;

namespace DuckGame
{
    public class Input
    {
        public static InputCode konamiCode = new InputCode()
        {
            triggers = new List<string>()
      {
        "UP",
        "UP",
        "DOWN",
        "DOWN",
        "LEFT",
        "RIGHT",
        "LEFT",
        "RIGHT",
        "QUACK",
        "JUMP"
      }
        };
        public static InputCode konamiCodeAlternate = new InputCode()
        {
            triggers = new List<string>()
      {
        "UP|JUMP",
        "UP|JUMP",
        "DOWN",
        "DOWN",
        "LEFT",
        "RIGHT",
        "LEFT",
        "RIGHT",
        "QUACK",
        "UP|JUMP"
      }
        };
        public static InputCode hookCode = new InputCode()
        {
            triggers = new List<string>()
      {
        "JUMP",
        "QUACK",
        "RAGDOLL",
        "RAGDOLL",
        "GRAB"
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
        private static Dictionary<string, Sprite> _triggerImageMap = new Dictionary<string, Sprite>();
        private static List<GenericController> _gamePads = new List<GenericController>();
        private static Array _keys = Enum.GetValues(typeof(Keys));
        private static List<DeviceInputMapping> _defaultInputMapping = new List<DeviceInputMapping>();
        private static List<DeviceInputMapping> _oldInputDefaults = new List<DeviceInputMapping>()
    {
      new DeviceInputMapping()
      {
        deviceName = "KEYBOARD P1",
        deviceGUID = "",
        map = new Dictionary<string, int>()
        {
          {
            "LEFT",
            65
          },
          {
            "RIGHT",
            68
          },
          {
            "UP",
            87
          },
          {
            "DOWN",
            83
          },
          {
            "JUMP",
            87
          },
          {
            "SHOOT",
            86
          },
          {
            "GRAB",
            67
          },
          {
            "START",
            27
          },
          {
            "RAGDOLL",
            81
          },
          {
            "STRAFE",
            66
          },
          {
            "QUACK",
            69
          },
          {
            "SELECT",
            32
          },
          {
            "CHAT",
            13
          }
        }
      },
      new DeviceInputMapping()
      {
        deviceName = "KEYBOARD P2",
        deviceGUID = "",
        map = new Dictionary<string, int>()
        {
          {
            "LEFT",
            37
          },
          {
            "RIGHT",
            39
          },
          {
            "UP",
            38
          },
          {
            "DOWN",
            40
          },
          {
            "JUMP",
            38
          },
          {
            "SHOOT",
            186
          },
          {
            "GRAB",
            76
          },
          {
            "START",
            187
          },
          {
            "RAGDOLL",
            73
          },
          {
            "STRAFE",
            75
          },
          {
            "QUACK",
            79
          },
          {
            "SELECT",
            161
          }
        }
      }
    };
        public static List<DeviceInputMapping> _defaultInputMappingPresets = new List<DeviceInputMapping>()
    {
      new DeviceInputMapping()
      {
        deviceName = "KEYBOARD P1",
        deviceGUID = "",
        map = new Dictionary<string, int>()
        {
          {
            "LEFT",
            65
          },
          {
            "RIGHT",
            68
          },
          {
            "UP",
            87
          },
          {
            "DOWN",
            83
          },
          {
            "JUMP",
            32
          },
          {
            "SHOOT",
            72
          },
          {
            "GRAB",
            71
          },
          {
            "START",
            27
          },
          {
            "RAGDOLL",
            70
          },
          {
            "STRAFE",
            160
          },
          {
            "QUACK",
            69
          },
          {
            "SELECT",
            32
          },
          {
            "CHAT",
            13
          },
          {
            "CANCEL",
            69
          },
          {
            "MENU1",
            72
          },
          {
            "MENU2",
            81
          },
          {
            "MENULEFT",
            65
          },
          {
            "MENURIGHT",
            68
          },
          {
            "MENUUP",
            87
          },
          {
            "MENUDOWN",
            83
          },
          {
            "RSTICK",
            9
          },
          {
            "VOICEREG",
            36
          },
          {
            "KBDF",
            70
          }
        }
      },
      new DeviceInputMapping()
      {
        deviceName = "KEYBOARD P2",
        deviceGUID = "",
        map = new Dictionary<string, int>()
        {
          {
            "LEFT",
            37
          },
          {
            "RIGHT",
            39
          },
          {
            "UP",
            38
          },
          {
            "DOWN",
            40
          },
          {
            "JUMP",
            163
          },
          {
            "SHOOT",
            222
          },
          {
            "GRAB",
            186
          },
          {
            "START",
            187
          },
          {
            "RAGDOLL",
            79
          },
          {
            "STRAFE",
            76
          },
          {
            "QUACK",
            80
          },
          {
            "SELECT",
            161
          },
          {
            "CANCEL",
            80
          },
          {
            "MENU1",
            222
          },
          {
            "MENU2",
            186
          },
          {
            "MENULEFT",
            37
          },
          {
            "MENURIGHT",
            39
          },
          {
            "MENUUP",
            38
          },
          {
            "MENUDOWN",
            40
          },
          {
            "RSTICK",
            9
          }
        }
      },
      new DeviceInputMapping()
      {
        deviceName = "XBOX GAMEPAD",
        deviceGUID = "",
        map = new Dictionary<string, int>()
        {
          {
            "LEFT",
            4
          },
          {
            "RIGHT",
            8
          },
          {
            "UP",
            1
          },
          {
            "DOWN",
            2
          },
          {
            "JUMP",
            4096
          },
          {
            "SHOOT",
            16384
          },
          {
            "GRAB",
            32768
          },
          {
            "START",
            16
          },
          {
            "RAGDOLL",
            512
          },
          {
            "STRAFE",
            256
          },
          {
            "QUACK",
            8192
          },
          {
            "SELECT",
            4096
          },
          {
            "LTRIGGER",
            8388608
          },
          {
            "RTRIGGER",
            4194304
          },
          {
            "LBUMPER",
            256
          },
          {
            "RBUMPER",
            512
          },
          {
            "LSTICK",
            64
          },
          {
            "RSTICK",
            128
          },
          {
            "CANCEL",
            8192
          },
          {
            "LOPTION",
            32
          },
          {
            "MENU1",
            16384
          },
          {
            "MENU2",
            32768
          },
          {
            "MENULEFT",
            4
          },
          {
            "MENURIGHT",
            8
          },
          {
            "MENUUP",
            1
          },
          {
            "MENUDOWN",
            2
          }
        }
      },
      new DeviceInputMapping()
      {
        deviceName = "GENERIC GAMEPAD",
        deviceGUID = "",
        map = new Dictionary<string, int>()
        {
          {
            "LEFT",
            4
          },
          {
            "RIGHT",
            8
          },
          {
            "UP",
            1
          },
          {
            "DOWN",
            2
          },
          {
            "JUMP",
            4096
          },
          {
            "SHOOT",
            16384
          },
          {
            "GRAB",
            32768
          },
          {
            "START",
            16
          },
          {
            "RAGDOLL",
            512
          },
          {
            "STRAFE",
            256
          },
          {
            "QUACK",
            8192
          },
          {
            "SELECT",
            4096
          },
          {
            "LTRIGGER",
            8388608
          },
          {
            "RTRIGGER",
            4194304
          },
          {
            "LBUMPER",
            256
          },
          {
            "RBUMPER",
            512
          },
          {
            "LSTICK",
            64
          },
          {
            "RSTICK",
            128
          },
          {
            "CANCEL",
            8192
          },
          {
            "LOPTION",
            32
          },
          {
            "MENU1",
            16384
          },
          {
            "MENU2",
            32768
          },
          {
            "MENULEFT",
            4
          },
          {
            "MENURIGHT",
            8
          },
          {
            "MENUUP",
            1
          },
          {
            "MENUDOWN",
            2
          }
        }
      }
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
                return !Input.debuggerInputOverride && (!Graphics.inFocus || Input._ignoreInput);
            }
            set
            {
                Input._ignoreInput = value;
            }
        }

        public static List<Sprite> buttonStyles => DuckGame.Input._buttonStyles;

        public static Sprite GetTriggerSprite(string trigger)
        {
            Sprite triggerSprite;
            DuckGame.Input._triggerImageMap.TryGetValue(trigger, out triggerSprite);
            return triggerSprite;
        }

        public static List<InputDevice> GetInputDevices() => DuckGame.Input._devices;

        public static void Save()
        {
            DevConsole.Log(DCSection.General, "Input.Save()...");
            DuckXML doc = new DuckXML();
            DXMLNode node = new DXMLNode("Mappings");
            foreach (DeviceInputMapping deviceInputMapping in DuckGame.Input._defaultInputMapping)
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
                foreach (DeviceInputMapping inputMappingPreset in DuckGame.Input._defaultInputMappingPresets)
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
                return DuckGame.Input.GetDefaultMapping(productName, productGUID, presets, makeClone, p.linkedProfile);
            List<DeviceInputMapping> source = DuckGame.Input._defaultInputMapping;
            if (p != null && p.inputMappingOverrides.FirstOrDefault<DeviceInputMapping>(x => x.deviceGUID == productGUID && x.deviceName == productName) == null)
                p = null;
            if (presets)
                source = DuckGame.Input.defaultInputMappingPresets;
            if (p != null)
                source = p.inputMappingOverrides;
            foreach (DeviceInputMapping defaultMapping in source)
            {
                if (defaultMapping.deviceName == productName && defaultMapping.deviceGUID == productGUID)
                    return defaultMapping;
            }
            if (p != null)
                return null;
            DeviceInputMapping defaultMapping1 = source.FirstOrDefault<DeviceInputMapping>(x => x.deviceName == "GENERIC GAMEPAD");
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
            DeviceInputMapping defaultMapping = DuckGame.Input.GetDefaultMapping(pMapping.deviceName, pMapping.deviceGUID);
            return defaultMapping != null && defaultMapping.IsEqual(pMapping);
        }

        public static void SetDefaultMapping(DeviceInputMapping mapping, Profile overrideProfile = null)
        {
            if (overrideProfile != null && DuckGame.Input.MappingIsDefault(mapping))
                return;
            DuckGame.Input.TryFallback("MENULEFT", "LEFT", mapping);
            DuckGame.Input.TryFallback("MENURIGHT", "RIGHT", mapping);
            DuckGame.Input.TryFallback("MENUUP", "UP", mapping);
            DuckGame.Input.TryFallback("MENUDOWN", "DOWN", mapping);
            DuckGame.Input.TryFallback("MENU1", "SHOOT", mapping);
            DuckGame.Input.TryFallback("MENU2", "GRAB", mapping);
            DuckGame.Input.TryFallback("CANCEL", "QUACK", mapping);
            DeviceInputMapping defaultMapping = DuckGame.Input.GetDefaultMapping(mapping.deviceName, mapping.deviceGUID);
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
            List<DeviceInputMapping> source = DuckGame.Input._defaultInputMapping;
            if (overrideProfile != null)
                source = overrideProfile.inputMappingOverrides;
            DeviceInputMapping deviceInputMapping1 = source.FirstOrDefault<DeviceInputMapping>(x => x.deviceName == mapping.deviceName && x.deviceGUID == mapping.deviceGUID);
            DeviceInputMapping deviceInputMapping2 = DuckGame.Input.defaultInputMappingPresets.FirstOrDefault<DeviceInputMapping>(x => x.deviceName == mapping.deviceName && x.deviceGUID == mapping.deviceGUID);
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
                DeviceInputMapping compare = DuckGame.Input._defaultInputMapping.FirstOrDefault<DeviceInputMapping>(x => x.deviceName == mapping.deviceName && x.deviceGUID == mapping.deviceGUID);
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
                    if (DuckGame.Input._defaultInputMapping.FirstOrDefault<DeviceInputMapping>(x => x.deviceName == "GENERIC GAMEPAD").IsEqual(mapping))
                        return;
                    source.Add(mapping);
                }
            }
        }

        public static List<DeviceInputMapping> CloneDefaultMappings()
        {
            List<DeviceInputMapping> deviceInputMappingList = new List<DeviceInputMapping>();
            foreach (DeviceInputMapping deviceInputMapping in DuckGame.Input._defaultInputMapping)
                deviceInputMappingList.Add(deviceInputMapping.Clone());
            return deviceInputMappingList;
        }

        public static void SetDefaultMappings(List<DeviceInputMapping> mappings) => DuckGame.Input._defaultInputMapping = mappings;

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
                            DuckGame.Input.ApplyDefaultMapping(defaultProfile, duckProfile);
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                    DuckGame.Input.ApplyDefaultMapping(defaultProfile);
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
                        DeviceInputMapping deviceInputMapping = DuckGame.Input.GetDefaultMapping(device.device.productName, device.device.productGUID, p: p1) ?? DuckGame.Input.GetDefaultMapping(device.device.productName, device.device.productGUID);
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
                    DeviceInputMapping deviceInputMapping = DuckGame.Input.GetDefaultMapping("KEYBOARD P1", "", p: duckProfile) ?? DuckGame.Input.GetDefaultMapping("KEYBOARD P1", "");
                    if (deviceInputMapping == null)
                        return;
                    foreach (KeyValuePair<string, int> keyValuePair in deviceInputMapping.map)
                        p.Map(DuckGame.Input.GetDevice<Keyboard>(), keyValuePair.Key, keyValuePair.Value);
                }
                else
                {
                    if (p != InputProfile.defaultProfiles[Options.Data.keyboard2PlayerIndex])
                        return;
                    DeviceInputMapping deviceInputMapping = DuckGame.Input.GetDefaultMapping("KEYBOARD P2", "", p: duckProfile) ?? DuckGame.Input.GetDefaultMapping("KEYBOARD P2", "");
                    if (deviceInputMapping == null)
                        return;
                    foreach (KeyValuePair<string, int> keyValuePair in deviceInputMapping.map)
                        p.Map(DuckGame.Input.GetDevice<Keyboard>(1), keyValuePair.Key, keyValuePair.Value);
                }
            }
        }

        public static void InitializeGraphics()
        {
            MonoMain.loadMessage = "Loading Input";
            foreach (Keys key in DuckGame.Input._keys)
            {
                char ch = KeyHelper.KeyToChar(key);
                if (ch > ' ' && ch < '\u007F')
                    DuckGame.Input.keyToChar[key] = ch;
            }
            DuckGame.Input._triggerImageMap.Add("MOUSEWHEEL", new Sprite("buttons/mousewheel"));
            DuckGame.Input._triggerImageMap.Add("PLANET", new Sprite("smallEarth"));
            DuckGame.Input._triggerImageMap.Add("ARENA", new Sprite("smallArena"));
            DuckGame.Input._triggerImageMap.Add("MOON", new Sprite("smallMoon"));
            DuckGame.Input._triggerImageMap.Add("PLUG", new Sprite("plugRect"));
            DuckGame.Input._triggerImageMap.Add("UNPLUG", new Sprite("unplugRect"));
            DuckGame.Input._triggerImageMap.Add("CLIPCOPY", new Sprite("clipcopy"));
            DuckGame.Input._triggerImageMap.Add("SKIPICON", new Sprite("skipIcon"));
            DuckGame.Input._triggerImageMap.Add("SETTINGSCHANGED", new Sprite("wrenchRect"));
            DuckGame.Input._triggerImageMap.Add("NORMALICON", new Sprite("normalIcon"));
            DuckGame.Input._triggerImageMap.Add("RAINBOWICON", new Sprite("rainbowIcon"));
            DuckGame.Input._triggerImageMap.Add("CUSTOMICON", new Sprite("customIcon"));
            DuckGame.Input._triggerImageMap.Add("RANDOMICON", new Sprite("randomIcons"));
            DuckGame.Input._triggerImageMap.Add("ESCAPE", new Sprite("buttons/keyboard/escape"));
            DuckGame.Input._triggerImageMap.Add("CONSOLE", new Sprite("buttons/keyboard/tilde"));
            DuckGame.Input._triggerImageMap.Add("TINYLOCK", new Sprite("tinyLock"));
            DuckGame.Input._triggerImageMap.Add("RETICULE", new Sprite("challenge/reticule"));
            DuckGame.Input._triggerImageMap.Add("TICKET", new Sprite("arcade/ticket"));
            DuckGame.Input._triggerImageMap.Add("CHECK", new Sprite("checkIcon"));
            Dictionary<string, Sprite> triggerImageMap = DuckGame.Input._triggerImageMap;
            Sprite sprite1 = new Sprite("checkIcon")
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -1f
            };
            triggerImageMap.Add("CHECKSMALL", sprite1);
            DuckGame.Input._triggerImageMap.Add("SPEEDCLOCK", new Sprite("speedrunClock"));
            DuckGame.Input._triggerImageMap.Add("STARGOODY", new Sprite("challenge/star"));
            DuckGame.Input._triggerImageMap.Add("SUITCASEGOODY", new Sprite("challenge/suitcase"));
            DuckGame.Input._triggerImageMap.Add("LAPGOODY", new Sprite("challenge/goal"));
            DuckGame.Input._triggerImageMap.Add("EDITORCURRENCY", new Sprite("editorCurrency"));
            DuckGame.Input._triggerImageMap.Add("LWING", new Sprite("arcade/titleWing"));
            Sprite sprite2 = new Sprite("arcade/titleWing")
            {
                flipH = true
            };
            sprite2.centerx = sprite2.width;
            DuckGame.Input._triggerImageMap.Add("RWING", sprite2);
            Sprite sprite3 = new Sprite("arcade/titleWing")
            {
                color = new Color(96, 119, 124)
            };
            ++sprite3.centery;
            DuckGame.Input._triggerImageMap.Add("LWINGGRAY", sprite3);
            Sprite sprite4 = new Sprite("arcade/titleWing")
            {
                flipH = true
            };
            sprite4.centerx = sprite4.width;
            ++sprite4.centery;
            sprite4.color = new Color(96, 119, 124);
            DuckGame.Input._triggerImageMap.Add("RWINGGRAY", sprite4);
            DuckGame.Input._triggerImageMap.Add("WRENCH", new Sprite("titleWrench"));
            DuckGame.Input._triggerImageMap.Add("SCREWDRIVER", new Sprite("titleScrewdriver"));
            DuckGame.Input._triggerImageMap.Add("BASELINE", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 0
            });
            DuckGame.Input._triggerImageMap.Add("BRONZE", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 1
            });
            DuckGame.Input._triggerImageMap.Add("SILVER", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 2
            });
            DuckGame.Input._triggerImageMap.Add("GOLD", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 3
            });
            DuckGame.Input._triggerImageMap.Add("PLATINUM", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 4
            });
            DuckGame.Input._triggerImageMap.Add("DEVELOPER", new SpriteMap("challengeTrophyIcons", 16, 16)
            {
                frame = 5
            });
            DuckGame.Input._triggerImageMap.Add("ONLINEBAD", new SpriteMap("onlineStatusIcons", 7, 7)
            {
                frame = 0
            });
            DuckGame.Input._triggerImageMap.Add("ONLINENEUTRAL", new SpriteMap("onlineStatusIcons", 7, 7)
            {
                frame = 1
            });
            DuckGame.Input._triggerImageMap.Add("ONLINEGOOD", new SpriteMap("onlineStatusIcons", 7, 7)
            {
                frame = 2
            });
            Sprite sprite5 = new Sprite("crownIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite5.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("HOSTCROWN", sprite5);
            Sprite sprite6 = new Sprite("subPlus");
            DuckGame.Input._triggerImageMap.Add("SUBPLUS", sprite6);
            Sprite sprite7 = new Sprite("steamIcon")
            {
                scale = new Vec2(0.25f, 0.25f)
            };
            sprite7.centery -= 48f;
            DuckGame.Input._triggerImageMap.Add("STEAMICON", sprite7);
            Sprite sprite8 = new Sprite("steamIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite8.centery -= 16f;
            DuckGame.Input._triggerImageMap.Add("STEAMICONMED", sprite8);
            Sprite sprite9 = new Sprite("accessIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite9.centery -= 8f;
            DuckGame.Input._triggerImageMap.Add("ACCESSICON", sprite9);
            Sprite sprite10 = new Sprite("vanillaIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite10.centery -= 8f;
            DuckGame.Input._triggerImageMap.Add("VANILLAICON", sprite10);
            Sprite sprite11 = new Sprite("spectatorIcon");
            DuckGame.Input._triggerImageMap.Add("SPECTATOR", sprite11);
            Sprite sprite12 = new Sprite("spectatorIcon")
            {
                scale = new Vec2(2f)
            };
            DuckGame.Input._triggerImageMap.Add("SPECTATORBIG", sprite12);
            Sprite sprite13 = new Sprite("discordIcon")
            {
                scale = new Vec2(0.25f, 0.25f)
            };
            sprite13.centery -= 48f;
            DuckGame.Input._triggerImageMap.Add("DISCORDICON", sprite13);
            Sprite sprite14 = new Sprite("singleDuck")
            {
                scale = new Vec2(1f, 1f)
            };
            DuckGame.Input._triggerImageMap.Add("_!DUCKSPAWN", sprite14);
            Sprite sprite15 = new Sprite("skipSpin");
            DuckGame.Input._triggerImageMap.Add("SKIPSPIN", sprite15);
            Sprite sprite16 = new Sprite("exclamationMoji");
            DuckGame.Input._triggerImageMap.Add("error", sprite16);
            Sprite sprite17 = new Sprite("logEvent");
            DuckGame.Input._triggerImageMap.Add("LOGEVENT", sprite17);
            Sprite sprite18 = new Sprite("networkSent");
            DuckGame.Input._triggerImageMap.Add("sent", sprite18);
            Sprite sprite19 = new Sprite("networkReceived");
            DuckGame.Input._triggerImageMap.Add("received", sprite19);
            Sprite sprite20 = new Sprite("networkDisconnect");
            DuckGame.Input._triggerImageMap.Add("disconnect", sprite20);
            Sprite sprite21 = new Sprite("networkDrop");
            DuckGame.Input._triggerImageMap.Add("netdrop", sprite21);
            Sprite sprite22 = new Sprite("blacklistX");
            DuckGame.Input._triggerImageMap.Add("blacklist", sprite22);
            DuckGame.Input._triggerImageMap.Add("SIGNALDEAD", new SpriteMap("signal", 8, 5)
            {
                frame = 0
            });
            DuckGame.Input._triggerImageMap.Add("SIGNALBAD", new SpriteMap("signal", 8, 5)
            {
                frame = 1
            });
            DuckGame.Input._triggerImageMap.Add("SIGNALNORMAL", new SpriteMap("signal", 8, 5)
            {
                frame = 2
            });
            DuckGame.Input._triggerImageMap.Add("SIGNALGOOD", new SpriteMap("signal", 8, 5)
            {
                frame = 3
            });
            DuckGame.Input._triggerImageMap.Add("PLUSKEY", new KeyImage('+'));
            DuckGame.Input._triggerImageMap.Add("ENTERKEY", new Sprite("buttons/keyboard/enter"));
            DuckGame.Input._triggerImageMap.Add("ESCAPEKEY", new Sprite("buttons/keyboard/escape"));
            DuckGame.Input._triggerImageMap.Add("ICONGRADIENT", new Sprite("iconGradient"));
            SpriteMap spriteMap1 = new SpriteMap("chanceIcon", 10, 9)
            {
                frame = 0,
                centery = 1f
            };
            DuckGame.Input._triggerImageMap.Add("CHANCEICON", spriteMap1);
            SpriteMap spriteMap2 = new SpriteMap("iconEight", 8, 8);
            DuckGame.Input._triggerImageMap.Add("ICONEIGHT", spriteMap2);
            DuckGame.Input._triggerImageMap.Add("LEFTMOUSE", new SpriteMap("buttons/mouse", 12, 15)
            {
                frame = 0
            });
            DuckGame.Input._triggerImageMap.Add("MIDDLEMOUSE", new SpriteMap("buttons/mouse", 12, 15)
            {
                frame = 1
            });
            DuckGame.Input._triggerImageMap.Add("RIGHTMOUSE", new SpriteMap("buttons/mouse", 12, 15)
            {
                frame = 2
            });
            DuckGame.Input._triggerImageMap.Add("LOADICON", new SpriteMap("iconSheet", 16, 16)
            {
                frame = 1
            });
            DuckGame.Input._triggerImageMap.Add("SAVEICON", new SpriteMap("iconSheet", 16, 16)
            {
                frame = 2
            });
            SpriteMap spriteMap3 = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                frame = 1
            };
            DuckGame.Input._triggerImageMap.Add("LOADICONTINY", spriteMap3);
            DuckGame.Input._triggerImageMap.Add("LOCKEDFOLDERICON", new SpriteMap("iconSheet", 16, 16)
            {
                frame = 14
            });
            DuckGame.Input._triggerImageMap.Add("FOLDERICON", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 2
            });
            DuckGame.Input._triggerImageMap.Add("FOLDERDELETEICON", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 8
            });
            DuckGame.Input._triggerImageMap.Add("SELECTICON", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 3
            });
            DuckGame.Input._triggerImageMap.Add("DELETEFLAG_OFF", new SpriteMap("deleteFlag", 8, 8)
            {
                frame = 0
            });
            DuckGame.Input._triggerImageMap.Add("DELETEFLAG_ON", new SpriteMap("deleteFlag", 8, 8)
            {
                frame = 1
            });
            Sprite sprite23 = new Sprite("muteIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite23.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("MUTEICON", sprite23);
            Sprite sprite24 = new Sprite("blockIcon")
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            sprite24.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("BLOCKICON", sprite24);
            Sprite sprite25 = new Sprite("blockIcon")
            {
                scale = new Vec2(0.25f, 0.25f)
            };
            sprite25.centery -= 9f;
            DuckGame.Input._triggerImageMap.Add("BLOCKICONSMALL", sprite25);
            SpriteMap spriteMap4 = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                frame = 2
            };
            DuckGame.Input._triggerImageMap.Add("SAVEICONTINY", spriteMap4);
            SpriteMap spriteMap5 = new SpriteMap("iconSheet", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                frame = 0
            };
            DuckGame.Input._triggerImageMap.Add("NEWICONTINY", spriteMap5);
            DuckGame.Input._triggerImageMap.Add("RAINBOWTINY", new SpriteMap("tinyIcons", 8, 8)
            {
                frame = 6
            });
            SpriteMap spriteMap6 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 0
            };
            DuckGame.Input._triggerImageMap.Add("happyface", spriteMap6);
            SpriteMap spriteMap7 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 1
            };
            DuckGame.Input._triggerImageMap.Add("sadface", spriteMap7);
            SpriteMap spriteMap8 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 2
            };
            DuckGame.Input._triggerImageMap.Add("puffyface", spriteMap8);
            SpriteMap spriteMap9 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 3
            };
            DuckGame.Input._triggerImageMap.Add("angryface", spriteMap9);
            SpriteMap spriteMap10 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 4
            };
            DuckGame.Input._triggerImageMap.Add("yayface", spriteMap10);
            SpriteMap spriteMap11 = new SpriteMap("shrug", 78, 24)
            {
                scale = new Vec2(1f, 1f),
                centery = -1f
            };
            DuckGame.Input._triggerImageMap.Add("shrug", spriteMap11);
            SpriteMap spriteMap12 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 5
            };
            DuckGame.Input._triggerImageMap.Add("wowface", spriteMap12);
            SpriteMap spriteMap13 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 6
            };
            DuckGame.Input._triggerImageMap.Add("wtfface", spriteMap13);
            SpriteMap spriteMap14 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 7
            };
            DuckGame.Input._triggerImageMap.Add("straightface", spriteMap14);
            SpriteMap spriteMap15 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 8
            };
            DuckGame.Input._triggerImageMap.Add("oiface", spriteMap15);
            SpriteMap spriteMap16 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 9
            };
            DuckGame.Input._triggerImageMap.Add("blankface", spriteMap16);
            SpriteMap spriteMap17 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 10
            };
            DuckGame.Input._triggerImageMap.Add("sweatface", spriteMap17);
            SpriteMap spriteMap18 = new SpriteMap("moji", 11, 11)
            {
                scale = new Vec2(2f, 2f),
                centery = 0f,
                frame = 11
            };
            DuckGame.Input._triggerImageMap.Add("cryface", spriteMap18);
            Sprite sprite26 = new Sprite("cookedDuck")
            {
                scale = new Vec2(2f, 2f),
                centery = 2f
            };
            DuckGame.Input._triggerImageMap.Add("cooked", sprite26);
            Sprite sprite27 = new Sprite("grave")
            {
                scale = new Vec2(2f, 2f),
                centery = 0f
            };
            DuckGame.Input._triggerImageMap.Add("rip", sprite27);
            SpriteMap spriteMap19 = new SpriteMap("searchicon", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                centerx = -4f
            };
            DuckGame.Input._triggerImageMap.Add("searchicon", spriteMap19);
            Sprite sprite28 = new Sprite("filterOn", 16f, 16f)
            {
                scale = new Vec2(0.5f, 0.5f),
                center = new Vec2(0f, -9f)
            };
            DuckGame.Input._triggerImageMap.Add("languageFilterOn", sprite28);
            Sprite sprite29 = new Sprite("filterOff", 16f, 16f)
            {
                scale = new Vec2(0.5f, 0.5f),
                center = new Vec2(0f, -9f)
            };
            DuckGame.Input._triggerImageMap.Add("languageFilterOff", sprite29);
            SpriteMap spriteMap20 = new SpriteMap("searchiconwhite", 16, 16)
            {
                scale = new Vec2(0.5f, 0.5f),
                centery = -6f,
                centerx = -4f
            };
            DuckGame.Input._triggerImageMap.Add("searchiconwhite", spriteMap20);
            SpriteMap spriteMap21 = new SpriteMap("searchiconwhite", 16, 16)
            {
                scale = new Vec2(1f, 1f),
                centery = -6f,
                centerx = -4f
            };
            DuckGame.Input._triggerImageMap.Add("searchiconwhitebig", spriteMap21);
            SpriteMap spriteMap22 = new SpriteMap("cloudIcon", 16, 16)
            {
                scale = new Vec2(1f, 1f),
                centery = 0f,
                centerx = 0f
            };
            DuckGame.Input._triggerImageMap.Add("cloudicon", spriteMap22);
            SpriteMap spriteMap23 = new SpriteMap("exBox", 10, 10)
            {
                frame = 0,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap23.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("ITEMBOX", spriteMap23);
            SpriteMap spriteMap24 = new SpriteMap("exBox", 10, 10)
            {
                frame = 1,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap24.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("USERONLINE", spriteMap24);
            SpriteMap spriteMap25 = new SpriteMap("exBox", 10, 10)
            {
                frame = 2,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap25.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("USERAWAY", spriteMap25);
            SpriteMap spriteMap26 = new SpriteMap("exBox", 10, 10)
            {
                frame = 3,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap26.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("USERBUSY", spriteMap26);
            SpriteMap spriteMap27 = new SpriteMap("exBox", 10, 10)
            {
                frame = 4,
                scale = new Vec2(0.5f, 0.5f)
            };
            spriteMap27.centery -= 6f;
            DuckGame.Input._triggerImageMap.Add("USEROFFLINE", spriteMap27);
            DuckGame.Input._triggerImageMap.Add("KBDSHIFT", new Sprite("buttons/keyboard/shift"));
            DuckGame.Input._triggerImageMap.Add("KBDARROWS", new Sprite("buttons/keyboard/arrows"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/oButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/aButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/uButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/yButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/startButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/selectButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/dPadLeft"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/dPadRight"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/dPadUp"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/dPadDown"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/leftBumper"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/rightBumper"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/leftTrigger"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/rightTrigger"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/leftStick"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/xbox/rightStick"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/o"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/square"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/triangle"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/x"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/startButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/selectButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/leftBumper"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/rightBumper"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/leftTrigger"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/rightTrigger"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/a"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/b"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/x"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/y"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/aFami"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/bFami"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/xFami"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/yFami"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/startButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/selectButton"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/leftTrigger"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/SNES/rightTrigger"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/genesis/a"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/genesis/b"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/genesis/c"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/genesis/start"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/playstation/blank"));
            DuckGame.Input._buttonStyles.Add(new Sprite("buttons/genericButton"));
        }

        public static void InitDefaultProfiles()
        {
            for (int index = 0; index < DG.MaxPlayers; ++index)
            {
                InputProfile inputProfile = InputProfile.Add("MPPlayer" + (index + 1).ToString());
                inputProfile.mpIndex = index;
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "LEFT", 4);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "RIGHT", 8);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "UP", 1);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "DOWN", 2);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "JUMP", 4096);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "SHOOT", 16384);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "GRAB", 32768);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "QUACK", 8192);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "START", 16);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "STRAFE", 256);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "RAGDOLL", 512);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "LTRIGGER", 8388608);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "RTRIGGER", 4194304);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "SELECT", 4096);
                inputProfile.Map(DuckGame.Input.GetDevice<GenericController>(index), "CANCEL", 8192);
                if (index == 0)
                    InputProfile.active = inputProfile;
            }
            DuckGame.Input.ApplyDefaultMappings();
            InputProfile.Add("Blank");
        }

        public static void Initialize()
        {
            MonoMain.loadMessage = "Initializing Input System...";
            DevConsole.Log(DCSection.General, "Initializing Input...");
            foreach (DeviceInputMapping inputMappingPreset in DuckGame.Input._defaultInputMappingPresets)
                DuckGame.Input._defaultInputMapping.Add(inputMappingPreset.Clone());
            InputDevice device = new Keyboard("KEYBOARD P1", 0);
            DuckGame.Input._devices.Add(device);
            InputDevice inputDevice1 = new Keyboard("KEYBOARD P2", 1);
            DuckGame.Input._devices.Add(inputDevice1);
            InputDevice inputDevice2 = new Mouse();
            DuckGame.Input._devices.Add(inputDevice2);
            InputDevice inputDevice3 = new XInputPad(0);
            DuckGame.Input._devices.Add(inputDevice3);
            (inputDevice3 as XInputPad).InitializeState();
            InputDevice inputDevice4 = new XInputPad(1);
            DuckGame.Input._devices.Add(inputDevice4);
            (inputDevice4 as XInputPad).InitializeState();
            InputDevice inputDevice5 = new XInputPad(2);
            DuckGame.Input._devices.Add(inputDevice5);
            (inputDevice5 as XInputPad).InitializeState();
            InputDevice inputDevice6 = new XInputPad(3);
            DuckGame.Input._devices.Add(inputDevice6);
            (inputDevice6 as XInputPad).InitializeState();
            GenericController genericController1 = new GenericController(0);
            DuckGame.Input._gamePads.Add(genericController1);
            GenericController genericController2 = new GenericController(1);
            DuckGame.Input._gamePads.Add(genericController2);
            GenericController genericController3 = new GenericController(2);
            DuckGame.Input._gamePads.Add(genericController3);
            GenericController genericController4 = new GenericController(3);
            DuckGame.Input._gamePads.Add(genericController4);
            GenericController genericController5 = new GenericController(4);
            DuckGame.Input._gamePads.Add(genericController5);
            GenericController genericController6 = new GenericController(5);
            DuckGame.Input._gamePads.Add(genericController6);
            GenericController genericController7 = new GenericController(6);
            DuckGame.Input._gamePads.Add(genericController7);
            GenericController genericController8 = new GenericController(7);
            DuckGame.Input._gamePads.Add(genericController8);
            InputProfile.Default = new InputProfile("Default");
            DuckGame.Input.InitializeDInputAsync();
            DuckGame.Input._devices.Add(genericController1);
            DuckGame.Input._devices.Add(genericController2);
            DuckGame.Input._devices.Add(genericController3);
            DuckGame.Input._devices.Add(genericController4);
            DuckGame.Input._devices.Add(genericController5);
            DuckGame.Input._devices.Add(genericController6);
            DuckGame.Input._devices.Add(genericController7);
            DuckGame.Input._devices.Add(genericController8);
            InputProfile.Default.Map(device, "LEFT", 37);
            InputProfile.Default.Map(device, "RIGHT", 39);
            InputProfile.Default.Map(device, "UP", 38);
            InputProfile.Default.Map(device, "DOWN", 40);
            InputProfile.Default.Map(DuckGame.Input.GetDevice<XInputPad>(), "LEFT", 4);
            InputProfile.Default.Map(DuckGame.Input.GetDevice<XInputPad>(), "RIGHT", 8);
            InputProfile.Default.Map(DuckGame.Input.GetDevice<XInputPad>(), "UP", 1);
            InputProfile.Default.Map(DuckGame.Input.GetDevice<XInputPad>(), "DOWN", 2);
            DuckGame.Input._profiles[InputProfile.Default.name] = InputProfile.Default;
            DuckGame.Input.EnumerateGamepads();
            DuckGame.Input.InitDefaultProfiles();
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
                    foreach (DeviceInputMapping oldInputDefault in DuckGame.Input._oldInputDefaults)
                        DuckGame.Input.SetDefaultMapping(oldInputDefault);
                    DuckGame.Input.Save();
                }
                DuckXML duckXml = DuckFile.LoadDuckXML(str);
                if (duckXml == null)
                    return;
                IEnumerable<DXMLNode> source = duckXml.Elements("Mappings");
                if (source == null)
                    return;
                foreach (DXMLNode element in source.Elements<DXMLNode>())
                {
                    if (element.Name == "InputMapping")
                    {
                        DeviceInputMapping mapping = new DeviceInputMapping();
                        mapping.Deserialize(element);
                        DuckGame.Input.SetDefaultMapping(mapping);
                    }
                }
            }
        }

        private static void InitializeDInputAsync()
        {
            ++DuckGame.Input._dinputInitTimesCalled;
            if (DuckGame.Input._dinputInitException != null)
            {
                DevConsole.Log(DCSection.General, "DInput Initialization failed with exception: " + DuckGame.Input._dinputInitException.Message);
                DevConsole.Log(DCSection.General, "DInput has been disabled.");
                DuckGame.Input._dinputEnabled = false;
                DuckGame.Input._dinputInitException = null;
            }
            else
            {
                switch (DuckGame.Input._dinputInitializeStatus)
                {
                    case int.MinValue:
                        DuckGame.Input._dinputInitializeStatus = int.MaxValue;
                        if (MonoMain.disableDirectInput)
                        {
                            DuckGame.Input._dinputEnabled = false;
                            DevConsole.Log(DCSection.General, "MonoMain.disableDirectInput is true, skipping DInput initialize.");
                            return;
                        }
                        DevConsole.Log(DCSection.General, "Starting DInput Async Init...");
                        Task.Run(() =>
                       {
                           try
                           {
                               DuckGame.Input._dinputInitializeStatus = DInput.Initialize();
                           }
                           catch (Exception ex)
                           {
                               DuckGame.Input._dinputInitException = ex;
                           }
                       });
                        return;
                    case 0:
                        InputDevice inputDevice1 = new DInputPad(0);
                        DuckGame.Input._devices.Add(inputDevice1);
                        InputDevice inputDevice2 = new DInputPad(1);
                        DuckGame.Input._devices.Add(inputDevice2);
                        InputDevice inputDevice3 = new DInputPad(2);
                        DuckGame.Input._devices.Add(inputDevice3);
                        InputDevice inputDevice4 = new DInputPad(3);
                        DuckGame.Input._devices.Add(inputDevice4);
                        InputDevice inputDevice5 = new DInputPad(4);
                        DuckGame.Input._devices.Add(inputDevice5);
                        InputDevice inputDevice6 = new DInputPad(5);
                        DuckGame.Input._devices.Add(inputDevice6);
                        InputDevice inputDevice7 = new DInputPad(6);
                        DuckGame.Input._devices.Add(inputDevice7);
                        InputDevice inputDevice8 = new DInputPad(7);
                        DuckGame.Input._devices.Add(inputDevice8);
                        if (DuckGame.Input._dinputInitTimesCalled < 60)
                            DuckGame.Input._suppressInputChangeMessages = 300;
                        DuckGame.Input._dinputEnabled = true;
                        DuckGame.Input.devicesChanged = true;
                        break;
                    case int.MaxValue:
                        return;
                    default:
                        if (MonoMain.disableDirectInput)
                            DevConsole.Log(DCSection.General, "MonoMain.disableDirectInput was true, DInput has been disabled.");
                        else
                            DevConsole.Log(DCSection.General, "DInput.Initialize() failed with code " + DuckGame.Input._dinputInitializeStatus.ToString() + ". DInput has been disabled.");
                        DuckGame.Input._dinputEnabled = false;
                        break;
                }
                DuckGame.Input._dinputInitializeStatus = int.MaxValue;
            }
        }

        public static T GetDevice<T>(int index = 0) where T : InputDevice
        {
            System.Type type = typeof(T);
            foreach (InputDevice device in DuckGame.Input._devices)
            {
                if (type.IsAssignableFrom(device.GetType()) && device.index == index)
                    return device as T;
            }
            return default(T);
        }

        public static InputDevice GetDevice(string name)
        {
            foreach (InputDevice device in DuckGame.Input._devices)
            {
                if (device.name == name)
                    return device;
            }
            return null;
        }

        private static void CheckDInputChanges()
        {
            foreach (GenericController gamePad in DuckGame.Input._gamePads)
            {
                InputDevice device = gamePad.device;
                if (device is DInputPad)
                {
                    DInputPad dinputPad = device as DInputPad;
                    if (dinputPad.isConnected && !dinputPad.prevIsConnected)
                    {
                        DuckGame.Input._gamepadsChanged = true;
                        DuckGame.Input._changePluggedIn = true;
                        DuckGame.Input._changeName = gamePad.device.productName;
                        DuckGame.Input._padConnectionChange = true;
                    }
                    if (!dinputPad.isConnected && dinputPad.prevIsConnected)
                    {
                        DuckGame.Input._gamepadsChanged = true;
                        DuckGame.Input._changePluggedIn = false;
                        DuckGame.Input._changeName = gamePad.device.productName;
                        DuckGame.Input._padConnectionChange = true;
                        gamePad.device = null;
                    }
                    dinputPad.prevIsConnected = dinputPad.isConnected;
                }
            }
        }

        public static void InvalidateDirectInputDeviceIndex(int pIndex)
        {
        }

        private static void RunGamepadEnumerationThread()
        {
            if (DuckGame.Input.enumeratingGamepads)
                return;
            DuckGame.Input.enumeratingGamepads = true;
            --DuckGame.Input.timesToEnumerateGamepads;
            Task.Run(() => DInput.Thread_EnumGamepads());
        }

        public static void EnumerateGamepads()
        {
            foreach (GenericController gamePad in DuckGame.Input._gamePads)
            {
                InputDevice device1 = gamePad.device;
                if (!(device1 is XInputPad) || !DInput.ForceDirectInputMode())
                {
                    if (device1 is DInputPad && DuckGame.Input._dinputEnabled)
                    {
                        DInputPad dinputPad = device1 as DInputPad;
                        if (!dinputPad.isXInput || DInput.ForceDirectInputMode())
                        {
                            if (dinputPad.isConnected != dinputPad.prevIsConnected)
                            {
                                DuckGame.Input._gamepadsChanged = true;
                                DuckGame.Input._changePluggedIn = dinputPad.isConnected;
                                DuckGame.Input._changeName = gamePad.device.productName;
                                DuckGame.Input._padConnectionChange = true;
                                if (!dinputPad.isConnected)
                                    gamePad.device = null;
                            }
                            dinputPad.prevIsConnected = dinputPad.isConnected;
                        }
                        else
                            continue;
                    }
                    else if (device1 != null && !device1.isConnected)
                    {
                        DuckGame.Input._changePluggedIn = false;
                        DuckGame.Input._changeName = device1.productName;
                        DuckGame.Input._padConnectionChange = true;
                        DuckGame.Input._gamepadsChanged = true;
                        gamePad.device = null;
                    }
                    if (gamePad.device == null)
                    {
                        foreach (InputDevice device2 in DuckGame.Input._devices)
                        {
                            if (!(device2 is GenericController) && device2.isConnected && device2.genericController == null)
                            {
                                if (device2 is XInputPad && !DInput.ForceDirectInputMode())
                                {
                                    gamePad.device = device2 as AnalogGamePad;
                                    DuckGame.Input._gamepadsChanged = true;
                                    DuckGame.Input._changePluggedIn = true;
                                    DuckGame.Input._changeName = gamePad.device.productName;
                                    DuckGame.Input._padConnectionChange = true;
                                    break;
                                }
                                if (device2 is DInputPad && DuckGame.Input._dinputEnabled)
                                {
                                    gamePad.device = device2 as AnalogGamePad;
                                    DuckGame.Input._gamepadsChanged = true;
                                    DuckGame.Input._changePluggedIn = true;
                                    DuckGame.Input._changeName = gamePad.device.productName;
                                    if (device2.productName == "Wireless Controller")
                                    {
                                        DuckGame.Input.mightHavePlaystationController = true;
                                        break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Update()
        {
            if (!DuckGame.Input._initializedMessageHook)
            {
                InputSystem.Initialize(MonoMain.instance.Window);
                DuckGame.Input._initializedMessageHook = true;
            }
            if (Options.Data.imeSupport && !DuckGame.Input._initializedIME)
            {
                InputSystem.InitializeIme(MonoMain.instance.Window);
                InputSystem.IMECharEntered += new CharEnteredHandler(Keyboard.IMECharEnteredHandler);
                DuckGame.Input._initializedIME = true;
            }
            InputSystem.CharEntered += new CharEnteredHandler(Keyboard.ALTCharEnteredHandler);
            bool flag = Options.Data.imeSupport && DuckGame.Input._imeAllowed;
            if (flag != DuckGame.Input._prevImeAllowed)
            {
                if (flag)
                    InputSystem.StartIME();
                else
                    InputSystem.EndIME();
            }
            if (!MonoMain.disableDirectInput && !DuckGame.Input._dinputEnabled)
                DuckGame.Input.InitializeDInputAsync();
            DuckGame.Input._prevImeAllowed = flag;
            DuckGame.Input._imeAllowed = false;
            if (DuckGame.Input._prevForceMode != DInput.ForceDirectInputMode())
            {
                foreach (InputDevice device in DuckGame.Input._devices)
                {
                    if (device is GenericController)
                        (device as GenericController).device = null;
                    DuckGame.Input.devicesChanged = true;
                }
                DuckGame.Input._prevForceMode = DInput.ForceDirectInputMode();
            }
            if (DuckGame.Input.devicesChanged)
            {
                ++DuckGame.Input._deviceUpdateWait;
                if (DuckGame.Input._deviceUpdateWait > 120)
                {
                    DuckGame.Input._deviceUpdateWait = 0;
                    DuckGame.Input.devicesChanged = false;
                    ++DuckGame.Input.timesToEnumerateGamepads;
                    if (!DuckGame.Input._dinputEnabled)
                        DuckGame.Input.EnumerateGamepads();
                }
            }
            if (DuckGame.Input._dinputEnabled && DuckGame.Input.timesToEnumerateGamepads > 0 && !DuckGame.Input.enumeratingGamepads)
                DuckGame.Input.RunGamepadEnumerationThread();
            if (DuckGame.Input._gamepadsChanged)
            {
                DuckGame.Input.ApplyDefaultMappings();
                TeamSelect2.ControllerLayoutsChanged();
                DuckGame.Input._gamepadsChanged = false;
                DuckGame.Input.uiDevicesHaveChanged = true;
            }
            if (DuckGame.Input._updateWaitFrames > 0)
            {
                --DuckGame.Input._updateWaitFrames;
            }
            else
            {
                if (DuckGame.Input._dinputEnabled)
                {
                    if (DInput.Update())
                    {
                        DuckGame.Input.enumeratingGamepads = false;
                        DuckGame.Input.EnumerateGamepads();
                    }
                    DuckGame.Input.CheckDInputChanges();
                }
                if (DuckGame.Input._padConnectionChange)
                {
                    DuckGame.Input._padConnectionChange = false;
                    if (MonoMain.started && !DuckGame.Input._ignoreFirstInputChange && DuckGame.Input._suppressInputChangeMessages <= 0)
                    {
                        DuckGame.Input._changeName = DuckGame.Input._changeName.Trim();
                        if (DuckGame.Input._changeName.Length > 25)
                            DuckGame.Input._changeName = DuckGame.Input._changeName.Substring(0, 25) + "...";
                        string str = "@PLUG@|LIME|";
                        if (!DuckGame.Input._changePluggedIn)
                            str = "@UNPLUG@|RED|";
                        HUD.AddInputChangeDisplay(str + DuckGame.Input._changeName);
                    }
                }
                foreach (InputDevice device in DuckGame.Input._devices)
                    device.Update();
            }
            if (MonoMain.started)
                DuckGame.Input._ignoreFirstInputChange = false;
            if (DuckGame.Input._suppressInputChangeMessages <= 0)
                return;
            --DuckGame.Input._suppressInputChangeMessages;
        }

        public static void Terminate()
        {
            if (DuckGame.Input._gamepadThread != null)
                DuckGame.Input._gamepadThread.Abort();
            DuckGame.Input._gamepadThread = null;
            InputSystem.Terminate();
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.IsConnected)
                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            state = GamePad.GetState(PlayerIndex.Two);
            if (state.IsConnected)
                GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);
            state = GamePad.GetState(PlayerIndex.Three);
            if (state.IsConnected)
                GamePad.SetVibration(PlayerIndex.Three, 0f, 0f);
            state = GamePad.GetState(PlayerIndex.Four);
            if (!state.IsConnected)
                return;
            GamePad.SetVibration(PlayerIndex.Four, 0f, 0f);
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
            return DuckGame.Input._profiles.TryGetValue(profile, out inputProfile) && inputProfile.Pressed(trigger);
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
            return DuckGame.Input._profiles.TryGetValue(profile, out inputProfile) && inputProfile.Released(trigger);
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
            return DuckGame.Input._profiles.TryGetValue(profile, out inputProfile) && inputProfile.Down(trigger);
        }
    }
}
