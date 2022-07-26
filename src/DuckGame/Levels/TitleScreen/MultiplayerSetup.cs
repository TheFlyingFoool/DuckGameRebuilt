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
            this.camera.x = 480f;
            UIMenu menu = new UIMenu("MULTIPLAYER", (float)Graphics.width / 2f, (float)Graphics.height / 2f, 160f);
            menu.scale = new Vec2(4f);
            menu.Add((UIComponent)new UIMenuItemNumber("ROUNDS PER SET", field: new FieldBinding((object)this, "roundsPerSet", max: 50f)), true);
            menu.Add((UIComponent)new UIMenuItemNumber("SETS PER GAME", field: new FieldBinding((object)this, "setsPerGame", max: 50f)), true);
            menu.Add((UIComponent)new UIText(" ", Color.White), true);
            menu.Add((UIComponent)new UIMenuItem("START", (UIMenuAction)new UIMenuActionChangeLevel((UIComponent)menu, (Level)new TeamSelect2())), true);
            menu.Add((UIComponent)new UIMenuItem("BACK", (UIMenuAction)new UIMenuActionChangeLevel((UIComponent)menu, (Level)new TitleScreen())), true);
            Level.Add((Thing)menu);
            base.Initialize();
        }

        public override void Update()
        {
        }
    }
}
