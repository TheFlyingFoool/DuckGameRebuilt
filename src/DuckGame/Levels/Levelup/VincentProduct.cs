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
                if (this.furnitureData != null)
                    return (Sprite)this.furnitureData.sprite;
                return this.teamData != null ? (Sprite)this.teamData.hat : (Sprite)null;
            }
        }

        public Color color => this.furnitureData != null ? this.furnitureData.group.color : Color.White;

        public string name
        {
            get
            {
                if (this.furnitureData != null)
                    return this.furnitureData.name;
                return this.teamData != null ? this.teamData.name + " HAT" : "Something";
            }
        }

        public string group => this.furnitureData != null ? this.furnitureData.group.name : "HATS";

        public string description
        {
            get
            {
                if (this.furnitureData != null)
                    return this.furnitureData.description;
                return this.teamData != null ? this.teamData.description : "What a fine piece of furniture.";
            }
        }

        public void Draw(Vec2 pos, float alpha, float deep)
        {
            if (this.furnitureData != null)
            {
                SpriteMap g = this.furnitureData.sprite;
                if (this.furnitureData.icon != null)
                    g = this.furnitureData.icon;
                if (this.furnitureData.font != null && this.furnitureData.sprite == null)
                {
                    this.furnitureData.font.scale = new Vec2(1f, 1f);
                    this.furnitureData.font.Draw("F", pos + new Vec2(-3.5f, -3f), Color.Black, (Depth)(deep + 0.005f));
                }
                g.depth = (Depth)deep;
                g.frame = 0;
                g.alpha = alpha;
                Graphics.Draw((Sprite)g, pos.x, pos.y);
                g.alpha = 1f;
            }
            if (this.teamData == null)
                return;
            SpriteMap hat = this.teamData.hat;
            hat.depth = (Depth)deep;
            hat.frame = 0;
            hat.alpha = alpha;
            hat.center = new Vec2(16f, 16f) + this.teamData.hatOffset;
            Graphics.Draw((Sprite)hat, pos.x, pos.y);
            hat.alpha = 1f;
        }
    }
}
