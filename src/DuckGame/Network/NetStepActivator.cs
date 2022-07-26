// Decompiled with JetBrains decompiler
// Type: DuckGame.NetStepActivator
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NetStepActivator
    {
        private NetStepActivator.Function _function;
        private int _index;

        public NetStepActivator(NetStepActivator.Function del) => this._function = del;

        public int index
        {
            get => this._index;
            set
            {
                this._index = value;
                if (this._index <= 3)
                    return;
                this._index = 0;
            }
        }

        public void Activate()
        {
            if (this._function != null)
                this._function();
            this.Step();
        }

        public void Step() => ++this.index;

        public delegate void Function();
    }
}
