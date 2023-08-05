// Decompiled with JetBrains decompiler
// Type: DuckGame.DemoBlaster
using System;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Guns")]
    [BaggedProperty("canSpawn", false)]
    public class ReDemoBlaster : Gun
    {
        private FluidStream _stream;
        private ConstantSound _sound;
        private int _wait;

        public ReDemoBlaster(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 99;
            graphic = new Sprite("demoBlaster");
            center = new Vec2(18f, 8f);
            collisionOffset = new Vec2(-16f, -8f);
            collisionSize = new Vec2(32f, 15f);
            _barrelOffsetTL = new Vec2(37f, 7f);
            _kickForce = 0.4f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-1f, -4f);
            weight = 8f;
            editorTooltip = "";
            _editorName = "Demo Blaster";
            physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Initialize()
        {
            _sound = new ConstantSound("demoBlaster");
        }

        public override void Terminate() => Level.Remove(_stream);


        public int extraCharge;

        public float shake;
        public override void Update()
        {
            _sound.lerpVolume = _triggerHeld ? 0.25f : 0f;
            if (_triggerHeld)
            {
                if (duck != null && isServerForObject)
                {
                    float f = extraCharge / 150f;
                    if (extraCharge == 151) f = 6;
                    Thing thing = duck;
                    if (duck._trapped != null)
                        thing = duck._trapped;
                    if (owner != null && duck.ragdoll != null && duck.ragdoll.part2 != null && duck.ragdoll.part1 != null && duck.ragdoll.part3 != null)
                    {
                        Vec2 vec2 = -barrelVector * f * 0.5f;
                        duck.ragdoll.part1.hSpeed += vec2.x;
                        duck.ragdoll.part1.vSpeed += vec2.y;
                        duck.ragdoll.part2.hSpeed += vec2.x;
                        duck.ragdoll.part2.vSpeed += vec2.y;
                        duck.ragdoll.part3.hSpeed += vec2.x;
                        duck.ragdoll.part3.vSpeed += vec2.y;
                    }
                    else
                    {

                        Vec2 vec2 = -barrelVector * f;
                        if (Math.Sign(thing.hSpeed) != Math.Sign(vec2.x) || Math.Abs(vec2.x) > Math.Abs(thing.hSpeed))
                            thing.hSpeed = vec2.x;
                        if (owner != null)
                        {
                            if (duck.crouch)
                                duck.sliding = true;
                            thing.vSpeed += vec2.y;
                        }
                        else
                            thing.vSpeed += vec2.y;
                    }
                }
                if (extraCharge > 90)
                {
                    if (extraCharge > 150)
                    {
                        _sound.lerpVolume = 0.15f;
                        extraCharge = 152;
                        shake = Lerp.Float(shake, 1.5f, 0.1f);
                        _sound.pitch = Lerp.Float(_sound.pitch, 1.8f, 0.01f);
                    }
                    else
                    {
                        _sound.lerpVolume = 0.2f;
                        shake = Lerp.Float(shake, 0.5f, 0.01f);
                        _sound.pitch = Lerp.Float(_sound.pitch, 1.4f, 0.01f);
                        if (_sound.pitch == 1.4f && isServerForObject) extraCharge++;
                    }
                }
                else
                {
                    shake = Lerp.Float(shake, 0.2f, 0.01f);
                    _sound.pitch = Lerp.Float(_sound.pitch, 0.9f, 0.01f);
                    if (_sound.pitch == 0.9f && isServerForObject) extraCharge++;
                }
            }
            else
            {
                shake = Lerp.Float(shake, 0, 0.1f);
                _sound.pitch = 0;
                extraCharge = 0;
            }
            base.Update();
        }
        public override void Draw()
        {
            Vec2 ps = position;

            if (extraCharge > 150)
            {

            }
            
            position += new Vec2(Rando.Float(-shake, shake), Rando.Float(-shake, shake));
            base.Draw();
            position = ps;
        }
        public override void OnPressAction()
        {
        }

        public override void OnHoldAction()
        {
            
        }
        public override void OnReleaseAction()
        {
            if (extraCharge > 90)
            {
                if (extraCharge > 150)
                {

                }
                else
                {

                }
            }
            else
            {

            }
        }
    }
}
