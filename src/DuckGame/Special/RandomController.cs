// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomController
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class RandomController : Thing
    {
        public EditorProperty<int> max_up = new EditorProperty<int>(1, min: 1f, max: 32f, increment: 1f);
        public EditorProperty<float> wait = new EditorProperty<float>(0.0f, max: 100f);
        private float _waitCount;
        private bool _started;
        private int _totalUp;
        private List<SequenceItem> _up = new List<SequenceItem>();
        private SequenceItem _lastUp;
        public static bool isRand;

        public RandomController()
          : base()
        {
            this.graphic = new Sprite("swirl");
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this._canFlip = false;
            this._visibleInGame = false;
        }

        public override void Update()
        {
            if (!Level.current.simulatePhysics)
                return;
            if (!this._started)
            {
                this.PopUpItems();
                this._started = true;
            }
            if (this._up.Count > 0)
            {
                for (int index = 0; index < this._up.Count; ++index)
                {
                    if (this._up[index].finished)
                    {
                        this._lastUp = this._up[index];
                        this._up[index].Reset();
                        this._up.Remove(this._up[index]);
                        --index;
                    }
                }
            }
            if (this._up.Count >= (int)this.max_up)
                return;
            if (this._up.Count == 0)
            {
                this.PopUpItems();
                this._waitCount = 0.0f;
            }
            else
            {
                this._waitCount += Maths.IncFrameTimer();
                if ((double)this._waitCount < (double)this.wait.value)
                    return;
                this._waitCount = 0.0f;
                this.PopUpItems();
            }
        }

        private void PopUpItems()
        {
            bool flag1 = false;
            while (this._up.Count < this.max_up.value)
            {
                List<Thing> list = Level.current.things[typeof(ISequenceItem)].ToList<Thing>();
                list.RemoveAll((Predicate<Thing>)(v => !v.sequence.isValid));
                if (this._up.Count >= list.Count)
                    break;
                int num1 = 0;
                List<SequenceItem> sequenceItemList = new List<SequenceItem>();
                bool flag2 = false;
                while (list.Count > 0)
                {
                    Thing thing = list[ChallengeRando.Int(0, list.Count - 1)];
                    list.Remove(thing);
                    SequenceItem sequence = thing.sequence;
                    if ((!sequence.activated || sequence.finished) && sequence.isValid)
                    {
                        flag2 = true;
                        if (sequence != this._lastUp || flag1)
                        {
                            ++sequence.likelyhood;
                            num1 += sequence.likelyhood;
                            sequenceItemList.Add(sequence);
                        }
                    }
                }
                if (!flag2)
                    break;
                if (num1 == 0)
                    num1 = 1;
                float num2 = ChallengeRando.Float(1f);
                float num3 = 0.0f;
                if (sequenceItemList.Count == 0)
                    flag1 = true;
                foreach (SequenceItem sequenceItem in sequenceItemList)
                {
                    float num4 = (float)sequenceItem.likelyhood / (float)num1;
                    if ((double)num2 > (double)num3 && (double)num2 < (double)num3 + (double)num4)
                    {
                        sequenceItem.randomMode = true;
                        RandomController.isRand = true;
                        sequenceItem.Activate();
                        RandomController.isRand = false;
                        ++this._totalUp;
                        this._up.Add(sequenceItem);
                        break;
                    }
                    num3 += num4;
                }
            }
        }
    }
}
