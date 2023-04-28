// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextToolbarItem
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            toolBarToolTip = "";
        }

        public void ShowUnsavedChangesDialogue(
          string pContext,
          string pDescription,
          ContextMenu pConfirmItem)
        {
            Closed();
            _unsavedChangesDialogue.Open("UNSAVED CHANGES", pDescription: ("Your current level has unsaved\nchanges! Are you sure you want to\n" + pDescription + "?"));
            Editor.lockInput = _unsavedChangesDialogue;
            _unsavedChangesDialogue.result = false;
            _unsavedChangesDialogue.contextString = pContext;
            _unsavedChangesDialogue.confirmItem = pConfirmItem;
        }

        public override void Selected(ContextMenu item)
        {
            if (item.text == "SPECIAL")
            {
                base.Selected(item);
            }
            else
            {
                if (item.text == Triggers.Cancel)
                    (Level.current as Editor).CloseMenu();
                if (item.text == "NEW")
                {
                    if (!Editor.hasUnsavedChanges || _unsavedChangesDialogue.contextString == "NEW" && _unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        Editor._currentLevelData.metaData.onlineMode = false;
                        current.CloseMenu();
                        _unsavedChangesDialogue.result = false;
                    }
                    else
                        ShowUnsavedChangesDialogue("NEW", "create a new level", item);
                }
                if (item.text == "NEW ONLINE")
                {
                    if (!Editor.hasUnsavedChanges || _unsavedChangesDialogue.contextString == "NEW ONLINE" && _unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        Editor._currentLevelData.metaData.onlineMode = true;
                        current.CloseMenu();
                        _unsavedChangesDialogue.result = false;
                    }
                    else
                        ShowUnsavedChangesDialogue("NEW ONLINE", "create a new online level", item);
                }
                if (item.text == "NEW ARCADE MACHINE")
                {
                    if (!Editor.hasUnsavedChanges || _unsavedChangesDialogue.contextString == "NEW ARCADE MACHINE" && _unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        current.AddObject(new ImportMachine(0f, 0f));
                        current.CloseMenu();
                        _unsavedChangesDialogue.result = false;
                    }
                    else
                        ShowUnsavedChangesDialogue("NEW ARCADE MACHINE", "Create a new Arcade Machine.", item);
                }
                if (item.text == "NEW MAP PART")
                {
                    if (!Editor.hasUnsavedChanges || _unsavedChangesDialogue.contextString == "NEW RANDOM MAP PART" && _unsavedChangesDialogue.result)
                    {
                        Editor current = Level.current as Editor;
                        current.ClearEverything();
                        current.saveName = "";
                        current._miniMode = true;
                        current.CloseMenu();
                        _unsavedChangesDialogue.result = false;
                    }
                    else
                        ShowUnsavedChangesDialogue("NEW MAP PART", "Create a new Map Part", item);
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
                if (item.text == "4x4")
                {
                    Editor current = Level.current as Editor;
                    current.cellSize = 4f;
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
                if (!Editor.hasUnsavedChanges || _unsavedChangesDialogue.contextString == "QUIT" && _unsavedChangesDialogue.result)
                {
                    Editor current = Level.current as Editor;
                    current.Quit();
                    current.CloseMenu();
                    _unsavedChangesDialogue.result = false;
                }
                else
                    ShowUnsavedChangesDialogue("QUIT", "quit to the main menu", item);
            }
        }

        public override void Hover() => opened = true;

        public override void Initialize()
        {
            _unsavedChangesDialogue = new MessageDialogue(this);
            Level.Add(_unsavedChangesDialogue);
            _newButton = new ToolbarButton(this, 0, "NEW LEVEL");
            _saveButton = new ToolbarButton(this, 2, "SAVE LEVEL");
            _loadButton = new ToolbarButton(this, 1, "LOAD LEVEL");
            _playButton = new ToolbarButton(this, 10, "TEST LEVEL");
            _gridButton = new ToolbarButton(this, 11, "CHANGE GRID");
            _quitButton = new ToolbarButton(this, 12, "EXIT EDITOR");
            itemSize.y = 16f;
            float x = position.x;
            _playButton.x = x;
            _playButton.y = position.y;
            _buttons.Add(_playButton);
            float num1 = x + 18f;
            _saveButton.x = num1;
            _saveButton.y = position.y;
            _buttons.Add(_saveButton);
            float num2 = num1 + 18f;
            _loadButton.x = num2;
            _loadButton.y = position.y;
            _buttons.Add(_loadButton);
            float num3 = num2 + 18f;
            _newButton.x = num3;
            _newButton.y = position.y;
            _buttons.Add(_newButton);
            float num4 = num3 + 18f;
            _gridButton.x = num4;
            _gridButton.y = position.y;
            _buttons.Add(_gridButton);
            _quitButton.x = num4 + 18f;
            _quitButton.y = position.y;
            _buttons.Add(_quitButton);
        }

        public override void ParentCloseAction()
        {
            _selectedIndex = -1;
            foreach (ToolbarButton button in _buttons)
                button.hover = false;
        }

        private bool menuOpen
        {
            get
            {
                if (_gridMenu != null && _gridMenu.opened || _saveMenu != null && _saveMenu.opened || _newMenu != null && _newMenu.opened || _quitMenu != null && _quitMenu.opened)
                    return true;
                return _uploadMenu != null && _uploadMenu.opened;
            }
        }

        public override void Update()
        {
            if (_unsavedChangesDialogue.opened)
                return;
            if (_doLoad)
            {
                _doLoad = false;
                if (!Editor.hasUnsavedChanges || _unsavedChangesDialogue.result)
                {
                    Editor current = Level.current as Editor;
                    current.Load();
                    current.CloseMenu();
                }
                _unsavedChangesDialogue.result = false;
            }
            toolBarToolTip = "";
            if (!_opening && opened && Editor.inputMode == EditorInput.Gamepad)
            {
                if (menuOpen)
                    return;
                if (Input.Pressed(Triggers.MenuUp))
                {
                    opened = false;
                    if (this.owner is ContextMenu owner)
                    {
                        owner._opening = true;
                        foreach (ToolbarButton button in _buttons)
                            button.hover = false;
                        toolBarToolTip = "";
                        return;
                    }
                }
                if (Input.Pressed(Triggers.MenuDown))
                {
                    opened = false;
                    if (this.owner is ContextMenu owner)
                    {
                        ++owner.selectedIndex;
                        foreach (ToolbarButton button in _buttons)
                            button.hover = false;
                        toolBarToolTip = "";
                        return;
                    }
                }
                if (Input.Pressed(Triggers.MenuLeft))
                {
                    --_selectedIndex;
                    if (_selectedIndex < 0)
                        _selectedIndex = _buttons.Count - 1;
                }
                else if (Input.Pressed(Triggers.MenuRight))
                {
                    ++_selectedIndex;
                    if (_selectedIndex > _buttons.Count - 1)
                        _selectedIndex = 0;
                }
                int num = 0;
                foreach (ToolbarButton button in _buttons)
                {
                    if (_selectedIndex == num)
                    {
                        button.hover = true;
                        toolBarToolTip = button.hoverText;
                        if (Input.Pressed(Triggers.Select))
                            ButtonPressed(button);
                    }
                    else
                        button.hover = false;
                    ++num;
                }
            }
            float x = position.x;
            _playButton.x = x;
            _playButton.y = position.y;
            float num1 = x + 18f;
            _saveButton.x = num1;
            _saveButton.y = position.y;
            float num2 = num1 + 18f;
            _loadButton.x = num2;
            _loadButton.y = position.y;
            float num3 = num2 + 18f;
            _newButton.x = num3;
            _newButton.y = position.y;
            float num4 = num3 + 18f;
            _gridButton.x = num4;
            _gridButton.y = position.y;
            _quitButton.x = num4 + 18f;
            _quitButton.y = position.y;
            foreach (Thing button in _buttons)
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
            Closed();
            base.Terminate();
        }

        public override bool HasOpen() => opened;

        public void ButtonPressed(ToolbarButton button)
        {
            SFX.Play("highClick", 0.3f, 0.2f);
            ContextMenu contextMenu1 = null;
            Vec2 vec2 = new Vec2(2f, 21f);
            if (button == _newButton)
            {
                Closed();
                _newMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = position.x - vec2.x,
                    y = position.y + vec2.y,
                    root = true,
                    depth = depth + 10
                };
                Selected();
                _newMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "NEW"
                });
                _newMenu.AddItem(new ContextMenu(this)
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
                _newMenu.AddItem(contextMenu2);
                _newMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = Triggers.Cancel
                });
                Level.Add(_newMenu);
                _newMenu.opened = true;
                contextMenu1 = _newMenu;
            }
            if (button == _saveButton)
            {
                Closed();
                _saveMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = position.x - vec2.x,
                    y = position.y + vec2.y,
                    root = true,
                    depth = depth + 10
                };
                Selected();
                _saveMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 40f
          },
                    text = "SAVE"
                });
                _saveMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 40f
          },
                    text = "SAVE AS"
                });
                if (Steam.IsInitialized())
                    _saveMenu.AddItem(new ContextMenu(this)
                    {
                        itemSize = {
              x = 40f
            },
                        text = "@STEAMICON@ PUBLISH"
                    });
                Level.Add(_saveMenu);
                _saveMenu.opened = true;
                contextMenu1 = _saveMenu;
            }
            if (button == _gridButton)
            {
                Closed();
                _gridMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = position.x - vec2.x,
                    y = position.y + vec2.y,
                    root = true,
                    depth = depth + 10
                };
                Selected();
                _gridMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
                        x = 60f
                    },
                    text = "4x4"
                });
                _gridMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
                        x = 60f
                    },
                    text = "8x8"
                });
                _gridMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
                        x = 60f
                    },
                    text = "16x16"
                });
                _gridMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
                        x = 60f
                    },
                    text = "32x32"
                });
                Level.Add(_gridMenu);
                _gridMenu.opened = true;
                contextMenu1 = _gridMenu;
            }
            if (button == _loadButton)
            {
                _doLoad = true;
                if (Editor.hasUnsavedChanges)
                    ShowUnsavedChangesDialogue("LOAD", "load a new level", null);
            }
            if (button == _playButton)
                (Level.current as Editor).Play();
            if (button == _quitButton)
            {
                Closed();
                _quitMenu = new ContextMenu(this, hasToproot: true, topRoot: button.position)
                {
                    x = position.x - vec2.x,
                    y = position.y + vec2.y,
                    root = true,
                    depth = depth + 10
                };
                Selected();
                _quitMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = "QUIT"
                });
                _quitMenu.AddItem(new ContextMenu(this)
                {
                    itemSize = {
            x = 60f
          },
                    text = Triggers.Cancel
                });
                Level.Add(_quitMenu);
                _quitMenu.opened = true;
                contextMenu1 = _quitMenu;
            }
            if (contextMenu1 == null || contextMenu1.y + contextMenu1.menuSize.y <= layer.camera.height - 4f)
                return;
            isPushingUp = true;
            float y = contextMenu1.y;
            contextMenu1.y = layer.camera.height - 4f - contextMenu1.menuSize.y;
            contextMenu1._toprootPosition.y += contextMenu1.y - y;
            ContextMenu i = owner as ContextMenu;
            if (i != null)
            {
                i._openedOffset = 0f;
                i.y = contextMenu1.y - 16f - i.menuSize.y;
            }
            i.PositionItems();
            contextMenu1.PositionItems();
            isPushingUp = false;
        }

        public override void Closed()
        {
            if (_newMenu != null)
            {
                Level.Remove(_newMenu);
                _newMenu = null;
            }
            if (_saveMenu != null)
            {
                Level.Remove(_saveMenu);
                _saveMenu = null;
            }
            if (_gridMenu != null)
            {
                Level.Remove(_gridMenu);
                _gridMenu = null;
            }
            if (_quitMenu != null)
            {
                Level.Remove(_quitMenu);
                _quitMenu = null;
            }
            if (_uploadMenu != null)
            {
                Level.Remove(_uploadMenu);
                _uploadMenu = null;
            }
            toolBarToolTip = "";
        }

        public override void Draw()
        {
            if (toolBarToolTip != "" && toolBarToolTip != null && !menuOpen)
            {
                Vec2 p1 = new Vec2(position.x - 2f, position.y + 18f);
                Graphics.DrawRect(p1, p1 + new Vec2(100f, 15f), Color.Black * alpha, depth + 10);
                if (Editor.inputMode == EditorInput.Mouse)
                    Graphics.DrawString(toolBarToolTip, p1 + new Vec2(4f, 4f), Color.White * alpha, depth + 11);
                else
                    Graphics.DrawString("@SELECT@" + toolBarToolTip, p1 + new Vec2(0f, 4f), Color.White * alpha, depth + 11);
            }
            float x = position.x;
            _playButton.x = x;
            _playButton.y = position.y;
            float num1 = x + 18f;
            _saveButton.x = num1;
            _saveButton.y = position.y;
            float num2 = num1 + 18f;
            _loadButton.x = num2;
            _loadButton.y = position.y;
            float num3 = num2 + 18f;
            _newButton.x = num3;
            _newButton.y = position.y;
            float num4 = num3 + 18f;
            _gridButton.x = num4;
            _gridButton.y = position.y;
            _quitButton.x = num4 + 18f;
            _quitButton.y = position.y;
            foreach (Thing button in _buttons)
                button.DoDraw();
        }
    }
}
