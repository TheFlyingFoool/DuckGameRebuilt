using System;
using System.Collections.Generic;

namespace DuckGame
{
	[EditorGroup("Guns|Misc")]
	[BaggedProperty("isFatal", false)]
	public class Trombone : Gun
	{
		public Trombone(float xval, float yval) : base(xval, yval)
		{
			this.ammo = 4;
			this._ammoType = new ATLaser();
			this._ammoType.range = 170f;
			this._ammoType.accuracy = 0.8f;
			this.wideBarrel = true;
			this.barrelInsertOffset = new Vec2(-1f, -3f);
			this._type = "gun";
			this.graphic = new Sprite("tromboneBody", 0f, 0f);
			this.center = new Vec2(10f, 16f);
			this.collisionOffset = new Vec2(-4f, -5f);
			this.collisionSize = new Vec2(8f, 11f);
			this._barrelOffsetTL = new Vec2(19f, 14f);
			this._fireSound = "smg";
			this._fullAuto = true;
			this._fireWait = 1f;
			this._kickForce = 3f;
			this._holdOffset = new Vec2(6f, 2f);
			this._slide = new Sprite("tromboneSlide", 0f, 0f);
			this._slide.CenterOrigin();
			this._notePitchBinding.skipLerp = true;
			this.editorTooltip = "Just look at this thing. It's amazing. The instrument of kings.";
			this.isFatal = false;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public float NormalizePitch(float val)
		{
			return val;
		}

		public override void Update()
		{
			Duck d = this.owner as Duck;
			if (d != null)
			{
				if (base.isServerForObject && d.inputProfile != null)
				{
					this.handPitch = d.inputProfile.leftTrigger;
					if (d.inputProfile.hasMotionAxis)
					{
						this.handPitch += d.inputProfile.motionAxis;
					}
					int keyboardNote = Keyboard.CurrentNote(d.inputProfile, this);
					if (keyboardNote >= 0)
					{
						this.notePitch = keyboardNote / 12f + 0.01f;
						this.handPitch = this.notePitch;
						if (this.notePitch != this.prevNotePitch)
						{
							this.prevNotePitch = 0f;
							if (this.noteSound != null)
							{
								this.noteSound.Stop();
								this.noteSound = null;
							}
						}
					}
					else if (d.inputProfile.Down("SHOOT"))
					{
						this.notePitch = this.handPitch + 0.01f;
					}
					else
					{
						this.notePitch = 0f;
					}
				}
				if (this.notePitch != this.prevNotePitch)
				{
					if (this.notePitch != 0f)
					{
						int note = (int)Math.Round((double)(this.notePitch * 12f));
						if (note < 0)
						{
							note = 0;
						}
						if (note > 12)
						{
							note = 12;
						}
						if (this.noteSound == null)
						{
							this.hitPitch = this.notePitch;
							Sound snd = SFX.Play("trombone" + Change.ToString(note), 1f, 0f, 0f, false);
							this.noteSound = snd;
							Level.Add(new MusicNote(base.barrelPosition.x, base.barrelPosition.y, base.barrelVector));
						}
						else
						{
							this.noteSound.Pitch = Maths.Clamp(this.notePitch - this.hitPitch, -1f, 1f);
						}
					}
					else if (this.noteSound != null)
					{
						this.noteSound.Stop();
						this.noteSound = null;
					}
				}
				if (this._raised)
				{
					this.handAngle = 0f;
					this.handOffset = new Vec2(0f, 0f);
					this._holdOffset = new Vec2(0f, 2f);
					this.collisionOffset = new Vec2(-4f, -7f);
					this.collisionSize = new Vec2(8f, 16f);
				}
				else
				{
					this.handOffset = new Vec2(6f + (1f - this.handPitch) * 4f, -4f + (1f - this.handPitch) * 4f);
                    this.handAngle = (1f - this.handPitch) * 0.4f * offDir;
                    this._holdOffset = new Vec2(5f + this.handPitch * 2f, -9f + this.handPitch * 2f);
					this.collisionOffset = new Vec2(-4f, -7f);
					this.collisionSize = new Vec2(2f, 16f);
					this._slideVal = 1f - this.handPitch;
				}
			}
			else
			{
				this.collisionOffset = new Vec2(-4f, -5f);
				this.collisionSize = new Vec2(8f, 11f);
			}
			this.prevNotePitch = this.notePitch;
			base.Update();
		}

		public override void OnPressAction()
		{
		}

		public override void OnReleaseAction()
		{
		}

		public override void Fire()
		{
		}

		public override void Draw()
		{
			base.Draw();
			Material material = Graphics.material;
			Graphics.material = base.material;
			base.Draw(this._slide, new Vec2(6f + this._slideVal * 8f, 0f), -1);
			Graphics.material = material;
		}

		public StateBinding _notePitchBinding = new StateBinding("notePitch", -1, false, false);

		public StateBinding _handPitchBinding = new StateBinding("handPitch", -1, false, false);

		public float notePitch;

		public float handPitch;

		private float prevNotePitch;

		private float hitPitch;

		private Sound noteSound;

		private List<InstrumentNote> _notes = new List<InstrumentNote>();

		private Sprite _slide;

		private float _slideVal;
	}
}
