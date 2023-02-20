// Decompiled with JetBrains decompiler
// Type: DuckGame.Trophy
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("canSpawn", false)]
    public class Trophy : Holdable
    {
        public StateBinding _actionBinding = new StateBinding(nameof(ownerAction));
        private bool ownerAction;
        private SpriteMap _sprite;
        private int _framesSinceThrown;
        private float _throwSpin;

        public Trophy(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("trophy", 17, 20);
            graphic = _sprite;
            center = new Vec2(8f, 10f);
            collisionOffset = new Vec2(-7f, -10f);
            collisionSize = new Vec2(15f, 19f);
            depth = -0.5f;
            thickness = 4f;
            weight = 4f;
            flammable = 0.3f;
            collideSounds.Add("rockHitGround2");
            physicsMaterial = PhysicsMaterial.Metal;
            _holdOffset = new Vec2(-2f, 0f);
            editorTooltip = "It doesn't count if you don't EARN it.";
        }

        public override void Update()
        {
            if (isServerForObject)
                ownerAction = action;
            if (duck != null && ownerAction)
            {
                _holdOffset = Lerp.Vec2(_holdOffset, new Vec2(-13f, -4f), 2f);
                angle = Lerp.Float(angle, -1f, 0.1f);
                handFlip = true;
                handOffset = Lerp.Vec2(handOffset, new Vec2(-3f, -4f), 1f);
                _canRaise = false;
            }
            else
            {
                float num = 1f;
                if (duck != null)
                    angle = Lerp.Float(angle, 0f, 0.1f * num);
                else
                    num = 20f;
                _holdOffset = Lerp.Vec2(_holdOffset, new Vec2(-2f, 0f), num * 2f);
                handFlip = false;
                handOffset = Lerp.Vec2(handOffset, new Vec2(0f, 0f), 1f * num);
                _canRaise = true;
            }
            if (owner == null && level.simulatePhysics)
            {
                if (_framesSinceThrown == 0)
                    _throwSpin = angleDegrees;
                ++_framesSinceThrown;
                if (_framesSinceThrown > 15)
                    _framesSinceThrown = 15;
                angleDegrees = _throwSpin;
                bool flag1 = false;
                bool flag2 = false;
                if ((Math.Abs(hSpeed) + Math.Abs(vSpeed) > 2.0 || !grounded) && gravMultiplier > 0.0 && !flag2 && !_grounded)
                {
                    if (offDir > 0)
                        _throwSpin += (float)((Math.Abs(hSpeed * 2f) + Math.Abs(vSpeed)) * 1.0 + 5.0);
                    else
                        _throwSpin -= (float)((Math.Abs(hSpeed * 2f) + Math.Abs(vSpeed)) * 1.0 + 5.0);
                    flag1 = true;
                }
                if (!flag1 | flag2)
                {
                    _throwSpin %= 360f;
                    if (_throwSpin < 0.0)
                        _throwSpin += 360f;
                    if (flag2)
                        _throwSpin = Math.Abs(_throwSpin - 90f) >= Math.Abs(_throwSpin + 90f) ? Lerp.Float(-90f, 0f, 16f) : Lerp.Float(_throwSpin, 90f, 16f);
                    else if (_throwSpin > 90.0 && _throwSpin < 270.0)
                    {
                        _throwSpin = Lerp.Float(_throwSpin, 180f, 14f);
                    }
                    else
                    {
                        if (_throwSpin > 180.0)
                            _throwSpin -= 360f;
                        else if (_throwSpin < -180.0)
                            _throwSpin += 360f;
                        _throwSpin = Lerp.Float(_throwSpin, 0f, 14f);
                    }
                }
            }
            else
                _throwSpin = 0f;
            base.Update();
        }
    }
}
