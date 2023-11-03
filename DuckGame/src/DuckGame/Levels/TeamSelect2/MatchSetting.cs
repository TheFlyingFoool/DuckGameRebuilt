using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class MatchSetting
    {
        public Dictionary<int, int> stepMap;
        public string id = "";
        public string name = "";
        public string suffix = "";
        public string prefix = "";
        public string filterText = Triggers.Any;
        public FilterMode filterMode;
        private object _value;
        public object prevValue;
        public int min;
        public int max = 10;
        public string minString;
        public int step = 1;
        public int altStep;
        public string maxSyncID = "";
        public string minSyncID = "";
        public bool filtered;
        public object defaultValue;
        public bool createOnly;
        public bool filterOnly;
        public List<string> valueStrings;
        public List<string> percentageLinks;
        public Func<bool> condition;

        public object value
        {
            get => _value;
            set
            {
                if (_value == null && value != null)
                    defaultValue = value;
                _value = value;
            }
        }
    }
}
