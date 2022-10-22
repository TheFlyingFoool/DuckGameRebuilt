// Decompiled with JetBrains decompiler
// Type: DuckGame.CustomBackground
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Background|custom", EditorItemType.Custom)]
    public class CustomBackground : BackgroundTile
    {
        private static CustomType _customType = CustomType.Background;
        public int customIndex;
        private string _currentTileset = "";

        public static string customBackground01
        {
            get => Custom.data[CustomBackground._customType][0];
            set
            {
                Custom.data[CustomBackground._customType][0] = value;
                Custom.Clear(CustomType.Background, value);
            }
        }

        public CustomBackground(float xpos, float ypos)
          : base(xpos, ypos)
        {
            customIndex = 0;
            graphic = new SpriteMap("arcadeBackground", 16, 16, true);
            _opacityFromGraphic = true;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -8f);
            _editorName = "01";
            UpdateCurrentTileset();
        }

        public void UpdateCurrentTileset()
        {
            CustomTileData data = Custom.GetData(customIndex, CustomBackground._customType);
            int num = 0;
            if (graphic is SpriteMap)
                num = _frame;
            if (data != null && data.texture != null)
                graphic = new SpriteMap((Tex2D)data.texture, 16, 16);
            else
                graphic = new SpriteMap("blueprintTileset", 16, 16);
            (graphic as SpriteMap).frame = num;
            _currentTileset = Custom.data[CustomBackground._customType][customIndex];
        }

        public override void Draw()
        {
            if (Level.current is Editor && _currentTileset != Custom.data[CustomBackground._customType][customIndex])
                UpdateCurrentTileset();
            base.Draw();
        }
    }
}
