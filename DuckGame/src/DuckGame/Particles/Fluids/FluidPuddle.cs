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

        public float fireID => _fireID;

        public FluidPuddle(float xpos, float ypos, Block b)
          : base(xpos, ypos)
        {
            _collisionOffset.y = -4f;
            _collisionSize.y = 1f;
            _block = b;
            depth = (Depth)0.3f;
            flammable = 0.9f;
            alpha = 0f;
            List<BlockCorner> groupCorners = b.GetGroupCorners();
            _coll = new List<PhysicsObject>();
            _leftCorner = null;
            _rightCorner = null;
            foreach (BlockCorner blockCorner in groupCorners)
            {
                if (Math.Abs(ypos - blockCorner.corner.y) < 4)
                {
                    if (blockCorner.corner.x > xpos)
                    {
                        if (_rightCorner == null)
                            _rightCorner = blockCorner;
                        else if (blockCorner.corner.x < _rightCorner.corner.x)
                            _rightCorner = blockCorner;
                    }
                    else if (blockCorner.corner.x < xpos)
                    {
                        if (_leftCorner == null)
                            _leftCorner = blockCorner;
                        else if (blockCorner.corner.x > _leftCorner.corner.x)
                            _leftCorner = blockCorner;
                    }
                }
            }
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (!_onFire && data.flammable > 0.5)
            {
                _fireID = FireManager.GetFireID();
                SFX.Play("ignite", pitch: (Rando.Float(0.3f) - 0.3f));
                _onFire = true;
                alpha = 1f;
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
            _surfaceFire.Add(spriteMap);
        }

        public override void Initialize()
        {
            if (_leftCorner == null || _rightCorner == null) Level.Remove(this);
            else y = _leftCorner.corner.y;
        }

        public void Feed(FluidData dat)
        {
            if (_lava == null && dat.sprite != "" && dat.sprite != null)
            {
                if (data.sprite == null)
                    data.sprite = dat.sprite;
                _lava = new SpriteMap(dat.sprite, 16, 16);
                _lava.AddAnimation("idle", 0.1f, true, 0, 1, 2, 3);
                _lava.SetAnimation("idle");
                _lava.center = new Vec2(8f, 10f);
                _lavaAlternate = new SpriteMap(dat.sprite, 16, 16);
                _lavaAlternate.AddAnimation("idle", 0.1f, true, 2, 3, 0, 1);
                _lavaAlternate.SetAnimation("idle");
                _lavaAlternate.center = new Vec2(8f, 10f);
            }
            if (_lightRect == null && Layer.lighting)
            {
                _lightRect = new WhiteRectangle(x, y, width, height, dat.heat <= 0);
                Level.Add(_lightRect);
            }
            if (dat.amount > 0)
                _framesSinceFeed = 0;
            data.Mix(dat);
            data.amount = Maths.Clamp(data.amount, 0f, MaxFluidFill());
            _wide = FeedAmountToDistance(data.amount);
            float num1 = _wide + 4f;
            _collisionOffset.x = (float)-(num1 / 2);
            _collisionSize.x = num1;
            FeedEdges();
            if (_leftCorner != null && _rightCorner != null && _wide > _rightCorner.corner.x - _leftCorner.corner.x)
            {
                _wide = _rightCorner.corner.x - _leftCorner.corner.x;
                x = _leftCorner.corner.x + (float)((_rightCorner.corner.x - _leftCorner.corner.x) / 2);
            }
            float num2 = _wide + 4f;
            _collisionOffset.x = (float)-(num2 / 2);
            _collisionSize.x = num2;
            if (!(data.sprite == "water") || _leftCorner == null)
                return;
            Block block = _leftCorner.block;
            while (true)
            {
                switch (block)
                {
                    case null:
                        goto label_19;
                    case SnowTileset _:
                        if (block.left + 2 > left && block.right - 2 < right)
                        {
                            (block as SnowTileset).Freeze();
                            break;
                        }
                        break;
                    case SnowIceTileset _:
                        if (block.left + 2 > left && block.right - 2 < right)
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
            if (_topLeftCorner == null || _topRightCorner == null)
                return 999999f;
            float num = _topLeftCorner.corner.y + 8f;
            if (_topRightCorner.corner.y > num)
                num = _topRightCorner.corner.y + 8f;
            return DistanceToFeedAmount((_leftCorner.corner.y - num) * _collisionSize.x);
        }

        public void FeedEdges()
        {
            if (_rightCorner != null && right > _rightCorner.corner.x && _rightCorner.wallCorner)
                x -= right - _rightCorner.corner.x;
            if (_leftCorner != null && left < _leftCorner.corner.x && _leftCorner.wallCorner)
                x += _leftCorner.corner.x - left;
            if (_rightCorner != null && right > _rightCorner.corner.x && !_rightCorner.wallCorner)
            {
                float feedAmount = DistanceToFeedAmount(right - _rightCorner.corner.x);
                x -= ((right - _rightCorner.corner.x) / 2f);
                if (_rightStream == null)
                    _rightStream = new FluidStream(_rightCorner.corner.x - 2f, y, new Vec2(1f, 0f), 1f);
                _rightStream.position.y = y - _collisionOffset.y;
                _rightStream.position.x = _rightCorner.corner.x + 2f;
                _rightStream.Feed(data.Take(feedAmount));
            }
            _wide = FeedAmountToDistance(data.amount);
            float num1 = _wide + 4f;
            _collisionOffset.x = -(num1 / 2f);
            _collisionSize.x = num1;
            if (_leftCorner != null && left < _leftCorner.corner.x && !_leftCorner.wallCorner)
            {
                float feedAmount = DistanceToFeedAmount(_leftCorner.corner.x - left);
                x += ((_leftCorner.corner.x - left) / 2f);
                if (_leftStream == null)
                    _leftStream = new FluidStream(_leftCorner.corner.x - 2f, y, new Vec2(-1f, 0f), 1f);
                _leftStream.position.y = y - _collisionOffset.y;
                _leftStream.position.x = _leftCorner.corner.x - 2f;
                _leftStream.Feed(data.Take(feedAmount));
            }
            _wide = FeedAmountToDistance(data.amount);
            float num2 = _wide + 4f;
            _collisionOffset.x = -(num2 / 2f);
            _collisionSize.x = num2;
        }

        public float CalculateDepth()
        {
            double distance = FeedAmountToDistance(data.amount);
            if (_wide == 0f)
                _wide = 1f / 1000f;
            double wide = _wide;
            return Maths.Clamp((float)(distance / wide), 1f, 99999f);
        }

        public void PrepareFloaters()
        {
            if (collisionSize.y <= 10f)
                return;
            foreach (PhysicsObject physicsObject in Level.CheckLineAll<PhysicsObject>(topLeft + new Vec2(0f, -8f), topRight + new Vec2(0f, -8f)))
            {
                physicsObject.position.y = top;
                physicsObject.DoFloat();
            }
        }
        public float timer;
        public MaterialLavaWobble mt;
        public override void Update()
        {
            //1 per frame if 1000 wide
            if (DGRSettings.AmbientParticles)
            {
                if (mt == null)
                {
                    mt = new MaterialLavaWobble(this);
                }
                if (data.heat > 0)
                {
                    timer += 0.001f * collisionSize.x * DGRSettings.ActualParticleMultiplier * data.heat;
                    while (timer >= 1)
                    {
                        Level.Add(new Ember(Rando.Float(left, right), top));
                        timer--;
                    }
                }
                else if (onFire)
                {
                    timer += 0.0005f * collisionSize.x * DGRSettings.ActualParticleMultiplier;
                    while (timer >= 1)
                    {
                        Level.Add(new Ember(Rando.Float(left, right), top));
                        timer--;
                    }
                }
            }
            ++_framesSinceFeed;
            fluidWave += 0.1f;
            if (data.amount < 0.0001f)
                Level.Remove(this);
            if (collisionSize.y > 10f)
            {
                if (DGRSettings.S_ParticleMultiplier != 0)
                {
                    ++bubbleWait;
                    if (bubbleWait > Rando.Int(15, 25) / DGRSettings.ActualParticleMultiplier)
                    {
                        for (int index = 0; index < (int)Math.Floor(collisionSize.x / 16f); ++index)
                        {
                            if (Rando.Float(1f) > 0.85f)
                                Level.Add(new TinyBubble(left + index * 16 + Rando.Float(-4f, 4f), bottom + Rando.Float(-4f), 0f, top + 10f));
                        }
                        bubbleWait = 0;
                    }
                }
                _coll.Clear();
                Level.CheckRectAll(topLeft, bottomRight, _coll);
                foreach (PhysicsObject physicsObject in _coll)
                {
                    physicsObject.sleeping = false;
                }
            }
            FluidPuddle fluidPuddle = Level.CheckLine<FluidPuddle>(new Vec2(left, y), new Vec2(right, y), this);
            if (fluidPuddle != null && fluidPuddle.data.amount < data.amount)
            {
                fluidPuddle.active = false;
                float num1 = Math.Min(fluidPuddle.left, left);
                float num2 = Math.Max(fluidPuddle.right, right);
                x = num1 + ((num2 - num1) / 2f);
                Feed(fluidPuddle.data);
                Level.Remove(fluidPuddle);
            }
            if (_leftStream != null)
            {
                _leftStream.Update();
                _leftStream.onFire = onFire;
            }
            if (_rightStream != null)
            {
                _rightStream.Update();
                _rightStream.onFire = onFire;
            }
            double distance = FeedAmountToDistance(data.amount);
            if (_wide == 0f)
                _wide = 1f / 1000f;
            double wide = _wide;
            float num = Maths.Clamp((float)(distance / wide), 1f, 99999f);
            if (onFire)
            {
                _fireRise = Lerp.FloatSmooth(_fireRise, 1f, 0.1f, 1.2f);
                if (_framesSinceFeed > 10)
                {
                    // this.Feed(this.data with { amount = -1f / 1000f });
                    FluidData dat = data;
                    dat.amount = -0.001f;
                    Feed(dat);
                    if (data.amount <= 0f)
                    {
                        data.amount = 0f;
                        alpha = Lerp.Float(alpha, 0f, 0.04f);
                    }
                    else
                    {
                        alpha = Lerp.Float(alpha, 1f, 0.04f);
                    }
                    if (alpha <= 0f)
                    {
                        Level.Remove(this);
                    }
                }
            }
            else
            {
                alpha = Lerp.Float(alpha, 1f, 0.04f);
                if (num < 3)
                {
                    FluidData dat2 = data;
                    dat2.amount = -0.0001f;
                    Feed(dat2);
                }
                //  this.Feed(this.data with { amount = -0.0001f });


            }
            float depth = CalculateDepth();
            if (depth > 4 && !_initializedUpperCorners)
            {
                _initializedUpperCorners = true;
                foreach (BlockCorner groupCorner in _block.GetGroupCorners())
                {
                    if (_leftCorner != null && groupCorner.corner.x == _leftCorner.corner.x && groupCorner.corner.y < _leftCorner.corner.y)
                    {
                        if (_topLeftCorner == null)
                            _topLeftCorner = groupCorner;
                        else if (groupCorner.corner.y > _topLeftCorner.corner.y)
                            _topLeftCorner = groupCorner;
                    }
                    else if (_rightCorner != null && groupCorner.corner.x == _rightCorner.corner.x && groupCorner.corner.y < _rightCorner.corner.y)
                    {
                        if (_topRightCorner == null)
                            _topRightCorner = groupCorner;
                        else if (groupCorner.corner.y > _topRightCorner.corner.y)
                            _topRightCorner = groupCorner;
                    }
                }
            }
            if (_leftStream != null)
                _leftStream.position.y = y - _collisionOffset.y;
            if (_rightStream != null)
                _rightStream.position.y = y - _collisionOffset.y;
            _collisionOffset.y = (float)(-depth - 1);
            _collisionSize.y = depth;
        }

        public override void Draw()
        {
            Graphics.DrawLine(position + new Vec2(-_collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), position + new Vec2(_collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), new Color(data.color) * data.transparent, _collisionSize.y, (Depth)0.9f);
            Graphics.DrawLine(position + new Vec2(-_collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), position + new Vec2(_collisionOffset.x, (collisionOffset.y / 2f + 0.5f)), new Color(data.color), _collisionSize.y, -0.99f);
            if (_lightRect != null)
            {
                _lightRect.position = topLeft;
                _lightRect.size = new Vec2(width, height);
            }
            int num1 = (int)Math.Ceiling(_collisionSize.x / 16f);
            float num2 = _collisionSize.x / num1;
            if (_onFire)
            {
                while (_surfaceFire.Count < num1)
                    AddFire();
                float num3 = 0f;
                if (_collisionSize.y > 2f)
                    num3 = 2f;
                for (int index = 0; index < num1; ++index)
                {
                    _surfaceFire[index].alpha = alpha;
                    _surfaceFire[index].yscale = _fireRise;
                    _surfaceFire[index].depth = depth + 1;
                    Graphics.Draw(_surfaceFire[index], (left + 8f + index * num2), (y + _collisionOffset.y + 1f) - num3);
                }
            }
            if (_lava != null && collisionSize.y > 2f)
            {
                bool flag = false;
                for (int index = 0; index < num1; ++index)
                {
                    SpriteMap g = _lava;
                    if (flag)
                        g = _lavaAlternate;
                    g.depth = -0.7f;
                    SpriteMap spriteMap = g;
                    spriteMap.depth += index;
                    g.alpha = 1f;
                    Graphics.DrawWithoutUpdate(g, (float)Math.Round(left + 8f + index * num2), (y + _collisionOffset.y - 4.5f));
                    flag = !flag;
                }
                _lava.UpdateFrame();
                _lavaAlternate.UpdateFrame();
            }
            base.Draw();
        }

        public override void Terminate()
        {
            if (_lightRect != null)
                Level.Remove(_lightRect);
            base.Terminate();
        }
    }
}
