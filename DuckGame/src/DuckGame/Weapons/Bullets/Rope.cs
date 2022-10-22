// Decompiled with JetBrains decompiler
// Type: DuckGame.Rope
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Rope : Thing
    {
        private Thing _attach1;
        private Thing _attach2;
        public Vec2 offsetDir = Vec2.Zero;
        public float linkDirectionOnSplit;
        public float offsetDegrees;
        private float _properLength = -1f;
        private BlockCorner _corner;
        private Vec2 _pos1;
        private Vec2 _pos2;
        private bool _terminated;
        public Thing _thing;
        private Sprite _vine;
        private bool _isVine;
        public Thing _belongsTo;
        private Vec2 dirLine;
        private Vec2 tpos = Vec2.Zero;
        private bool pulled;
        public Vec2 linkVector;
        public Vec2 cornerVector;
        public Vec2 breakVector;
        public float startLength;
        private Rope pull;
        public bool serverForObject;

        public Thing attach1
        {
            get => _attach1;
            set => _attach1 = value;
        }

        public Thing attach2
        {
            get => _attach2;
            set => _attach2 = value;
        }

        public override NetworkConnection connection => _belongsTo != null ? _belongsTo.connection : base.connection;

        public override NetIndex8 authority => _belongsTo != null ? _belongsTo.authority : base.authority;

        public float linkDirection => this.attach2 is Rope attach2 ? Maths.PointDirection(new Vec2(0f, 0f), (attach2Point - attach1Point).Rotate(Maths.DegToRad(attach2.offsetDegrees), Vec2.Zero)) : 0f;

        public float linkDirectionNormalized => Maths.PointDirection(new Vec2(0f, 0f), attach2Point - attach1Point);

        public float properLength
        {
            get => _properLength;
            set => _properLength = value;
        }

        public Vec2 attach1Point
        {
            get
            {
                if (!(_attach1 is Rope attach1) || attach1._corner == null)
                    return _attach1.position;
                Vec2 vec2 = attach1._corner.corner - attach1._corner.block.position;
                vec2.Normalize();
                return _attach1.position + vec2 * 4f;
            }
        }

        public Vec2 attach2Point
        {
            get
            {
                if (!(_attach2 is Rope attach2) || attach2._corner == null)
                    return _attach2.position;
                Vec2 vec2 = attach2._corner.corner - attach2._corner.block.position;
                vec2.Normalize();
                return _attach2.position + vec2 * 4f;
            }
        }

        public float length => _attach1 != null && _attach2 != null ? (_attach1.position - _attach2.position).length : 0f;

        public Rope(
          float xpos,
          float ypos,
          Thing attach1Val,
          Thing attach2Val,
          Thing thing = null,
          bool vine = false,
          Sprite tex = null,
          Thing belongsTo = null)
          : base(xpos, ypos)
        {
            _belongsTo = belongsTo;
            if (attach1Val == null)
                attach1Val = this;
            if (attach2Val == null)
                attach2Val = this;
            _attach1 = attach1Val;
            _attach2 = attach2Val;
            _pos1 = attach1Val.position;
            _pos2 = attach2Val.position;
            _thing = thing;
            if (vine)
            {
                _vine = new Sprite(nameof(vine))
                {
                    center = new Vec2(8f, 0f)
                };
            }
            if (tex != null)
                _vine = tex;
            _isVine = vine;
            depth = -0.5f;
        }

        public void RemoveRope()
        {
            _terminated = true;
            visible = false;
            Level.Remove(this);
            if (_attach1 is Rope attach1 && !attach1._terminated)
                attach1.RemoveRope();
            if (!(_attach2 is Rope attach2) || attach2._terminated)
                return;
            attach2.RemoveRope();
        }

        public void TerminateLaterRopes()
        {
            if (_attach2 is Rope attach2 && !attach2._terminated)
                attach2.TerminateLaterRopesRecurse();
            _attach2 = null;
        }

        public void TerminateLaterRopesRecurse()
        {
            _terminated = true;
            visible = false;
            Level.Remove(this);
            if (!(_attach2 is Rope attach2) || attach2._terminated)
                return;
            attach2.TerminateLaterRopesRecurse();
        }

        public override void Terminate()
        {
        }

        public void CheckLinks()
        {
            if (!(_attach2.GetType() == typeof(Rope)) || !(cornerVector != Vec2.Zero))
                return;
            Rope attach2_1 = _attach2 as Rope;
            bool flag = false;
            if ((attach1Point - attach2Point).length < 4.0)
            {
                flag = true;
            }
            else
            {
                float deg = cornerVector.x <= 0.0 ? attach2_1.linkDirectionNormalized + 90f : attach2_1.linkDirectionNormalized - 90f;
                breakVector = Maths.AngleToVec(Maths.DegToRad(deg));
                if (Math.Acos(Vec2.Dot(breakVector, cornerVector)) > Math.PI / 2.0) // the decompiler put Math.PI here not me dan :)
                    breakVector = Maths.AngleToVec(Maths.DegToRad(deg + 180f));
                dirLine = (attach1.position - attach2.position).normalized;
                if (Math.Acos(Vec2.Dot(breakVector, dirLine)) < 1.52079632604984) //uhhhh i think this is just / 2 of pi
                    flag = true;
            }
            if (!flag)
                return;
            _attach2 = attach2_1.attach2;
            _properLength += attach2_1.properLength;
            Level.Remove(attach2_1);
            cornerVector = attach2_1.cornerVector;
        }

        public void AddLength(float length)
        {
            if (_attach2 is Rope attach2_1)
            {
                attach2_1.AddLength(length);
            }
            else
            {
                if (!(_attach2 is Harpoon attach2))
                    return;
                Vec2 vec2 = position - attach2.position;
                vec2.Normalize();
                Harpoon harpoon = attach2;
                harpoon.position -= vec2 * length;
            }
        }

        public void Pull(float length)
        {
            pulled = true;
            Rope pull = this.pull;
            if (pull != null)
                pull.Pull(length);
            else
                properLength += length;
        }

        public override void Update()
        {
            if (Network.isActive && _belongsTo != null && _belongsTo is Grapple)
            {
                _isVine = false;
                _vine = (_belongsTo as Grapple)._ropeSprite;
            }
            if (_terminated || !isServerForObject || !serverForObject)
                return;
            bool flag = false;
            if (_attach1.position != _pos1)
            {
                flag = true;
                _pos1 = _attach1.position;
            }
            if (_attach2.position != _pos2)
            {
                flag = true;
                _pos2 = _attach2.position;
            }
            if (flag || pulled)
            {
                pulled = false;
                Vec2 attach1Point = this.attach1Point;
                Vec2 attach2Point = this.attach2Point;
                Vec2 vec2_1 = attach2Point - attach1Point;
                vec2_1.Normalize();
                int num1 = 0;
                while (Level.CheckPoint<Block>(attach2Point) != null)
                {
                    ++num1;
                    if (num1 <= 30)
                        attach2Point -= vec2_1;
                    else
                        break;
                }
                int num2 = 0;
                Vec2 vec2_2 = attach2Point - attach1Point;
                float num3 = vec2_2.length;
                vec2_2.Normalize();
                if (_belongsTo is IPullBack)
                {
                    if (pull == null && _attach2 is Harpoon)
                        properLength = startLength;
                    else if (attach2 is Harpoon)
                    {
                        Pull(properLength - num3);
                        properLength = num3;
                    }
                }
                while (Level.CheckPoint<Block>(attach1Point) != null)
                {
                    ++num2;
                    if (num2 > 30)
                    {
                        num3 = 0f;
                        break;
                    }
                    attach1Point += vec2_2;
                    --num3;
                }
                if (num3 > 8.0)
                {
                    Vec2 position;
                    AutoBlock autoBlock = Level.CheckLine<AutoBlock>(attach1Point, attach1Point + vec2_2 * num3, out position);
                    if (autoBlock != null)
                    {
                        BlockCorner nearestCorner = autoBlock.GetNearestCorner(position);
                        if (nearestCorner != null)
                        {
                            BlockCorner blockCorner = nearestCorner.Copy();
                            Vec2 vec2_3 = blockCorner.corner - blockCorner.block.position;
                            vec2_3.Normalize();
                            blockCorner.corner += vec2_3 * 1f;
                            Vec2 vec2_4 = blockCorner.corner - attach2.position;
                            if (vec2_4.length > 4.0)
                            {
                                vec2_4 = this.attach2Point - this.attach1Point;
                                linkVector = vec2_4.normalized;
                                Rope rope = new Rope(blockCorner.corner.x, blockCorner.corner.y, null, _attach2, vine: _isVine, tex: _vine)
                                {
                                    cornerVector = cornerVector
                                };
                                vec2_4 = new Vec2(vec2_3.x > 0.0 ? 1f : -1f, vec2_3.y > 0.0 ? 1f : -1f);
                                cornerVector = vec2_4.normalized;
                                rope._corner = blockCorner;
                                rope._belongsTo = _belongsTo;
                                _attach2 = rope;
                                Level.Add(rope);
                                properLength -= rope.length;
                                rope.properLength = rope.length;
                                rope.pull = this;
                                rope.offsetDegrees = rope.linkDirectionNormalized;
                                rope.offsetDir = rope.attach2Point - rope.attach1Point;
                                rope.linkDirectionOnSplit = linkDirection;
                            }
                        }
                    }
                }
            }
            CheckLinks();
        }

        public void SetServer(bool server)
        {
            serverForObject = server;
            if (!(_attach2 is Rope attach2) || attach2._terminated)
                return;
            attach2.SetServer(server);
        }

        public override void Draw()
        {
            if (DevConsole.showCollision && cornerVector != Vec2.Zero)
            {
                Graphics.DrawLine(_attach2.position, _attach2.position + cornerVector * 32f, Color.Red);
                Graphics.DrawLine(_attach2.position, _attach2.position + breakVector * 16f, Color.Blue);
                Graphics.DrawLine(_attach2.position, _attach2.position + dirLine * 8f, Color.Orange);
            }
            float num1 = length / properLength;
            if (!serverForObject)
                num1 = 1f;
            if (_vine != null)
            {
                Vec2 vec2_1 = attach2Point - attach1Point;
                Vec2 normalized = vec2_1.normalized;
                Vec2 vec2_2 = normalized;
                vec2_2 = vec2_2.Rotate(Maths.DegToRad(90f), Vec2.Zero);
                double length1 = vec2_1.length;
                float num2 = 16f;
                Vec2 vec2_3 = attach1Point + normalized * num2;
                Vec2 p1 = attach1Point;
                Depth depth = this.depth;
                double num3 = num2;
                int num4 = (int)Math.Ceiling(length1 / num3);
                for (int index = 0; index < num4; ++index)
                {
                    float a = 6.283185f / num4 * index;
                    float num5 = (float)((1.0 - num1) * 16.0);
                    Vec2 p2 = vec2_3 + vec2_2 * ((float)Math.Sin(a) * num5);
                    if (index == num4 - 1)
                        p2 = attach2Point;
                    _vine.angleDegrees = (float)-(Maths.PointDirection(p1, p2) + 90.0);
                    _vine.depth = depth;
                    depth += 1;
                    float length2 = (p2 - p1).length;
                    if (index == num4 - 1)
                    {
                        _vine.yscale = 1f;
                        Graphics.Draw(_vine, p1.x, p1.y, new Rectangle(0f, 0f, 16f, (int)(length2 % num2)));
                    }
                    else
                    {
                        _vine.yscale = length2 / 16f + 0.1f;
                        Graphics.Draw(_vine, p1.x, p1.y);
                    }
                    p1 = p2;
                    vec2_3 += normalized * num2;
                    float num6 = a + 6.283185f / num4;
                }
            }
            else if (num1 < 0.95f && num1 > 0.0f)
            {
                Vec2 vec2_4 = attach2Point - attach1Point;
                Vec2 vec2_5 = vec2_4.normalized;
                vec2_5 = vec2_5.Rotate(Maths.DegToRad(90f), Vec2.Zero);
                float a = 0.7853982f;
                Vec2 vec2_6 = attach1Point + vec2_4 / 8f;
                Vec2 p1 = attach1Point;
                for (int index = 0; index < 8; ++index)
                {
                    float num7 = (float)((1.0 - num1) * 8.0);
                    Graphics.DrawLine(p1, vec2_6 + vec2_5 * (float)Math.Sin(a) * num7, Color.White * 0.8f, depth: (depth - 1));
                    p1 = vec2_6 + vec2_5 * (float)Math.Sin(a) * num7;
                    vec2_6 += vec2_4 / 8f;
                    a += 0.7853982f;
                }
            }
            else
                Graphics.DrawLine(attach1Point, attach2Point, Color.White * 0.8f, depth: (depth - 1));
        }
    }
}
