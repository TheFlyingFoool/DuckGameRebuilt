// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextCheckBox
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            this._checkBox = new SpriteMap("Editor/checkBox", 16, 16);
            this.depth = (Depth)0.8f;
            this._myType = myType;
            if (field == null)
                this._field = new FieldBinding((object)this, nameof(isChecked));
            this.tooltip = valTooltip;
        }

        public ContextCheckBox(string text, IContextListener owner, FieldBinding field = null, System.Type myType = null)
          : base(owner)
        {
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            this._checkBox = new SpriteMap("Editor/checkBox", 16, 16);
            this.depth = (Depth)0.8f;
            this._myType = myType;
            if (field != null)
                return;
            this._field = new FieldBinding((object)this, nameof(isChecked));
        }

        public override void Selected()
        {
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor)
            {
                if (this._field == null)
                    return;
                if (this._field.value is IList)
                {
                    IList list = this._field.value as IList;
                    if (list.Contains((object)this._myType))
                        list.Remove((object)this._myType);
                    else
                        list.Add((object)this._myType);
                }
                else
                {
                    this._field.value = (object)!(bool)this._field.value;
                    Editor.hasUnsavedChanges = true;
                }
            }
            else
            {
                if (this._owner == null)
                    return;
                this._owner.Selected((ContextMenu)this);
            }
        }

        public override void Update() => base.Update();

        public override void Draw()
        {
            if (this._hover)
                Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), (Depth)0.82f);
            Graphics.DrawString(this._text, this.position + new Vec2(2f, 5f), Color.White, (Depth)0.85f);
            bool flag = !(this._field.value is IList) ? (bool)this._field.value : (this._field.value as IList).Contains((object)this._myType);
            this._checkBox.depth = (Depth)0.9f;
            this._checkBox.x = (float)((double)this.x + (double)this.itemSize.x - 16.0);
            this._checkBox.y = this.y;
            this._checkBox.frame = flag ? 1 : 0;
            this._checkBox.Draw();
        }
    }
}
