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
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                _prevEnabled = _enabled;
                _enabled = value;
            }
        }

        public bool prevEnabled
        {
            get => _prevEnabled;
            set => _prevEnabled = value;
        }

        public bool filtered
        {
            get => _filtered;
            set => _filtered = value;
        }

        public bool onlineEnabled
        {
            get => _onlineEnabled;
            set => _onlineEnabled = value;
        }

        public string name
        {
            get => _name;
            set => _name = value;
        }

        public string shortName
        {
            get => _shortName != null ? _shortName : name;
            set => _shortName = value;
        }

        public string id
        {
            get => _id;
            set => _id = value;
        }

        public int icon
        {
            get => _icon;
            set => _icon = value;
        }

        public int cost
        {
            get => _cost;
            set => _cost = value;
        }

        public string description
        {
            get => _description;
            set => _description = value;
        }

        public string longDescription
        {
            get => _longDescription;
            set => _longDescription = value;
        }

        public UnlockType type
        {
            get => _type;
            set => _type = value;
        }

        public UnlockPrice priceTier
        {
            get => _priceTier;
            set => _priceTier = value;
        }

        public string GetNameForDisplay() => name.ToUpperInvariant();

        public string GetShortNameForDisplay() => shortName.ToUpperInvariant();

        public bool unlocked
        {
            get
            {
                if (_unlocked || FireDebug.Debugging || DGRSettings.TemporaryUnlockAll)
                    return true;
                foreach (Profile universalProfile in Profiles.universalProfileList)
                {
                    if (universalProfile.unlocks.Contains(_id))
                        return true;
                }
                return false;
            }
            set => _unlocked = value;
        }

        public bool ProfileUnlocked(Profile p)
        {
            return p.unlocks.Contains(_id) || FireDebug.Debugging;
        }

        public List<UnlockData> children => _children;

        public UnlockData parent
        {
            get => _parent;
            set => _parent = value;
        }

        public int layer
        {
            get => _layer;
            set => _layer = value;
        }

        public void AddChild(UnlockData child)
        {
            children.Add(child);
            child.parent = this;
            child.layer = layer + 1;
        }

        public bool AllParentsUnlocked(Profile who)
        {
            if (parent == null)
                return true;
            foreach (UnlockData unlockData in Unlocks.GetTreeLayer(parent.layer))
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
