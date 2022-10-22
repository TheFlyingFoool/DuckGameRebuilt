// Decompiled with JetBrains decompiler
// Type: DuckGame.PowerHolster
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("isInDemo", true)]
    public class PowerHolster : Holster
    {
        public StateBinding _triggerBinding = new StateBinding(nameof(trigger));
        public bool trigger;

        public PowerHolster(float pX, float pY)
          : base(pX, pY)
        {
            _sprite = new SpriteMap("powerHolster", 14, 12);
            _overPart = new SpriteMap("powerHolsterOver", 10, 3)
            {
                center = new Vec2(6f, -1f)
            };
            _underPart = new SpriteMap("powerHolsterUnder", 11, 10)
            {
                center = new Vec2(10f, 8f)
            };
            graphic = _sprite;
            collisionOffset = new Vec2(-5f, -5f);
            collisionSize = new Vec2(10f, 10f);
            center = new Vec2(6f, 6f);
            physicsMaterial = PhysicsMaterial.Wood;
            _equippedDepth = 4;
            _wearOffset = new Vec2(1f, 1f);
            editorTooltip = "Lets you carry around an additional item!";
            backOffset = -10f;
        }

        public override void Update()
        {
            if (isServerForObject)
            {
                if (_equippedDuck != null && _equippedDuck.inputProfile != null)
                    trigger = _equippedDuck.inputProfile.Down("QUACK");
                if (containedObject != null && (!trigger || !containedObject.HolsterActivate(this)))
                {
                    containedObject.triggerAction = trigger;
                    if (containedObject is PowerHolster)
                        (containedObject as PowerHolster).trigger = trigger;
                }
                if (owner == null)
                    trigger = false;
            }
            base.Update();
        }

        protected override void DrawParts()
        {
            if (_equippedDuck != null)
            {
                Thing thing = owner;
                if (_equippedDuck != null && _equippedDuck._trapped != null)
                    thing = _equippedDuck._trapped;
                _overPart.flipH = owner.offDir <= 0;
                _overPart.angle = angle;
                _overPart.alpha = alpha;
                _overPart.scale = scale;
                _overPart.depth = thing.depth + 5;
                _overPart.frame = _equippedDuck.quack > 0 ? 1 : 0;
                Graphics.Draw(_overPart, x, y);
                _underPart.flipH = owner.offDir <= 0;
                _underPart.angle = angle;
                _underPart.alpha = alpha;
                _underPart.scale = scale;
                if (_equippedDuck.ragdoll != null && _equippedDuck.ragdoll.part2 != null)
                    _underPart.depth = _equippedDuck.ragdoll.part2.depth + -11;
                else
                    _underPart.depth = thing.depth + -7;
                _underPart.frame = trigger ? 1 : 0;
                Vec2 vec2 = Offset(new Vec2(-2f, 0f));
                Graphics.Draw(_underPart, vec2.x, vec2.y);
            }
            else
                _sprite.frame = trigger ? 1 : 0;
        }
    }
}
