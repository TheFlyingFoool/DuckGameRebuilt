// Decompiled with JetBrains decompiler
// Type: DuckGame.DXMLAttribute
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DXMLAttribute
    {
        private string _name = "";
        private string _value = "";

        public string Name => _name;

        public string Value => _value;

        public DXMLAttribute(string varName, string varValue)
        {
            _name = varName;
            _value = varValue;
        }
    }
}
