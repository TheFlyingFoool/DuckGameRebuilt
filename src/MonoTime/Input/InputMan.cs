// Decompiled with JetBrains decompiler
// Type: Microsoft.Xna.Framework.Input.CharacterEventArgs
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public char Character => this.character;

        public int Param => this.lParam;

        public int RepeatCount => this.lParam & ushort.MaxValue;

        public bool ExtendedKey => (this.lParam & 16777216) > 0;

        public bool AltPressed => (this.lParam & 536870912) > 0;

        public bool PreviousState => (this.lParam & 1073741824) > 0;

        public bool TransitionState => (this.lParam & int.MinValue) > 0;
    }
}
