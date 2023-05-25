// Decompiled with JetBrains decompiler
// Type: DuckGame.ToolbarButton
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            get => _hover;
            set => _hover = value;
        }

        public ToolbarButton(ContextToolbarItem owner, int image, string ht)
          : base()
        {
            _owner = owner;
            layer = Editor.objectMenuLayer;
            if (image == 99)
            {
                graphic = new Sprite("steamIcon")
                {
                    scale = new Vec2(0.5f, 0.5f)
                };
            }
            else
                graphic = new SpriteMap("iconSheet", 16, 16)
                {
                    frame = image
                };
            hoverText = ht;
            depth = (Depth)0.88f;
        }

        public override void Update()
        {
            Rectangle pRect = new Rectangle(position.x, position.y, 16f, 16f);
            switch (Editor.inputMode)
            {
                case EditorInput.Mouse:
                    if (Mouse.x > x && Mouse.x < x + 16f && Mouse.y > y && Mouse.y < y + 16f)
                    {
                        _owner.toolBarToolTip = hoverText;
                        _hover = true;
                        if (Mouse.left != InputState.Pressed)
                            return;
                        _owner.toolBarToolTip = null;
                        Editor.clickedMenu = true;
                        _owner.ButtonPressed(this);
                        return;
                    }
                    _hover = false;
                    return;
                case EditorInput.Touch:
                    if (TouchScreen.GetTap().Check(pRect, layer.camera))
                    {
                        _hover = true;
                        Editor.clickedMenu = true;
                        _owner.ButtonPressed(this);
                        return;
                    }
                    break;
            }
            if (!_hover)
                return;
            _owner.toolBarToolTip = hoverText;
        }

        public override void Draw()
        {
            Graphics.DrawRect(position, position + new Vec2(16f, 16f), _hover ? new Color(170, 170, 170) : new Color(70, 70, 70), (Depth)0.87f);
            graphic.position = position;
            graphic.depth = (Depth)0.88f;
            graphic.Draw();
        }
    }
}
