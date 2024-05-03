using System.Collections.Generic;

namespace DuckGame
{
    public class DrawList
    {
        protected HashSet<Thing> _transparent = new HashSet<Thing>();
        protected HashSet<Thing> _opaque = new HashSet<Thing>();
        protected HashSet<Thing> _transparentRemove = new HashSet<Thing>();
        protected HashSet<Thing> _opaqueRemove = new HashSet<Thing>();

        public void Add(Thing obj)
        {
            if (obj.opaque)
            {
                _opaque.Add(obj);
                _opaqueRemove.Remove(obj);
            }
            else
            {
                _transparent.Add(obj);
                _transparentRemove.Remove(obj);
            }
        }

        public void Remove(Thing obj)
        {
            if (obj.opaque)
                _opaque.Remove(obj);
            else
                _transparent.Remove(obj);
        }

        public void RemoveSoon(Thing obj)
        {
            if (obj.opaque)
                _opaqueRemove.Add(obj);
            else
                _transparentRemove.Add(obj);
        }

        public void Clear()
        {
            _transparent.Clear();
            _transparentRemove.Clear();
            _opaque.Clear();
            _opaqueRemove.Clear();
        }
    }
}
