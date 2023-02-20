// Decompiled with JetBrains decompiler
// Type: DuckGame.TestArea
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class TestArea : Level
    {
        private Editor _editor;
        protected int _seed;
        protected RandomLevelData _center;

        public TestArea(Editor editor, string level, int seed = 0, RandomLevelData center = null)
        {
            _editor = editor;
            _level = level;
            _seed = seed;
            _center = center;
        }

        public override void Initialize()
        {
            if (_level == "RANDOM")
            {
                LevelGenerator.MakeLevel(allowSymmetry: (_center.left && _center.right), seed: _seed).LoadParts(0f, 0f, this, _seed);
            }
            else
            {
                IEnumerable<DXMLNode> source = DuckXML.Load(_level).Element("Level").Elements("Objects");
                if (source == null)
                    return;
                foreach (DXMLNode element in source.Elements("Object"))
                {
                    Thing t = Thing.LegacyLoadThing(element);
                    if (t != null)
                        AddThing(t);
                }
            }
        }

        public override void Update() => base.Update();
    }
}
