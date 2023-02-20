// Decompiled with JetBrains decompiler
// Type: DuckGame.NMMojiData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMMojiData : NMDuckNetwork
    {
        public string data;
        public string name;

        public NMMojiData()
        {
        }

        public NMMojiData(string dat, string nam)
        {
            data = dat;
            name = nam;
        }

        protected override void OnSerialize()
        {
            _serializedData.Write(name);
            _serializedData.Write(data);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            name = d.ReadString();
            data = d.ReadString();
        }
    }
}
