using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class NCPacketBreakdown
    {
        private static IEnumerable<NCPacketDataType> _dataTypes;
        private Dictionary<NCPacketDataType, int> _bitsPerType = new Dictionary<NCPacketDataType, int>();

        public static IEnumerable<NCPacketDataType> dataTypes
        {
            get
            {
                if (_dataTypes == null)
                    _dataTypes = Enum.GetValues(typeof(NCPacketDataType)).Cast<NCPacketDataType>();
                return _dataTypes;
            }
        }

        public NCPacketBreakdown()
        {
            if (_dataTypes == null)
                _dataTypes = Enum.GetValues(typeof(NCPacketDataType)).Cast<NCPacketDataType>();
            foreach (NCPacketDataType dataType in _dataTypes)
                _bitsPerType[dataType] = 0;
        }

        public void Add(NCPacketDataType type, int bits) => _bitsPerType[type] += bits;

        public int Get(NCPacketDataType type) => _bitsPerType[type];

        public static Color GetTypeColor(NCPacketDataType type)
        {
            switch (type)
            {
                case NCPacketDataType.InputStream:
                    return Color.Pink;
                case NCPacketDataType.Ghost:
                    return Color.Red;
                case NCPacketDataType.Ack:
                    return Color.Lime;
                case NCPacketDataType.Event:
                    return Color.Blue;
                case NCPacketDataType.ExtraData:
                    return Color.White;
                default:
                    return Color.Yellow;
            }
        }
    }
}
