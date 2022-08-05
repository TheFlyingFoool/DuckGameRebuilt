// Decompiled with JetBrains decompiler
// Type: DuckGame.NMFuneral
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMFuneral : NMEvent
    {
        public Profile profile;
        public Duck lay;

        public NMFuneral()
        {
        }

        public NMFuneral(Profile pProfile, Duck pLay)
        {
            profile = pProfile;
            lay = pLay;
        }

        public override void Activate()
        {
            if (lay != null && profile != null)
            {
                lay.isConversionMessage = true;
                lay.LayToRest(profile);
                lay.isConversionMessage = false;
                if (!Music.currentSong.Contains("MarchOfDuck"))
                    Music.Play("MarchOfDuck", false);
            }
            base.Activate();
        }
    }
}
