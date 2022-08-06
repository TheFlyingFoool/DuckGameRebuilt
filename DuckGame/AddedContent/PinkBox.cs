using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
	[ClientOnly]
	[EditorGroup("Rebuilt")]
	public class PinkBox : Block
	{
		public bool canBounce
		{
			get
			{
				return _canBounce;
			}
		}
		public const string DanBoxSpr = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAQCAMAAABA3o1rAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAASUExURQAAALLc7zGi8gBXhP///wAAACzSHb4AAAAGdFJOU///////ALO/pL8AAAAJcEhZcwAADsEAAA7BAbiRa+0AAACJSURBVChTfZELDsQgCES1yP2v3Plgpdl0X9KUGUbUODJHI0GVAirHbNCpUqA95tWIXw0jSqFQ4K21IuDx2xO6phFrRaxJ19pta45cYiKhAJeKvYUDOPPnGZhQ34GdeLbAn3em7YATpWUwYXNr90/AlzqBpmG4Bp+HPDwTCugc0eBjVSnQ/v/cmTe6ywO8M6DLhAAAAABJRU5ErkJggg==";

		public const string PinkBoxSpr = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAQCAMAAABA3o1rAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAASUExURQAAAOOMutVMgYQeRvzJ5QAAAMoIbWIAAAAGdFJOU///////ALO/pL8AAAAJcEhZcwAADsIAAA7CARUoSoAAAACJSURBVChTfZELDsQgCES1yP2v3Plgpdl0X9KUGUbUODJHI0GVAirHbNCpUqA95tWIXw0jSqFQ4K21IuDx2xO6phFrRaxJ19pta45cYiKhAJeKvYUDOPPnGZhQ34GdeLbAn3em7YATpWUwYXNr90/AlzqBpmG4Bp+HPDwTCugc0eBjVSnQ/v/cmTe6ywO8M6DLhAAAAABJRU5ErkJggg==";
		public PinkBox(float xpos, float ypos) : base(xpos, ypos)
		{
			if (Steam.user != null && Steam.user.id == 76561198124539558)
            {
				_sprite = new SpriteMap(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(DanBoxSpr))), "danbox"), 16, 16);
			}
			else
            {
				_sprite = new SpriteMap(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(PinkBoxSpr))), "pinkbox"), 16, 16);
			}
			graphic = _sprite;
			layer = Layer.Foreground;
			center = new Vec2(8f, 8f);
			collisionSize = new Vec2(16f, 16f);
			collisionOffset = new Vec2(-8f, -8f);
			depth = 0.5f;
			_canFlip = false;
			_editorName = "Pink Box";
			editorTooltip = "Spread your love for duck game with glittery explosions and death!";
		}
		public void Pop(Duck duck)
		{
			Bounce();
			if (!_hit)
			{
				SuperFondle(this, DuckNetwork.localConnection);
				_hit = true;
				D = duck;
				lD = duck;
			}
		}
		public Duck D;
		public Duck lD;
		public void Bounce()
		{
			if (_canBounce)
			{
				bounceAmount = 8f;
				_canBounce = false;
				if (Network.isActive)
				{
					netDisarmIndex += 1;
					return;
				}
				_aboveList = Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, -4f), bottomRight + new Vec2(-1f, -12f)).ToList<PhysicsObject>();
				foreach (PhysicsObject p in _aboveList)
				{
					if (p.grounded || p.vSpeed > 0f || p.vSpeed == 0f)
					{
						Fondle(p);
						p.y -= 2f;
						p.vSpeed = -3f;
						Duck d = p as Duck;
						if (d != null)
						{
							if (!d.isServerForObject)
							{
								Send.Message(new NMDisarmVertical(d, -3f), d.connection);
							}
							else
							{
								d.Disarm(this);
							}
						}
					}
				}
			}
		}

		public SinWave SIN = new SinWave(0.1f);
		public override void Draw()
		{
			if (D != null)
			{
				if (D.team.hasHat)
				{
					SpriteMap spr = D.team.hat;
					float prev = spr.alpha;
					spr.alpha = 0.5f;
					Graphics.Draw(spr, x, top - 16, 1);
					spr.alpha = prev;
				}
				else
				{
					SpriteMap spr = D.persona.defaultHead;
					float prev = spr.alpha;
					spr.alpha = 0.5f;
					spr.frame = D.quack > 0 ? 1 : 0;
					Graphics.Draw(spr, x - 2, top - 16 + SIN * 3, 1);
					spr.alpha = prev;
				}
			}
			base.Draw();
		}
		public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
		{
			if (from == ImpactedFrom.Bottom && with.isServerForObject && D == null)
			{
				Holdable h = with as Holdable;
				if (h != null && (h.lastThrownBy != null || (h is RagdollPart && !Network.isActive)))
				{
					Duck dd = h.lastThrownBy as Duck;
					Pop(dd);
					return;
				}
				else
				{
					Duck duck = with as Duck;
					if (duck != null)
					{
						RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None, RumbleType.Gameplay));
						Pop(duck);
					}
				}
			}
		}
		public static void ExplodeEffect(Vec2 pPosition)
        {
			for (int i = 0; i < 16; i++)
			{
				SmallSmoke sm = SmallSmoke.New(pPosition.x, pPosition.y);
				sm.hSpeed = Rando.Float(-3, 3);
				sm.vSpeed = Rando.Float(-3, 3);
				Level.Add(sm);
			}
			for (int i = 0; i < 24; i++)
			{
				ConfettiParticle confettiParticle = new ConfettiParticle();
				confettiParticle.Init(pPosition.x + Rando.Float(-4f, 0f), pPosition.y + Rando.Float(-4f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)), 0.01f);
				confettiParticle.velocity *= Rando.Float(1, 2);
				confettiParticle._color = Color.Pink;
				Level.Add(confettiParticle);
			}
		}
		public void Explode()
        {
			List<PhysicsObject> physicsObjects = Level.CheckCircleAll<PhysicsObject>(position, 128).ToList();
			for (int i = 0; i < physicsObjects.Count; i++)
			{
				PhysicsObject po = physicsObjects[i];
				Vec2 travel = Maths.AngleToVec(Maths.DegToRad(-Maths.PointDirection(position, po.position)));
				travel.x *= 10;
				travel.y *= -10;
				if (po is not IAmADuck)
				{
					Fondle(po, DuckNetwork.localConnection);
					po.velocity = travel / ((Extensions.Distance(position, po.position) / 50) + 0.01f);
				}
			}
			SFX.Play("explode");
			ExplodeEffect(position);
			Send.Message(new NMPinkExplode(position));
			Level.Remove(this);
		}
		public bool collision;
		public override void Update()
		{
			if (D != null && D.isServerForObject)
			{
				Fondle(this);
				//Failsafe for if multiple people happen to hit the box it explodes
				if (D != lD && !collision)
				{
					collision = true;
					Send.Message(new NMPinkCollision(this));
				}
				if (D.dead)
				{
					UnstoppableFondle(D, DuckNetwork.localConnection);
					D.position = position;
					D.Ressurect();
					D.position = position;
					Explode();
				}
				lD = D;
			}
			_aboveList.Clear();
			if (startY < -9999f)
			{
				startY = y;
			}
			_sprite.frame = (_hit ? 1 : 0);
			if (netDisarmIndex != localNetDisarm)
			{
				localNetDisarm = netDisarmIndex;
				_aboveList = Level.CheckRectAll<PhysicsObject>(topLeft + new Vec2(1f, -4f), bottomRight + new Vec2(-1f, -12f)).ToList<PhysicsObject>();
				foreach (PhysicsObject p in _aboveList)
				{
					if (isServerForObject && p.owner == null)
					{
						Fondle(p);
					}
					if (p.isServerForObject && (p.grounded || p.vSpeed > 0f || p.vSpeed == 0f))
					{
						p.y -= 2f;
						p.vSpeed = -3f;
						Duck d = p as Duck;
						if (d != null)
						{
							if (!d.isServerForObject)
							{
								Send.Message(new NMDisarmVertical(d, -3f), d.connection);
							}
							else
							{
								d.Disarm(this);
							}
						}
					}
				}
			}
			if (bounceAmount > 0f)
			{
				bounceAmount -= 0.8f;
			}
			else
			{
				bounceAmount = 0f;
			}
			y -= bounceAmount;
			if (!_canBounce)
			{
				if (y < startY)
				{
					y += 0.8f + Math.Abs(y - startY) * 0.4f;
				}
				if (y > startY)
				{
					y -= 0.8f - Math.Abs(y - startY) * 0.4f;
				}
				if (Math.Abs(y - startY) < 0.8f)
				{
					_canBounce = true;
					y = startY;
				}
			}
		}
		public StateBinding _positionBinding = new StateBinding("position", -1, false, false);
		public StateBinding _hitBinding = new StateBinding("_hit");
		public StateBinding _netDisarmIndexBinding = new StateBinding("netDisarmIndex", -1, false, false);
		public byte netDisarmIndex;
		public byte localNetDisarm;
		public float bounceAmount;
		public bool _hit;
		public float startY = -99999f;
		protected List<PhysicsObject> _aboveList = new List<PhysicsObject>();
		protected SpriteMap _sprite;
		public bool _canBounce = true;
	}
}
