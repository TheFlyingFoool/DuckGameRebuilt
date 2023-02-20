//// Decompiled with JetBrains decompiler
//// Type: DuckGame.AIStateFindTarget
////removed for regex reasons Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class AIStateFindTarget : AIState
    {
        private Duck _target;
        private int _refresh;
        private float _targetWait = 1f;
        private float _scatterWait = 1f;

        public override AIState Update(Duck duck, DuckAI ai)
        {
            if (!(duck.holdObject is Gun holdObject))
                return null;
            if (holdObject.ammo <= 0)
            {
                duck.ThrowItem();
                return new AIStateWait(1f);
            }
            ++_refresh;
            if (_refresh > 10 && ai.canRefresh)
            {
                _refresh = 0;
                _target = null;
                ai.canRefresh = false;
            }
            if (_target == null)
            {
                List<Thing> list = Level.current.things[typeof(Duck)].Where(x => x != duck && !(x as Duck).dead).ToList();
                if (!(AI.Nearest(duck.position, list) is Duck duck1))
                    return new AIStateWait(Rando.Float(0.8f, 1f));
                _target = duck1;
                ai.SetTarget(duck1.position);
            }
            else
            {
                if ((duck.position - _target.position).length < 10f)
                {
                    _scatterWait -= 0.01f;
                    if (_scatterWait < 0.0)
                    {
                        List<Thing> list = Level.current.things[typeof(PathNode)].ToList();
                        ai.SetTarget(list[Rando.Int(list.Count - 1)].position);
                        _state.Push(new AIStateWait(1f + Rando.Float(1f)));
                        _scatterWait = 1f;
                    }
                }
                if (Math.Abs(duck.y - _target.y) < 16f && Math.Abs(duck.x - _target.x) < 150.0 && Level.CheckRay<Duck>(duck.position + new Vec2(duck.offDir * 10, 0f), _target.position) == _target)
                {
                    if (Level.CheckLine<Block>(duck.position, _target.position) == null)
                    {
                        _targetWait -= 0.2f;
                        if (_targetWait <= 0f && Rando.Float(1f) > 0.6f)
                        {
                            ai.Press(Triggers.Shoot);
                            _state.Push(new AIStateWait(Rando.Float(0.2f, 0.3f)));
                            return this;
                        }
                    }
                }
                else
                    _targetWait = 1f;
            }
            return this;
        }
    }
}
