namespace DuckGame
{
    public class DoorVessel : SomethingSomethingVessel
    {
        public DoorVessel(Thing th) : base(th)
        {
            tatchedTo.Add(typeof(Door));
            tatchedTo.Add(typeof(FlimsyDoor));
            AddSynncl("locked", new SomethingSync(typeof(bool)));
            AddSynncl("hitpoints", new SomethingSync(typeof(float)));
        }
        public override SomethingSomethingVessel RecDeserialize(BitBuffer b)
        {
            Vec2 WHAT_THE_VARIABLE_V_IS_ALREADY_IN_USE_I_BETTER_MAKE_A_CONVENIENT_NAME_FOR_THIS_VARIABLE = b.ReadVec2();
            bool WHAT_THE_VARIABLE_B_IS_IN_USE = b.ReadBool();
            bool flimsy = b.ReadBool();
            if (flimsy)
            {
                DoorVessel v = new DoorVessel(new FlimsyDoor(WHAT_THE_VARIABLE_V_IS_ALREADY_IN_USE_I_BETTER_MAKE_A_CONVENIENT_NAME_FOR_THIS_VARIABLE.x, WHAT_THE_VARIABLE_V_IS_ALREADY_IN_USE_I_BETTER_MAKE_A_CONVENIENT_NAME_FOR_THIS_VARIABLE.y));
                ((Door)v.t).locked = WHAT_THE_VARIABLE_B_IS_IN_USE;
                return v;
            }
            else
            {
                DoorVessel v = new DoorVessel(new Door(WHAT_THE_VARIABLE_V_IS_ALREADY_IN_USE_I_BETTER_MAKE_A_CONVENIENT_NAME_FOR_THIS_VARIABLE.x, WHAT_THE_VARIABLE_V_IS_ALREADY_IN_USE_I_BETTER_MAKE_A_CONVENIENT_NAME_FOR_THIS_VARIABLE.y));
                ((Door)v.t).locked = WHAT_THE_VARIABLE_B_IS_IN_USE;
                return v;
            }
        }
        public override BitBuffer RecSerialize(BitBuffer prevBuffer)
        {
            prevBuffer.Write(t.position);
            prevBuffer.Write(((Door)t)._lockDoor);
            prevBuffer.Write(t is FlimsyDoor);
            return prevBuffer;
        }
        public override void PlaybackUpdate()
        {
            Door d = (Door)t;
            d.locked = (bool)valOf("locked");
            d._hitPoints = (float)valOf("hitpoints");
            base.PlaybackUpdate();
        }
        public override void RecordUpdate()
        {
            Door d = (Door)t;
            addVal("locked", d.locked);
            addVal("hitpoints", d._hitPoints);
        }
    }
}
