// Decompiled with JetBrains decompiler
// Type: DuckGame.LSFilterLevelType
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class LSFilterLevelType : IFilterLSItems
    {
        private static Dictionary<string, LevelType> _types = new Dictionary<string, LevelType>();
        private LevelType _type;
        private bool _needsDeathmatchTag;

        public LSFilterLevelType(LevelType type, bool needsDeathmatchTag = false)
        {
            this._type = type;
            this._needsDeathmatchTag = needsDeathmatchTag;
        }

        public bool Filter(string lev, LevelLocation location = LevelLocation.Any)
        {
            try
            {
                LevelType levelType = LevelType.Invalid;
                if (LSFilterLevelType._types.TryGetValue(lev, out levelType))
                    return levelType == this._type;
                LevelData levelData = DuckFile.LoadLevelHeaderCached(lev);
                if (levelData == null)
                {
                    LSFilterLevelType._types[lev] = LevelType.Invalid;
                    return false;
                }
                LevelType type = levelData.metaData.type;
                LSFilterLevelType._types[lev] = type;
                return type == this._type;
            }
            catch
            {
                LSFilterLevelType._types[lev] = LevelType.Invalid;
                return false;
            }
        }
    }
}
