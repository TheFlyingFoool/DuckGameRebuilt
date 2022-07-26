// Decompiled with JetBrains decompiler
// Type: DuckGame.ProceduralChunkData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ProceduralChunkData : BinaryClassChunk
    {
        public int sideMask;
        public float chance = 1f;
        public int maxPerLevel = 1;
        public bool enableSingle;
        public bool enableMulti;
        public bool canMirror;
        public bool isMirrored;
        public int numArmor;
        public int numEquipment;
        public int numSpawns;
        public int numTeamSpawns;
        public int numLockedDoors;
        public int numKeys;
        public string weaponConfig;
        public string spawnerConfig;

        public LevelObjects openAirAlternateObjects => this.GetChunk<LevelObjects>(nameof(openAirAlternateObjects));
    }
}
