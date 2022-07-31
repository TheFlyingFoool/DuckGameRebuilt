// Decompiled with JetBrains decompiler
// Type: DuckGame.Keytar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public override void EditorPropertyChanged(object property) => this.RefreshColor();

        public override Sprite GeneratePreview(
          int wide = 16,
          int high = 16,
          bool transparentBack = false,
          Effect effect = null,
          RenderTarget2D target = null)
        {
            this.color.value = 0;
            this.RefreshColor();
            return base.GeneratePreview(wide, high, transparentBack, effect, target);
        }

        public Keytar(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 4;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this._sprite = new SpriteMap("keytar", 23, 8);
            this.graphic = _sprite;
            this.center = new Vec2(12f, 3f);
            this.collisionOffset = new Vec2(-8f, -1f);
            this.collisionSize = new Vec2(16f, 7f);
            this._barrelOffsetTL = new Vec2(12f, 3f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 3f;
            this._holdOffset = new Vec2(-8f, 2f);
            this._keybed = new SpriteMap("keytarKeybed", 13, 4);
            this._settingStrip = new SpriteMap("keytarSettingStrip", 9, 1);
            this.thickness = 1f;
            this.color = new EditorProperty<int>(-1, this, -1f, 3f, 1f);
            this.collideSounds.Add("rockHitGround");
            this._canRaise = false;
            this.ignoreHands = true;
            this._editorName = nameof(Keytar);
            this.editorTooltip = "Eats batteries and steals hearts.";
            this.isFatal = false;
        }

        private void RefreshColor()
        {
            if (this.color.value < 0)
            {
                this.colorVariation = (byte)Rando.Int(3);
                if (Rando.Int(100) != 0 || Level.current is Editor)
                    return;
                this.colorVariation = 4;
            }
            else
                this.colorVariation = (byte)this.color.value;
        }

        public override void Initialize()
        {
            this.RefreshColor();
            base.Initialize();
        }

        public float bender
        {
            get => Maths.Clamp(this._bender + this._benderOffset, 0f, 1f);
            set => this._bender = value;
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
            if (this.isServerForObject && ((double)Math.Abs(this.hSpeed) > 4.0 || (double)Math.Abs(this.vSpeed) > 4.0) && !this._ruined && this.owner == null)
                this._ruined = true;
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (this.isServerForObject)
                this._ruined = true;
            else if (bullet.isLocal && this.owner == null)
            {
                Thing.Fondle(this, DuckNetwork.localConnection);
                this._ruined = true;
            }
            return base.Hit(bullet, hitPos);
        }

        public override bool Destroy(DestroyType type = null)
        {
            if (this.isServerForObject)
                this._ruined = true;
            return false;
        }

        public override void Update()
        {
            if (colorVariation != _prevColorVariation)
                this._keybed = new SpriteMap(this.colorVariation == 4 ? "keytarKeybedBlue" : "keytarKeybed", 13, 4);
            this._prevColorVariation = this.colorVariation;
            if (!this._prevRuined && this._ruined)
            {
                SFX.Play("smallElectronicBreak", 0.8f, Rando.Float(-0.1f, 0.1f));
                for (int index = 0; index < 8; ++index)
                    Level.Add(Spark.New(this.x + Rando.Float(-8f, 8f), this.y + Rando.Float(-4f, 4f), new Vec2(Rando.Float(-1f, 1f), Rando.Float(-1f, 1f))));
                if (this.isServerForObject && Rando.Int(5) == 0)
                    this.brokenKey = (byte)(1 + Rando.Int(12));
            }
            this._prevRuined = this._ruined;
            if (this.owner is Duck owner)
            {
                if (this.isServerForObject && owner.inputProfile != null)
                {
                    if (this._ruined && Rando.Int(20) == 0)
                        this._benderOffset += Rando.Float(-0.05f, 0.05f);
                    if (owner.inputProfile.Pressed("STRAFE"))
                    {
                        ++this.preset;
                        if (preset >= this.presets.Length)
                            this.preset = 0;
                    }
                    this.handPitch = owner.inputProfile.leftTrigger;
                    this.bender = owner.inputProfile.rightTrigger;
                    if (owner.inputProfile.hasMotionAxis)
                        this.handPitch += owner.inputProfile.motionAxis;
                    int num = Keyboard.CurrentNote(owner.inputProfile, this);
                    if (num >= 0)
                    {
                        this.notePitch = (num / 13f + 0.01f);
                        this.handPitch = this.notePitch;
                    }
                    else
                        this.notePitch = !owner.inputProfile.Down("SHOOT") ? 0f : this.handPitch + 0.01f;
                }
                else
                    this._benderOffset = 0f;
                if (this.noteSound != null && this._ruined && Rando.Int(30) == 0)
                    this.noteSound.Volume *= 0.75f;
                this.duckMoving = owner._sprite.currentAnimation == "run";
                this.hideRightWing = true;
                this.ignoreHands = true;
                this.hideLeftWing = !owner._hovering;
                if (this.noteSound != null && owner._hovering)
                {
                    this.noteSound.Stop();
                    this.noteSound = null;
                }
                int num1 = this.currentNote;
                if (preset == this.presets.Length - 1)
                    num1 = (int)Math.Round(currentNote / 2.0);
                if (notePitch == 0.0 || (num1 != this.prevNote || this.noteSound != null && (double)this.noteSound.Pitch + 1.0 != (double)this.bender / 12.0) && !owner._hovering)
                {
                    if (notePitch != 0.0)
                    {
                        if (this.noteSound == null || num1 != this.prevNote)
                        {
                            bool flag = this.brokenKey != 0 && num1 == brokenKey - 1;
                            float vol = 1f;
                            if (this._ruined)
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
                            if (this.noteSound != null)
                                this._prevSounds.Add(this.noteSound);
                            this.noteSound = SFX.Play(this.presets[preset] + "-" + (num1 < 10 ? "0" : "") + Change.ToString(num1), vol, -1f);
                            this.playPitch = this.notePitch;
                            this.prevNote = num1;
                            if (this._ruined)
                            {
                                this._benderOffset = Rando.Float(0.05f, 0.1f);
                                if (Rando.Int(10) == 0)
                                    this._benderOffset = Rando.Float(0.15f, 0.2f);
                                if (flag)
                                    this._benderOffset += 0.1f + Rando.Float(0.2f);
                            }
                            if (!this._ruined)
                                Level.Add(new MusicNote(this.barrelPosition.x, this.barrelPosition.y, this.barrelVector));
                        }
                        else
                            this.noteSound.Pitch = (float)((double)this.bender / 12.0 - 1.0);
                    }
                    else
                    {
                        if (this.noteSound != null)
                        {
                            this._prevSounds.Add(this.noteSound);
                            this.noteSound = null;
                        }
                        this.prevNote = -1;
                    }
                }
                this.handOffset = new Vec2((5f + (1f - handPitch) * 2f), ((1f - handPitch) * 4f - 2f));
                this.handAngle = (float)((this.duckMoving ? 0f : 1f - handPitch) * 0.2f + 0.2f + (notePitch > 0f ? 0.05f : 0f)) * offDir;
                this._holdOffset = new Vec2((float)(handPitch * 1f - 4f), (-this.handPitch * 1f + 3f - (this.duckMoving ? 3f : 0f) + (this.duck._hovering ? 2f : 0f)));
                this.collisionOffset = new Vec2(-1f, -7f);
                this.collisionSize = new Vec2(2f, 16f);
            }
            else
            {
                this.collisionOffset = new Vec2(-8f, -2f);
                this.collisionSize = new Vec2(16f, 6f);
                if (this.noteSound != null)
                {
                    this._prevSounds.Add(this.noteSound);
                    this.noteSound = null;
                }
            }
            for (int index = 0; index < this._prevSounds.Count; ++index)
            {
                if (this._prevSounds[index].Volume < 0.01f)
                {
                    this._prevSounds[index].Stop();
                    this._prevSounds.RemoveAt(index);
                    --index;
                }
                else
                    this._prevSounds[index].Volume = Lerp.Float(this._prevSounds[index].Volume, 0f, 0.15f);
            }
            if (preset != _prevPreset)
                SFX.Play("click");
            this._prevPreset = this.preset;
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
            this._sprite.frame = (this._ruined ? 1 : 0) + colorVariation * 2;
            if (this.duck != null && !this.raised)
            {
                SpriteMap fingerPositionSprite = this.duck.profile.persona.fingerPositionSprite;
                if (!this.duck._hovering)
                {
                    float x;
                    if (this.noteSound == null)
                    {
                        fingerPositionSprite.frame = 5;
                        x = (int)(2f + (currentNote / 12f * 8f - 4f));
                    }
                    else
                    {
                        fingerPositionSprite.frame = 6 + this.currentNote;
                        x = 2f;
                    }
                    fingerPositionSprite.depth = this.depth + 4;
                    fingerPositionSprite.flipH = this.offDir <= 0;
                    fingerPositionSprite.angle = this.angle;
                    Vec2 vec2 = this.Offset(new Vec2(x, -3f));
                    DuckGame.Graphics.Draw(fingerPositionSprite, vec2.x, vec2.y);
                }
                fingerPositionSprite.frame = 19;
                Vec2 vec2_1 = this.Offset(new Vec2(-8f, (-this.bender * 1f)));
                DuckGame.Graphics.Draw(fingerPositionSprite, vec2_1.x, vec2_1.y);
            }
            this._keybed.depth = this.depth + 2;
            this._keybed.flipH = this.offDir <= 0;
            this._keybed.angle = this.angle;
            this._keybed.frame = notePitch != 0f ? this.currentNote + 1 : 0;
            Vec2 vec2_2 = this.Offset(new Vec2(-5f, -2f));
            DuckGame.Graphics.Draw(_keybed, vec2_2.x, vec2_2.y);
            this._settingStrip.depth = this.depth + 2;
            this._settingStrip.flipH = this.offDir <= 0;
            this._settingStrip.angle = this.angle;
            this._settingStrip.frame = preset;
            Vec2 vec2_3 = this.Offset(new Vec2(-1f, 3f));
            DuckGame.Graphics.Draw(_settingStrip, vec2_3.x, vec2_3.y);
            base.Draw();
        }
    }
}
