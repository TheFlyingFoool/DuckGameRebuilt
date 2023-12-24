using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        private static List<DelayedCommand> s_delayedCommands = new();
        
        [Marker.DevConsoleCommand(Description = "Executes a command after a given delay in frames", To = ImplementTo.DuckShell)]
        public static void Wait(
            [AutoCompl("1", "2", "30", "60", "300", "600")] uint frames,
            [CommandAutoCompl] string commandString)
        {
            s_delayedCommands.Add(new DelayedCommand(frames, commandString));
        }

        [Marker.UpdateContext]
        public static void UpdateDelayedCommands()
        {
            for (int i = 0; i < s_delayedCommands.Count; i++)
            {
                DelayedCommand delayedCommand = s_delayedCommands[i];

                if (delayedCommand.FramesLeft == 0)
                {
                    console.Run(delayedCommand.Command, false);
                    s_delayedCommands.RemoveAt(i--);
                }
                else
                {
                    delayedCommand.FramesLeft--;
                }
            }
        }

        private class DelayedCommand
        {
            public uint FramesLeft;
            public string Command;

            public DelayedCommand(uint framesLeft, string command)
            {
                FramesLeft = framesLeft;
                Command = command;
            }
        }
    }
}