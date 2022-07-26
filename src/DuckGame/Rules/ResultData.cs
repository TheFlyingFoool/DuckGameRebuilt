// Decompiled with JetBrains decompiler
// Type: DuckGame.ResultData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public struct ResultData
    {
        public string name;
        public bool multi;
        public object data;
        public int score;
        public BitmapFont font;

        public ResultData(Team t)
        {
            this.font = Profiles.EnvironmentProfile.font;
            if (t.activeProfiles.Count > 1)
            {
                this.name = t.GetNameForDisplay();
                this.multi = true;
            }
            else
            {
                this.name = !Profiles.IsDefault(t.activeProfiles[0]) ? t.activeProfiles[0].name : t.GetNameForDisplay();
                this.font = t.activeProfiles[0].font;
                this.multi = false;
            }
            this.data = (object)t;
            this.score = t.score;
        }
    }
}
