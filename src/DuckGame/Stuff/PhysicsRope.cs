// Decompiled with JetBrains decompiler
// Type: DuckGame.PhysicsRope
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Stuff|Ropes")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class PhysicsRope : Thing
    {
        protected bool chain;
        protected Sprite _vine;
        protected Sprite _vineEnd;
        protected Sprite _beam;
        private bool _root;
        private List<PhysicsRopeSection> _nodes = new List<PhysicsRopeSection>();
        private int _divisions = 10;
        private float _length = 0.5f;
        private float _lenDiv;
        private float _gravity = 0.25f;
        private bool _create = true;
        public Vine _lowestVine;
        private int _lowestVineSection;
        public EditorProperty<int> length = new EditorProperty<int>(4, min: 1f, max: 16f, increment: 1f);
        private float soundWait;

        public bool root
        {
            get => _root;
            set => _root = value;
        }

        public PhysicsRope(float xpos, float ypos, PhysicsRope next = null)
          : base(xpos, ypos)
        {
            _vine = new SpriteMap("vine", 16, 16);
            graphic = _vine;
            center = new Vec2(8f, 8f);
            _vineEnd = new Sprite("vineStretchEnd")
            {
                center = new Vec2(8f, 0f)
            };
            collisionOffset = new Vec2(-5f, -4f);
            collisionSize = new Vec2(11f, 7f);
            graphic = _vine;
            _beam = new Sprite("vineStretch");
            _editorName = "Vine";
            editorTooltip = "The original rope. Great for swinging through a jungle.";
            _placementCost += 50;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            position.y -= 8f;
            _length = length.value / 6.5f;
            _divisions = (int)(length.value * 16.0 / 8.0);
            _lenDiv = _length / _divisions;
            for (int index = 0; index <= _divisions; ++index)
            {
                _nodes.Add(new PhysicsRopeSection(x + _lenDiv * index, y, this));
                Level.Add(_nodes[_nodes.Count - 1]);
            }
        }

        public override void Terminate()
        {
            foreach (Thing node in _nodes)
                Level.Remove(node);
            base.Terminate();
        }

        public virtual Vine GetSection(float x, float y, int div) => new Vine(x, y, div);

        public Vine LatchOn(PhysicsRopeSection section, Duck d)
        {
            UpdateVineProgress();
            for (int index = 0; index <= _divisions; ++index)
            {
                if (section == _nodes[index])
                {
                    if (index < _lowestVineSection)
                    {
                        int num = index;
                        Vine lowestVine1 = _lowestVine;
                        Vec2 vec2 = new Vec2(x, y + 8f);
                        _lowestVine = GetSection(vec2.x, vec2.y, num * 8);
                        _lowestVine.length.value = num / 2;
                        _lowestVine.owner = d;
                        _lowestVine.sectionIndex = index;
                        Level.Add(_lowestVine);
                        if (lowestVine1 != null)
                        {
                            lowestVine1._rope.attach2 = _lowestVine.owner;
                            lowestVine1._rope.properLength = (lowestVine1._rope.attach2Point - lowestVine1._rope.attach1Point).length;
                            _lowestVine.nextVine = lowestVine1;
                            lowestVine1.prevVine = _lowestVine;
                        }
                        _lowestVine.changeSpeed = false;
                        if (d.vSpeed > 0.0)
                            d.vSpeed = 0f;
                        _lowestVine.UpdateRopeStuff();
                        _lowestVine.UpdateRopeStuff();
                        _lowestVine.changeSpeed = true;
                        Vine lowestVine2 = _lowestVine;
                        _lowestVine = lowestVine1;
                        return lowestVine2;
                    }
                    int num1 = index - _lowestVineSection;
                    Vine lowestVine = _lowestVine;
                    Vec2 vec2_1 = new Vec2(x, y + 8f);
                    if (_lowestVine != null)
                        vec2_1 = _lowestVine._rope.attach1Point;
                    _lowestVine = GetSection(vec2_1.x, vec2_1.y, num1 * 8);
                    _lowestVine.length.value = num1 / 2;
                    _lowestVine.owner = d;
                    _lowestVine.sectionIndex = index;
                    Level.Add(_lowestVine);
                    if (lowestVine != null)
                    {
                        _lowestVine._rope.attach2 = lowestVine.owner;
                        lowestVine.nextVine = _lowestVine;
                        _lowestVine.prevVine = lowestVine;
                    }
                    _lowestVine.changeSpeed = false;
                    if (d.vSpeed > 0.0)
                        d.vSpeed = 0f;
                    _lowestVine.UpdateRopeStuff();
                    _lowestVine.UpdateRopeStuff();
                    _lowestVine.changeSpeed = true;
                    _lowestVineSection = index;
                    return _lowestVine;
                }
            }
            return null;
        }

        public Vine highestVine
        {
            get
            {
                if (_lowestVine == null || _lowestVine.removeFromLevel)
                    return null;
                Vine highestVine = _lowestVine;
                while (highestVine.prevVine != null)
                    highestVine = highestVine.prevVine;
                return highestVine;
            }
        }

        public void UpdateVineProgress()
        {
            if (_lowestVine != null && _lowestVine.owner != null)
                _nodes[_lowestVineSection].position = _lowestVine.owner.position;
            else if (_lowestVine != null && _lowestVine.prevVine != null)
            {
                _lowestVine = _lowestVine.prevVine;
                _lowestVineSection = _lowestVine.sectionIndex;
            }
            else
            {
                _lowestVineSection = 0;
                _lowestVine = null;
            }
        }

        public override void Update()
        {
            if (_create)
            {
                int num = 0;
                foreach (Transform node in _nodes)
                {
                    node.position = position + new Vec2(0f, num * 8);
                    ++num;
                }
                _create = false;
                for (int index = 0; index < 10; ++index)
                    Update();
            }
            base.Update();
            if (_nodes.Count == 0)
                return;
            UpdateVineProgress();
            _nodes[0].position = position;
            foreach (PhysicsRopeSection node in _nodes)
            {
                node.accel.y = _gravity;
                node.calcPos = node.position;
            }
            for (int index = 1; index <= _divisions; ++index)
            {
                float x = _nodes[index].position.x;
                _nodes[index].calcPos.x += (float)(0.999f * _nodes[index].calcPos.x - 0.999f * _nodes[index].tempPos.x) + _nodes[index].accel.x;
                float y = _nodes[index].position.y;
                _nodes[index].calcPos.y += (float)(0.999f * _nodes[index].calcPos.y - 0.999f * _nodes[index].tempPos.y) + _nodes[index].accel.y;
                _nodes[index].tempPos.x = x;
                _nodes[index].tempPos.y = y;
            }
            for (int index1 = 1; index1 <= _divisions; ++index1)
            {
                for (int index2 = 1; index2 <= _divisions; ++index2)
                {
                    float num1 = (float)((_nodes[index2].calcPos.x - _nodes[index2 - 1].calcPos.x) / 100.0);
                    float num2 = (float)((_nodes[index2].calcPos.y - _nodes[index2 - 1].calcPos.y) / 100.0);
                    float num3 = (float)Math.Sqrt(num1 * num1 + num2 * num2);
                    float num4 = (float)((num3 - _lenDiv) * 50.0);
                    _nodes[index2].calcPos.x -= num1 / num3 * num4;
                    _nodes[index2].calcPos.y -= num2 / num3 * num4;
                    _nodes[index2 - 1].calcPos.x += num1 / num3 * num4;
                    _nodes[index2 - 1].calcPos.y += num2 / num3 * num4;
                }
            }
            Vine highestVine = this.highestVine;
            List<VineSection> vineSectionList = null;
            VineSection vineSection = null;
            int index3 = 0;
            if (highestVine != null)
            {
                vineSectionList = highestVine.points;
                vineSection = vineSectionList[0];
            }
            bool flag1 = false;
            bool flag2 = false;
            int num5 = 0;
            for (int index4 = 0; index4 <= _divisions; ++index4)
            {
                if (vineSection != null)
                {
                    if (index4 >= vineSection.lowestSection)
                    {
                        ++index3;
                        vineSection = index3 >= vineSectionList.Count ? null : vineSectionList[index3];
                        num5 = 0;
                    }
                    if (vineSection != null && index4 < vineSection.lowestSection)
                    {
                        Vec2 vec2 = vineSection.pos2 - vineSection.pos1;
                        vec2.Normalize();
                        _nodes[index4].position = vineSection.pos1 + vec2 * num5 * 8f;
                        _nodes[index4].calcPos = _nodes[index4].position;
                    }
                }
                ++num5;
                _nodes[index4].frictionMult = 0f;
                _nodes[index4].gravMultiplier = 0f;
                _nodes[index4].hSpeed = _nodes[index4].calcPos.x - _nodes[index4].position.x;
                _nodes[index4].vSpeed = _nodes[index4].calcPos.y - _nodes[index4].position.y;
                float num6 = 5f;
                if (_nodes[index4].hSpeed > 0.0 && _nodes[index4].hSpeed > num6)
                    _nodes[index4].hSpeed = num6;
                if (_nodes[index4].hSpeed < 0.0 && _nodes[index4].hSpeed < -num6)
                    _nodes[index4].hSpeed = -num6;
                foreach (PhysicsObject physicsObject in Level.CheckPointAll<PhysicsObject>(_nodes[index4].position))
                {
                    if (physicsObject.hSpeed > 0.0 && _nodes[index4].hSpeed < physicsObject.hSpeed)
                    {
                        _nodes[index4].hSpeed += physicsObject.hSpeed;
                        if (Math.Abs(physicsObject.hSpeed) > 2.0)
                        {
                            if (Math.Abs(physicsObject.hSpeed) > 4.0)
                                flag2 = true;
                            flag1 = true;
                        }
                    }
                    if (physicsObject.hSpeed < 0.0 && _nodes[index4].hSpeed > physicsObject.hSpeed)
                    {
                        _nodes[index4].hSpeed += physicsObject.hSpeed;
                        if (Math.Abs(physicsObject.hSpeed) > 2.0)
                        {
                            if (Math.Abs(physicsObject.hSpeed) > 4.0)
                                flag2 = true;
                            flag1 = true;
                        }
                    }
                }
                _nodes[index4].UpdatePhysics();
            }
            if (soundWait > 0.0)
                soundWait -= 0.01f;
            if (!(chain & flag1) || soundWait > 0.0)
                return;
            soundWait = 0.1f;
            if (!flag1)
                return;
            int num7 = Rando.Int(2);
            if (flag2)
            {
                if (num7 == 0)
                    SFX.Play("chainShake01", Rando.Float(0.6f, 0.8f), Rando.Float(-0.2f, 0.2f));
                else if (num7 == 1)
                    SFX.Play("chainShake02", Rando.Float(0.6f, 0.8f), Rando.Float(-0.2f, 0.2f));
                else
                    SFX.Play("chainShake03", Rando.Float(0.6f, 0.8f), Rando.Float(-0.2f, 0.2f));
            }
            else if (num7 == 0)
                SFX.Play("chainShakeSmall", Rando.Float(0.3f, 0.5f), Rando.Float(-0.2f, 0.2f));
            else if (num7 == 1)
                SFX.Play("chainShakeSmall02", Rando.Float(0.3f, 0.5f), Rando.Float(-0.2f, 0.2f));
            else
                SFX.Play("chainShakeSmall03", Rando.Float(0.3f, 0.5f), Rando.Float(-0.2f, 0.2f));
        }

        public override void Draw()
        {
            if (Level.current is Editor)
            {
                graphic.center = new Vec2(8f, 8f);
                graphic.depth = depth;
                for (int index = 0; index < (int)length; ++index)
                    Graphics.Draw(graphic, x, y + index * 16);
            }
            else
            {
                UpdateVineProgress();
                Depth depth = -0.5f;
                Vec2 p1 = position + new Vec2(0f, -4f);
                if (_lowestVine != null && _lowestVine.owner != null)
                {
                    p1 = _lowestVine.owner.position;
                    if (highestVine != null && highestVine._rope.attach2 is Harpoon)
                        Graphics.DrawTexturedLine(_beam.texture, position + new Vec2(0f, -4f), _nodes[0].position + new Vec2(0f, 2f), Color.White, depth: depth);
                }
                int num = -1;
                foreach (PhysicsRopeSection node in _nodes)
                {
                    depth += 1;
                    ++num;
                    if (num >= _lowestVineSection)
                    {
                        Vec2 normalized = (node.position - p1).normalized;
                        if (num == _nodes.Count - 1)
                            Graphics.DrawTexturedLine(_vineEnd.texture, p1, node.position + normalized, Color.White, depth: depth);
                        else
                            Graphics.DrawTexturedLine(_beam.texture, p1, node.position + normalized, Color.White, depth: depth);
                        p1 = node.position;
                    }
                }
            }
        }
    }
}
