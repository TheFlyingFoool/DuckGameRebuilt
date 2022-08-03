// Decompiled with JetBrains decompiler
// Type: DuckGame.CollisionTree
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DuckGame
{
    public class CollisionTree
    {
        private Vec2 _position;
        private float _width;
        private CollisionTree[] _children;
        private SpriteFont _font;
        private List<Thing> _objects = new List<Thing>();
        private int _depth;

        public Vec2 position => _position;

        public CollisionTree(float xpos, float ypos, float wval, int divisions)
        {
            _depth = divisions;
            _position = new Vec2(xpos, ypos);
            _width = wval;
            _font = Content.Load<SpriteFont>("font_SuperNew");
            if (_depth <= 0)
                return;
            _children = new CollisionTree[4];
            _children[0] = new CollisionTree(_position.x, _position.y, _width * 0.5f, _depth - 1);
            _children[1] = new CollisionTree(_position.x + _width * 0.5f, _position.y, _width * 0.5f, _depth - 1);
            _children[2] = new CollisionTree(_position.x, _position.y + _width * 0.5f, _width * 0.5f, _depth - 1);
            _children[3] = new CollisionTree(_position.x + _width * 0.5f, _position.y + _width * 0.5f, _width * 0.5f, _depth - 1);
        }

        public void Add(Thing t)
        {
        }

        public void Draw()
        {
        }
    }
}
