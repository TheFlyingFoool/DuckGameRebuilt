using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class InputProfile
    {
        public static InputProfileCore core
        {
            get
            {
                return _core;
            }
            set
            {
                _core = value;
            }
        }

        public static string MPPlayer1
        {
            get
            {
                return _defaultPlayerMappingStrings[0];
            }
        }

        public static string MPPlayer2
        {
            get
            {
                return _defaultPlayerMappingStrings[1];
            }
        }

        public static string MPPlayer3
        {
            get
            {
                return _defaultPlayerMappingStrings[2];
            }
        }

        public static string MPPlayer4
        {
            get
            {
                return _defaultPlayerMappingStrings[3];
            }
        }

        public static string MPPlayer5
        {
            get
            {
                return _defaultPlayerMappingStrings[4];
            }
        }

        public static string MPPlayer6
        {
            get
            {
                return _defaultPlayerMappingStrings[5];
            }
        }

        public static string MPPlayer7
        {
            get
            {
                return _defaultPlayerMappingStrings[6];
            }
        }

        public static string MPPlayer8
        {
            get
            {
                return _defaultPlayerMappingStrings[7];
            }
        }

        public static string[] MPPlayers
        {
            get
            {
                return _defaultPlayerMappingStrings;
            }
        }

        public static void ReassignDefaultInputProfiles(bool fullRemap = false)
        {
            if (fullRemap)
            {
                Profiles.DefaultPlayer1.inputProfile = Get(MPPlayer1);
                Profiles.DefaultPlayer2.inputProfile = Get(MPPlayer2);
                Profiles.DefaultPlayer3.inputProfile = Get(MPPlayer3);
                Profiles.DefaultPlayer4.inputProfile = Get(MPPlayer4);
                Profiles.DefaultPlayer5.inputProfile = Get(MPPlayer5);
                Profiles.DefaultPlayer6.inputProfile = Get(MPPlayer6);
                Profiles.DefaultPlayer7.inputProfile = Get(MPPlayer7);
                Profiles.DefaultPlayer8.inputProfile = Get(MPPlayer8);
                return;
            }
            Profiles.DefaultPlayer1.SetInputProfileLink(Get(MPPlayer1));
            Profiles.DefaultPlayer2.SetInputProfileLink(Get(MPPlayer2));
            Profiles.DefaultPlayer3.SetInputProfileLink(Get(MPPlayer3));
            Profiles.DefaultPlayer4.SetInputProfileLink(Get(MPPlayer4));
            Profiles.DefaultPlayer5.SetInputProfileLink(Get(MPPlayer5));
            Profiles.DefaultPlayer6.SetInputProfileLink(Get(MPPlayer6));
            Profiles.DefaultPlayer7.SetInputProfileLink(Get(MPPlayer7));
            Profiles.DefaultPlayer8.SetInputProfileLink(Get(MPPlayer8));
        }

        public static void SwapDefaultInputStrings(string from, string to)
        {
            int fromIdx = -1;
            int toIdx = -1;
            for (int i = 0; i < DG.MaxPlayers; i++)
            {
                if (_defaultPlayerMappingStrings[i] == from)
                {
                    fromIdx = i;
                }
                else if (_defaultPlayerMappingStrings[i] == to)
                {
                    toIdx = i;
                }
            }
            if (fromIdx >= 0 && toIdx >= 0)
            {
                string temp = _defaultPlayerMappingStrings[toIdx];
                _defaultPlayerMappingStrings[toIdx] = _defaultPlayerMappingStrings[fromIdx];
                _defaultPlayerMappingStrings[fromIdx] = temp;
            }
        }

        public static InputProfile active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
            }
        }

        public static List<InputProfile> defaultProfiles
        {
            get
            {
                return _core.defaultProfiles;
            }
        }

        public static InputProfile FirstProfileWithDevice
        {
            get
            {
                foreach (InputProfile p in defaultProfiles)
                {
                    if (p.lastActiveDevice != null && p.lastActiveDevice.productName != null)
                    {
                        return p;
                    }
                }
                return DefaultPlayer1;
            }
        }

        public static void SetDefaultProfile(int idx, InputProfile p)
        {
            if (idx == 0)
            {
                _core._profiles[MPPlayer1] = p;
            }
            if (idx == 1)
            {
                _core._profiles[MPPlayer2] = p;
            }
            if (idx == 2)
            {
                _core._profiles[MPPlayer3] = p;
            }
            if (idx == 3)
            {
                _core._profiles[MPPlayer4] = p;
            }
            if (idx == 4)
            {
                _core._profiles[MPPlayer5] = p;
            }
            if (idx == 5)
            {
                _core._profiles[MPPlayer6] = p;
            }
            if (idx == 6)
            {
                _core._profiles[MPPlayer7] = p;
            }
            if (idx == 7)
            {
                _core._profiles[MPPlayer8] = p;
            }
        }

        public static InputProfile DefaultPlayer1
        {
            get
            {
                return _core.DefaultPlayer1;
            }
        }

        public static InputProfile DefaultPlayer2
        {
            get
            {
                return _core.DefaultPlayer2;
            }
        }

        public static InputProfile DefaultPlayer3
        {
            get
            {
                return _core.DefaultPlayer3;
            }
        }

        public static InputProfile DefaultPlayer4
        {
            get
            {
                return _core.DefaultPlayer4;
            }
        }

        public static InputProfile DefaultPlayer5
        {
            get
            {
                return _core.DefaultPlayer5;
            }
        }

        public static InputProfile DefaultPlayer6
        {
            get
            {
                return _core.DefaultPlayer6;
            }
        }

        public static InputProfile DefaultPlayer7
        {
            get
            {
                return _core.DefaultPlayer7;
            }
        }

        public static InputProfile DefaultPlayer8
        {
            get
            {
                return _core.DefaultPlayer8;
            }
        }

        public GenericController genericController
        {
            get
            {
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in _mappings)
                {
                    if (pair.Key is GenericController)
                    {
                        return pair.Key as GenericController;
                    }
                }
                return null;
            }
        }

        protected virtual void UpdateStickPress()
        {
            StickPress(PadButton.DPadLeft, leftStick.x < -0.6f);
            StickPress(PadButton.DPadRight, leftStick.x > 0.6f);
            StickPress(PadButton.DPadUp, leftStick.y > 0.6f);
            StickPress(PadButton.DPadDown, leftStick.y < -0.6f);
        }

        protected void StickPress(PadButton b, bool press)
        {
            if (press)
            {
                if (_leftStickStates[b] == InputState.None)
                {
                    _leftStickStates[b] = InputState.Pressed;
                    return;
                }
                _leftStickStates[b] = InputState.Down;
                return;
            }
            else
            {
                if (_leftStickStates[b] == InputState.Down || _leftStickStates[PadButton.DPadLeft] == InputState.Pressed)
                {
                    _leftStickStates[b] = InputState.Released;
                    return;
                }
                _leftStickStates[b] = InputState.None;
                return;
            }
        }

        public static Dictionary<string, InputProfile> profiles
        {
            get
            {
                return _core._profiles;
            }
        }

        public static InputProfile Add(string name)
        {
            return _core.Add(name);
        }

        public static void Update()
        {
            _core.Update();
        }

        public static InputProfile Get(string name)
        {
            return _core.Get(name);
        }

        private InputDevice _lastActiveDevice {get; set;}

        public InputDevice lastActiveDevice
        {
            get
            {
                if (lastActiveOverride != null)
                {
                    return lastActiveOverride;
                }
                if (!MonoMain.started)
                {
                    return new InputDevice(0);
                }
                if (_lastActiveDevice != null && !_lastActiveDevice.isConnected)
                {
                    InputDevice _locallastActiveDevice = null;
                    foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in _mappings)
                    {
                        if (_locallastActiveDevice == null && pair.Key is Keyboard)
                        {
                            _locallastActiveDevice = pair.Key;
                        }
                        else if (pair.Key is GenericController && (pair.Key as GenericController).device is XInputPad)
                        {
                            _locallastActiveDevice = pair.Key;
                        }
                    }
                    if (_locallastActiveDevice == null)
                    {
                        return _defaultLastActiveDevice;
                    }
                    return _locallastActiveDevice;
                }
                if (_lastActiveDevice == null)
                {
                    foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in _mappings)
                    {
                        if (_lastActiveDevice == null && pair.Key is Keyboard)
                        {
                            _lastActiveDevice = pair.Key;
                        }
                        else if (pair.Key is GenericController && (pair.Key as GenericController).device is XInputPad)
                        {
                            _lastActiveDevice = pair.Key;
                        }
                    }
                    if (_lastActiveDevice == null)
                    {
                        return _defaultLastActiveDevice;
                    }
                }
                return _lastActiveDevice;
            }
            set
            {
                _lastActiveDevice = value;
            }
        }

        public bool HasAnyConnectedDevice()
        {
            using (Dictionary<InputDevice, MultiMap<string, int>>.KeyCollection.Enumerator enumerator = _mappings.Keys.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.isConnected)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public void ClearMappings()
        {
            _mappings.Clear();
        }

        public MultiMap<string, int> GetMappings(Type t)
        {
            foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in _mappings)
            {
                if (pair.Key.GetType() == t)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public InputDevice GetDevice(Type t)
        {
            foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in _mappings)
            {
                if (pair.Key.GetType() == t)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        public int GetMapping(Type t, string trigger)
        {
            MultiMap<string, int> map = GetMappings(t);
            if (map == null)
            {
                return -1;
            }
            List<int> val;
            map.TryGetValue(trigger, out val);
            if (val != null && val.Count > 0)
            {
                return val[0];
            }
            return -1;
        }

        public string GetMappingString(Type t, string trigger)
        {
            int mapping = GetMapping(t, trigger);
            if (mapping == -1)
            {
                return "";
            }
            foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in _mappings)
            {
                if (pair.Key.GetType() == t)
                {
                    Dictionary<int, string> mappings = pair.Key.GetTriggerNames();
                    if (mappings != null)
                    {
                        string name;
                        if (mappings.TryGetValue(mapping, out name))
                        {
                            return name;
                        }
                        return "???";
                    }
                }
            }
            return "";
        }

        public virtual Sprite GetTriggerImage(string trigger)
        {
            InputDevice last_device = lastActiveDevice;
            int mapping = 9999;
            if (trigger != "DPAD" && trigger != "WASD")
            {
                mapping = GetMapping(last_device.GetType(), trigger);
            }
            else if (trigger == "WASD")
            {
                mapping = 9998;
            }
            if (mapping == -1)
            {
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in _mappings)
                {
                    mapping = GetMapping(pair.Key.GetType(), trigger);
                    if (mapping != -1)
                    {
                        return pair.Key.DoGetMapImage(mapping, false);
                    }
                }
                return null;
            }
            return last_device.DoGetMapImage(mapping, false);
        }

        public InputProfile(string profile = "")
        {
            _name = profile;
            _leftStickStates[PadButton.DPadLeft] = InputState.None;
            _leftStickStates[PadButton.DPadRight] = InputState.None;
            _leftStickStates[PadButton.DPadUp] = InputState.None;
            _leftStickStates[PadButton.DPadDown] = InputState.None;
        }

        public static InputProfile GetVirtualInput(int index)
        {
            return _core.GetVirtualInput(index);
        }

        public VirtualInput virtualDevice
        {
            get
            {
                if (!_virtualInputInitialized)
                {
                    foreach (KeyValuePair<InputDevice, MultiMap<string, int>> kvp in _mappings)
                    {
                        if (kvp.Key is VirtualInput)
                        {
                            _virtualInput = (kvp.Key as VirtualInput);
                            break;
                        }
                    }
                    _virtualInputInitialized = true;
                }
                return _virtualInput;
            }
            set
            {
                _virtualInput = value;
            }
        }

        public virtual void Map(InputDevice device, string trigger, int mapping, bool clearExisting = false)
        {
            if (!_mappings.ContainsKey(device))
            {
                _mappings[device] = new MultiMap<string, int>();
            }
            if (clearExisting && _mappings[device].ContainsKey(trigger))
            {
                _mappings[device][trigger].Clear();
            }
            _mappings[device].Add(trigger, mapping);
        }

        public MultiMap<string, int> GetControllerMap<T>() where T : InputDevice
        {
            MultiMap<string, int> controls = null;
            InputDevice d = Input.GetDevice<T>(0);
            if (d != null && _mappings.ContainsKey(d))
            {
                controls = _mappings[d];
            }
            d = Input.GetDevice<T>(1);
            if (d != null && _mappings.ContainsKey(d))
            {
                controls = _mappings[d];
            }
            d = Input.GetDevice<T>(2);
            if (d != null && _mappings.ContainsKey(d))
            {
                controls = _mappings[d];
            }
            d = Input.GetDevice<T>(3);
            if (d != null && _mappings.ContainsKey(d))
            {
                controls = _mappings[d];
            }
            return controls;
        }

        public void SetGenericControllerMapIndex<T>(int i, MultiMap<string, int> controls) where T : InputDevice
        {
            InputDevice d = Input.GetDevice<T>(0);
            if (d != null && _mappings.ContainsKey(d))
            {
                _mappings.Remove(d);
            }
            d = Input.GetDevice<T>(1);
            if (d != null && _mappings.ContainsKey(d))
            {
                _mappings.Remove(d);
            }
            d = Input.GetDevice<T>(2);
            if (d != null && _mappings.ContainsKey(d))
            {
                _mappings.Remove(d);
            }
            d = Input.GetDevice<T>(3);
            if (d != null && _mappings.ContainsKey(d))
            {
                _mappings.Remove(d);
            }
            if (controls != null)
            {
                _mappings[Input.GetDevice<T>(i)] = controls;
            }
        }

        public InputProfile Clone()
        {
            InputProfile clone = new InputProfile("")
            {
                _name = name,
                _state = _state,
                _prevState = _prevState,
                dindex = dindex,
                swapBack = swapBack
            };
            foreach (KeyValuePair<InputDevice, MultiMap<string, int>> kvp in _mappings)
            {
                clone._mappings[kvp.Key] = new MultiMap<string, int>();
                foreach (KeyValuePair<string, List<int>> pair in kvp.Value)
                {
                    clone._mappings[kvp.Key].AddRange(pair.Key, pair.Value);
                }
            }
            return clone;
        }

        public virtual void UpdateExtraInput()
        {
        }

        public bool CheckCode(InputCode c)
        {
            return c.Update(this);
        }

        public bool JoinGamePressed()
        {
            return Pressed(Triggers.Start, false);
        }

        public virtual bool Pressed(string trigger, bool any = false)
        {
            if (Input.ignoreInput && _virtualInput == null)
            {
                return false;
            }
            if (trigger == Triggers.Any)
            {
                any = true;
            }
            if (_repeatList.Contains(trigger))
            {
                return true;
            }
            foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
            {
                InputDevice mapped_device = map.Key;
                if ((!(mapped_device is Keyboard) || !ignoreKeyboard) && (_virtualInput == null || mapped_device is VirtualInput))
                {
                    List<int> vals;
                    if (any)
                    {
                        if (mapped_device.MapPressed(-1, true))
                        {
                            lastPressFrame = Graphics.frame;
                            return true;
                        }
                    }
                    else if (map.Value.TryGetValue(trigger, out vals))
                    {
                        foreach (int val in vals)
                        {
                            if ((mapped_device is AnalogGamePad || mapped_device is GenericController) && _leftStickStates.ContainsKey((PadButton)val) && _leftStickStates[(PadButton)val] == InputState.Pressed)
                            {
                                return true;
                            }
                            if (mapped_device.MapPressed(val, any))
                            {
                                lastPressFrame = Graphics.frame;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public virtual bool Released(string trigger)
        {
            if (Input.ignoreInput && _virtualInput == null)
            {
                return false;
            }
            foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
            {
                InputDevice mapped_device = map.Key;
                List<int> vals;
                if ((_virtualInput == null || mapped_device is VirtualInput) && map.Value.TryGetValue(trigger, out vals))
                {
                    foreach (int val in vals)
                    {
                        if ((mapped_device is AnalogGamePad || mapped_device is GenericController) && _leftStickStates.ContainsKey((PadButton)val) && _leftStickStates[(PadButton)val] == InputState.Released)
                        {
                            lastPressFrame = Graphics.frame;
                            return true;
                        }
                        if (mapped_device.MapReleased(val))
                        {
                            lastPressFrame = Graphics.frame;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public virtual bool Down(string trigger)
        {
            if (Input.ignoreInput && _virtualInput == null)
            {
                return false;
            }
            foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
            {
                InputDevice mapped_device = map.Key;
                List<int> vals;
                if ((_virtualInput == null || mapped_device is VirtualInput) && map.Value.TryGetValue(trigger, out vals))
                {
                    foreach (int val in vals)
                    {
                        if ((mapped_device is AnalogGamePad || mapped_device is GenericController) && _leftStickStates.ContainsKey((PadButton)val) && (_leftStickStates[(PadButton)val] == InputState.Down || _leftStickStates[(PadButton)val] == InputState.Pressed))
                        {
                            lastPressFrame = Graphics.frame;
                            return true;
                        }
                        if (mapped_device.MapDown(val, false))
                        {
                            if ((!(mapped_device is Keyboard) || !DuckNetwork.core.enteringText) && !(mapped_device is VirtualInput))
                            {
                                _lastActiveDevice = map.Key;
                                Input.lastActiveProfile = this;
                            }
                            lastPressFrame = Graphics.frame;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public virtual float leftTrigger
        {
            get
            {
                if (_virtualInput != null)
                {
                    return _virtualInput.leftTrigger;
                }
                if (Input.ignoreInput)
                {
                    return 0f;
                }
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
                {
                    if (_virtualInput == null || map.Key is VirtualInput)
                    {
                        AnalogGamePad pad = map.Key as AnalogGamePad;
                        if (pad == null && map.Key is GenericController)
                        {
                            pad = (map.Key as GenericController).device;
                        }
                        if (pad != null)
                        {
                            List<int> mappings;
                            if (!map.Value.TryGetValue(Triggers.LeftTrigger, out mappings) || mappings.Count <= 0)
                            {
                                return pad.leftTrigger;
                            }
                            int mapping = mappings[0];
                            if (mapping == 8388608)
                            {
                                return pad.leftTrigger;
                            }
                            if (mapping == 4194304)
                            {
                                return pad.rightTrigger;
                            }
                        }
                    }
                }
                return 0f;
            }
        }

        public float rightTrigger
        {
            get
            {
                if (_virtualInput != null)
                {
                    return _virtualInput.rightTrigger;
                }
                if (Input.ignoreInput)
                {
                    return 0f;
                }
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
                {
                    if (_virtualInput == null || map.Key is VirtualInput)
                    {
                        AnalogGamePad pad = map.Key as AnalogGamePad;
                        if (pad == null && map.Key is GenericController)
                        {
                            pad = (map.Key as GenericController).device;
                        }
                        if (pad != null)
                        {
                            List<int> mappings;
                            if (!map.Value.TryGetValue(Triggers.RightTrigger, out mappings) || mappings.Count <= 0)
                            {
                                return pad.rightTrigger;
                            }
                            int mapping = mappings[0];
                            if (mapping == 8388608)
                            {
                                return pad.leftTrigger;
                            }
                            if (mapping == 4194304)
                            {
                                return pad.rightTrigger;
                            }
                        }
                    }
                }
                return 0f;
            }
        }

        public Vec2 leftStick
        {
            get
            {
                if (_virtualInput != null)
                {
                    return _virtualInput.leftStick;
                }
                if (Input.ignoreInput)
                {
                    return Vec2.Zero;
                }
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
                {
                    if (_virtualInput == null || map.Key is VirtualInput)
                    {
                        AnalogGamePad pad = map.Key as AnalogGamePad;
                        if (pad == null && map.Key is GenericController)
                        {
                            pad = (map.Key as GenericController).device;
                        }
                        if (pad != null)
                        {
                            List<int> mappings;
                            if (!map.Value.TryGetValue(Triggers.LeftStick, out mappings) || mappings.Count <= 0)
                            {
                                if (pad.leftStick != Vec2.Zero)
                                {
                                    _lastActiveDevice = map.Key;
                                }
                                return pad.leftStick;
                            }
                            int mapping = mappings[0];
                            if (mapping == 64)
                            {
                                if (pad.leftStick != Vec2.Zero)
                                {
                                    _lastActiveDevice = map.Key;
                                }
                                return pad.leftStick;
                            }
                            if (mapping == 128)
                            {
                                if (pad.rightStick != Vec2.Zero)
                                {
                                    _lastActiveDevice = map.Key;
                                }
                                return pad.rightStick;
                            }
                        }
                    }
                }
                return new Vec2(0f, 0f);
            }
        }

        public Vec2 rightStick
        {
            get
            {
                if (_virtualInput != null)
                {
                    return _virtualInput.rightStick;
                }
                if (Input.ignoreInput)
                {
                    return Vec2.Zero;
                }
                if (Mouse.left == InputState.Pressed || _mouseAnchor == Vec2.Zero)
                {
                    _mouseAnchor = Mouse.position;
                }
                else
                {
                    if (Mouse.left == InputState.Down)
                    {
                        Vec2 dif = (Mouse.position - _mouseAnchor) / 16f;
                        dif.y *= -1f;
                        float len = dif.length;
                        if (len > 1f)
                        {
                            len = 1f;
                        }
                        return dif.normalized * len;
                    }
                    if (Mouse.left == InputState.None)
                    {
                        _mouseAnchor = Vec2.Zero;
                    }
                }
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
                {
                    if (_virtualInput == null || map.Key is VirtualInput)
                    {
                        AnalogGamePad pad = map.Key as AnalogGamePad;
                        if (pad == null && map.Key is GenericController)
                        {
                            pad = (map.Key as GenericController).device;
                        }
                        if (pad != null)
                        {
                            List<int> mappings;
                            if (!map.Value.TryGetValue(Triggers.RightStick, out mappings) || mappings.Count <= 0)
                            {
                                return pad.rightStick;
                            }
                            int mapping = mappings[0];
                            if (mapping == 64)
                            {
                                return pad.leftStick;
                            }
                            if (mapping == 128)
                            {
                                return pad.rightStick;
                            }
                        }
                    }
                }
                return new Vec2(0f, 0f);
            }
        }

        public bool hasMotionAxis
        {
            get
            {
                if (Input.ignoreInput)
                {
                    return false;
                }
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
                {
                    if (_virtualInput == null || map.Key is VirtualInput)
                    {
                        AnalogGamePad pad = map.Key as AnalogGamePad;
                        if (pad == null && map.Key is GenericController)
                        {
                            pad = (map.Key as GenericController).device;
                        }
                        if (pad != null)
                        {
                            return pad.hasMotionAxis;
                        }
                    }
                }
                return false;
            }
        }

        public float motionAxis
        {
            get
            {
                if (Input.ignoreInput)
                {
                    return 0f;
                }
                foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in _mappings)
                {
                    if (_virtualInput == null || map.Key is VirtualInput)
                    {
                        AnalogGamePad pad = map.Key as AnalogGamePad;
                        if (pad == null && map.Key is GenericController)
                        {
                            pad = (map.Key as GenericController).device;
                        }
                        if (pad != null)
                        {
                            return pad.motionAxis;
                        }
                    }
                }
                return 0f;
            }
        }

        public ushort state
        {
            get
            {
                return _state;
            }
        }

        public ushort prevState
        {
            get
            {
                return _prevState;
            }
        }

        public static bool repeat
        {
            get
            {
                return _repeat;
            }
            set
            {
                _repeat = value;
            }
        }

        public void UpdateTriggerStates()
        {
            _repeatList.Clear();
            if (_repeat)
            {
                if (Pressed(Triggers.MenuLeft, false) || Pressed(Triggers.MenuRight, false) || Pressed(Triggers.MenuUp, false) || Pressed(Triggers.MenuDown, false))
                {
                    if (!_repeating)
                    {
                        _repeatTime = 1.8f;
                        _repeating = true;
                    }
                    else
                    {
                        _repeatTime = 0.5f;
                    }
                }
                if (_repeatTime > 0f)
                {
                    _repeatTime -= 0.1f;
                    bool down = false;
                    if (Down(Triggers.MenuLeft))
                    {
                        if (_repeatTime <= 0f)
                        {
                            _repeatList.Add(Triggers.MenuLeft);
                        }
                        down = true;
                    }
                    if (Down(Triggers.MenuRight))
                    {
                        if (_repeatTime <= 0f)
                        {
                            _repeatList.Add(Triggers.MenuRight);
                        }
                        down = true;
                    }
                    if (Down(Triggers.MenuUp))
                    {
                        if (_repeatTime <= 0f)
                        {
                            _repeatList.Add(Triggers.MenuUp);
                        }
                        down = true;
                    }
                    if (Down(Triggers.MenuDown))
                    {
                        if (_repeatTime <= 0f)
                        {
                            _repeatList.Add(Triggers.MenuDown);
                        }
                        down = true;
                    }
                    if (_repeatTime <= 0f && down)
                    {
                        _repeatTime = 0.3f;
                    }
                    if (!down)
                    {
                        _repeating = false;
                        _repeatTime = 0f;
                    }
                }
                else
                {
                    _repeatTime = 0f;
                }
            }
            UpdateStickPress();
            _prevState = _state;
            _state = 0;
            foreach (string t in Network.synchronizedTriggers)
            {
                _state |= (Down(t) ? (ushort)1 : (ushort)0);
                _state = (ushort)(_state << 1);
            }
        }

        // Note: this type is marked as 'beforefieldinit'.
        static InputProfile()
        {
        }

        public int mpIndex = -1;

        public bool oldAngles;

        public InputProfile swapBack;

        private static InputProfileCore _core = new InputProfileCore();

        public const string SinglePlayer = "SinglePlayer";

        private static string[] _defaultPlayerMappingStrings = new string[]
        {
            "MPPlayer1",
            "MPPlayer2",
            "MPPlayer3",
            "MPPlayer4",
            "MPPlayer5",
            "MPPlayer6",
            "MPPlayer7",
            "MPPlayer8"
        };

        public const string Blank = "Blank";

        private static InputProfile _active;

        public InputDevice lastSynchronizedDevice;

        private InputDevice _defaultLastActiveDevice = new InputDevice(0);

        public InputDevice lastActiveOverride;

        private string _name;

        private Dictionary<InputDevice, MultiMap<string, int>> _mappings = new Dictionary<InputDevice, MultiMap<string, int>>();

        protected Dictionary<PadButton, InputState> _leftStickStates = new Dictionary<PadButton, InputState>(default(PadButtonComparer));

        public static InputProfile Default;

        public int dindex;

        private bool _virtualInputInitialized;

        private VirtualInput _virtualInput;

        public long lastPressFrame;

        public static bool ignoreKeyboard = false;

        private Vec2 _mouseAnchor = Vec2.Zero;

        private ushort _state;

        private ushort _prevState;

        private static bool _repeat = false;

        private float _repeatTime;

        private bool _repeating;

        private List<string> _repeatList = new List<string>();
    }
}
