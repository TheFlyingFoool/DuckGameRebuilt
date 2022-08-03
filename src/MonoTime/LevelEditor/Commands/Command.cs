// Decompiled with JetBrains decompiler
// Type: DuckGame.Command
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Command
    {
        private bool _done;
        private bool _inverse;

        public Command Inverse()
        {
            _inverse = !_inverse;
            return this;
        }

        public void Do()
        {
            if (_done)
                return;
            if (_inverse)
                OnUndo();
            else
                OnDo();
            _done = true;
        }

        public void Undo()
        {
            if (!_done)
                return;
            if (_inverse)
                OnDo();
            else
                OnUndo();
            _done = false;
        }

        public virtual void OnDo()
        {
        }

        public virtual void OnUndo()
        {
        }
    }
}
