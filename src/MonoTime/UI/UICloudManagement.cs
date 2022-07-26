// Decompiled with JetBrains decompiler
// Type: DuckGame.UICloudManagement
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class UICloudManagement : UIMenu
    {
        private List<UICloudManagement.File> _flagged = new List<UICloudManagement.File>();
        private Sprite _downArrow;
        private BitmapFont _littleFont;
        public UICloudManagement.File root = new UICloudManagement.File()
        {
            files = new List<UICloudManagement.File>()
        };
        public UICloudManagement.File profileRoot;
        public UICloudManagement.File currentFolder;
        public UIMenu _deleteMenu;
        private UIMenu _openOnClose;
        private bool _opening;
        private int _topOffset;
        private readonly int kMaxInView = 16;

        public UICloudManagement(UIMenu openOnClose)
          : base("MANAGE CLOUD", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 260f, 180f)
        {
            this.Add((UIComponent)new UIBox(0.0f, 0.0f, 100f, 150f, isVisible: false), true);
            this._littleFont = new BitmapFont("smallBiosFont", 7, 6);
            this._downArrow = new Sprite("cloudDown");
            this._downArrow.CenterOrigin();
            this._deleteMenu = new UIMenu("ARE YOU SURE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@WASD@ADJUST @CANCEL@EXIT");
            this._deleteMenu.Add((UIComponent)new UIText("The selected files will be", Colors.DGBlue), true);
            this._deleteMenu.Add((UIComponent)new UIText("|DGRED|permenantly deleted|DGBLUE| from", Colors.DGBlue), true);
            this._deleteMenu.Add((UIComponent)new UIText("everywhere, |DGRED|forever!", Colors.DGBlue), true);
            this._deleteMenu.Add((UIComponent)new UIText(" ", Colors.DGBlue), true);
            this._deleteMenu.Add((UIComponent)new UIMenuItem("|DGRED|DELETE", (UIMenuAction)new UIMenuActionCallFunctionOpenMenu((UIComponent)this._deleteMenu, (UIComponent)this, new UIMenuActionCallFunctionOpenMenu.Function(this.DeleteFiles)), backButton: true), true);
            this._deleteMenu.Add((UIComponent)new UIMenuItem("|DGGREEN|CANCEL", (UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._deleteMenu, (UIComponent)this), backButton: true), true);
            this._deleteMenu._defaultSelection = 1;
            this._deleteMenu.SetBackFunction((UIMenuAction)new UIMenuActionOpenMenu((UIComponent)this._deleteMenu, (UIComponent)this));
            this._deleteMenu.Close();
            this._openOnClose = openOnClose;
        }

        private void DeleteFiles()
        {
            foreach (UICloudManagement.File pFolder in this._flagged)
            {
                if (pFolder.files != null)
                    this.DeleteFolder(pFolder);
                else
                    DuckFile.Delete(pFolder.fullPath);
            }
        }

        private void DeleteFolder(UICloudManagement.File pFolder)
        {
            foreach (UICloudManagement.File file in pFolder.files)
            {
                if (file.files != null)
                    this.DeleteFolder(file);
                else
                    DuckFile.Delete(file.fullPath);
            }
        }

        private UICloudManagement.File GetFolder(string pPath, string pCloudPath)
        {
            UICloudManagement.File folder = this.root;
            if (pCloudPath.StartsWith("nq500000_"))
            {
                if (this.profileRoot == null)
                {
                    this.profileRoot = new UICloudManagement.File()
                    {
                        parent = this.root,
                        name = Steam.user.id.ToString(),
                        files = new List<UICloudManagement.File>()
                    };
                    this.root.files.Add(this.profileRoot);
                }
                folder = this.profileRoot;
            }
            string str1 = pPath;
            char[] chArray = new char[1] { '/' };
            foreach (string str2 in str1.Split(chArray))
            {
                if (!(str2 == ""))
                {
                    bool flag = false;
                    foreach (UICloudManagement.File file in folder.files)
                    {
                        if (file.name == str2)
                        {
                            folder = file;
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        UICloudManagement.File file = new UICloudManagement.File()
                        {
                            name = str2,
                            files = new List<UICloudManagement.File>()
              {
                new UICloudManagement.File()
                {
                  name = "..",
                  files = new List<UICloudManagement.File>()
                }
              },
                            parent = folder
                        };
                        folder.files.Add(file);
                        folder = file;
                    }
                }
                else
                    break;
            }
            return folder;
        }

        private void SortFiles(UICloudManagement.File pRoot)
        {
            if (pRoot.files == null)
                return;
            pRoot.files = pRoot.files.OrderBy<UICloudManagement.File, bool>((Func<UICloudManagement.File, bool>)(x => x.name != "..")).ThenBy<UICloudManagement.File, bool>((Func<UICloudManagement.File, bool>)(x => x.files == null)).ThenBy<UICloudManagement.File, string>((Func<UICloudManagement.File, string>)(x => x.name)).ToList<UICloudManagement.File>();
            foreach (UICloudManagement.File file in pRoot.files)
                this.SortFiles(file);
        }

        public override void Open()
        {
            HUD.CloseAllCorners();
            this._opening = true;
            this._flagged.Clear();
            this.root = new UICloudManagement.File()
            {
                files = new List<UICloudManagement.File>()
            };
            this.profileRoot = (UICloudManagement.File)null;
            int count = Steam.FileGetCount();
            for (int file = 0; file < count; ++file)
            {
                CloudFile cloudFile = CloudFile.Get(Steam.FileGetName(file), true);
                if (cloudFile != null)
                {
                    string path = cloudFile.cloudPath.Substring(9, cloudFile.cloudPath.Length - 9);
                    this.GetFolder(Path.GetDirectoryName(path).Replace('\\', '/'), cloudFile.cloudPath).files.Add(new UICloudManagement.File()
                    {
                        name = Path.GetFileName(path),
                        fullPath = cloudFile.localPath
                    });
                }
            }
            this.SortFiles(this.root);
            this.currentFolder = this.root;
            base.Open();
        }

        public override void Close()
        {
            HUD.CloseAllCorners();
            base.Close();
        }

        public override void Update()
        {
            if (this.open && !this._opening)
            {
                if (Input.Pressed("MENUUP") && this._selection > 0)
                {
                    --this._selection;
                    if (this._selection < this._topOffset)
                        --this._topOffset;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("MENUDOWN") && this._selection < this.currentFolder.files.Count - 1)
                {
                    ++this._selection;
                    if (this._selection > this._topOffset + this.kMaxInView)
                        ++this._topOffset;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("SELECT") && this.currentFolder.files.Count > 0)
                {
                    if (this.currentFolder.files[this._selection].name == "..")
                        this.SelectFolder(this.currentFolder.parent);
                    else if (this.currentFolder.files[this._selection].files != null)
                        this.SelectFolder(this.currentFolder.files[this._selection]);
                }
                if (Input.Pressed("MENU1") && this.currentFolder.files.Count > 0)
                {
                    if (this._flagged.Contains(this.currentFolder.files[this._selection]))
                        this._flagged.Remove(this.currentFolder.files[this._selection]);
                    else
                        this._flagged.Add(this.currentFolder.files[this._selection]);
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("MENU2") && this._flagged.Count > 0)
                {
                    this._deleteMenu.dirty = true;
                    new UIMenuActionOpenMenu((UIComponent)this, (UIComponent)this._deleteMenu).Activate();
                }
                if (Input.Pressed("CANCEL"))
                {
                    if (this.currentFolder.parent != null)
                        this.SelectFolder(this.currentFolder.parent);
                    else if (this._openOnClose != null)
                        new UIMenuActionOpenMenu((UIComponent)this, (UIComponent)this._openOnClose).Activate();
                    else
                        new UIMenuActionCloseMenu((UIComponent)this).Activate();
                }
            }
            this._opening = false;
            base.Update();
        }

        private void SelectFolder(UICloudManagement.File pFolder)
        {
            this.currentFolder = pFolder;
            this._selection = 0;
            this._topOffset = 0;
            SFX.Play("textLetter", 0.7f);
        }

        public override void Draw()
        {
            if (this.open && this.currentFolder != null)
            {
                Vec2 vec2 = new Vec2(this.x - 124f, this.y - 66f);
                float y = 0.0f;
                int num1 = 0;
                int num2 = 0;
                foreach (UICloudManagement.File file in this.currentFolder.files)
                {
                    if (num1 < this._topOffset)
                    {
                        ++num1;
                    }
                    else
                    {
                        if (this._topOffset > 0)
                        {
                            this._downArrow.flipV = true;
                            Graphics.Draw(this._downArrow, this.x, vec2.y - 2f, (Depth)0.5f);
                        }
                        if (num2 > this.kMaxInView)
                        {
                            this._downArrow.flipV = false;
                            Graphics.Draw(this._downArrow, this.x, vec2.y + y, (Depth)0.5f);
                            break;
                        }
                        string str1 = file.name;
                        if (str1.Length > 31)
                            str1 = str1.Substring(0, 30) + "..";
                        string str2 = file.files == null ? (this._flagged.Contains(file) ? "@DELETEFLAG_ON@" : "@DELETEFLAG_OFF@") + str1 : (this._flagged.Contains(file) ? "@FOLDERDELETEICON@" : "@FOLDERICON@") + str1;
                        this._littleFont.Draw(num1 != this._selection ? " " + str2 : "@SELECTICON@" + str2, vec2 + new Vec2(0.0f, y), Color.White, (Depth)0.5f);
                        y += 8f;
                        ++num1;
                        ++num2;
                    }
                }
                string text = "@CANCEL@BACK";
                if (this.currentFolder.files.Count > 0)
                {
                    if (this.currentFolder.files[this._selection].files != null)
                        text += " @SELECT@OPEN";
                    text = !this._flagged.Contains(this.currentFolder.files[this._selection]) ? text + " @MENU1@FLAG" : text + " @MENU1@UNFLAG";
                }
                if (this._flagged.Count > 0)
                    text += " @MENU2@DELETE";
                this._littleFont.Draw(text, new Vec2(this.x - this._littleFont.GetWidth(text) / 2f, this.y + 74f), Color.White, (Depth)0.5f);
            }
            base.Draw();
        }

        [DebuggerDisplay("{name}")]
        public class File
        {
            public string name;
            public string fullPath;
            public List<UICloudManagement.File> files;
            public UICloudManagement.File parent;
        }
    }
}
