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
            ammo = 4;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f
            };
            _type = "gun";
            graphic = new Sprite("saxaphone", 0f, 0f);
            center = new Vec2(20f, 18f);
            collisionOffset = new Vec2(-4f, -7f);
            collisionSize = new Vec2(8f, 16f);
            _barrelOffsetTL = new Vec2(24f, 16f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _holdOffset = new Vec2(6f, 2f);
            holsterAngle = 90f;
            _notePitchBinding.skipLerp = true;
            _editorName = "Saxophone";
            editorTooltip = "Crave the dulcet tones of smooth jazz? This is the item for you.";
            isFatal = false;
        }

        // Token: 0x06001E23 RID: 7715 RVA: 0x0001411D File Offset: 0x0001231D
        public override void Initialize()
        {
            base.Initialize();
        }

        // Token: 0x06001E24 RID: 7716 RVA: 0x0014301C File Offset: 0x0014121C
        public override void Update()
        {
            Duck d = owner as Duck;
            if (d != null)
            {
                if (isServerForObject && d.inputProfile != null && !Recorderator.Playing)
                {
                    handPitch = d.inputProfile.leftTrigger;
                    if (d.inputProfile.hasMotionAxis)
                    {
                        handPitch += d.inputProfile.motionAxis;
                    }
                    int keyboardNote = Keyboard.CurrentNote(d.inputProfile, this);
                    if (keyboardNote >= 0)
                    {
                        notePitch = keyboardNote / 12f + 0.01f;
                        handPitch = notePitch;
                        if (notePitch != prevNotePitch)
                        {
                            prevNotePitch = 0f;
                            if (noteSound != null)
                            {
                                noteSound.Stop();
                                noteSound = null;
                            }
                        }
                    }
                    else if (d.inputProfile.Down(Triggers.Shoot))
                    {
                        notePitch = handPitch + 0.01f;
                    }
                    else
                    {
                        notePitch = 0f;
                    }
                }
                if (notePitch != prevNotePitch)
                {
                    if (notePitch != 0f)
                    {
                        int note = (int)Math.Round((double)(notePitch * 12f));
                        if (note < 0)
                        {
                            note = 0;
                        }
                        if (note > 12)
                        {
                            note = 12;
                        }
                        if (noteSound == null)
                        {
                            hitPitch = notePitch;
                            SFX.DontSave = 1;
                            Sound snd = SFX.Play("sax" + Change.ToString(note), 1f, 0f, 0f, false);
                            noteSound = snd;
                            Level.Add(new MusicNote(barrelPosition.x, barrelPosition.y, barrelVector));
                        }
                        else
                        {
                            noteSound.Pitch = Maths.Clamp((notePitch - hitPitch) * 0.1f, -1f, 1f);
                        }
                    }
                    else if (noteSound != null)
                    {
                        noteSound.Stop();
                        noteSound = null;
                    }
                }
                if (_raised)
                {
                    handAngle = 0f;
                    handOffset = new Vec2(0f, 0f);
                    _holdOffset = new Vec2(0f, 2f);
                    collisionOffset = new Vec2(-4f, -7f);
                    collisionSize = new Vec2(8f, 16f);
                    OnReleaseAction();
                }
                else
                {
                    handOffset = new Vec2(5f + (1f - handPitch) * 2f, -2f + (1f - handPitch) * 4f);
                    handAngle = (1f - handPitch) * 0.4f * offDir;
                    _holdOffset = new Vec2(4f + handPitch * 2f, handPitch * 2f);
                    collisionOffset = new Vec2(-1f, -7f);
                    collisionSize = new Vec2(2f, 16f);
                }
            }
            else
            {
                collisionOffset = new Vec2(-4f, -7f);
                collisionSize = new Vec2(8f, 16f);
            }
            prevNotePitch = notePitch;
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
