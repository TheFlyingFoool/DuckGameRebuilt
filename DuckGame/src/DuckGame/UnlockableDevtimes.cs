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
            _persona = Persona.alllist[Rando.Int(3)];
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
