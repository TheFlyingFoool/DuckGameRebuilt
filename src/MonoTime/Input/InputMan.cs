// Decompiled with JetBrains decompiler
// Type: Microsoft.Xna.Framework.Input.CharacterEventArgs
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace Microsoft.Xna.Framework.Input
{
    public class CharacterEventArgs : EventArgs
    {
        private readonly char character;
        private readonly int lParam;

        public CharacterEventArgs(char character, int lParam)
        {
            this.character = character;
            this.lParam = lParam;
        }

        public char Character => character;

        public int Param => lParam;

        public int RepeatCount => lParam & ushort.MaxValue;

        public bool ExtendedKey => (lParam & 16777216) > 0;

        public bool AltPressed => (lParam & 536870912) > 0;

        public bool PreviousState => (lParam & 1073741824) > 0;

        public bool TransitionState => (lParam & int.MinValue) > 0;
    }
}
