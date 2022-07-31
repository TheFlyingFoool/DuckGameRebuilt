// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomControllerNew
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Arcade|Targets", EditorItemType.ArcadeNew)]
    public class RandomControllerNew : Thing
    {
        public EditorProperty<int> Max_Up = new EditorProperty<int>(1, max: 32f, increment: 1f, minSpecial: "NO LIMIT");
        public EditorProperty<float> Delay = new EditorProperty<float>(0f, max: 100f, increment: 0.05f);
        public EditorProperty<bool> Continuous = new EditorProperty<bool>(true);
        public EditorProperty<int> Group = new EditorProperty<int>(0, min: -1f, max: 99f, increment: 1f, minSpecial: "ALL");
        public EditorProperty<bool> Ordered_Groups = new EditorProperty<bool>(false, "GROUP");
        public EditorProperty<bool> Group_Wait = new EditorProperty<bool>(false, "GROUP");
        public EditorProperty<SequenceItemType> Type = new EditorProperty<SequenceItemType>(SequenceItemType.Activator, "TYPE");
        private int _originalMaxUp;
        private SequenceItem _lastUp;
        private bool _started;
        private float _waitCount;
        private int _totalUp;
        private List<SequenceItem> _up = new List<SequenceItem>();
        private int _sequenceNumber;
        private bool _hadFutureItems;
        private int _activationCycle;
        private bool _finished;
        private HashSet<int> _processedSequences = new HashSet<int>();

        public RandomControllerNew()
          : base()
        {
            this.graphic = new Sprite("swirl");
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this._canFlip = false;
            this._visibleInGame = false;
            this._editorName = "Random Controller";
            this.editorTooltip = "Allows you to make it so Targets/Goodies appear randomly.";
            this.Max_Up._tooltip = "How many Targets/Goodies can be active at once.";
            this.Delay._tooltip = "The delay (in seconds) before new Targets/Goodies popup.";
            this.Continuous._tooltip = "If true, Targets/Goodies will keep popping up forever. Otherwise, each will appear only once.";
            this.Group._tooltip = "Which sequence group this controller activates.";
            this.Ordered_Groups._tooltip = "If true, Targets/Goodie Order Groups will run sequentially. Otherwise groups will appear randomly.";
            this.Group_Wait._tooltip = "If true, each Target/Goodie Order Group will wait until the previous group is finished before appearing.";
            this.Type._tooltip = "Selects which sort of Sequence this controller activates.";
        }

        public override void Initialize()
        {
            this._originalMaxUp = this.Max_Up.value;
            if (this.Group.value > 0)
                this._sequenceNumber = this.Group.value;
            base.Initialize();
        }

        public override void Update()
        {
            if (this._finished || !this.isServerForObject || !Level.current.simulatePhysics)
                return;
            if (!this._started)
            {
                if (!this.Ordered_Groups.value)
                {
                    List<Thing> list = Level.current.things[typeof(ISequenceItem)].ToList<Thing>();
                    if (list.Count > 0)
                        this._sequenceNumber = list[ChallengeRando.Int(list.Count - 1)].sequence.order;
                }
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
                        if (!this.Continuous.value)
                            this._lastUp.isValid = false;
                        else if (!this._hadFutureItems)
                            this._lastUp.timesActivated = 0;
                        --index;
                    }
                }
            }
            if (this.Group_Wait.value && this._up.Count != 0 || this._up.Count >= (int)this.Max_Up && (int)this.Max_Up != 0)
                return;
            this._waitCount += Maths.IncFrameTimer();
            if (_waitCount < (double)this.Delay.value)
                return;
            this.PopUpItems();
            this._waitCount = 0f;
        }

        private void PopUpItems()
        {
            bool flag1 = false;
            int num1 = this.Max_Up.value;
            if (num1 == 0)
                num1 = 9999;
            while (this._up.Count < num1)
            {
                List<Thing> source1 = this.Group.value >= 0 ? (this.Type.value != SequenceItemType.ALL ? Level.current.things[typeof(ISequenceItem)].Where<Thing>(x => x.sequence.type == this.Type.value && x.sequence.order == this.Group.value).ToList<Thing>() : Level.current.things[typeof(ISequenceItem)].Where<Thing>(x => x.sequence.order == this.Group.value).ToList<Thing>()) : (this.Type.value != SequenceItemType.ALL ? Level.current.things[typeof(ISequenceItem)].Where<Thing>(x => x.sequence.type == this.Type.value).ToList<Thing>() : Level.current.things[typeof(ISequenceItem)].ToList<Thing>());
                IEnumerable<Thing> source2;
                while (true)
                {
                    IEnumerable<Thing> source3 = source1.Where<Thing>(v => v.sequence.isValid && v.sequence.order != this._sequenceNumber && v.sequence.timesActivated <= this._activationCycle);
                    source2 = source1.Where<Thing>(v => v.sequence.isValid && v.sequence.order == this._sequenceNumber);
                    if (source3.Count<Thing>() > 0)
                        this._hadFutureItems = true;
                    if (this._hadFutureItems)
                    {
                        source2 = source2.Where<Thing>(v => v.sequence.timesActivated == this._activationCycle);
                        if (source2.Count<Thing>() == 0)
                        {
                            if (!this.Ordered_Groups.value)
                            {
                                IEnumerable<Thing> source4 = source1.Where<Thing>(x => x.sequence.order != this._sequenceNumber);
                                if (!this.Continuous.value)
                                {
                                    source4 = source4.Where<Thing>(x => x.sequence.timesActivated <= this._activationCycle);
                                }
                                else
                                {
                                    foreach (Thing thing in source4)
                                        thing.sequence.timesActivated = 0;
                                }
                                if (source4.Count<Thing>() > 0)
                                {
                                    this._sequenceNumber = source4.OrderBy<Thing, int>(x => x.sequence.likelyhood + ChallengeRando.Int(8)).ElementAt<Thing>(0).sequence.order;
                                    this.Max_Up.value = this._originalMaxUp;
                                    continue;
                                }
                            }
                            if (source3.Count<Thing>() == 0)
                            {
                                if (this.Continuous.value)
                                {
                                    if (this.Group.value < 0)
                                        this._sequenceNumber = 0;
                                    ++this._activationCycle;
                                    this.Max_Up.value = this._originalMaxUp;
                                }
                                else
                                    break;
                            }
                            else
                            {
                                if (this.Group.value < 0)
                                    ++this._sequenceNumber;
                                this.Max_Up.value = this._originalMaxUp;
                            }
                        }
                        else
                            goto label_29;
                    }
                    else
                        goto label_31;
                }
                this._finished = true;
                break;
            label_29:
                if (this.Group_Wait.value && this._up.Count == 0)
                    this.Max_Up.value = source2.Count<Thing>();
                label_31:
                int num2 = 0;
                List<SequenceItem> sequenceItemList = new List<SequenceItem>();
                List<Thing> list = source2.ToList<Thing>();
                bool flag2 = false;
                while (list.Count<Thing>() > 0)
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
                            num2 += sequence.likelyhood;
                            sequenceItemList.Add(sequence);
                        }
                    }
                }
                if (!flag2)
                    break;
                if (num2 == 0)
                    num2 = 1;
                float num3 = ChallengeRando.Float(1f);
                float num4 = 0f;
                if (sequenceItemList.Count == 0)
                    flag1 = true;
                bool flag3 = false;
                foreach (SequenceItem sequenceItem in sequenceItemList)
                {
                    float num5 = sequenceItem.likelyhood / (float)num2;
                    if ((double)num3 > (double)num4 && (double)num3 < (double)num4 + (double)num5)
                    {
                        sequenceItem.randomMode = true;
                        sequenceItem.Activate();
                        ++this._totalUp;
                        flag3 = true;
                        this._up.Add(sequenceItem);
                        break;
                    }
                    num4 += num5;
                }
                if (flag3 && this.Max_Up.value == 0)
                    break;
            }
        }
    }
}
