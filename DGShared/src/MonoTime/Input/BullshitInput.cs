//// Decompiled with JetBrains decompiler
//// Type: DuckGame.BullshitInput
////removed for regex reasons Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class BullshitInput : InputProfile
    {
        private Stack<AIState> _state = new Stack<AIState>();
        private Dictionary<string, InputState> _inputState = new Dictionary<string, InputState>();
        private AILocomotion _locomotion = new AILocomotion();
        private Dictionary<string, CountdownPair> _bullshitTriggerStates = new Dictionary<string, CountdownPair>()
    {
      {
        Triggers.Left,
        new CountdownPair()
      },
      {
        Triggers.Right,
        new CountdownPair()
      },
      {
        Triggers.Up,
        new CountdownPair()
      },
      {
        Triggers.Down,
        new CountdownPair()
      },
      {
        Triggers.Jump,
        new CountdownPair()
      },
      {
        Triggers.Quack,
        new CountdownPair()
      },
      {
        Triggers.Shoot,
        new CountdownPair()
      },
      {
        Triggers.Grab,
        new CountdownPair()
      },
      {
        Triggers.Ragdoll,
        new CountdownPair()
      },
      {
        Triggers.Strafe,
        new CountdownPair()
      },
      {
        Triggers.Select,
        new CountdownPair()
      },
      {
        Triggers.LeftTrigger,
        new CountdownPair()
      },
      {
        Triggers.RightTrigger,
        new CountdownPair()
      },
      {
        Triggers.LeftStick,
        new CountdownPair()
      },
      {
        Triggers.RightStick,
        new CountdownPair()
      }
    };

        public AILocomotion locomotion => _locomotion;

        public override bool Pressed(string trigger, bool any = false) => _bullshitTriggerStates.ContainsKey(trigger) && _bullshitTriggerStates[trigger].current > _bullshitTriggerStates[trigger].previous;

        public override bool Released(string trigger) => _bullshitTriggerStates.ContainsKey(trigger) && _bullshitTriggerStates[trigger].current <= 0f && _bullshitTriggerStates[trigger].previous > 0f;

        public override bool Down(string trigger) => _bullshitTriggerStates.ContainsKey(trigger) && _bullshitTriggerStates[trigger].current > 0f;

        public BullshitInput()
          : base()
        {
        }

        public override void UpdateExtraInput()
        {
            foreach (KeyValuePair<string, CountdownPair> bullshitTriggerState in _bullshitTriggerStates)
            {
                bullshitTriggerState.Value.previous = bullshitTriggerState.Value.current;
                if (Rando.Int(100) == 0)
                    _bullshitTriggerStates[bullshitTriggerState.Key].current = Rando.Float(2f);
                _bullshitTriggerStates[bullshitTriggerState.Key].current -= Maths.IncFrameTimer();
            }
        }

        public override float leftTrigger => 0f;

        private class CountdownPair
        {
            public float current;
            public float previous;
        }
    }
}
