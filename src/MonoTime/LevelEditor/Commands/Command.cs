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
            this._inverse = !this._inverse;
            return this;
        }

        public void Do()
        {
            if (this._done)
                return;
            if (this._inverse)
                this.OnUndo();
            else
                this.OnDo();
            this._done = true;
        }

        public void Undo()
        {
            if (!this._done)
                return;
            if (this._inverse)
                this.OnDo();
            else
                this.OnUndo();
            this._done = false;
        }

        public virtual void OnDo()
        {
        }

        public virtual void OnUndo()
        {
        }
    }
}
