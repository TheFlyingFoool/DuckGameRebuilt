using System;

namespace Microsoft.Xna.Framework.Input
{
    public class KeyEventArgs : EventArgs
    {
        private Keys keyCode;

        public KeyEventArgs(Keys keyCode) => this.keyCode = keyCode;

        public Keys KeyCode => keyCode;
    }
}
