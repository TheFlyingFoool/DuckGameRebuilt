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
            Rope at2 = _attach2 as Rope;
            bool regroup = false;
            if ((attach1Point - attach2Point).length < 4.0)
            {
                regroup = true;
            }
            else
            {
                float angleDir = 0.0f;
                if (cornerVector.x > 0)
                    angleDir = at2.linkDirectionNormalized - 90;
                else
                    angleDir = at2.linkDirectionNormalized + 90;
                breakVector = Maths.AngleToVec(Maths.DegToRad(angleDir));
                if (Math.Acos(Vec2.Dot(breakVector, cornerVector)) > Math.PI / 2) // the decompiler put Math.PI here not me dan :)
                    breakVector = Maths.AngleToVec(Maths.DegToRad(angleDir + 180f));
                dirLine = (attach1.position - attach2.position).normalized;
                if (Math.Acos(Vec2.Dot(breakVector, dirLine)) < ((Math.PI / 2) - 0.05f))
                    regroup = true;
            }
            if (!regroup)
                return;
            _attach2 = at2.attach2;
            _properLength += at2.properLength;
            Level.Remove(at2);
            cornerVector = at2.cornerVector;
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
            float amount = (length / properLength);
            if (!serverForObject)
                amount = 1.0f;

            if (_vine != null)
            {
                Vec2 travel = (attach2Point - attach1Point);
                Vec2 travelNorm = travel.normalized;
                Vec2 travelOffset = travelNorm;
                travelOffset = travelOffset.Rotate(Maths.DegToRad(90.0f), Vec2.Zero);
                float len = travel.length;
                float stepSize = 16;
                Vec2 drawStart = attach1Point + (travelNorm * stepSize);
                Vec2 drawPrev = attach1Point;
                Depth d = depth;
                int num = (int)Math.Ceiling(len / stepSize);
                for (int i = 0; i < num; i++)
                {
                    float sinVal = (((float)Math.PI * 2.0f) / num) * i;
                    float sinMult = (1.0f - amount) * 16.0f;

                    Vec2 toPos = drawStart + ((travelOffset * (float)(Math.Sin(sinVal) * sinMult)));
                    if (i == num - 1)
                        toPos = attach2Point;
                    _vine.angleDegrees = -(Maths.PointDirection(drawPrev, toPos) + 90);
                    _vine.depth = d;
                    d += 1;
                    float lent = (toPos - drawPrev).length;
                    if (i == num - 1)
                    {
                        _vine.yscale = 1.0f;
                        Graphics.Draw(_vine, drawPrev.x, drawPrev.y, new Rectangle(0, 0, 16, (int)(lent % stepSize)));
                    }
                    else
                    {
                        _vine.yscale = (lent / 16.0f) + 0.1f;
                        Graphics.Draw(_vine, drawPrev.x, drawPrev.y);
                    }
                    drawPrev = toPos;
                    drawStart += travelNorm * stepSize;
                    //sinVal += (((float)Math.PI * 2.0f) / num);
                }
            }
            else
            {

                if (amount < 0.95f && amount > 0.0f)
                {
                    Vec2 travel = (attach2Point - attach1Point);
                    Vec2 travelOffset = travel.normalized;
                    travelOffset = travelOffset.Rotate(Maths.DegToRad(90.0f), Vec2.Zero);


                    float sinVal = (((float)Math.PI * 2.0f) / 8.0f);
                    Vec2 drawStart = attach1Point + (travel / 8);
                    Vec2 drawPrev = attach1Point;
                    for (int i = 0; i < 8; i++)
                    {
                        float sinMult = (1.0f - amount) * 8.0f;
                        Graphics.DrawLine(drawPrev, drawStart + ((travelOffset * (float)Math.Sin(sinVal)) * sinMult), Color.White * 0.8f, 1.0f, depth - 1);
                        drawPrev = drawStart + ((travelOffset * (float)Math.Sin(sinVal)) * sinMult);
                        drawStart += (travel / 8);
                        sinVal += (((float)Math.PI * 2.0f) / 8.0f);
                    }
                }
                else
                    Graphics.DrawLine(attach1Point, attach2Point, Color.White * 0.8f, 1.0f, depth - 1);
            }
        }
    }
}
