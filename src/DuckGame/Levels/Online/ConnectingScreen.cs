// Decompiled with JetBrains decompiler
// Type: DuckGame.ConnectingScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ConnectingScreen : Level
    {
        private float _dots;

        public ConnectingScreen() => this._centeredView = true;

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            ConnectionStatusUI.Hide();
            base.Initialize();
        }

        public override void Draw()
        {
            this._dots += 0.01f;
            if (_dots > 1.0)
                this._dots = 0f;
            string str = "";
            for (int index = 0; index < 3; ++index)
            {
                if (_dots * 4.0 > index + 1)
                    str += ".";
            }
            string text = "Connecting";
            Graphics.DrawString(text + str, new Vec2((float)((double)Layer.HUD.width / 2.0 - (double)Graphics.GetStringWidth(text) / 2.0), (float)((double)Layer.HUD.height / 2.0 - 4.0)), Color.White);
        }
    }
}
