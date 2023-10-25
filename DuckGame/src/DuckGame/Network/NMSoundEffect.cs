namespace DuckGame
{
    public class NMSoundEffect : NMEvent
    {
        public int soundHash;
        public float volume;
        public float pitch;

        public NMSoundEffect()
        {
            manager = BelongsToManager.EventManager;
            priority = NetMessagePriority.UnreliableUnordered;
        }

        public NMSoundEffect(string pSound, float pVolume, float pPitch)
        {
            manager = BelongsToManager.EventManager;
            priority = NetMessagePriority.UnreliableUnordered;
            soundHash = SFX.SoundHash(pSound);
            volume = pVolume;
            pitch = pPitch;
        }

        public override void Activate() => SFX.Play(soundHash, volume, pitch);
    }
}
