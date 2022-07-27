// Decompiled with JetBrains decompiler
// Type: DuckGame.DeviceInputMapping
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public void MapInput(string pTrigger, int pIndex) => this.map[pTrigger] = pIndex;

        public InputDevice device
        {
            get
            {
                if (this.deviceOverride != null)
                    return this.deviceOverride;
                if (this.deviceName == "XBOX GAMEPAD")
                    return Input.GetDevice<XInputPad>();
                foreach (InputDevice inputDevice in Input.GetInputDevices())
                {
                    if (inputDevice.productName == this.deviceName && inputDevice.productGUID == this.deviceGUID)
                        return inputDevice;
                }
                return new InputDevice();
            }
        }

        public List<InputDevice> devices => this.deviceName == "XBOX GAMEPAD" ? Input.GetInputDevices().Where<InputDevice>(x => x is XInputPad).ToList<InputDevice>() : Input.GetInputDevices().Where<InputDevice>(x => x.productName == this.deviceName && x.productGUID == this.deviceGUID).ToList<InputDevice>();

        public Sprite GetSprite(int mapping)
        {
            string str;
            if (!this.graphicMap.TryGetValue(mapping, out str))
                return null;
            Sprite sprite1;
            if (this._spriteMap.TryGetValue(str, out sprite1))
                return sprite1;
            Sprite sprite2 = new Sprite(str);
            this._spriteMap[str] = sprite2;
            return sprite2;
        }

        public DeviceInputMapping() => this._nodeName = "InputMapping";

        public string GetMappingString(string trigger)
        {
            int key;
            if (!this.map.TryGetValue(trigger, out key))
                return "";
            Dictionary<int, string> triggerNames = this.device.GetTriggerNames();
            string mappingString = "???";
            triggerNames?.TryGetValue(key, out mappingString);
            return mappingString;
        }

        public bool IsEqual(DeviceInputMapping compare)
        {
            if (this.map.Count != compare.map.Count)
                return false;
            foreach (KeyValuePair<string, int> keyValuePair in this.map)
            {
                int num1;
                this.map.TryGetValue(keyValuePair.Key, out num1);
                int num2;
                compare.map.TryGetValue(keyValuePair.Key, out num2);
                if (num1 != num2)
                    return false;
            }
            if (this.graphicMap.Count != compare.graphicMap.Count)
                return false;
            foreach (KeyValuePair<int, string> graphic in this.graphicMap)
            {
                string str1;
                this.graphicMap.TryGetValue(graphic.Key, out str1);
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
                deviceName = this.deviceName,
                deviceGUID = this.deviceGUID
            };
            foreach (KeyValuePair<string, int> keyValuePair in this.map)
                deviceInputMapping.MapInput(keyValuePair.Key, keyValuePair.Value);
            foreach (KeyValuePair<int, string> graphic in this.graphicMap)
                deviceInputMapping.graphicMap[graphic.Key] = graphic.Value;
            return deviceInputMapping;
        }

        private void CleanDupes(string trigger, int padButton, bool allowDupes)
        {
            if (Triggers.IsUITrigger(trigger) || Triggers.IsBasicMovement(trigger))
                allowDupes = false;
            if (allowDupes)
                return;
            int pIndex = this.map[trigger];
            string pTrigger = null;
            foreach (KeyValuePair<string, int> keyValuePair in this.map)
            {
                if (keyValuePair.Key != trigger && keyValuePair.Value == padButton && (Triggers.IsUITrigger(trigger) == Triggers.IsUITrigger(keyValuePair.Key) || Triggers.IsBasicMovement(trigger) && Triggers.IsBasicMovement(keyValuePair.Key)))
                {
                    pTrigger = keyValuePair.Key;
                    break;
                }
            }
            if (pTrigger == null)
                return;
            this.MapInput(pTrigger, pIndex);
        }

        public bool RunMappingUpdate(string trigger, bool allowDupes = true)
        {
            bool finished = false;
            if (this.device is AnalogGamePad || (this.device is GenericController && (this.device as GenericController).device != null))
            {
                AnalogGamePad pad = this.device as AnalogGamePad;
                if (pad == null && this.device is GenericController)
                {
                    pad = (this.device as GenericController).device;
                }
                if (trigger == "LSTICK" || trigger == "RSTICK")
                {
                    if (pad.leftStick.length > 0.1f)
                    {
                        this.CleanDupes(trigger, 64, allowDupes);
                        this.MapInput(trigger, 64);
                        return true;
                    }
                    if (pad.rightStick.length > 0.1f)
                    {
                        this.CleanDupes(trigger, 128, allowDupes);
                        this.MapInput(trigger, 128);
                        return true;
                    }
                    return finished;
                }
                else if (trigger == "LTRIGGER" || trigger == "RTRIGGER")
                {
                    if (pad.leftTrigger > 0.1f)
                    {
                        this.CleanDupes(trigger, 8388608, allowDupes);
                        this.MapInput(trigger, 8388608);
                        return true;
                    }
                    if (pad.rightTrigger > 0.1f)
                    {
                        this.CleanDupes(trigger, 4194304, allowDupes);
                        this.MapInput(trigger, 4194304);
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
                            if (b != PadButton.LeftThumbstickUp && b != PadButton.LeftThumbstickDown && b != PadButton.LeftThumbstickLeft && b != PadButton.LeftThumbstickRight && b != PadButton.RightThumbstickUp && b != PadButton.RightThumbstickDown && b != PadButton.RightThumbstickLeft && b != PadButton.RightThumbstickRight && this.device.MapPressed((int)b, false))
                            {
                                this.CleanDupes(trigger, (int)b, allowDupes);
                                this.MapInput(trigger, (int)b);
                                finished = true;
                            }
                        }
                        return finished;
                    }
                }
            }
            if (this.device is Keyboard)
            {
                foreach (Keys b2 in Enum.GetValues(typeof(Keys)).Cast<Keys>())
                {
                    if (this.device.MapPressed((int)b2, false))
                    {
                        this.CleanDupes(trigger, (int)b2, allowDupes);
                        this.MapInput(trigger, (int)b2);
                        finished = true;
                    }
                }
                if (!finished)
                {
                    if (Mouse.left == InputState.Pressed)
                    {
                        this.MapInput(trigger, 999990);
                        finished = true;
                    }
                    else if (Mouse.middle == InputState.Pressed)
                    {
                        this.MapInput(trigger, 999991);
                        finished = true;
                    }
                    else if (Mouse.right == InputState.Pressed)
                    {
                        this.MapInput(trigger, 999992);
                        finished = true;
                    }
                }
            }
            return finished;
        }
    }
}
