using AddedContent.Firebreak;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Sets the cell size of the Editor's grid")]
        public static void EditorCellSize(float newValue)
        {
            Main.editor.cellSize = newValue;
        }
    }
}