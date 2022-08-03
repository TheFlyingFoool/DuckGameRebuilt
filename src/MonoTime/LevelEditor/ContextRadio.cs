// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextRadio
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContextRadio : ContextMenu
    {
        private SpriteMap _radioButton;
        private FieldBinding _field;
        private object _index = 0;
        private bool _selected;

        public ContextRadio(
          string text,
          bool selected,
          object index,
          IContextListener owner,
          FieldBinding field = null)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _selected = field == null ? selected : field.value == index;
            _field = field;
            _index = index;
            depth = (Depth)0.8f;
            _radioButton = new SpriteMap("Editor/radioButton", 16, 16);
            itemSize.x = Graphics.GetFancyStringWidth(_text) + 40f;
            if (index == null || (object)(index as System.Type) == null)
                return;
            _image = Editor.GetThing(index as System.Type).GeneratePreview(transparentBack: true);
            itemSize.x += 32f;
        }

        public override void Selected()
        {
            if (greyOut)
                return;
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor)
            {
                if (_field == null)
                    return;
                _field.value = _index;
                Editor.hasUnsavedChanges = true;
            }
            else
            {
                if (_owner == null)
                    return;
                _owner.Selected(this);
            }
        }

        public override void Update()
        {
            base.Update();
            if (_field == null)
                return;
            if (_index == null)
                _selected = _field.value == null;
            else if (_field.value != null)
                _selected = _field.value.Equals(_index);
            else
                _selected = false;
        }

        public override void Draw()
        {
            if (_hover && !greyOut)
                Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), (Depth)0.83f);
            Color color = Color.White;
            if (greyOut)
                color = Color.White * 0.3f;
            if (_image != null)
            {
                Graphics.DrawString(_text, position + new Vec2(20f, 5f), color, (Depth)0.85f);
                _image.depth = depth + 3;
                _image.x = x + 1f;
                _image.y = y;
                _image.color = color;
                _image.scale = new Vec2(1f);
                _image.Draw();
            }
            else
                Graphics.DrawString(_text, position + new Vec2(4f, 5f), color, (Depth)0.85f);
            _radioButton.depth = (Depth)0.9f;
            _radioButton.x = (x + itemSize.x - 16f);
            _radioButton.y = y;
            _radioButton.frame = _selected ? 1 : 0;
            _radioButton.color = color;
            _radioButton.Draw();
        }
    }
}
