using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
	[EditorGroup("Wump|Lasers")]
	public class WumpHugeLaser : Gun
	{
		public byte netAnimationIndex
		{
			get
			{
				if (_chargeAnim == null)
				{
					return 0;
				}
				return (byte)_chargeAnim.animationIndex;
			}
			set
			{
				if (_chargeAnim != null && _chargeAnim.animationIndex != value)
				{
					_chargeAnim.animationIndex = value;
				}
			}
		}

		public byte spriteFrame
		{
			get
			{
				if (_chargeAnim == null)
				{
					return 0;
				}
				return (byte)_chargeAnim._frame;
			}
			set
			{
				if (_chargeAnim != null)
				{
					_chargeAnim._frame = value;
				}
			}
		}
		public const string wumphugelaser = "iVBORw0KGgoAAAANSUhEUgAAACMAAAAYCAMAAABHurQFAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAkUExURQAAALLc7zGi8gBXhJ2dnWB3fBsmMv///y9ITkk8K8AgLQAAAHFpNE8AAAAMdFJOU///////////////ABLfzs4AAAAJcEhZcwAADsMAAA7DAcdvqGQAAADoSURBVDhPlZFZEsMgDEO9AKXl/vetZCCBCf2oyBCQH45NpN1a16v+ZER/QJctYAzTgZsWkri7HHPdjIkbZ2qYQwuDTGRSymWHuIuDCCMHmJzKkxFVBeZguEqvYx4z1ssHXORZhDilam5oKpiox4ZYJZhSU4CUuz7ydCbDjyFA+C6FG+YReb9JlYw6E4VDfBGBwRw6GEIlb4MTTka/IJAnoIcYRhOsGUwldBLa5P8LJknNfST+2UsdmUwXW4TmzRjq6feMSOXSsAHzgXOLCBhaiOMJptubhmUT6dtdi8nuxnLXypy+01prX3yXFHoiciNUAAAAAElFTkSuQmCC";
		public const string wumphugelasercharge = "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";
		public WumpHugeLaser(float xval, float yval) : base(xval, yval)
		{
			ammo = 99;
			_type = "gun";
			graphic = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(wumphugelaser))), "wumphugelaser"));
			graphic.Namebase = "wumphugelaser";
			Content.textures[graphic.Namebase] = graphic.texture;
			center = new Vec2(17.5f, 12f);
			collisionOffset = new Vec2(-16f, -4f);
			collisionSize = new Vec2(32f, 15f);
			_barrelOffsetTL = new Vec2(33, 13);
			_fireSound = "";
			_fullAuto = false;
			_fireWait = 1f;
			_kickForce = 0.4f;
			_holdOffset = new Vec2(1, -4);
			_fireRumble = RumbleIntensity.Light;
			_editorName = "Death Laser";
			_tip = new Sprite("bigLaserTip", 0f, 0f);
			_tip.CenterOrigin();
			_chargeAnim = new SpriteMap(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(wumphugelasercharge))), "wumphugelasercharge"), 35, 24);
			_chargeAnim.Namebase = "wumphugelasercharge";
			Content.textures[_chargeAnim.Namebase] = _chargeAnim.texture;
			_chargeAnim.AddAnimation("idle", 1f, true, new int[1]);
			_chargeAnim.AddAnimation("load", 0.05f, false, new int[]
			{
				0,
				1,
				2,
				3,
				4
			});
			_chargeAnim.AddAnimation("loaded", 1f, true, new int[]
			{
				0
			});
			_chargeAnim.AddAnimation("charge", 0.25f, false, new int[]
			{
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13,
				14,
				15,
				16,
				17,
				18,
				19,
				20,
				21,
				22,
				23,
				24,
				25,
				26,
				27,
				28,
				29,
				30,
				31,
				32,
				33
			});
			_chargeAnim.AddAnimation("uncharge", 1.2f, false, new int[]
			{
				37,
				36,
				35,
				34,
				33,
				32,
				31,
				30,
				29,
				28,
				28,
				27,
				26,
				25,
				24,
				23,
				22,
				21,
				20,
				19,
				18,
				17,
				16,
				15,
				14,
				13,
				12,
				11,
				10,
				9,
				8,
				7,
				6
			});
			_chargeAnim.AddAnimation("drain", 2f, false, new int[]
			{
				29,
				30,
				31,
				32,
				33,
				34,
				35,
				36,
				37,
				38,
				39,
				40
			});
			_chargeAnim.SetAnimation("loaded");
			_chargeAnim.center = new Vec2(17.5f, 12f);
			_editorName = "Death Ray";
			editorTooltip = "Hold the trigger to charge a beam of pure death and destruction. You know, for kids!";
			_bio = "Invented by Dr.Death for scanning items at your local super market. Also has some military application.";
		}
		public bool uncharge;
		public override void Initialize()
		{
			_chargeSound = SFX.Get("chaingunSpinUp", 0f, -2);
			_chargeSoundShort = SFX.Get("chaingunSpinUp", 0f, -2);
			_unchargeSound = SFX.Get("chaingunSpinDown", 0f, -2);
			_unchargeSoundShort = SFX.Get("chaingunSpinDown", 0, -2);
		}

		public void PostFireLogic()
		{
			if (isServerForObject)
			{
				_unchargeSound.Stop();
				_unchargeSound.Volume = 0f;
				_unchargeSoundShort.Stop();
				_unchargeSoundShort.Volume = 0f;
				_chargeSound.Stop();
				_chargeSound.Volume = 0f;
				_chargeSoundShort.Stop();
				_chargeSoundShort.Volume = 0f;
			}
			_chargeAnim.SetAnimation("drain");
			SFX.Play("laserBlast", 1f, 0f, 0f, false);
		}

		public override void Update()
		{
			if (weight > 6) _canRaise = false;
			else _canRaise = true;
			if (Network.isActive)
			{
				if (isServerForObject)
				{
					_chargeVolume = (_chargeSound.State == SoundState.Playing) ? _chargeSound.Volume : 0f;
					_chargeVolumeShort = (_chargeSoundShort.State == SoundState.Playing) ? _chargeSoundShort.Volume : 0f;
					_unchargeVolume = (_unchargeSound.State == SoundState.Playing) ? _unchargeSound.Volume : 0f;
					_unchargeVolumeShort = (_unchargeSoundShort.State == SoundState.Playing) ? _unchargeSoundShort.Volume : 0f;
				}
				else
				{
					_chargeSound.Volume = _chargeVolume;
					_chargeSoundShort.Volume = _chargeVolumeShort;
					_unchargeSound.Volume = _unchargeVolume;
					_unchargeSoundShort.Volume = _unchargeVolumeShort;
					if (_chargeVolume > 0f && _chargeSound.State != SoundState.Playing)
					{
						_chargeSound.Play();
					}
					else if (_chargeVolume <= 0f)
					{
						_chargeSound.Stop();
					}
					if (_chargeVolumeShort > 0f && _chargeSoundShort.State != SoundState.Playing)
					{
						_chargeSoundShort.Play();
					}
					else if (_chargeVolumeShort <= 0f)
					{
						_chargeSoundShort.Stop();
					}
					if (_unchargeVolume > 0f && _unchargeSound.State != SoundState.Playing)
					{
						_unchargeSound.Play();
					}
					else if (_unchargeVolume <= 0f)
					{
						_unchargeSound.Stop();
					}
					if (_unchargeVolumeShort > 0f && _unchargeSoundShort.State != SoundState.Playing)
					{
						_unchargeSoundShort.Play();
					}
					else if (_unchargeVolumeShort <= 0f)
					{
						_unchargeSoundShort.Stop();
					}
				}
			}
			base.Update();
			if (_charge > 0f)
			{
				_charge -= 0.1f;
			}
			else
			{
				_charge = 0f;
			}
			if (_chargeAnim.currentAnimation == "uncharge" && _chargeAnim.finished)
			{
				_chargeAnim.SetAnimation("loaded");
			}
			if (_chargeAnim.currentAnimation == "charge" && _chargeAnim.finished && isServerForObject)
			{
				PostFireLogic();
				Duck duck = owner as Duck;
				if (duck != null)
				{
					RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Medium, RumbleDuration.Pulse, RumbleFalloff.Short, RumbleType.Gameplay));
					duck.sliding = true;
					duck.crouch = true;
					Vec2 vec = barrelVector * 9f;
					if (duck.ragdoll != null && duck.ragdoll.part2 != null && duck.ragdoll.part1 != null && duck.ragdoll.part3 != null)
					{
						duck.ragdoll.part2.hSpeed -= vec.x;
						duck.ragdoll.part2.vSpeed -= vec.y;
						duck.ragdoll.part1.hSpeed -= vec.x;
						duck.ragdoll.part1.vSpeed -= vec.y;
						duck.ragdoll.part3.hSpeed -= vec.x;
						duck.ragdoll.part3.vSpeed -= vec.y;
					}
					else
					{
						duck.hSpeed -= vec.x;
						duck.vSpeed -= vec.y + 3f;
						duck.CancelFlapping();
					}
				}
				else
				{
					Vec2 barrelVector = base.barrelVector;
					hSpeed -= barrelVector.x * 9f;
					vSpeed -= barrelVector.y * 9f + 3f;
				}
				Vec2 vec2 = Offset(barrelOffset);
				Vec2 vec3 = Offset(barrelOffset + new Vec2(1200f, 0f)) - vec2;
				if (isServerForObject)
				{
					StatBinding laserBulletsFired = Global.data.laserBulletsFired;
					int valueInt = laserBulletsFired.valueInt;
					laserBulletsFired.valueInt = valueInt + 1;
				}
				Level.Add(new WumpBeam(vec2, vec3, this)
				{
					isLocal = isServerForObject
				});
				doBlast = true;
			}
			if (doBlast && isServerForObject)
			{
				_framesSinceBlast++;
				if (_framesSinceBlast > 10)
				{
					_framesSinceBlast = 0;
					doBlast = false;
				}
			}
			if (_chargeAnim.currentAnimation == "drain" && _chargeAnim.finished)
			{
				_chargeAnim.SetAnimation("loaded");
			}
			_lastDoBlast = doBlast;
		}

		public override void Draw()
		{
			base.Draw();
			Material material = Graphics.material;
			Graphics.material = base.material;
			_tip.depth = depth + 1;
			_tip.alpha = _charge;
			if (_chargeAnim.currentAnimation == "charge")
			{
				_tip.alpha = _chargeAnim.frame / 27f;
			}
			else if (_chargeAnim.currentAnimation == "uncharge")
			{
				_tip.alpha = (24 - _chargeAnim.frame) / 27f;
			}
			else
			{
				_tip.alpha = 0f;
			}
			Graphics.Draw(_tip, barrelPosition.x, barrelPosition.y);
			_chargeAnim.flipH = graphic.flipH;
			_chargeAnim.depth = depth + 1;
			_chargeAnim.angle = angle;
			_chargeAnim.alpha = alpha;
			Graphics.Draw(_chargeAnim, x, y);
			Graphics.material = material;
			float num = Maths.NormalizeSection(_tip.alpha, 0f, 0.7f);
			float num2 = Maths.NormalizeSection(_tip.alpha, 0.6f, 1f);
			float num3 = Maths.NormalizeSection(_tip.alpha, 0.75f, 1f);
			float num4 = Maths.NormalizeSection(_tip.alpha, 0.9f, 1f);
			float num5 = Maths.NormalizeSection(_tip.alpha, 0.8f, 1f) * 0.5f;
			if (num > 0f)
			{
				Vec2 p = Offset(barrelOffset);
				Vec2 p2 = Offset(barrelOffset + new Vec2(num * 1200f, 0f));
				Graphics.DrawLine(p, p2, new Color(_tip.alpha * 0.7f + 0.3f, _tip.alpha, _tip.alpha) * (0.3f + num5), 1f + num2 * 12f, default(Depth));
				Graphics.DrawLine(p, p2, Color.LightBlue * (0.2f + num5), 1f + num3 * 28f, default(Depth));
				Graphics.DrawLine(p, p2, Color.LightBlue * (0.1f + num5), 0.2f + num4 * 40f, default(Depth));
			}
		}

		public override void OnPressAction()
		{
			if (_chargeAnim == null || _chargeSound == null || uncharge)
			{
				return;
			}
			if (_chargeAnim.currentAnimation == "loaded")
			{
				_chargeAnim.SetAnimation("charge");
				if (isServerForObject)
				{
					_chargeSound.Volume = 1f;
					_chargeSound.Play();
					_unchargeSound.Stop();
					_unchargeSound.Volume = 0f;
					_unchargeSoundShort.Stop();
					_unchargeSoundShort.Volume = 0f;
					return;
				}
			}
			else if (_chargeAnim.currentAnimation == "uncharge")
			{
				if (isServerForObject)
				{
					if (_chargeAnim.frame > 18)
					{
						_chargeSound.Volume = 1f;
						_chargeSound.Play();
					}
					else
					{
						_chargeSoundShort.Volume = 1f;
						_chargeSoundShort.Play();
					}
				}
				int frame = _chargeAnim.frame;
				_chargeAnim.SetAnimation("charge");
				_chargeAnim.frame = 22 - frame;
				if (isServerForObject)
				{
					_unchargeSound.Stop();
					_unchargeSound.Volume = 0f;
					_unchargeSoundShort.Stop();
					_unchargeSoundShort.Volume = 0f;
				}
			}
		}

		public override void OnHoldAction()
		{
		}

		public override void OnReleaseAction()
		{
			if (_chargeAnim.currentAnimation == "charge")
			{
				if (isServerForObject)
				{
					if (_chargeAnim.frame > 20)
					{
						_unchargeSound.Stop();
						_unchargeSound.Volume = 1f;
						_unchargeSound.Play();
					}
					else
					{
						_unchargeSoundShort.Stop();
						_unchargeSoundShort.Volume = 1f;
						_unchargeSoundShort.Play();
					}
				}
				int frame = _chargeAnim.frame;
				_chargeAnim.SetAnimation("uncharge");
				_chargeAnim.frame = 22 - frame;
				if (isServerForObject)
				{
					_chargeSound.Stop();
					_chargeSound.Volume = 0f;
					_chargeSoundShort.Stop();
					_chargeSoundShort.Volume = 0f;
				}
			}
		}

	//	public StateBinding _laserStateBinding = new HugeLaserFlagBinding(GhostPriority.Normal);

		public StateBinding _animationIndexBinding = new StateBinding("netAnimationIndex", 4, false, false);

		public StateBinding _frameBinding = new StateBinding("spriteFrame", -1, false, false);

		public StateBinding _chargeVolumeBinding = new CompressedFloatBinding(GhostPriority.High, "_chargeVolume", 1f, 8, false, false);

		public StateBinding _chargeVolumeShortBinding = new CompressedFloatBinding(GhostPriority.High, "_chargeVolumeShort", 1f, 8, false, false);

		public StateBinding _unchargeVolumeBinding = new CompressedFloatBinding(GhostPriority.High, "_unchargeVolume", 1f, 8, false, false);

		public StateBinding _unchargeVolumeShortBinding = new CompressedFloatBinding(GhostPriority.High, "_unchargeVolumeShort", 1f, 8, false, false);

		private float _chargeVolume;

		private float _chargeVolumeShort;

		private float _unchargeVolume;

		private float _unchargeVolumeShort;

		public bool doBlast;

		private bool _lastDoBlast;

		private Sprite _tip;

		private float _charge;

		public bool _charging;

		public bool _fired;

		private SpriteMap _chargeAnim;

		private Sound _chargeSound;

		private Sound _chargeSoundShort;

		private Sound _unchargeSound;

		private Sound _unchargeSoundShort;

		private int _framesSinceBlast;
	}
}
