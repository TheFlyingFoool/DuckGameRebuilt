// Decompiled with JetBrains decompiler
// Type: DuckGame.Rope
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            get => this._attach1;
            set => this._attach1 = value;
        }

        public Thing attach2
        {
            get => this._attach2;
            set => this._attach2 = value;
        }

        public override NetworkConnection connection => this._belongsTo != null ? this._belongsTo.connection : base.connection;

        public override NetIndex8 authority => this._belongsTo != null ? this._belongsTo.authority : base.authority;

        public float linkDirection => this.attach2 is Rope attach2 ? Maths.PointDirection(new Vec2(0f, 0f), (this.attach2Point - this.attach1Point).Rotate(Maths.DegToRad(attach2.offsetDegrees), Vec2.Zero)) : 0f;

        public float linkDirectionNormalized => Maths.PointDirection(new Vec2(0f, 0f), this.attach2Point - this.attach1Point);

        public float properLength
        {
            get => this._properLength;
            set => this._properLength = value;
        }

        public Vec2 attach1Point
        {
            get
            {
                if (!(this._attach1 is Rope attach1) || attach1._corner == null)
                    return this._attach1.position;
                Vec2 vec2 = attach1._corner.corner - attach1._corner.block.position;
                vec2.Normalize();
                return this._attach1.position + vec2 * 4f;
            }
        }

        public Vec2 attach2Point
        {
            get
            {
                if (!(this._attach2 is Rope attach2) || attach2._corner == null)
                    return this._attach2.position;
                Vec2 vec2 = attach2._corner.corner - attach2._corner.block.position;
                vec2.Normalize();
                return this._attach2.position + vec2 * 4f;
            }
        }

        public float length => this._attach1 != null && this._attach2 != null ? (this._attach1.position - this._attach2.position).length : 0f;

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
            this._belongsTo = belongsTo;
            if (attach1Val == null)
                attach1Val = this;
            if (attach2Val == null)
                attach2Val = this;
            this._attach1 = attach1Val;
            this._attach2 = attach2Val;
            this._pos1 = attach1Val.position;
            this._pos2 = attach2Val.position;
            this._thing = thing;
            if (vine)
            {
                this._vine = new Sprite(nameof(vine))
                {
                    center = new Vec2(8f, 0f)
                };
            }
            if (tex != null)
                this._vine = tex;
            this._isVine = vine;
            this.depth = -0.5f;
        }

        public void RemoveRope()
        {
            this._terminated = true;
            this.visible = false;
            Level.Remove(this);
            if (this._attach1 is Rope attach1 && !attach1._terminated)
                attach1.RemoveRope();
            if (!(this._attach2 is Rope attach2) || attach2._terminated)
                return;
            attach2.RemoveRope();
        }

        public void TerminateLaterRopes()
        {
            if (this._attach2 is Rope attach2 && !attach2._terminated)
                attach2.TerminateLaterRopesRecurse();
            this._attach2 = null;
        }

        public void TerminateLaterRopesRecurse()
        {
            this._terminated = true;
            this.visible = false;
            Level.Remove(this);
            if (!(this._attach2 is Rope attach2) || attach2._terminated)
                return;
            attach2.TerminateLaterRopesRecurse();
        }

        public override void Terminate()
        {
        }

        public void CheckLinks()
        {
            if (!(this._attach2.GetType() == typeof(Rope)) || !(this.cornerVector != Vec2.Zero))
                return;
            Rope attach2_1 = this._attach2 as Rope;
            bool flag = false;
            if ((double)(this.attach1Point - this.attach2Point).length < 4.0)
            {
                flag = true;
            }
            else
            {
                float deg = cornerVector.x <= 0.0 ? attach2_1.linkDirectionNormalized + 90f : attach2_1.linkDirectionNormalized - 90f;
                this.breakVector = Maths.AngleToVec(Maths.DegToRad(deg));
                if (Math.Acos((double)Vec2.Dot(this.breakVector, this.cornerVector)) > Math.PI / 2.0)
                    this.breakVector = Maths.AngleToVec(Maths.DegToRad(deg + 180f));
                this.dirLine = (this.attach1.position - this.attach2.position).normalized;
                if (Math.Acos((double)Vec2.Dot(this.breakVector, this.dirLine)) < 1.52079632604984)
                    flag = true;
            }
            if (!flag)
                return;
            this._attach2 = attach2_1.attach2;
            this._properLength += attach2_1.properLength;
            Level.Remove(attach2_1);
            this.cornerVector = attach2_1.cornerVector;
        }

        public void AddLength(float length)
        {
            if (this._attach2 is Rope attach2_1)
            {
                attach2_1.AddLength(length);
            }
            else
            {
                if (!(this._attach2 is Harpoon attach2))
                    return;
                Vec2 vec2 = this.position - attach2.position;
                vec2.Normalize();
                Harpoon harpoon = attach2;
                harpoon.position -= vec2 * length;
            }
        }

        public void Pull(float length)
        {
            this.pulled = true;
            Rope pull = this.pull;
            if (pull != null)
                pull.Pull(length);
            else
                this.properLength += length;
        }

        public override void Update()
        {
            if (Network.isActive && this._belongsTo != null && this._belongsTo is Grapple)
            {
                this._isVine = false;
                this._vine = (this._belongsTo as Grapple)._ropeSprite;
            }
            if (this._terminated || !this.isServerForObject || !this.serverForObject)
                return;
            bool flag = false;
            if (this._attach1.position != this._pos1)
            {
                flag = true;
                this._pos1 = this._attach1.position;
            }
            if (this._attach2.position != this._pos2)
            {
                flag = true;
                this._pos2 = this._attach2.position;
            }
            if (flag || this.pulled)
            {
                this.pulled = false;
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
                if (this._belongsTo is IPullBack)
                {
                    if (this.pull == null && this._attach2 is Harpoon)
                        this.properLength = this.startLength;
                    else if (this.attach2 is Harpoon)
                    {
                        this.Pull(this.properLength - num3);
                        this.properLength = num3;
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
                if ((double)num3 > 8.0)
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
                            Vec2 vec2_4 = blockCorner.corner - this.attach2.position;
                            if ((double)vec2_4.length > 4.0)
                            {
                                vec2_4 = this.attach2Point - this.attach1Point;
                                this.linkVector = vec2_4.normalized;
                                Rope rope = new Rope(blockCorner.corner.x, blockCorner.corner.y, null, this._attach2, vine: this._isVine, tex: this._vine)
                                {
                                    cornerVector = this.cornerVector
                                };
                                vec2_4 = new Vec2(vec2_3.x > 0.0 ? 1f : -1f, vec2_3.y > 0.0 ? 1f : -1f);
                                this.cornerVector = vec2_4.normalized;
                                rope._corner = blockCorner;
                                rope._belongsTo = this._belongsTo;
                                this._attach2 = rope;
                                Level.Add(rope);
                                this.properLength -= rope.length;
                                rope.properLength = rope.length;
                                rope.pull = this;
                                rope.offsetDegrees = rope.linkDirectionNormalized;
                                rope.offsetDir = rope.attach2Point - rope.attach1Point;
                                rope.linkDirectionOnSplit = this.linkDirection;
                            }
                        }
                    }
                }
            }
            this.CheckLinks();
        }

        public void SetServer(bool server)
        {
            this.serverForObject = server;
            if (!(this._attach2 is Rope attach2) || attach2._terminated)
                return;
            attach2.SetServer(server);
        }

        public override void Draw()
        {
            if (DevConsole.showCollision && this.cornerVector != Vec2.Zero)
            {
                Graphics.DrawLine(this._attach2.position, this._attach2.position + this.cornerVector * 32f, Color.Red);
                Graphics.DrawLine(this._attach2.position, this._attach2.position + this.breakVector * 16f, Color.Blue);
                Graphics.DrawLine(this._attach2.position, this._attach2.position + this.dirLine * 8f, Color.Orange);
            }
            float num1 = this.length / this.properLength;
            if (!this.serverForObject)
                num1 = 1f;
            if (this._vine != null)
            {
                Vec2 vec2_1 = this.attach2Point - this.attach1Point;
                Vec2 normalized = vec2_1.normalized;
                Vec2 vec2_2 = normalized;
                vec2_2 = vec2_2.Rotate(Maths.DegToRad(90f), Vec2.Zero);
                double length1 = (double)vec2_1.length;
                float num2 = 16f;
                Vec2 vec2_3 = this.attach1Point + normalized * num2;
                Vec2 p1 = this.attach1Point;
                Depth depth = this.depth;
                double num3 = (double)num2;
                int num4 = (int)Math.Ceiling(length1 / num3);
                for (int index = 0; index < num4; ++index)
                {
                    float a = 6.283185f / num4 * index;
                    float num5 = (float)((1.0 - (double)num1) * 16.0);
                    Vec2 p2 = vec2_3 + vec2_2 * ((float)Math.Sin((double)a) * num5);
                    if (index == num4 - 1)
                        p2 = this.attach2Point;
                    this._vine.angleDegrees = (float)-((double)Maths.PointDirection(p1, p2) + 90.0);
                    this._vine.depth = depth;
                    depth += 1;
                    float length2 = (p2 - p1).length;
                    if (index == num4 - 1)
                    {
                        this._vine.yscale = 1f;
                        Graphics.Draw(this._vine, p1.x, p1.y, new Rectangle(0f, 0f, 16f, (int)((double)length2 % (double)num2)));
                    }
                    else
                    {
                        this._vine.yscale = (float)((double)length2 / 16.0 + 0.100000001490116);
                        Graphics.Draw(this._vine, p1.x, p1.y);
                    }
                    p1 = p2;
                    vec2_3 += normalized * num2;
                    float num6 = a + 6.283185f / num4;
                }
            }
            else if ((double)num1 < 0.949999988079071 && (double)num1 > 0.0)
            {
                Vec2 vec2_4 = this.attach2Point - this.attach1Point;
                Vec2 vec2_5 = vec2_4.normalized;
                vec2_5 = vec2_5.Rotate(Maths.DegToRad(90f), Vec2.Zero);
                float a = 0.7853982f;
                Vec2 vec2_6 = this.attach1Point + vec2_4 / 8f;
                Vec2 p1 = this.attach1Point;
                for (int index = 0; index < 8; ++index)
                {
                    float num7 = (float)((1.0 - (double)num1) * 8.0);
                    Graphics.DrawLine(p1, vec2_6 + vec2_5 * (float)Math.Sin((double)a) * num7, Color.White * 0.8f, depth: (this.depth - 1));
                    p1 = vec2_6 + vec2_5 * (float)Math.Sin((double)a) * num7;
                    vec2_6 += vec2_4 / 8f;
                    a += 0.7853982f;
                }
            }
            else
                Graphics.DrawLine(this.attach1Point, this.attach2Point, Color.White * 0.8f, depth: (this.depth - 1));
        }
    }
}
