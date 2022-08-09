using System;
using System.Linq;
using DuckGame.AddedContent.Drake;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static string Test(string argument = "None")
    {
        return argument;
    }

    [DrawingContext(DrawingLayer.Console, DoDraw = false)]
    public static void DrawTest()
    {
        Graphics.DrawString("Console", Vec2.Zero, Color.White, 2);
    }

    [DrawingContext(DrawingLayer.HUD, CustomID = "two", DoDraw = false)]
    public static void DrawTestTwo()
    {
        Graphics.DrawString("HUD", Vec2.Zero, Color.White, 2);
    }

    [DrawingContext(DrawingLayer.HUD, CustomID = "3", DoDraw = false)]
    public static void DrawTestThree()
    {
        Graphics.DrawString("HUD", new Vec2(16, 16), Color.Red, 2);
    }

    
    [DrawingContext(DrawingLayer.Blocks)]
    public static void PolygonDrawTest()
    {
        Graphics.polyBatcher.ResetBuffer();
        Graphics.polyBatcher.Vert(Vector2.Zero).Col(Color.Aqua).Vert(Vector2.UnitX * 100).Vert(Vector2.UnitY * 100).DrawTriList();
    }
}
