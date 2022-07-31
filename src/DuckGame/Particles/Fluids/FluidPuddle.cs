// Decompiled with JetBrains decompiler
// Type: DuckGame.FluidPuddle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class FluidPuddle : MaterialThing
    {
        private WhiteRectangle _lightRect;
        public FluidData data;
        public float _wide;
        public float fluidWave;
        private FluidStream _leftStream;
        private FluidStream _rightStream;
        private BlockCorner _leftCorner;
        private BlockCorner _rightCorner;
        private BlockCorner _topLeftCorner;
        private BlockCorner _topRightCorner;
        private bool _initializedUpperCorners;
        private List<SpriteMap> _surfaceFire = new List<SpriteMap>();
        private Block _block;
        private SpriteMap _lava;
        private SpriteMap _lavaAlternate;
        private int _framesSinceFeed;
        private float _fireID;
        private List<PhysicsObject> _coll;
        private float _fireRise;
        private int bubbleWait;

        public float fireID => this._fireID;

        public FluidPuddle(float xpos, float ypos, Block b)
          : base(xpos, ypos)
        {
            this._collisionOffset.y = -4f;
            this._collisionSize.y = 1f;
            this._block = b;
            this.depth = (Depth)0.3f;
            this.flammable = 0.9f;
            this.alpha = 0.0f;
            List<BlockCorner> groupCorners = b.GetGroupCorners();
            this._coll = new List<PhysicsObject>();
            this._leftCorner = null;
            this._rightCorner = null;
            foreach (BlockCorner blockCorner in groupCorners)
            {
                if ((double)Math.Abs(ypos - blockCorner.corner.y) < 4.0)
                {
                    if (blockCorner.corner.x > (double)xpos)
                    {
                        if (this._rightCorner == null)
                            this._rightCorner = blockCorner;
                        else if (blockCorner.corner.x < (double)this._rightCorner.corner.x)
                            this._rightCorner = blockCorner;
                    }
                    else if (blockCorner.corner.x < (double)xpos)
                    {
                        if (this._leftCorner == null)
                            this._leftCorner = blockCorner;
                        else if (blockCorner.corner.x > (double)this._leftCorner.corner.x)
                            this._leftCorner = blockCorner;
                    }
                }
            }
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (!this._onFire && data.flammable > 0.5)
            {
                this._fireID = FireManager.GetFireID();
                SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                this._onFire = true;
                this.alpha = 1f;
            }
            return true;
        }

        public override void AddFire()
        {
            SpriteMap spriteMap = new SpriteMap("surfaceFire", 16, 10);
            spriteMap.AddAnimation("idle", 0.2f, true, 0, 1, 2, 3);
            spriteMap.SetAnimation("idle");
            spriteMap.center = new Vec2(8f, 10f);
            spriteMap.frame = Rando.Int(3);
            this._surfaceFire.Add(spriteMap);
        }

        public override void Initialize()
        {
            if (this._leftCorner == null || this._rightCorner == null)
                Level.Remove(this);
            else
                this.y = this._leftCorner.corner.y;
        }

        public void Feed(FluidData dat)
        {
            if (this._lava == null && dat.sprite != "" && dat.sprite != null)
            {
                if (this.data.sprite == null)
                    this.data.sprite = dat.sprite;
                this._lava = new SpriteMap(dat.sprite, 16, 16);
                this._lava.AddAnimation("idle", 0.1f, true, 0, 1, 2, 3);
                this._lava.SetAnimation("idle");
                this._lava.center = new Vec2(8f, 10f);
                this._lavaAlternate = new SpriteMap(dat.sprite, 16, 16);
                this._lavaAlternate.AddAnimation("idle", 0.1f, true, 2, 3, 0, 1);
                this._lavaAlternate.SetAnimation("idle");
                this._lavaAlternate.center = new Vec2(8f, 10f);
            }
            if (this._lightRect == null && Layer.lighting)
            {
                this._lightRect = new WhiteRectangle(this.x, this.y, this.width, this.height, dat.heat <= 0.0);
                Level.Add(_lightRect);
            }
            if (dat.amount > 0.0)
                this._framesSinceFeed = 0;
            this.data.Mix(dat);
            this.data.amount = Maths.Clamp(this.data.amount, 0.0f, this.MaxFluidFill());
            this._wide = this.FeedAmountToDistance(this.data.amount);
            float num1 = this._wide + 4f;
            this._collisionOffset.x = (float)-((double)num1 / 2.0);
            this._collisionSize.x = num1;
            this.FeedEdges();
            if (this._leftCorner != null && this._rightCorner != null && _wide > _rightCorner.corner.x - (double)this._leftCorner.corner.x)
            {
                this._wide = this._rightCorner.corner.x - this._leftCorner.corner.x;
                this.x = this._leftCorner.corner.x + (float)((_rightCorner.corner.x - (double)this._leftCorner.corner.x) / 2.0);
            }
            float num2 = this._wide + 4f;
            this._collisionOffset.x = (float)-((double)num2 / 2.0);
            this._collisionSize.x = num2;
            if (!(this.data.sprite == "water") || this._leftCorner == null)
                return;
            Block block = this._leftCorner.block;
            while (true)
            {
                switch (block)
                {
                    case null:
                        goto label_19;
                    case SnowTileset _:
                        if ((double)block.left + 2.0 > (double)this.left && (double)block.right - 2.0 < (double)this.right)
                        {
                            (block as SnowTileset).Freeze();
                            break;
                        }
                        break;
                    case SnowIceTileset _:
                        if ((double)block.left + 2.0 > (double)this.left && (double)block.right - 2.0 < (double)this.right)
                        {
                            (block as SnowIceTileset).Freeze();
                            break;
                        }
                        break;
                }
                block = block.rightBlock;
            }
        label_19:;
        }

        public float DistanceToFeedAmount(float distance) => distance / 600f;

        public float FeedAmountToDistance(float feed) => feed * 600f;

        public float MaxFluidFill()
        {
            if (this._topLeftCorner == null || this._topRightCorner == null)
                return 999999f;
            float num = this._topLeftCorner.corner.y + 8f;
            if (_topRightCorner.corner.y > num)
                num = this._topRightCorner.corner.y + 8f;
            return this.DistanceToFeedAmount((this._leftCorner.corner.y - num) * this._collisionSize.x);
        }

        public void FeedEdges()
        {
            if (this._rightCorner != null && (double)this.right > _rightCorner.corner.x && this._rightCorner.wallCorner)
                this.x -= this.right - this._rightCorner.corner.x;
            if (this._leftCorner != null && (double)this.left < _leftCorner.corner.x && this._leftCorner.wallCorner)
                this.x += this._leftCorner.corner.x - this.left;
            if (this._rightCorner != null && (double)this.right > _rightCorner.corner.x && !this._rightCorner.wallCorner)
            {
                float feedAmount = this.DistanceToFeedAmount(this.right - this._rightCorner.corner.x);
                this.x -= ((this.right - _rightCorner.corner.x) / 2f);
                if (this._rightStream == null)
                    this._rightStream = new FluidStream(this._rightCorner.corner.x - 2f, this.y, new Vec2(1f, 0f), 1f);
                this._rightStream.position.y = this.y - this._collisionOffset.y;
                this._rightStream.position.x = this._rightCorner.corner.x + 2f;
                this._rightStream.Feed(this.data.Take(feedAmount));
            }
            this._wide = this.FeedAmountToDistance(this.data.amount);
            float num1 = this._wide + 4f;
            this._collisionOffset.x = -(num1 / 2f);
            this._collisionSize.x = num1;
            if (this._leftCorner != null && (double)this.left < _leftCorner.corner.x && !this._leftCorner.wallCorner)
            {
                float feedAmount = this.DistanceToFeedAmount(this._leftCorner.corner.x - this.left);
                this.x += ((_leftCorner.corner.x - this.left) / 2f);
                if (this._leftStream == null)
                    this._leftStream = new FluidStream(this._leftCorner.corner.x - 2f, this.y, new Vec2(-1f, 0.0f), 1f);
                this._leftStream.position.y = this.y - this._collisionOffset.y;
                this._leftStream.position.x = this._leftCorner.corner.x - 2f;
                this._leftStream.Feed(this.data.Take(feedAmount));
            }
            this._wide = this.FeedAmountToDistance(this.data.amount);
            float num2 = this._wide + 4f;
            this._collisionOffset.x = (float)-((double)num2 / 2f);
            this._collisionSize.x = num2;
        }

        public float CalculateDepth()
        {
            double distance = this.FeedAmountToDistance(this.data.amount);
            if (_wide == 0f)
                this._wide = 1f / 1000f;
            double wide = _wide;
            return Maths.Clamp((float)(distance / wide), 1f, 99999f);
        }

        public void PrepareFloaters()
        {
            if (collisionSize.y <= 10f)
                return;
            foreach (PhysicsObject physicsObject in Level.CheckLineAll<PhysicsObject>(this.topLeft + new Vec2(0.0f, -8f), this.topRight + new Vec2(0.0f, -8f)))
            {
                physicsObject.position.y = this.top;
                physicsObject.DoFloat();
            }
        }

        public override void Update()
        {
            ++this._framesSinceFeed;
            this.fluidWave += 0.1f;
            if (data.amount < 0.0001f)
                Level.Remove(this);
            if (collisionSize.y > 10f)
            {
                ++this.bubbleWait;
                if (this.bubbleWait > Rando.Int(15, 25))
                {
                    for (int index = 0; index < (int)Math.Floor(collisionSize.x / 16f); ++index)
                    {
                        if (Rando.Float(1f) > 0.85f)
                            Level.Add(new TinyBubble(this.left + index * 16 + Rando.Float(-4f, 4f), this.bottom + Rando.Float(-4f), 0f, this.top + 10f));
                    }
                    this.bubbleWait = 0;
                }
                this._coll.Clear();
                Level.CheckRectAll<PhysicsObject>(this.topLeft, this.bottomRight, this._coll);
                foreach (PhysicsObject physicsObject in this._coll)
                    physicsObject.sleeping = false;
            }
            FluidPuddle fluidPuddle = Level.CheckLine<FluidPuddle>(new Vec2(this.left, this.y), new Vec2(this.right, this.y), this);
            if (fluidPuddle != null && fluidPuddle.data.amount < this.data.amount)
            {
                fluidPuddle.active = false;
                float num1 = Math.Min(fluidPuddle.left, this.left);
                float num2 = Math.Max(fluidPuddle.right, this.right);
                this.x = num1 + ((num2 - num1) / 2f);
                this.Feed(fluidPuddle.data);
                Level.Remove(fluidPuddle);
            }
            if (this._leftStream != null)
            {
                this._leftStream.Update();
                this._leftStream.onFire = this.onFire;
            }
            if (this._rightStream != null)
            {
                this._rightStream.Update();
                this._rightStream.onFire = this.onFire;
            }
            double distance = this.FeedAmountToDistance(this.data.amount);
            if (_wide == 0f)
                this._wide = 1f / 1000f;
            double wide = _wide;
            float num = Maths.Clamp((float)(distance / wide), 1f, 99999f);
            if (this.onFire)
            {
                this._fireRise = Lerp.FloatSmooth(this._fireRise, 1f, 0.1f, 1.2f);
                if (this._framesSinceFeed > 10)
                {
                    // this.Feed(this.data with { amount = -1f / 1000f });
                    FluidData dat = this.data;
                    dat.amount = -0.001f;
                    this.Feed(dat);
                    if (this.data.amount <= 0f)
                    {
                        this.data.amount = 0f;
                        base.alpha = Lerp.Float(base.alpha, 0f, 0.04f);
                    }
                    else
                    {
                        base.alpha = Lerp.Float(base.alpha, 1f, 0.04f);
                    }
                    if (base.alpha <= 0f)
                    {
                        Level.Remove(this);
                    }
                }
            }
            else
            {
                this.alpha = Lerp.Float(this.alpha, 1f, 0.04f);
                if ((double)num < 3.0)
                {
                    FluidData dat2 = this.data;
                    dat2.amount = -0.0001f;
                    this.Feed(dat2);
                }
                //  this.Feed(this.data with { amount = -0.0001f });


            }
            float depth = this.CalculateDepth();
            if ((double)depth > 4.0 && !this._initializedUpperCorners)
            {
                this._initializedUpperCorners = true;
                foreach (BlockCorner groupCorner in this._block.GetGroupCorners())
                {
                    if (this._leftCorner != null && groupCorner.corner.x == (double)this._leftCorner.corner.x && groupCorner.corner.y < (double)this._leftCorner.corner.y)
                    {
                        if (this._topLeftCorner == null)
                            this._topLeftCorner = groupCorner;
                        else if (groupCorner.corner.y > (double)this._topLeftCorner.corner.y)
                            this._topLeftCorner = groupCorner;
                    }
                    else if (this._rightCorner != null && groupCorner.corner.x == (double)this._rightCorner.corner.x && groupCorner.corner.y < (double)this._rightCorner.corner.y)
                    {
                        if (this._topRightCorner == null)
                            this._topRightCorner = groupCorner;
                        else if (groupCorner.corner.y > (double)this._topRightCorner.corner.y)
                            this._topRightCorner = groupCorner;
                    }
                }
            }
            if (this._leftStream != null)
                this._leftStream.position.y = this.y - this._collisionOffset.y;
            if (this._rightStream != null)
                this._rightStream.position.y = this.y - this._collisionOffset.y;
            this._collisionOffset.y = (float)(-(double)depth - 1.0);
            this._collisionSize.y = depth;
        }

        public override void Draw()
        {
            Graphics.DrawLine(this.position + new Vec2(-this._collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), this.position + new Vec2(this._collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), new Color(this.data.color) * this.data.transparent, this._collisionSize.y, (Depth)0.9f);
            Graphics.DrawLine(this.position + new Vec2(-this._collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), this.position + new Vec2(this._collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), new Color(this.data.color), this._collisionSize.y, -0.99f);
            if (this._lightRect != null)
            {
                this._lightRect.position = this.topLeft;
                this._lightRect.size = new Vec2(this.width, this.height);
            }
            int num1 = (int)Math.Ceiling(_collisionSize.x / 16f);
            float num2 = this._collisionSize.x / num1;
            if (this._onFire)
            {
                while (this._surfaceFire.Count < num1)
                    this.AddFire();
                float num3 = 0f;
                if (_collisionSize.y > 2f)
                    num3 = 2f;
                for (int index = 0; index < num1; ++index)
                {
                    this._surfaceFire[index].alpha = this.alpha;
                    this._surfaceFire[index].yscale = this._fireRise;
                    this._surfaceFire[index].depth = this.depth + 1;
                    Graphics.Draw(this._surfaceFire[index], (this.left + 8f + index * num2), (this.y + _collisionOffset.y + 1f) - num3);
                }
            }
            if (this._lava != null && collisionSize.y > 2f)
            {
                bool flag = false;
                for (int index = 0; index < num1; ++index)
                {
                    SpriteMap g = this._lava;
                    if (flag)
                        g = this._lavaAlternate;
                    g.depth = -0.7f;
                    SpriteMap spriteMap = g;
                    spriteMap.depth += index;
                    g.alpha = 1f;
                    Graphics.DrawWithoutUpdate(g, (float)Math.Round(this.left + 8f + index * num2), (this.y + _collisionOffset.y - 4.5f));
                    flag = !flag;
                }
                this._lava.UpdateFrame();
                this._lavaAlternate.UpdateFrame();
            }
            base.Draw();
        }

        public override void Terminate()
        {
            if (this._lightRect != null)
                Level.Remove(_lightRect);
            base.Terminate();
        }
    }
}
