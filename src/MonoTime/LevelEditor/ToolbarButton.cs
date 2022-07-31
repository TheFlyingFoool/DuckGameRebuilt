// Decompiled with JetBrains decompiler
// Type: DuckGame.ToolbarButton
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ToolbarButton : Thing
    {
        private ContextToolbarItem _owner;
        private bool _hover;
        public string hoverText = "";

        public bool hover
        {
            get => this._hover;
            set => this._hover = value;
        }

        public ToolbarButton(ContextToolbarItem owner, int image, string ht)
          : base()
        {
            this._owner = owner;
            this.layer = Editor.objectMenuLayer;
            if (image == 99)
            {
                this.graphic = new Sprite("steamIcon")
                {
                    scale = new Vec2(0.5f, 0.5f)
                };
            }
            else
                this.graphic = new SpriteMap("iconSheet", 16, 16)
                {
                    frame = image
                };
            this.hoverText = ht;
            this.depth = (Depth)0.88f;
        }

        public override void Update()
        {
            Rectangle pRect = new Rectangle(this.position.x, this.position.y, 16f, 16f);
            switch (Editor.inputMode)
            {
                case EditorInput.Mouse:
                    if (Mouse.x > this.x && Mouse.x < this.x + 16.0 && Mouse.y > this.y && Mouse.y < this.y + 16.0)
                    {
                        this._owner.toolBarToolTip = this.hoverText;
                        this._hover = true;
                        if (Mouse.left != InputState.Pressed)
                            return;
                        this._owner.toolBarToolTip = null;
                        Editor.clickedMenu = true;
                        this._owner.ButtonPressed(this);
                        return;
                    }
                    this._hover = false;
                    return;
                case EditorInput.Touch:
                    if (TouchScreen.GetTap().Check(pRect, this.layer.camera))
                    {
                        this._hover = true;
                        Editor.clickedMenu = true;
                        this._owner.ButtonPressed(this);
                        return;
                    }
                    break;
            }
            if (!this._hover)
                return;
            this._owner.toolBarToolTip = this.hoverText;
        }

        public override void Draw()
        {
            Graphics.DrawRect(this.position, this.position + new Vec2(16f, 16f), this._hover ? new Color(170, 170, 170) : new Color(70, 70, 70), (Depth)0.87f);
            this.graphic.position = this.position;
            this.graphic.depth = (Depth)0.88f;
            this.graphic.Draw();
        }
    }
}
