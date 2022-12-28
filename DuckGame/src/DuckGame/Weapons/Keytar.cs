// Decompiled with JetBrains decompiler
// Type: DuckGame.Keytar
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class Keytar : Gun
    {
        private Sound noteSound;
        public string[] presets = new string[5]
        {
      "metalkeys",
      "touch",
      "deepbass",
      "strings2",
      "synthdrums"
        };
        public EditorProperty<int> color;
        private SpriteMap _sprite;
        private SpriteMap _keybed;
        private SpriteMap _settingStrip;
        public float handPitch;
        public float notePitch;
        public float prevNotePitch;
        private float _benderOffset;
        private float _bender;
        public sbyte preset;
        public int prevNote;
        public float playPitch;
        public byte colorVariation = byte.MaxValue;
        private byte _prevColorVariation = byte.MaxValue;
        public StateBinding _ruinedBinding = new StateBinding("_ruined");
        public StateBinding _benderBinding = new StateBinding(nameof(bender));
        public StateBinding _notePitchBinding = new StateBinding(nameof(notePitch));
        public StateBinding _handPitchBinding = new StateBinding(nameof(handPitch));
        public StateBinding _presetBinding = new StateBinding(nameof(preset));
        public StateBinding _brokenKeyBinding = new StateBinding(nameof(brokenKey));
        public StateBinding _colorVariationBinding = new StateBinding(nameof(colorVariation));
        private bool _prevRuined;
        private sbyte _prevPreset;
        private byte brokenKey;
        private List<Sound> _prevSounds = new List<Sound>();
        public bool duckMoving;

        public override void EditorPropertyChanged(object property) => RefreshColor();

        public override Sprite GeneratePreview(
          int wide = 16,
          int high = 16,
          bool transparentBack = false,
          Effect effect = null,
          RenderTarget2D target = null)
        {
            color.value = 0;
            RefreshColor();
            return base.GeneratePreview(wide, high, transparentBack, effect, target);
        }

        public Keytar(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f
            };
            _type = "gun";
            _sprite = new SpriteMap("keytar", 23, 8);
            graphic = _sprite;
            center = new Vec2(12f, 3f);
            collisionOffset = new Vec2(-8f, -1f);
            collisionSize = new Vec2(16f, 7f);
            _barrelOffsetTL = new Vec2(12f, 3f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _holdOffset = new Vec2(-8f, 2f);
            _keybed = new SpriteMap("keytarKeybed", 13, 4);
            _settingStrip = new SpriteMap("keytarSettingStrip", 9, 1);
            thickness = 1f;
            color = new EditorProperty<int>(-1, this, -1f, 3f, 1f);
            collideSounds.Add("rockHitGround");
            _canRaise = false;
            ignoreHands = true;
            _editorName = nameof(Keytar);
            editorTooltip = "Eats batteries and steals hearts.";
            isFatal = false;
        }

        private void RefreshColor()
        {
            if (color.value < 0)
            {
                colorVariation = (byte)Rando.Int(3);
                if (Rando.Int(100) != 0 || Level.current is Editor)
                    return;
                colorVariation = 4;
            }
            else
                colorVariation = (byte)color.value;
        }

        public override void Initialize()
        {
            RefreshColor();
            base.Initialize();
        }

        public float bender
        {
            get => Maths.Clamp(_bender + _benderOffset, 0f, 1f);
            set => _bender = value;
        }

        public int currentNote
        {
            get
            {
                int currentNote = (int)Math.Round(handPitch * 13.0);
                if (currentNote < 0)
                    currentNote = 0;
                if (currentNote > 12)
                    currentNote = 12;
                return currentNote;
            }
        }

        public override void CheckIfHoldObstructed()
        {
            if (!(this.owner is Duck owner))
                return;
            owner.holdObstructed = false;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (isServerForObject && (Math.Abs(hSpeed) > 4.0 || Math.Abs(vSpeed) > 4.0) && !_ruined && owner == null)
                _ruined = true;
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (isServerForObject)
                _ruined = true;
            else if (bullet.isLocal && owner == null)
            {
                Fondle(this, DuckNetwork.localConnection);
                _ruined = true;
            }
            return base.Hit(bullet, hitPos);
        }

        public override bool Destroy(DestroyType type = null)
        {
            if (isServerForObject)
                _ruined = true;
            return false;
        }

        public override void Update()
        {
            if (colorVariation != _prevColorVariation)
                _keybed = new SpriteMap(colorVariation == 4 ? "keytarKeybedBlue" : "keytarKeybed", 13, 4);
            _prevColorVariation = colorVariation;
            if (!_prevRuined && _ruined)
            {
                SFX.Play("smallElectronicBreak", 0.8f, Rando.Float(-0.1f, 0.1f));
                for (int index = 0; index < 8; ++index)
                    Level.Add(Spark.New(x + Rando.Float(-8f, 8f), y + Rando.Float(-4f, 4f), new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
                if (isServerForObject && Rando.Int(5) == 0)
                    brokenKey = (byte)(1 + Rando.Int(12));
            }
            _prevRuined = _ruined;
            if (this.owner is Duck owner)
            {
                if (isServerForObject && owner.inputProfile != null)
                {
                    if (_ruined && Rando.Int(20) == 0)
                        _benderOffset += Rando.Float(-0.05f, 0.05f);
                    if (owner.inputProfile.Pressed(Triggers.Strafe))
                    {
                        ++preset;
                        if (preset >= presets.Length)
                            preset = 0;
                    }
                    handPitch = owner.inputProfile.leftTrigger;
                    bender = owner.inputProfile.rightTrigger;
                    if (owner.inputProfile.hasMotionAxis)
                        handPitch += owner.inputProfile.motionAxis;
                    int num = Keyboard.CurrentNote(owner.inputProfile, this);
                    if (num >= 0)
                    {
                        notePitch = (num / 13f + 0.01f);
                        handPitch = notePitch;
                    }
                    else
                        notePitch = !owner.inputProfile.Down(Triggers.Shoot) ? 0f : handPitch + 0.01f;
                }
                else
                    _benderOffset = 0f;
                if (noteSound != null && _ruined && Rando.Int(30) == 0)
                    noteSound.Volume *= 0.75f;
                duckMoving = owner._sprite.currentAnimation == "run";
                hideRightWing = true;
                ignoreHands = true;
                hideLeftWing = !owner._hovering;
                if (noteSound != null && owner._hovering)
                {
                    noteSound.Stop();
                    noteSound = null;
                }
                int num1 = currentNote;
                if (preset == presets.Length - 1)
                    num1 = (int)Math.Round(currentNote / 2.0);
                if (notePitch == 0.0 || (num1 != prevNote || noteSound != null && noteSound.Pitch + 1.0 != bender / 12.0) && !owner._hovering)
                {
                    if (notePitch != 0.0)
                    {
                        if (noteSound == null || num1 != prevNote)
                        {
                            bool flag = brokenKey != 0 && num1 == brokenKey - 1;
                            float vol = 1f;
                            if (_ruined)
                            {
                                float val = vol - (0.15f + Rando.Float(-0.15f));
                                if (Rando.Int(5) == 0)
                                    val -= 0.13f;
                                if (Rando.Int(7) == 0)
                                    val -= 0.25f;
                                if (flag)
                                    val = Rando.Int(3) == 0 ? 0.2f : 0f;
                                vol = Maths.Clamp(val, 0f, 1f);
                            }
                            if (noteSound != null)
                                _prevSounds.Add(noteSound);
                            noteSound = SFX.Play(presets[preset] + "-" + (num1 < 10 ? "0" : "") + Change.ToString(num1), vol, -1f);
                            playPitch = notePitch;
                            prevNote = num1;
                            if (_ruined)
                            {
                                _benderOffset = Rando.Float(0.05f, 0.1f);
                                if (Rando.Int(10) == 0)
                                    _benderOffset = Rando.Float(0.15f, 0.2f);
                                if (flag)
                                    _benderOffset += 0.1f + Rando.Float(0.2f);
                            }
                            if (!_ruined)
                                Level.Add(new MusicNote(barrelPosition.x, barrelPosition.y, barrelVector));
                        }
                        else
                            noteSound.Pitch = (float)(bender / 12.0 - 1.0);
                    }
                    else
                    {
                        if (noteSound != null)
                        {
                            _prevSounds.Add(noteSound);
                            noteSound = null;
                        }
                        prevNote = -1;
                    }
                }
                handOffset = new Vec2((5f + (1f - handPitch) * 2f), ((1f - handPitch) * 4f - 2f));
                handAngle = (float)((duckMoving ? 0f : 1f - handPitch) * 0.2f + 0.2f + (notePitch > 0f ? 0.05f : 0f)) * offDir;
                _holdOffset = new Vec2((float)(handPitch * 1f - 4f), (-handPitch * 1f + 3f - (duckMoving ? 3f : 0f) + (duck._hovering ? 2f : 0f)));
                collisionOffset = new Vec2(-1f, -7f);
                collisionSize = new Vec2(2f, 16f);
            }
            else
            {
                collisionOffset = new Vec2(-8f, -2f);
                collisionSize = new Vec2(16f, 6f);
                if (noteSound != null)
                {
                    _prevSounds.Add(noteSound);
                    noteSound = null;
                }
            }
            for (int index = 0; index < _prevSounds.Count; ++index)
            {
                if (_prevSounds[index].Volume < 0.01f)
                {
                    _prevSounds[index].Stop();
                    _prevSounds.RemoveAt(index);
                    --index;
                }
                else
                    _prevSounds[index].Volume = Lerp.Float(_prevSounds[index].Volume, 0f, 0.15f);
            }
            if (preset != _prevPreset)
                SFX.Play("click");
            _prevPreset = preset;
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
            _sprite.frame = (_ruined ? 1 : 0) + colorVariation * 2;
            if (duck != null && !raised)
            {
                Material mat = Graphics.material;
                Graphics.material = null;
                SpriteMap fingerPositionSprite = duck.profile.persona.fingerPositionSprite;
                if (!duck._hovering)
                {
                    float x;
                    if (noteSound == null)
                    {
                        fingerPositionSprite.frame = 5;
                        x = (int)(2f + (currentNote / 12f * 8f - 4f));
                    }
                    else
                    {
                        fingerPositionSprite.frame = 6 + currentNote;
                        x = 2f;
                    }
                    fingerPositionSprite.depth = depth + 4;
                    fingerPositionSprite.flipH = offDir <= 0;
                    fingerPositionSprite.angle = angle;
                    Vec2 vec2 = Offset(new Vec2(x, -3f));
                    Graphics.Draw(fingerPositionSprite, vec2.x, vec2.y);
                }
                fingerPositionSprite.frame = 19;
                Vec2 vec2_1 = Offset(new Vec2(-8f, (-bender * 1f)));
                Graphics.Draw(fingerPositionSprite, vec2_1.x, vec2_1.y);
                Graphics.material = mat;
            }
            _keybed.depth = depth + 2;
            _keybed.flipH = offDir <= 0;
            _keybed.angle = angle;
            _keybed.frame = notePitch != 0f ? currentNote + 1 : 0;
            Vec2 vec2_2 = Offset(new Vec2(-5f, -2f));
            Graphics.Draw(_keybed, vec2_2.x, vec2_2.y);
            _settingStrip.depth = depth + 2;
            _settingStrip.flipH = offDir <= 0;
            _settingStrip.angle = angle;
            _settingStrip.frame = preset;
            Vec2 vec2_3 = Offset(new Vec2(-1f, 3f));
            Graphics.Draw(_settingStrip, vec2_3.x, vec2_3.y);
            base.Draw();
        }
    }
}
