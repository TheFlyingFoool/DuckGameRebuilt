// Decompiled with JetBrains decompiler
// Type: DuckGame.NMDuckPhysicsState
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMDuckPhysicsState : NMPhysicsState
    {
        public int inputState;
        public ushort holding;

        public NMDuckPhysicsState()
        {
        }

        public NMDuckPhysicsState(
          Vec2 Position,
          Vec2 Velocity,
          ushort ObjectID,
          int ClientFrame,
          int InputState,
          ushort Holding)
          : base(Position, Velocity, ObjectID, ClientFrame)
        {
            inputState = InputState;
            holding = Holding;
        }
    }
}
