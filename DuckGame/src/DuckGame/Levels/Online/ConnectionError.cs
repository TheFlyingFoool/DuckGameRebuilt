using System.Linq;

namespace DuckGame
{
    public class ConnectionError : Level, IConnectionScreen
    {
        private string _text;
        public static Lobby joinLobby;
        private UIMenu _downloadModsMenu;

        public ConnectionError(string text)
        {
            _text = text;
            _centeredView = true;
            if (_text == "CRASH")
            {
                _text = "The Host Crashed!";
            }
            else
            {
                if (!(_text == "CLOSED"))
                    return;
                _text = "Host Closed Duck Game!";
            }
        }

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            ConnectionStatusUI.Hide();
            if (joinLobby != null)
            {
                string loadedMods = joinLobby.GetLobbyData("mods");
                if (loadedMods != null && loadedMods != "" && loadedMods.Split('|').Contains("LOCAL"))
                    _text = "Host has non-workshop mods enabled!";
                else
                {
                    if (_text == "INCOMPATIBLE MOD SETUP!" || _text == "Host has different Mods enabled!")
                    {
                        _downloadModsMenu = new UIMenu("MODS REQUIRED!", Layer.HUD.camera.width / 2, Layer.HUD.camera.height / 2, 290, -1, "@SELECT@SELECT");
                        _downloadModsMenu.Add(new UIText("You're missing the mods required", Colors.DGBlue));
                        _downloadModsMenu.Add(new UIText("to join this game!", Colors.DGBlue));
                        _downloadModsMenu.Add(new UIText("", Colors.DGBlue));
                        _downloadModsMenu.Add(new UIText("Would you like to restart the", Colors.DGBlue));
                        _downloadModsMenu.Add(new UIText("game, automatically download the", Colors.DGBlue));
                        _downloadModsMenu.Add(new UIText("required mods and join the game?", Colors.DGBlue));
                        _downloadModsMenu.Add(new UIText("", Colors.DGBlue));

                        _downloadModsMenu.Add(new UIMenuItem("CANCEL", new UIMenuActionCloseMenu(_downloadModsMenu)));
                        _downloadModsMenu.Add(new UIMenuItem("RESTART AND DOWNLOAD", new UIMenuActionCloseMenuCallFunction(_downloadModsMenu, UIServerBrowser.DownloadRequiredMods)));
                        _downloadModsMenu.Close();

                        _downloadModsMenu.Open();
                        Main.pauseMenu = _downloadModsMenu;
                    }
                }
            }
            core.gameFinished = true;
            _startCalled = true;
            HUD.AddCornerMessage(HUDCorner.BottomRight, "@START@CONTINUE");
            base.Initialize();
        }

        public override void Update()
        {
            if ((_downloadModsMenu == null || !_downloadModsMenu.open) && Input.Pressed(Triggers.Start))
            {
                current = new TitleScreen();
                joinLobby = null;
            }
            base.Update();
        }

        public override void Draw()
        {
            string[] source = _text.Split('{');
            float num = -(source.Length - 1) * 8;
            foreach (string text in source)
            {
                float stringHeight = Graphics.GetStringHeight(text);
                Graphics.DrawString(text, new Vec2((Layer.HUD.camera.width / 2f - Graphics.GetStringWidth(text) / 2f), (Layer.HUD.camera.height / 2f - stringHeight / 2f)), Color.White);
                num = stringHeight + 8f;
            }
        }
    }
}
