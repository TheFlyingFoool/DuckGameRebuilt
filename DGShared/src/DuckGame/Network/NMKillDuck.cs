// Decompiled with JetBrains decompiler
// Type: DuckGame.NMKillDuck
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMKillDuck : NMEvent
    {
        public byte index;
        public bool crush;
        public bool cook;
        public bool fall;
        public byte lifeChange;
        private byte _levelIndex;

        public NMKillDuck(byte idx, bool wasCrush, bool wasCook, bool wasFall, byte pLifeChange)
        {
            index = idx;
            crush = wasCrush;
            cook = wasCook;
            fall = wasFall;
            lifeChange = pLifeChange;
        }

        public NMKillDuck(byte idx, bool wasCrush, bool wasCook)
        {
            index = idx;
            crush = wasCrush;
            cook = wasCook;
        }

        public NMKillDuck()
        {
        }

        public override void Activate()
        {
            if (DuckNetwork.levelIndex != _levelIndex || index >= DuckNetwork.profiles.Count)
                return;
            Profile profile = DuckNetwork.profiles[index];
            if (profile.duck == null || !profile.duck.WillAcceptLifeChange(lifeChange))
                return;
            DestroyType type = !crush ? (!fall ? new DTImpact(null) : new DTFall()) : new DTCrush(null);
            profile.duck.isKillMessage = true;
            if (profile.duck.Kill(type))
            {
                if (!cook)
                    profile.duck.GoRagdoll();
                Thing.Fondle(profile.duck, connection);
                if (profile.duck._ragdollInstance != null)
                    Thing.Fondle(profile.duck._ragdollInstance, connection);
                if (profile.duck._trappedInstance != null)
                    Thing.Fondle(profile.duck._trappedInstance, connection);
                if (profile.duck._cookedInstance != null)
                    Thing.Fondle(profile.duck._cookedInstance, connection);
            }
            profile.duck.isKillMessage = false;
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write(DuckNetwork.levelIndex);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            _levelIndex = d.ReadByte();
        }
    }
}
