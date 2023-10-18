using System;

namespace DuckGame
{
    [global::AddedContent.Firebreak.Marker.FireSerializer]
    public class ConsoleBindSerializeModule : IFireSerializerModule<DevConsoleCommands.ConsoleBind>
    {
        public DevConsoleCommands.ConsoleBind Deserialize(string s)
        {
            int firstCommaIndex = s.IndexOf(',');
            string hotkey = s.Substring(0, firstCommaIndex);
            string command = s.Substring(firstCommaIndex + 1);
            return new DevConsoleCommands.ConsoleBind(hotkey, command);
        }

        public string Serialize(DevConsoleCommands.ConsoleBind obj)
        {
            return $"{obj.hotkey},{obj.command}";
        }

        public bool CanSerialize(Type t)
        {
            return t == typeof(DevConsoleCommands.ConsoleBind);
        }

        string IFireSerializerModule.Serialize(object obj) => Serialize((DevConsoleCommands.ConsoleBind)obj);
        object IFireSerializerModule.Deserialize(string s) => Deserialize(s);
    }
}