// Decompiled with JetBrains decompiler
// Type: DuckGame.BlockGroup
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public List<Block> blocks => this._blocks;

        public override void SetTranslation(Vec2 translation)
        {
            foreach (Thing block in this._blocks)
                block.SetTranslation(translation);
            base.SetTranslation(translation);
        }

        public void Add(Block b)
        {
            this._blocks.Add(b);
            b.group = this;
            this._impactThreshold = Math.Min(this._impactThreshold, b.impactThreshold);
            this.willHeat = this.willHeat || b.willHeat;
            if (!(b is AutoBlock))
                return;
            this._tileset = (b as AutoBlock)._tileset;
        }

        public void Remove(Block b)
        {
            this._blocks.Remove(b);
            this._wreck = true;
        }

        public BlockGroup()
          : base(0.0f, 0.0f, "")
        {
            this._isStatic = true;
        }

        public void CalculateSize()
        {
            Vec2 vec2_1 = new Vec2(99999f, 99999f);
            Vec2 vec2_2 = new Vec2(-99999f, -99999f);
            foreach (Block block in this._blocks)
            {
                if ((double)block.left < (double)vec2_1.x)
                    vec2_1.x = block.left;
                if ((double)block.right > (double)vec2_2.x)
                    vec2_2.x = block.right;
                if ((double)block.top < (double)vec2_1.y)
                    vec2_1.y = block.top;
                if ((double)block.bottom > (double)vec2_2.y)
                    vec2_2.y = block.bottom;
                this.physicsMaterial = block.physicsMaterial;
                this.thickness = block.thickness;
            }
            this.position = (vec2_1 + vec2_2) / 2f;
            this.collisionOffset = vec2_1 - this.position;
            this.collisionSize = vec2_2 - vec2_1;
        }

        public void Wreck()
        {
            foreach (Thing block in this._blocks)
                Level.Add(block);
            Level.Remove((Thing)this);
        }

        public override void Update()
        {
            if (this._wreck)
            {
                foreach (Thing block in this._blocks)
                    Level.Add(block);
                Level.Remove((Thing)this);
                this._wreck = false;
            }
            if (this.needsRefresh)
            {
                foreach (Block block in this._blocks)
                {
                    if (block is AutoBlock)
                        (block as AutoBlock).PlaceBlock();
                }
                this.needsRefresh = false;
            }
            base.Update();
        }

        public override List<BlockCorner> GetGroupCorners() => this._blocks.Count > 0 ? this._blocks[0].GetGroupCorners() : base.GetGroupCorners();

        public override void Draw()
        {
            foreach (Thing block in this._blocks)
                block.Draw();
            if (!DevConsole.showCollision)
                return;
            Graphics.DrawRect(this.topLeft + new Vec2(-0.5f, 0.5f), this.bottomRight + new Vec2(0.5f, -0.5f), Color.Green * 0.5f, (Depth)1f);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            foreach (Block block in this._blocks)
            {
                if (Collision.Rect(block.topLeft, block.bottomRight, (Thing)with))
                    block.OnSolidImpact(with, from);
            }
        }

        public override void HeatUp(Vec2 location)
        {
            if (this.willHeat)
            {
                foreach (Block block in this._blocks)
                {
                    if (Collision.Circle(location, 3f, (Thing)block))
                        block.HeatUp(location);
                }
            }
            base.HeatUp(location);
        }

        public override void Terminate()
        {
            foreach (Block block in this._blocks)
            {
                block.groupedWithNeighbors = false;
                block.Terminate();
            }
        }
    }
}
