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
