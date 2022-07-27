// Decompiled with JetBrains decompiler
// Type: DuckGame.SteamUploadDialog
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
          : base((IContextListener)null)
        {
        }

        public override void Initialize()
        {
            this.layer = Layer.HUD;
            this.depth = (Depth)0.9f;
            this._showBackground = false;
            this.itemSize = new Vec2(386f, 16f);
            this._root = true;
            this.drawControls = false;
            this._descriptionBox = new Textbox(this.x + 5f, this.y + 225f, 316f, 40f, 0.5f, 9, "<ENTER DESCRIPTION>");
            this._nameBox = new Textbox(this.x + 5f, this.y + (float)byte.MaxValue, 316f, 12f, maxLines: 1, emptyText: "<ENTER NAME>");
            this._font = new FancyBitmapFont("smallFont");
            this._confirm = new MessageDialogue();
            Level.Add((Thing)this._confirm);
            this._upload = new UploadDialogue();
            Level.Add((Thing)this._upload);
            this._notify = new NotifyDialogue();
            Level.Add((Thing)this._notify);
            this._deathmatchTest = new DeathmatchTestDialogue();
            Level.Add((Thing)this._deathmatchTest);
            this._testSuccess = new TestSuccessDialogue();
            Level.Add((Thing)this._testSuccess);
            this._arcadeTest = new ArcadeTestDialogue();
            Level.Add((Thing)this._arcadeTest);
        }

        public void Open(LevelData pData)
        {
            SteamUploadDialog._editingMod = false;
            this._publishItem = (EditorWorkshopItem)null;
            Editor.lockInput = (ContextMenu)this;
            SFX.Play("openClick", 0.4f);
            this.opened = true;
            this._publishItem = new EditorWorkshopItem(pData);
            this._previewTarget = new Sprite(this._publishItem.preview);
            this._nameBox.text = pData.workshopData.name;
            this._descriptionBox.text = pData.workshopData.description;
            this._workshopTag = new SpriteMap("workshopTag", 4, 8);
            this._workshopTagMiddle = new Sprite("workshopTagMiddle");
            this._tagPlus = new Sprite("tagPlus");
            this._levelData = pData;
            this._arcadeTestIndex = 0;
        }

        public void Open(Mod pData)
        {
            SteamUploadDialog._editingMod = true;
            this._publishItem = (EditorWorkshopItem)null;
            Editor.lockInput = (ContextMenu)this;
            SFX.Play("openClick", 0.4f);
            this.opened = true;
            this._publishItem = new EditorWorkshopItem(pData);
            this._previewTarget = new Sprite(this._publishItem.preview);
            this._nameBox.text = pData.workshopData.name;
            this._descriptionBox.text = pData.workshopData.description;
            this._workshopTag = new SpriteMap("workshopTag", 4, 8);
            this._workshopTagMiddle = new Sprite("workshopTagMiddle");
            this._tagPlus = new Sprite("tagPlus");
            this._mod = pData;
            this._arcadeTestIndex = 0;
        }

        public void Close()
        {
            Editor.lockInput = (ContextMenu)null;
            this.opened = false;
            this._descriptionBox.LoseFocus();
            this._nameBox.LoseFocus();
            this._publishItem = (EditorWorkshopItem)null;
            this.ClearItems();
        }

        public override void Selected(ContextMenu item)
        {
            SFX.Play("highClick", 0.3f, 0.2f);
            if (item != null && item.text != "")
                this._publishItem.AddTag(item.text);
            if (this._tagMenu == null)
                return;
            this._tagMenu.opened = false;
            Level.Remove((Thing)this._tagMenu);
            this._tagMenu = (ContextMenu)null;
            if (Editor.PeekFocus() != this._tagMenu)
                return;
            Editor.PopFocus();
        }

        public override void Toggle(ContextMenu item)
        {
        }

        public static List<string> possibleTags => SteamUploadDialog._editingMod ? SteamUploadDialog._possibleTagsMod : SteamUploadDialog._possibleTagsLevel;

        private void Publish()
        {
            this._publishItem.name = this._nameBox.text;
            this._publishItem.description = this._descriptionBox.text;
            if (this._publishItem.PrepareItem() != SteamResult.OK)
            {
                this._notify.Open("Failed with code " + ((int)this._publishItem.result).ToString() + " (" + this._publishItem.result.ToString() + ")");
            }
            else
            {
                this._publishStack.Clear();
                this._publishStack.Push(this._publishItem);
                foreach (EditorWorkshopItem subItem in this._publishItem.subItems)
                    this._publishStack.Push(subItem);
                this.UploadNext();
            }
        }

        public bool UploadNext()
        {
            if (this._publishStack.Count == 0)
                return false;
            EditorWorkshopItem editorWorkshopItem = this._publishStack.Peek();
            editorWorkshopItem.Upload();
            if (editorWorkshopItem.subIndex == -1)
                this._upload.Open("Uploading...", editorWorkshopItem.item);
            else
                this._upload.Open("Uploading Sub Item(" + editorWorkshopItem.subIndex.ToString() + ")...", editorWorkshopItem.item);
            return true;
        }

        public override void Update()
        {
            if (this._publishStack.Count > 0)
            {
                if (!this._publishStack.Peek().finishedProcessing)
                    return;
                this._publishStack.Peek().FinishUpload();
                this._upload.Close();
                if (this._publishStack.Peek().result == SteamResult.OK)
                {
                    EditorWorkshopItem editorWorkshopItem = this._publishStack.Peek();
                    this._publishStack.Pop();
                    if (this.UploadNext())
                        return;
                    this._upload.Close();
                    this._notify.Open("Item published!");
                    Steam.ShowWorkshopLegalAgreement(editorWorkshopItem.item.id.ToString());
                }
                else
                {
                    this._notify.Open("Failed with code " + ((int)this._publishStack.Peek().result).ToString() + " (" + this._publishStack.Peek().result.ToString() + ")");
                    this._publishStack.Clear();
                }
            }
            else if (!this.opened || this._opening || this._confirm.opened || this._upload.opened || this._deathmatchTest.opened || this._arcadeTest.opened || this._testSuccess.opened)
            {
                if (this.opened)
                    Keyboard.keyString = "";
                if (this.opened)
                    Editor.lockInput = (ContextMenu)this;
                this._opening = false;
                foreach (ContextMenu contextMenu in this._items)
                    contextMenu.disabled = true;
            }
            else if (this._confirm.result)
            {
                if (this._publishItem.levelType == LevelType.Arcade_Machine)
                    this._arcadeTest.Open("This machine can automatically show up in generated arcades, if you pass this validity test. You need to get the Platinum trophy on all 3 challenges (oh boy)!");
                else
                    this._deathmatchTest.Open("In order to upload this map as a deathmatch level, all ducks need to be able to be eliminated. Do you want to launch the map and show that the map is functional? You don't have to do this, but the map won't show up with the DEATHMATCH tag without completing this test. If this is a challenge map, then don't worry about it!");
                this._confirm.result = false;
            }
            else if (this._testing)
            {
                Keyboard.keyString = "";
                if (DeathmatchTestDialogue.success)
                {
                    this._testSuccess.Open("Test success! The level can now be published as a deathmatch level!");
                    this._publishItem.deathmatchTestSuccess = true;
                }
                else if (ArcadeTestDialogue.success)
                {
                    if (this._arcadeTestIndex > 0 && this._arcadeTestIndex < 3)
                        this._publishItem.subItems.ElementAt<EditorWorkshopItem>(this._arcadeTestIndex).challengeTestSuccess = true;
                    do
                    {
                        ++this._arcadeTestIndex;
                    }
                    while (this._arcadeTestIndex <= 2 && this._publishItem.subItems.ElementAt<EditorWorkshopItem>(this._arcadeTestIndex).challengeTestSuccess);
                    this._arcadeTestIndex = 3;
                    if (this._arcadeTestIndex != 3)
                    {
                        ArcadeTestDialogue.success = false;
                        ArcadeTestDialogue.currentEditor = Level.current as Editor;
                        if (this._arcadeTestIndex == 0)
                            Level.current = (Level)new ChallengeLevel((ArcadeTestDialogue.currentEditor.levelThings[0] as ArcadeMachine).challenge01Data, true);
                        else if (this._arcadeTestIndex == 1)
                            Level.current = (Level)new ChallengeLevel((ArcadeTestDialogue.currentEditor.levelThings[0] as ArcadeMachine).challenge02Data, true);
                        else if (this._arcadeTestIndex == 2)
                            Level.current = (Level)new ChallengeLevel((ArcadeTestDialogue.currentEditor.levelThings[0] as ArcadeMachine).challenge03Data, true);
                        this._testing = true;
                        return;
                    }
                    this._testSuccess.Open("Test success! The arcade machine can now be published to the workshop!");
                }
                else if (DeathmatchTestDialogue.tooSlow)
                {
                    this._notify.Open("Framerate too low!");
                }
                else
                {
                    this._notify.Open("Testing failed.");
                    this._arcadeTestIndex = -1;
                }
                DeathmatchTestDialogue.success = false;
                ArcadeTestDialogue.success = false;
                this._testing = false;
            }
            else if (this._testSuccess.result)
            {
                this.Publish();
                this._testSuccess.result = false;
            }
            else if (this._deathmatchTest.result != -1)
            {
                if (this._deathmatchTest.result == 1)
                    this.Publish();
                else if (this._deathmatchTest.result == 0)
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
                    Level.current = (Level)new GameLevel(this._levelData.GetPath(), validityTest: true);
                    this._testing = true;
                }
                this._deathmatchTest.result = -1;
            }
            else if (this._arcadeTest.result != -1)
            {
                if (this._arcadeTest.result == 1)
                    this.Publish();
                else if (this._arcadeTest.result == 0)
                {
                    ArcadeTestDialogue.success = true;
                    this._testing = true;
                    this._arcadeTest.result = -1;
                    return;
                }
                this._arcadeTest.result = -1;
            }
            else
            {
                if (this._tagMenu != null)
                    return;
                Vec2 vec2 = new Vec2((float)((double)this.layer.width / 2.0 - (double)this.width / 2.0) + this.hOffset, (float)((double)this.layer.height / 2.0 - (double)this.height / 2.0 - 15.0)) + new Vec2(7f, 276f);
                foreach (KeyValuePair<string, Vec2> tagPosition in this.tagPositions)
                {
                    if ((double)Mouse.x > (double)tagPosition.Value.x && (double)Mouse.x < (double)tagPosition.Value.x + 8.0 && (double)Mouse.y > (double)tagPosition.Value.y && (double)Mouse.y < (double)tagPosition.Value.y + 8.0 && Mouse.left == InputState.Pressed)
                    {
                        this._publishItem.RemoveTag(tagPosition.Key);
                        return;
                    }
                }
                if (this.tagPositions.Count != SteamUploadDialog.possibleTags.Count)
                {
                    bool flag = false;
                    if ((double)Mouse.x > (double)this._plusPosition.x && (double)Mouse.x < (double)this._plusPosition.x + 8.0 && (double)Mouse.y > (double)this._plusPosition.y && (double)Mouse.y < (double)this._plusPosition.y + 8.0)
                        flag = true;
                    if (flag && Mouse.left == InputState.Pressed)
                    {
                        ContextMenu contextMenu = new ContextMenu((IContextListener)this);
                        contextMenu.x = this._plusPosition.x;
                        contextMenu.y = this._plusPosition.y;
                        contextMenu.root = true;
                        contextMenu.depth = this.depth + 20;
                        int num = 0;
                        foreach (string possibleTag in SteamUploadDialog.possibleTags)
                        {
                            if (!this._publishItem.tags.Contains<string>(possibleTag))
                            {
                                contextMenu.AddItem(new ContextMenu((IContextListener)this)
                                {
                                    itemSize = {
                    x = 40f
                  },
                                    text = possibleTag
                                });
                                ++num;
                            }
                        }
                        contextMenu.y -= (float)(num * 16 + 10);
                        Level.Add((Thing)contextMenu);
                        contextMenu.opened = true;
                        contextMenu.closeOnRight = true;
                        this._tagMenu = contextMenu;
                        Editor.PopFocus();
                        return;
                    }
                }
                Editor.lockInput = (ContextMenu)this;
                this._descriptionBox.Update();
                this._nameBox.Update();
                this._acceptHover = false;
                this._cancelHover = false;
                if ((double)Mouse.x > (double)this._acceptPos.x && (double)Mouse.x < (double)this._acceptPos.x + (double)this._acceptSize.x && (double)Mouse.y > (double)this._acceptPos.y && (double)Mouse.y < (double)this._acceptPos.y + (double)this._acceptSize.y)
                    this._acceptHover = true;
                if ((double)Mouse.x > (double)this._cancelPos.x && (double)Mouse.x < (double)this._cancelPos.x + (double)this._cancelSize.x && (double)Mouse.y > (double)this._cancelPos.y && (double)Mouse.y < (double)this._cancelPos.y + (double)this._cancelSize.y)
                    this._cancelHover = true;
                if (this._acceptHover && Mouse.left == InputState.Pressed)
                {
                    if (this._nameBox.text == "")
                        this._notify.Open("Please enter a name :(");
                    else
                        this._confirm.Open("Upload to workshop?");
                }
                if (this._cancelHover && Mouse.left == InputState.Pressed)
                    this.Close();
                base.Update();
            }
        }

        public override void Draw()
        {
            this.menuSize.y = this._fdHeight;
            if (!this.opened)
                return;
            base.Draw();
            float num1 = 328f;
            float num2 = this._fdHeight + 22f;
            Vec2 p1 = new Vec2((float)((double)this.layer.width / 2.0 - (double)num1 / 2.0) + this.hOffset, (float)((double)this.layer.height / 2.0 - (double)num2 / 2.0 - 15.0));
            Vec2 p2 = new Vec2((float)((double)this.layer.width / 2.0 + (double)num1 / 2.0) + this.hOffset, (float)((double)this.layer.height / 2.0 + (double)num2 / 2.0 - 12.0));
            Graphics.DrawRect(p1, p2, new Color(70, 70, 70), this.depth, false);
            Graphics.DrawRect(p1, p2, new Color(30, 30, 30), this.depth - 8);
            Graphics.DrawRect(p1 + new Vec2(4f, 23f), p2 + new Vec2(-4f, -160f), new Color(10, 10, 10), this.depth - 4);
            Graphics.DrawRect(p1 + new Vec2(4f, 206f), p2 + new Vec2(-4f, -66f), new Color(10, 10, 10), this.depth - 4);
            Graphics.DrawRect(p1 + new Vec2(4f, 224f), p2 + new Vec2(-4f, -14f), new Color(10, 10, 10), this.depth - 4);
            Graphics.DrawRect(p1 + new Vec2(3f, 3f), new Vec2(p2.x - 3f, p1.y + 19f), new Color(70, 70, 70), this.depth - 4);
            if (this._mod != null)
                Graphics.DrawString("Upload Mod to Workshop", p1 + new Vec2(5f, 7f), Color.White, this.depth + 8);
            else if (Editor.arcadeMachineMode)
                Graphics.DrawString("Upload " + this._publishItem.levelType.ToString() + " to Workshop", p1 + new Vec2(5f, 7f), Color.White, this.depth + 8);
            else
                Graphics.DrawString("Upload " + this._publishItem.levelSize.ToString() + " " + this._publishItem.levelType.ToString() + " to Workshop", p1 + new Vec2(5f, 7f), Color.White, this.depth + 8);
            this._descriptionBox.position = p1 + new Vec2(6f, 226f);
            this._descriptionBox.depth = this.depth + 2;
            this._descriptionBox.Draw();
            this._nameBox.position = p1 + new Vec2(6f, 208f);
            this._nameBox.depth = this.depth + 2;
            this._nameBox.Draw();
            int num3 = 0;
            Vec2 vec2 = p1 + new Vec2(7f, 276f);
            int num4 = 0;
            this.tagPositions.Clear();
            foreach (string tag in this._publishItem.tags)
            {
                int num5 = SteamUploadDialog.possibleTags.Contains(tag) ? 1 : 0;
                this._workshopTag.depth = this.depth + 8;
                this._workshopTag.frame = 0;
                Graphics.Draw((Sprite)this._workshopTag, vec2.x, vec2.y);
                float stringWidth = Graphics.GetStringWidth(tag, scale: 0.5f);
                float num6 = 4f;
                if (num5 == 0)
                    num6 = 0.0f;
                else
                    ++num4;
                Graphics.DrawTexturedLine(this._workshopTagMiddle.texture, vec2 + new Vec2(4f, 4f), vec2 + new Vec2(4f + stringWidth + num6, 4f), Color.White, depth: (this.depth + 10));
                Graphics.DrawString(tag, vec2 + new Vec2(4f, 2f), Color.Black, this.depth + 14, scale: 0.5f);
                if (num5 != 0)
                {
                    Vec2 position = vec2 + new Vec2(stringWidth + 6f, 2f);
                    this.tagPositions[tag] = position;
                    Graphics.DrawString("x", position, Color.Red, this.depth + 14, scale: 0.5f);
                }
                this._workshopTag.frame = 1;
                Graphics.Draw((Sprite)this._workshopTag, (float)((double)vec2.x + (double)num6 + 4.0) + stringWidth, vec2.y);
                vec2.x += stringWidth + 11f + num6;
                ++num3;
            }
            if (num4 < SteamUploadDialog.possibleTags.Count)
            {
                this._tagPlus.depth = this.depth + 8;
                vec2.x += 2f;
                Graphics.Draw(this._tagPlus, vec2.x, vec2.y);
                this._plusPosition = vec2;
            }
            this._acceptPos = p2 + new Vec2(-78f, -12f);
            this._acceptSize = new Vec2(34f, 8f);
            Graphics.DrawRect(this._acceptPos, this._acceptPos + this._acceptSize, this._acceptHover ? new Color(180, 180, 180) : new Color(110, 110, 110), this.depth - 4);
            Graphics.DrawString("PUBLISH!", this._acceptPos + new Vec2(2f, 2f), Color.White, this.depth + 8, scale: 0.5f);
            this._cancelPos = p2 + new Vec2(-36f, -12f);
            this._cancelSize = new Vec2(32f, 8f);
            Graphics.DrawRect(this._cancelPos, this._cancelPos + this._cancelSize, this._cancelHover ? new Color(180, 180, 180) : new Color(110, 110, 110), this.depth - 4);
            Graphics.DrawString("CANCEL!", this._cancelPos + new Vec2(2f, 2f), Color.White, this.depth + 8, scale: 0.5f);
            if (this._previewTarget.width < 300)
            {
                this._previewTarget.depth = this.depth + 10;
                this._previewTarget.scale = new Vec2(0.5f, 0.5f);
                Graphics.Draw(this._previewTarget, (float)((double)p1.x + ((double)p2.x - (double)p1.x) / 2.0 - (double)this._previewTarget.width * (double)this._previewTarget.scale.x / 2.0), (float)((double)p1.y + ((double)p2.y - (double)p1.y) / 2.0 - (double)this._previewTarget.height * (double)this._previewTarget.scale.y / 2.0 - 20.0));
            }
            else
            {
                this._previewTarget.depth = this.depth + 10;
                this._previewTarget.scale = new Vec2(0.25f, 0.25f);
                Graphics.Draw(this._previewTarget, p1.x + 4f, p1.y + 23f);
            }
        }
    }
}
