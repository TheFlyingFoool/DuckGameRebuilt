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

        public NetStepActivator(NetStepActivator.Function del) => _function = del;

        public int index
        {
            get => _index;
            set
            {
                _index = value;
                if (_index <= 3)
                    return;
                _index = 0;
            }
        }

        public void Activate()
        {
            if (_function != null)
                _function();
            Step();
        }

        public void Step() => ++index;

        public delegate void Function();
    }
}
