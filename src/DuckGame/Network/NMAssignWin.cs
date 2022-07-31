// Decompiled with JetBrains decompiler
// Type: DuckGame.NMAssignWin
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class NMAssignWin : NMEvent
    {
        public List<Profile> profiles = new List<Profile>();
        public Profile theRealWinnerHere;
        protected string _sound = "scoreDing";

        public NMAssignWin(List<Profile> pProfiles, Profile pTheRealWinnerHere)
        {
            this.profiles = pProfiles;
            this.theRealWinnerHere = pTheRealWinnerHere;
        }

        public NMAssignWin()
        {
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            this._serializedData.Write((byte)this.profiles.Count);
            foreach (Profile profile in this.profiles)
                this._serializedData.WriteProfile(profile);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            byte num = d.ReadByte();
            for (int index = 0; index < num; ++index)
                this.profiles.Add(d.ReadProfile());
        }

        public override void Activate()
        {
            SFX.Play(this._sound, 0.8f);
            foreach (Profile profile in this.profiles)
            {
                GameMode.lastWinners.Add(profile);
                Profile p = this.theRealWinnerHere != null ? this.theRealWinnerHere : profile;
                if (profile.duck != null)
                {
                    PlusOne plusOne = new PlusOne(0f, 0f, p, testMode: true)
                    {
                        _duck = profile.duck,
                        anchor = (Anchor)profile.duck
                    };
                    plusOne.anchor.offset = new Vec2(0f, -16f);
                    plusOne.depth = (Depth)0.95f;
                    Level.Add(plusOne);
                }
            }
            if (this is NMPlusOne)
                return;
            ++GameMode.numMatchesPlayed;
        }
    }
}
