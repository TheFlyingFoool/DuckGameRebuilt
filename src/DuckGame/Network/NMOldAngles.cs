// Decompiled with JetBrains decompiler
// Type: DuckGame.NMOldAngles
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMOldAngles : NMEvent
    {
        public bool enabled;
        public Profile profile;

        public NMOldAngles()
        {
        }

        public NMOldAngles(Profile pProfile, bool pEnabled)
        {
            enabled = pEnabled;
            profile = pProfile;
        }

        protected override void OnSerialize()
        {
            _serializedData.WriteProfile(profile);
            _serializedData.Write(enabled);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            profile = d.ReadProfile();
            enabled = d.ReadBool();
        }

        public override void Activate()
        {
            if (profile != null && profile.inputProfile != null)
                profile.inputProfile.oldAngles = enabled;
            base.Activate();
        }
    }
}
