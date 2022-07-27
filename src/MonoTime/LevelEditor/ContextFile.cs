// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextFile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class ContextFile : ContextMenu
    {
        public FieldBinding _field;
        public string path = "";
        private bool selecting;
        private ContextFileType _type;

        public ContextFile(
          string text,
          IContextListener owner,
          FieldBinding field,
          ContextFileType type,
          string valTooltip)
          : base(owner)
        {
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            this.depth = (Depth)0.8f;
            this._type = type;
            this.fancy = true;
            if (field == null)
                this._field = new FieldBinding(this, "isChecked");
            this.tooltip = valTooltip;
        }

        public ContextFile(
          string text,
          IContextListener owner,
          FieldBinding field = null,
          ContextFileType type = ContextFileType.Level)
          : base(owner)
        {
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            this.depth = (Depth)0.8f;
            this._type = type;
            this.fancy = true;
            if (field != null)
                return;
            this._field = new FieldBinding(this, "isChecked");
        }

        public override void Initialize()
        {
        }

        public override void Terminate()
        {
        }

        public override void Selected()
        {
            if (this._field != null && this._field.value is string)
            {
                object obj = this._field.value;
            }
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor current)
            {
                if (this._type == ContextFileType.Block)
                    current.fileDialog.Open(DuckFile.customBlockDirectory, DuckFile.customBlockDirectory, false, loadLevel: false, type: this._type);
                else if (this._type == ContextFileType.Background)
                    current.fileDialog.Open(DuckFile.customBackgroundDirectory, DuckFile.customBackgroundDirectory, false, loadLevel: false, type: this._type);
                else if (this._type == ContextFileType.Platform)
                    current.fileDialog.Open(DuckFile.customPlatformDirectory, DuckFile.customPlatformDirectory, false, loadLevel: false, type: this._type);
                else if (this._type == ContextFileType.Parallax)
                    current.fileDialog.Open(DuckFile.customParallaxDirectory, DuckFile.customParallaxDirectory, false, loadLevel: false, type: this._type);
                else if (this._type == ContextFileType.ArcadeStyle || this._type == ContextFileType.ArcadeAnimation)
                    current.fileDialog.Open(DuckFile.customArcadeDirectory, DuckFile.customArcadeDirectory, false, loadLevel: false, type: this._type);
                else
                    current.fileDialog.Open(Editor.initialDirectory, Editor.initialDirectory, false, loadLevel: false);
                this.selecting = true;
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
            if (!(Level.current is Editor current) || current.fileDialog.opened)
                return;
            if (this.selecting && current.fileDialog.result != null)
            {
                this.selecting = false;
                string path = current.fileDialog.rootFolder + current.fileDialog.result;
                if (this._type == ContextFileType.Level)
                {
                    LevelData levelData = DuckFile.LoadLevel(path);
                    this._field.value = levelData == null ? current.fileDialog.result.Substring(1, current.fileDialog.result.Length - 5) : (object)levelData.metaData.guid;
                }
                else
                    this._field.value = !current.fileDialog.result.StartsWith("/") ? current.fileDialog.result.Substring(0, current.fileDialog.result.Length - 4) : (object)current.fileDialog.result.Substring(1, current.fileDialog.result.Length - 5);
                Editor.hasUnsavedChanges = true;
                current.fileDialog.result = null;
            }
            if (this.selecting && !current.fileDialog.opened)
                this.selecting = false;
            base.Update();
        }

        public override void Draw()
        {
            string guid = "";
            if (this._field != null && this._field.value is string)
                guid = this._field.value as string;
            LevelData level = Content.GetLevel(guid);
            if (level != null)
                guid = level.GetPath();
            if (this._hover)
            {
                Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), (Depth)0.83f);
                if (guid.Length > 0)
                {
                    Vec2 vec2 = new Vec2(this.x, this.y);
                    vec2.x += this.itemSize.x + 4f;
                    vec2.y -= 2f;
                    int startIndex = guid.LastIndexOf("/") + 1;
                    string str = guid.Substring(startIndex, guid.Length - startIndex);
                    if (str.Length > 20)
                        str = str.Substring(0, 20);
                    Graphics.DrawString(str + "...", this.position + new Vec2(2f, 5f), Color.White, (Depth)0.85f);
                }
                else
                    Graphics.DrawString("NO FILE", this.position + new Vec2(2f, 5f), Color.White, (Depth)0.85f);
            }
            else if (guid.Length > 0)
            {
                int startIndex = guid.LastIndexOf("/") + 1;
                string text = guid.Substring(startIndex, guid.Length - startIndex);
                if (text.Length > 20)
                    text = text.Substring(0, 20);
                Graphics.DrawString(text, this.position + new Vec2(2f, 5f), Color.LimeGreen, (Depth)0.85f);
            }
            else
                Graphics.DrawString(this._text, this.position + new Vec2(2f, 5f), Color.Red, (Depth)0.85f);
        }
    }
}
