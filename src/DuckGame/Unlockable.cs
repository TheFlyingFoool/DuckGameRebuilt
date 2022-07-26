// Decompiled with JetBrains decompiler
// Type: DuckGame.Unlockable
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Unlockable
    {
        public bool allowHints;
        public bool _locked = true;
        private string _description;
        private string _name;
        private string _achievement;
        private string _id;
        protected bool _showScreen;
        private Func<bool> _condition;

        public bool locked => this._locked;

        public string description => this._description;

        public string name => this._name;

        public string achievement => this._achievement;

        public string id => this._id;

        public bool showScreen => this._showScreen;

        public Unlockable(
          string identifier,
          Func<bool> condition,
          string nam,
          string desc,
          string achieve = "")
        {
            this._condition = condition;
            this._description = desc;
            this._name = nam;
            this._achievement = achieve;
            this._id = identifier;
        }

        public string GetNameForDisplay() => this.name.ToUpperInvariant();

        public bool CheckCondition() => (!NetworkDebugger.enabled || NetworkDebugger.currentIndex != 1) && this._condition();

        public virtual void Initialize()
        {
        }

        public virtual void DoUnlock()
        {
            this.Unlock();
            this._locked = false;
            if (this._achievement == null || !(this._achievement != ""))
                return;
            Global.GiveAchievement(this._achievement);
        }

        protected virtual void Unlock()
        {
        }

        public virtual void DoLock()
        {
            this.Lock();
            this._locked = true;
        }

        protected virtual void Lock()
        {
        }

        public virtual void Draw(float xpos, float ypos, Depth depth)
        {
        }
    }
}
