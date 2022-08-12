using System;
using System.Linq;
using DuckGame.AddedContent.Drake.PolyRender;
using Microsoft.Xna.Framework;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DrawingContext(CustomID = "collision", DoDraw = false)]
    public static void DrawCollisions()
    {
        const float length = 2f;
        PolyRenderer.Line(Mouse.position.ButY(-length, true), Mouse.position.ButY(length, true), 0.5f, Color.White);
        PolyRenderer.Line(Mouse.position.ButX(-length, true), Mouse.position.ButX(length, true), 0.5f, Color.White);
    }
    
    [DrawingContext(DrawingLayer.Foreground, CustomID = "collision2", DoDraw = false)]
    public static void DrawCollisions2()
    {
        foreach (var t in Level.current.things)
        {
            if (new Rectangle(t.topLeft, t.bottomRight).Contains(Mouse.positionScreen))
                PolyRenderer.Rect(t.topLeft, t.bottomRight, Color.Coral * 0.3f);
        }
    }

    [DevConsoleCommand(Description = "Toggle the activity of a Drawing Context from it's ID")]
    public static string DoDraw(string id)
    {
        if (DrawingContextAttribute.ReflectionMethodsUsing
            .TryFirst(x => (x.Attribute.CustomID ?? x.MemberInfo.Name).CaselessEquals(id),
                out var pair))
            pair!.Attribute.DoDraw ^= true;
        else if (DrawingContextAttribute.PrecompiledMethodsUsing
                 .TryFirst(x => x.Attribute.CustomID.CaselessEquals(id),
                     out var tuple))
            tuple.Attribute.DoDraw ^= true;
        else
            return $"|DGRED|No Drawing Context matches id of [{id}]";

        return $"|DGBLUE|Drawing Context [{id}] has been toggled";
    }
}