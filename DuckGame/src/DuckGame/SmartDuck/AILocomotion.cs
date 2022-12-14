using System;

namespace DuckGame
{
	public class AILocomotion
	{
		public AIPathFinder pathFinder
		{
			get
			{
				return _path;
			}
		}

		public Vec2 target
		{
			get
			{
				if (_path.target == null)
				{
					return Vec2.Zero;
				}
				return _path.target.link.position;
			}
			set
			{
				_path.SetTarget(value);
			}
		}

		public void RunLeft()
		{
			_ai.Release(Triggers.Right);
			_ai.Press(Triggers.Left);
		}

		public void RunRight()
		{
			_ai.Press(Triggers.Right);
			_ai.Release(Triggers.Left);
		}

		public void Jump(int frames)
		{
			_jumpFrames = frames;
			_ai.Press(Triggers.Jump);
		}

		public void Quack(int frames)
		{
			_quackFrames = frames;
			_ai.Press(Triggers.Quack);
		}

		public void Slide(int frames)
		{
			_slideFrames = frames;
			_ai.Press(Triggers.Down);

        }

		public void TrimLastTarget()
		{
			if (_path.path != null && _path.path.Count > 0)
			{
				_path.path.RemoveAt(_path.path.Count - 1);
			}
		}

		public void Update(DuckAI ai, Duck duck)
		{
			if (Mouse.right == InputState.Pressed)
			{
				_path.followObject = duck;
				_path.SetTarget(Mouse.positionScreen);
				_path.Refresh();
			}
			_ai = ai;
			_path.followObject = duck;
			if (_jumpFrames == 1f)
			{
				_jumpFrames = 0f;
				_ai.Release(Triggers.Jump);
			}
			else if (_jumpFrames > 0f)
			{
				_jumpFrames -= 1f;
			}
			if (_slideFrames == 1f)
			{
				_slideFrames = 0f;
				_ai.Release(Triggers.Down);
			}
			else if (_slideFrames > 0f)
			{
				_slideFrames -= 1f;
			}
			if (_quackFrames == 1f)
			{
				_quackFrames = 0f;
				_ai.Release(Triggers.Quack);
			}
			else if (_quackFrames > 0f)
			{
				_quackFrames -= 1f;
			}
			ai.Release(Triggers.Left);
			ai.Release(Triggers.Right);
			if (_path.path == null || _path.path.Count == 0)
			{
				return;
			}
			Vec2 nextPoint = target;
			Vec2 dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			if (!PathNode.LineIsClear(duck.position, nextPoint, null))
			{
				_path.Refresh();
				if (_path.path == null)
				{
					return;
				}
				nextPoint = target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			}
			if (_path.path == null)
			{
				return;
			}
			if (dist.y < duck.y && Math.Abs(dist.y) > 64f && _path.path.Count > 1)
			{
				_path.Refresh();
				if (_path.path == null)
				{
					return;
				}
				nextPoint = target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			}
			if (!PathNode.LineIsClear(duck.position, nextPoint, null))
			{
				_path.Refresh();
				if (_path.path == null)
				{
					return;
				}
				nextPoint = target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			}
			if (_path.path == null)
			{
				return;
			}
			bool quickTapJump = false;
			if (_tryJump > 0)
			{
				_tryJump--;
			}
			if (_tryJump == 0 && duck.grounded)
			{
				_path.Refresh();
				if (_path.path == null)
				{
					return;
				}
				nextPoint = target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
				_tryJump = -1;
			}
			if (_path.path == null)
			{
				return;
			}
			float speedMul = 1f;
			if (_path.target.position.y == target.y)
			{
				speedMul = 0f;
			}
			if (dist.x < (duck.hSpeed * 3f - 2f) * speedMul)
			{
				if (duck.grounded && Level.CheckLine<Window>(duck.position, duck.position + new Vec2(-32f, 0f)) != null)
				{
					Slide(30);
				}
				RunLeft();
			}
			else if (dist.x > (duck.hSpeed * 3f + 2f) * speedMul)
			{
				if (duck.grounded && Level.CheckLine<Window>(duck.position, duck.position + new Vec2(32f, 0f)) != null)
				{
					Slide(30);
				}
				RunRight();
			}
			if (_path.peek.gap && duck.grounded)
			{
				Jump((int)(Maths.Clamp(Math.Abs(dist.x), 0f, 48f) / 48f * 16f));
				_tryJump = 5;
			}
			if (dist.y <= -4f && duck.grounded)
			{
				Jump((int)(Maths.Clamp(Math.Abs(dist.y), 0f, 48f) / 48f * 16f));
				_tryJump = 5;
			}
			if (quickTapJump)
			{
				ai.Release(Triggers.Jump);
			}
			float minYdist = 8f;
			if (Math.Abs(_path.peek.owner.y - nextPoint.y) < 8f)
			{
				minYdist = 200f;
			}
			if (Math.Abs(dist.x) < 4f && Math.Abs(dist.y) < minYdist && PathNode.LineIsClear(duck.position - new Vec2(0f, 8f), nextPoint, null))
			{
				if (_path.peek.link.position.y < duck.y - 8f && !duck.grounded)
				{
					return;
				}
				if (!duck.grounded)
				{
					return;
				}
				_path.AtTarget();
				_ai.canRefresh = true;
				_tryJump = -1;
			}
		}

		public AILocomotion()
		{
		}

		private AIPathFinder _path = new AIPathFinder(null);

		private DuckAI _ai;

		private float _jumpFrames;

		private float _quackFrames;

		private float _slideFrames;

		private int _tryJump = -1;
	}
}
