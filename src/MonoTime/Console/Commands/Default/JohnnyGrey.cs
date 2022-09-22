using System;
using System.Diagnostics;
using System.Linq;

namespace DuckGame
{

    public static partial class DevConsoleCommands
    {
        [DevConsoleCommand(
            Aliases = new[] { "johnnygray" },
            Description = "Try it ;D")]
        public static void JohnnyGrey()
        {
            Global.data.typedJohnny = true;
            Global.Save();

            if (!Unlockables.HasPendingUnlocks())
                return;

            DevConsole.core.open = false;

            MonoMain.pauseMenu = new UIUnlockBox(Unlockables.GetPendingUnlocks().ToList(),
                Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 190f);
        }
    }
}