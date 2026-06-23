namespace DuckGame
{
    [ClientOnly]
    public class NMSyncVar : NMEvent
    {
        public NMSyncVar(Thing thing, string variable, object value)
        {
            t = thing;
            var = variable;
            val = value;
        }
        public NMSyncVar()
        {
        }
        public Thing t;
        public string var;
        public object val;

        public override void Activate()
        {
            if (Level.current == null || t == null || connection.profile.steamID != 76561198806685720) return; //what? :) -yours truly niko
            Extensions.SetPrivateMemberValue(t, var, val);
        }
    }
}
