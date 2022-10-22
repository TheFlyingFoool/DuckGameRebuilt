// Decompiled with JetBrains decompiler
// Type: DuckGame.ConnectionError
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            if (ConnectionError.joinLobby != null)
            {
                string lobbyData = ConnectionError.joinLobby.GetLobbyData("mods");
                if (lobbyData != null && lobbyData != "")
                {
                    if (lobbyData.Split('|').Contains<string>("LOCAL"))
                    {
                        _text = "Host has non-workshop mods enabled!";
                        goto label_6;
                    }
                }
                if (_text == "INCOMPATIBLE MOD SETUP!" || _text == "Host has different Mods enabled!")
                {
                    _downloadModsMenu = new UIMenu("MODS REQUIRED!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 290f, conString: "@SELECT@SELECT");
                    _downloadModsMenu.Add(new UIText("You're missing the mods required", Colors.DGBlue), true);
                    _downloadModsMenu.Add(new UIText("to join this game. Would you", Colors.DGBlue), true);
                    _downloadModsMenu.Add(new UIText("like to automatically subscribe to", Colors.DGBlue), true);
                    _downloadModsMenu.Add(new UIText("all required mods, restart and", Colors.DGBlue), true);
                    _downloadModsMenu.Add(new UIText("join the game?", Colors.DGBlue), true);
                    _downloadModsMenu.Add(new UIText("", Colors.DGBlue), true);
                    _downloadModsMenu.Add(new UIMenuItem("NO!", new UIMenuActionCloseMenu(_downloadModsMenu)), true);
                    _downloadModsMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuCallFunction(_downloadModsMenu, new UIMenuActionCloseMenuCallFunction.Function(UIServerBrowser.SubscribeAndRestart))), true);
                    _downloadModsMenu.Close();
                    _downloadModsMenu.Open();
                    MonoMain.pauseMenu = _downloadModsMenu;
                }
            }
        label_6:
            Level.core.gameFinished = true;
            _startCalled = true;
            HUD.AddCornerMessage(HUDCorner.BottomRight, "@START@CONTINUE");
            base.Initialize();
        }

        public override void Update()
        {
            if ((_downloadModsMenu == null || !_downloadModsMenu.open) && Input.Pressed("START"))
            {
                Level.current = new TitleScreen();
                ConnectionError.joinLobby = null;
            }
            base.Update();
        }

        public override void Draw()
        {
            string[] source = _text.Split('{');
            float num = -(source.Count<string>() - 1) * 8;
            foreach (string text in source)
            {
                float stringHeight = Graphics.GetStringHeight(text);
                Graphics.DrawString(text, new Vec2((Layer.HUD.camera.width / 2f - Graphics.GetStringWidth(text) / 2f), (Layer.HUD.camera.height / 2f - stringHeight / 2f)), Color.White);
                num = stringHeight + 8f;
            }
        }
    }
}
