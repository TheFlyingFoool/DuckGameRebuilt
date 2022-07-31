// Decompiled with JetBrains decompiler
// Type: DuckGame.ProfileSelector
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ProfileSelector : Thing
    {
        private float _fade;
        private bool _open;
        private bool _closing;
        private int _controlPage;
        private ControlSetting _selectedSetting;
        private List<List<ControlSetting>> _controlSettingPages = new List<List<ControlSetting>>()
    {
      new List<ControlSetting>()
      {
        new ControlSetting()
        {
          name = "{",
          trigger = "LEFT",
          position = new Vec2(0f, 0f),
          column = 0,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "/",
          trigger = "RIGHT",
          position = new Vec2(35f, 0f),
          column = 0,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "}",
          trigger = "UP",
          position = new Vec2(70f, 0f),
          column = 1,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "~",
          trigger = "DOWN",
          position = new Vec2(105f, 0f),
          column = 1,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "JUMP  ",
          trigger = "JUMP",
          position = new Vec2(0f, 12f),
          column = 0
        },
        new ControlSetting()
        {
          name = "GRAB  ",
          trigger = "GRAB",
          position = new Vec2(0f, 24f),
          column = 0
        },
        new ControlSetting()
        {
          name = "STRAFE",
          trigger = "STRAFE",
          position = new Vec2(0f, 36f),
          column = 0
        },
        new ControlSetting()
        {
          name = "USE   ",
          trigger = "SHOOT",
          position = new Vec2(70f, 12f),
          column = 1
        },
        new ControlSetting()
        {
          name = "QUACK ",
          trigger = "QUACK",
          position = new Vec2(70f, 24f),
          column = 1
        },
        new ControlSetting()
        {
          name = "FALL  ",
          trigger = "RAGDOLL",
          position = new Vec2(70f, 36f),
          column = 1
        },
        new ControlSetting()
        {
          name = "START ",
          trigger = "START",
          position = new Vec2(0f, 48f),
          column = 0,
          condition =  x => x.allowStartRemap
        },
        new ControlSetting()
        {
          name = "PAGE 2>",
          trigger = "ANY",
          position = new Vec2(70f, 48f),
          column = 1,
          condition =  x => !(x is Keyboard),
          action =  x =>
          {
            ++x._controlPage;
            x._selectedSetting =  null;
            SFX.Play("page");
          }
        },
        new ControlSetting()
        {
          name = "PAGE 2>",
          trigger = "ANY",
          position = new Vec2(70f, 48f),
          column = 1,
          condition =  x => x is Keyboard,
          action =  x =>
          {
            x._controlPage += 2;
            x._selectedSetting =  null;
            SFX.Play("page");
          }
        }
      },
      new List<ControlSetting>()
      {
        new ControlSetting()
        {
          name = "MOVE  ",
          trigger = "LSTICK",
          position = new Vec2(0f, 12f),
          column = 0,
          condition =  x => x.numSticks > 1
        },
        new ControlSetting()
        {
          name = "PITCH ",
          trigger = "LTRIGGER",
          position = new Vec2(0f, 24f),
          column = 0,
          condition =  x => x.numTriggers > 1
        },
        new ControlSetting()
        {
          name = "LICK  ",
          trigger = "RSTICK",
          position = new Vec2(70f, 12f),
          column = 1,
          condition =  x => x.numSticks > 1
        },
        new ControlSetting()
        {
          name = "ZOOM  ",
          trigger = "RTRIGGER",
          position = new Vec2(70f, 24f),
          column = 1,
          condition =  x => x.numTriggers > 1
        },
        new ControlSetting()
        {
          name = "<PAGE 1",
          trigger = "ANY",
          position = new Vec2(0f, 60f),
          column = 0,
          action =  x =>
          {
            --x._controlPage;
            x._selectedSetting =  null;
            SFX.Play("page");
          }
        },
        new ControlSetting()
        {
          name = "PAGE 3>",
          trigger = "ANY",
          position = new Vec2(70f, 60f),
          column = 1,
          action =  x =>
          {
            ++x._controlPage;
            x._selectedSetting =  null;
            SFX.Play("page");
          }
        }
      },
      new List<ControlSetting>()
      {
        new ControlSetting()
        {
          name = "UI CONTROLS...",
          trigger = "ANY",
          position = new Vec2(0f, 0f),
          column = 0,
          caption = true
        },
        new ControlSetting()
        {
          name = "{",
          trigger = "MENULEFT",
          position = new Vec2(0f, 12f),
          column = 0,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "/",
          trigger = "MENURIGHT",
          position = new Vec2(35f, 12f),
          column = 0,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "}",
          trigger = "MENUUP",
          position = new Vec2(70f, 12f),
          column = 1,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "~",
          trigger = "MENUDOWN",
          position = new Vec2(105f, 12f),
          column = 1,
          condition =  x => x.allowDirectionalMapping
        },
        new ControlSetting()
        {
          name = "SELECT",
          trigger = "SELECT",
          position = new Vec2(0f, 24f),
          column = 0
        },
        new ControlSetting()
        {
          name = "MENU 1",
          trigger = "MENU1",
          position = new Vec2(0f, 36f),
          column = 0
        },
        new ControlSetting()
        {
          name = "CANCEL",
          trigger = "CANCEL",
          position = new Vec2(70f, 24f),
          column = 1
        },
        new ControlSetting()
        {
          name = "MENU 2",
          trigger = "MENU2",
          position = new Vec2(70f, 36f),
          column = 1
        },
        new ControlSetting()
        {
          name = "<PAGE 2",
          trigger = "ANY",
          position = new Vec2(0f, 48f),
          column = 0,
          condition =  x => !(x is Keyboard),
          action =  x =>
          {
            --x._controlPage;
            x._selectedSetting =  null;
            SFX.Play("page");
          }
        },
        new ControlSetting()
        {
          name = "<PAGE 1",
          trigger = "ANY",
          position = new Vec2(0f, 48f),
          column = 0,
          condition =  x => x is Keyboard,
          action =  x =>
          {
            x._controlPage -= 2;
            x._selectedSetting =  null;
            SFX.Play("page");
          }
        },
        new ControlSetting()
        {
          name = "RESET ",
          trigger = "ANY",
          position = new Vec2(70f, 48f),
          column = 1,
          action =  x =>
          {
            ProfileSelector._madeControlChanges = true;
            x._configInputMapping = Input.GetDefaultMapping(x._inputProfile.lastActiveDevice.productName, x._inputProfile.lastActiveDevice.productGUID).Clone();
            SFX.Play("consoleSelect");
          }
        }
      }
    };
        private float _takenFlash;
        private string _name = "";
        private string _maskName = "aaaaaaaaa";
        private List<char> _characters = new List<char>()
    {
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      '{',
      '}',
      ' ',
      '-',
      '!'
    };
        private float _slide;
        private float _slideTo;
        private int _editControlSelection;
        private bool _changeName;
        private int _currentLetter;
        private int _controlPosition = 1;
        private bool _editControl;
        private PSMode _mode;
        private PSMode _desiredMode;
        private PSCreateSelection _createSelection;
        private InputProfile _inputProfile;
        private int _selectorPosition = -1;
        private int _desiredSelectorPosition = -1;
        private Profile _profile;
        private BitmapFont _font;
        private BitmapFont _smallFont;
        private BitmapFont _controlsFont;
        private List<Profile> _profiles;
        private ProfileBox2 _box;
        private SpriteMap _happyIcons;
        private SpriteMap _angryIcons;
        private SpriteMap _spinnerArrows;
        private Profile _starterProfile;
        private HatSelector _selector;
        public static bool _madeControlChanges;
        private DeviceInputMapping _configInputMapping;
        private UIMenu _confirmMenu;
        private MenuBoolean _deleteProfile = new MenuBoolean();
        private Profile _deleteContext;
        private bool _wasDown;
        private bool _autoSelect;
        private List<DeviceInputMapping> _pendingMaps = new List<DeviceInputMapping>();
        private bool isEditing;
        private float _moodVal = 0.5f;
        private int _preferredColor;

        public float fade => this._fade;

        public bool open => this._open;

        public ProfileSelector(float xpos, float ypos, ProfileBox2 box, HatSelector sel)
          : base(xpos, ypos)
        {
            this._font = new BitmapFont("biosFontUI", 8, 7)
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            this._collisionSize = new Vec2(141f, 89f);
            this._spinnerArrows = new SpriteMap("spinnerArrows", 8, 4);
            this._box = box;
            this._selector = sel;
            this._happyIcons = new SpriteMap("happyFace", 16, 16);
            this._happyIcons.CenterOrigin();
            this._angryIcons = new SpriteMap("angryFace", 16, 16);
            this._angryIcons.CenterOrigin();
            this._smallFont = new BitmapFont("smallBiosFont", 7, 6)
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            this._controlsFont = new BitmapFont("biosFontUIArrows", 8, 7)
            {
                scale = new Vec2(1f)
            };
        }

        public override void Initialize()
        {
            this._confirmMenu = new UIMenu("DELETE PROFILE!?", 320f / 2f, 180f / 2f, 160f, conString: "@SELECT@SELECT @CANCEL@OH NO!");
            this._confirmMenu.Add(new UIMenuItem("WHAT? NO!", new UIMenuActionCloseMenu(_confirmMenu), backButton: true), true);
            this._confirmMenu.Add(new UIMenuItem("YEAH!", new UIMenuActionCloseMenuSetBoolean(_confirmMenu, this._deleteProfile)), true);
            this._confirmMenu.Close();
            Level.Add(_confirmMenu);
            base.Initialize();
        }

        public void Reset()
        {
            this._open = false;
            this._selector.fade = 1f;
            this._fade = 0f;
            this._desiredMode = PSMode.SelectProfile;
            this._controlPage = 0;
            this._selectedSetting = null;
            this._configInputMapping = null;
        }

        public string GetMaskName(int length)
        {
            string maskName = "";
            for (int index = 0; index < 9; ++index)
                maskName = index >= length ? maskName + " " : maskName + this._maskName[index].ToString();
            return maskName;
        }

        public void SelectDown()
        {
            if (this._desiredSelectorPosition >= this._profiles.Count - 1)
                this._desiredSelectorPosition = -1;
            else
                ++this._desiredSelectorPosition;
            this._slideTo = 1f;
        }

        public void SelectUp()
        {
            if (this._desiredSelectorPosition <= -1)
                this._desiredSelectorPosition = this._profiles.Count - 1;
            else
                --this._desiredSelectorPosition;
            this._slideTo = -1f;
        }

        public int GetCharIndex(char c)
        {
            for (int index = 0; index < this._characters.Count; ++index)
            {
                if (this._characters[index] == c)
                    return index;
            }
            return -1;
        }

        private void ApplyInputSettings(Profile p)
        {
            if (p == null)
                return;
            if (this._pendingMaps.Count > 0)
            {
                p.inputMappingOverrides.Clear();
                foreach (DeviceInputMapping pendingMap in this._pendingMaps)
                    Input.SetDefaultMapping(pendingMap, p);
            }
            p.inputProfile = this._inputProfile;
            this._pendingMaps.Clear();
            Input.ApplyDefaultMapping(this._inputProfile, this._profile);
        }

        private void RebuildProfileList()
        {
            this._profiles = Profiles.allCustomProfiles;
            if (this._box.controllerIndex != 0 || !Options.Data.defaultAccountMerged)
                this._profiles.Add(Profiles.universalProfileList.ElementAt<Profile>(this._box.controllerIndex));
            if (!Network.isActive)
                return;
            this._profiles.Remove(Profiles.experienceProfile);
        }

        private void SaveSettings(bool pIsEditing, bool pAccepted)
        {
            if (!pIsEditing && !pAccepted)
                return;
            string varName = this._name.Replace(" ", "");
            Profile p = this._profile;
            if (!pIsEditing)
                p = new Profile(varName);
            p.funslider = this._moodVal;
            p.preferredColor = this._preferredColor;
            p.UpdatePersona();
            this.ApplyInputSettings(p);
            if (!pIsEditing)
                Profiles.Add(p);
            if (!pIsEditing)
            {
                this.RebuildProfileList();
                for (int index = 0; index < this._profiles.Count; ++index)
                {
                    if (this._profiles[index].name == varName)
                    {
                        this._selectorPosition = index;
                        this._desiredSelectorPosition = this._selectorPosition;
                        break;
                    }
                }
                this._desiredMode = PSMode.SelectProfile;
                this._autoSelect = true;
            }
            else
            {
                Profiles.Save(this._profile);
                this._desiredMode = PSMode.SelectProfile;
                this._mode = PSMode.SelectProfile;
                this._open = false;
                this._selector.fade = 1f;
                this._fade = 0f;
                this._selector.screen.DoFlashTransition();
            }
            SFX.Play("consoleSelect", 0.4f);
        }

        private bool HoveredProfileIsCustom() => this._selectorPosition != -1 && this.hoveredProfile.steamID == 0UL && Profiles.experienceProfile != this.hoveredProfile && !Profiles.IsDefault(this.hoveredProfile);

        private Profile hoveredProfile => this._selectorPosition >= 0 && this._selectorPosition < this._profiles.Count ? this._profiles[this._selectorPosition] : Profiles.DefaultPlayer1;

        public override void Update()
        {
            if (this._selector.screen.transitioning)
                return;
            this._takenFlash = Lerp.Float(this._takenFlash, 0f, 0.02f);
            if (!this._open)
            {
                if (_fade >= 0.01f || !this._closing)
                    return;
                this._closing = false;
            }
            else if (this._configInputMapping != null && this._inputProfile != null && this._configInputMapping.device.productName + this._configInputMapping.device.productGUID != this._inputProfile.lastActiveDevice.productName + this._inputProfile.lastActiveDevice.productGUID)
            {
                this._open = false;
                this._selector.fade = 1f;
                this._fade = 0f;
                this._selector.screen.DoFlashTransition();
                this._desiredMode = PSMode.SelectProfile;
                SFX.Play("consoleCancel", 0.4f);
            }
            else
            {
                if (this._mode != this._desiredMode)
                {
                    this._selector.screen.DoFlashTransition();
                    this._mode = this._desiredMode;
                }
                if (_fade > 0.9f && this._mode != PSMode.CreateProfile && this._mode != PSMode.EditProfile && this._mode != PSMode.EditControls && this._mode != PSMode.EditControlsConfirm && this._desiredSelectorPosition == this._selectorPosition)
                {
                    if (this._inputProfile.Down("MENUUP"))
                    {
                        this.SelectUp();
                        this._wasDown = false;
                        if (this._profiles.Count > 0)
                            SFX.Play("consoleTick");
                    }
                    if (this._inputProfile.Down("MENUDOWN"))
                    {
                        this.SelectDown();
                        this._wasDown = true;
                        if (this._profiles.Count > 0)
                            SFX.Play("consoleTick");
                    }
                    if (this.HoveredProfileIsCustom() && MonoMain.pauseMenu == null && this._inputProfile.Pressed("MENU2"))
                    {
                        this._deleteContext = this._profiles[this._selectorPosition];
                        MonoMain.pauseMenu = _confirmMenu;
                        this._confirmMenu.Open();
                        SFX.Play("pause", 0.6f);
                    }
                    if (this._deleteProfile.value)
                    {
                        this._deleteProfile.value = false;
                        if (this._deleteContext != null)
                        {
                            Profiles.Delete(this._deleteContext);
                            this.SelectUp();
                            this.RebuildProfileList();
                            this._slide = this._slideTo;
                            this._deleteContext = null;
                        }
                    }
                    if (this._inputProfile.Pressed("CANCEL"))
                    {
                        if (Profiles.IsDefault(this._starterProfile) || !(Level.current is TeamSelect2))
                            this._box.ChangeProfile(this._starterProfile);
                        this._open = false;
                        this._selector.fade = 1f;
                        this._fade = 0f;
                        this._selector.screen.DoFlashTransition();
                        SFX.Play("consoleCancel", 0.4f);
                        return;
                    }
                    if (this._inputProfile.Pressed("SELECT") || this._autoSelect)
                    {
                        this._autoSelect = false;
                        if (this._selectorPosition == -1)
                        {
                            this._desiredMode = PSMode.CreateProfile;
                            this._changeName = true;
                            this._currentLetter = 0;
                            this._createSelection = PSCreateSelection.ChangeName;
                            this._maskName = "aaaaaaaaa";
                            this._name = this.GetMaskName(1);
                            SFX.Play("consoleSelect", 0.4f);
                        }
                        else if (this.ProfileAlreadySelected(this._profiles[this._selectorPosition]))
                        {
                            SFX.Play("consoleError");
                        }
                        else
                        {
                            if (this._profiles[this._selectorPosition].linkedProfile == null)
                            {
                                if (Network.isActive)
                                {
                                    this._profile.linkedProfile = this._profiles[this._selectorPosition];
                                    Input.ApplyDefaultMapping(this._inputProfile, this._profile);
                                    this._profile.UpdatePersona();
                                }
                                else if (this._selectorPosition != -1)
                                {
                                    this._box.ChangeProfile(this._profiles[this._selectorPosition]);
                                    this._profile = this._profiles[this._selectorPosition];
                                    this._profile.inputProfile = null;
                                    this._profile.inputProfile = this._inputProfile;
                                    Input.ApplyDefaultMapping(this._inputProfile, this._profile);
                                }
                            }
                            this._selector.ConfirmProfile();
                            this._open = false;
                            this._selector.fade = 1f;
                            this._fade = 0f;
                            this._selector.screen.DoFlashTransition();
                            SFX.Play("consoleSelect", 0.4f);
                        }
                    }
                }
                else if (this._mode == PSMode.EditControlsConfirm)
                {
                    if (this._inputProfile.Pressed("MENUUP"))
                    {
                        SFX.Play("consoleTick");
                        --this._editControlSelection;
                    }
                    else if (this._inputProfile.Pressed("MENUDOWN"))
                    {
                        SFX.Play("consoleTick");
                        ++this._editControlSelection;
                    }
                    else
                    {
                        if (this._inputProfile.Pressed("CANCEL"))
                        {
                            this._desiredMode = PSMode.EditControls;
                            SFX.Play("consoleError");
                            return;
                        }
                        if (this._inputProfile.Pressed("SELECT"))
                        {
                            SFX.Play("consoleSelect");
                            if (this._editControlSelection == 0)
                            {
                                this._pendingMaps.Add(this._configInputMapping);
                                this.ApplyInputSettings(this._profile);
                            }
                            else
                                this._configInputMapping = Input.GetDefaultMapping(this._inputProfile.lastActiveDevice.productName, this._inputProfile.lastActiveDevice.productGUID, p: (this.isEditing ? this._profile : null)).Clone();
                            this._desiredMode = PSMode.CreateProfile;
                        }
                    }
                    if (this._editControlSelection > 1)
                        this._editControlSelection = 1;
                    if (this._editControlSelection < 0)
                        this._editControlSelection = 0;
                }
                else if (this._mode == PSMode.EditControls)
                {
                    if (!this._editControl)
                    {
                        InputDevice d = this._inputProfile.lastActiveDevice;
                        if (d is GenericController)
                            d = (d as GenericController).device;
                        if (this._selectedSetting == null)
                            this._selectedSetting = this._controlSettingPages[this._controlPage].Find(x => (x.condition == null || x.condition(d)) && !x.caption);
                        Vec2 zero = Vec2.Zero;
                        if (this._inputProfile.Pressed("MENUUP"))
                            zero += new Vec2(0f, -8f);
                        else if (this._inputProfile.Pressed("MENUDOWN"))
                            zero += new Vec2(0f, 8f);
                        else if (this._inputProfile.Pressed("MENULEFT"))
                            zero += new Vec2(-30f, 0f);
                        else if (this._inputProfile.Pressed("MENURIGHT"))
                            zero += new Vec2(30f, 0f);
                        if (zero != Vec2.Zero)
                        {
                            ControlSetting controlSetting1 = null;
                            foreach (ControlSetting controlSetting2 in this._controlSettingPages[this._controlPage])
                            {
                                if ((controlSetting2.condition == null || controlSetting2.condition(d)) && !controlSetting2.caption)
                                {
                                    if (zero.x != 0.0)
                                    {
                                        if (controlSetting2.position.y == (double)this._selectedSetting.position.y)
                                        {
                                            if (zero.x > 0.0)
                                            {
                                                if (controlSetting2.position.x > (double)this._selectedSetting.position.x && (controlSetting1 == null || controlSetting2.position.x < (double)controlSetting1.position.x))
                                                    controlSetting1 = controlSetting2;
                                            }
                                            else if (controlSetting2.position.x < (double)this._selectedSetting.position.x && (controlSetting1 == null || controlSetting2.position.x > (double)controlSetting1.position.x))
                                                controlSetting1 = controlSetting2;
                                        }
                                    }
                                    else if (controlSetting2.position.x == (double)this._selectedSetting.position.x || controlSetting2.column == this._selectedSetting.column)
                                    {
                                        if (zero.y > 0.0)
                                        {
                                            if (controlSetting2.position.y > (double)this._selectedSetting.position.y && (controlSetting1 == null || controlSetting2.position.y < (double)controlSetting1.position.y))
                                                controlSetting1 = controlSetting2;
                                        }
                                        else if (controlSetting2.position.y < (double)this._selectedSetting.position.y && (controlSetting1 == null || controlSetting2.position.y > (double)controlSetting1.position.y))
                                            controlSetting1 = controlSetting2;
                                    }
                                }
                            }
                            if (controlSetting1 != null)
                                this._selectedSetting = controlSetting1;
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("SELECT"))
                        {
                            if (this._selectedSetting.action != null)
                            {
                                this._selectedSetting.action(this);
                            }
                            else
                            {
                                this._editControl = true;
                                SFX.Play("consoleTick");
                            }
                        }
                        else
                        {
                            if (this._inputProfile.Pressed("CANCEL"))
                            {
                                if (ProfileSelector._madeControlChanges)
                                {
                                    this._editControlSelection = 0;
                                    this._desiredMode = PSMode.EditControlsConfirm;
                                    SFX.Play("consoleError");
                                    return;
                                }
                                this._desiredMode = PSMode.CreateProfile;
                                SFX.Play("consoleError");
                                return;
                            }
                            if (this._inputProfile.Pressed("START"))
                            {
                                this._pendingMaps.Add(this._configInputMapping);
                                this.ApplyInputSettings(this._profile);
                                this._desiredMode = PSMode.CreateProfile;
                                SFX.Play("consoleSelect");
                                return;
                            }
                        }
                    }
                    else if (this._inputProfile.Pressed("START"))
                    {
                        this._editControl = false;
                        SFX.Play("consoleError");
                    }
                    else
                    {
                        this._configInputMapping.deviceOverride = this._inputProfile.lastActiveDevice;
                        if (this._configInputMapping.deviceOverride is GenericController)
                            this._configInputMapping.deviceOverride = (_configInputMapping.deviceOverride as GenericController).device;
                        if (this._selectedSetting.trigger != "ANY" && this._configInputMapping.RunMappingUpdate(this._selectedSetting.trigger, false))
                        {
                            this._editControl = false;
                            SFX.Play("consoleSelect");
                            ProfileSelector._madeControlChanges = true;
                            this._configInputMapping.deviceOverride = null;
                            return;
                        }
                        this._configInputMapping.deviceOverride = null;
                    }
                }
                else if (this._mode == PSMode.CreateProfile)
                {
                    if (!this._changeName)
                    {
                        if (this._createSelection == PSCreateSelection.Controls && this._inputProfile.Pressed("SELECT"))
                        {
                            this._desiredMode = PSMode.EditControls;
                            this._selectedSetting = null;
                            this._controlPage = 0;
                            ProfileSelector._madeControlChanges = false;
                            if (this._configInputMapping == null)
                                this._configInputMapping = Input.GetDefaultMapping(this._inputProfile.lastActiveDevice.productName, this._inputProfile.lastActiveDevice.productGUID, p: (this.isEditing ? this._profile : null)).Clone();
                            SFX.Play("consoleTick");
                        }
                        if (this._createSelection == PSCreateSelection.Mood)
                        {
                            if (this._inputProfile.Pressed("MENULEFT"))
                            {
                                this._moodVal = Maths.Clamp(this._moodVal - 0.25f, 0f, 1f);
                                SFX.Play("consoleTick");
                            }
                            if (this._inputProfile.Pressed("MENURIGHT"))
                            {
                                this._moodVal = Maths.Clamp(this._moodVal + 0.25f, 0f, 1f);
                                SFX.Play("consoleTick");
                            }
                        }
                        if (this._createSelection == PSCreateSelection.Color)
                        {
                            if (this._inputProfile.Pressed("MENULEFT"))
                            {
                                this._preferredColor = Maths.Clamp(this._preferredColor - 1, -1, DG.MaxPlayers - 1);
                                SFX.Play("consoleTick");
                            }
                            if (this._inputProfile.Pressed("MENURIGHT"))
                            {
                                this._preferredColor = Maths.Clamp(this._preferredColor + 1, -1, DG.MaxPlayers - 1);
                                SFX.Play("consoleTick");
                            }
                        }
                        if (this._inputProfile.Pressed("MENUDOWN") && this._name != "" && this._createSelection < PSCreateSelection.Accept)
                        {
                            ++this._createSelection;
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("MENUUP") && this._name != "" && this._createSelection > (this.isEditing ? PSCreateSelection.Mood : PSCreateSelection.ChangeName))
                        {
                            --this._createSelection;
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("SELECT"))
                        {
                            if (this._createSelection == PSCreateSelection.ChangeName)
                            {
                                if (!this.isEditing)
                                {
                                    this._changeName = true;
                                    if (this._name == "")
                                        this._name = this.GetMaskName(1);
                                    SFX.Play("consoleSelect", 0.4f);
                                }
                                else
                                    SFX.Play("consoleError", 0.8f);
                            }
                            else if (this._createSelection == PSCreateSelection.Accept)
                            {
                                this.SaveSettings(this.isEditing, true);
                                SFX.Play("consoleSelect", 0.4f);
                            }
                        }
                        if (this._inputProfile.Pressed("CANCEL"))
                        {
                            this.SaveSettings(this.isEditing, false);
                            if (!this.isEditing)
                            {
                                this._desiredMode = PSMode.SelectProfile;
                            }
                            else
                            {
                                this._desiredMode = PSMode.SelectProfile;
                                this._mode = PSMode.SelectProfile;
                                this._open = false;
                                this._selector.fade = 1f;
                                this._fade = 0f;
                                this._selector.screen.DoFlashTransition();
                            }
                            SFX.Play("consoleCancel", 0.4f);
                        }
                    }
                    else
                    {
                        InputProfile.repeat = true;
                        Keyboard.repeat = true;
                        if (this._inputProfile.Pressed("SELECT"))
                        {
                            string str = this._name.Replace(" ", "");
                            if (str == "")
                            {
                                str = "duckis91";
                                this._name = str + " ";
                                this._currentLetter = 7;
                            }
                            List<Profile> allCustomProfiles = Profiles.allCustomProfiles;
                            bool flag = false;
                            if (this._selector == null || !this._selector.isArcadeHatSelector)
                            {
                                foreach (Profile profile in allCustomProfiles)
                                {
                                    if (profile.name == str)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                            if (flag)
                                this._takenFlash = 1f;
                            else
                                this._changeName = false;
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("MENULEFT"))
                        {
                            --this._currentLetter;
                            if (this._currentLetter < 0)
                            {
                                this._currentLetter = 0;
                            }
                            else
                            {
                                this._name = this._name.Remove(this._currentLetter + 1, 1);
                                this._name = this._name.Insert(this._currentLetter + 1, " ");
                            }
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("MENURIGHT"))
                        {
                            ++this._currentLetter;
                            if (this._currentLetter > 8)
                            {
                                this._currentLetter = 8;
                            }
                            else
                            {
                                this._name = this._name.Remove(this._currentLetter, 1);
                                if (this._currentLetter > 0)
                                    this._name = this._name.Insert(this._currentLetter, this._name[this._currentLetter - 1].ToString() ?? "");
                            }
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("MENUUP"))
                        {
                            int index = this.GetCharIndex(this._name[this._currentLetter]) + 1;
                            if (index >= this._characters.Count)
                                index = 0;
                            char character = this._characters[index];
                            this._name = this._name.Remove(this._currentLetter, 1);
                            this._name = this._name.Insert(this._currentLetter, character.ToString() ?? "");
                            this._maskName = this._maskName.Remove(this._currentLetter, 1);
                            this._maskName = this._maskName.Insert(this._currentLetter, character.ToString() ?? "");
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("MENUDOWN"))
                        {
                            int index = this.GetCharIndex(this._name[this._currentLetter]) - 1;
                            if (index < 0)
                                index = this._characters.Count - 1;
                            char character = this._characters[index];
                            this._name = this._name.Remove(this._currentLetter, 1);
                            this._name = this._name.Insert(this._currentLetter, character.ToString() ?? "");
                            this._maskName = this._maskName.Remove(this._currentLetter, 1);
                            this._maskName = this._maskName.Insert(this._currentLetter, character.ToString() ?? "");
                            SFX.Play("consoleTick");
                        }
                        if (this._inputProfile.Pressed("CANCEL"))
                        {
                            this._desiredMode = PSMode.SelectProfile;
                            SFX.Play("consoleCancel", 0.4f);
                        }
                    }
                }
                if (_slideTo != 0.0 && _slide != (double)this._slideTo)
                    this._slide = Lerp.Float(this._slide, this._slideTo, 0.1f);
                else if (_slideTo != 0.0 && _slide == (double)this._slideTo)
                {
                    this._slide = 0f;
                    this._slideTo = 0f;
                    if (this._desiredSelectorPosition != -1 && this.ProfileAlreadySelected(this._profiles[this._desiredSelectorPosition]))
                    {
                        this._selectorPosition = this._desiredSelectorPosition;
                        if (this._wasDown)
                            this.SelectDown();
                        else
                            this.SelectUp();
                    }
                    else
                    {
                        this._selectorPosition = this._desiredSelectorPosition;
                        if (!(Level.current is TeamSelect2))
                        {
                            if (this._selectorPosition != -1)
                            {
                                this._box.ChangeProfile(this._profiles[this._selectorPosition]);
                                this._profile = this._profiles[this._selectorPosition];
                            }
                            else
                            {
                                this._box.ChangeProfile(null);
                                this._profile = this._box.profile;
                            }
                        }
                    }
                }
                this._font.alpha = this._fade;
                this._font.depth = (Depth)0.96f;
                this._font.scale = new Vec2(1f, 1f);
                if (this._mode == PSMode.EditControlsConfirm)
                {
                    Vec2 position = this.position;
                    this.position = Vec2.Zero;
                    this._selector.screen.BeginDraw();
                    string text = "SAVE CHANGES?";
                    this._smallFont.Draw(text, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._smallFont.GetWidth(text) / 2.0), this.y + 22f)), Colors.MenuOption * (this._controlPosition == 0 ? 1f : 0.6f), (Depth)0.95f);
                    Vec2 vec2 = new Vec2((float)((double)this.x + (double)this.width / 2.0 - 66.0), this.y + 18f) + new Vec2(0.5f, 0f);
                    this._smallFont.Draw("YES", Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._smallFont.GetWidth("YES") / 2.0), this.y + 34f)), Colors.MenuOption * (this._editControlSelection == 0 ? 1f : 0.6f), (Depth)0.95f);
                    this._smallFont.Draw("NO", Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._smallFont.GetWidth("NO") / 2.0), (float)((double)this.y + 34.0 + 8.0))), Colors.MenuOption * (this._editControlSelection == 1 ? 1f : 0.6f), (Depth)0.95f);
                    this._font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    this._font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    this.position = position;
                    this._selector.screen.EndDraw();
                }
                else if (this._mode == PSMode.EditControls)
                {
                    Vec2 position1 = this.position;
                    this.position = Vec2.Zero;
                    this._selector.screen.BeginDraw();
                    InputProfile inputProfile = this._inputProfile;
                    this._smallFont.scale = new Vec2(1f, 1f);
                    float num = 6f;
                    string text = inputProfile.lastActiveDevice.productName;
                    if (text == null)
                    {
                        this._desiredMode = PSMode.CreateProfile;
                        SFX.Play("consoleError");
                    }
                    else
                    {
                        if (text == "Joy-Con (L)" || text == "Joy-Con (R)")
                            text = "Joy-Con (L)/(R)";
                        if (text.Length > 15)
                            text = text.Substring(0, 15) + "...";
                        if (this._controlPosition == 0)
                            text = "< " + text + " >";
                        this._smallFont.Draw(text, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._smallFont.GetWidth(text) / 2.0), this.y + num)), Colors.MenuOption * (this._controlPosition == 0 ? 1f : 0.6f), (Depth)0.95f);
                        Vec2 vec2 = new Vec2((float)((double)this.x + (double)this.width / 2.0 - 66.0), (float)((double)this.y + (double)num + 9.0)) + new Vec2(0.5f, 0f);
                        bool flag = false;
                        foreach (ControlSetting controlSetting in this._controlSettingPages[this._controlPage])
                        {
                            InputDevice inputDevice = this._inputProfile.lastActiveDevice;
                            if (inputDevice is GenericController)
                                inputDevice = (inputDevice as GenericController).device;
                            if (controlSetting.condition == null || controlSetting.condition(inputDevice))
                            {
                                string name = controlSetting.name;
                                Vec2 position2 = controlSetting.position;
                                if (position2.y == 0.0)
                                    flag = true;
                                else if (!flag && (this._controlPage != 0 || controlSetting != this._controlSettingPages[this._controlPage][this._controlSettingPages[this._controlPage].Count - 1]))
                                    position2.y -= 12f;
                                if (controlSetting.trigger != "ANY")
                                {
                                    name += ":|DGBLUE|";
                                    if (!this._editControl || this._selectedSetting != controlSetting)
                                        Graphics.Draw(inputProfile.lastActiveDevice.GetMapImage(this._configInputMapping.map[controlSetting.trigger]), (float)(vec2.x + (double)position2.x + (double)this._smallFont.GetWidth(name) - 2.0), (float)(vec2.y + (double)position2.y - 3.0));
                                    else
                                        name += "_";
                                }
                                this._smallFont.Draw(name, Maths.RoundToPixel(new Vec2(position2.x, position2.y) + vec2), Colors.MenuOption * (controlSetting == this._selectedSetting ? 1f : 0.6f), (Depth)0.95f);
                            }
                        }
                        if (!this._editControl)
                        {
                            this._font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                            this._font.Draw("@START@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                        }
                        else
                            this._font.Draw("@START@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                        this.position = position1;
                        this._selector.screen.EndDraw();
                    }
                }
                else if (this._mode == PSMode.SelectProfile)
                {
                    this._pendingMaps.Clear();
                    Vec2 position = this.position;
                    this.position = Vec2.Zero;
                    this._selector.screen.BeginDraw();
                    string text1 = "@LWING@PICK PROFILE@RWING@";
                    this._font.Draw(text1, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text1) / 2.0), this.y + 8f)), Color.White, (Depth)0.95f);
                    float num1 = 8f;
                    for (int index1 = 0; index1 < 7; ++index1)
                    {
                        int index = this.ProfileIndexAdd(this._selectorPosition, index1 - 3);
                        string text2 = "NEW PROFILE";
                        bool flag1 = true;
                        bool flag2 = false;
                        if (index != -1)
                        {
                            if (Profiles.IsDefault(this._profiles[index]))
                            {
                                text2 = "DEFAULT";
                                flag2 = true;
                            }
                            else
                                text2 = this._profiles[index].name;
                            flag1 = false;
                            if (this._profiles[index] == Profiles.experienceProfile)
                                text2 = "@RAINBOWICON@|DGBLUE|" + text2 + "|WHITE|";
                            else if (this._profiles[index].steamID != 0UL)
                                text2 = "@STEAMICON@|DGBLUE|" + text2 + "|WHITE|";
                        }
                        string text3 = null;
                        if (this._desiredSelectorPosition == index && (index1 == 3 || _slideTo > 0.0 && index1 == 4 || _slideTo < 0.0 && index1 == 2))
                            text3 = "> " + text2 + " <";
                        float num2 = (float)((double)this.y + (double)num1 + 33.0);
                        float y = (float)((double)this.y + (double)num1 + index1 * 11 + -(double)this._slide * 11.0);
                        float num3 = Maths.Clamp((float)((33.0 - (double)Math.Abs(y - num2)) / 33.0), 0f, 1f);
                        float num4 = num3 * Maths.NormalizeSection(num3, 0f, 0.9f);
                        float num5 = 0.2f;
                        float num6 = Maths.Clamp((double)num3 >= 0.300000011920929 ? ((double)num3 >= 0.800000011920929 ? Maths.NormalizeSection(num3, 0.8f, 1f) + num5 : num5) : Maths.NormalizeSection(num3, 0f, 0.3f) * num5, 0f, 1f);
                        bool flag3 = false;
                        if ((this._selector == null || !this._selector.isArcadeHatSelector) && index != -1 && (Profiles.active.Contains(this._profiles[index]) || Profiles.active.FirstOrDefault<Profile>(x => x.linkedProfile == this._profiles[index]) != null))
                            flag3 = true;
                        if (flag3)
                            text2 = text2.Replace("|DGBLUE|", "");
                        this._font.Draw(text2, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text2) / 2.0), y)), (flag3 ? Color.Red : (flag1 ? Color.Lime : (flag2 ? Colors.DGYellow : Colors.MenuOption))) * num6, (Depth)0.95f);
                        if (text3 != null)
                            this._font.Draw(text3, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text3) / 2.0), y)), Color.White, (Depth)0.92f);
                    }
                    float y1 = num1 + 32f;
                    Graphics.DrawRect(this.position + new Vec2(2f, y1), this.position + new Vec2(138f, y1 + 9f), new Color(30, 30, 30) * this._fade, (Depth)0.8f);
                    this._font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    this._font.Draw(this.HoveredProfileIsCustom() ? "@MENU2@" : "@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    this.position = position;
                    this._selector.screen.EndDraw();
                }
                else
                {
                    if (this._mode != PSMode.CreateProfile)
                        return;
                    Vec2 position = this.position;
                    this.position = Vec2.Zero;
                    this._selector.screen.BeginDraw();
                    string str1 = "NONAME";
                    if (this._name != "")
                    {
                        str1 = this._name;// ParentalControls.RunProfanityCheck(ref this._name) > 0  
                    }
                    Vec2 pos = new Vec2(this.x + 36f, this.y + 8f);
                    if (!this.isEditing)
                    {
                        if (this._changeName)
                        {
                            pos.x -= 2f;
                            for (int index = 0; index < 9; ++index)
                            {
                                Graphics.DrawRect(pos + new Vec2(index * 8, 0f), pos + new Vec2(index * 8 + 7, 7f), new Color(60, 60, 60), (Depth)0.8f);
                                if (index == this._currentLetter)
                                {
                                    this._spinnerArrows.frame = 0;
                                    Vec2 vec2_1 = pos + new Vec2(index * 8, -6f);
                                    Graphics.Draw(_spinnerArrows, vec2_1.x, vec2_1.y, (Depth)0.95f);
                                    this._spinnerArrows.frame = 1;
                                    Vec2 vec2_2 = pos + new Vec2(index * 8, 9f);
                                    Graphics.Draw(_spinnerArrows, vec2_2.x, vec2_2.y, (Depth)0.95f);
                                    Graphics.DrawRect(pos + new Vec2(index * 8 - 2, -2f), pos + new Vec2(index * 8 + 9, 9f), Color.White * 0.8f, (Depth)0.97f, false);
                                }
                            }
                            this._font.Draw(str1, Maths.RoundToPixel(pos), Color.Lime * (this._createSelection == PSCreateSelection.ChangeName ? 1f : 0.6f), (Depth)0.95f);
                            pos.x += 2f;
                            string text4 = ">              <";
                            this._font.Draw(text4, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text4) / 2.0), pos.y)), Color.White * (this._createSelection == PSCreateSelection.ChangeName ? 1f : 0.6f), (Depth)0.95f);
                            if (_takenFlash > 0.0500000007450581)
                            {
                                string text5 = "Name Taken";
                                this._font.Draw(text5, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text5) / 2.0), pos.y)), Color.Red * this._takenFlash, (Depth)0.97f);
                                Graphics.DrawRect(new Vec2(this.x + 20f, pos.y), new Vec2((float)((double)this.x + (double)this.width - 20.0), pos.y + 8f), Color.Black, (Depth)0.96f);
                            }
                        }
                        else
                        {
                            string str2 = str1.Replace(" ", "");
                            string text = this._createSelection != PSCreateSelection.ChangeName ? "@LWING@" + str2.Reduced(12) + "@RWING@" : "> " + str2.Reduced(12) + " <";
                            this._font.Draw(text, Maths.RoundToPixel(new Vec2((float)((double)this.x + 2.0 + (double)this.width / 2.0 - (double)this._font.GetWidth(text) / 2.0), pos.y)), Color.White * (this._createSelection == PSCreateSelection.ChangeName ? 1f : 0.6f), (Depth)0.95f);
                        }
                    }
                    else
                    {
                        string text = "@LWING@" + str1.Reduced(12) + "@RWING@";
                        this._font.Draw(text, Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text) / 2.0), this.y + 8f)), Color.White * (1f - Math.Min(1f, this._takenFlash * 2f)), (Depth)0.95f);
                    }
                    pos.y += 14f;
                    string text6 = "            ";
                    if (this._createSelection == PSCreateSelection.Mood)
                        text6 = "< " + text6 + " >";
                    this._font.Draw(text6, (float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text6) / 2.0), pos.y, Color.White * (this._createSelection == PSCreateSelection.Mood ? 1f : 0.6f), (Depth)0.95f);
                    Graphics.DrawLine(new Vec2((float)((double)this.x + (double)this.width / 4.0 + 4.0), pos.y + 5f), new Vec2(this.x + (float)((double)this.width / 4.0 * 3.0), pos.y + 5f), Colors.MenuOption * (this._createSelection == PSCreateSelection.Mood ? 1f : 0.6f), 2f, (Depth)0.95f);
                    float num = 60f;
                    Graphics.DrawLine(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)num / 2.0 + (double)num * _moodVal + 2.0), pos.y + 1f), new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)num / 2.0 + (double)num * _moodVal + 2.0), pos.y + 4f), Colors.MenuOption * (this._createSelection == PSCreateSelection.Mood ? 1f : 0.6f), 3f, (Depth)0.95f);
                    Graphics.DrawLine(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)num / 2.0 + (double)num * _moodVal + 2.0), pos.y + 6f), new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)num / 2.0 + (double)num * _moodVal + 2.0), pos.y + 9f), Colors.MenuOption * (this._createSelection == PSCreateSelection.Mood ? 1f : 0.6f), 3f, (Depth)0.95f);
                    this._happyIcons.color = Color.White * (this._createSelection == PSCreateSelection.Mood ? 1f : 0.6f);
                    this._happyIcons.alpha = this._fade;
                    this._happyIcons.frame = (int)Math.Round(_moodVal * 4.0);
                    this._happyIcons.depth = (Depth)0.95f;
                    Graphics.Draw(_happyIcons, (float)((double)this.x + (double)this.width / 6.0 + 2.0), pos.y + 4f);
                    this._angryIcons.color = Color.White * (this._createSelection == PSCreateSelection.Mood ? 1f : 0.6f);
                    this._angryIcons.alpha = this._fade;
                    this._angryIcons.frame = (int)Math.Round((1.0 - _moodVal) * 4.0);
                    this._angryIcons.depth = (Depth)0.95f;
                    Graphics.Draw(_angryIcons, this.x + (float)((double)this.width / 6.0 * 5.0), pos.y + 4f);
                    pos.y += 16f;
                    string text7 = this._preferredColor >= 0 ? "COLOR" : "NO COLOR";
                    if (this._createSelection == PSCreateSelection.Color)
                        text7 = "< " + text7 + " >";
                    if (this._preferredColor >= 0)
                    {
                        Graphics.DrawRect(new Vec2(this.x + 20f, pos.y - 2f), new Vec2(this.x + (this.width - 20f), pos.y + 9f), Persona.all.ElementAt<DuckPersona>(this._preferredColor).colorDark.ToColor() * (this._createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.93f, false);
                        this._font.Draw(text7, Maths.RoundToPixel(new Vec2((float)((double)this.x + 2.0 + (double)this.width / 2.0 - (double)this._font.GetWidth(text7) / 2.0), pos.y)), Persona.all.ElementAt<DuckPersona>(this._preferredColor).color.ToColor() * (this._createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.95f);
                    }
                    else
                    {
                        Graphics.DrawRect(new Vec2(this.x + 20f, pos.y - 2f), new Vec2(this.x + (this.width - 20f), pos.y + 9f), Colors.BlueGray * (this._createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.93f, false);
                        this._font.Draw(text7, Maths.RoundToPixel(new Vec2((float)((double)this.x + 2.0 + (double)this.width / 2.0 - (double)this._font.GetWidth(text7) / 2.0), pos.y)), Colors.BlueGray * (this._createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.95f);
                    }
                    pos.y += 12f;
                    string text8 = "CONTROLS";
                    if (this._createSelection == PSCreateSelection.Controls)
                        text8 = "> " + text8 + " <";
                    this._font.Draw(text8, Maths.RoundToPixel(new Vec2((float)((double)this.x + 2.0 + (double)this.width / 2.0 - (double)this._font.GetWidth(text8) / 2.0), pos.y)), Colors.MenuOption * (this._createSelection == PSCreateSelection.Controls ? 1f : 0.6f), (Depth)0.95f);
                    string text9 = "OK";
                    if (this._createSelection == PSCreateSelection.Accept)
                        text9 = "> " + text9 + " <";
                    pos.y += 12f;
                    this._font.Draw(text9, Maths.RoundToPixel(new Vec2((float)((double)this.x + 2.0 + (double)this.width / 2.0 - (double)this._font.GetWidth(text9) / 2.0), pos.y)), Colors.MenuOption * (this._createSelection == PSCreateSelection.Accept ? 1f : 0.6f), (Depth)0.95f);
                    if (this._changeName)
                    {
                        string text10 = "@DPAD@";
                        if (this._selector != null && (this._selector.profileBoxNumber == 0 || this._selector.profileBoxNumber == 2))
                            text10 = "@WASD@";
                        this._font.Draw(text10, 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                        this._font.Draw("@SELECT@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    }
                    else if (this._createSelection == PSCreateSelection.ChangeName)
                    {
                        this._font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                        this._font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    }
                    else if (this._createSelection == PSCreateSelection.Mood)
                    {
                        string text11 = "@DPAD@";
                        if (this._selector != null && (this._selector.profileBoxNumber == 0 || this._selector.profileBoxNumber == 2))
                            text11 = "@WASD@";
                        this._font.Draw(text11, 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                        this._font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    }
                    else if (this._createSelection == PSCreateSelection.Color)
                    {
                        string text12 = "@DPAD@";
                        if (this._selector != null && (this._selector.profileBoxNumber == 0 || this._selector.profileBoxNumber == 2))
                            text12 = "@WASD@";
                        this._font.Draw(text12, 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                        this._font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    }
                    else
                    {
                        this._font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                        this._font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this._inputProfile);
                    }
                    this.position = position;
                    this._selector.screen.EndDraw();
                }
            }
        }

        public void Open(Profile p)
        {
            this._desiredSelectorPosition = this._selectorPosition = 0;
            if (this._box == null && Level.current is TeamSelect2)
                this._box = (Level.current as TeamSelect2).GetBox(p.networkIndex);
            if (this._box == null)
                return;
            this.isEditing = false;
            this._inputProfile = p.inputProfile;
            this._profile = this._starterProfile = p;
            this.RebuildProfileList();
            for (int index = 0; index < this._profiles.Count; ++index)
            {
                if (this._profiles[index] == this._profile)
                {
                    this._selectorPosition = index;
                    break;
                }
            }
            this._desiredSelectorPosition = this._selectorPosition;
            this._open = true;
            this._fade = 1f;
        }

        private bool ProfileAlreadySelected(Profile p) => this._profile.linkedProfile != null ? p != null && Profiles.active.FirstOrDefault<Profile>(x => x.linkedProfile == p) != null && p != this._profile.linkedProfile : p != null && Profiles.active.Contains(p) && p != this._profile;

        public void EditProfile(Profile p)
        {
            this.Open(p);
            this.isEditing = true;
            this._mode = PSMode.EditProfile;
            this._desiredMode = PSMode.EditProfile;
            this._name = p.name;
            this._desiredMode = PSMode.CreateProfile;
            this._changeName = false;
            this._currentLetter = 0;
            this._moodVal = p.funslider;
            this._preferredColor = p.preferredColor;
            this._createSelection = PSCreateSelection.Accept;
            this._configInputMapping = Input.GetDefaultMapping(this._inputProfile.lastActiveDevice.productName, this._inputProfile.lastActiveDevice.productGUID, p: (this.isEditing ? this._profile : null)).Clone();
        }

        private int ProfileIndexAdd(int index, int plus)
        {
            if (this._profiles.Count == 0)
                return -1;
            int num = index + plus;
            while (num >= this._profiles.Count)
                num -= this._profiles.Count + 1;
            while (num < -1)
                num += this._profiles.Count + 1;
            return num;
        }

        public override void Draw()
        {
            if (_fade < 0.01f)
                return;
            if (this._mode == PSMode.EditControlsConfirm)
            {
                this._selector.firstWord = "OK";
                this._selector.secondWord = "BACK";
            }
            else if (this._mode == PSMode.CreateProfile)
            {
                if (this._changeName)
                {
                    this._selector.firstWord = "MOVE";
                    this._selector.secondWord = "OK";
                }
                else if (this._createSelection == PSCreateSelection.ChangeName)
                {
                    this._selector.firstWord = "ALTER";
                    this._selector.secondWord = "BACK";
                }
                else if (this._createSelection == PSCreateSelection.Mood)
                {
                    this._selector.firstWord = "MOVE";
                    this._selector.secondWord = "BACK";
                }
                else
                {
                    this._selector.firstWord = "OK";
                    this._selector.secondWord = "BACK";
                }
            }
            else if (this._mode == PSMode.SelectProfile)
            {
                if (!this.HoveredProfileIsCustom())
                {
                    this._selector.firstWord = "PICK";
                    this._selector.secondWord = "BACK";
                }
                else
                {
                    this._selector.firstWord = "PICK";
                    this._selector.secondWord = "KILL";
                }
            }
            else if (!this._editControl)
            {
                this._selector.firstWord = "EDIT";
                this._selector.secondWord = "SAVE";
            }
            else
            {
                this._selector.firstWord = "BACK";
                this._selector.secondWord = "";
            }
        }
    }
}
