// Decompiled with JetBrains decompiler
// Type: DuckGame.PowerHolster
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._sprite = new SpriteMap("powerHolster", 14, 12);
            this._overPart = new SpriteMap("powerHolsterOver", 10, 3);
            this._overPart.center = new Vec2(6f, -1f);
            this._underPart = new SpriteMap("powerHolsterUnder", 11, 10);
            this._underPart.center = new Vec2(10f, 8f);
            this.graphic = (Sprite)this._sprite;
            this.collisionOffset = new Vec2(-5f, -5f);
            this.collisionSize = new Vec2(10f, 10f);
            this.center = new Vec2(6f, 6f);
            this.physicsMaterial = PhysicsMaterial.Wood;
            this._equippedDepth = 4;
            this._wearOffset = new Vec2(1f, 1f);
            this.editorTooltip = "Lets you carry around an additional item!";
            this.backOffset = -10f;
        }

        public override void Update()
        {
            if (this.isServerForObject)
            {
                if (this._equippedDuck != null && this._equippedDuck.inputProfile != null)
                    this.trigger = this._equippedDuck.inputProfile.Down("QUACK");
                if (this.containedObject != null && (!this.trigger || !this.containedObject.HolsterActivate((Holster)this)))
                {
                    this.containedObject.triggerAction = this.trigger;
                    if (this.containedObject is PowerHolster)
                        (this.containedObject as PowerHolster).trigger = this.trigger;
                }
                if (this.owner == null)
                    this.trigger = false;
            }
            base.Update();
        }

        protected override void DrawParts()
        {
            if (this._equippedDuck != null)
            {
                Thing thing = this.owner;
                if (this._equippedDuck != null && this._equippedDuck._trapped != null)
                    thing = (Thing)this._equippedDuck._trapped;
                this._overPart.flipH = this.owner.offDir <= (sbyte)0;
                this._overPart.angle = this.angle;
                this._overPart.alpha = this.alpha;
                this._overPart.scale = this.scale;
                this._overPart.depth = thing.depth + 5;
                this._overPart.frame = this._equippedDuck.quack > 0 ? 1 : 0;
                Graphics.Draw((Sprite)this._overPart, this.x, this.y);
                this._underPart.flipH = this.owner.offDir <= (sbyte)0;
                this._underPart.angle = this.angle;
                this._underPart.alpha = this.alpha;
                this._underPart.scale = this.scale;
                if (this._equippedDuck.ragdoll != null && this._equippedDuck.ragdoll.part2 != null)
                    this._underPart.depth = this._equippedDuck.ragdoll.part2.depth + -11;
                else
                    this._underPart.depth = thing.depth + -7;
                this._underPart.frame = this.trigger ? 1 : 0;
                Vec2 vec2 = this.Offset(new Vec2(-2f, 0.0f));
                Graphics.Draw((Sprite)this._underPart, vec2.x, vec2.y);
            }
            else
                this._sprite.frame = this.trigger ? 1 : 0;
        }
    }
}
