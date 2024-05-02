using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class AIStateFindGun : AIState
    {
        private int _refresh;
        private Thing _target;

        public override AIState Update(Duck duck, DuckAI ai)
        {
            if (duck.holdObject != null && duck.holdObject is Gun)
                return null;
            duck.ThrowItem();
            if (_target == null)
            {
                List<Thing> list1 = Level.current.things[typeof(Gun)].Where(x => (x as Gun).ammo > 0 && (x as Gun).owner == null).ToList();
                if (AI.Nearest(duck.position, list1) is Gun gun)
                {
                    _target = gun;
                    ai.SetTarget(gun.position);
                }
                else
                {
                    List<Thing> list2 = Level.current.things[typeof(ItemBox)].Where(x => !(x as ItemBox)._hit).ToList();
                    if (!(AI.Nearest(duck.position, list2) is ItemBox itemBox))
                        return new AIStateWait(Rando.Float(0.8f, 1f));
                    _target = itemBox;
                    ai.SetTarget(itemBox.position + new Vec2(0f, 32f));
                }
            }
            else if (_target is ItemBox)
            {
                if (Math.Abs(_target.x - duck.x) < 8f)
                {
                    ai.locomotion.Jump(15);
                    return new AIStateWait(Rando.Float(0.8f, 1f));
                }
            }
            else if (_target.owner != null && _target.owner != duck)
                _target = null;
            else if ((_target.position - duck.position).length < 18f)
            {
                ai.Press(Triggers.Grab);
            }
            else
            {
                ++_refresh;
                if (_refresh > 10 && ai.canRefresh)
                {
                    _target = null;
                    ai.canRefresh = false;
                }
            }
            return this;
        }
    }
}
