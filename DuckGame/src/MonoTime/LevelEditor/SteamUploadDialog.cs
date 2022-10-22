// Decompiled with JetBrains decompiler
// Type: DuckGame.SteamUploadDialog
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class SteamUploadDialog : ContextMenu
    {
        private FancyBitmapFont _font;
        private Textbox _descriptionBox;
        private Textbox _nameBox;
        private MessageDialogue _confirm;
        private NotifyDialogue _notify;
        private UploadDialogue _upload;
        private DeathmatchTestDialogue _deathmatchTest;
        private TestSuccessDialogue _testSuccess;
        private ArcadeTestDialogue _arcadeTest;
        private Sprite _previewTarget;
        private SpriteMap _workshopTag;
        private Sprite _workshopTagMiddle;
        private Sprite _tagPlus;
        private static bool _editingMod;
        private LevelData _levelData;
        private Mod _mod;
        private int _arcadeTestIndex = -1;
        private static List<string> _possibleTagsLevel = new List<string>()
    {
      "Dumb",
      "Fast",
      "Luck",
      "Weird",
      "Fire",
      "Pro",
      "Cute"
    };
        private static List<string> _possibleTagsMod = new List<string>()
    {
      "Weapons",
      "Hats",
      "Items",
      "Equipment",
      "Total Conversion"
    };
        private Vec2 _acceptPos;
        private Vec2 _acceptSize;
        private bool _acceptHover;
        private Vec2 _cancelPos;
        private Vec2 _cancelSize;
        private bool _cancelHover;
        private EditorWorkshopItem _publishItem;
        //private WorkshopItem _currentUploadItem;
        private bool _testing;
        private Vec2 _plusPosition;
        private ContextMenu _tagMenu;
        //private bool _creatingSubItem;
        //private int _subItemUploadIndex = -1;
        //private int _subItemTries;
        private Dictionary<string, Vec2> tagPositions = new Dictionary<string, Vec2>();
        private Stack<EditorWorkshopItem> _publishStack = new Stack<EditorWorkshopItem>();
        private float hOffset;
        private float _fdHeight = 262f;
        public bool drag;

        public SteamUploadDialog()
          : base(null)
        {
        }

        public override void Initialize()
        {
            layer = Layer.HUD;
            depth = (Depth)0.9f;
            _showBackground = false;
            itemSize = new Vec2(386f, 16f);
            _root = true;
            drawControls = false;
            _descriptionBox = new Textbox(x + 5f, y + 225f, 316f, 40f, 0.5f, 9, "<ENTER DESCRIPTION>");
            _nameBox = new Textbox(x + 5f, y + byte.MaxValue, 316f, 12f, maxLines: 1, emptyText: "<ENTER NAME>");
            _font = new FancyBitmapFont("smallFont");
            _confirm = new MessageDialogue();
            Level.Add(_confirm);
            _upload = new UploadDialogue();
            Level.Add(_upload);
            _notify = new NotifyDialogue();
            Level.Add(_notify);
            _deathmatchTest = new DeathmatchTestDialogue();
            Level.Add(_deathmatchTest);
            _testSuccess = new TestSuccessDialogue();
            Level.Add(_testSuccess);
            _arcadeTest = new ArcadeTestDialogue();
            Level.Add(_arcadeTest);
        }

        public void Open(LevelData pData)
        {
            SteamUploadDialog._editingMod = false;
            _publishItem = null;
            Editor.lockInput = this;
            SFX.Play("openClick", 0.4f);
            opened = true;
            _publishItem = new EditorWorkshopItem(pData);
            _previewTarget = new Sprite(_publishItem.preview);
            _nameBox.text = pData.workshopData.name;
            _descriptionBox.text = pData.workshopData.description;
            _workshopTag = new SpriteMap("workshopTag", 4, 8);
            _workshopTagMiddle = new Sprite("workshopTagMiddle");
            _tagPlus = new Sprite("tagPlus");
            _levelData = pData;
            _arcadeTestIndex = 0;
        }

        public void Open(Mod pData)
        {
            SteamUploadDialog._editingMod = true;
            _publishItem = null;
            Editor.lockInput = this;
            SFX.Play("openClick", 0.4f);
            opened = true;
            _publishItem = new EditorWorkshopItem(pData);
            _previewTarget = new Sprite(_publishItem.preview);
            _nameBox.text = pData.workshopData.name;
            _descriptionBox.text = pData.workshopData.description;
            _workshopTag = new SpriteMap("workshopTag", 4, 8);
            _workshopTagMiddle = new Sprite("workshopTagMiddle");
            _tagPlus = new Sprite("tagPlus");
            _mod = pData;
            _arcadeTestIndex = 0;
        }

        public void Close()
        {
            Editor.lockInput = null;
            opened = false;
            _descriptionBox.LoseFocus();
            _nameBox.LoseFocus();
            _publishItem = null;
            ClearItems();
        }

        public override void Selected(ContextMenu item)
        {
            SFX.Play("highClick", 0.3f, 0.2f);
            if (item != null && item.text != "")
                _publishItem.AddTag(item.text);
            if (_tagMenu == null)
                return;
            _tagMenu.opened = false;
            Level.Remove(_tagMenu);
            _tagMenu = null;
            if (Editor.PeekFocus() != _tagMenu)
                return;
            Editor.PopFocus();
        }

        public override void Toggle(ContextMenu item)
        {
        }

        public static List<string> possibleTags => SteamUploadDialog._editingMod ? SteamUploadDialog._possibleTagsMod : SteamUploadDialog._possibleTagsLevel;

        private void Publish()
        {
            _publishItem.name = _nameBox.text;
            _publishItem.description = _descriptionBox.text;
            if (_publishItem.PrepareItem() != SteamResult.OK)
            {
                _notify.Open("Failed with code " + ((int)_publishItem.result).ToString() + " (" + _publishItem.result.ToString() + ")");
            }
            else
            {
                _publishStack.Clear();
                _publishStack.Push(_publishItem);
                foreach (EditorWorkshopItem subItem in _publishItem.subItems)
                    _publishStack.Push(subItem);
                UploadNext();
            }
        }

        public bool UploadNext()
        {
            if (_publishStack.Count == 0)
                return false;
            EditorWorkshopItem editorWorkshopItem = _publishStack.Peek();
            editorWorkshopItem.Upload();
            if (editorWorkshopItem.subIndex == -1)
                _upload.Open("Uploading...", editorWorkshopItem.item);
            else
                _upload.Open("Uploading Sub Item(" + editorWorkshopItem.subIndex.ToString() + ")...", editorWorkshopItem.item);
            return true;
        }

        public override void Update()
        {
            if (_publishStack.Count > 0)
            {
                if (!_publishStack.Peek().finishedProcessing)
                    return;
                _publishStack.Peek().FinishUpload();
                _upload.Close();
                if (_publishStack.Peek().result == SteamResult.OK)
                {
                    EditorWorkshopItem editorWorkshopItem = _publishStack.Peek();
                    _publishStack.Pop();
                    if (UploadNext())
                        return;
                    _upload.Close();
                    _notify.Open("Item published!");
                    Steam.ShowWorkshopLegalAgreement(editorWorkshopItem.item.id.ToString());
                }
                else
                {
                    _notify.Open("Failed with code " + ((int)_publishStack.Peek().result).ToString() + " (" + _publishStack.Peek().result.ToString() + ")");
                    _publishStack.Clear();
                }
            }
            else if (!opened || _opening || _confirm.opened || _upload.opened || _deathmatchTest.opened || _arcadeTest.opened || _testSuccess.opened)
            {
                if (opened)
                    Keyboard.keyString = "";
                if (opened)
                    Editor.lockInput = this;
                _opening = false;
                foreach (ContextMenu contextMenu in _items)
                    contextMenu.disabled = true;
            }
            else if (_confirm.result)
            {
                if (_publishItem.levelType == LevelType.Arcade_Machine)
                    _arcadeTest.Open("This machine can automatically show up in generated arcades, if you pass this validity test. You need to get the Platinum trophy on all 3 challenges (oh boy)!");
                else
                    _deathmatchTest.Open("In order to upload this map as a deathmatch level, all ducks need to be able to be eliminated. Do you want to launch the map and show that the map is functional? You don't have to do this, but the map won't show up with the DEATHMATCH tag without completing this test. If this is a challenge map, then don't worry about it!");
                _confirm.result = false;
            }
            else if (_testing)
            {
                Keyboard.keyString = "";
                if (DeathmatchTestDialogue.success)
                {
                    _testSuccess.Open("Test success! The level can now be published as a deathmatch level!");
                    _publishItem.deathmatchTestSuccess = true;
                }
                else if (ArcadeTestDialogue.success)
                {
                    if (_arcadeTestIndex > 0 && _arcadeTestIndex < 3)
                        _publishItem.subItems.ElementAt<EditorWorkshopItem>(_arcadeTestIndex).challengeTestSuccess = true;
                    do
                    {
                        ++_arcadeTestIndex;
                    }
                    while (_arcadeTestIndex <= 2 && _publishItem.subItems.ElementAt<EditorWorkshopItem>(_arcadeTestIndex).challengeTestSuccess);
                    _arcadeTestIndex = 3;
                    if (_arcadeTestIndex != 3)
                    {
                        ArcadeTestDialogue.success = false;
                        ArcadeTestDialogue.currentEditor = Level.current as Editor;
                        if (_arcadeTestIndex == 0)
                            Level.current = new ChallengeLevel((ArcadeTestDialogue.currentEditor.levelThings[0] as ArcadeMachine).challenge01Data, true);
                        else if (_arcadeTestIndex == 1)
                            Level.current = new ChallengeLevel((ArcadeTestDialogue.currentEditor.levelThings[0] as ArcadeMachine).challenge02Data, true);
                        else if (_arcadeTestIndex == 2)
                            Level.current = new ChallengeLevel((ArcadeTestDialogue.currentEditor.levelThings[0] as ArcadeMachine).challenge03Data, true);
                        _testing = true;
                        return;
                    }
                    _testSuccess.Open("Test success! The arcade machine can now be published to the workshop!");
                }
                else if (DeathmatchTestDialogue.tooSlow)
                {
                    _notify.Open("Framerate too low!");
                }
                else
                {
                    _notify.Open("Testing failed.");
                    _arcadeTestIndex = -1;
                }
                DeathmatchTestDialogue.success = false;
                ArcadeTestDialogue.success = false;
                _testing = false;
            }
            else if (_testSuccess.result)
            {
                Publish();
                _testSuccess.result = false;
            }
            else if (_deathmatchTest.result != -1)
            {
                if (_deathmatchTest.result == 1)
                    Publish();
                else if (_deathmatchTest.result == 0)
                {
                    DeathmatchTestDialogue.success = false;
                    DeathmatchTestDialogue.currentEditor = Level.current as Editor;
                    int num = 4;
                    if (Level.current is Editor && (Level.current as Editor).things[typeof(EightPlayer)].Count<Thing>() > 0)
                        num = 8;
                    for (int index = 0; index < num; ++index)
                    {
                        Profiles.defaultProfiles[index].team = Teams.allStock[index];
                        Profiles.defaultProfiles[index].persona = Profiles.defaultProfiles[index].defaultPersona;
                        Profiles.defaultProfiles[index].UpdatePersona();
                        Input.ApplyDefaultMapping(Profiles.defaultProfiles[index].inputProfile, Profiles.defaultProfiles[index]);
                    }
                    Level.current = new GameLevel(_levelData.GetPath(), validityTest: true);
                    _testing = true;
                }
                _deathmatchTest.result = -1;
            }
            else if (_arcadeTest.result != -1)
            {
                if (_arcadeTest.result == 1)
                    Publish();
                else if (_arcadeTest.result == 0)
                {
                    ArcadeTestDialogue.success = true;
                    _testing = true;
                    _arcadeTest.result = -1;
                    return;
                }
                _arcadeTest.result = -1;
            }
            else
            {
                if (_tagMenu != null)
                    return;
                Vec2 vec2 = new Vec2((float)(layer.width / 2.0 - width / 2.0) + hOffset, (float)(layer.height / 2.0 - height / 2.0 - 15.0)) + new Vec2(7f, 276f);
                foreach (KeyValuePair<string, Vec2> tagPosition in tagPositions)
                {
                    if (Mouse.x > tagPosition.Value.x && Mouse.x < tagPosition.Value.x + 8.0 && Mouse.y > tagPosition.Value.y && Mouse.y < tagPosition.Value.y + 8.0 && Mouse.left == InputState.Pressed)
                    {
                        _publishItem.RemoveTag(tagPosition.Key);
                        return;
                    }
                }
                if (tagPositions.Count != SteamUploadDialog.possibleTags.Count)
                {
                    bool flag = false;
                    if (Mouse.x > _plusPosition.x && Mouse.x < _plusPosition.x + 8.0 && Mouse.y > _plusPosition.y && Mouse.y < _plusPosition.y + 8.0)
                        flag = true;
                    if (flag && Mouse.left == InputState.Pressed)
                    {
                        ContextMenu contextMenu = new ContextMenu(this)
                        {
                            x = _plusPosition.x,
                            y = _plusPosition.y,
                            root = true,
                            depth = depth + 20
                        };
                        int num = 0;
                        foreach (string possibleTag in SteamUploadDialog.possibleTags)
                        {
                            if (!_publishItem.tags.Contains<string>(possibleTag))
                            {
                                contextMenu.AddItem(new ContextMenu(this)
                                {
                                    itemSize = {
                    x = 40f
                  },
                                    text = possibleTag
                                });
                                ++num;
                            }
                        }
                        contextMenu.y -= num * 16 + 10;
                        Level.Add(contextMenu);
                        contextMenu.opened = true;
                        contextMenu.closeOnRight = true;
                        _tagMenu = contextMenu;
                        Editor.PopFocus();
                        return;
                    }
                }
                Editor.lockInput = this;
                _descriptionBox.Update();
                _nameBox.Update();
                _acceptHover = false;
                _cancelHover = false;
                if (Mouse.x > _acceptPos.x && Mouse.x < _acceptPos.x + _acceptSize.x && Mouse.y > _acceptPos.y && Mouse.y < _acceptPos.y + _acceptSize.y)
                    _acceptHover = true;
                if (Mouse.x > _cancelPos.x && Mouse.x < _cancelPos.x + _cancelSize.x && Mouse.y > _cancelPos.y && Mouse.y < _cancelPos.y + _cancelSize.y)
                    _cancelHover = true;
                if (_acceptHover && Mouse.left == InputState.Pressed)
                {
                    if (_nameBox.text == "")
                        _notify.Open("Please enter a name :(");
                    else
                        _confirm.Open("Upload to workshop?");
                }
                if (_cancelHover && Mouse.left == InputState.Pressed)
                    Close();
                base.Update();
            }
        }

        public override void Draw()
        {
            menuSize.y = _fdHeight;
            if (!opened)
                return;
            base.Draw();
            float num1 = 328f;
            float num2 = _fdHeight + 22f;
            Vec2 p1 = new Vec2((float)(layer.width / 2.0 - num1 / 2.0) + hOffset, (float)(layer.height / 2.0 - num2 / 2.0 - 15.0));
            Vec2 p2 = new Vec2((float)(layer.width / 2.0 + num1 / 2.0) + hOffset, (float)(layer.height / 2.0 + num2 / 2.0 - 12.0));
            Graphics.DrawRect(p1, p2, new Color(70, 70, 70), depth, false);
            Graphics.DrawRect(p1, p2, new Color(30, 30, 30), depth - 8);
            Graphics.DrawRect(p1 + new Vec2(4f, 23f), p2 + new Vec2(-4f, -160f), new Color(10, 10, 10), depth - 4);
            Graphics.DrawRect(p1 + new Vec2(4f, 206f), p2 + new Vec2(-4f, -66f), new Color(10, 10, 10), depth - 4);
            Graphics.DrawRect(p1 + new Vec2(4f, 224f), p2 + new Vec2(-4f, -14f), new Color(10, 10, 10), depth - 4);
            Graphics.DrawRect(p1 + new Vec2(3f, 3f), new Vec2(p2.x - 3f, p1.y + 19f), new Color(70, 70, 70), depth - 4);
            if (_mod != null)
                Graphics.DrawString("Upload Mod to Workshop", p1 + new Vec2(5f, 7f), Color.White, depth + 8);
            else if (Editor.arcadeMachineMode)
                Graphics.DrawString("Upload " + _publishItem.levelType.ToString() + " to Workshop", p1 + new Vec2(5f, 7f), Color.White, depth + 8);
            else
                Graphics.DrawString("Upload " + _publishItem.levelSize.ToString() + " " + _publishItem.levelType.ToString() + " to Workshop", p1 + new Vec2(5f, 7f), Color.White, depth + 8);
            _descriptionBox.position = p1 + new Vec2(6f, 226f);
            _descriptionBox.depth = depth + 2;
            _descriptionBox.Draw();
            _nameBox.position = p1 + new Vec2(6f, 208f);
            _nameBox.depth = depth + 2;
            _nameBox.Draw();
            int num3 = 0;
            Vec2 vec2 = p1 + new Vec2(7f, 276f);
            int num4 = 0;
            tagPositions.Clear();
            foreach (string tag in _publishItem.tags)
            {
                int num5 = SteamUploadDialog.possibleTags.Contains(tag) ? 1 : 0;
                _workshopTag.depth = depth + 8;
                _workshopTag.frame = 0;
                Graphics.Draw(_workshopTag, vec2.x, vec2.y);
                float stringWidth = Graphics.GetStringWidth(tag, scale: 0.5f);
                float num6 = 4f;
                if (num5 == 0)
                    num6 = 0f;
                else
                    ++num4;
                Graphics.DrawTexturedLine(_workshopTagMiddle.texture, vec2 + new Vec2(4f, 4f), vec2 + new Vec2(4f + stringWidth + num6, 4f), Color.White, depth: (depth + 10));
                Graphics.DrawString(tag, vec2 + new Vec2(4f, 2f), Color.Black, depth + 14, scale: 0.5f);
                if (num5 != 0)
                {
                    Vec2 position = vec2 + new Vec2(stringWidth + 6f, 2f);
                    tagPositions[tag] = position;
                    Graphics.DrawString("x", position, Color.Red, depth + 14, scale: 0.5f);
                }
                _workshopTag.frame = 1;
                Graphics.Draw(_workshopTag, (float)(vec2.x + num6 + 4.0) + stringWidth, vec2.y);
                vec2.x += stringWidth + 11f + num6;
                ++num3;
            }
            if (num4 < SteamUploadDialog.possibleTags.Count)
            {
                _tagPlus.depth = depth + 8;
                vec2.x += 2f;
                Graphics.Draw(_tagPlus, vec2.x, vec2.y);
                _plusPosition = vec2;
            }
            _acceptPos = p2 + new Vec2(-78f, -12f);
            _acceptSize = new Vec2(34f, 8f);
            Graphics.DrawRect(_acceptPos, _acceptPos + _acceptSize, _acceptHover ? new Color(180, 180, 180) : new Color(110, 110, 110), depth - 4);
            Graphics.DrawString("PUBLISH!", _acceptPos + new Vec2(2f, 2f), Color.White, depth + 8, scale: 0.5f);
            _cancelPos = p2 + new Vec2(-36f, -12f);
            _cancelSize = new Vec2(32f, 8f);
            Graphics.DrawRect(_cancelPos, _cancelPos + _cancelSize, _cancelHover ? new Color(180, 180, 180) : new Color(110, 110, 110), depth - 4);
            Graphics.DrawString("CANCEL!", _cancelPos + new Vec2(2f, 2f), Color.White, depth + 8, scale: 0.5f);
            if (_previewTarget.width < 300)
            {
                _previewTarget.depth = depth + 10;
                _previewTarget.scale = new Vec2(0.5f, 0.5f);
                Graphics.Draw(_previewTarget, (float)(p1.x + (p2.x - p1.x) / 2.0 - _previewTarget.width * _previewTarget.scale.x / 2.0), (float)(p1.y + (p2.y - p1.y) / 2.0 - _previewTarget.height * _previewTarget.scale.y / 2.0 - 20.0));
            }
            else
            {
                _previewTarget.depth = depth + 10;
                _previewTarget.scale = new Vec2(0.25f, 0.25f);
                Graphics.Draw(_previewTarget, p1.x + 4f, p1.y + 23f);
            }
        }
    }
}
