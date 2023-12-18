using System.Reflection;

namespace DuckGame
{
    public class NMRunNetworkAction : NMEvent
    {
        public PhysicsObject target;
        public byte actionIndex;

        public NMRunNetworkAction(PhysicsObject pTarget, byte pNetworkActionIndex)
        {
            target = pTarget;
            actionIndex = pNetworkActionIndex;
        }

        public NMRunNetworkAction()
        {
        }

        public override void Activate()
        {
            if (target == null || actionIndex == byte.MaxValue)
                return;
            MethodInfo methodInfo = Editor.MethodFromNetworkActionIndex(target.GetType(), actionIndex);
            if (!(methodInfo != null))
                return;
            methodInfo.Invoke(target, null);
        }
    }
}
