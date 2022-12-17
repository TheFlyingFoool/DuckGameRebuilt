using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class BigRagdoll : Ragdoll
    {
        public float size;
        public BigRagdoll(float xpos, float ypos, Duck d, bool slide, float degrees, int off, Vec2 v) : base(xpos, ypos, d, slide, degrees, off, v)
        {
            size = d.duckSize;
            partSep *= size;
        }
        public override void Solve(PhysicsObject body1, PhysicsObject body2, float dist)
        {
            float num1 = dist;
            Vec2 vec2_1 = body2.position - body1.position;
            float num2 = vec2_1.length;
            if (num2 < 0.0001f)
                num2 = 0.0001f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(body1.hSpeed, body1.vSpeed);
            Vec2 vec2_4 = new Vec2(body2.hSpeed, body2.vSpeed);
            double num3 = Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.1f * size;
            float num6 = 2.1f * size;
            if (body1 == part1 && jetting)
                num5 = 6f;
            else if (body2 == part1 && jetting)
                num6 = 6f;
            double num7 = num4;
            float num8 = (float)((num3 + num7) / (num5 + num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            Vec2 vec2_7 = vec2_4 - vec2_5 * num6;
            if (body1.owner == null)
            {
                body1.hSpeed = vec2_6.x;
                body1.vSpeed = vec2_6.y;
            }
            if (body2.owner != null)
                return;
            body2.hSpeed = vec2_7.x;
            body2.vSpeed = vec2_7.y;
        }
        public override void Organize()
        {
            Vec2 vec = Maths.AngleToVec(angle);
            if (_part1 == null)
            {
                _part1 = new BigRagdollPart(x - vec.x * partSep, y - vec.y * partSep, 0, _duck != null ? _duck.persona : persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part1);
                _part2 = new BigRagdollPart(x, y, 2, _duck != null ? _duck.persona : persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part2);
                _part3 = new BigRagdollPart(x + vec.x * partSep, y + vec.y * partSep, 1, _duck != null ? _duck.persona : persona, offDir, this);
                if (Network.isActive && !GhostManager.inGhostLoop)
                    GhostManager.context.MakeGhost(_part3);
                Level.Add(_part1);
                Level.Add(_part2);
                Level.Add(_part3);
            }
            else
            {
                _part1.SortOutDetails(x - vec.x * partSep, y - vec.y * partSep, 0, _duck != null ? _duck.persona : persona, offDir, this);
                _part2.SortOutDetails(x, y, 2, _duck != null ? _duck.persona : persona, offDir, this);
                _part3.SortOutDetails(x + vec.x * partSep, y + vec.y * partSep, 1, _duck != null ? _duck.persona : persona, offDir, this);
            }
            _part1.joint = _part2;
            _part3.joint = _part2;
            _part1.connect = _part3;
            _part3.connect = _part1;
            _part1.framesSinceGrounded = 99;
            _part2.framesSinceGrounded = 99;
            _part3.framesSinceGrounded = 99;
            if (_duck == null)
                return;
            if (!(Level.current is GameLevel) || !(Level.current as GameLevel).isRandom)
            {
                _duck.ReturnItemToWorld(_part1);
                _duck.ReturnItemToWorld(_part2);
                _duck.ReturnItemToWorld(_part3);
            }
            _part3.depth = new Depth(_duck.depth.value);
            _part1.depth = _part3.depth - 1;
        }
    }
}
