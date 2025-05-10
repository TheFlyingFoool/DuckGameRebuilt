namespace DuckGame
{
    public class NMSpawnDuck : NMEvent
    {
        public byte index;

        public NMSpawnDuck(byte idx) => index = idx;

        public NMSpawnDuck()
        {
        }

        public override void Activate()
        {
            if (index < 0 || index >= DuckNetwork.profiles.Count)
                return;
            Profile profile = DuckNetwork.profiles[index];
            if (profile == null || profile.duck == null || profile.persona == null)
                return;
            if (profile.localPlayer)
                profile.duck.connection = DuckNetwork.localConnection;
            else
                profile.duck.connection = profile.connection;
            profile.duck.visible = true;
            Vec3 color = profile.persona.color;
            Level.Add(new SpawnLine(profile.duck.x, profile.duck.y, 0, 0f, new Color((int)color.x, (int)color.y, (int)color.z), 32f));
            Level.Add(new SpawnLine(profile.duck.x, profile.duck.y, 0, -4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
            Level.Add(new SpawnLine(profile.duck.x, profile.duck.y, 0, 4f, new Color((int)color.x, (int)color.y, (int)color.z), 4f));
            SFX.Play("pullPin", 0.7f);
        }
    }
}
