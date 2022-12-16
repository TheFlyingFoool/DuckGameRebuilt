// Decompiled with JetBrains decompiler
// Type: DuckGame.Nubber
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            if (_sprite != null)
                num = _sprite.frame;
            if (tileset == "CUSTOM01")
            {
                CustomTileData data = Custom.GetData(0, CustomType.Block);
                _sprite = data == null || data.texture == null ? new SpriteMap("blueprintTileset", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (tileset == "CUSTOM02")
            {
                CustomTileData data = Custom.GetData(1, CustomType.Block);
                _sprite = data == null || data.texture == null ? new SpriteMap("blueprintTileset", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (tileset == "CUSTOM03")
            {
                CustomTileData data = Custom.GetData(2, CustomType.Block);
                _sprite = data == null || data.texture == null ? new SpriteMap("blueprintTileset", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (tileset == "CUSTOMPLAT01")
            {
                CustomTileData data = Custom.GetData(0, CustomType.Platform);
                _sprite = data == null || data.texture == null ? new SpriteMap("scaffolding", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (tileset == "CUSTOMPLAT02")
            {
                CustomTileData data = Custom.GetData(1, CustomType.Platform);
                _sprite = data == null || data.texture == null ? new SpriteMap("scaffolding", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            else if (tileset == "CUSTOMPLAT03")
            {
                CustomTileData data = Custom.GetData(2, CustomType.Platform);
                _sprite = data == null || data.texture == null ? new SpriteMap("scaffolding", 16, 16) : new SpriteMap((Tex2D)data.texture, 16, 16);
            }
            if (_sprite == null)
                _sprite = new SpriteMap(tileset, 16, 16);
            graphic = _sprite;
            _sprite.frame = num;
        }

        public Nubber(float x, float y, bool left, string tset)
          : base(x, y)
        {
            tileset = tset;
            UpdateCustomTileset();
            graphic = _sprite;
            collisionSize = new Vec2(8f, 5f);
            _sprite.frame = left ? 62 : 63;
            if (left)
                collisionOffset = new Vec2(13f, 0f);
            else
                collisionOffset = new Vec2(-5f, 0f);
            _editorCanModify = false;
            UpdateCustomTileset();
            shouldbeinupdateloop = false;
            cheap = true;
        }

        public override void Terminate()
        {
        }

        public virtual void DoPositioning()
        {
            //if (Level.current is Editor || graphic == null)
            //     return;
            graphic.position = position;
            graphic.scale = scale;
            graphic.center = center;
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            (graphic as SpriteMap).ClearCache();
            (graphic as SpriteMap).UpdateFrame();
        }

        public override void Draw()
        {
            //if (cheap && !Editor.editorDraw)
            //{
            //    DoPositioning();
            //    graphic.UltraCheapStaticDraw(flipHorizontal);
            //}
            //else
            //{
            //    base.Draw();
            //}  
            if (removeFromLevel && layer != null)
            {
                layer.RemoveSoon(this);
            }
            if (graphic.position != position)
            {
                (graphic as SpriteMap).ClearCache();
            }
            graphic.position = position;
            graphic.scale = scale;
            graphic.center = center;
            graphic.depth = depth;
            graphic.alpha = alpha;
            graphic.angle = angle;
            graphic.cheapmaterial = material;
            (graphic as SpriteMap).UpdateFrame();
            graphic.UltraCheapStaticDraw(flipHorizontal);
            //  graphic.Draw() FUCK NORMAL DRAWING I AM CHEAP BASTERD 
        }
    }
}
