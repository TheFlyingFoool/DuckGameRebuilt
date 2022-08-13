// Decompiled with JetBrains decompiler
// Type: DuckGame.ConnectingScreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ConnectingScreen : Level
    {
        private float _dots;

        public ConnectingScreen() => _centeredView = true;

        public override void Initialize()
        {
            DuckNetwork.ClosePauseMenu();
            ConnectionStatusUI.Hide();
            base.Initialize();
        }

        public override void Draw()
        {
            _dots += 0.01f;
            if (_dots > 1.0)
                _dots = 0f;
            string str = "";
            for (int index = 0; index < 3; ++index)
            {
                if (_dots * 4.0 > index + 1)
                    str += ".";
            }
            string text = "Connecting";
            Graphics.DrawString(text + str, new Vec2((Layer.HUD.width / 2f - Graphics.GetStringWidth(text) / 2f), (Layer.HUD.height / 2f - 4f)), Color.White);
        }
    }
}
