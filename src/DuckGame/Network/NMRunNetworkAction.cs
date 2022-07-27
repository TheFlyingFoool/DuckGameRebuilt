// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRunNetworkAction
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Reflection;

namespace DuckGame
{
    public class NMRunNetworkAction : NMEvent
    {
        public PhysicsObject target;
        public byte actionIndex;

        public NMRunNetworkAction(PhysicsObject pTarget, byte pNetworkActionIndex)
        {
            this.target = pTarget;
            this.actionIndex = pNetworkActionIndex;
        }

        public NMRunNetworkAction()
        {
        }

        public override void Activate()
        {
            if (this.target == null || this.actionIndex == byte.MaxValue)
                return;
            MethodInfo methodInfo = Editor.MethodFromNetworkActionIndex(this.target.GetType(), this.actionIndex);
            if (!(methodInfo != null))
                return;
            methodInfo.Invoke(target, null);
        }
    }
}
