using System.Linq;

namespace DuckGame
{
    public class NMSetPersona : NMEvent
    {
        public Profile profile;
        public byte persona;

        public NMSetPersona()
        {
        }

        public NMSetPersona(Profile pProfile, DuckPersona pPersona)
        {
            profile = pProfile;
            persona = (byte)pPersona.index;
        }

        public override void Activate()
        {
            if (profile == null || persona < 0 || persona >= Persona.alllist.Count)
                return;
            profile.persona = Persona.alllist[persona];
            if (profile.inputProfile != null && profile.inputProfile.lastActiveDevice != null && profile.persona != null)
            {
                profile.inputProfile.lastActiveDevice.SetLightBar(profile.persona.colorUsable);
            }
        }
    }
}
