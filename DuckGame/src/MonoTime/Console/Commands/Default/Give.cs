namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(Description = "Gives a player an item by name")]
        public static void Give(Profile player, Holdable item, string arguments)
        {
            //bool hold = true;

            // Parameter checks


            //if (item is Gun g)
            //{
            //    if (arguments.Contains("-i"))
            //        g.infiniteAmmoVal = true;

            //    if (arguments.Contains("-ph"))
            //    {
            //        item = new PowerHolster(g.x, g.y)
            //        {
            //            containedObject = g
            //        };
            //    }
            //    else if (arguments.Contains("-h"))
            //    {
            //        item = new Holster(g.x, g.y)
            //        {
            //            containedObject = g
            //        };
            //    }
            //}
            // Commented out because auto equipping is dysfunctional currently with this method.
            //if (item is Equipment e)
            //{
            //    if (arguments.Contains("-e"))
            //        player.duck.Equip(e);
            //}

            if (item is Gun g && arguments.Contains("-i"))
            {
                g.infiniteAmmoVal = true;
            }


            SFX.Play("hitBox");
            Level.Add(item);
            player.duck.GiveHoldable(item);
        }
    }
}