// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomController
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class RandomController : Thing
    {
        public EditorProperty<int> max_up = new EditorProperty<int>(1, min: 1f, max: 32f, increment: 1f);
        public EditorProperty<float> wait = new EditorProperty<float>(0f, max: 100f);
        private float _waitCount;
        private bool _started;
        private int _totalUp;
        private List<SequenceItem> _up = new List<SequenceItem>();
        private SequenceItem _lastUp;
        public static bool isRand;

        public RandomController()
          : base()
        {
            graphic = new Sprite("swirl");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _canFlip = false;
            _visibleInGame = false;
        }

        public override void Update()
        {
            if (!Level.current.simulatePhysics)
                return;
            if (!_started)
            {
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
                        --index;
                    }
                }
            }
            if (_up.Count >= (int)max_up)
                return;
            if (_up.Count == 0)
            {
                PopUpItems();
                _waitCount = 0f;
            }
            else
            {
                _waitCount += Maths.IncFrameTimer();
                if (_waitCount < wait.value)
                    return;
                _waitCount = 0f;
                PopUpItems();
            }
        }

        private void PopUpItems()
        {
            bool flag1 = false;
            while (_up.Count < max_up.value)
            {
                List<Thing> list = Level.current.things[typeof(ISequenceItem)].ToList<Thing>();
                list.RemoveAll(v => !v.sequence.isValid);
                if (_up.Count >= list.Count)
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
                        if (sequence != _lastUp || flag1)
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
                float num3 = 0f;
                if (sequenceItemList.Count == 0)
                    flag1 = true;
                foreach (SequenceItem sequenceItem in sequenceItemList)
                {
                    float num4 = sequenceItem.likelyhood / (float)num1;
                    if (num2 > num3 && num2 < num3 + num4)
                    {
                        sequenceItem.randomMode = true;
                        RandomController.isRand = true;
                        sequenceItem.Activate();
                        RandomController.isRand = false;
                        ++_totalUp;
                        _up.Add(sequenceItem);
                        break;
                    }
                    num3 += num4;
                }
            }
        }
    }
}
