namespace DuckGame
{
    public class ConsoleCommand
    {
        private string _command;

        public ConsoleCommand(string command) => _command = command;

        public string NextWord(bool toLower = true, bool peek = false)
        {
            int num = 0;
            if (_command.Length <= 0)
                return "";
            while (_command[num] == ' ')
            {
                ++num;
                if (num >= _command.Length)
                    return "";
            }
            int startIndex = num;
            while (_command[num] != ' ')
            {
                ++num;
                if (num >= _command.Length)
                    break;
            }
            string str = _command.Substring(startIndex, num - startIndex);
            if (!peek)
                _command = _command.Substring(num, _command.Length - num);
            return !toLower ? str : str.ToLower();
        }

        public string Remainder(bool toLower = false) => (toLower ? _command.ToLower() : _command).Trim();
    }
}
