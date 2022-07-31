// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextToolbarItem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class ContextToolbarItem : ContextMenu, IContextListener
    {
        private MessageDialogue _unsavedChangesDialogue;
        private ToolbarButton _newButton;
        private ToolbarButton _saveButton;
        private ToolbarButton _loadButton;
        private ToolbarButton _playButton;
        private ToolbarButton _gridButton;
        private ToolbarButton _quitButton;
        private ContextMenu _newMenu;
        private ContextMenu _saveMenu;
        private ContextMenu _gridMenu;
        private ContextMenu _quitMenu;
        private ContextMenu _uploadMenu;
        public string toolBarToolTip;
        private List<ToolbarButton> _buttons = new List<ToolbarButton>();
        private bool _doLoad;
        public bool isPushingUp;

        public ContextToolbarItem(ContextMenu owner)
          : base(owner)
        {
            this.toolBarToolTip = "";
        }

        public void ShowUnsavedChangesDialogue(
          string pContext,
          string pDescription,
          ContextMenu pConfirmItem)
        {
            this.Closed();
            this._unsavedChangesDialogue.Open("UNSAVED CHANGES", pDescription: ("Your current level has unsaved\nchanges! Are you sure you want to\n" + pDescription + "?"));
            Editor.lockInput = _unsavedChangesDialogue;
            this._unsavedChangesDialogue.result = false;
            this._unsavedChangesDialogue.contextString = pContext;
            this._unsavedChangesDialogue.confirmItem = pConfirmItem;
        }

        public override void Selected(ContextMenu item)
        {
            if (item.text == "SPECIAL")
            {
                base.Selected(item);
            }
            else
            {
                if (item.text == "CANCEL")
                    (Level.current as Editor).CloseMenu();
                if (item.text == "NEW")
                {
                    if (!Editor.hasUnsavedChanges || this._unsavedChangesDialogue.contextString == "NEW" && this._unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        Editor._currentLevelData.metaData.onlineMode = false;
                        current.CloseMenu();
                        this._unsavedChangesDialogue.result = false;
                    }
                    else
                        this.ShowUnsavedChangesDialogue("NEW", "create a new level", item);
                }
                if (item.text == "NEW ONLINE")
                {
                    if (!Editor.hasUnsavedChanges || this._unsavedChangesDialogue.contextString == "NEW ONLINE" && this._unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        Editor._currentLevelData.metaData.onlineMode = true;
                        current.CloseMenu();
                        this._unsavedChangesDialogue.result = false;
                    }
                    else
                        this.ShowUnsavedChangesDialogue("NEW ONLINE", "create a new online level", item);
                }
                if (item.text == "NEW ARCADE MACHINE")
                {
                    if (!Editor.hasUnsavedChanges || this._unsavedChangesDialogue.contextString == "NEW ARCADE MACHINE" && this._unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        current.AddObject(new ImportMachine(0f, 0f));
                        current.CloseMenu();
                        this._unsavedChangesDialogue.result = false;
                    }
                    else
                        this.ShowUnsavedChangesDialogue("NEW ARCADE MACHINE", "Create a new Arcade Machine.", item);
                }
                if (item.text == "NEW MAP PART")
                {
                    if (!Editor.hasUnsavedChanges || this._unsavedChangesDialogue.contextString == "NEW RANDOM MAP PART" && this._unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        current._miniMode = true;
                        current.CloseMenu();
                        this._unsavedChangesDialogue.result = false;
                    }
                    else
                        this.ShowUnsavedChangesDialogue("NEW MAP PART", "Create a new Map Part", item);
                }
                if (item.text == "SAVE")
                {
                    Editor current = Level.current as Editor;
                    current.Save();
                    current.CloseMenu();
                }
                if (item.text.Contains("PUBLISH"))
                {
                    Editor current = Level.current as Editor;
                    current.SteamUpload();
                    current.CloseMenu();
                }
                if (item.text == "SAVE AS")
                {
                    Editor current = Level.current as Editor;
                    current.SaveAs();
                    current.CloseMenu();
                }
                if (item.text == "8x8")
                {
                    Editor current = Level.current as Editor;
                    current.cellSize = 8f;
                    current.CloseMenu();
                }
                if (item.text == "16x16")
                {
                    Editor current = Level.current as Editor;
                    current.cellSize = 16f;
                    current.CloseMenu();
                }
                if (item.text == "32x32")
                {
                    Editor current = Level.current as Editor;
                    current.cellSize = 32f;
                    current.CloseMenu();
                }
                if (!(item.text == "QUIT"))
                    return;
                if (!Editor.hasUnsavedChanges || this._unsavedChangesDialogue.contextString == "QUIT" && this._unsavedChangesDialogue.result)
                {
                    Editor current = Level.current as Editor;
                    current.Quit();
                    current.CloseMenu();
                    this._unsavedChangesDialogue.result = false;
                }
                else
                    this.ShowUnsavedChangesDialogue("QUIT", "quit to the main menu", item);
            }
        }

        public override void Hover() => this.opened = true;

        public override void Initialize()
        {
            this._unsavedChangesDialogue = new MessageDialogue(this);
            Level.Add(_unsavedChangesDialogue);
            this._newButton = new ToolbarButton(this, 0, "NEW LEVEL");
            this._saveButton = new ToolbarButton(this, 2, "SAVE LEVEL");
            this._loadButton = new ToolbarButton(this, 1, "LOAD LEVEL");
            this._playButton = new ToolbarButton(this, 10, "TEST LEVEL");
            this._gridButton = new ToolbarButton(this, 11, "CHANGE GRID");
            this._quitButton = new ToolbarButton(this, 12, "EXIT EDITOR");
            this.itemSize.y = 16f;
            float x = this.position.x;
            this._playButton.x = x;
            this._playButton.y = this.position.y;
            this._buttons.Add(this._playButton);
            float num1 = x + 18f;
            this._saveButton.x = num1;
            this._saveButton.y = this.position.y;
            this._buttons.Add(this._saveButton);
            float num2 = num1 + 18f;
            this._loadButton.x = num2;
            this._loadButton.y = this.position.y;
            this._buttons.Add(this._loadButton);
            float num3 = num2 + 18f;
            this._newButton.x = num3;
            this._newButton.y = this.position.y;
            this._buttons.Add(this._newButton);
            float num4 = num3 + 18f;
            this._gridButton.x = num4;
            this._gridButton.y = this.position.y;
            this._buttons.Add(this._gridButton);
            this._quitButton.x = num4 + 18f;
            this._quitButton.y = this.position.y;
            this._buttons.Add(this._quitButton);
        }

        public override void ParentCloseAction()
        {
            this._selectedIndex = -1;
            foreach (ToolbarButton button in this._buttons)
                button.hover = false;
        }

        private bool menuOpen
        {
            get
            {
                if (this._gridMenu != null && this._gridMenu.opened || this._saveMenu != null && this._saveMenu.opened || this._newMenu != null && this._newMenu.opened || this._quitMenu != null && this._quitMenu.opened)
                    return true;
                return this._uploadMenu != null && this._uploadMenu.opened;
            }
        }

        public override void Update()
        {
            if (this._unsavedChangesDialogue.opened)
                return;
            if (this._doLoad)
            {
                this._doLoad = false;
                if (!Editor.hasUnsavedChanges || this._unsavedChangesDialogue.result)
                {
                    Editor current = Level.current as Editor;
                    current.Load();
                    current.CloseMenu();
                }
                this._unsavedChangesDialogue.result = false;
            }
            this.toolBarToolTip = "";
            if (!this._opening && this.opened && Editor.inputMode == EditorInput.Gamepad)
            {
                if (this.menuOpen)
                    return;
                if (Input.Pressed("MENUUP"))
                {
                    this.opened = false;
                    if (this.owner is ContextMenu owner)
                    {
                        owner._opening = true;
                        foreach (ToolbarButton button in this._buttons)
                            button.hover = false;
                        this.toolBarToolTip = "";
                        return;
                    }
                }
                if (Input.Pressed("MENUDOWN"))
                {
                    this.opened = false;
                    if (this.owner is ContextMenu owner)
                    {
                        ++owner.selectedIndex;
                        foreach (ToolbarButton button in this._buttons)
                            button.hover = false;
                        this.toolBarToolTip = "";
                        return;
                    }
                }
                if (Input.Pressed("MENULEFT"))
                {
                    --this._selectedIndex;
                    if (this._selectedIndex < 0)
                        this._selectedIndex = this._buttons.Count - 1;
                }
                else if (Input.Pressed("MENURIGHT"))
                {
                    ++this._selectedIndex;
                    if (this._selectedIndex > this._buttons.Count - 1)
                        this._selectedIndex = 0;
                }
                int num = 0;
                foreach (ToolbarButton button in this._buttons)
                {
                    if (this._selectedIndex == num)
                    {
                        button.hover = true;
                        this.toolBarToolTip = button.hoverText;
                        if (Input.Pressed("SELECT"))
                            this.ButtonPressed(button);
                    }
                    else
                        button.hover = false;
                    ++num;
                }
            }
            float x = this.position.x;
            this._playButton.x = x;
            this._playButton.y = this.position.y;
            float num1 = x + 18f;
            this._saveButton.x = num1;
            this._saveButton.y = this.position.y;
            float num2 = num1 + 18f;
            this._loadButton.x = num2;
            this._loadButton.y = this.position.y;
            float num3 = num2 + 18f;
            this._newButton.x = num3;
            this._newButton.y = this.position.y;
            float num4 = num3 + 18f;
            this._gridButton.x = num4;
            this._gridButton.y = this.position.y;
            this._quitButton.x = num4 + 18f;
            this._quitButton.y = this.position.y;
            foreach (Thing button in this._buttons)
                button.DoUpdate();
            base.Update();
        }

        public override void Terminate()
        {
            Level.current.RemoveThing(_newButton);
            Level.current.RemoveThing(_saveButton);
            Level.current.RemoveThing(_loadButton);
            Level.current.RemoveThing(_playButton);
            Level.current.RemoveThing(_gridButton);
            Level.current.RemoveThing(_quitButton);
            this.Closed();
            base.Terminate();
        }

        public override bool HasOpen() => this.opened;

        public void ButtonPressed(ToolbarButton button)
        {
            SFX.Play("highClick", 0.3f, 0.2f);
            ContextMenu contextMenu1 = null;
            Vec2 vec2 = new Vec2(2f, 21f);
            if (button == this._newButton)
            {
                this.Closed();
                this._newMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = this.position.x - vec2.x,
                    y = this.position.y + vec2.y,
                    root = true,
                    depth = this.depth + 10
                };
                this.Selected();
                this._newMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "NEW"
                });
                this._newMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "NEW ONLINE"
                });
                ContextMenu contextMenu2 = new ContextMenu(this);
                contextMenu2.itemSize.x = 60f;
                contextMenu2.text = "SPECIAL";
                contextMenu2.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 90f
          },
                    text = "NEW ARCADE MACHINE"
                });
                this._newMenu.AddItem(contextMenu2);
                this._newMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "CANCEL"
                });
                Level.Add(_newMenu);
                this._newMenu.opened = true;
                contextMenu1 = this._newMenu;
            }
            if (button == this._saveButton)
            {
                this.Closed();
                this._saveMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = this.position.x - vec2.x,
                    y = this.position.y + vec2.y,
                    root = true,
                    depth = this.depth + 10
                };
                this.Selected();
                this._saveMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 40f
          },
                    text = "SAVE"
                });
                this._saveMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 40f
          },
                    text = "SAVE AS"
                });
                if (Steam.IsInitialized())
                    this._saveMenu.AddItem(new ContextMenu(this)
                    {
                        itemSize = {
              x = 40f
            },
                        text = "@STEAMICON@ PUBLISH"
                    });
                Level.Add(_saveMenu);
                this._saveMenu.opened = true;
                contextMenu1 = this._saveMenu;
            }
            if (button == this._gridButton)
            {
                this.Closed();
                this._gridMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = this.position.x - vec2.x,
                    y = this.position.y + vec2.y,
                    root = true,
                    depth = this.depth + 10
                };
                this.Selected();
                this._gridMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "8x8"
                });
                this._gridMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "16x16"
                });
                this._gridMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "32x32"
                });
                Level.Add(_gridMenu);
                this._gridMenu.opened = true;
                contextMenu1 = this._gridMenu;
            }
            if (button == this._loadButton)
            {
                this._doLoad = true;
                if (Editor.hasUnsavedChanges)
                    this.ShowUnsavedChangesDialogue("LOAD", "load a new level", null);
            }
            if (button == this._playButton)
                (Level.current as Editor).Play();
            if (button == this._quitButton)
            {
                this.Closed();
                this._quitMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = this.position.x - vec2.x,
                    y = this.position.y + vec2.y,
                    root = true,
                    depth = this.depth + 10
                };
                this.Selected();
                this._quitMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "QUIT"
                });
                this._quitMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "CANCEL"
                });
                Level.Add(_quitMenu);
                this._quitMenu.opened = true;
                contextMenu1 = this._quitMenu;
            }
            if (contextMenu1 == null || (double)contextMenu1.y + contextMenu1.menuSize.y <= (double)this.layer.camera.height - 4.0)
                return;
            this.isPushingUp = true;
            float y = contextMenu1.y;
            contextMenu1.y = this.layer.camera.height - 4f - contextMenu1.menuSize.y;
            contextMenu1._toprootPosition.y += contextMenu1.y - y;
            ContextMenu i = this.owner as ContextMenu;
            if (i != null)
            {
                i._openedOffset = 0f;
                i.y = contextMenu1.y - 16f - i.menuSize.y;
            }
            i.PositionItems();
            contextMenu1.PositionItems();
            this.isPushingUp = false;
        }

        public override void Closed()
        {
            if (this._newMenu != null)
            {
                Level.Remove(_newMenu);
                this._newMenu = null;
            }
            if (this._saveMenu != null)
            {
                Level.Remove(_saveMenu);
                this._saveMenu = null;
            }
            if (this._gridMenu != null)
            {
                Level.Remove(_gridMenu);
                this._gridMenu = null;
            }
            if (this._quitMenu != null)
            {
                Level.Remove(_quitMenu);
                this._quitMenu = null;
            }
            if (this._uploadMenu != null)
            {
                Level.Remove(_uploadMenu);
                this._uploadMenu = null;
            }
            this.toolBarToolTip = "";
        }

        public override void Draw()
        {
            if (this.toolBarToolTip != "" && this.toolBarToolTip != null && !this.menuOpen)
            {
                Vec2 p1 = new Vec2(this.position.x - 2f, this.position.y + 18f);
                Graphics.DrawRect(p1, p1 + new Vec2(100f, 15f), Color.Black * this.alpha, this.depth + 10);
                if (Editor.inputMode == EditorInput.Mouse)
                    Graphics.DrawString(this.toolBarToolTip, p1 + new Vec2(4f, 4f), Color.White * this.alpha, this.depth + 11);
                else
                    Graphics.DrawString("@SELECT@" + this.toolBarToolTip, p1 + new Vec2(0f, 4f), Color.White * this.alpha, this.depth + 11);
            }
            float x = this.position.x;
            this._playButton.x = x;
            this._playButton.y = this.position.y;
            float num1 = x + 18f;
            this._saveButton.x = num1;
            this._saveButton.y = this.position.y;
            float num2 = num1 + 18f;
            this._loadButton.x = num2;
            this._loadButton.y = this.position.y;
            float num3 = num2 + 18f;
            this._newButton.x = num3;
            this._newButton.y = this.position.y;
            float num4 = num3 + 18f;
            this._gridButton.x = num4;
            this._gridButton.y = this.position.y;
            this._quitButton.x = num4 + 18f;
            this._quitButton.y = this.position.y;
            foreach (Thing button in this._buttons)
                button.DoDraw();
        }
    }
}
