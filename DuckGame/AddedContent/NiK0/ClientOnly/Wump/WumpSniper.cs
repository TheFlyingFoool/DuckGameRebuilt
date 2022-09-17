using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace DuckGame.MarineSpace
{
    [ClientOnly]
    [EditorGroup("Wump|Rifles")]
	public class WumpSniper : Gun
	{
		public const string wumpsniper = "iVBORw0KGgoAAAANSUhEUgAAAB0AAAAOCAMAAADkD+cIAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAhUExURQAAAABXhLLc752dnTGi8v///2B3fC9IThsmMkk8KwAAAAwpv/QAAAALdFJOU/////////////8ASk8B8gAAAAlwSFlzAAAOwwAADsMBx2+oZAAAAH5JREFUKFN9jtEWwyAIQ4M4df7/By9B2vKw7Z7WIwkRsG9wyCqorjW3/tP9myVRlaZvrlvLAcU9mYsjxRloKzfjd5LZVjHLC9BeOnpA2X34CCb4Um90u/oUmWLNtSRIk9PBIPB+5sQWOdcIBanw8WyJRSHIupp0+Odd3G3J3h8BtwkTsyWw+AAAAABJRU5ErkJggg==";
		public WumpSniper(float xval, float yval) : base(xval, yval)
		{
			ammo = 3;
			_ammoType = new ATSniper();
			_ammoType.range = 500;
			_ammoType.penetration = 10;
			_type = "gun";
			graphic = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(wumpsniper))), "wumpsniper"));
			graphic.Namebase = "wumpsniper";
			Content.textures[graphic.Namebase] = graphic.texture;
			center = new Vec2(16f, 8f);
			collisionOffset = new Vec2(-8f, -4f);
			collisionSize = new Vec2(16f, 8f);
			_barrelOffsetTL = new Vec2(29f, 8f);
			_fireSound = "sniper";
			_kickForce = 2f;
			_fireRumble = RumbleIntensity.Light;
			laserSight = true;
			_laserOffsetTL = new Vec2(32f, 3.5f);
			_manualLoad = true;
			_editorName = "Sniper";
			editorTooltip = "3 Scopes but only 2 modes of fire? Now thats wasteful.";
		}
		public bool awesome;
		public bool lastCrouch;
		public override void Update()
		{
			base.Update();
			if (duck != null)
            {
				if (duck.crouch && !lastCrouch)
                {
					awesome = !awesome;
					SFX.Play("click");
                }
				lastCrouch = duck.crouch;
            }
            else lastCrouch = false;
			if (awesome)
            {
                _ammoType.range = infiniteAmmoVal ? 400 : 200;
				_ammoType.penetration = 10;
                _ammoType.bulletThickness = 3;
				_ammoType.bulletSpeed = 96;
				_kickForce = 5;
			}
			else
            {
				_ammoType.bulletThickness = 1;
				_ammoType.range = infiniteAmmoVal?800:400;
				_ammoType.penetration = 5;
				_ammoType.bulletSpeed = 48;
				_kickForce = 3;
			}
			if (_loadState > -1)
			{
				if (owner == null)
				{
					if (_loadState == 3)
					{
						loaded = true;
					}
					_loadState = -1;
					_angleOffset = 0f;
					handOffset = Vec2.Zero;
				}
				if (_loadState == 0)
				{
					if (Network.isActive)
					{
						if (isServerForObject)
						{
							_netLoad.Play(1f, 0f);
						}
					}
					else
					{
						SFX.Play("loadSniper", 1f, 0f, 0f, false);
					}
					_loadState++;
				}
				else if (_loadState == 1)
				{
					if (_angleOffset < 0.16f)
					{
						_angleOffset = MathHelper.Lerp(_angleOffset, 0.2f, 0.15f);
					}
					else
					{
						_loadState++;
					}
				}
				else if (_loadState == 2)
				{
					handOffset.x = handOffset.x + 0.4f;
					if (handOffset.x > 4f)
					{
						_loadState++;
						Reload(true);
						loaded = false;
					}
				}
				else if (_loadState == 3)
				{
					handOffset.x = handOffset.x - 0.4f;
					if (handOffset.x <= 0f)
					{
						_loadState++;
						handOffset.x = 0f;
					}
				}
				else if (_loadState == 4)
				{
					if (_angleOffset > 0.04f)
					{
						_angleOffset = MathHelper.Lerp(_angleOffset, 0f, 0.15f);
					}
					else
					{
						_loadState = -1;
						loaded = true;
						_angleOffset = 0f;
					}
				}
			}
			if (loaded && owner != null && _loadState == -1)
			{
				laserSight = true;
				return;
			}
			laserSight = false;
		}

		public override void OnPressAction()
		{
			if (loaded)
			{
				base.OnPressAction();
				return;
			}
			if (ammo > 0 && _loadState == -1)
			{
				_loadState = 0;
				_loadAnimation = 0;
			}
		}
		public override void DrawGlow()
		{
			if (laserSight && held && _laserTex != null)
			{
				float num = 1f;
				if (!Options.Data.fireGlow)
				{
					num = 0.4f;
				}
				Vec2 vec = barrelPosition;
				Vec2 vec2 = Extensions.stopPoint(barrelPosition, ammoType, Maths.PointDirection(Vec2.Zero, barrelVector));
				Vec2 normalized = (vec2 - vec).normalized;
                float f = 0.5f;
				if (awesome) f = 0.75f + sinner * 0.5f;

				Color c = Color.LightBlue;

				c *= num;
				if (infiniteAmmoVal) c = Color.Yellow;

                Graphics.DrawTexturedLine(_laserTex, vec, vec2, c, f, depth - 1);
				for (int i = 1; i < 4; i++)
				{
					Graphics.DrawTexturedLine(_laserTex, vec2, vec2 + normalized * 2f, c * (1f - i * 0.2f), f, depth - 1);
					vec2 += normalized * 2f;
				}
			}
		}
		public SinWave sinner = new SinWave(0.1f);
        public override void DoUpdate()
        {
			if (laserSight && _laserTex == null)
			{
				_laserTex = Content.Load<Tex2D>("pointerLaser");
			}
			base.DoUpdate();
        }
        public bool _laserInit;
		private Tex2D _laserTex;

		public override void Draw()
		{
			float angle = this.angle;
			if (offDir > 0)
			{
				this.angle -= _angleOffset;
			}
			else
			{
				this.angle += _angleOffset;
			}
			base.Draw();
			this.angle = angle;
		}

		public StateBinding _loadStateBinding = new StateBinding("_loadState", -1, false, false);

		public StateBinding _angleOffsetBinding = new StateBinding("_angleOffset", -1, false, false);

		public StateBinding _awesomeBinding = new StateBinding("awesome");

		public StateBinding _netLoadBinding = new NetSoundBinding("_netLoad");

		public NetSoundEffect _netLoad = new NetSoundEffect(new string[]
		{
			"loadSniper"
		});

		public int _loadState = -1;

		public int _loadAnimation = -1;

		public float _angleOffset;
	}
}
