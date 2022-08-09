using System;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
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