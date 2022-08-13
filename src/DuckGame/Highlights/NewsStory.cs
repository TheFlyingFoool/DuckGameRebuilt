// Decompiled with JetBrains decompiler
// Type: DuckGame.NewsStory
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class NewsStory
    {
        protected float _modifier;
        protected float _importance;
        protected float _value;
        protected float _awfulValue;
        protected float _impressiveValue;
        private string _storyName;
        public string name;
        public string name2;
        public string extra01;
        public string extra02;
        public string remark;
        protected NewsSection _section = NewsSection.MatchComments;
        private List<NewsStory> _subStories = new List<NewsStory>();

        public NewsStory FromName(
          string storyName,
          string name1Val = null,
          string name2Val = null,
          string extra01Val = null,
          string extra02Val = null)
        {
            NewsStory newsStory = new NewsStory()
            {
                name = name1Val,
                name2 = name2Val,
                extra01 = extra01Val,
                extra02 = extra02Val,
                _storyName = storyName
            };
            newsStory._importance = newsStory.importance;
            newsStory._section = _section;
            return newsStory;
        }

        public string remarkModifierString
        {
            get
            {
                if (_impressiveValue - _awfulValue == 0f)
                    return "";
                if (badRange > 0.2f)
                    return "Bad";
                return goodRange > 0.2f ? "Good" : "";
            }
        }

        public float importance => _importance;

        public float weight => _impressiveValue - _awfulValue == 0f ? 1f : goodRange + badRange;

        public float goodRange
        {
            get
            {
                float num = Math.Abs(_impressiveValue - _awfulValue);
                return _impressiveValue < _awfulValue ? Maths.Clamp(((_impressiveValue + num / 2f - (_impressiveValue + _value)) * 2f) / num, 0f, 99f) : Maths.Clamp(((_value - num / 2f) / (_impressiveValue - num / 2f)), 0f, 99f);
            }
        }

        public float badRange
        {
            get
            {
                float num = Math.Abs(_impressiveValue - _awfulValue);
                return _impressiveValue < _awfulValue ? Maths.Clamp(((_value - num / 2f) / (_awfulValue - num / 2f)), 0f, 99f) : Maths.Clamp(((_awfulValue + num / 2f - (_awfulValue + _value)) * 2) / num, 0f, 99f);
            }
        }

        public NewsSection section => _section;

        public void AddSubStory(NewsStory story)
        {
            if (story.DoCalculateRemark() == null)
                return;
            _subStories.Add(story);
        }

        public void DoCalculate(List<Team> teams)
        {
            _value = 0f;
            _storyName = null;
            name = null;
            name2 = null;
            extra01 = null;
            extra02 = null;
            remark = null;
            Calculate(teams);
        }

        protected virtual void Calculate(List<Team> teams)
        {
        }

        public string DoCalculateRemark()
        {
            remark = CalculateRemark();
            return remark;
        }

        protected virtual string CalculateRemark() => Dialogue.GetRemark(_storyName != null ? _storyName : GetType().Name + remarkModifierString, name, name2, extra01, extra02);
    }
}
