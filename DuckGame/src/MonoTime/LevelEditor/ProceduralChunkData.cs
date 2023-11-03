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

        public LevelObjects openAirAlternateObjects => GetChunk<LevelObjects>(nameof(openAirAlternateObjects));
    }
}
