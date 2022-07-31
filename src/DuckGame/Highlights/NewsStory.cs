// Decompiled with JetBrains decompiler
// Type: DuckGame.NewsStory
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            newsStory._section = this._section;
            return newsStory;
        }

        public string remarkModifierString
        {
            get
            {
                if (_impressiveValue - this._awfulValue == 0f)
                    return "";
                if (this.badRange > 0.2f)
                    return "Bad";
                return this.goodRange > 0.2f ? "Good" : "";
            }
        }

        public float importance => this._importance;

        public float weight => _impressiveValue - this._awfulValue == 0f ? 1f : this.goodRange + this.badRange;

        public float goodRange
        {
            get
            {
                float num = Math.Abs(this._impressiveValue - this._awfulValue);
                return _impressiveValue < this._awfulValue ? Maths.Clamp(((_impressiveValue + num / 2f - (_impressiveValue + this._value)) * 2f) / num, 0f, 99f) : Maths.Clamp(((_value - num / 2f) / (_impressiveValue - num / 2f)), 0f, 99f);
            }
        }

        public float badRange
        {
            get
            {
                float num = Math.Abs(this._impressiveValue - this._awfulValue);
                return _impressiveValue < this._awfulValue ? Maths.Clamp(((_value - num / 2f) / (_awfulValue - num / 2f)), 0f, 99f) : Maths.Clamp(((_awfulValue + num / 2f - (_awfulValue + this._value)) * 2) / num, 0f, 99f);
            }
        }

        public NewsSection section => this._section;

        public void AddSubStory(NewsStory story)
        {
            if (story.DoCalculateRemark() == null)
                return;
            this._subStories.Add(story);
        }

        public void DoCalculate(List<Team> teams)
        {
            this._value = 0f;
            this._storyName = null;
            this.name = null;
            this.name2 = null;
            this.extra01 = null;
            this.extra02 = null;
            this.remark = null;
            this.Calculate(teams);
        }

        protected virtual void Calculate(List<Team> teams)
        {
        }

        public string DoCalculateRemark()
        {
            this.remark = this.CalculateRemark();
            return this.remark;
        }

        protected virtual string CalculateRemark() => Dialogue.GetRemark(this._storyName != null ? this._storyName : this.GetType().Name + this.remarkModifierString, this.name, this.name2, this.extra01, this.extra02);
    }
}
