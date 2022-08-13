// Decompiled with JetBrains decompiler
// Type: DuckGame.NMNewPong
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [FixedNetworkID(30015)]
    public class NMNewPong : NMNetworkCoreMessage
    {
        public byte index;

        public NMNewPong(byte pIndex) => index = pIndex;

        public NMNewPong()
        {
        }
    }
}
