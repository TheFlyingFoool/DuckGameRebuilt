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
        private Dictionary<string, BullshitInput.CountdownPair> _bullshitTriggerStates = new Dictionary<string, BullshitInput.CountdownPair>()
    {
      {
        "LEFT",
        new BullshitInput.CountdownPair()
      },
      {
        "RIGHT",
        new BullshitInput.CountdownPair()
      },
      {
        "UP",
        new BullshitInput.CountdownPair()
      },
      {
        "DOWN",
        new BullshitInput.CountdownPair()
      },
      {
        "JUMP",
        new BullshitInput.CountdownPair()
      },
      {
        "QUACK",
        new BullshitInput.CountdownPair()
      },
      {
        "SHOOT",
        new BullshitInput.CountdownPair()
      },
      {
        "GRAB",
        new BullshitInput.CountdownPair()
      },
      {
        "RAGDOLL",
        new BullshitInput.CountdownPair()
      },
      {
        "STRAFE",
        new BullshitInput.CountdownPair()
      },
      {
        "SELECT",
        new BullshitInput.CountdownPair()
      },
      {
        "LTRIGGER",
        new BullshitInput.CountdownPair()
      },
      {
        "RTRIGGER",
        new BullshitInput.CountdownPair()
      },
      {
        "LSTICK",
        new BullshitInput.CountdownPair()
      },
      {
        "RSTICK",
        new BullshitInput.CountdownPair()
      }
    };

        public AILocomotion locomotion => this._locomotion;

        public override bool Pressed(string trigger, bool any = false) => this._bullshitTriggerStates.ContainsKey(trigger) && _bullshitTriggerStates[trigger].current > this._bullshitTriggerStates[trigger].previous;

        public override bool Released(string trigger) => this._bullshitTriggerStates.ContainsKey(trigger) && _bullshitTriggerStates[trigger].current <= 0f && _bullshitTriggerStates[trigger].previous > 0f;

        public override bool Down(string trigger) => this._bullshitTriggerStates.ContainsKey(trigger) && _bullshitTriggerStates[trigger].current > 0f;

        public BullshitInput()
          : base()
        {
        }

        public override void UpdateExtraInput()
        {
            foreach (KeyValuePair<string, BullshitInput.CountdownPair> bullshitTriggerState in this._bullshitTriggerStates)
            {
                bullshitTriggerState.Value.previous = bullshitTriggerState.Value.current;
                if (Rando.Int(100) == 0)
                    this._bullshitTriggerStates[bullshitTriggerState.Key].current = Rando.Float(2f);
                this._bullshitTriggerStates[bullshitTriggerState.Key].current -= Maths.IncFrameTimer();
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
