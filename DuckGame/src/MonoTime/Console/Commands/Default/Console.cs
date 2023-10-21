using AddedContent.Firebreak;
using System;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Edit properties of the console")]
        public static void Console(ConsoleProperty property, string argString)
        {
            string[] args = argString.Split(' ');
            
            if (args.Length == 0)
                throw new Exception("Missing arguments");

            switch (property)
            {
                case ConsoleProperty.Width:
                    {
                        if (!int.TryParse(args[0], out int newWidth))
                            throw new Exception($"Unable to parse as integer: {args[0]}");
                        
                        Options.Data.consoleWidth = Maths.Clamp(newWidth, 25, 100);
                        Options.Save();
                        break;
                    }

                case ConsoleProperty.Height:
                    {
                        if (!int.TryParse(args[0], out int newHeight))
                            throw new Exception($"Unable to parse as integer: {args[0]}");
                        
                        Options.Data.consoleHeight = Maths.Clamp(newHeight, 25, 100);
                        Options.Save();
                        break;
                    }

                case ConsoleProperty.Scale:
                    {
                        if (!int.TryParse(args[0], out int newScale))
                            throw new Exception($"Unable to parse as integer: {args[0]}");
                        
                        Options.Data.consoleScale = Maths.Clamp(newScale, 1, 5);
                        Options.Save();
                        break;
                    }

                // vile
                case ConsoleProperty.Font:
                    {
                        string newFont = argString.ToLower();
                        switch (newFont)
                        {
                            case "clear":
                            case "default":
                            case "none":
                                Options.Data.consoleFont = "";
                                Options.Save();
                                DevConsole.Log("|DGGREEN|Console font reset.");
                                break;

                            case "comic sans":
                                newFont = "comic sans ms";
                                goto default;

                            default:
                                if (RasterFont.GetName(newFont) != null)
                                {
                                    DevConsole.RasterFont = new RasterFont(newFont, DevConsole.fontPoints);
                                    Options.Data.consoleFont = newFont;
                                    DevConsole.RasterFont.scale = new Vec2(0.5f);
                                    Options.Save();
                                    if (DevConsole.RasterFont.data.name == "Comic Sans MS")
                                        DevConsole.Log($"|DGGREEN|Font is now {DevConsole.RasterFont.data.name}! What a laugh!");
                                    else DevConsole.Log($"|DGGREEN|Font is now {DevConsole.RasterFont.data.name}!");
                                }
                                else DevConsole.Log($"|DGRED|Could not find font ({newFont})!");

                                break;
                        }

                        break;
                    }
            }
        }

        public enum ConsoleProperty
        {
            Width,
            Height,
            Scale,
            Font,
        }
    }
}