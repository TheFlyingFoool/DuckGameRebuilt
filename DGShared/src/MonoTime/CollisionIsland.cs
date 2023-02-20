// Decompiled with JetBrains decompiler
// Type: DuckGame.CollisionIsland
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class CollisionIsland
    {
        private HashSet<MaterialThing> _things = new HashSet<MaterialThing>();
        public MaterialThing owner;
        public float radius = 200f;
        public float radiusCheck = 220f;
        public float radiusSquared;
        public float radiusCheckSquared;
        public QuadTreeObjectList level;
        public bool willDie;

        public HashSet<MaterialThing> things => _things;

        public CollisionIsland(MaterialThing own, QuadTreeObjectList lev)
        {
            radiusSquared = radius * radius;
            radiusCheckSquared = radiusCheck * radiusCheck;
            owner = own;
            level = lev;
            AddThing(own);
        }

        public void KillIsland() => level.RemoveIsland(this);

        public void AddThing(MaterialThing thing)
        {
            if (thing.island != null && thing.island != this)
                thing.island.RemoveThing(thing);
            thing.island = this;
            _things.Add(thing);
        }

        public void RemoveThing(MaterialThing thing)
        {
            _things.Remove(thing);
            thing.island = null;
            if (thing != owner)
                return;
            KillIsland();
        }
    }
}
