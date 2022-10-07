// Decompiled with JetBrains decompiler
// Type: DuckGame.Window
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff")]
    public class Window : Block, IPlatform, ISequenceItem, IDontMove
    {
        public StateBinding _positionBinding = new StateBinding(nameof(netPosition));
        public StateBinding _hitPointsBinding = new StateBinding(nameof(hitPoints));
        public StateBinding _destroyedBinding = new StateBinding("_destroyed");
        public StateBinding _damageMultiplierBinding = new StateBinding(nameof(damageMultiplier));
        public StateBinding _shakeTimesBinding = new StateBinding(nameof(shakeTimes));
        public NetIndex4 shakeTimes = (NetIndex4)0;
        private NetIndex4 _localShakeTimes = (NetIndex4)0;
        public float maxHealth = 5f;
        public float hitPoints = 5f;
        public float damageMultiplier = 1f;
        protected Sprite _sprite;
        protected Sprite _borderSprite;
        protected Sprite _barSprite;
        public bool landed = true;
        private List<Vec2> _hits = new List<Vec2>();
        private SinWaveManualUpdate _shake = (SinWaveManualUpdate)0.8f;
        private float _shakeVal;
        private Vec2 _shakeMult = Vec2.Zero;
        public bool floor;
        public bool doShake;
        protected WindowFrame _frame;
        public EditorProperty<int> windowHeight;
        public EditorProperty<int> tint = new EditorProperty<int>(0, max: Window.windowColors.Count - 1, increment: 1f);
        public EditorProperty<bool> valid;
        public EditorProperty<bool> bars = new EditorProperty<bool>(false);
        public static List<Color> windowColors = new List<Color>()
    {
      new Color(102, 186, 245),
      Color.Red,
      Color.Orange,
      Color.Yellow,
      Color.Pink,
      Color.Purple,
      Color.Green,
      Color.Lime,
      Color.Maroon,
      Color.Magenta,
      Color.Cyan,
      Color.DarkGoldenrod
    };
        public bool noframe;
        public bool lobbyRemoving;
        private Vec2 _enter;
        private bool _wrecked;
        private bool _hasGlass = true;

        public override Vec2 netPosition
        {
            get => position;
            set
            {
                if (!(position != value))
                    return;
                position = value;
                if (_frame != null)
                    _frame.position = position;
                Level.current.things.quadTree.Remove(this);
                Level.current.things.quadTree.Add(this);
            }
        }

        public override void EditorPropertyChanged(object property)
        {
            UpdateHeight();
            sequence.isValid = valid.value;
        }

        public override void SetTranslation(Vec2 translation)
        {
            if (_frame != null)
                _frame.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public virtual void UpdateHeight()
        {
            float num = windowHeight.value * 16f;
            center = new Vec2(3f, 0f);
            if (floor)
            {
                collisionSize = new Vec2(num, 6f);
                collisionOffset = new Vec2((float)(-num + 16.0), -2f);
                _sprite.angleDegrees = -90f;
            }
            else
            {
                collisionSize = new Vec2(6f, num);
                collisionOffset = new Vec2(-3f, (float)(-num + 8.0));
                _sprite.angle = 0f;
            }
            _sprite.yscale = num;
            _borderSprite.yscale = num;
            if (_frame != null)
                _frame.high = num;
            sequence.isValid = valid.value;
        }

        public Window(float xpos, float ypos)
          : base(xpos, ypos)
        {
            windowHeight = new EditorProperty<int>(2, this, 1f, 16f, 1f);
            valid = new EditorProperty<bool>(false, this);
            _sprite = new Sprite("window32", 6f, 1f);
            _barSprite = new Sprite("windowBars", 8f, 1f);
            _borderSprite = new Sprite("window32border");
            _editorIcon = new Sprite("windowIconVertical");
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            physicsMaterial = PhysicsMaterial.Glass;
            center = new Vec2(3f, 24f);
            collisionSize = new Vec2(6f, 32f);
            collisionOffset = new Vec2(-3f, -24f);
            depth = -0.5f;
            _editorName = nameof(Window);
            editorTooltip = "Classic window. Really opens up the room.";
            thickness = 0.3f;
            _sprite.color = new Color(1f, 1f, 1f, 0.2f);
            alpha = 0.7f;
            breakForce = 3f;
            _canFlip = false;
            _translucent = true;
            UpdateHeight();
        }

        public override void Initialize()
        {
            if (!floor && !noframe)
            {
                _frame = new WindowFrame(x, y, floor);
                Level.Add(_frame);
            }
            UpdateHeight();
        }

        public override void Terminate()
        {
            if (!(Level.current is Editor) && !_wrecked && !lobbyRemoving)
            {
                _wrecked = true;
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
                {
                    GlassParticle glassParticle = new GlassParticle(x - 4f + Rando.Float(8f), y - 16f + Rando.Float(32f), Vec2.Zero, tint.value)
                    {
                        hSpeed = (Rando.Float(1f) > 0.5 ? 1f : -1f) * Rando.Float(3f),
                        vSpeed = -Rando.Float(1f)
                    };
                    Level.Add(glassParticle);
                }
                if (this is FloorWindow)
                {
                    int ix = (int)(DGRSettings.ActualParticleMultiplier * 8);
                    float fr = 32f / ix;
                    for (int index = 0; index < ix; ++index)
                    {
                        Level.Add(new GlassDebris(false, left + index * fr, y, -Rando.Float(2f), -Rando.Float(2f), 1));
                    }
                    foreach (PhysicsObject physicsObject in Level.CheckLineAll<PhysicsObject>(topLeft + new Vec2(-2f, -3f), topRight + new Vec2(2f, -3f)))
                    {
                        physicsObject._sleeping = false;
                        physicsObject.vSpeed -= 2f;
                    }
                }
                else
                {
                    int ix = (int)(DGRSettings.ActualParticleMultiplier * 8);
                    float fr = 32f / ix;
                    for (int index = 0; index < ix; ++index)
                    {
                        Level.Add(new GlassDebris(false, x, top + index * fr, -Rando.Float(2f), -Rando.Float(2f), 1, tint.value));
                    }
                }
                SFX.Play("glassBreak");
            }
            if (!floor && !_wrecked)
            {
                Level.Remove(_frame);
                _frame = null;
            }
            base.Terminate();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (!_hasGlass)
                return base.Hit(bullet, hitPos);
            _enter = hitPos + bullet.travelDirNormalized;
            if (_enter.x < x && _enter.x < left + 2.0f)
                _enter.x = left;
            else if (_enter.x > x && _enter.x > right - 2.0f)
                _enter.x = right;
            if (_enter.y < y && _enter.y < top + 2.0f)
                _enter.y = top;
            else if (_enter.y > y && _enter.y > bottom - 2.0f)
                _enter.y = bottom;
            if (hitPoints <= 0.0)
                return false;
            hitPos -= bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
            {
                Level.Add(new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized, tint.value));
                if (index > 32) break;
                //Anticrash measure, since damagemultiplier is synced you can make it an insanely high number to spawn infinite particles on someone elses side
                //this still doesn't completely solve the problem but its a good enough bandaid since the particles will remove themselves from the cap, making it
                //only a lag exploit rather than a softlock/crash like it is in base dg
                //-NiK0
            }
            SFX.Play("glassHit", 0.5f);
            if (isServerForObject && bullet.isLocal)
            {
                hitPoints -= damageMultiplier;
                ++damageMultiplier;
            }
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            if (!_hasGlass)
                return;
            _hits.Add(_enter);
            Vec2 vec2 = exitPos - bullet.travelDirNormalized;
            if (vec2.x < x && vec2.x < left + 2.0f)
                vec2.x = left;
            else if (vec2.x > x && vec2.x > right - 2.0f)
                vec2.x = right;
            if (vec2.y < y && vec2.y < top + 2.0f)
                vec2.y = top;
            else if (vec2.y > y && vec2.y > bottom - 2.0f)
                vec2.y = bottom;
            _hits.Add(vec2);
            exitPos += bullet.travelDirNormalized;
            for (int index = 0; index < 1.0f + damageMultiplier / 2.0f; ++index)
                Level.Add(new GlassParticle(exitPos.x, exitPos.y, -bullet.travelDirNormalized, tint.value));
        }

        public void Shake()
        {
            if (_hasGlass)
                SFX.Play("glassBump", 0.7f);
            _shakeVal = 3.1415927f;
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            with.Fondle(this);
            if (floor && with.top > top && CalculateImpactPower(with, from) > 2.8f && with.isServerForObject)
            {
                if (with is Duck duck)
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                Destroy(new DTImpact(with));
            }
            else
            {
                float num = Math.Abs(with.hSpeed) + Math.Abs(with.vSpeed);
                if (!destroyed && num > 1.5f)
                {
                    ++shakeTimes;
                    if (isServerForObject && Level.current is TeamSelect2 && with is PhysicsObject && (with as PhysicsObject).gravMultiplier < 0.1f)
                        Destroy(new DTImpact(with));
                }
                if (!destroyed || !(with is Duck duck))
                    return;
                RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
            }
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (!_hasGlass)
                return false;
            if (bars.value)
                _hasGlass = false;
            else
                Level.Remove(this);
            if (sequence != null && sequence.isValid)
            {
                sequence.Finished();
                if (ChallengeLevel.running)
                    ++ChallengeLevel.goodiesGot;
            }
            return !bars.value;
        }

        public override void Update()
        {
            _shake.Update();
            breakForce = (float)(6.0 * (hitPoints / maxHealth));
            if (hitPoints <= 0.0)
                Destroy(new DTImpact(null));
            base.Update();
            if (damageMultiplier > 1.0)
                damageMultiplier -= 0.2f;
            else
                damageMultiplier = 1f;
            _shakeMult = Lerp.Vec2(_shakeMult, Vec2.Zero, 0.1f);
            if (_localShakeTimes < shakeTimes)
            {
                Shake();
                _localShakeTimes = shakeTimes;
            }
            _shakeVal = Lerp.Float(_shakeVal, 0f, 0.05f);
        }

        public override void Draw()
        {
            Vec2 zero = Vec2.Zero;
            float num1 = (float)((float)_shake * _shakeVal * 0.800000011920929);
            if (floor)
                zero.y = num1;
            else
                zero.x = num1;
            position += zero;
            float num2 = windowHeight.value * 16f;
            _sprite.depth = depth;
            _borderSprite.depth = depth;
            _borderSprite.angle = _sprite.angle;
            _barSprite.depth = _sprite.depth + 4;
            _barSprite.yscale = _sprite.yscale;
            _barSprite.alpha = _sprite.alpha;
            _barSprite.angle = angle;
            if (_hasGlass)
            {
                Color windowColor = Window.windowColors[tint.value];
                windowColor.a = 51;
                _sprite.color = windowColor;
                alpha = 0.7f;
                if (floor)
                {
                    Graphics.Draw(_sprite, (float)(x - num2 + 16.0), y + 4f);
                    Graphics.Draw(_borderSprite, (float)(x - num2 + 16.0), y + 4f);
                }
                else
                {
                    Graphics.Draw(_sprite, x - 3f, (float)(y - num2 + 8.0));
                    Graphics.Draw(_borderSprite, x - 3f, (float)(y - num2 + 8.0));
                }
                for (int index = 0; index < _hits.Count; index += 2)
                {
                    if (index + 1 > _hits.Count)
                        return;

                    Color col = new Color((byte)(windowColor.r * 0.5f), (byte)(windowColor.g * 0.5f), (byte)(windowColor.b * 0.8f), (byte)178);
                    Graphics.DrawLine(this._hits[index] + zero, this._hits[index + 1] + zero, col, 1f, default(Depth));
                }
            }
            position -= zero;
            if (floor)
            {
                if (bars.value)
                    Graphics.Draw(_barSprite, (float)(x - num2 + 16.0), y + 5f);
            }
            else if (bars.value)
                Graphics.Draw(_barSprite, x - 4f, (float)(y - num2 + 8.0));
            base.Draw();
        }
    }
}
