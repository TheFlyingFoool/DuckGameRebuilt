// Decompiled with JetBrains decompiler
// Type: DuckGame.DuckAI
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class DuckAI : InputProfile
    {
        //private Stack<AIState> _state = new Stack<AIState>();
        private Dictionary<string, InputState> _inputState = new Dictionary<string, InputState>();
       // private AILocomotion _locomotion = new AILocomotion();
        //public bool canRefresh;
        public InputProfile _manualQuack;
        //private int quackWait = 10;
        //private int jumpWait = 10;
        public bool virtualQuack;

       // public AILocomotion locomotion => this._locomotion;

        public void Press(string trigger) => _inputState[trigger] = InputState.Pressed;

        public void HoldDown(string trigger) => _inputState[trigger] = InputState.Down;

        public void Release(string trigger) => _inputState[trigger] = InputState.Released;

        public override bool Pressed(string trigger, bool any = false)
        {
            InputState inputState;
            return _inputState.TryGetValue(trigger, out inputState) && inputState == InputState.Pressed;
        }

        public override bool Released(string trigger)
        {
            InputState inputState;
            return _inputState.TryGetValue(trigger, out inputState) && inputState == InputState.Released;
        }

        public override bool Down(string trigger)
        {
            InputState inputState;
            if (!_inputState.TryGetValue(trigger, out inputState))
                return false;
            return inputState == InputState.Pressed || inputState == InputState.Down;
        }

        //public bool SetTarget(Vec2 t)
        //{
        //    this._locomotion.target = t;
        //    return this._locomotion.target == Vec2.Zero;
        //}

        //public void TrimLastTarget() => this._locomotion.TrimLastTarget();

        public DuckAI(InputProfile manualQuacker = null)
          : base()
        {
            //this._state.Push(new AIStateDeathmatchBot());
            _manualQuack = manualQuacker;
        }

        //public virtual void Update(Duck duck)
        //{
        //    this.Release("GRAB");
        //    this.Release("SHOOT");
        //    this._locomotion.Update(this, duck);
        //    if (this.jumpWait > 0)
        //    {
        //        --this.jumpWait;
        //    }
        //    else
        //    {
        //        this.jumpWait = 10;
        //        this._locomotion.Jump(5);
        //    }
        //    if (this.quackWait > 0)
        //    {
        //        --this.quackWait;
        //    }
        //    else
        //    {
        //        this.quackWait = 4;
        //        this._locomotion.Quack(2);
        //    }
        //}

        public override void UpdateExtraInput()
        {
            if (_inputState.ContainsKey("QUACK") && _inputState["QUACK"] == InputState.Pressed)
                _inputState["QUACK"] = InputState.Down;
            if (_inputState.ContainsKey("STRAFE") && _inputState["STRAFE"] == InputState.Pressed)
                _inputState["STRAFE"] = InputState.Down;
            if (_manualQuack == null)
                return;
            if (_manualQuack.Pressed("QUACK"))
                Press("QUACK");
            else if (_manualQuack.Released("QUACK"))
                Release("QUACK");
            if (_manualQuack.Pressed("STRAFE"))
            {
                Press("STRAFE");
            }
            else
            {
                if (!_manualQuack.Released("STRAFE"))
                    return;
                Release("STRAFE");
            }
        }

        public override float leftTrigger
        {
            get
            {
                if (virtualQuack)
                    return virtualDevice.leftTrigger;
                return _manualQuack != null ? _manualQuack.leftTrigger : 0f;
            }
        }

        //public void Draw()
        //{
        //    if (this._locomotion.pathFinder.path == null)
        //        return;
        //    Vec2 p1 = Vec2.Zero;
        //    foreach (PathNodeLink pathNodeLink in this._locomotion.pathFinder.path)
        //    {
        //        if (p1 != Vec2.Zero)
        //            Graphics.DrawLine(p1, pathNodeLink.owner.position, new Color((int)byte.MaxValue, 0, (int)byte.MaxValue), 2f, (Depth)0.9f);
        //        p1 = pathNodeLink.owner.position;
        //    }
        //}
    }
}
