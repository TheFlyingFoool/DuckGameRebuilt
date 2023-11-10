using DuckGame.ConsoleEngine;
using DuckGame.ConsoleInterface;

namespace DuckShell.Manager.Interface.Console
{
    public interface IDSHConsole
    {
        CommandRunner Shell { get; set; }
        bool Active { get; set; }
        void Clear();
        void Run(string command, bool byUser);
        void WriteLine(object o, DSHConsoleLine.Significance significance);
    }
}