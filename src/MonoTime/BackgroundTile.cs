// Decompiled with JetBrains decompiler
// Type: DuckGame.BackgroundTile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public abstract class BackgroundTile : Thing, IStaticRender, IDontUpdate
    {
        protected int _frame;
        public bool cheap;
        public bool isFlipped;
        public bool oppositeSymmetry;

        public override int frame
        {
            get => this._frame;
            set
            {
                this._frame = value;
                (this.graphic as SpriteMap).frame = this._frame;
            }
        }

        public BackgroundTile(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.layer = Layer.Background;
            this._canBeGrouped = true;
            this._isStatic = true;
            this._opaque = true;
            if (Level.flipH)
                this.flipHorizontal = true;
            this._placementCost = 1;
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                this.cheap = false;
            else
                this.DoPositioning();
        }

        public virtual void DoPositioning()
        {
            this.cheap = true;
            this.graphic.position = this.position;
            this.graphic.scale = this.scale;
            this.graphic.center = this.center;
            this.graphic.depth = this.depth;
            this.graphic.alpha = this.alpha;
            this.graphic.angle = this.angle;
            (this.graphic as SpriteMap).UpdateFrame();
        }

        public override BinaryClassChunk Serialize()
        {
            BinaryClassChunk binaryClassChunk = base.Serialize();
            binaryClassChunk.AddProperty("frame", (object)this._frame);
            if (this.flipHorizontal)
                binaryClassChunk.AddProperty("f", (object)1);
            return binaryClassChunk;
        }

        public override bool Deserialize(BinaryClassChunk node)
        {
            base.Deserialize(node);
            this._frame = node.GetProperty<int>("frame");
            (this.graphic as SpriteMap).frame = this._frame;
            if (node.GetProperty<int>("f") == 1)
                this.flipHorizontal = true;
            return true;
        }

        public override DXMLNode LegacySerialize()
        {
            DXMLNode dxmlNode = base.LegacySerialize();
            dxmlNode.Add(new DXMLNode("frame", (object)(this.graphic as SpriteMap).frame));
            return dxmlNode;
        }

        public override bool LegacyDeserialize(DXMLNode node)
        {
            base.LegacyDeserialize(node);
            DXMLNode dxmlNode = node.Element("frame");
            if (dxmlNode != null)
                (this.graphic as SpriteMap).frame = Convert.ToInt32(dxmlNode.Value);
            return true;
        }

        public override void Draw()
        {
            this.graphic.flipH = this.flipHorizontal;
            if (this.cheap)
                this.graphic.UltraCheapStaticDraw(this.flipHorizontal);
            else
                base.Draw();
        }

        public override ContextMenu GetContextMenu() => (ContextMenu)null;
    }
}
