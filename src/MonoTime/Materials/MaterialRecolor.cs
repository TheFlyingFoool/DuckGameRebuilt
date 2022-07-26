// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialRecolor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;

namespace DuckGame
{
    public class MaterialRecolor : Material
    {
        public Vec3 color;

        public MaterialRecolor(Vec3 col)
        {
            this.color = col;
            this._effect = Content.Load<MTEffect>("Shaders/recolor");
        }

        public override void Update()
        {
        }

        public override void Apply()
        {
            this._effect.effect.Parameters["fcol"].SetValue((Vector3)this.color);
            base.Apply();
        }
    }
}
