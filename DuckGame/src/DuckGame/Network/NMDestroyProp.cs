namespace DuckGame
{
    public class NMDestroyProp : NMEvent
    {
        public Thing prop;
        private byte _levelIndex;

        public NMDestroyProp(Thing t)
        {
            priority = NetMessagePriority.UnreliableUnordered;
            prop = t;
        }

        public NMDestroyProp()
        {
        }

        public override void Activate()
        {
            if (!(Level.current is GameLevel) || DuckNetwork.levelIndex != _levelIndex || !(this.prop is MaterialThing prop))
                return;
            prop.NetworkDestroy();
        }

        protected override void OnSerialize()
        {
            base.OnSerialize();
            _serializedData.Write(DuckNetwork.levelIndex);
        }

        public override void OnDeserialize(BitBuffer d)
        {
            base.OnDeserialize(d);
            _levelIndex = d.ReadByte();
        }
    }
}
