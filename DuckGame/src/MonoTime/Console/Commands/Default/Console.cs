using AddedContent.Firebreak;
using System;

namespace DuckGame
{
    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Edit properties of the console")]
        public static void Console(ConsoleProperty property, params string[] args)
        {
            if (args.Length == 0)
                throw new Exception("Missing arguments");

            switch (property)
            {
                case ConsoleProperty.Width:
                    {
                        float newWidth = int.Parse(args[0]);
                        Options.Data.consoleWidth = (int)Maths.Clamp(newWidth, 25, 100);
                        Options.Save();
                        break;
                    }

                case ConsoleProperty.Height:
                    {
                        float newHeight = int.Parse(args[0]);
                        Options.Data.consoleHeight = (int)Maths.Clamp(newHeight, 25, 100);
                        Options.Save();
                        break;
                    }

                case ConsoleProperty.Scale:
                    {
                        float newScale = int.Parse(args[0]);
                        Options.Data.consoleScale = (int)Maths.Clamp(newScale, 1, 5);
                        Options.Save();
                        break;
                    }

                // vile
                case ConsoleProperty.Font:
                    {
                        string newFont = string.Join(" ", args).ToLower();
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
                                        Log($"|DGGREEN|Font is now {DevConsole.RasterFont.data.name}! What a laugh!");
                                    else Log($"|DGGREEN|Font is now {DevConsole.RasterFont.data.name}!");
                                }
                                else Log($"|DGRED|Could not find font ({newFont})!");

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