using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class Trumpet : Gun
    {
        public StateBinding _notePitchBinding = new StateBinding(nameof(notePitch));
        public float notePitch;
        private float prevNotePitch;
        private float hitPitch;
        private Sound noteSound;
        private List<InstrumentNote> _notes = new List<InstrumentNote>();
        public int currentPitch = -1;
        private bool leftPressed;
        private bool rightPressed;
        private SpriteMap _fingerSprite;

        public Trumpet(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f
            };
            wideBarrel = true;
            barrelInsertOffset = new Vec2(-4f, -2f);
            _type = "gun";
            graphic = new Sprite("trumpet");
            center = new Vec2(12f, 5f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(12f, 8f);
            _barrelOffsetTL = new Vec2(24f, 4f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _holdOffset = new Vec2(6f, 2f);
            hoverRaise = false;
            ignoreHands = true;
            _notePitchBinding.skipLerp = true;
            editorTooltip = "The poor man's trombone.";
            isFatal = false;
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (this.owner is Duck owner && owner.inputProfile != null)
            {
                hideLeftWing = ignoreHands = !raised;
                if (isServerForObject && !Recorderator.Playing)
                {
                    if (owner.inputProfile.Pressed(Triggers.Shoot))
                        currentPitch = 2;
                    if (owner.inputProfile.Pressed(Triggers.Strafe))
                        currentPitch = 0;
                    if (owner.inputProfile.Pressed(Triggers.Ragdoll))
                        currentPitch = 1;
                    if (owner.inputProfile.leftTrigger > 0.5 && !leftPressed)
                    {
                        currentPitch = 2;
                        leftPressed = true;
                    }
                    if (owner.inputProfile.rightTrigger > 0.5 && !rightPressed)
                    {
                        currentPitch = 3;
                        rightPressed = true;
                    }
                    if (owner.inputProfile.Released(Triggers.Strafe) && currentPitch == 0)
                        currentPitch = -1;
                    if (owner.inputProfile.Released(Triggers.Shoot) && currentPitch == 2)
                        currentPitch = -1;
                    if (owner.inputProfile.Released(Triggers.Ragdoll) && currentPitch == 1)
                        currentPitch = -1;
                    if (owner.inputProfile.leftTrigger <= 0.5)
                    {
                        if (currentPitch == 2 && leftPressed)
                            currentPitch = -1;
                        leftPressed = false;
                    }
                    if (owner.inputProfile.rightTrigger <= 0.5)
                    {
                        if (currentPitch == 3 && rightPressed)
                            currentPitch = -1;
                        rightPressed = false;
                    }
                    notePitch = currentPitch < 0 || _raised ? 0f : (float)(currentPitch / 3f + 0.01f);
                }
                if (Recorderator.Playing)
                {
                    notePitch = currentPitch < 0 || _raised ? 0f : (float)(currentPitch / 3f + 0.01f);
                }
                if (notePitch != prevNotePitch)
                {
                    if (notePitch != 0)
                    {
                        if (noteSound != null)
                        {
                            noteSound.Stop();
                            noteSound = null;
                        }
                        int num = (int)Math.Round(notePitch * 3f);
                        if (num < 0)
                            num = 0;
                        if (num > 12)
                            num = 12;
                        if (noteSound == null)
                        {
                            hitPitch = notePitch;
                            SFX.DontSave = 1;
                            noteSound = SFX.Play("trumpet0" + Change.ToString(num + 1), 0.8f);
                            Level.Add(new MusicNote(barrelPosition.x, barrelPosition.y, barrelVector));
                        }
                        else
                            noteSound.Pitch = Maths.Clamp((float)((notePitch - hitPitch) * 0.01f), -1f, 1f);
                    }
                    else if (noteSound != null)
                    {
                        noteSound.Stop();
                        noteSound = null;
                    }
                }
                if (_raised)
                {
                    collisionOffset = new Vec2(4f, -4f);
                    collisionSize = new Vec2(8f, 8f);
                    _holdOffset = new Vec2(0f, 0f);
                    handOffset = new Vec2(0f, 0f);
                    OnReleaseAction();
                }
                else
                {
                    collisionOffset = new Vec2(-6f, -4f);
                    collisionSize = new Vec2(8f, 8f);
                    _holdOffset = new Vec2(10f, -2f);
                    handOffset = new Vec2(5f, -2f);
                }
            }
            else
            {
                leftPressed = false;
                rightPressed = false;
                currentPitch = -1;
                collisionOffset = new Vec2(-6f, -4f);
                collisionSize = new Vec2(8f, 8f);
                _holdOffset = new Vec2(6f, 2f);
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
            if (duck != null && !raised)
            {
                SpriteMap fingerPositionSprite = duck.profile.persona.fingerPositionSprite;
                if (_fingerSprite == null || _fingerSprite.texture != fingerPositionSprite.texture)
                    _fingerSprite = fingerPositionSprite.CloneMap();
                _fingerSprite.frame = currentPitch + 1;
                _fingerSprite.depth = depth - 100;
                _fingerSprite.flipH = offDir <= 0;
                _fingerSprite.angle = 0f;
                _fingerSprite.LerpState.CanLerp = true;
                _fingerSprite.SkipIntraTick = duck.SkipIntratick;
                Vec2 vec2 = Offset(new Vec2(-8f, -2f));
                Material mat = Graphics.material;
                Graphics.material = null;
                _fingerSprite.position = vec2;
                _fingerSprite.Draw();
                Graphics.material = mat;
            }
            base.Draw();
        }
    }
}
