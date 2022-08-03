// Decompiled with JetBrains decompiler
// Type: DuckGame.NMSkySay
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Linq;

namespace DuckGame
{
    public class NMSkySay : NMEvent
    {
        public string _text;
        public Vec2 _spawn;
        public bool _flyLeft;

        public NMSkySay(string pText, Vec2 pSpawn, bool pFlyLeft)
        {
            _text = pText;
            _spawn = pSpawn;
            _flyLeft = pFlyLeft;
        }

        public NMSkySay()
        {
        }

        public override void Activate()
        {
            if (Level.current == null || !(Level.current.things[typeof(CityBackground)].FirstOrDefault<Thing>() is CityBackground cityBackground))
                return;
            cityBackground.SkySay(_text, _spawn, _flyLeft);
        }
    }
}
