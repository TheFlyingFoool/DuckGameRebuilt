// Decompiled with JetBrains decompiler
// Type: DuckGame.InputDevice
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class InputDevice
    {
        protected string _name;
        public float rumbleIntensity;
        protected int _index;
        protected string _productName;
        protected string _productGUID;
        private volatile GenericController _genericController;
        public DeviceInputMapping overrideMap;
        private bool _rumbleThisFrame;
        private int _framesRumbled;

        public byte inputDeviceType
        {
            get
            {
                switch (this)
                {
                    case Keyboard _:
                        return 0;
                    case XInputPad _:
                        return 1;
                    case GenericController _:
                        if ((this as GenericController).device != null)
                            return (this as GenericController).device.inputDeviceType;
                        break;
                }
                return byte.MaxValue;
            }
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public int index => _index;

        public virtual string productName
        {
            get => _productName;
            set => _productName = value;
        }

        public virtual string productGUID
        {
            get => _productGUID;
            set => _productGUID = value;
        }

        public virtual bool hasMotionAxis => false;

        public virtual float motionAxis => 0f;

        public virtual bool allowStartRemap => true;

        public virtual int numSticks => 0;

        public virtual int numTriggers => 0;

        public virtual bool allowDirectionalMapping => true;

        public GenericController genericController
        {
            get => _genericController;
            set => _genericController = value;
        }

        public virtual bool isConnected => true;

        public InputDevice(int idx = 0) => _index = idx;

        public virtual Dictionary<int, string> GetTriggerNames() => null;

        public virtual Sprite DoGetMapImage(int map, bool skipStyleCheck = false)
        {
            if (skipStyleCheck)
                return GetMapImage(map);
            DeviceInputMapping deviceInputMapping = overrideMap;
            if (overrideMap == null)
                deviceInputMapping = Input.GetDefaultMapping(productName, productGUID, makeClone: false);
            Sprite sprite = deviceInputMapping.GetSprite(map);
            if (sprite != null)
                return sprite;
            if (map == 9999 || map == 9998)
                map = !deviceInputMapping.map.ContainsKey("LEFT") || deviceInputMapping.map["LEFT"] != 37 ? 9998 : 9999;
            return GetMapImage(map);
        }

        public float RumbleIntensityModifier()
        {
            if (rumbleIntensity > 0.3)
                _rumbleThisFrame = true;
            return _framesRumbled > 120 ? 0f : Options.Data.rumbleIntensity;
        }

        public virtual void Rumble(float leftIntensity = 0f, float rightIntensity = 0f)
        {
        }

        public virtual Sprite GetMapImage(int map) => null;

        public virtual void Update()
        {
            if (_rumbleThisFrame)
                ++_framesRumbled;
            else
                _framesRumbled = 0;
            _rumbleThisFrame = false;
        }

        public virtual bool MapPressed(int mapping, bool any = false) => false;

        public virtual bool MapReleased(int mapping) => false;

        public virtual bool MapDown(int mapping, bool any = false) => false;
    }
}
