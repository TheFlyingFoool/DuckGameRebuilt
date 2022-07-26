// Decompiled with JetBrains decompiler
// Type: DuckGame.MTEffect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;

namespace DuckGame
{
    public class MTEffect
    {
        private Effect _base;
        private short _effectIndex;
        private string _effectName;

        public short effectIndex => this._effectIndex;

        public string effectName => this._effectName;

        public void SetEffectIndex(short index) => this._effectIndex = index;

        public Effect effect => this._base;

        public MTEffect(Effect tex, string cureffectName, short cureffectIndex = 0)
        {
            this._base = tex;
            this._effectIndex = cureffectIndex;
            this._effectName = cureffectName;
        }

        public static implicit operator Effect(MTEffect tex) => tex?._base;

        public static implicit operator MTEffect(Effect tex) => Content.GetMTEffect(tex);
    }
}
