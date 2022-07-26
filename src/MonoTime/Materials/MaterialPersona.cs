// Decompiled with JetBrains decompiler
// Type: DuckGame.MaterialPersona
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework;

namespace DuckGame
{
    public class MaterialPersona : Material
    {
        private DuckPersona persona;

        public MaterialPersona(DuckPersona pPersona)
        {
            this.persona = pPersona;
            this._effect = Content.Load<MTEffect>("Shaders/recolor_duo");
        }

        public override void Update()
        {
        }

        public override void Apply()
        {
            this._effect.effect.Parameters["replace1"].SetValue((Vector3)(this.persona.color / (float)byte.MaxValue));
            this._effect.effect.Parameters["replace2"].SetValue((Vector3)(this.persona.colorDark / (float)byte.MaxValue));
            base.Apply();
        }
    }
}
