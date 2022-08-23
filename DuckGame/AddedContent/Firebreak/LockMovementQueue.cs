using System.Collections.Generic;

namespace DuckGame;

public static class LockMovementQueue
{
    public static List<string> LockedMovementQueue = new();

    public static bool Add(string id)
    {
        if (!LockedMovementQueue.Contains(id) && !DevConsole.open && !DuckNetwork.enteringText && !Editor.enteringText)
            LockedMovementQueue.Add(id);
        else return false;

        return true;
    }

    public static bool Remove(string id)
    {
        if (LockedMovementQueue.Contains(id))
            LockedMovementQueue.Remove(id);
        else return false;

        return true;
    }

    public static bool Empty => LockedMovementQueue.Count == 0;
}