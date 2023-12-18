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
            _type = type;
            _needsDeathmatchTag = needsDeathmatchTag;
        }

        public bool Filter(string lev, LevelLocation location = LevelLocation.Any)
        {
            if (DGRSettings.IgnoreLevRestrictions) return true;
            try
            {
                LevelType levelType = LevelType.Invalid;
                if (_types.TryGetValue(lev, out levelType))
                    return levelType == _type;
                LevelData levelData = DuckFile.LoadLevelHeaderCached(lev);
                if (levelData == null)
                {
                    _types[lev] = LevelType.Invalid;
                    return false;
                }
                LevelType type = levelData.metaData.type;
                _types[lev] = type;
                return type == _type;
            }
            catch
            {
                _types[lev] = LevelType.Invalid;
                return false;
            }
        }
    }
}
