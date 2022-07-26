// Decompiled with JetBrains decompiler
// Type: DuckGame.SequenceItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class SequenceItem
    {
        public static List<SequenceItem> sequenceItems = new List<SequenceItem>();
        public int order;
        private bool _finished;
        private bool _activated;
        private Thing _thing;
        private SequenceItemType _type;
        private bool _loop;
        public bool waitTillOrder;
        public bool isValid = true;
        public int timesActivated;
        public int likelyhood;
        public bool randomMode;
        public bool _resetLikelyhood = true;

        public bool finished => this._finished;

        public bool activated => this._activated;

        public SequenceItemType type
        {
            get => this._type;
            set => this._type = value;
        }

        public bool loop
        {
            get => this._loop;
            set => this._loop = value;
        }

        public SequenceItem(Thing t) => this._thing = t;

        public virtual void Finished()
        {
            this._finished = true;
            if (this.order < 0)
                return;
            this.CheckSequence();
        }

        public void Reset()
        {
            this._activated = false;
            this._finished = false;
        }

        public void BeginRandomSequence()
        {
            List<int> intList = new List<int>();
            foreach (ISequenceItem sequenceItem in Level.current.things[typeof(ISequenceItem)])
            {
                SequenceItem sequence = (sequenceItem as Thing).sequence;
                sequence._finished = false;
                sequence._activated = false;
                if (sequence.order != this.order && !intList.Contains(sequence.order))
                    intList.Add(sequence.order);
            }
            if (intList.Count == 0)
                intList.Add(this.order);
            int num = Rando.ChooseInt(intList.ToArray());
            foreach (ISequenceItem sequenceItem in Level.current.things[typeof(ISequenceItem)])
            {
                SequenceItem sequence = (sequenceItem as Thing).sequence;
                if (sequence.order == num)
                    sequence.Activate();
            }
        }

        private bool SequenceFinished()
        {
            foreach (ISequenceItem sequenceItem in Level.current.things[typeof(ISequenceItem)])
            {
                SequenceItem sequence = (sequenceItem as Thing).sequence;
                if (sequence.order == this.order && !sequence._finished)
                    return false;
            }
            return true;
        }

        private void CheckSequence()
        {
            if (this.randomMode)
                return;
            List<SequenceItem> sequenceItemList = new List<SequenceItem>();
            int num1 = 9999999;
            int num2 = this.order;
            if (this.loop && this.SequenceFinished())
            {
                num2 = -1;
                foreach (ISequenceItem sequenceItem in Level.current.things[typeof(ISequenceItem)])
                {
                    SequenceItem sequence = (sequenceItem as Thing).sequence;
                    sequence._activated = false;
                    sequence._finished = false;
                }
            }
            bool flag = false;
            foreach (ISequenceItem sequenceItem in Level.current.things[typeof(ISequenceItem)])
            {
                SequenceItem sequence = (sequenceItem as Thing).sequence;
                if ((sequence != this || this.loop) && (!(sequenceItem is Window) && !(sequenceItem is Door) || sequence.isValid))
                {
                    if (!sequence._activated && sequence.order > num2)
                    {
                        if (sequence.order == num1)
                            sequenceItemList.Add(sequence);
                        else if (sequence.order < num1)
                        {
                            sequenceItemList.Clear();
                            sequenceItemList.Add(sequence);
                            num1 = sequence.order;
                        }
                    }
                    if (sequence.order == num2 && !sequence._finished)
                    {
                        sequenceItemList.Clear();
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag && ChallengeLevel.random)
            {
                this.BeginRandomSequence();
            }
            else
            {
                foreach (SequenceItem sequenceItem in sequenceItemList)
                    sequenceItem.Activate();
            }
        }

        public static bool IsFinished()
        {
            bool flag = true;
            foreach (ISequenceItem sequenceItem in Level.current.things[typeof(ISequenceItem)])
            {
                SequenceItem sequence = (sequenceItem as Thing).sequence;
                if (sequence != null && !sequence._finished && sequence.isValid)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public static bool IsFinished(SequenceItemType tp)
        {
            bool flag = true;
            foreach (ISequenceItem sequenceItem in Level.current.things[typeof(ISequenceItem)])
            {
                SequenceItem sequence = (sequenceItem as Thing).sequence;
                if (sequence != null && sequence.type == tp && !sequence._finished && sequence.isValid)
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public void Activate()
        {
            if (this._activated)
                return;
            if (this._resetLikelyhood)
                this.likelyhood = 0;
            this._activated = true;
            this._thing.OnSequenceActivate();
            this.OnActivate();
            ++this.timesActivated;
        }

        public virtual void OnActivate()
        {
        }
    }
}
