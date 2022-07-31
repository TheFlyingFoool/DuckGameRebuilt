// Decompiled with JetBrains decompiler
// Type: DuckGame.GenericController
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._device;
            set
            {
                if (this._device != null)
                    this._device.genericController = null;
                this._device = value;
                if (this._device == null)
                    return;
                this._device.genericController = this;
            }
        }

        public override Dictionary<int, string> GetTriggerNames() => this._device != null ? this._device.GetTriggerNames() : null;

        public override Sprite GetMapImage(int map) => this._device != null ? this._device.GetMapImage(map) : null;

        public override string productName
        {
            get => this._device == null ? this._productName : this._device.productName;
            set => this._productName = value;
        }

        public override string productGUID
        {
            get => this._device == null ? this._productGUID : this._device.productGUID;
            set => this._productName = value;
        }

        public override bool isConnected => this._device != null && this._device.isConnected;

        public float leftTrigger => this._device == null ? 0f : this._device.leftTrigger;

        public float rightTrigger => this._device == null ? 0f : this._device.rightTrigger;

        public Vec2 leftStick => this._device == null ? Vec2.Zero : this._device.leftStick;

        public Vec2 rightStick => this._device == null ? Vec2.Zero : this._device.rightStick;

        public GenericController(int index)
          : base(index)
        {
        }

        public override bool MapPressed(int mapping, bool any = false) => this._device != null && this._device.MapPressed(mapping, any);

        public override bool MapReleased(int mapping) => this._device != null && this._device.MapReleased(mapping);

        public override bool MapDown(int mapping, bool any = false) => this._device != null && this._device.MapDown(mapping, any);

        public override void Rumble(float leftIntensity = 0f, float rightIntensity = 0f)
        {
            if (!(this.device is XInputPad))
                return;
            (this.device as XInputPad).Rumble(Math.Min(leftIntensity * 1.5f, 1f), Math.Min(rightIntensity * 1.5f, 1f));
        }
    }
}
