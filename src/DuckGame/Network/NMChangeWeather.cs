// Decompiled with JetBrains decompiler
// Type: DuckGame.NMChangeWeather
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMChangeWeather : NMEvent
    {
        public byte weather;

        public NMChangeWeather()
        {
        }

        public NMChangeWeather(byte weatherVal) => weather = weatherVal;

        public override void Activate()
        {
            if (Level.current is RockScoreboard)
                (Level.current as RockScoreboard).SetWeather((Weather)weather);
            base.Activate();
        }
    }
}
