using AddedContent.Firebreak;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [Marker.DevConsoleCommand(Description = "Gets the element in the collection at the specified index", To = ImplementTo.DuckShell)]
        public static string At(int index, DSHKeyword @in, string[] collection)
        {
            return collection[index];
        }
    }
}