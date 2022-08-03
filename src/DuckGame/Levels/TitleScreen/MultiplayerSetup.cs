// Decompiled with JetBrains decompiler
// Type: DuckGame.MultiplayerSetup
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class MultiplayerSetup : Level
    {
        public int roundsPerSet = 8;
        public int setsPerGame = 3;

        public override void Initialize()
        {
            camera.x = 480f;
            UIMenu menu = new UIMenu("MULTIPLAYER", Graphics.width / 2f, Graphics.height / 2f, 160f)
            {
                scale = new Vec2(4f)
            };
            menu.Add(new UIMenuItemNumber("ROUNDS PER SET", field: new FieldBinding(this, "roundsPerSet", max: 50f)), true);
            menu.Add(new UIMenuItemNumber("SETS PER GAME", field: new FieldBinding(this, "setsPerGame", max: 50f)), true);
            menu.Add(new UIText(" ", Color.White), true);
            menu.Add(new UIMenuItem("START", new UIMenuActionChangeLevel(menu, new TeamSelect2())), true);
            menu.Add(new UIMenuItem("BACK", new UIMenuActionChangeLevel(menu, new TitleScreen())), true);
            Level.Add(menu);
            base.Initialize();
        }

        public override void Update()
        {
        }
    }
}
