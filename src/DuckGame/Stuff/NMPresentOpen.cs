// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPresentOpen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.position = pPosition;
            this.frame = pFrame;
        }

        public override void Activate()
        {
            Present.OpenEffect(this.position, (int)this.frame, true);
            base.Activate();
        }
    }
}
