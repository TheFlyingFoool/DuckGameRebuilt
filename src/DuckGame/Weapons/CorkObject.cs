// Decompiled with JetBrains decompiler
// Type: DuckGame.CorkObject
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class CorkObject : PhysicsObject, ISwing, IPullBack
    {
        private Thing _gun;
        public Sprite _ropeSprite;
        private Rope _rope;
        private Harpoon _sticker;

        public CorkObject(float pX, float pY, Thing pOwner)
          : base(pX, pY)
        {
            this.graphic = new Sprite("cork");
            this._collisionSize = new Vec2(4f, 4f);
            this._collisionOffset = new Vec2(-2f, -3f);
            this.center = new Vec2(3f, 3f);
            this._gun = pOwner;
            this.weight = 0.1f;
            this.bouncy = 0.5f;
            this.airFrictionMult = 0f;
            this._ropeSprite = new Sprite("grappleWire")
            {
                center = new Vec2(8f, 0f)
            };
        }

        public Rope GetRopeParent(Thing child)
        {
            for (Rope ropeParent = this._rope; ropeParent != null; ropeParent = ropeParent.attach2 as Rope)
            {
                if (ropeParent.attach2 == child)
                    return ropeParent;
            }
            return null;
        }

        public override void Initialize()
        {
            if (this._gun != null)
            {
                this._sticker = new Harpoon(this);
                this.level.AddThing(_sticker);
                this._sticker.SetStuckPoint(this._gun.position);
                this._rope = new Rope(this.x, this.y, null, _sticker, this, tex: this._ropeSprite, belongsTo: this);
                Level.Add(_rope);
            }
            base.Initialize();
        }

        public override void Terminate()
        {
            if (this._sticker != null)
                Level.Remove(_sticker);
            if (this._rope != null)
                this._rope.RemoveRope();
            base.Terminate();
        }

        public float WindUp(float pAmount)
        {
            if ((double)pAmount <= 0.0 || _rope.startLength <= 0.0)
                return 100f;
            this._rope.Pull(-pAmount);
            this._rope.startLength -= pAmount;
            return this._rope.startLength;
        }

        public override void Update()
        {
            if (this._rope != null)
            {
                if (!this.grounded)
                    this.specialFrictionMod = 0f;
                else
                    this.specialFrictionMod = 1f;
                this._rope.position = this.position;
                this._rope.SetServer(this.isServerForObject);
                Vec2 vec2_1 = this._rope.attach1.position - this._rope.attach2.position;
                bool flag = true;
                if ((double)this._rope.properLength < 0.0)
                {
                    this._rope.startLength = this._rope.properLength = 100f;
                    flag = false;
                }
                if ((double)vec2_1.length > (double)this._rope.properLength)
                {
                    vec2_1 = vec2_1.normalized;
                    Vec2 position2 = this.position;
                    Vec2 vec2_2 = this._rope.attach2.position + vec2_1 * this._rope.properLength;
                    Vec2 end = vec2_2;
                    Vec2 vec2_3;
                    // ref Vec2 local = ref vec2_3;
                    Level.CheckRay<Block>(position2, end, out vec2_3);
                    if (flag)
                    {
                        this.hSpeed = vec2_2.x - this.position.x;
                        this.vSpeed = vec2_2.y - this.position.y;
                        this.gravMultiplier = 0f;
                        float specialFrictionMod = this.specialFrictionMod;
                        this.specialFrictionMod = 0f;
                        this.airFrictionMult = 0f;
                        Vec2 lastPosition = this.lastPosition;
                        this.UpdatePhysics();
                        this.gravMultiplier = 1f;
                        this.specialFrictionMod = specialFrictionMod;
                        Vec2 vec2_4 = vec2_2 - lastPosition;
                        if ((double)vec2_4.length > 32.0)
                            this.position = vec2_2;
                        else if ((double)vec2_4.length > 6.0)
                        {
                            this.hSpeed = Rando.Float(-2f, 2f);
                            this.vSpeed = Rando.Float(-2f, 2f);
                        }
                        else
                        {
                            this.hSpeed = vec2_4.x;
                            this.vSpeed = vec2_4.y;
                        }
                    }
                    else
                        this.position = vec2_2;
                }
                this._sticker.SetStuckPoint((this._gun as Gun).barrelPosition);
            }
            base.Update();
        }
    }
}
