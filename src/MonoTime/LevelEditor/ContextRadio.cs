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
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._selected = field == null ? selected : field.value == index;
            this._field = field;
            this._index = index;
            this.depth = (Depth)0.8f;
            this._radioButton = new SpriteMap("Editor/radioButton", 16, 16);
            this.itemSize.x = Graphics.GetFancyStringWidth(this._text) + 40f;
            if (index == null || (object)(index as System.Type) == null)
                return;
            this._image = Editor.GetThing(index as System.Type).GeneratePreview(transparentBack: true);
            this.itemSize.x += 32f;
        }

        public override void Selected()
        {
            if (this.greyOut)
                return;
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor)
            {
                if (this._field == null)
                    return;
                this._field.value = this._index;
                Editor.hasUnsavedChanges = true;
            }
            else
            {
                if (this._owner == null)
                    return;
                this._owner.Selected(this);
            }
        }

        public override void Update()
        {
            base.Update();
            if (this._field == null)
                return;
            if (this._index == null)
                this._selected = this._field.value == null;
            else if (this._field.value != null)
                this._selected = this._field.value.Equals(this._index);
            else
                this._selected = false;
        }

        public override void Draw()
        {
            if (this._hover && !this.greyOut)
                Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), (Depth)0.83f);
            Color color = Color.White;
            if (this.greyOut)
                color = Color.White * 0.3f;
            if (this._image != null)
            {
                Graphics.DrawString(this._text, this.position + new Vec2(20f, 5f), color, (Depth)0.85f);
                this._image.depth = this.depth + 3;
                this._image.x = this.x + 1f;
                this._image.y = this.y;
                this._image.color = color;
                this._image.scale = new Vec2(1f);
                this._image.Draw();
            }
            else
                Graphics.DrawString(this._text, this.position + new Vec2(4f, 5f), color, (Depth)0.85f);
            this._radioButton.depth = (Depth)0.9f;
            this._radioButton.x = (float)((double)this.x + itemSize.x - 16.0);
            this._radioButton.y = this.y;
            this._radioButton.frame = this._selected ? 1 : 0;
            this._radioButton.color = color;
            this._radioButton.Draw();
        }
    }
}
