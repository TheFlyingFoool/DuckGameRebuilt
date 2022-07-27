// Decompiled with JetBrains decompiler
// Type: DuckGame.UnlockData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class UnlockData
    {
        private bool _enabled;
        private bool _prevEnabled;
        private bool _filtered = true;
        private bool _onlineEnabled = true;
        private string _name;
        private string _shortName;
        private string _id;
        private int _icon = -1;
        private int _cost;
        private string _description;
        private string _longDescription = "";
        private UnlockType _type;
        private UnlockPrice _priceTier;
        private bool _unlocked;
        private List<UnlockData> _children = new List<UnlockData>();
        private UnlockData _parent;
        private int _layer;

        public bool enabled
        {
            get => this._enabled;
            set
            {
                if (this._enabled == value)
                    return;
                this._prevEnabled = this._enabled;
                this._enabled = value;
            }
        }

        public bool prevEnabled
        {
            get => this._prevEnabled;
            set => this._prevEnabled = value;
        }

        public bool filtered
        {
            get => this._filtered;
            set => this._filtered = value;
        }

        public bool onlineEnabled
        {
            get => this._onlineEnabled;
            set => this._onlineEnabled = value;
        }

        public string name
        {
            get => this._name;
            set => this._name = value;
        }

        public string shortName
        {
            get => this._shortName != null ? this._shortName : this.name;
            set => this._shortName = value;
        }

        public string id
        {
            get => this._id;
            set => this._id = value;
        }

        public int icon
        {
            get => this._icon;
            set => this._icon = value;
        }

        public int cost
        {
            get => this._cost;
            set => this._cost = value;
        }

        public string description
        {
            get => this._description;
            set => this._description = value;
        }

        public string longDescription
        {
            get => this._longDescription;
            set => this._longDescription = value;
        }

        public UnlockType type
        {
            get => this._type;
            set => this._type = value;
        }

        public UnlockPrice priceTier
        {
            get => this._priceTier;
            set => this._priceTier = value;
        }

        public string GetNameForDisplay() => this.name.ToUpperInvariant();

        public string GetShortNameForDisplay() => this.shortName.ToUpperInvariant();

        public bool unlocked
        {
            get
            {
                if (this._unlocked)
                    return true;
                foreach (Profile universalProfile in Profiles.universalProfileList)
                {
                    if (universalProfile.unlocks.Contains(this._id))
                        return true;
                }
                return false;
            }
            set => this._unlocked = value;
        }

        public bool ProfileUnlocked(Profile p) => p.unlocks.Contains(this._id);

        public List<UnlockData> children => this._children;

        public UnlockData parent
        {
            get => this._parent;
            set => this._parent = value;
        }

        public int layer
        {
            get => this._layer;
            set => this._layer = value;
        }

        public void AddChild(UnlockData child)
        {
            this.children.Add(child);
            child.parent = this;
            child.layer = this.layer + 1;
        }

        public bool AllParentsUnlocked(Profile who)
        {
            if (this.parent == null)
                return true;
            foreach (UnlockData unlockData in Unlocks.GetTreeLayer(this.parent.layer))
            {
                if (unlockData.children.Contains(this) && !unlockData.ProfileUnlocked(who))
                    return false;
            }
            return true;
        }

        public UnlockData GetUnlockedParent()
        {
            for (UnlockData unlockedParent = this; unlockedParent != null; unlockedParent = unlockedParent.parent)
            {
                if (unlockedParent.parent == null || unlockedParent.parent.unlocked)
                    return unlockedParent;
            }
            return null;
        }
    }
}
