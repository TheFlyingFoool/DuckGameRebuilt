using System.Collections.Generic;

namespace DuckGame
{
    public static class LockMovementQueue
    {
        public static List<string> LockedMovementQueue = new();

        public static bool TryAdd(string id)
        {
            if (!LockedMovementQueue.Contains(id) 
                && !DevConsole.open 
                && !DuckNetwork.enteringText 
                && !Editor.enteringText)
                LockedMovementQueue.Add(id);
            else return false;

            return true;
        }

        public static bool TryRemove(string id)
        {
            if (LockedMovementQueue.Contains(id))
                LockedMovementQueue.Remove(id);
            else return false;

            return true;
        }

        public static bool Empty => LockedMovementQueue.Count == 0;
    }
}