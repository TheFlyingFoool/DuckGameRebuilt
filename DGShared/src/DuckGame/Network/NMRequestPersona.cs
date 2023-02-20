// Decompiled with JetBrains decompiler
// Type: DuckGame.NMRequestPersona
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Linq;

namespace DuckGame
{
    public class NMRequestPersona : NMEvent
    {
        public Profile profile;
        public byte persona;

        public NMRequestPersona()
        {
        }

        public NMRequestPersona(Profile pProfile, DuckPersona pPersona)
        {
            profile = pProfile;
            persona = (byte)pPersona.index;
        }

        public override void Activate()
        {
            if (profile == null || persona < 0 || persona >= Persona.alllist.Count)
                return;
            DuckNetwork.RequestPersona(profile, Persona.alllist[persona]);
        }
    }
}
