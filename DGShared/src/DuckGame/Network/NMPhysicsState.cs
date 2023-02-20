// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPhysicsState
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMPhysicsState : NMObjectMessage
    {
        public Vec2 position;
        public Vec2 velocity;
        public int clientFrame;

        public NMPhysicsState()
        {
        }

        public NMPhysicsState(Vec2 Position, Vec2 Velocity, ushort ObjectID, int ClientFrame)
          : base(ObjectID)
        {
            position = Position;
            velocity = Velocity;
            clientFrame = ClientFrame;
        }
    }
}
