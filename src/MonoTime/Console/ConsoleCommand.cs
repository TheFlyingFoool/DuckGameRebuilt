// Decompiled with JetBrains decompiler
// Type: DuckGame.ConsoleCommand
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ConsoleCommand
    {
        private string _command;

        public ConsoleCommand(string command) => this._command = command;

        public string NextWord(bool toLower = true, bool peek = false)
        {
            int num = 0;
            if (this._command.Length <= 0)
                return "";
            while (this._command[num] == ' ')
            {
                ++num;
                if (num >= this._command.Length)
                    return "";
            }
            int startIndex = num;
            while (this._command[num] != ' ')
            {
                ++num;
                if (num >= this._command.Length)
                    break;
            }
            string str = this._command.Substring(startIndex, num - startIndex);
            if (!peek)
                this._command = this._command.Substring(num, this._command.Length - num);
            return !toLower ? str : str.ToLower();
        }

        public string Remainder(bool toLower = true) => (toLower ? this._command.ToLower() : this._command).Trim();
    }
}
