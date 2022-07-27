// Decompiled with JetBrains decompiler
// Type: DuckGame.TargetDuckNew
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Arcade|Targets", EditorItemType.ArcadeNew)]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class TargetDuckNew : TargetDuck
    {
        protected bool _holdAction;
        public EditorProperty<int> Order = new EditorProperty<int>(0, min: -1f, max: 256f, increment: 1f, minSpecial: "RANDOM");
        public EditorProperty<int> AutoFire = new EditorProperty<int>(-1, min: -1f, max: 240f, increment: 2f, minSpecial: "INF");
        public EditorProperty<int> FireSpeed = new EditorProperty<int>(60, min: -1f, max: 240f, increment: 2f);
        public EditorProperty<int> ReloadSpeed = new EditorProperty<int>(60, min: -1f, max: 240f, increment: 2f);
        private float _reloadAdd;
        //private float _targetLerpOut = 1f;

        public override bool action => this._holdAction;

        public TargetDuckNew(float pX, float pY, TargetStance pStance)
          : base(pX, pY, pStance)
        {
            this.Order._tooltip = "All Targets/Goodies with smaller Order numbers must be destroyed/collected before this target appears.";
            this.AutoFire._tooltip = "How long the target waits (in frames) before auto firing it's weapon.";
            this.FireSpeed._tooltip = "How long the target waits (in frames) before firing it's weapon once it sees a target.";
            this.ReloadSpeed._tooltip = "How long the target waits (in frames) firing again once it's already fired.";
            this._editorName = "Target Duck";
            this._contextMenuFilter.Add("autofire");
            this._contextMenuFilter.Add("time");
            this._contextMenuFilter.Add("random");
            this._contextMenuFilter.Add("maxrandom");
            this._contextMenuFilter.Add("dropgun");
            this._contextMenuFilter.Add("speediness");
            this._contextMenuFilter.Add("Sequence");
            this.sequence._resetLikelyhood = false;
        }

        public override void OnSequenceActivate()
        {
            this._popup = true;
            this._waitFire = this.autofire.value;
            this._reloadAdd = 0.0f;
            this._holdAction = false;
        }

        public override void Initialize()
        {
            this.sequence.order = this.Order.value;
            base.Initialize();
            this.autofire = (EditorProperty<float>)((float)this.AutoFire.value * Maths.IncFrameTimer());
            this.speediness = (EditorProperty<float>)((float)this.FireSpeed.value * Maths.IncFrameTimer());
        }

        public override void UpdateFire()
        {
            if (!Level.current.simulatePhysics)
                return;
            Gun holdObject = this.holdObject as Gun;
            float num = 300f;
            if (holdObject.ammoType != null)
                num = holdObject.ammoType.range;
            Vec2 vec2 = this.holdObject.Offset(new Vec2(num * this.holdObject.angleMul, 0.0f));
            if ((double)this._waitFire <= 0.0)
            {
                foreach (Duck duck in Level.current.things[typeof(Duck)].Where<Thing>((Func<Thing, bool>)(d => !(d is TargetDuck))))
                {
                    if (Collision.Line(this.holdObject.position + new Vec2(0.0f, -5f), vec2 + new Vec2(0.0f, -5f), duck.rectangle) || Collision.Line(this.holdObject.position + new Vec2(0.0f, 5f), vec2 + new Vec2(0.0f, 5f), duck.rectangle))
                    {
                        IEnumerable<Block> blocks = Level.CheckLineAll<Block>(this.holdObject.position, duck.position);
                        bool flag = false;
                        foreach (Block block in blocks)
                        {
                            if (!(block is Window))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            this._waitFire = Math.Max((float)this.speediness + this._reloadAdd, 0.0f);
                            break;
                        }
                        break;
                    }
                }
            }
            this._holdAction = false;
            if ((double)this._waitFire < 0.0)
                return;
            this._waitFire -= Maths.IncFrameTimer();
            if ((double)this._waitFire > 0.0)
                return;
            holdObject.PressAction();
            this._holdAction = true;
            this._reloadAdd = (float)this.ReloadSpeed.value * Maths.IncFrameTimer();
        }

        public override void Draw()
        {
            if (this.holdObject is Gun && (this.holdObject as Gun).ammoType != null && (double)this._waitFire < 1.0 && (double)this._waitFire > 0.0)
            {
                float num = this._waitFire * this._waitFire;
                Vec2 barrelPosition = (this.holdObject as Gun).barrelPosition;
                Vec2 p1_1 = barrelPosition + new Vec2(0.0f, (float)(-(double)num * 64.0));
                Vec2 p1_2 = barrelPosition + new Vec2(0.0f, num * 64f);
                float amount = (float)(1.0 - (double)Math.Min(this._waitFire, 0.08f) / 0.0799999982118607);
                Color color = Lerp.ColorSmooth(Color.White, Color.Red, amount);
                Graphics.DrawLine(p1_1, p1_1 + new Vec2((this.holdObject as Gun).ammoType.range * (float)this.offDir, 0.0f), color * Math.Max((float)(1.0 - (double)this._waitFire - 0.5), 0.0f), 1f + amount, (Depth)0.99f);
                Graphics.DrawLine(p1_2, p1_2 + new Vec2((this.holdObject as Gun).ammoType.range * (float)this.offDir, 0.0f), color * Math.Max((float)(1.0 - (double)this._waitFire - 0.5), 0.0f), 1f + amount, (Depth)0.99f);
            }
            base.Draw();
        }
    }
}
