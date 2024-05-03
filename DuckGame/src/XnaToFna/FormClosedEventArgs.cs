using System.ComponentModel;

namespace XnaToFna.ProxyForms
{
    public class FormClosedEventArgs : CancelEventArgs
    {
        public CloseReason CloseReason { get; private set; }

        public FormClosedEventArgs(CloseReason closeReason) => CloseReason = closeReason;
    }
}
