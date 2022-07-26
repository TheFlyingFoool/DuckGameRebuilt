// Decompiled with JetBrains decompiler
// Type: DuckGame.DisconnectError
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class DisconnectError : Level
    {
        private Profile _profile;

        public DisconnectError(Profile who)
        {
            this._profile = who;
            this._centeredView = true;
        }

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            ConnectionStatusUI.Hide();
            HUD.AddCornerMessage(HUDCorner.BottomRight, "@START@CONTINUE");
            this._startCalled = true;
            base.Initialize();
        }

        public override void Update()
        {
            if (Input.Pressed("START"))
                Level.current = (Level)new TitleScreen();
            base.Update();
        }

        public override void Draw()
        {
            if (this._profile != null)
            {
                string text = " |RED|" + this._profile.name + " has disconnected.";
                Graphics.DrawString(text, new Vec2((float)((double)Layer.HUD.camera.width / 2.0 - (double)Graphics.GetStringWidth(text) / 2.0), Layer.HUD.camera.height / 2f), Color.White);
            }
            else
            {
                string text = " |RED|The host has disconnected.";
                Graphics.DrawString(text, new Vec2((float)((double)Layer.HUD.camera.width / 2.0 - (double)Graphics.GetStringWidth(text) / 2.0), Layer.HUD.camera.height / 2f), Color.White);
            }
        }
    }
}
