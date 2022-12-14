// Decompiled with JetBrains decompiler
// Type: DuckGame.DeviceInputMapping
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DeviceInputMapping : DataClass
    {
        public string deviceName;
        public string deviceGUID;
        public Dictionary<string, int> map = new Dictionary<string, int>();
        public Dictionary<int, string> graphicMap = new Dictionary<int, string>();
        private Dictionary<string, Sprite> _spriteMap = new Dictionary<string, Sprite>();
        public int inputOverrideType;
        public InputDevice deviceOverride;

        public void MapInput(string pTrigger, int pIndex) => map[pTrigger] = pIndex;

        public InputDevice device
        {
            get
            {
                if (deviceOverride != null)
                    return deviceOverride;
                if (deviceName == "XBOX GAMEPAD")
                    return Input.GetDevice<XInputPad>();
                foreach (InputDevice inputDevice in Input.GetInputDevices())
                {
                    if (inputDevice.productName == deviceName && inputDevice.productGUID == deviceGUID)
                        return inputDevice;
                }
                return new InputDevice();
            }
        }

        public List<InputDevice> devices => deviceName == "XBOX GAMEPAD" ? Input.GetInputDevices().Where(x => x is XInputPad).ToList() : Input.GetInputDevices().Where(x => x.productName == deviceName && x.productGUID == deviceGUID).ToList();

        public Sprite GetSprite(int mapping)
        {
            string str;
            if (!graphicMap.TryGetValue(mapping, out str))
                return null;
            Sprite sprite1;
            if (_spriteMap.TryGetValue(str, out sprite1))
                return sprite1;
            Sprite sprite2 = new Sprite(str);
            _spriteMap[str] = sprite2;
            return sprite2;
        }

        public DeviceInputMapping() => _nodeName = "InputMapping";

        public string GetMappingString(string trigger)
        {
            int key;
            if (!map.TryGetValue(trigger, out key))
                return "";
            Dictionary<int, string> triggerNames = device.GetTriggerNames();
            string mappingString = "???";
            triggerNames?.TryGetValue(key, out mappingString);
            return mappingString;
        }

        public bool IsEqual(DeviceInputMapping compare)
        {
            if (map.Count != compare.map.Count)
                return false;
            foreach (KeyValuePair<string, int> keyValuePair in map)
            {
                int num1;
                map.TryGetValue(keyValuePair.Key, out num1);
                int num2;
                compare.map.TryGetValue(keyValuePair.Key, out num2);
                if (num1 != num2)
                    return false;
            }
            if (graphicMap.Count != compare.graphicMap.Count)
                return false;
            foreach (KeyValuePair<int, string> graphic in graphicMap)
            {
                string str1;
                graphicMap.TryGetValue(graphic.Key, out str1);
                string str2;
                compare.graphicMap.TryGetValue(graphic.Key, out str2);
                if (str1 != str2)
                    return false;
            }
            return true;
        }

        public DeviceInputMapping Clone()
        {
            DeviceInputMapping deviceInputMapping = new DeviceInputMapping
            {
                deviceName = deviceName,
                deviceGUID = deviceGUID
            };
            foreach (KeyValuePair<string, int> keyValuePair in map)
                deviceInputMapping.MapInput(keyValuePair.Key, keyValuePair.Value);
            foreach (KeyValuePair<int, string> graphic in graphicMap)
                deviceInputMapping.graphicMap[graphic.Key] = graphic.Value;
            return deviceInputMapping;
        }

        private void CleanDupes(string trigger, int padButton, bool allowDupes)
        {
            if (Triggers.IsUITrigger(trigger) || Triggers.IsBasicMovement(trigger))
                allowDupes = false;
            if (allowDupes)
                return;
            int pIndex = map[trigger];
            string pTrigger = null;
            foreach (KeyValuePair<string, int> keyValuePair in map)
            {
                if (keyValuePair.Key != trigger && keyValuePair.Value == padButton && (Triggers.IsUITrigger(trigger) == Triggers.IsUITrigger(keyValuePair.Key) || Triggers.IsBasicMovement(trigger) && Triggers.IsBasicMovement(keyValuePair.Key)))
                {
                    pTrigger = keyValuePair.Key;
                    break;
                }
            }
            if (pTrigger == null)
                return;
            MapInput(pTrigger, pIndex);
        }

        public bool RunMappingUpdate(string trigger, bool allowDupes = true)
        {
            bool finished = false;
            if (device is AnalogGamePad || (device is GenericController && (device as GenericController).device != null))
            {
                AnalogGamePad pad = device as AnalogGamePad;
                if (pad == null && device is GenericController)
                {
                    pad = (device as GenericController).device;
                }
                if (trigger == "LSTICK" || trigger == "RSTICK")
                {
                    if (pad.leftStick.length > 0.1f)
                    {
                        CleanDupes(trigger, 64, allowDupes);
                        MapInput(trigger, 64);
                        return true;
                    }
                    if (pad.rightStick.length > 0.1f)
                    {
                        CleanDupes(trigger, 128, allowDupes);
                        MapInput(trigger, 128);
                        return true;
                    }
                    return finished;
                }
                else if (trigger == "LTRIGGER" || trigger == "RTRIGGER")
                {
                    if (pad.leftTrigger > 0.1f)
                    {
                        CleanDupes(trigger, 8388608, allowDupes);
                        MapInput(trigger, 8388608);
                        return true;
                    }
                    if (pad.rightTrigger > 0.1f)
                    {
                        CleanDupes(trigger, 4194304, allowDupes);
                        MapInput(trigger, 4194304);
                        return true;
                    }
                    return finished;
                }
                else
                {
                    using (IEnumerator<PadButton> enumerator = Enum.GetValues(typeof(PadButton)).Cast<PadButton>().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            PadButton b = enumerator.Current;
                            if (b != PadButton.LeftThumbstickUp && b != PadButton.LeftThumbstickDown && b != PadButton.LeftThumbstickLeft && b != PadButton.LeftThumbstickRight && b != PadButton.RightThumbstickUp && b != PadButton.RightThumbstickDown && b != PadButton.RightThumbstickLeft && b != PadButton.RightThumbstickRight && device.MapPressed((int)b, false))
                            {
                                CleanDupes(trigger, (int)b, allowDupes);
                                MapInput(trigger, (int)b);
                                finished = true;
                            }
                        }
                        return finished;
                    }
                }
            }
            if (device is Keyboard)
            {
                foreach (Keys b2 in Enum.GetValues(typeof(Keys)).Cast<Keys>())
                {
                    if (device.MapPressed((int)b2, false))
                    {
                        CleanDupes(trigger, (int)b2, allowDupes);
                        MapInput(trigger, (int)b2);
                        finished = true;
                    }
                }
                if (!finished)
                {
                    if (Mouse.left == InputState.Pressed)
                    {
                        MapInput(trigger, 999990);
                        finished = true;
                    }
                    else if (Mouse.middle == InputState.Pressed)
                    {
                        MapInput(trigger, 999991);
                        finished = true;
                    }
                    else if (Mouse.right == InputState.Pressed)
                    {
                        MapInput(trigger, 999992);
                        finished = true;
                    }
                }
            }
            return finished;
        }
    }
}
