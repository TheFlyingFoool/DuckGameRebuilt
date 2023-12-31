using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        private static Dictionary<string, Queue<DelayedCommand>> s_delayedCommandsPipeline = new();
        
        [Marker.DevConsoleCommand(Description = "todo", To = ImplementTo.DuckShell)]
        public static void Queue(
            [AutoCompl("alpha", "beta", "gamma", "delta")] string line,
            [AutoCompl("1", "2", "30", "60", "300", "600")] uint frames,
            [CommandAutoCompl] string commandString)
        {
            s_delayedCommandsPipeline.TryAdd(line, new Queue<DelayedCommand>());
            
            s_delayedCommandsPipeline[line].Enqueue(new DelayedCommand(frames, commandString));
        }
        
        [Marker.UpdateContext]
        private static void UpdatePipleineDelayedCommands()
        {
            List<string> markedForDeletion = new();
            
            foreach ((string line, Queue<DelayedCommand> queue) in s_delayedCommandsPipeline)
            {
                if (queue.Count == 0)
                {
                    markedForDeletion.Add(line);
                    continue;
                }
                
                DelayedCommand current = queue.Peek();
                
                if (current.FramesLeft == 0)
                {
                    console.Run(current.Command, false);
                    queue.Dequeue();
                }
                else
                {
                    current.FramesLeft--;
                    continue;
                }
            }

            foreach (string line in markedForDeletion)
            {
                s_delayedCommandsPipeline.Remove(line);
            }
        }
    }
}