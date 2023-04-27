using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class AdvancedConfigItem
    {
        public string Name;
        public Type Type;
        public object Value;
        public List<Attribute> Attributes = new();
        public bool IsHeader = false;
    }
}