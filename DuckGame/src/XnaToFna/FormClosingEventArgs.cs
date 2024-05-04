using System.ComponentModel;

namespace XnaToFna.ProxyForms
{
    public class FormClosingEventArgs : CancelEventArgs
    {
        public CloseReason CloseReason { get; private set; }

        public FormClosingEventArgs(CloseReason closeReason, bool cancel)
          : base(cancel)
        {
            CloseReason = closeReason;
        }
    }
}
