using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand(Description = "Gives a player an item by name")]
    public static void Give(Profile player, Holdable item, string arguments)
    {
        bool hold = true;
        
        if (item is Gun g)
        {
            if (arguments.Contains("-i"))
                g.infiniteAmmoVal = true;

            if (arguments.Contains("-ph"))
            {
                item = new PowerHolster(g.x, g.y)
                {
                    containedObject = g
                };
                
                hold = false;
            }
            else if (arguments.Contains("-h"))
            {
                item = new Holster(g.x, g.y)
                {
                    containedObject = g
                };
                
                hold = false;
            }
        }
        
        if (item is Equipment e)
        {
            if (arguments.Contains("-e"))
                player.duck.Equip(e);
            
            hold = false;
        }
        
        SFX.Play("hitBox");
        
        DuckGame.Level.Add(item);
        
        if (player == DuckNetwork.localProfile && hold)
        {
            player.duck.GiveHoldable(item);
        }
    }
}