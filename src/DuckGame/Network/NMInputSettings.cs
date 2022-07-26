// Decompiled with JetBrains decompiler
// Type: DuckGame.NMInputSettings
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [FixedNetworkID(30232)]
    public class NMInputSettings : NMDuckNetworkEvent
    {
        private InputDevice _device;
        private InputProfile _inputProfile;
        private Profile _profile;
        private DeviceInputMapping _mapping;
        private byte _index;
        private bool _valid = true;

        public NMInputSettings()
        {
        }

        public NMInputSettings(Profile pro, InputDevice d = null)
        {
            this._device = d != null ? d : pro.inputProfile.lastActiveDevice;
            if (this._device != null && this._device.productName == null && this._device.productGUID == null)
                this._device = (InputDevice)null;
            this._index = pro.networkIndex;
            this._profile = pro;
            this._inputProfile = pro.inputProfile;
        }

        protected override void OnSerialize()
        {
            if (this._serializedData == null || this._device == null || this._inputProfile == null)
            {
                this._serializedData.Write(false);
            }
            else
            {
                this._serializedData.Write(true);
                this._serializedData.Write(this._index);
                this._serializedData.Write(this._device.inputDeviceType);
                MultiMap<string, int> multiMap = (MultiMap<string, int>)null;
                if (this._device != null)
                    multiMap = this._inputProfile.GetMappings(this._device.GetType());
                if (multiMap != null)
                {
                    this.serializedData.Write(true);
                    byte val1 = 0;
                    foreach (KeyValuePair<string, List<int>> keyValuePair in (MultiMap<string, int, List<int>>)multiMap)
                    {
                        if (keyValuePair.Value.Count > 0 && Triggers.toIndex.ContainsKey(keyValuePair.Key))
                            ++val1;
                    }
                    this.serializedData.Write(val1);
                    foreach (KeyValuePair<string, List<int>> keyValuePair in (MultiMap<string, int, List<int>>)multiMap)
                    {
                        if (keyValuePair.Value.Count > 0 && Triggers.toIndex.ContainsKey(keyValuePair.Key))
                        {
                            this.serializedData.Write(Triggers.toIndex[keyValuePair.Key]);
                            this.serializedData.Write(keyValuePair.Value[0]);
                        }
                    }
                    DeviceInputMapping deviceInputMapping = this._device.overrideMap == null ? Input.GetDefaultMapping(this._device.productName, this._device.productGUID, p: this._profile) : this._device.overrideMap;
                    if (deviceInputMapping.graphicMap.Count > 0)
                    {
                        this.serializedData.Write(true);
                        this.serializedData.Write((byte)deviceInputMapping.graphicMap.Count);
                        foreach (KeyValuePair<int, string> graphic in deviceInputMapping.graphicMap)
                        {
                            KeyValuePair<int, string> pair = graphic;
                            Sprite sprite = Input.buttonStyles.FirstOrDefault<Sprite>((Func<Sprite, bool>)(x => x.texture != null && x.texture.textureName == pair.Value));
                            byte val2 = 0;
                            if (sprite != null)
                                val2 = (byte)Input.buttonStyles.IndexOf(sprite);
                            this.serializedData.Write(pair.Key);
                            this.serializedData.Write(val2);
                        }
                    }
                    else
                        this.serializedData.Write(false);
                }
                else
                    this.serializedData.Write(false);
                base.OnSerialize();
            }
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            if (!msg.ReadBool())
            {
                this._valid = false;
            }
            else
            {
                this._index = msg.ReadByte();
                byte num1 = msg.ReadByte();
                DeviceInputMapping deviceInputMapping = new DeviceInputMapping();
                deviceInputMapping.inputOverrideType = (int)num1;
                switch (num1)
                {
                    case 0:
                        deviceInputMapping.deviceOverride = (InputDevice)new Keyboard("", 0);
                        break;
                    case 1:
                        deviceInputMapping.deviceOverride = (InputDevice)new XInputPad(0);
                        break;
                    default:
                        deviceInputMapping.deviceOverride = (InputDevice)new DInputPad(0);
                        break;
                }
                deviceInputMapping.deviceOverride.overrideMap = deviceInputMapping;
                if (msg.ReadBool())
                {
                    byte num2 = msg.ReadByte();
                    for (int index = 0; index < (int)num2; ++index)
                    {
                        byte key = msg.ReadByte();
                        int pIndex = msg.ReadInt();
                        deviceInputMapping.MapInput(Triggers.fromIndex[key], pIndex);
                    }
                    if (msg.ReadBool())
                    {
                        byte num3 = msg.ReadByte();
                        for (int index1 = 0; index1 < (int)num3; ++index1)
                        {
                            int key = msg.ReadInt();
                            int index2 = (int)msg.ReadByte();
                            deviceInputMapping.graphicMap[key] = Input.buttonStyles[index2].texture.textureName;
                        }
                    }
                }
                this._mapping = deviceInputMapping;
                base.OnDeserialize(msg);
            }
        }

        public override void Activate()
        {
            if (this._index < (byte)0 || this._index > (byte)3 || !this._valid)
                return;
            Profile profile = DuckNetwork.profiles[(int)this._index];
            profile.inputMappingOverrides.RemoveAll((Predicate<DeviceInputMapping>)(x => x.inputOverrideType == this._mapping.inputOverrideType));
            profile.inputMappingOverrides.Add(this._mapping);
            foreach (KeyValuePair<string, int> keyValuePair in this._mapping.map)
                profile.inputProfile.Map(this._mapping.deviceOverride, keyValuePair.Key, keyValuePair.Value, true);
            profile.inputProfile.lastActiveOverride = this._mapping.deviceOverride;
            base.Activate();
        }
    }
}
