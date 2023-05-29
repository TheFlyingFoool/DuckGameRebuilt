namespace DuckGame
{
    public class InstrumentNote
    {
        public Sound sound;
        public bool pressed = true;
        public float hitPitch;

        public InstrumentNote(Sound s) => sound = s;
    }
}
