//// Decompiled with JetBrains decompiler
//// Type: DuckGame.AIStatePickHat
//// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//using System.Collections.Generic;
//using System.Linq;

//namespace DuckGame
//{
//    public class AIStatePickHat : AIState
//    {
//        private Thing _target;
//        private float _wait = Rando.Float(0.5f);
//        private float _wait2 = Rando.Float(0.5f);
//        private float _wait3 = Rando.Float(0.5f);
//        private float _wait4 = Rando.Float(0.5f);
//        private bool _did1;
//        private bool _did2;
//        private bool _did3;
//        private int _moveUp = Rando.Int(0, 8);
//        private int _moveLeft = Rando.Int(1, 8);

//        public override AIState Update(Duck duck, DuckAI ai)
//        {
//            if (this._target == null)
//            {
//                List<Thing> list = Level.current.things[typeof(HatConsole)].ToList<Thing>();
//                if (!(AI.Nearest(duck.position, list) is HatConsole hatConsole))
//                    return new AIStateWait(Rando.Float(0.8f, 1f));
//                this._target = hatConsole;
//                ai.SetTarget(hatConsole.position);
//                return this;
//            }
//            if ((this._target.position - duck.position).length < 10f && duck.grounded)
//            {
//                this._wait -= 0.016f;
//                if (_wait <= 0f)
//                {
//                    if (!this._did1 || !(this._target as HatConsole).box._hatSelector.open)
//                    {
//                        ai.Press("SHOOT");
//                        this._did1 = true;
//                    }
//                    this._wait2 -= 0.016f;
//                    if (_wait2 <= 0f && (this._target as HatConsole).box._hatSelector.open)
//                    {
//                        if (!this._did2)
//                        {
//                            ai.Press("JUMP");
//                            this._did2 = true;
//                        }
//                        this._wait3 -= 0.016f;
//                        if (_wait3 <= 0f)
//                        {
//                            this._wait3 = Rando.Float(0.2f);
//                            if (Rando.Float(1f) > 0.5f)
//                            {
//                                if (this._moveLeft > 0)
//                                {
//                                    ai.Press("LEFT");
//                                    --this._moveLeft;
//                                }
//                                else if (this._moveUp > 0)
//                                {
//                                    ai.Press("UP");
//                                    --this._moveUp;
//                                }
//                            }
//                            else if (this._moveUp > 0)
//                            {
//                                ai.Press("UP");
//                                --this._moveUp;
//                            }
//                            else if (this._moveLeft > 0)
//                            {
//                                ai.Press("LEFT");
//                                --this._moveLeft;
//                            }
//                            if (this._moveLeft == 0 && this._moveUp == 0)
//                            {
//                                if (!this._did3)
//                                {
//                                    ai.Press("JUMP");
//                                    this._did3 = true;
//                                }
//                                this._wait4 -= 0.016f;
//                                if (_wait4 <= 0.0)
//                                {
//                                    ai.Press("QUACK");
//                                    return new AIStateWait(Rando.Float(0.8f, 1f));
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return this;
//        }
//    }
//}
