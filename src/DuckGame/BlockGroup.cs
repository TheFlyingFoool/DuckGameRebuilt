// Decompiled with JetBrains decompiler
// Type: DuckGame.BlockGroup
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class BlockGroup : AutoBlock
    {
        private List<Block> _blocks = new List<Block>();
        private bool _wreck;

        public List<Block> blocks => _blocks;

        public override void SetTranslation(Vec2 translation)
        {
            foreach (Thing block in _blocks)
                block.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public void Add(Block b)
        {
            _blocks.Add(b);
            b.group = this;
            _impactThreshold = Math.Min(_impactThreshold, b.impactThreshold);
            willHeat = willHeat || b.willHeat;
            if (!(b is AutoBlock))
                return;
            _tileset = (b as AutoBlock)._tileset;
        }

        public void Remove(Block b)
        {
            _blocks.Remove(b);
            _wreck = true;
        }

        public BlockGroup()
          : base(0f, 0f, "")
        {
            _isStatic = true;
        }

        public void CalculateSize()
        {
            Vec2 vec2_1 = new Vec2(99999f, 99999f);
            Vec2 vec2_2 = new Vec2(-99999f, -99999f);
            foreach (Block block in _blocks)
            {
                if (block.left < vec2_1.x)
                    vec2_1.x = block.left;
                if (block.right > vec2_2.x)
                    vec2_2.x = block.right;
                if (block.top < vec2_1.y)
                    vec2_1.y = block.top;
                if (block.bottom > vec2_2.y)
                    vec2_2.y = block.bottom;
                physicsMaterial = block.physicsMaterial;
                thickness = block.thickness;
            }
            position = (vec2_1 + vec2_2) / 2f;
            collisionOffset = vec2_1 - position;
            collisionSize = vec2_2 - vec2_1;
        }

        public void Wreck()
        {
            foreach (Thing block in _blocks)
                Level.Add(block);
            Level.Remove(this);
        }

        public override void Update()
        {
            if (_wreck)
            {
                foreach (Thing block in _blocks)
                    Level.Add(block);
                Level.Remove(this);
                _wreck = false;
            }
            if (needsRefresh)
            {
                foreach (Block block in _blocks)
                {
                    if (block is AutoBlock)
                        (block as AutoBlock).PlaceBlock();
                }
                needsRefresh = false;
            }
            base.Update();
        }
        public override void Initialize()
        {
            _level.AddUpdateOnce(this);
            shouldbeinupdateloop = false;
            //base.Initialize();
        }
        public override void PreLevelInitialize()
        {
        }
        public override List<BlockCorner> GetGroupCorners() => _blocks.Count > 0 ? _blocks[0].GetGroupCorners() : base.GetGroupCorners();

        public override void Draw()
        {
            foreach (Thing block in _blocks)
                block.Draw();
            if (!DevConsole.showCollision)
                return;
            Graphics.DrawRect(topLeft + new Vec2(-0.5f, 0.5f), bottomRight + new Vec2(0.5f, -0.5f), Color.Green * 0.5f, (Depth)1f);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            foreach (Block block in _blocks)
            {
                if (Collision.Rect(block.topLeft, block.bottomRight, with))
                    block.OnSolidImpact(with, from);
            }
        }

        public override void HeatUp(Vec2 location)
        {
            if (willHeat)
            {
                foreach (Block block in _blocks)
                {
                    if (Collision.Circle(location, 3f, block))
                        block.HeatUp(location);
                }
            }
            base.HeatUp(location);
        }

        public override void Terminate()
        {
            foreach (Block block in _blocks)
            {
                block.groupedWithNeighbors = false;
                block.Terminate();
            }
        }
    }
}
