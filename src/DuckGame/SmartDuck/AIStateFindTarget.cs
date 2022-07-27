// Decompiled with JetBrains decompiler
// Type: DuckGame.AIStateFindTarget
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            ++this._refresh;
            if (this._refresh > 10 && ai.canRefresh)
            {
                this._refresh = 0;
                this._target = null;
                ai.canRefresh = false;
            }
            if (this._target == null)
            {
                List<Thing> list = Level.current.things[typeof(Duck)].Where<Thing>(x => x != duck && !(x as Duck).dead).ToList<Thing>();
                if (!(AI.Nearest(duck.position, list) is Duck duck1))
                    return new AIStateWait(Rando.Float(0.8f, 1f));
                this._target = duck1;
                ai.SetTarget(duck1.position);
            }
            else
            {
                if ((double)(duck.position - this._target.position).length < 10.0)
                {
                    this._scatterWait -= 0.01f;
                    if (_scatterWait < 0.0)
                    {
                        List<Thing> list = Level.current.things[typeof(PathNode)].ToList<Thing>();
                        ai.SetTarget(list[Rando.Int(list.Count - 1)].position);
                        this._state.Push(new AIStateWait(1f + Rando.Float(1f)));
                        this._scatterWait = 1f;
                    }
                }
                if ((double)Math.Abs(duck.y - this._target.y) < 16.0 && (double)Math.Abs(duck.x - this._target.x) < 150.0 && Level.CheckRay<Duck>(duck.position + new Vec2(duck.offDir * 10, 0.0f), this._target.position) == this._target)
                {
                    if (Level.CheckLine<Block>(duck.position, this._target.position) == null)
                    {
                        this._targetWait -= 0.2f;
                        if (_targetWait <= 0.0 && (double)Rando.Float(1f) > 0.600000023841858)
                        {
                            ai.Press("SHOOT");
                            this._state.Push(new AIStateWait(Rando.Float(0.2f, 0.3f)));
                            return this;
                        }
                    }
                }
                else
                    this._targetWait = 1f;
            }
            return this;
        }
    }
}
