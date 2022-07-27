// Decompiled with JetBrains decompiler
// Type: DuckGame.Nubber
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Nubber : MaterialThing, IPlatform, IDontMove, IDontUpdate
    {
        private SpriteMap _sprite;
        public string tileset;
        public bool cheap;

        public void UpdateCustomTileset()
        {
            int num = 0;
            if (this._sprite != null)
                num = this._sprite.frame;
            if (this.tileset == "CUSTOM01")
            {
                CustomTileData data = Custom.GetData(0, CustomType.Block);
                this._sprite = data == null || data.texture == null ? new SpriteMap("blueprintTileset", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (this.tileset == "CUSTOM02")
            {
                CustomTileData data = Custom.GetData(1, CustomType.Block);
                this._sprite = data == null || data.texture == null ? new SpriteMap("blueprintTileset", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (this.tileset == "CUSTOM03")
            {
                CustomTileData data = Custom.GetData(2, CustomType.Block);
                this._sprite = data == null || data.texture == null ? new SpriteMap("blueprintTileset", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (this.tileset == "CUSTOMPLAT01")
            {
                CustomTileData data = Custom.GetData(0, CustomType.Platform);
                this._sprite = data == null || data.texture == null ? new SpriteMap("scaffolding", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (this.tileset == "CUSTOMPLAT02")
            {
                CustomTileData data = Custom.GetData(1, CustomType.Platform);
                this._sprite = data == null || data.texture == null ? new SpriteMap("scaffolding", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (this.tileset == "CUSTOMPLAT03")
            {
                CustomTileData data = Custom.GetData(2, CustomType.Platform);
                this._sprite = data == null || data.texture == null ? new SpriteMap("scaffolding", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            if (this._sprite == null)
                this._sprite = new SpriteMap(this.tileset, 16, 16);
            this.graphic = _sprite;
            this._sprite.frame = num;
        }

        public Nubber(float x, float y, bool left, string tset)
          : base(x, y)
        {
            this.tileset = tset;
            this.UpdateCustomTileset();
            this.graphic = _sprite;
            this.collisionSize = new Vec2(8f, 5f);
            this._sprite.frame = left ? 62 : 63;
            if (left)
                this.collisionOffset = new Vec2(13f, 0.0f);
            else
                this.collisionOffset = new Vec2(-5f, 0.0f);
            this._editorCanModify = false;
            this.UpdateCustomTileset();
        }

        public override void Terminate()
        {
        }

        public virtual void DoPositioning()
        {
            if (Level.current is Editor || this.graphic == null)
                return;
            this.graphic.position = this.position;
            this.graphic.scale = this.scale;
            this.graphic.center = this.center;
            this.graphic.depth = this.depth;
            this.graphic.alpha = this.alpha;
            this.graphic.angle = this.angle;
            (this.graphic as SpriteMap).ClearCache();
            (this.graphic as SpriteMap).UpdateFrame();
        }

        public override void Draw()
        {
            if (this.cheap)
                this.graphic.UltraCheapStaticDraw(this.flipHorizontal);
            else
                base.Draw();
        }
    }
}
