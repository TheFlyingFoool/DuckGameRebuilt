// Decompiled with JetBrains decompiler
// Type: DuckGame.NMOldAngles
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.enabled = pEnabled;
            this.profile = pProfile;
        }

        protected override void OnSerialize()
        {
            this._serializedData.WriteProfile(this.profile);
            this._serializedData.Write(this.enabled);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            this.profile = d.ReadProfile();
            this.enabled = d.ReadBool();
        }

        public override void Activate()
        {
            if (this.profile != null && this.profile.inputProfile != null)
                this.profile.inputProfile.oldAngles = this.enabled;
            base.Activate();
        }
    }
}
