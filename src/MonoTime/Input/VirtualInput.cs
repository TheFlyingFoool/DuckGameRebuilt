// Decompiled with JetBrains decompiler
// Type: DuckGame.VirtualInput
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class VirtualInput : InputDevice
    {
        public static List<VirtualInput> debuggerInputs = new List<VirtualInput>()
    {
      new VirtualInput(0),
      new VirtualInput(0),
      new VirtualInput(0),
      new VirtualInput(0),
      new VirtualInput(0),
      new VirtualInput(0),
      new VirtualInput(0),
      new VirtualInput(0)
    };
        public int pdraw;
        private ushort _state;
        private ushort _prevState;
        private List<string> _availableTriggers = new List<string>();
        public Vec2 leftStick;
        public Vec2 rightStick;
        public float leftTrigger;
        public float rightTrigger;
        public bool setThisFrame;

        public ushort state
        {
            get => _state;
            set => _state = value;
        }

        public ushort prevState
        {
            get => _prevState;
            set => _prevState = value;
        }

        public List<string> availableTriggers
        {
            get => _availableTriggers;
            set => _availableTriggers = value;
        }

        public VirtualInput(int idx)
          : base(idx)
        {
            _name = "virtual" + idx.ToString();
        }

        public override void Update()
        {
        }

        private bool GetState(int mapping, bool prev = false) => ((prev ? _prevState : _state) & 1 << _availableTriggers.Count - mapping) != 0;

        public void SetState(ushort val, bool flagPrev = true)
        {
            if (flagPrev)
                _prevState = _state;
            _state = val;
            setThisFrame = true;
            leftStick = Vec2.Zero;
            rightStick = Vec2.Zero;
        }

        public override bool MapPressed(int mapping, bool any = false)
        {
            if (any)
            {
                for (int mapping1 = 0; mapping1 < _availableTriggers.Count; ++mapping1)
                {
                    if (MapPressed(mapping1, false))
                        return true;
                }
                return false;
            }
            return GetState(mapping) && !GetState(mapping, true);
        }

        public override bool MapReleased(int mapping) => !GetState(mapping) && GetState(mapping, true);

        public override bool MapDown(int mapping, bool any = false)
        {
            if (any)
                return _state > 0;
            return GetState(mapping);
        }
    }
}
