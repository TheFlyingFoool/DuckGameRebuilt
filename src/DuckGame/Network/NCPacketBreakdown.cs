// Decompiled with JetBrains decompiler
// Type: DuckGame.NCPacketBreakdown
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
                if (NCPacketBreakdown._dataTypes == null)
                    NCPacketBreakdown._dataTypes = Enum.GetValues(typeof(NCPacketDataType)).Cast<NCPacketDataType>();
                return NCPacketBreakdown._dataTypes;
            }
        }

        public NCPacketBreakdown()
        {
            if (NCPacketBreakdown._dataTypes == null)
                NCPacketBreakdown._dataTypes = Enum.GetValues(typeof(NCPacketDataType)).Cast<NCPacketDataType>();
            foreach (NCPacketDataType dataType in NCPacketBreakdown._dataTypes)
                this._bitsPerType[dataType] = 0;
        }

        public void Add(NCPacketDataType type, int bits) => this._bitsPerType[type] += bits;

        public int Get(NCPacketDataType type) => this._bitsPerType[type];

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
