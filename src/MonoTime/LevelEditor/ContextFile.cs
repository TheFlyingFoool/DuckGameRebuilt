// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextFile
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.IO;

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
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            depth = (Depth)0.8f;
            _type = type;
            fancy = true;
            if (field == null)
                _field = new FieldBinding(this, "isChecked");
            tooltip = valTooltip;
        }

        public ContextFile(
          string text,
          IContextListener owner,
          FieldBinding field = null,
          ContextFileType type = ContextFileType.Level)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            depth = (Depth)0.8f;
            _type = type;
            fancy = true;
            if (field != null)
                return;
            _field = new FieldBinding(this, "isChecked");
        }

        public override void Initialize()
        {
        }

        public override void Terminate()
        {
        }

        public override void Selected()
        {
            if (_field != null && _field.value is string)
            {
                object obj = _field.value;
            }
            SFX.Play("highClick", 0.3f, 0.2f);
            if (Level.current is Editor current)
            {
                if (_type == ContextFileType.Block)
                    current.fileDialog.Open(DuckFile.customBlockDirectory, DuckFile.customBlockDirectory, false, loadLevel: false, type: _type);
                else if (_type == ContextFileType.Background)
                    current.fileDialog.Open(DuckFile.customBackgroundDirectory, DuckFile.customBackgroundDirectory, false, loadLevel: false, type: _type);
                else if (_type == ContextFileType.Platform)
                    current.fileDialog.Open(DuckFile.customPlatformDirectory, DuckFile.customPlatformDirectory, false, loadLevel: false, type: _type);
                else if (_type == ContextFileType.Parallax)
                    current.fileDialog.Open(DuckFile.customParallaxDirectory, DuckFile.customParallaxDirectory, false, loadLevel: false, type: _type);
                else if (_type == ContextFileType.ArcadeStyle || _type == ContextFileType.ArcadeAnimation)
                    current.fileDialog.Open(DuckFile.customArcadeDirectory, DuckFile.customArcadeDirectory, false, loadLevel: false, type: _type);
                else
                    current.fileDialog.Open(Editor.initialDirectory, Editor.initialDirectory, false, loadLevel: false);
                selecting = true;
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
            if (!(Level.current is Editor current) || current.fileDialog.opened)
                return;
            if (selecting && current.fileDialog.result != null)
            {
                selecting = false;
                string path = current.fileDialog.rootFolder + current.fileDialog.result;
                if (_type == ContextFileType.Level)
                {
                    LevelData levelData = DuckFile.LoadLevel(path);
                    _field.value = levelData == null ? current.fileDialog.result.Substring(1, current.fileDialog.result.Length - 5) : (object)levelData.metaData.guid;
                }
                else
                {
                    if (!(current.fileDialog.result.StartsWith("/") && (!Program.IsLinuxD || !Path.IsPathRooted(current.fileDialog.result))))
                    {
                        _field.value = current.fileDialog.result.Substring(0, current.fileDialog.result.Length - 4);
                    }
                    else
                    {
                        _field.value = current.fileDialog.result.Substring(1, current.fileDialog.result.Length - 5);
                    }
                }
                Editor.hasUnsavedChanges = true;
                current.fileDialog.result = null;
            }
            if (selecting && !current.fileDialog.opened)
                selecting = false;
            base.Update();
        }

        public override void Draw()
        {
            string guid = "";
            if (_field != null && _field.value is string)
                guid = _field.value as string;
            LevelData level = Content.GetLevel(guid);
            if (level != null)
                guid = level.GetPath();
            if (_hover)
            {
                Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), (Depth)0.83f);
                if (guid.Length > 0)
                {
                    Vec2 vec2 = new Vec2(x, y);
                    vec2.x += itemSize.x + 4f;
                    vec2.y -= 2f;
                    int startIndex = guid.LastIndexOf("/") + 1;
                    string str = guid.Substring(startIndex, guid.Length - startIndex);
                    if (str.Length > 20)
                        str = str.Substring(0, 20);
                    Graphics.DrawString(str + "...", position + new Vec2(2f, 5f), Color.White, (Depth)0.85f);
                }
                else
                    Graphics.DrawString("NO FILE", position + new Vec2(2f, 5f), Color.White, (Depth)0.85f);
            }
            else if (guid.Length > 0)
            {
                int startIndex = guid.LastIndexOf("/") + 1;
                string text = guid.Substring(startIndex, guid.Length - startIndex);
                if (text.Length > 20)
                    text = text.Substring(0, 20);
                Graphics.DrawString(text, position + new Vec2(2f, 5f), Color.LimeGreen, (Depth)0.85f);
            }
            else
                Graphics.DrawString(_text, position + new Vec2(2f, 5f), Color.Red, (Depth)0.85f);
        }
    }
}
