using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Places a Thing in the editor at the specified position",
                                  To = ImplementTo.DuckShell)]
        public static void EditorPlace(Thing thing, Vec2 position)
        {
            thing.position = position;
            
            History.Add(() => // do
            {
                Main.editor.AddObject(thing);
            }, () => // undo
            {
                Main.editor.RemoveObject(thing);
            });
        }
    }
}