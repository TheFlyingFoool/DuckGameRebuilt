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
            ammo = 4;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f
            };
            wideBarrel = true;
            barrelInsertOffset = new Vec2(-1f, -3f);
            _type = "gun";
            graphic = new Sprite("tromboneBody", 0f, 0f);
            center = new Vec2(10f, 16f);
            collisionOffset = new Vec2(-4f, -5f);
            collisionSize = new Vec2(8f, 11f);
            _barrelOffsetTL = new Vec2(19f, 14f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _holdOffset = new Vec2(6f, 2f);
            _slide = new Sprite("tromboneSlide", 0f, 0f);
            _slide.CenterOrigin();
            _notePitchBinding.skipLerp = true;
            editorTooltip = "Just look at this thing. It's amazing. The instrument of kings.";
            isFatal = false;
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
                            Sound snd = SFX.Play("trombone" + Change.ToString(note), 1f, 0f, 0f, false);
                            noteSound = snd;
                            Level.Add(new MusicNote(barrelPosition.x, barrelPosition.y, barrelVector));
                        }
                        else
                        {
                            noteSound.Pitch = Maths.Clamp(notePitch - hitPitch, -1f, 1f);
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
                }
                else
                {
                    handOffset = new Vec2(6f + (1f - handPitch) * 4f, -4f + (1f - handPitch) * 4f);
                    handAngle = (1f - handPitch) * 0.4f * offDir;
                    _holdOffset = new Vec2(5f + handPitch * 2f, -9f + handPitch * 2f);
                    collisionOffset = new Vec2(-4f, -7f);
                    collisionSize = new Vec2(2f, 16f);
                    _slideVal = 1f - handPitch;
                }
            }
            else
            {
                collisionOffset = new Vec2(-4f, -5f);
                collisionSize = new Vec2(8f, 11f);
            }
            prevNotePitch = notePitch;
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
            Draw(ref _slide, new Vec2(6f + _slideVal * 8f, 0f), -1);
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
