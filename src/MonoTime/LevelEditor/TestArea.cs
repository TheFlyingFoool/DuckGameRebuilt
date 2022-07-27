// Decompiled with JetBrains decompiler
// Type: DuckGame.TestArea
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._editor = editor;
            this._level = level;
            this._seed = seed;
            this._center = center;
        }

        public override void Initialize()
        {
            if (this._level == "RANDOM")
            {
                LevelGenerator.MakeLevel(allowSymmetry: (this._center.left && this._center.right), seed: this._seed).LoadParts(0.0f, 0.0f, this, this._seed);
            }
            else
            {
                IEnumerable<DXMLNode> source = DuckXML.Load(this._level).Element("Level").Elements("Objects");
                if (source == null)
                    return;
                foreach (DXMLNode element in source.Elements<DXMLNode>("Object"))
                {
                    Thing t = Thing.LegacyLoadThing(element);
                    if (t != null)
                        this.AddThing(t);
                }
            }
        }

        public override void Update() => base.Update();
    }
}
