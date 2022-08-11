using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [AutoConfigField]
    public static int savethisfield = 2;

    [AutoConfigField]
    public static string[] arraytest = { "x", "D" };

    [AutoConfigField]
    public static List<string> listo = new() { "wdawsa", "you shoudl" };

    [AutoConfigField]
    public static List<Vec2> listOfVectors = new()
    {
        new(1, 2),
        new(3, 4),
        new(5, 6),
        new(7, 8),
    };

    [AutoConfigField]
    public static string[] arraytesttwo;
    
    [DevConsoleCommand]
    public static object Debug(int i, string arguments = "")
    {
        string[] args = arguments?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) 
                        ?? Array.Empty<string>();
        switch (i)
        {
            case 0:
            {
                savethisfield = int.Parse(args[0]);
                break;
            }
            case 1:
            {
                AutoConfigHandler.SaveAll(false);
                break;
            }
            case 2:
                return listo;
            case 3:
                listOfVectors = listOfVectors.Select(x => new Vec2(x.x * 2, x.y * 2)).ToList();
                break;
        }

        return null;
    }
}