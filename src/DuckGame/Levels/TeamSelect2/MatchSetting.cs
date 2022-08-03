// Decompiled with JetBrains decompiler
// Type: DuckGame.MatchSetting
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        public string filterText = "ANY";
        public FilterMode filterMode;
        private object _value;
        public object prevValue;
        public int min;
        public int max = 10;
        public string minString;
        public int step = 1;
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
