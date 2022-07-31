// Decompiled with JetBrains decompiler
// Type: DuckGame.Saxaphone
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("previewPriority", true)]
    public class Saxaphone : Gun
    {
        public StateBinding _notePitchBinding = new StateBinding(nameof(notePitch));
        public StateBinding _handPitchBinding = new StateBinding(nameof(handPitch));
        public float notePitch;
        public float handPitch;
        private float prevNotePitch;
        private float hitPitch;
        private Sound noteSound;
        private List<InstrumentNote> _notes = new List<InstrumentNote>();

        public Saxaphone(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this.graphic = new Sprite("saxaphone");
            this.center = new Vec2(20f, 18f);
            this.collisionOffset = new Vec2(-4f, -7f);
            this.collisionSize = new Vec2(8f, 16f);
            this._barrelOffsetTL = new Vec2(24f, 16f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._holdOffset = new Vec2(6f, 2f);
            this.holsterAngle = 90f;
            this._notePitchBinding.skipLerp = true;
            this._editorName = "Saxophone";
            this.editorTooltip = "Crave the dulcet tones of smooth jazz? This is the item for you.";
            this.isFatal = false;
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (this.owner is Duck owner)
            {
                if (this.isServerForObject && owner.inputProfile != null)
                {
                    this.handPitch = owner.inputProfile.leftTrigger;
                    if (owner.inputProfile.hasMotionAxis)
                        this.handPitch += owner.inputProfile.motionAxis;
                    int num = Keyboard.CurrentNote(owner.inputProfile, this);
                    if (num >= 0)
                    {
                        this.notePitch = (float)(num / 12.0 + 0.00999999977648258);
                        this.handPitch = this.notePitch;
                        if (notePitch != (double)this.prevNotePitch)
                        {
                            this.prevNotePitch = 0f;
                            if (this.noteSound != null)
                            {
                                this.noteSound.Stop();
                                this.noteSound = null;
                            }
                        }
                    }
                    else
                        this.notePitch = !owner.inputProfile.Down("SHOOT") ? 0f : this.handPitch + 0.01f;
                }
                if (notePitch != (double)this.prevNotePitch)
                {
                    if (notePitch != 0.0)
                    {
                        int num = (int)Math.Round(notePitch * 12.0);
                        if (num < 0)
                            num = 0;
                        if (num > 12)
                            num = 12;
                        if (this.noteSound == null)
                        {
                            this.hitPitch = this.notePitch;
                            this.noteSound = SFX.Play("sax" + Change.ToString(num));
                            Level.Add(new MusicNote(this.barrelPosition.x, this.barrelPosition.y, this.barrelVector));
                        }
                        else
                            this.noteSound.Pitch = Maths.Clamp((float)((notePitch - (double)this.hitPitch) * 0.100000001490116), -1f, 1f);
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
                    this.OnReleaseAction();
                }
                else
                {
                    this.handOffset = new Vec2((float)(5.0 + (1.0 - handPitch) * 2.0), (float)((1.0 - handPitch) * 4.0 - 2.0));
                    this.handAngle = (float)((1.0 - handPitch) * 0.400000005960464) * offDir;
                    this._holdOffset = new Vec2((float)(4.0 + handPitch * 2.0), this.handPitch * 2f);
                    this.collisionOffset = new Vec2(-1f, -7f);
                    this.collisionSize = new Vec2(2f, 16f);
                }
            }
            else
            {
                this.collisionOffset = new Vec2(-4f, -7f);
                this.collisionSize = new Vec2(8f, 16f);
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
    }
}
