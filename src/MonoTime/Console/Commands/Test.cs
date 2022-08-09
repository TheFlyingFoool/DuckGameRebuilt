using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static string Test(string argument = "None")
    {
        return argument;
    }

    [DrawingContext(DrawingLayer.Console)]
    public static void DrawTest()
    {
        Graphics.DrawString("Console", Vec2.Zero, Color.White, 2);
    }

    [DrawingContext(DrawingLayer.HUD, CustomID = "two")]
    public static void DrawTestTwo()
    {
        Graphics.DrawString("HUD", Vec2.Zero, Color.White, 2);
    }

    [DrawingContext(DrawingLayer.HUD, CustomID = "3", DoDraw = false)]
    public static void DrawTestThree()
    {
        Graphics.DrawString("HUD", new Vec2(16, 16), Color.Red, 2);
    }
}