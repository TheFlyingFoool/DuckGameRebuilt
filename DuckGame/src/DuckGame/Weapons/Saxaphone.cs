using System;
using System.Collections.Generic;

namespace DuckGame
{
    // Token: 0x0200052E RID: 1326
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("previewPriority", true)]
    public class Saxaphone : Gun
    {
        // Token: 0x06001E22 RID: 7714 RVA: 0x00142EB8 File Offset: 0x001410B8
        public Saxaphone(float xval, float yval) : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this.graphic = new Sprite("saxaphone", 0f, 0f);
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

        // Token: 0x06001E23 RID: 7715 RVA: 0x0001411D File Offset: 0x0001231D
        public override void Initialize()
        {
            base.Initialize();
        }

        // Token: 0x06001E24 RID: 7716 RVA: 0x0014301C File Offset: 0x0014121C
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
                        this.notePitch = (float)keyboardNote / 12f + 0.01f;
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
                            Sound snd = SFX.Play("sax" + Change.ToString(note), 1f, 0f, 0f, false);
                            this.noteSound = snd;
                            Level.Add(new MusicNote(base.barrelPosition.x, base.barrelPosition.y, base.barrelVector));
                        }
                        else
                        {
                            this.noteSound.Pitch = Maths.Clamp((this.notePitch - this.hitPitch) * 0.1f, -1f, 1f);
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
                    this.OnReleaseAction();
                }
                else
                {
                    this.handOffset = new Vec2(5f + (1f - this.handPitch) * 2f, -2f + (1f - this.handPitch) * 4f);
                    this.handAngle = (1f - this.handPitch) * 0.4f * (float)this.offDir;
                    this._holdOffset = new Vec2(4f + this.handPitch * 2f, this.handPitch * 2f);
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

        // Token: 0x06001E25 RID: 7717 RVA: 0x00003845 File Offset: 0x00001A45
        public override void OnPressAction()
        {
        }

        // Token: 0x06001E26 RID: 7718 RVA: 0x00003845 File Offset: 0x00001A45
        public override void OnReleaseAction()
        {
        }

        // Token: 0x06001E27 RID: 7719 RVA: 0x00003845 File Offset: 0x00001A45
        public override void Fire()
        {
        }

        // Token: 0x04001DCE RID: 7630
        public StateBinding _notePitchBinding = new StateBinding("notePitch", -1, false, false);

        // Token: 0x04001DCF RID: 7631
        public StateBinding _handPitchBinding = new StateBinding("handPitch", -1, false, false);

        // Token: 0x04001DD0 RID: 7632
        public float notePitch;

        // Token: 0x04001DD1 RID: 7633
        public float handPitch;

        // Token: 0x04001DD2 RID: 7634
        private float prevNotePitch;

        // Token: 0x04001DD3 RID: 7635
        private float hitPitch;

        // Token: 0x04001DD4 RID: 7636
        private Sound noteSound;

        // Token: 0x04001DD5 RID: 7637
        private List<InstrumentNote> _notes = new List<InstrumentNote>();
    }
}
