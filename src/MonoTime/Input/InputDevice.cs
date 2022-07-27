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
            get => this._name;
            set => this._name = value;
        }

        public int index => this._index;

        public virtual string productName
        {
            get => this._productName;
            set => this._productName = value;
        }

        public virtual string productGUID
        {
            get => this._productGUID;
            set => this._productGUID = value;
        }

        public virtual bool hasMotionAxis => false;

        public virtual float motionAxis => 0.0f;

        public virtual bool allowStartRemap => true;

        public virtual int numSticks => 0;

        public virtual int numTriggers => 0;

        public virtual bool allowDirectionalMapping => true;

        public GenericController genericController
        {
            get => this._genericController;
            set => this._genericController = value;
        }

        public virtual bool isConnected => true;

        public InputDevice(int idx = 0) => this._index = idx;

        public virtual Dictionary<int, string> GetTriggerNames() => null;

        public virtual Sprite DoGetMapImage(int map, bool skipStyleCheck = false)
        {
            if (skipStyleCheck)
                return this.GetMapImage(map);
            DeviceInputMapping deviceInputMapping = this.overrideMap;
            if (this.overrideMap == null)
                deviceInputMapping = Input.GetDefaultMapping(this.productName, this.productGUID, makeClone: false);
            Sprite sprite = deviceInputMapping.GetSprite(map);
            if (sprite != null)
                return sprite;
            if (map == 9999 || map == 9998)
                map = !deviceInputMapping.map.ContainsKey("LEFT") || deviceInputMapping.map["LEFT"] != 37 ? 9998 : 9999;
            return this.GetMapImage(map);
        }

        public float RumbleIntensityModifier()
        {
            if (rumbleIntensity > 0.3)
                this._rumbleThisFrame = true;
            return this._framesRumbled > 120 ? 0.0f : Options.Data.rumbleIntensity;
        }

        public virtual void Rumble(float leftIntensity = 0.0f, float rightIntensity = 0.0f)
        {
        }

        public virtual Sprite GetMapImage(int map) => null;

        public virtual void Update()
        {
            if (this._rumbleThisFrame)
                ++this._framesRumbled;
            else
                this._framesRumbled = 0;
            this._rumbleThisFrame = false;
        }

        public virtual bool MapPressed(int mapping, bool any = false) => false;

        public virtual bool MapReleased(int mapping) => false;

        public virtual bool MapDown(int mapping, bool any = false) => false;
    }
}
