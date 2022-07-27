// Decompiled with JetBrains decompiler
// Type: DuckGame.LSFilterMods
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class LSFilterMods : IFilterLSItems
    {
        private bool _isOnline;
        private static Dictionary<string, Dictionary<bool, bool>> _filters = new Dictionary<string, Dictionary<bool, bool>>();

        public LSFilterMods(bool isOnline) => this._isOnline = isOnline;

        private bool Cache(string lev, bool result)
        {
            Dictionary<bool, bool> dictionary;
            if (!LSFilterMods._filters.TryGetValue(lev, out dictionary))
                LSFilterMods._filters[lev] = dictionary = new Dictionary<bool, bool>();
            dictionary[this._isOnline] = result;
            return result;
        }

        public bool Filter(string lev, LevelLocation location = LevelLocation.Any)
        {
            try
            {
                Dictionary<bool, bool> dictionary = null;
                bool flag;
                if (LSFilterMods._filters.TryGetValue(lev, out dictionary) && dictionary.TryGetValue(this._isOnline, out flag))
                    return flag;
                LevelData levelData = DuckFile.LoadLevelHeaderCached(lev);
                if (levelData == null)
                    return this.Cache(lev, false);
                ModMetaData modData = levelData.modData;
                if (this._isOnline)
                {
                    if (modData.hasLocalMods && !MonoMain.modDebugging)
                        return this.Cache(lev, false);
                    HashSet<ulong> other = new HashSet<ulong>();
                    foreach (Mod accessibleMod in (IEnumerable<Mod>)ModLoader.accessibleMods)
                    {
                        if (accessibleMod.configuration.isWorkshop || accessibleMod.configuration.assignedWorkshopID != 0UL)
                            other.Add(accessibleMod.configuration.assignedWorkshopID);
                        if (accessibleMod.workshopIDFacade != 0UL)
                            other.Add(accessibleMod.workshopIDFacade);
                    }
                    if (!modData.workshopIDs.IsSubsetOf(other))
                        return this.Cache(lev, false);
                }
                return this.Cache(lev, true);
            }
            catch
            {
                return this.Cache(lev, false);
            }
        }
    }
}
