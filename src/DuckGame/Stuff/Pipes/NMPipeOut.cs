// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPipeOut
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMPipeOut : NMEvent
    {
        public Vec2 position;
        public byte direction;

        public NMPipeOut(Vec2 pPosition, byte pDirection)
        {
            this.direction = pDirection;
            this.position = pPosition;
        }

        public NMPipeOut()
        {
        }

        public override void Activate()
        {
            for (int index = 0; index < 6; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(this.position.x + Rando.Float(-4f, 4f), this.position.y + Rando.Float(-4f, 4f));
                if (this.direction == (byte)0)
                {
                    smallSmoke.velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.0f, -0.5f));
                    Level.current.CollisionPoint<PipeTileset>(this.position)?.FlapPipe();
                }
                else if (this.direction == (byte)1)
                {
                    smallSmoke.velocity = new Vec2(Rando.Float(0.2f, 0.7f), Rando.Float(-0.5f, 0.5f));
                    Level.current.CollisionPoint<PipeTileset>(this.position + new Vec2(-10f, 0.0f))?.FlapPipe();
                }
                else if (this.direction == (byte)3)
                {
                    smallSmoke.velocity = new Vec2(Rando.Float(-0.7f, -0.2f), Rando.Float(-0.5f, 0.5f));
                    Level.current.CollisionPoint<PipeTileset>(this.position + new Vec2(10f, 0.0f))?.FlapPipe();
                }
                else if (this.direction == (byte)2)
                {
                    smallSmoke.velocity = new Vec2(Rando.Float(-0.5f, 0.5f), Rando.Float(0.2f, 0.7f));
                    Level.current.CollisionPoint<PipeTileset>(this.position + new Vec2(0.0f, -10f))?.FlapPipe();
                }
                Level.Add((Thing)smallSmoke);
            }
            SFX.Play("pipeOut", pitch: Rando.Float(-0.1f, 0.1f));
            base.Activate();
        }
    }
}
