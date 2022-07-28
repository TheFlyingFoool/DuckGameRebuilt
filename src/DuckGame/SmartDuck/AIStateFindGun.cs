//// Decompiled with JetBrains decompiler
//// Type: DuckGame.AIStateFindGun
//// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace DuckGame
//{
//    public class AIStateFindGun : AIState
//    {
//        private int _refresh;
//        private Thing _target;

//        public override AIState Update(Duck duck, DuckAI ai)
//        {
//            if (duck.holdObject != null && duck.holdObject is Gun)
//                return null;
//            duck.ThrowItem();
//            if (this._target == null)
//            {
//                List<Thing> list1 = Level.current.things[typeof(Gun)].Where<Thing>(x => (x as Gun).ammo > 0 && (x as Gun).owner == null).ToList<Thing>();
//                if (AI.Nearest(duck.position, list1) is Gun gun)
//                {
//                    this._target = gun;
//                    ai.SetTarget(gun.position);
//                }
//                else
//                {
//                    List<Thing> list2 = Level.current.things[typeof(ItemBox)].Where<Thing>(x => !(x as ItemBox)._hit).ToList<Thing>();
//                    if (!(AI.Nearest(duck.position, list2) is ItemBox itemBox))
//                        return new AIStateWait(Rando.Float(0.8f, 1f));
//                    this._target = itemBox;
//                    ai.SetTarget(itemBox.position + new Vec2(0.0f, 32f));
//                }
//            }
//            else if (this._target is ItemBox)
//            {
//                if ((double)Math.Abs(this._target.x - duck.x) < 8.0)
//                {
//                    ai.locomotion.Jump(15);
//                    return new AIStateWait(Rando.Float(0.8f, 1f));
//                }
//            }
//            else if (this._target.owner != null && this._target.owner != duck)
//                this._target = null;
//            else if ((double)(this._target.position - duck.position).length < 18.0)
//            {
//                ai.Press("GRAB");
//            }
//            else
//            {
//                ++this._refresh;
//                if (this._refresh > 10 && ai.canRefresh)
//                {
//                    this._target = null;
//                    ai.canRefresh = false;
//                }
//            }
//            return this;
//        }
//    }
//}
