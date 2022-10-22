// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPresentOpen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMPresentOpen : NMEvent
    {
        public Vec2 position;
        public byte frame;

        public NMPresentOpen()
        {
        }

        public NMPresentOpen(Vec2 pPosition, byte pFrame)
        {
            position = pPosition;
            frame = pFrame;
        }

        public override void Activate()
        {
            Present.OpenEffect(position, frame, true);
            base.Activate();
        }
    }
}
