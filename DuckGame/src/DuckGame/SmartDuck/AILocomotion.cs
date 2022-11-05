using System;

namespace DuckGame
{
	public class AILocomotion
	{
		public AIPathFinder pathFinder
		{
			get
			{
				return this._path;
			}
		}

		public Vec2 target
		{
			get
			{
				if (this._path.target == null)
				{
					return Vec2.Zero;
				}
				return this._path.target.link.position;
			}
			set
			{
				this._path.SetTarget(value);
			}
		}

		public void RunLeft()
		{
			this._ai.Release("RIGHT");
			this._ai.Press("LEFT");
		}

		public void RunRight()
		{
			this._ai.Press("RIGHT");
			this._ai.Release("LEFT");
		}

		public void Jump(int frames)
		{
			this._jumpFrames = (float)frames;
			this._ai.Press("JUMP");
		}

		public void Quack(int frames)
		{
			this._quackFrames = (float)frames;
			this._ai.Press("QUACK");
		}

		public void Slide(int frames)
		{
			this._slideFrames = (float)frames;
			this._ai.Press("DOWN");
		}

		public void TrimLastTarget()
		{
			if (this._path.path != null && this._path.path.Count > 0)
			{
				this._path.path.RemoveAt(this._path.path.Count - 1);
			}
		}

		public void Update(DuckAI ai, Duck duck)
		{
			if (Mouse.right == InputState.Pressed)
			{
				this._path.followObject = duck;
				this._path.SetTarget(Mouse.positionScreen);
				this._path.Refresh();
			}
			this._ai = ai;
			this._path.followObject = duck;
			if (this._jumpFrames == 1f)
			{
				this._jumpFrames = 0f;
				this._ai.Release("JUMP");
			}
			else if (this._jumpFrames > 0f)
			{
				this._jumpFrames -= 1f;
			}
			if (this._slideFrames == 1f)
			{
				this._slideFrames = 0f;
				this._ai.Release("DOWN");
			}
			else if (this._slideFrames > 0f)
			{
				this._slideFrames -= 1f;
			}
			if (this._quackFrames == 1f)
			{
				this._quackFrames = 0f;
				this._ai.Release("QUACK");
			}
			else if (this._quackFrames > 0f)
			{
				this._quackFrames -= 1f;
			}
			ai.Release("LEFT");
			ai.Release("RIGHT");
			if (this._path.path == null || this._path.path.Count == 0)
			{
				return;
			}
			Vec2 nextPoint = this.target;
			Vec2 dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			if (!PathNode.LineIsClear(duck.position, nextPoint, null))
			{
				this._path.Refresh();
				if (this._path.path == null)
				{
					return;
				}
				nextPoint = this.target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			}
			if (this._path.path == null)
			{
				return;
			}
			if (dist.y < duck.y && Math.Abs(dist.y) > 64f && this._path.path.Count > 1)
			{
				this._path.Refresh();
				if (this._path.path == null)
				{
					return;
				}
				nextPoint = this.target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			}
			if (!PathNode.LineIsClear(duck.position, nextPoint, null))
			{
				this._path.Refresh();
				if (this._path.path == null)
				{
					return;
				}
				nextPoint = this.target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
			}
			if (this._path.path == null)
			{
				return;
			}
			bool quickTapJump = false;
			if (this._tryJump > 0)
			{
				this._tryJump--;
			}
			if (this._tryJump == 0 && duck.grounded)
			{
				this._path.Refresh();
				if (this._path.path == null)
				{
					return;
				}
				nextPoint = this.target;
				dist = new Vec2(nextPoint.x - duck.x, nextPoint.y - duck.y);
				this._tryJump = -1;
			}
			if (this._path.path == null)
			{
				return;
			}
			float speedMul = 1f;
			if (this._path.target.position.y == this.target.y)
			{
				speedMul = 0f;
			}
			if (dist.x < (duck.hSpeed * 3f - 2f) * speedMul)
			{
				if (duck.grounded && Level.CheckLine<Window>(duck.position, duck.position + new Vec2(-32f, 0f)) != null)
				{
					this.Slide(30);
				}
				this.RunLeft();
			}
			else if (dist.x > (duck.hSpeed * 3f + 2f) * speedMul)
			{
				if (duck.grounded && Level.CheckLine<Window>(duck.position, duck.position + new Vec2(32f, 0f)) != null)
				{
					this.Slide(30);
				}
				this.RunRight();
			}
			if (this._path.peek.gap && duck.grounded)
			{
				this.Jump((int)(Maths.Clamp(Math.Abs(dist.x), 0f, 48f) / 48f * 16f));
				this._tryJump = 5;
			}
			if (dist.y <= -4f && duck.grounded)
			{
				this.Jump((int)(Maths.Clamp(Math.Abs(dist.y), 0f, 48f) / 48f * 16f));
				this._tryJump = 5;
			}
			if (quickTapJump)
			{
				ai.Release("JUMP");
			}
			float minYdist = 8f;
			if (Math.Abs(this._path.peek.owner.y - nextPoint.y) < 8f)
			{
				minYdist = 200f;
			}
			if (Math.Abs(dist.x) < 4f && Math.Abs(dist.y) < minYdist && PathNode.LineIsClear(duck.position - new Vec2(0f, 8f), nextPoint, null))
			{
				if (this._path.peek.link.position.y < duck.y - 8f && !duck.grounded)
				{
					return;
				}
				if (!duck.grounded)
				{
					return;
				}
				this._path.AtTarget();
				this._ai.canRefresh = true;
				this._tryJump = -1;
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
