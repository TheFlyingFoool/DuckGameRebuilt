// Decompiled with JetBrains decompiler
// Type: DuckGame.DrawList
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
                this._opaque.Add(obj);
                this._opaqueRemove.Remove(obj);
            }
            else
            {
                this._transparent.Add(obj);
                this._transparentRemove.Remove(obj);
            }
        }

        public void Remove(Thing obj)
        {
            if (obj.opaque)
                this._opaque.Remove(obj);
            else
                this._transparent.Remove(obj);
        }

        public void RemoveSoon(Thing obj)
        {
            if (obj.opaque)
                this._opaqueRemove.Add(obj);
            else
                this._transparentRemove.Add(obj);
        }

        public void Clear()
        {
            this._transparent.Clear();
            this._transparentRemove.Clear();
            this._opaque.Clear();
            this._opaqueRemove.Clear();
        }
    }
}
