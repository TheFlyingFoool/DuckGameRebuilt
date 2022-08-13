// Decompiled with JetBrains decompiler
// Type: DuckGame.Trumpet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        private int currentPitch = -1;
        private bool leftPressed;
        private bool rightPressed;

        public Trumpet(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATLaser();
            _ammoType.range = 170f;
            _ammoType.accuracy = 0.8f;
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
                if (isServerForObject)
                {
                    if (owner.inputProfile.Pressed("SHOOT"))
                        currentPitch = 2;
                    if (owner.inputProfile.Pressed("STRAFE"))
                        currentPitch = 0;
                    if (owner.inputProfile.Pressed("RAGDOLL"))
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
                    if (owner.inputProfile.Released("STRAFE") && currentPitch == 0)
                        currentPitch = -1;
                    if (owner.inputProfile.Released("SHOOT") && currentPitch == 2)
                        currentPitch = -1;
                    if (owner.inputProfile.Released("RAGDOLL") && currentPitch == 1)
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
                    notePitch = currentPitch < 0 || _raised ? 0f : (float)(currentPitch / 3.0 +  0.01f);
                }
                if (notePitch != prevNotePitch)
                {
                    if (notePitch != 0.0)
                    {
                        if (noteSound != null)
                        {
                            noteSound.Stop();
                            noteSound = null;
                        }
                        int num = (int)Math.Round(notePitch * 3.0);
                        if (num < 0)
                            num = 0;
                        if (num > 12)
                            num = 12;
                        if (noteSound == null)
                        {
                            hitPitch = notePitch;
                            noteSound = SFX.Play("trumpet0" + Change.ToString(num + 1), 0.8f);
                            Level.Add(new MusicNote(barrelPosition.x, barrelPosition.y, barrelVector));
                        }
                        else
                            noteSound.Pitch = Maths.Clamp((float)((notePitch - hitPitch) *  0.01f), -1f, 1f);
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
                fingerPositionSprite.frame = currentPitch + 1;
                fingerPositionSprite.depth = depth - 100;
                fingerPositionSprite.flipH = offDir <= 0;
                fingerPositionSprite.angle = 0f;
                Vec2 vec2 = Offset(new Vec2(-8f, -2f));
                Graphics.Draw(fingerPositionSprite, vec2.x, vec2.y);
            }
            base.Draw();
        }
    }
}
