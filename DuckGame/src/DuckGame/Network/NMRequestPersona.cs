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
