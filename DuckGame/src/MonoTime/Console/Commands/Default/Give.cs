using AddedContent.Firebreak;
using DuckGame.ConsoleEngine;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [Marker.DevConsoleCommand(Description = "Gives a player an item by name", IsCheat = true)]
        public static void Give(Profile player, Holdable item, [AutoCompl("i", "h", "ph", "e")] string arguments = "")
        {
            bool noHold = false;
            
            if (item is Gun g)
            {
                if (arguments.Contains("i"))
                {
                    g.infinite.value = true;
                }

                if (arguments.Contains("ph") || arguments.Contains("h"))
                {
                    if (player.duck.GetEquipment(typeof(Holster)) is Holster holster)
                    {
                        holster.SetContainedObject(g);
                    }
                    else
                    {
                        item = arguments.Contains("ph")
                            ? new PowerHolster(0, 0)
                            : new Holster(0, 0);

                        Holster holsteredItem = (Holster) item;
                        
                        holsteredItem.SetContainedObject(g);
                        player.duck.Equip(holsteredItem);
                    }
                    
                    noHold = true;
                }
            }
            
            SFX.Play("hitBox");
            DuckGame.Level.Add(item);
            
            if (item is Equipment e && arguments.Contains("e"))
            {
                player.duck.Equip(e);
            }
            else if (!noHold)
            {
                player.duck.GiveHoldable(item);
            }
        }
    }
}