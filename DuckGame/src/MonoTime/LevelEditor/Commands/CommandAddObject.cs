namespace DuckGame
{
    public class CommandAddObject : Command
    {
        private Thing _object;

        public CommandAddObject(Thing obj) => _object = obj;

        public override void OnDo()
        {
            if (!(Level.current is Editor current))
                return;
            current.AddObject(_object);
        }

        public override void OnUndo()
        {
            if (!(Level.current is Editor current))
                return;
            current.RemoveObject(_object);
        }
    }
}
