// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextCheckBox
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContextButton : ContextMenu
    {
        private SpriteMap _checkBox;
        private FunctionBinding _method;
        public bool isChecked;
        public string path = "";
        public System.Type _myType;

        public ContextButton(
          string text,
          IContextListener owner,
          FunctionBinding field,
          System.Type myType,
          string valTooltip)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _method = field;
            _checkBox = new SpriteMap("Editor/checkBox", 16, 16);
            depth = (Depth)0.8f;
            _myType = myType;
            if (field == null)
                _method = new FunctionBinding(this, nameof(isChecked));
            tooltip = valTooltip;
        }

        public ContextButton(string text, IContextListener owner, FunctionBinding method = null, System.Type myType = null)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _method = method;
            _checkBox = new SpriteMap("Editor/checkBox", 16, 16);
            depth = (Depth)0.8f;
            _myType = myType;
            if (method != null)
                return;
            _method = new FunctionBinding(this, nameof(isChecked));
        }

        public override void Selected()
        {
            SFX.Play("highClick", 0.3f, 0.2f);
            _method.Call();
        }

        public override void Update() => base.Update();

        public override void Draw()
        {
            if (_hover)
                Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), (Depth)0.82f);
            Graphics.DrawString(_text, position + new Vec2(2f, 5f), Color.White, (Depth)0.85f);
            //bool flag = true;
            //_checkBox.depth = (Depth)0.9f;
            //_checkBox.x = (x + itemSize.x - 16f);
            //_checkBox.y = y;
            //_checkBox.frame = flag ? 1 : 0;
            //_checkBox.Draw();
        }
    }
}
