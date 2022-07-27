// Decompiled with JetBrains decompiler
// Type: DuckGame.ConnectionError
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
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
            this._text = text;
            this._centeredView = true;
            if (this._text == "CRASH")
            {
                this._text = "The Host Crashed!";
            }
            else
            {
                if (!(this._text == "CLOSED"))
                    return;
                this._text = "Host Closed Duck Game!";
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
                        this._text = "Host has non-workshop mods enabled!";
                        goto label_6;
                    }
                }
                if (this._text == "INCOMPATIBLE MOD SETUP!" || this._text == "Host has different Mods enabled!")
                {
                    this._downloadModsMenu = new UIMenu("MODS REQUIRED!", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 290f, conString: "@SELECT@SELECT");
                    this._downloadModsMenu.Add(new UIText("You're missing the mods required", Colors.DGBlue), true);
                    this._downloadModsMenu.Add(new UIText("to join this game. Would you", Colors.DGBlue), true);
                    this._downloadModsMenu.Add(new UIText("like to automatically subscribe to", Colors.DGBlue), true);
                    this._downloadModsMenu.Add(new UIText("all required mods, restart and", Colors.DGBlue), true);
                    this._downloadModsMenu.Add(new UIText("join the game?", Colors.DGBlue), true);
                    this._downloadModsMenu.Add(new UIText("", Colors.DGBlue), true);
                    this._downloadModsMenu.Add(new UIMenuItem("NO!", new UIMenuActionCloseMenu(_downloadModsMenu)), true);
                    this._downloadModsMenu.Add(new UIMenuItem("YES!", new UIMenuActionCloseMenuCallFunction(_downloadModsMenu, new UIMenuActionCloseMenuCallFunction.Function(UIServerBrowser.SubscribeAndRestart))), true);
                    this._downloadModsMenu.Close();
                    this._downloadModsMenu.Open();
                    MonoMain.pauseMenu = _downloadModsMenu;
                }
            }
        label_6:
            Level.core.gameFinished = true;
            this._startCalled = true;
            HUD.AddCornerMessage(HUDCorner.BottomRight, "@START@CONTINUE");
            base.Initialize();
        }

        public override void Update()
        {
            if ((this._downloadModsMenu == null || !this._downloadModsMenu.open) && Input.Pressed("START"))
            {
                Level.current = new TitleScreen();
                ConnectionError.joinLobby = null;
            }
            base.Update();
        }

        public override void Draw()
        {
            string[] source = this._text.Split('{');
            float num = -(source.Count<string>() - 1) * 8;
            foreach (string text in source)
            {
                float stringHeight = Graphics.GetStringHeight(text);
                Graphics.DrawString(text, new Vec2((float)((double)Layer.HUD.camera.width / 2.0 - (double)Graphics.GetStringWidth(text) / 2.0), (float)((double)Layer.HUD.camera.height / 2.0 - (double)stringHeight / 2.0)), Color.White);
                num = stringHeight + 8f;
            }
        }
    }
}
