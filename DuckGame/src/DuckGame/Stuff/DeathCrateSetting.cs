namespace DuckGame
{
    public abstract class DeathCrateSetting
    {
        public float likelyhood = 1f;

        public virtual void Update(DeathCrate pCrate)
        {
        }

        public abstract void Activate(DeathCrate c, bool server = true);
    }
}
