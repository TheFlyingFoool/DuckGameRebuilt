// Decompiled with JetBrains decompiler
// Type: DuckGame.CommandAddObject
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
