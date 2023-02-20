// Decompiled with JetBrains decompiler
// Type: DuckGame.Vine
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
                Rope rope = _rope;
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
                float num1 = 0f;
                foreach (VineSection vineSection in points)
                    num1 += vineSection.length;
                int num2 = 0;
                foreach (VineSection vineSection in points)
                {
                    vineSection.lowestSection = num2 + (int)Math.Round(vineSection.length / num1 * sectionIndex);
                    num2 = vineSection.lowestSection;
                }
                return points;
            }
        }

        public Vine(float xpos, float ypos, float init)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("vine", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            _vinePartSprite = new Sprite("vine")
            {
                center = new Vec2(8f, 0f)
            };
            collisionOffset = new Vec2(-5f, -4f);
            collisionSize = new Vec2(11f, 7f);
            weight = 0.1f;
            thickness = 0.1f;
            canPickUp = false;
            initLength = init;
            depth = -0.5f;
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
            _harpoon = new Harpoon(this);
            Level.Add(_harpoon);
            if (Level.current is Editor)
                return;
            Vec2 position = this.position;
            this.position.y += (int)length * 16 - 8;
            _harpoon.noisy = false;
            _harpoon.Fire(position + new Vec2(0f, -8f), new Vec2(0f, -1f));
            _rope = new Rope(x, y, null, _harpoon, duck, true, _vinePartSprite);
            if (initLength != 0.0)
                _rope.properLength = initLength;
            Level.Add(_rope);
        }

        public override void Terminate()
        {
            if (_rope == null)
                return;
            _rope.RemoveRope();
            Level.Remove(_harpoon);
            Level.Remove(_rope);
        }

        public Rope GetRopeParent(Thing child)
        {
            for (Rope ropeParent = _rope; ropeParent != null; ropeParent = ropeParent.attach2 as Rope)
            {
                if (ropeParent.attach2 == child)
                    return ropeParent;
            }
            return null;
        }

        public void Degrapple()
        {
            if (nextVine != null && nextVine._rope != null)
            {
                nextVine._rope.attach2 = _rope.attach2;
                nextVine._rope.properLength = (nextVine._rope.attach1Point - _rope.attach2Point).length;
                nextVine.prevVine = null;
                nextVine = null;
            }
            if (prevVine != null)
                prevVine.nextVine = null;
            _harpoon.Return();
            _harpoon.visible = false;
            if (_rope != null)
            {
                _rope.RemoveRope();
                _rope.visible = false;
                visible = false;
            }
            _rope = null;
            if (duck != null)
            {
                duck.frictionMult = 1f;
                duck.gravMultiplier = 1f;
            }
            owner = null;
            frictionMult = 1f;
            gravMultiplier = 1f;
            visible = false;
            Level.Remove(_harpoon);
            Level.Remove(this);
            Update();
        }

        public void UpdateRopeStuff()
        {
            _rope.Update();
            Update();
        }

        //public void MoveDuck()
        //{
        //    Vec2 vec2_1 = this._rope.attach1.position - this._rope.attach2.position;
        //    if (vec2_1.length <= this._rope.properLength)
        //        return;
        //    vec2_1 = vec2_1.normalized;
        //    if (this.duck == null)
        //        return;
        //    PhysicsObject duck = this.duck;
        //    Vec2 position = duck.position;
        //    duck.position = this._rope.attach2.position + vec2_1 * this._rope.properLength;
        //    Vec2 vec2_2 = duck.position - duck.lastPosition;
        //}

        public Vec2 wallPoint => _wallPoint;

        public Vec2 grappelTravel => _grappleTravel;

        public override void Update()
        {
            base.Update();
            if (owner != null)
                offDir = owner.offDir;
            if (duck != null && (duck.ragdoll != null || duck._trapped != null || duck.dead))
            {
                owner = null;
                _rope.visible = false;
            }
            if (_rope == null)
                return;
            if (owner != null)
            {
                _rope.position = owner.position;
            }
            else
            {
                _rope.position = position;
                if (prevOwner != null)
                {
                    PhysicsObject prevOwner = this.prevOwner as PhysicsObject;
                    prevOwner.frictionMult = 1f;
                    prevOwner.gravMultiplier = 1f;
                    _prevOwner = null;
                    frictionMult = 1f;
                    gravMultiplier = 1f;
                    Level.Remove(this);
                }
            }
            if (!_harpoon.stuck)
                return;
            if (duck != null)
            {
                if (!duck.grounded)
                {
                    duck.frictionMult = 0f;
                }
                else
                {
                    duck.frictionMult = 1f;
                    duck.gravMultiplier = 1f;
                }
            }
            else if (!grounded)
            {
                frictionMult = 0f;
            }
            else
            {
                frictionMult = 1f;
                gravMultiplier = 1f;
            }
            Vec2 vec2_1 = _rope.attach1.position - _rope.attach2.position;
            if (_rope.properLength < 0.0)
                _rope.properLength = vec2_1.length;
            if (vec2_1.length <= _rope.properLength)
                return;
            Vec2 normalized = vec2_1.normalized;
            if (duck != null)
            {
                PhysicsObject duck = this.duck;
                if (this.duck.ragdoll != null)
                {
                    Degrapple();
                }
                else
                {
                    Vec2 position = duck.position;
                    duck.position = _rope.attach2.position + normalized * _rope.properLength;
                    Vec2 vec2_2 = duck.position - duck.lastPosition;
                    if (!changeSpeed)
                        return;
                    duck.hSpeed = vec2_2.x;
                    duck.vSpeed = vec2_2.y;
                }
            }
            else
            {
                position = _rope.attach2.position + normalized * _rope.properLength;
                Vec2 vec2_3 = position - lastPosition;
                hSpeed = vec2_3.x;
                vSpeed = vec2_3.y;
            }
        }

        public override void Draw()
        {
            if (!(Level.current is Editor))
                return;
            graphic.center = new Vec2(8f, 8f);
            graphic.depth = depth;
            for (int index = 0; index < (int)length; ++index)
                Graphics.Draw(graphic, x, y + index * 16);
        }
    }
}
