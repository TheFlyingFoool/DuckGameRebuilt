// Decompiled with JetBrains decompiler
// Type: DuckGame.NMSoundEffect
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
