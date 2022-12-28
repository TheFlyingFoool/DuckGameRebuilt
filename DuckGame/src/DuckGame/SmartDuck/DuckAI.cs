using System.Collections.Generic;

namespace DuckGame
{
    public class DuckAI : InputProfile
	{
		public AILocomotion locomotion
		{
			get
			{
				return _locomotion;
			}
		}

		public void Press(string trigger)
		{
			_inputState[trigger] = InputState.Pressed;
		}

		public void HoldDown(string trigger)
		{
			_inputState[trigger] = InputState.Down;
		}

		public void Release(string trigger)
		{
			_inputState[trigger] = InputState.Released;
		}

		public override bool Pressed(string trigger, bool any = false)
		{
			InputState outVal;
			return _inputState.TryGetValue(trigger, out outVal) && outVal == InputState.Pressed;
		}

		public override bool Released(string trigger)
		{
			InputState outVal;
			return _inputState.TryGetValue(trigger, out outVal) && outVal == InputState.Released;
		}

		public override bool Down(string trigger)
		{
			InputState outVal;
			return _inputState.TryGetValue(trigger, out outVal) && (outVal == InputState.Pressed || outVal == InputState.Down);
		}

		public bool SetTarget(Vec2 t)
		{
			_locomotion.target = t;
			return _locomotion.target == Vec2.Zero;
		}

		public void TrimLastTarget()
		{
			_locomotion.TrimLastTarget();
		}

		public DuckAI(InputProfile manualQuacker = null) : base("")
		{
			_state.Push(new AIStateDeathmatchBot());
			_manualQuack = manualQuacker;
		}

		public virtual void Update(Duck duck)
		{
			Release(Triggers.Grab);
			Release(Triggers.Shoot);
			_locomotion.Update(this, duck);
			if (jumpWait > 0)
			{
				jumpWait--;
			}
			else
			{
				jumpWait = 10;
				_locomotion.Jump(5);
			}
			if (quackWait > 0)
			{
				quackWait--;
				return;
			}
			quackWait = 4;
			_locomotion.Quack(2);
		}

		public override void UpdateExtraInput()
		{
			if (_inputState.ContainsKey(Triggers.Quack) && _inputState[Triggers.Quack] == InputState.Pressed)
			{
				_inputState[Triggers.Quack] = InputState.Down;
			}
			if (_inputState.ContainsKey(Triggers.Strafe) && _inputState[Triggers.Strafe] == InputState.Pressed)
			{
				_inputState[Triggers.Strafe] = InputState.Down;
			}
			if (_manualQuack != null)
			{
				if (_manualQuack.Pressed(Triggers.Quack, false))
				{
					Press(Triggers.Quack);
				}
				else if (_manualQuack.Released(Triggers.Quack))
				{
					Release(Triggers.Quack);
				}
				if (_manualQuack.Pressed(Triggers.Strafe, false))
				{
					Press(Triggers.Strafe);
					return;
				}
				if (_manualQuack.Released(Triggers.Strafe))
				{
					Release(Triggers.Strafe);
				}
			}
		}

		public override float leftTrigger
		{
			get
			{
				if (virtualQuack)
				{
					return virtualDevice.leftTrigger;
				}
				if (_manualQuack != null)
				{
					return _manualQuack.leftTrigger;
				}
				return 0f;
			}
		}

		public void Draw()
		{
			if (_locomotion.pathFinder.path != null)
			{
				Vec2 lastNode = Vec2.Zero;
				foreach (PathNodeLink i in _locomotion.pathFinder.path)
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
