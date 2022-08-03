// Decompiled with JetBrains decompiler
// Type: DuckGame.NCError
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NCError
    {
        public string text;
        public NCErrorType type;

        public NCError(string s, NCErrorType tp)
        {
            text = s;
            type = tp;
        }

        public Color color
        {
            get
            {
                switch (type)
                {
                    case NCErrorType.Success:
                        return Color.Lime;
                    case NCErrorType.Message:
                        return Color.White;
                    case NCErrorType.Warning:
                        return Color.Yellow;
                    case NCErrorType.Debug:
                        return Color.LightPink;
                    default:
                        return Color.Red;
                }
            }
        }
    }
}
