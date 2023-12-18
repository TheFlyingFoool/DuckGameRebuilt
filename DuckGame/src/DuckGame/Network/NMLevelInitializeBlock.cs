namespace DuckGame
{
    public class NMLevelInitializeBlock : NetMessage
    {
        public NMLevelInitializeBlock() => manager = BelongsToManager.DatablockManager;
    }
}
