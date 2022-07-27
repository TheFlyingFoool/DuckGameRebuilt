using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
	public class Cape : Thing
	{
		public Cape(float xpos, float ypos, PhysicsObject attach, bool trail = false) : base(xpos, ypos, null)
		{
			this.graphic = new Sprite("cape", 0f, 0f);
			this.visible = attach.visible;
			this.killTimer = this.killTime;
			this._attach = attach;
			base.depth = -0.5f;
			this._trail = trail;
			this._editorCanModify = false;
			if (this._trail)
			{
				this.metadata.CapeTaperStart.value = 0.8f;
				this.metadata.CapeTaperEnd.value = 0f;
			}
		}

		public override void Update()
		{
			base.Update();
			Vec2 attachOffset = Vec2.Zero;
			if (this._attach != null)
			{
				attachOffset = this._attach.OffsetLocal(this.metadata.CapeOffset.value);
				if (this._attach.removeFromLevel)
				{
					Level.Remove(this);
					return;
				}
			}
			this._trail = this.metadata.CapeIsTrail.value;
			if (this._initLastPos)
			{
				this._lastPos = this._attach.position + attachOffset;
				this._initLastPos = false;
			}
			Thing attach = this._attach;
			float yOffset = 1f;
			if (attach is TeamHat && attach.owner != null)
			{
				attach = attach.owner;
			}
			if (attach is Duck)
			{
				if ((attach as Duck).ragdoll != null && (attach as Duck).ragdoll.part1 != null)
				{
					attach = (attach as Duck).ragdoll.part1;
				}
				else
				{
					if ((attach as Duck).crouch)
					{
						yOffset += 5f;
					}
					if ((attach as Duck).sliding)
					{
						yOffset += 2f;
					}
				}
			}
			float velLength = attach.velocity.length;
			if (velLength > 3f)
			{
				velLength = 3f;
			}
			float inverseVel = 1f - velLength / 3f;
			this._capeWave += velLength * 0.1f;
			this._inverseWave += inverseVel * 0.09f;
			this._inverseWave2 += inverseVel * 0.06f;
			float sin = (float)Math.Sin((double)this._capeWave);
			float sin2 = (float)Math.Sin((double)this._inverseWave);
			float sin3 = (float)Math.Sin((double)this._inverseWave2);
			if (this._trail)
			{
				sin = 0f;
				sin2 = 0f;
				sin3 = 0f;
			}
			this._capeWaveMult = velLength * 0.5f;
			float inverseMult = inverseVel * 0.5f;
			this.offDir = (sbyte)-this._attach.offDir;
			Vec2 p = attach.position + attachOffset;
			Vec2 p2 = attach.position + attachOffset;
			base.depth = (this.metadata.CapeForeground.value ? (attach.depth + 50) : (attach.depth - 50));
			p.y += yOffset;
			p2.y += yOffset;
			if (!this._trail)
			{
				p.y += sin * this._capeWaveMult * this.metadata.CapeWiggleModifier.value.y * (attach.velocity.x * 0.5f);
				p.x += sin * this._capeWaveMult * this.metadata.CapeWiggleModifier.value.x * (attach.velocity.y * 0.2f);
			}
			if (this.capePeices.Count > 0)
			{
				p2 = this.capePeices[this.capePeices.Count - 1].p1;
			}
			if (this._trail)
			{
				this.capePeices.Add(new CapePeice(attach.x + attachOffset.x, attach.y + attachOffset.y, this.metadata.CapeTaperStart.value, p, p2));
			}
			else
			{
				this.capePeices.Add(new CapePeice(attach.x - (float)(this.offDir * -10) + attachOffset.x, attach.y + 6f + attachOffset.y, this.metadata.CapeTaperStart.value, p, p2));
			}
			int idx = 0;
			foreach (CapePeice cp in this.capePeices)
			{
				cp.wide = Lerp.FloatSmooth(this.metadata.CapeTaperEnd.value, this.metadata.CapeTaperStart.value, (float)idx / (float)(this.capePeices.Count - 1), 1f);
				if (!this._trail)
				{
					CapePeice capePeice = cp;
					capePeice.p1.x = capePeice.p1.x + sin2 * inverseMult * this.metadata.CapeWiggleModifier.value.x * (cp.wide - 0.5f) * 0.9f;
					CapePeice capePeice2 = cp;
					capePeice2.p2.x = capePeice2.p2.x + sin2 * inverseMult * this.metadata.CapeWiggleModifier.value.x * (cp.wide - 0.5f) * 0.9f;
					CapePeice capePeice3 = cp;
					capePeice3.p1.y = capePeice3.p1.y + sin3 * inverseMult * this.metadata.CapeWiggleModifier.value.y * (cp.wide - 0.5f) * 0.8f;
					CapePeice capePeice4 = cp;
					capePeice4.p2.y = capePeice4.p2.y + sin3 * inverseMult * this.metadata.CapeWiggleModifier.value.y * (cp.wide - 0.5f) * 0.8f;
					CapePeice capePeice5 = cp;
					capePeice5.p1.y = capePeice5.p1.y + this.metadata.CapeSwayModifier.value.y;
					CapePeice capePeice6 = cp;
					capePeice6.p2.y = capePeice6.p2.y + this.metadata.CapeSwayModifier.value.y;
					CapePeice capePeice7 = cp;
					capePeice7.p1.x = capePeice7.p1.x + this.metadata.CapeSwayModifier.value.x * (float)this.offDir;
					CapePeice capePeice8 = cp;
					capePeice8.p2.x = capePeice8.p2.x + this.metadata.CapeSwayModifier.value.x * (float)this.offDir;
					CapePeice capePeice9 = cp;
					capePeice9.position.x = capePeice9.position.x + 0.5f * (float)this.offDir;
				}
				idx++;
			}
			if (this._trail)
			{
				this.maxLength = 16;
			}
			while (this.capePeices.Count > this.maxLength + 1 && this.capePeices.Count > 0)
			{
				this.capePeices.RemoveAt(0);
			}
			this._lastPos = attach.position + attachOffset;
			this.visible = attach.visible;
			if (attach is Holdable && attach.owner != null)
			{
				this.visible = attach.owner.visible;
				if (attach.owner.owner != null && attach.owner.owner is Duck)
				{
					this.visible = attach.owner.owner.visible;
				}
			}
			if (this._capeTexture == null)
			{
				this.SetCapeTexture(Content.Load<Texture2D>("plainCape"));
			}
		}

		public void SetCapeTexture(Texture2D tex)
		{
			this._capeTexture = tex;
			this.maxLength = this._capeTexture.height / 2 - 6;
			if (this.halfFlag)
			{
				this.maxLength = (int)((float)this._capeTexture.width * 0.28f) - 6;
			}
		}

		public override void Draw()
		{
			if (this._attach != null)
			{
				base.depth = (this.metadata.CapeForeground.value ? (this._attach.depth + 50) : (this._attach.depth - 50));
				bool hide = !this._attach.visible;
				if (this._attach.owner != null)
				{
					hide &= !this._attach.owner.visible;
					if (this._attach.owner.owner != null)
					{
						hide &= !this._attach.owner.owner.visible;
					}
				}
				if (hide)
				{
					return;
				}
			}
			float capeWide = 13f;
			Vec2 lastPart = Vec2.Zero;
			Vec2 lastEdgeOffset = Vec2.Zero;
			bool hasLastPart = false;
			bool bust = false;
			if (this._capeTexture != null)
			{
				float deep = Graphics.AdjustDepth(base.depth);
				float uvPart = 1f / (float)(this.capePeices.Count - 1);
				float uvInc = 0f;
				for (int i = this.capePeices.Count - 1; i >= 0; i--)
				{
					Vec2 texTL = new Vec2(0f, Math.Min(uvInc + uvPart, 1f));
					Vec2 texTR = new Vec2(1f, Math.Min(uvInc + uvPart, 1f));
					Vec2 texBL = new Vec2(0f, Math.Min(uvInc, 1f));
					Vec2 texBR = new Vec2(1f, Math.Min(uvInc, 1f));
					if (this.halfFlag)
					{
						texTL = new Vec2(Math.Min(uvInc + uvPart, 1f), 0f);
						texTR = new Vec2(Math.Min(uvInc + uvPart, 1f), 1f);
						texBL = new Vec2(Math.Min(uvInc, 1f), 0f);
						texBR = new Vec2(Math.Min(uvInc, 1f), 1f);
					}
					if (this.offDir > 0)
					{
						Vec2 vec = texTL;
						Vec2 bbl = texBL;
						texTL = texTR;
						texTR = vec;
						texBL = texBR;
						texBR = bbl;
					}
					CapePeice cp = this.capePeices[i];
					Vec2 edgeOffset = lastEdgeOffset;
					if (i > 0)
					{
						Vec2 v = cp.p1 - this.capePeices[i - 1].p1;
						v.Normalize();
						edgeOffset = v.Rotate(Maths.DegToRad(90f), Vec2.Zero);
					}
					Vec2 pos = cp.p1;
					if (hasLastPart)
					{
						Vec2 v2 = pos - lastPart;
						float length = v2.length;
						v2.Normalize();
						if (length > 2f)
						{
							pos = lastPart + v2 * 2f;
						}
						float drawAlpha = Lerp.Float(this.metadata.CapeAlphaStart.value, this.metadata.CapeAlphaEnd.value, (float)i / (float)(this.capePeices.Count - 1));
						Graphics.screen.DrawQuad(pos - edgeOffset * (capeWide * cp.wide / 2f), pos + edgeOffset * (capeWide * cp.wide / 2f), lastPart - lastEdgeOffset * (capeWide * cp.wide / 2f), lastPart + lastEdgeOffset * (capeWide * cp.wide / 2f), texTL, texTR, texBL, texBR, deep, this._capeTexture, Color.White * drawAlpha);
						if (bust)
						{
							break;
						}
					}
					if (hasLastPart)
					{
						uvInc += uvPart;
					}
					hasLastPart = true;
					lastPart = pos;
					lastEdgeOffset = edgeOffset;
				}
			}
		}

		private float killTime = 0.0001f;

		private float killTimer;

		private float counter;

		private PhysicsObject _attach;

		private List<CapePeice> capePeices = new List<CapePeice>();

		private int maxLength = 10;

		private int minLength = 8;

		private GeometryItemTexture _geo;

		public bool _trail;

		private float yDistance;

		private float _capeWave;

		private float _inverseWave;

		private float _inverseWave2;

		private float _capeWaveMult;

		private Vec2 _lastPos;

		private bool _initLastPos = true;

		public Team.CustomHatMetadata metadata = new Team.CustomHatMetadata(null);

		public Tex2D _capeTexture;

		public bool halfFlag;
	}
}
