// Decompiled with JetBrains decompiler
// Type: DuckGame.Trombone
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
    public class Trombone : Gun
    {
        public StateBinding _notePitchBinding = new StateBinding(nameof(notePitch));
        public StateBinding _handPitchBinding = new StateBinding(nameof(handPitch));
        public float notePitch;
        public float handPitch;
        private float prevNotePitch;
        private float hitPitch;
        private Sound noteSound;
        private List<InstrumentNote> _notes = new List<InstrumentNote>();
        private Sprite _slide;
        private float _slideVal;

        public Trombone(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this.wideBarrel = true;
            this.barrelInsertOffset = new Vec2(-1f, -3f);
            this._type = "gun";
            this.graphic = new Sprite("tromboneBody");
            this.center = new Vec2(10f, 16f);
            this.collisionOffset = new Vec2(-4f, -5f);
            this.collisionSize = new Vec2(8f, 11f);
            this._barrelOffsetTL = new Vec2(19f, 14f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._holdOffset = new Vec2(6f, 2f);
            this._slide = new Sprite("tromboneSlide");
            this._slide.CenterOrigin();
            this._notePitchBinding.skipLerp = true;
            this.editorTooltip = "Just look at this thing. It's amazing. The instrument of kings.";
            this.isFatal = false;
        }

        public override void Initialize() => base.Initialize();

        public float NormalizePitch(float val) => val;

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
                            this.noteSound = SFX.Play("trombone" + Change.ToString(num));
                            Level.Add(new MusicNote(this.barrelPosition.x, this.barrelPosition.y, this.barrelVector));
                        }
                        else
                            this.noteSound.Pitch = Maths.Clamp(this.notePitch - this.hitPitch, -1f, 1f);
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
                    this.handOffset = new Vec2((float)(6.0 + (1.0 - handPitch) * 4.0), (float)((1.0 - handPitch) * 4.0 - 4.0));
                    this.handAngle = (float)((1.0 - handPitch) * 0.400000005960464) * offDir;
                    this._holdOffset = new Vec2((float)(5.0 + handPitch * 2.0), (float)(handPitch * 2.0 - 9.0));
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
            Graphics.material = this.material;
            this.Draw(this._slide, new Vec2((float)(6.0 + _slideVal * 8.0), 0f), -1);
            Graphics.material = material;
        }
    }
}
