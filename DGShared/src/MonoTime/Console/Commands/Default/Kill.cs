namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Kills a player")]
        public static void Kill(Duck duck)
        {
            if (!DevConsole.CheckCheats())
            {
                duck.Kill(new DTIncinerate(null));
            }
            else
            {
                DevConsole.Log("You can't do that here!", Color.Red);
            }
        }
    }
}