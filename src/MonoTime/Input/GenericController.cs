// Decompiled with JetBrains decompiler
// Type: DuckGame.GenericController
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class GenericController : InputDevice
    {
        private volatile AnalogGamePad _device;

        public override bool allowStartRemap => true;

        public override int numSticks => 2;

        public override int numTriggers => 2;

        public AnalogGamePad device
        {
            get => _device;
            set
            {
                if (_device != null)
                    _device.genericController = null;
                _device = value;
                if (_device == null)
                    return;
                _device.genericController = this;
            }
        }

        public override Dictionary<int, string> GetTriggerNames() => _device != null ? _device.GetTriggerNames() : null;

        public override Sprite GetMapImage(int map) => _device != null ? _device.GetMapImage(map) : null;

        public override string productName
        {
            get => _device == null ? _productName : _device.productName;
            set => _productName = value;
        }

        public override string productGUID
        {
            get => _device == null ? _productGUID : _device.productGUID;
            set => _productName = value;
        }

        public override bool isConnected => _device != null && _device.isConnected;

        public float leftTrigger => _device == null ? 0f : _device.leftTrigger;

        public float rightTrigger => _device == null ? 0f : _device.rightTrigger;

        public Vec2 leftStick => _device == null ? Vec2.Zero : _device.leftStick;

        public Vec2 rightStick => _device == null ? Vec2.Zero : _device.rightStick;

        public GenericController(int index)
          : base(index)
        {
        }

        public override bool MapPressed(int mapping, bool any = false) => _device != null && _device.MapPressed(mapping, any);

        public override bool MapReleased(int mapping) => _device != null && _device.MapReleased(mapping);

        public override bool MapDown(int mapping, bool any = false) => _device != null && _device.MapDown(mapping, any);

        public override void Rumble(float leftIntensity = 0f, float rightIntensity = 0f)
        {
            if (!(device is XInputPad))
                return;
            (device as XInputPad).Rumble(Math.Min(leftIntensity * 1.5f, 1f), Math.Min(rightIntensity * 1.5f, 1f));
        }
    }
}
