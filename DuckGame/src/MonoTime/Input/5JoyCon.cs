using System.Runtime.CompilerServices;

namespace DuckGame
{
    public class SwitchHandheldController : SwitchJoyConDual
    {
        public SwitchHandheldController()
          : base(0)
        {
        }

        public extern override bool isConnected { [MethodImpl(MethodImplOptions.InternalCall)] get; }
    }
}
