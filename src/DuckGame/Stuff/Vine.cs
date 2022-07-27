// Decompiled with JetBrains decompiler
// Type: DuckGame.Vine
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class Vine : Holdable, ISwing
    {
        protected SpriteMap _sprite;
        protected Harpoon _harpoon;
        public Rope _rope;
        public int sectionIndex;
        public EditorProperty<int> length = new EditorProperty<int>(4, min: 1f, max: 16f, increment: 1f);
        public Vine nextVine;
        public Vine prevVine;
        protected Sprite _vinePartSprite;
        public float initLength;
        public bool changeSpeed = true;
        protected Vec2 _wallPoint;
        protected Vec2 _grappleTravel;

        public List<VineSection> points
        {
            get
            {
                List<VineSection> points = new List<VineSection>();
                List<VineSection> collection = new List<VineSection>();
                Rope rope = this._rope;
                Vine vine = this;
                while (rope != null)
                {
                    VineSection vineSection = new VineSection()
                    {
                        pos2 = rope.attach1Point,
                        pos1 = rope.attach2Point
                    };
                    vineSection.length = (vineSection.pos1 - vineSection.pos2).length;
                    rope = rope.attach2 as Rope;
                    collection.Add(vineSection);
                    if (rope == null && vine.nextVine != null)
                    {
                        collection.Reverse();
                        points.AddRange(collection);
                        collection.Clear();
                        vine = vine.nextVine;
                        rope = vine._rope;
                    }
                }
                if (collection.Count > 0)
                {
                    collection.Reverse();
                    points.AddRange(collection);
                }
                float num1 = 0.0f;
                foreach (VineSection vineSection in points)
                    num1 += vineSection.length;
                int num2 = 0;
                foreach (VineSection vineSection in points)
                {
                    vineSection.lowestSection = num2 + (int)Math.Round(vineSection.length / (double)num1 * sectionIndex);
                    num2 = vineSection.lowestSection;
                }
                return points;
            }
        }

        public Vine(float xpos, float ypos, float init)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("vine", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this._vinePartSprite = new Sprite("vine")
            {
                center = new Vec2(8f, 0.0f)
            };
            this.collisionOffset = new Vec2(-5f, -4f);
            this.collisionSize = new Vec2(11f, 7f);
            this.weight = 0.1f;
            this.thickness = 0.1f;
            this.canPickUp = false;
            this.initLength = init;
            this.depth = - 0.5f;
        }

        public override void OnPressAction()
        {
        }

        public override void OnReleaseAction()
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            this._harpoon = new Harpoon(this);
            Level.Add(_harpoon);
            if (Level.current is Editor)
                return;
            Vec2 position = this.position;
            this.position.y += (int)this.length * 16 - 8;
            this._harpoon.noisy = false;
            this._harpoon.Fire(position + new Vec2(0.0f, -8f), new Vec2(0.0f, -1f));
            this._rope = new Rope(this.x, this.y, null, _harpoon, duck, true, this._vinePartSprite);
            if (initLength != 0.0)
                this._rope.properLength = this.initLength;
            Level.Add(_rope);
        }

        public override void Terminate()
        {
            if (this._rope == null)
                return;
            this._rope.RemoveRope();
            Level.Remove(_harpoon);
            Level.Remove(_rope);
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

        public void Degrapple()
        {
            if (this.nextVine != null && this.nextVine._rope != null)
            {
                this.nextVine._rope.attach2 = this._rope.attach2;
                this.nextVine._rope.properLength = (this.nextVine._rope.attach1Point - this._rope.attach2Point).length;
                this.nextVine.prevVine = null;
                this.nextVine = null;
            }
            if (this.prevVine != null)
                this.prevVine.nextVine = null;
            this._harpoon.Return();
            this._harpoon.visible = false;
            if (this._rope != null)
            {
                this._rope.RemoveRope();
                this._rope.visible = false;
                this.visible = false;
            }
            this._rope = null;
            if (this.duck != null)
            {
                this.duck.frictionMult = 1f;
                this.duck.gravMultiplier = 1f;
            }
            this.owner = null;
            this.frictionMult = 1f;
            this.gravMultiplier = 1f;
            this.visible = false;
            Level.Remove(_harpoon);
            Level.Remove(this);
            this.Update();
        }

        public void UpdateRopeStuff()
        {
            this._rope.Update();
            this.Update();
        }

        public void MoveDuck()
        {
            Vec2 vec2_1 = this._rope.attach1.position - this._rope.attach2.position;
            if ((double)vec2_1.length <= (double)this._rope.properLength)
                return;
            vec2_1 = vec2_1.normalized;
            if (this.duck == null)
                return;
            PhysicsObject duck = this.duck;
            Vec2 position = duck.position;
            duck.position = this._rope.attach2.position + vec2_1 * this._rope.properLength;
            Vec2 vec2_2 = duck.position - duck.lastPosition;
        }

        public Vec2 wallPoint => this._wallPoint;

        public Vec2 grappelTravel => this._grappleTravel;

        public override void Update()
        {
            base.Update();
            if (this.owner != null)
                this.offDir = this.owner.offDir;
            if (this.duck != null && (this.duck.ragdoll != null || this.duck._trapped != null || this.duck.dead))
            {
                this.owner = null;
                this._rope.visible = false;
            }
            if (this._rope == null)
                return;
            if (this.owner != null)
            {
                this._rope.position = this.owner.position;
            }
            else
            {
                this._rope.position = this.position;
                if (this.prevOwner != null)
                {
                    PhysicsObject prevOwner = this.prevOwner as PhysicsObject;
                    prevOwner.frictionMult = 1f;
                    prevOwner.gravMultiplier = 1f;
                    this._prevOwner = null;
                    this.frictionMult = 1f;
                    this.gravMultiplier = 1f;
                    Level.Remove(this);
                }
            }
            if (!this._harpoon.stuck)
                return;
            if (this.duck != null)
            {
                if (!this.duck.grounded)
                {
                    this.duck.frictionMult = 0.0f;
                }
                else
                {
                    this.duck.frictionMult = 1f;
                    this.duck.gravMultiplier = 1f;
                }
            }
            else if (!this.grounded)
            {
                this.frictionMult = 0.0f;
            }
            else
            {
                this.frictionMult = 1f;
                this.gravMultiplier = 1f;
            }
            Vec2 vec2_1 = this._rope.attach1.position - this._rope.attach2.position;
            if ((double)this._rope.properLength < 0.0)
                this._rope.properLength = vec2_1.length;
            if ((double)vec2_1.length <= (double)this._rope.properLength)
                return;
            Vec2 normalized = vec2_1.normalized;
            if (this.duck != null)
            {
                PhysicsObject duck = this.duck;
                if (this.duck.ragdoll != null)
                {
                    this.Degrapple();
                }
                else
                {
                    Vec2 position = duck.position;
                    duck.position = this._rope.attach2.position + normalized * this._rope.properLength;
                    Vec2 vec2_2 = duck.position - duck.lastPosition;
                    if (!this.changeSpeed)
                        return;
                    duck.hSpeed = vec2_2.x;
                    duck.vSpeed = vec2_2.y;
                }
            }
            else
            {
                this.position = this._rope.attach2.position + normalized * this._rope.properLength;
                Vec2 vec2_3 = this.position - this.lastPosition;
                this.hSpeed = vec2_3.x;
                this.vSpeed = vec2_3.y;
            }
        }

        public override void Draw()
        {
            if (!(Level.current is Editor))
                return;
            this.graphic.center = new Vec2(8f, 8f);
            this.graphic.depth = this.depth;
            for (int index = 0; index < (int)this.length; ++index)
                Graphics.Draw(this.graphic, this.x, this.y + index * 16);
        }
    }
}
