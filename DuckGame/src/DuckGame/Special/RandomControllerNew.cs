// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomControllerNew
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
            _editorName = "Random Controller";
            editorTooltip = "Allows you to make it so Targets/Goodies appear randomly.";
            Max_Up._tooltip = "How many Targets/Goodies can be active at once.";
            Delay._tooltip = "The delay (in seconds) before new Targets/Goodies popup.";
            Continuous._tooltip = "If true, Targets/Goodies will keep popping up forever. Otherwise, each will appear only once.";
            Group._tooltip = "Which sequence group this controller activates.";
            Ordered_Groups._tooltip = "If true, Targets/Goodie Order Groups will run sequentially. Otherwise groups will appear randomly.";
            Group_Wait._tooltip = "If true, each Target/Goodie Order Group will wait until the previous group is finished before appearing.";
            Type._tooltip = "Selects which sort of Sequence this controller activates.";
        }

        public override void Initialize()
        {
            _originalMaxUp = Max_Up.value;
            if (Group.value > 0)
                _sequenceNumber = Group.value;
            base.Initialize();
        }

        public override void Update()
        {
            if (_finished || !isServerForObject || !Level.current.simulatePhysics)
                return;
            if (!_started)
            {
                if (!Ordered_Groups.value)
                {
                    List<Thing> list = Level.current.things[typeof(ISequenceItem)].ToList();
                    if (list.Count > 0)
                        _sequenceNumber = list[ChallengeRando.Int(list.Count - 1)].sequence.order;
                }
                PopUpItems();
                _started = true;
            }
            if (_up.Count > 0)
            {
                for (int index = 0; index < _up.Count; ++index)
                {
                    if (_up[index].finished)
                    {
                        _lastUp = _up[index];
                        _up[index].Reset();
                        _up.Remove(_up[index]);
                        if (!Continuous.value)
                            _lastUp.isValid = false;
                        else if (!_hadFutureItems)
                            _lastUp.timesActivated = 0;
                        --index;
                    }
                }
            }
            if (Group_Wait.value && _up.Count != 0 || _up.Count >= (int)Max_Up && (int)Max_Up != 0)
                return;
            _waitCount += Maths.IncFrameTimer();
            if (_waitCount < Delay.value)
                return;
            PopUpItems();
            _waitCount = 0f;
        }

        private void PopUpItems()
        {
            bool flag1 = false;
            int num1 = Max_Up.value;
            if (num1 == 0)
                num1 = 9999;
            while (_up.Count < num1)
            {
                List<Thing> source1 = Group.value >= 0 ? (Type.value != SequenceItemType.ALL ? Level.current.things[typeof(ISequenceItem)].Where(x => x.sequence.type == Type.value && x.sequence.order == Group.value).ToList() : Level.current.things[typeof(ISequenceItem)].Where(x => x.sequence.order == Group.value).ToList()) : (Type.value != SequenceItemType.ALL ? Level.current.things[typeof(ISequenceItem)].Where(x => x.sequence.type == Type.value).ToList() : Level.current.things[typeof(ISequenceItem)].ToList());
                IEnumerable<Thing> source2;
                while (true)
                {
                    IEnumerable<Thing> source3 = source1.Where(v => v.sequence.isValid && v.sequence.order != _sequenceNumber && v.sequence.timesActivated <= _activationCycle);
                    source2 = source1.Where(v => v.sequence.isValid && v.sequence.order == _sequenceNumber);
                    if (source3.Count() > 0)
                        _hadFutureItems = true;
                    if (_hadFutureItems)
                    {
                        source2 = source2.Where(v => v.sequence.timesActivated == _activationCycle);
                        if (source2.Count() == 0)
                        {
                            if (!Ordered_Groups.value)
                            {
                                IEnumerable<Thing> source4 = source1.Where(x => x.sequence.order != _sequenceNumber);
                                if (!Continuous.value)
                                {
                                    source4 = source4.Where(x => x.sequence.timesActivated <= _activationCycle);
                                }
                                else
                                {
                                    foreach (Thing thing in source4)
                                        thing.sequence.timesActivated = 0;
                                }
                                if (source4.Count() > 0)
                                {
                                    _sequenceNumber = source4.OrderBy(x => x.sequence.likelyhood + ChallengeRando.Int(8)).ElementAt(0).sequence.order;
                                    Max_Up.value = _originalMaxUp;
                                    continue;
                                }
                            }
                            if (source3.Count() == 0)
                            {
                                if (Continuous.value)
                                {
                                    if (Group.value < 0)
                                        _sequenceNumber = 0;
                                    ++_activationCycle;
                                    Max_Up.value = _originalMaxUp;
                                }
                                else
                                    break;
                            }
                            else
                            {
                                if (Group.value < 0)
                                    ++_sequenceNumber;
                                Max_Up.value = _originalMaxUp;
                            }
                        }
                        else
                            goto label_29;
                    }
                    else
                        goto label_31;
                }
                _finished = true;
                break;
            label_29:
                if (Group_Wait.value && _up.Count == 0)
                    Max_Up.value = source2.Count();
                label_31:
                int num2 = 0;
                List<SequenceItem> sequenceItemList = new List<SequenceItem>();
                List<Thing> list = source2.ToList();
                bool flag2 = false;
                while (list.Count() > 0)
                {
                    Thing thing = list[ChallengeRando.Int(0, list.Count - 1)];
                    list.Remove(thing);
                    SequenceItem sequence = thing.sequence;
                    if ((!sequence.activated || sequence.finished) && sequence.isValid)
                    {
                        flag2 = true;
                        if (sequence != _lastUp || flag1)
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
                    if (num3 > num4 && num3 < num4 + num5)
                    {
                        sequenceItem.randomMode = true;
                        sequenceItem.Activate();
                        ++_totalUp;
                        flag3 = true;
                        _up.Add(sequenceItem);
                        break;
                    }
                    num4 += num5;
                }
                if (flag3 && Max_Up.value == 0)
                    break;
            }
        }
    }
}
