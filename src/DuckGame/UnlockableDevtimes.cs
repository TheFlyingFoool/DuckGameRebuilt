// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockableDevtimes
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Linq;

namespace DuckGame
{
    public class UnlockableDevtimes : Unlockable
    {
        private DuckPersona _persona;

        public UnlockableDevtimes(string identifier, Func<bool> condition, string nam, string desc)
          : base(identifier, condition, nam, desc)
        {
            _persona = Persona.all.ElementAt<DuckPersona>(Rando.Int(3));
            _showScreen = true;
        }

        public override void Initialize()
        {
        }

        protected override void Unlock()
        {
        }

        protected override void Lock()
        {
        }

        public override void Draw(float x, float y, Depth depth)
        {
        }
    }
}
