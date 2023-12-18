using System.Collections;

namespace DuckGame
{
    public class ContextCheckBox : ContextMenu
    {
        private SpriteMap _checkBox;
        private FieldBinding _field;
        public bool isChecked;
        public string path = "";
        public System.Type _myType;

        public ContextCheckBox(
          string text,
          IContextListener owner,
          FieldBinding field,
          System.Type myType,
          string valTooltip)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            _checkBox = new SpriteMap("Editor/checkBox", 16, 16);
            depth = (Depth)0.8f;
            _myType = myType;
            if (field == null)
                _field = new FieldBinding(this, nameof(isChecked));
            tooltip = valTooltip;
        }

        public ContextCheckBox(string text, IContextListener owner, FieldBinding field = null, System.Type myType = null)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            _checkBox = new SpriteMap("Editor/checkBox", 16, 16);
            depth = (Depth)0.8f;
            _myType = myType;
            if (field != null)
                return;
            _field = new FieldBinding(this, nameof(isChecked));
        }

        public override void Selected()
        {
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor)
            {
                if (_field == null)
                    return;
                if (_field.value is IList)
                {
                    IList list = _field.value as IList;
                    if (list.Contains(_myType))
                        list.Remove(_myType);
                    else
                        list.Add(_myType);
                }
                else
                {
                    _field.value = !(bool)_field.value;
                    Editor.hasUnsavedChanges = true;
                }
            }
            else
            {
                if (_owner == null)
                    return;
                _owner.Selected(this);
            }
        }

        public override void Update() => base.Update();

        public override void Draw()
        {
            if (_hover)
                Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), (Depth)0.82f);
            Graphics.DrawString(_text, position + new Vec2(2f, 5f), Color.White, (Depth)0.85f);
            bool flag = !(_field.value is IList) ? (bool)_field.value : (_field.value as IList).Contains(_myType);
            _checkBox.depth = (Depth)0.9f;
            _checkBox.x = (x + itemSize.x - 16f);
            _checkBox.y = y;
            _checkBox.frame = flag ? 1 : 0;
            _checkBox.Draw();
        }
    }
}
