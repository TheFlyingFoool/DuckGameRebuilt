// Decompiled with JetBrains decompiler
// Type: Microsoft.Xna.Framework.Input.KeyEventArgs
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
