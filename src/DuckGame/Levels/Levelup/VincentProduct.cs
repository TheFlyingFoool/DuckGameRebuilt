// Decompiled with JetBrains decompiler
// Type: DuckGame.VincentProduct
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class VincentProduct
    {
        public VPType type;
        public int cost;
        public int originalCost;
        public int rarity;
        public int count;
        public Furniture furnitureData;
        public Team teamData;
        public bool sold;

        public Sprite sprite
        {
            get
            {
                if (furnitureData != null)
                    return furnitureData.sprite;
                return teamData != null ? teamData.hat : (Sprite)null;
            }
        }

        public Color color => furnitureData != null ? furnitureData.group.color : Color.White;

        public string name
        {
            get
            {
                if (furnitureData != null)
                    return furnitureData.name;
                return teamData != null ? teamData.name + " HAT" : "Something";
            }
        }

        public string group => furnitureData != null ? furnitureData.group.name : "HATS";

        public string description
        {
            get
            {
                if (furnitureData != null)
                    return furnitureData.description;
                return teamData != null ? teamData.description : "What a fine piece of furniture.";
            }
        }

        public void Draw(Vec2 pos, float alpha, float deep)
        {
            if (furnitureData != null)
            {
                SpriteMap g = furnitureData.sprite;
                if (furnitureData.icon != null)
                    g = furnitureData.icon;
                if (furnitureData.font != null && furnitureData.sprite == null)
                {
                    furnitureData.font.scale = new Vec2(1f, 1f);
                    furnitureData.font.Draw("F", pos + new Vec2(-3.5f, -3f), Color.Black, (Depth)(deep + 0.005f));
                }
                g.depth = (Depth)deep;
                g.frame = 0;
                g.alpha = alpha;
                Graphics.Draw(g, pos.x, pos.y);
                g.alpha = 1f;
            }
            if (teamData == null)
                return;
            SpriteMap hat = teamData.hat;
            hat.depth = (Depth)deep;
            hat.frame = 0;
            hat.alpha = alpha;
            hat.center = new Vec2(16f, 16f) + teamData.hatOffset;
            Graphics.Draw(hat, pos.x, pos.y);
            hat.alpha = 1f;
        }
    }
}
