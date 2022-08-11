using System;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static void Test()
    {
        RoomEditorExtra.favoriteHats.Add(1);
        RoomEditorExtra.favoriteHats.Add(9);
        RoomEditorExtra.favoriteHats.Add(8);
        RoomEditorExtra.favoriteHats.Add(4);

        BitBuffer b = new BitBuffer(RoomEditorExtra.room1.ToArray());
        b.Write(255);
        b.Write(false);
        b.Write("E");
        RoomEditorExtra.room1 = b.buffer.ToList();

        AutoConfigHandler.SaveAll(false);
        DevConsole.Log("yay");
    }
}
