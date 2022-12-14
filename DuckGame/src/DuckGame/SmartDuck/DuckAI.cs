using System;
using System.Collections.Generic;

namespace DuckGame
{
	public class DuckAI : InputProfile
	{
		public AILocomotion locomotion
		{
			get
			{
				return this._locomotion;
			}
		}

		public void Press(string trigger)
		{
			this._inputState[trigger] = InputState.Pressed;
		}

		public void HoldDown(string trigger)
		{
			this._inputState[trigger] = InputState.Down;
		}

		public void Release(string trigger)
		{
			this._inputState[trigger] = InputState.Released;
		}

		public override bool Pressed(string trigger, bool any = false)
		{
			InputState outVal;
			return this._inputState.TryGetValue(trigger, out outVal) && outVal == InputState.Pressed;
		}

		public override bool Released(string trigger)
		{
			InputState outVal;
			return this._inputState.TryGetValue(trigger, out outVal) && outVal == InputState.Released;
		}

		public override bool Down(string trigger)
		{
			InputState outVal;
			return this._inputState.TryGetValue(trigger, out outVal) && (outVal == InputState.Pressed || outVal == InputState.Down);
		}

		public bool SetTarget(Vec2 t)
		{
			this._locomotion.target = t;
			return this._locomotion.target == Vec2.Zero;
		}

		public void TrimLastTarget()
		{
			this._locomotion.TrimLastTarget();
		}

		public DuckAI(InputProfile manualQuacker = null) : base("")
		{
			this._state.Push(new AIStateDeathmatchBot());
			this._manualQuack = manualQuacker;
		}

		public virtual void Update(Duck duck)
		{
			this.Release("GRAB");
			this.Release("SHOOT");
			this._locomotion.Update(this, duck);
			if (this.jumpWait > 0)
			{
				this.jumpWait--;
			}
			else
			{
				this.jumpWait = 10;
				this._locomotion.Jump(5);
			}
			if (this.quackWait > 0)
			{
				this.quackWait--;
				return;
			}
			this.quackWait = 4;
			this._locomotion.Quack(2);
		}

		public override void UpdateExtraInput()
		{
			if (this._inputState.ContainsKey("QUACK") && this._inputState["QUACK"] == InputState.Pressed)
			{
				this._inputState["QUACK"] = InputState.Down;
			}
			if (this._inputState.ContainsKey("STRAFE") && this._inputState["STRAFE"] == InputState.Pressed)
			{
				this._inputState["STRAFE"] = InputState.Down;
			}
			if (this._manualQuack != null)
			{
				if (this._manualQuack.Pressed("QUACK", false))
				{
					this.Press("QUACK");
				}
				else if (this._manualQuack.Released("QUACK"))
				{
					this.Release("QUACK");
				}
				if (this._manualQuack.Pressed("STRAFE", false))
				{
					this.Press("STRAFE");
					return;
				}
				if (this._manualQuack.Released("STRAFE"))
				{
					this.Release("STRAFE");
				}
			}
		}

		public override float leftTrigger
		{
			get
			{
				if (this.virtualQuack)
				{
					return virtualDevice.leftTrigger;
				}
				if (this._manualQuack != null)
				{
					return this._manualQuack.leftTrigger;
				}
				return 0f;
			}
		}

		public void Draw()
		{
			if (this._locomotion.pathFinder.path != null)
			{
				Vec2 lastNode = Vec2.Zero;
				foreach (PathNodeLink i in this._locomotion.pathFinder.path)
				{
					if (lastNode != Vec2.Zero)
					{
						Graphics.DrawLine(lastNode, i.owner.position, new Color(255, 0, 255), 2f, 0.9f);
					}
					lastNode = i.owner.position;
				}
			}
		}

		private Stack<AIState> _state = new Stack<AIState>();

		private Dictionary<string, InputState> _inputState = new Dictionary<string, InputState>();

		private AILocomotion _locomotion = new AILocomotion();

		public bool canRefresh;

		public InputProfile _manualQuack;

		private int quackWait = 10;

		private int jumpWait = 10;

		public bool virtualQuack;
	}
}
