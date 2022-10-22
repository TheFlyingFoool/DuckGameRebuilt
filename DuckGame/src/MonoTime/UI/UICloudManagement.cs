// Decompiled with JetBrains decompiler
// Type: DuckGame.UICloudManagement
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            Add(new UIBox(0f, 0f, 100f, 150f, isVisible: false), true);
            _littleFont = new BitmapFont("smallBiosFont", 7, 6);
            _downArrow = new Sprite("cloudDown");
            _downArrow.CenterOrigin();
            _deleteMenu = new UIMenu("ARE YOU SURE?", Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 280f, conString: "@WASD@ADJUST @CANCEL@EXIT");
            _deleteMenu.Add(new UIText("The selected files will be", Colors.DGBlue), true);
            _deleteMenu.Add(new UIText("|DGRED|permenantly deleted|DGBLUE| from", Colors.DGBlue), true);
            _deleteMenu.Add(new UIText("everywhere, |DGRED|forever!", Colors.DGBlue), true);
            _deleteMenu.Add(new UIText(" ", Colors.DGBlue), true);
            _deleteMenu.Add(new UIMenuItem("|DGRED|DELETE", new UIMenuActionCallFunctionOpenMenu(_deleteMenu, this, new UIMenuActionCallFunctionOpenMenu.Function(DeleteFiles)), backButton: true), true);
            _deleteMenu.Add(new UIMenuItem("|DGGREEN|CANCEL", new UIMenuActionOpenMenu(_deleteMenu, this), backButton: true), true);
            _deleteMenu._defaultSelection = 1;
            _deleteMenu.SetBackFunction(new UIMenuActionOpenMenu(_deleteMenu, this));
            _deleteMenu.Close();
            _openOnClose = openOnClose;
        }

        private void DeleteFiles()
        {
            foreach (UICloudManagement.File pFolder in _flagged)
            {
                if (pFolder.files != null)
                    DeleteFolder(pFolder);
                else
                    DuckFile.Delete(pFolder.fullPath);
            }
        }

        private void DeleteFolder(UICloudManagement.File pFolder)
        {
            foreach (UICloudManagement.File file in pFolder.files)
            {
                if (file.files != null)
                    DeleteFolder(file);
                else
                    DuckFile.Delete(file.fullPath);
            }
        }

        private UICloudManagement.File GetFolder(string pPath, string pCloudPath)
        {
            UICloudManagement.File folder = root;
            if (pCloudPath.StartsWith("nq500000_"))
            {
                if (profileRoot == null)
                {
                    profileRoot = new UICloudManagement.File()
                    {
                        parent = root,
                        name = Steam.user.id.ToString(),
                        files = new List<UICloudManagement.File>()
                    };
                    root.files.Add(profileRoot);
                }
                folder = profileRoot;
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
            pRoot.files = pRoot.files.OrderBy<UICloudManagement.File, bool>(x => x.name != "..").ThenBy<UICloudManagement.File, bool>(x => x.files == null).ThenBy<UICloudManagement.File, string>(x => x.name).ToList<UICloudManagement.File>();
            foreach (UICloudManagement.File file in pRoot.files)
                SortFiles(file);
        }

        public override void Open()
        {
            HUD.CloseAllCorners();
            _opening = true;
            _flagged.Clear();
            root = new UICloudManagement.File()
            {
                files = new List<UICloudManagement.File>()
            };
            profileRoot = null;
            int count = Steam.FileGetCount();
            for (int file = 0; file < count; ++file)
            {
                CloudFile cloudFile = CloudFile.Get(Steam.FileGetName(file), true);
                if (cloudFile != null)
                {
                    string path = cloudFile.cloudPath.Substring(9, cloudFile.cloudPath.Length - 9);
                    GetFolder(Path.GetDirectoryName(path).Replace('\\', '/'), cloudFile.cloudPath).files.Add(new UICloudManagement.File()
                    {
                        name = Path.GetFileName(path),
                        fullPath = cloudFile.localPath
                    });
                }
            }
            SortFiles(root);
            currentFolder = root;
            base.Open();
        }

        public override void Close()
        {
            HUD.CloseAllCorners();
            base.Close();
        }

        public override void Update()
        {
            if (open && !_opening)
            {
                if (Input.Pressed("MENUUP") && _selection > 0)
                {
                    --_selection;
                    if (_selection < _topOffset)
                        --_topOffset;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("MENUDOWN") && _selection < currentFolder.files.Count - 1)
                {
                    ++_selection;
                    if (_selection > _topOffset + kMaxInView)
                        ++_topOffset;
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("SELECT") && currentFolder.files.Count > 0)
                {
                    if (currentFolder.files[_selection].name == "..")
                        SelectFolder(currentFolder.parent);
                    else if (currentFolder.files[_selection].files != null)
                        SelectFolder(currentFolder.files[_selection]);
                }
                if (Input.Pressed("MENU1") && currentFolder.files.Count > 0)
                {
                    if (_flagged.Contains(currentFolder.files[_selection]))
                        _flagged.Remove(currentFolder.files[_selection]);
                    else
                        _flagged.Add(currentFolder.files[_selection]);
                    SFX.Play("textLetter", 0.7f);
                }
                if (Input.Pressed("MENU2") && _flagged.Count > 0)
                {
                    _deleteMenu.dirty = true;
                    new UIMenuActionOpenMenu(this, _deleteMenu).Activate();
                }
                if (Input.Pressed("CANCEL"))
                {
                    if (currentFolder.parent != null)
                        SelectFolder(currentFolder.parent);
                    else if (_openOnClose != null)
                        new UIMenuActionOpenMenu(this, _openOnClose).Activate();
                    else
                        new UIMenuActionCloseMenu(this).Activate();
                }
            }
            _opening = false;
            base.Update();
        }

        private void SelectFolder(UICloudManagement.File pFolder)
        {
            currentFolder = pFolder;
            _selection = 0;
            _topOffset = 0;
            SFX.Play("textLetter", 0.7f);
        }

        public override void Draw()
        {
            if (open && currentFolder != null)
            {
                Vec2 vec2 = new Vec2(x - 124f, this.y - 66f);
                float y = 0f;
                int num1 = 0;
                int num2 = 0;
                foreach (UICloudManagement.File file in currentFolder.files)
                {
                    if (num1 < _topOffset)
                    {
                        ++num1;
                    }
                    else
                    {
                        if (_topOffset > 0)
                        {
                            _downArrow.flipV = true;
                            Graphics.Draw(_downArrow, x, vec2.y - 2f, (Depth)0.5f);
                        }
                        if (num2 > kMaxInView)
                        {
                            _downArrow.flipV = false;
                            Graphics.Draw(_downArrow, x, vec2.y + y, (Depth)0.5f);
                            break;
                        }
                        string str1 = file.name;
                        if (str1.Length > 31)
                            str1 = str1.Substring(0, 30) + "..";
                        string str2 = file.files == null ? (_flagged.Contains(file) ? "@DELETEFLAG_ON@" : "@DELETEFLAG_OFF@") + str1 : (_flagged.Contains(file) ? "@FOLDERDELETEICON@" : "@FOLDERICON@") + str1;
                        _littleFont.Draw(num1 != _selection ? " " + str2 : "@SELECTICON@" + str2, vec2 + new Vec2(0f, y), Color.White, (Depth)0.5f);
                        y += 8f;
                        ++num1;
                        ++num2;
                    }
                }
                string text = "@CANCEL@BACK";
                if (currentFolder.files.Count > 0)
                {
                    if (currentFolder.files[_selection].files != null)
                        text += " @SELECT@OPEN";
                    text = !_flagged.Contains(currentFolder.files[_selection]) ? text + " @MENU1@FLAG" : text + " @MENU1@UNFLAG";
                }
                if (_flagged.Count > 0)
                    text += " @MENU2@DELETE";
                _littleFont.Draw(text, new Vec2(x - _littleFont.GetWidth(text) / 2f, this.y + 74f), Color.White, (Depth)0.5f);
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
