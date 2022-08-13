// Decompiled with JetBrains decompiler
// Type: DuckGame.NMInputSettings
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            _device = d != null ? d : pro.inputProfile.lastActiveDevice;
            if (_device != null && _device.productName == null && _device.productGUID == null)
                _device = null;
            _index = pro.networkIndex;
            _profile = pro;
            _inputProfile = pro.inputProfile;
        }

        protected override void OnSerialize()
        {
            if (_serializedData == null || _device == null || _inputProfile == null)
            {
                _serializedData.Write(false);
            }
            else
            {
                _serializedData.Write(true);
                _serializedData.Write(_index);
                _serializedData.Write(_device.inputDeviceType);
                MultiMap<string, int> multiMap = null;
                if (_device != null)
                    multiMap = _inputProfile.GetMappings(_device.GetType());
                if (multiMap != null)
                {
                    serializedData.Write(true);
                    byte val1 = 0;
                    foreach (KeyValuePair<string, List<int>> keyValuePair in (MultiMap<string, int, List<int>>)multiMap)
                    {
                        if (keyValuePair.Value.Count > 0 && Triggers.toIndex.ContainsKey(keyValuePair.Key))
                            ++val1;
                    }
                    serializedData.Write(val1);
                    foreach (KeyValuePair<string, List<int>> keyValuePair in (MultiMap<string, int, List<int>>)multiMap)
                    {
                        if (keyValuePair.Value.Count > 0 && Triggers.toIndex.ContainsKey(keyValuePair.Key))
                        {
                            serializedData.Write(Triggers.toIndex[keyValuePair.Key]);
                            serializedData.Write(keyValuePair.Value[0]);
                        }
                    }
                    DeviceInputMapping deviceInputMapping = _device.overrideMap == null ? Input.GetDefaultMapping(_device.productName, _device.productGUID, p: _profile) : _device.overrideMap;
                    if (deviceInputMapping.graphicMap.Count > 0)
                    {
                        serializedData.Write(true);
                        serializedData.Write((byte)deviceInputMapping.graphicMap.Count);
                        foreach (KeyValuePair<int, string> graphic in deviceInputMapping.graphicMap)
                        {
                            KeyValuePair<int, string> pair = graphic;
                            Sprite sprite = Input.buttonStyles.FirstOrDefault<Sprite>(x => x.texture != null && x.texture.textureName == pair.Value);
                            byte val2 = 0;
                            if (sprite != null)
                                val2 = (byte)Input.buttonStyles.IndexOf(sprite);
                            serializedData.Write(pair.Key);
                            serializedData.Write(val2);
                        }
                    }
                    else
                        serializedData.Write(false);
                }
                else
                    serializedData.Write(false);
                base.OnSerialize();
            }
        }

        public override void OnDeserialize(BitBuffer msg)
        {
            if (!msg.ReadBool())
            {
                _valid = false;
            }
            else
            {
                _index = msg.ReadByte();
                byte num1 = msg.ReadByte();
                DeviceInputMapping deviceInputMapping = new DeviceInputMapping
                {
                    inputOverrideType = num1
                };
                switch (num1)
                {
                    case 0:
                        deviceInputMapping.deviceOverride = new Keyboard("", 0);
                        break;
                    case 1:
                        deviceInputMapping.deviceOverride = new XInputPad(0);
                        break;
                    default:
                        deviceInputMapping.deviceOverride = new DInputPad(0);
                        break;
                }
                deviceInputMapping.deviceOverride.overrideMap = deviceInputMapping;
                if (msg.ReadBool())
                {
                    byte num2 = msg.ReadByte();
                    for (int index = 0; index < num2; ++index)
                    {
                        byte key = msg.ReadByte();
                        int pIndex = msg.ReadInt();
                        deviceInputMapping.MapInput(Triggers.fromIndex[key], pIndex);
                    }
                    if (msg.ReadBool())
                    {
                        byte num3 = msg.ReadByte();
                        for (int index1 = 0; index1 < num3; ++index1)
                        {
                            int key = msg.ReadInt();
                            int index2 = msg.ReadByte();
                            deviceInputMapping.graphicMap[key] = Input.buttonStyles[index2].texture.textureName;
                        }
                    }
                }
                _mapping = deviceInputMapping;
                base.OnDeserialize(msg);
            }
        }

        public override void Activate()
        {
            if (_index < 0 || _index > 3 || !_valid)
                return;
            Profile profile = DuckNetwork.profiles[_index];
            profile.inputMappingOverrides.RemoveAll(x => x.inputOverrideType == _mapping.inputOverrideType);
            profile.inputMappingOverrides.Add(_mapping);
            foreach (KeyValuePair<string, int> keyValuePair in _mapping.map)
                profile.inputProfile.Map(_mapping.deviceOverride, keyValuePair.Key, keyValuePair.Value, true);
            profile.inputProfile.lastActiveOverride = _mapping.deviceOverride;
            base.Activate();
        }
    }
}
