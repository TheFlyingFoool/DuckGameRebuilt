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
              trigger = Triggers.Left,
              position = new Vec2(0f, 0f),
              column = 0,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = "/",
              trigger = Triggers.Right,
              position = new Vec2(35f, 0f),
              column = 0,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = "}",
              trigger = Triggers.Up,
              position = new Vec2(70f, 0f),
              column = 1,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = "~",
              trigger = Triggers.Down,
              position = new Vec2(105f, 0f),
              column = 1,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = "JUMP  ",
              trigger = Triggers.Jump,
              position = new Vec2(0f, 12f),
              column = 0
            },
            new ControlSetting()
            {
              name = "GRAB  ",
              trigger = Triggers.Grab,
              position = new Vec2(0f, 24f),
              column = 0
            },
            new ControlSetting()
            {
              name = Triggers.Strafe,
              trigger = Triggers.Strafe,
              position = new Vec2(0f, 36f),
              column = 0
            },
            new ControlSetting()
            {
              name = "USE   ",
              trigger = Triggers.Shoot,
              position = new Vec2(70f, 12f),
              column = 1
            },
            new ControlSetting()
            {
              name = "QUACK ",
              trigger = Triggers.Quack,
              position = new Vec2(70f, 24f),
              column = 1
            },
            new ControlSetting()
            {
              name = "FALL  ",
              trigger = Triggers.Ragdoll,
              position = new Vec2(70f, 36f),
              column = 1
            },
            new ControlSetting()
            {
              name = "START ",
              trigger = Triggers.Start,
              position = new Vec2(0f, 48f),
              column = 0,
              condition =  x => x.allowStartRemap
            },
            new ControlSetting()
            {
              name = "PAGE 2>",
              trigger = Triggers.Any,
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
              trigger = Triggers.Any,
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
              trigger = Triggers.LeftStick,
              position = new Vec2(0f, 12f),
              column = 0,
              condition =  x => x.numSticks > 1
            },
            new ControlSetting()
            {
              name = "PITCH ",
              trigger = Triggers.LeftTrigger,
              position = new Vec2(0f, 24f),
              column = 0,
              condition =  x => x.numTriggers > 1
            },
            new ControlSetting()
            {
              name = "LICK  ",
              trigger = Triggers.RightStick,
              position = new Vec2(70f, 12f),
              column = 1,
              condition =  x => x.numSticks > 1
            },
            new ControlSetting()
            {
              name = "ZOOM  ",
              trigger = Triggers.RightTrigger,
              position = new Vec2(70f, 24f),
              column = 1,
              condition =  x => x.numTriggers > 1
            },
            new ControlSetting()
            {
              name = "<PAGE 1",
              trigger = Triggers.Any,
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
              trigger = Triggers.Any,
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
              trigger = Triggers.Any,
              position = new Vec2(0f, 0f),
              column = 0,
              caption = true
            },
            new ControlSetting()
            {
              name = "{",
              trigger = Triggers.MenuLeft,
              position = new Vec2(0f, 12f),
              column = 0,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = "/",
              trigger = Triggers.MenuRight,
              position = new Vec2(35f, 12f),
              column = 0,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = "}",
              trigger = Triggers.MenuUp,
              position = new Vec2(70f, 12f),
              column = 1,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = "~",
              trigger = Triggers.MenuDown,
              position = new Vec2(105f, 12f),
              column = 1,
              condition =  x => x.allowDirectionalMapping
            },
            new ControlSetting()
            {
              name = Triggers.Select,
              trigger = Triggers.Select,
              position = new Vec2(0f, 24f),
              column = 0
            },
            new ControlSetting()
            {
              name = "MENU 1",
              trigger = Triggers.Menu1,
              position = new Vec2(0f, 36f),
              column = 0
            },
            new ControlSetting()
            {
              name = Triggers.Cancel,
              trigger = Triggers.Cancel,
              position = new Vec2(70f, 24f),
              column = 1
            },
            new ControlSetting()
            {
              name = "MENU 2",
              trigger = Triggers.Menu2,
              position = new Vec2(70f, 36f),
              column = 1
            },
            new ControlSetting()
            {
              name = "<PAGE 2",
              trigger = Triggers.Any,
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
              trigger = Triggers.Any,
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
              trigger = Triggers.Any,
              position = new Vec2(70f, 48f),
              column = 1,
              action =  x =>
              {
                _madeControlChanges = true;
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

        public int preferredColor => _preferredColor;
        public float fade => _fade;

        public bool open => _open;

        public ProfileSelector(float xpos, float ypos, ProfileBox2 box, HatSelector sel)
          : base(xpos, ypos)
        {
            _font = new BitmapFont("biosFontUI", 8, 7)
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            _collisionSize = new Vec2(141f, 89f);
            _spinnerArrows = new SpriteMap("spinnerArrows", 8, 4);
            _box = box;
            _selector = sel;
            _happyIcons = new SpriteMap("happyFace", 16, 16);
            _happyIcons.CenterOrigin();
            _angryIcons = new SpriteMap("angryFace", 16, 16);
            _angryIcons.CenterOrigin();
            _smallFont = new BitmapFont("smallBiosFont", 7, 6)
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            _controlsFont = new BitmapFont("biosFontUIArrows", 8, 7)
            {
                scale = new Vec2(1f)
            };
            if (sel.profile != null)
            {
                _preferredColor = sel.profile.preferredColor;
            }
            shouldbegraphicculled = false;
        }

        public override void Initialize()
        {
            _confirmMenu = new UIMenu("DELETE PROFILE!?", 320f / 2f, 180f / 2f, 160f, conString: "@SELECT@SELECT @CANCEL@OH NO!");
            _confirmMenu.Add(new UIMenuItem("WHAT? NO!", new UIMenuActionCloseMenu(_confirmMenu), backButton: true), true);
            _confirmMenu.Add(new UIMenuItem("YEAH!", new UIMenuActionCloseMenuSetBoolean(_confirmMenu, _deleteProfile)), true);
            _confirmMenu.Close();
            Level.Add(_confirmMenu);
            base.Initialize();
        }

        public void Reset()
        {
            _open = false;
            _selector.fade = 1f;
            _fade = 0f;
            _desiredMode = PSMode.SelectProfile;
            _controlPage = 0;
            _selectedSetting = null;
            _configInputMapping = null;
        }

        public string GetMaskName(int length)
        {
            string maskName = "";
            for (int index = 0; index < 9; ++index)
                maskName = index >= length ? maskName + " " : maskName + _maskName[index].ToString();
            return maskName;
        }

        public void SelectDown()
        {
            if (_desiredSelectorPosition >= _profiles.Count - 1)
                _desiredSelectorPosition = -1;
            else
                ++_desiredSelectorPosition;
            _slideTo = 1f;
        }

        public void SelectUp()
        {
            if (_desiredSelectorPosition <= -1)
                _desiredSelectorPosition = _profiles.Count - 1;
            else
                --_desiredSelectorPosition;
            _slideTo = -1f;
        }

        public int GetCharIndex(char c)
        {
            for (int index = 0; index < _characters.Count; ++index)
            {
                if (_characters[index] == c)
                    return index;
            }
            return -1;
        }

        private void ApplyInputSettings(Profile p)
        {
            if (p == null)
                return;
            if (_pendingMaps.Count > 0)
            {
                p.inputMappingOverrides.Clear();
                foreach (DeviceInputMapping pendingMap in _pendingMaps)
                    Input.SetDefaultMapping(pendingMap, p);
            }
            p.inputProfile = _inputProfile;
            _pendingMaps.Clear();
            Input.ApplyDefaultMapping(_inputProfile, _profile);
        }

        private void RebuildProfileList()
        {
            _profiles = Profiles.allCustomProfiles;
            if (_box.controllerIndex != 0 || !Options.Data.defaultAccountMerged) _profiles.Add(Profiles.universalProfileList.ElementAt(_box.controllerIndex));
            if (Network.isActive) _profiles.Remove(Profiles.experienceProfile);
        }

        private void SaveSettings(bool pIsEditing, bool pAccepted)
        {
            if (!pIsEditing && !pAccepted)
                return;
            string varName = _name.Replace(" ", "");
            Profile p = _profile;
            if (!pIsEditing)
                p = new Profile(varName);
            p.funslider = _moodVal;
            p.preferredColor = _preferredColor;
            p.UpdatePersona();
            ApplyInputSettings(p);
            if (!pIsEditing)
                Profiles.Add(p);
            if (!pIsEditing)
            {
                RebuildProfileList();
                for (int index = 0; index < _profiles.Count; ++index)
                {
                    if (_profiles[index].name == varName)
                    {
                        _selectorPosition = index;
                        _desiredSelectorPosition = _selectorPosition;
                        break;
                    }
                }
                _desiredMode = PSMode.SelectProfile;
                _autoSelect = true;
            }
            else
            {
                Profiles.Save(_profile);
                _desiredMode = PSMode.SelectProfile;
                _mode = PSMode.SelectProfile;
                _open = false;
                _selector.fade = 1f;
                _fade = 0f;
                _selector.screen.DoFlashTransition();
            }
            SFX.Play("consoleSelect", 0.4f);
        }

        private bool HoveredProfileIsCustom() => _selectorPosition != -1 && hoveredProfile.steamID == 0UL && Profiles.experienceProfile != hoveredProfile && !Profiles.IsDefault(hoveredProfile);

        private Profile hoveredProfile => _selectorPosition >= 0 && _selectorPosition < _profiles.Count ? _profiles[_selectorPosition] : Profiles.DefaultPlayer1;

        public override void Update()
        {
            if (_selector.screen.transitioning)
                return;
            _takenFlash = Lerp.Float(_takenFlash, 0f, 0.02f);
            if (!_open)
            {
                if (_fade >= 0.01f || !_closing)
                    return;
                _closing = false;
            }
            else if (_configInputMapping != null && _inputProfile != null && _configInputMapping.device.productName + _configInputMapping.device.productGUID != _inputProfile.lastActiveDevice.productName + _inputProfile.lastActiveDevice.productGUID)
            {
                _open = false;
                _selector.fade = 1f;
                _fade = 0f;
                _selector.screen.DoFlashTransition();
                _desiredMode = PSMode.SelectProfile;
                SFX.Play("consoleCancel", 0.4f);
            }
            else
            {
                if (_mode != _desiredMode)
                {
                    _selector.screen.DoFlashTransition();
                    _mode = _desiredMode;
                }
                if (_fade > 0.9f && _mode != PSMode.CreateProfile && _mode != PSMode.EditProfile && _mode != PSMode.EditControls && _mode != PSMode.EditControlsConfirm && _desiredSelectorPosition == _selectorPosition)
                {
                    if (_inputProfile.Down(Triggers.MenuUp))
                    {
                        SelectUp();
                        _wasDown = false;
                        if (_profiles.Count > 0)
                            SFX.Play("consoleTick");
                    }
                    if (_inputProfile.Down(Triggers.MenuDown))
                    {
                        SelectDown();
                        _wasDown = true;
                        if (_profiles.Count > 0)
                            SFX.Play("consoleTick");
                    }
                    if (HoveredProfileIsCustom() && MonoMain.pauseMenu == null && _inputProfile.Pressed(Triggers.Menu2))
                    {
                        _deleteContext = _profiles[_selectorPosition];
                        MonoMain.pauseMenu = _confirmMenu;
                        _confirmMenu.Open();
                        SFX.Play("pause", 0.6f);
                    }
                    if (_deleteProfile.value)
                    {
                        _deleteProfile.value = false;
                        if (_deleteContext != null)
                        {
                            Profiles.Delete(_deleteContext);
                            SelectUp();
                            RebuildProfileList();
                            _slide = _slideTo;
                            _deleteContext = null;

                            _box.ChangeProfile(_profiles[_selectorPosition]);
                            _profile = _profiles[_selectorPosition];
                            _profile.inputProfile = null;
                            _profile.inputProfile = _inputProfile;
                            Input.ApplyDefaultMapping(_inputProfile, _profile);
                            _selector.ConfirmProfile();
                            _open = false;
                            _selector.fade = 1f;
                            _fade = 0f;
                            _selector.screen.DoFlashTransition();
                        }
                    }
                    if (_inputProfile.Pressed(Triggers.Cancel))
                    {
                        if (Profiles.IsDefault(_starterProfile) || !(Level.current is TeamSelect2))
                            _box.ChangeProfile(_starterProfile);
                        _open = false;
                        _selector.fade = 1f;
                        _fade = 0f;
                        _selector.screen.DoFlashTransition();
                        SFX.Play("consoleCancel", 0.4f);
                        return;
                    }
                    if (_inputProfile.Pressed(Triggers.Select) || _autoSelect)
                    {
                        _autoSelect = false;
                        if (_profiles.Count == 0 || _selectorPosition < -1 || _selectorPosition >= _profiles.Count)
                        {
                            _selectorPosition = -1;
                        }
                        if (_selectorPosition == -1)
                        {
                            _desiredMode = PSMode.CreateProfile;
                            _changeName = true;
                            _currentLetter = 0;
                            _createSelection = PSCreateSelection.ChangeName;
                            _maskName = "aaaaaaaaa";
                            _name = GetMaskName(1);
                            SFX.Play("consoleSelect", 0.4f);
                        }
                        else if (ProfileAlreadySelected(_profiles[_selectorPosition]))
                        {
                            SFX.Play("consoleError");
                        }
                        else
                        {
                            if (_profiles[_selectorPosition].linkedProfile == null)
                            {
                                if (Network.isActive)
                                {
                                    _profile.linkedProfile = _profiles[_selectorPosition];
                                    Input.ApplyDefaultMapping(_inputProfile, _profile);
                                    _profile.UpdatePersona();
                                }
                                else if (_selectorPosition != -1)
                                {
                                    _box.ChangeProfile(_profiles[_selectorPosition]);
                                    _profile = _profiles[_selectorPosition];
                                    _profile.inputProfile = null;
                                    _profile.inputProfile = _inputProfile;
                                    Input.ApplyDefaultMapping(_inputProfile, _profile);
                                }
                            }
                            _selector.ConfirmProfile();
                            _open = false;
                            _selector.fade = 1f;
                            _fade = 0f;
                            _selector.screen.DoFlashTransition();
                            SFX.Play("consoleSelect", 0.4f);
                        }
                    }
                }
                else if (_mode == PSMode.EditControlsConfirm)
                {
                    if (_inputProfile.Pressed(Triggers.MenuUp))
                    {
                        SFX.Play("consoleTick");
                        --_editControlSelection;
                    }
                    else if (_inputProfile.Pressed(Triggers.MenuDown))
                    {
                        SFX.Play("consoleTick");
                        ++_editControlSelection;
                    }
                    else
                    {
                        if (_inputProfile.Pressed(Triggers.Cancel))
                        {
                            _desiredMode = PSMode.EditControls;
                            SFX.Play("consoleError");
                            return;
                        }
                        if (_inputProfile.Pressed(Triggers.Select))
                        {
                            SFX.Play("consoleSelect");
                            if (_editControlSelection == 0)
                            {
                                _pendingMaps.Add(_configInputMapping);
                                ApplyInputSettings(_profile);
                            }
                            else
                                _configInputMapping = Input.GetDefaultMapping(_inputProfile.lastActiveDevice.productName, _inputProfile.lastActiveDevice.productGUID, p: (isEditing ? _profile : null)).Clone();
                            _desiredMode = PSMode.CreateProfile;
                        }
                    }
                    if (_editControlSelection > 1)
                        _editControlSelection = 1;
                    if (_editControlSelection < 0)
                        _editControlSelection = 0;
                }
                else if (_mode == PSMode.EditControls)
                {
                    if (!_editControl)
                    {
                        InputDevice d = _inputProfile.lastActiveDevice;
                        if (d is GenericController)
                            d = (d as GenericController).device;
                        if (_selectedSetting == null)
                            _selectedSetting = _controlSettingPages[_controlPage].Find(x => (x.condition == null || x.condition(d)) && !x.caption);
                        Vec2 zero = Vec2.Zero;
                        if (_inputProfile.Pressed(Triggers.MenuUp))
                            zero += new Vec2(0f, -8f);
                        else if (_inputProfile.Pressed(Triggers.MenuDown))
                            zero += new Vec2(0f, 8f);
                        else if (_inputProfile.Pressed(Triggers.MenuLeft))
                            zero += new Vec2(-30f, 0f);
                        else if (_inputProfile.Pressed(Triggers.MenuRight))
                            zero += new Vec2(30f, 0f);
                        if (zero != Vec2.Zero)
                        {
                            ControlSetting controlSetting1 = null;
                            foreach (ControlSetting controlSetting2 in _controlSettingPages[_controlPage])
                            {
                                if ((controlSetting2.condition == null || controlSetting2.condition(d)) && !controlSetting2.caption)
                                {
                                    if (zero.x != 0)
                                    {
                                        if (controlSetting2.position.y == _selectedSetting.position.y)
                                        {
                                            if (zero.x > 0)
                                            {
                                                if (controlSetting2.position.x > _selectedSetting.position.x && (controlSetting1 == null || controlSetting2.position.x < controlSetting1.position.x))
                                                    controlSetting1 = controlSetting2;
                                            }
                                            else if (controlSetting2.position.x < _selectedSetting.position.x && (controlSetting1 == null || controlSetting2.position.x > controlSetting1.position.x))
                                                controlSetting1 = controlSetting2;
                                        }
                                    }
                                    else if (controlSetting2.position.x == _selectedSetting.position.x || controlSetting2.column == _selectedSetting.column)
                                    {
                                        if (zero.y > 0)
                                        {
                                            if (controlSetting2.position.y > _selectedSetting.position.y && (controlSetting1 == null || controlSetting2.position.y < controlSetting1.position.y))
                                                controlSetting1 = controlSetting2;
                                        }
                                        else if (controlSetting2.position.y < _selectedSetting.position.y && (controlSetting1 == null || controlSetting2.position.y > controlSetting1.position.y))
                                            controlSetting1 = controlSetting2;
                                    }
                                }
                            }
                            if (controlSetting1 != null)
                                _selectedSetting = controlSetting1;
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.Select))
                        {
                            if (_selectedSetting.action != null)
                            {
                                _selectedSetting.action(this);
                            }
                            else
                            {
                                _editControl = true;
                                SFX.Play("consoleTick");
                            }
                        }
                        else
                        {
                            if (_inputProfile.Pressed(Triggers.Cancel))
                            {
                                if (_madeControlChanges)
                                {
                                    _editControlSelection = 0;
                                    _desiredMode = PSMode.EditControlsConfirm;
                                    SFX.Play("consoleError");
                                    return;
                                }
                                _desiredMode = PSMode.CreateProfile;
                                SFX.Play("consoleError");
                                return;
                            }
                            if (_inputProfile.Pressed(Triggers.Start))
                            {
                                _pendingMaps.Add(_configInputMapping);
                                ApplyInputSettings(_profile);
                                _desiredMode = PSMode.CreateProfile;
                                SFX.Play("consoleSelect");
                                return;
                            }
                        }
                    }
                    else if (_inputProfile.Pressed(Triggers.Start))
                    {
                        _editControl = false;
                        SFX.Play("consoleError");
                    }
                    else
                    {
                        _configInputMapping.deviceOverride = _inputProfile.lastActiveDevice;
                        if (_configInputMapping.deviceOverride is GenericController)
                            _configInputMapping.deviceOverride = (_configInputMapping.deviceOverride as GenericController).device;
                        if (_selectedSetting.trigger != Triggers.Any && _configInputMapping.RunMappingUpdate(_selectedSetting.trigger, false))
                        {
                            _editControl = false;
                            SFX.Play("consoleSelect");
                            _madeControlChanges = true;
                            _configInputMapping.deviceOverride = null;
                            return;
                        }
                        _configInputMapping.deviceOverride = null;
                    }
                }
                else if (_mode == PSMode.CreateProfile)
                {
                    if (!_changeName)
                    {
                        if (_createSelection == PSCreateSelection.Controls && _inputProfile.Pressed(Triggers.Select))
                        {
                            _desiredMode = PSMode.EditControls;
                            _selectedSetting = null;
                            _controlPage = 0;
                            _madeControlChanges = false;
                            if (_configInputMapping == null)
                                _configInputMapping = Input.GetDefaultMapping(_inputProfile.lastActiveDevice.productName, _inputProfile.lastActiveDevice.productGUID, p: (isEditing ? _profile : null)).Clone();
                            SFX.Play("consoleTick");
                        }
                        if (_createSelection == PSCreateSelection.Mood)
                        {
                            if (_inputProfile.Pressed(Triggers.MenuLeft))
                            {
                                _moodVal = Maths.Clamp(_moodVal - 0.25f, 0f, 1f);
                                SFX.Play("consoleTick");
                            }
                            if (_inputProfile.Pressed(Triggers.MenuRight))
                            {
                                _moodVal = Maths.Clamp(_moodVal + 0.25f, 0f, 1f);
                                SFX.Play("consoleTick");
                            }
                        }
                        if (_createSelection == PSCreateSelection.Color)
                        {
                            if (_inputProfile.Pressed(Triggers.MenuLeft))
                            {
                                if (_preferredColor == -1)
                                {
                                    _preferredColor = DG.MaxPlayers;
                                }
                                _preferredColor = Maths.Clamp(_preferredColor - 1, -1, DG.MaxPlayers - 1);
                                SFX.Play("consoleTick");
                            }
                            if (_inputProfile.Pressed(Triggers.MenuRight))
                            {
                                if (_preferredColor == DG.MaxPlayers - 1)
                                {
                                    _preferredColor = -2;
                                }
                                _preferredColor = Maths.Clamp(_preferredColor + 1, -1, DG.MaxPlayers - 1);
                                SFX.Play("consoleTick");
                            }
                        }
                        if (_inputProfile.Pressed(Triggers.MenuDown) && _name != "" && _createSelection < PSCreateSelection.Accept)
                        {
                            ++_createSelection;
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.MenuUp) && _name != "" && _createSelection > (isEditing ? PSCreateSelection.Mood : PSCreateSelection.ChangeName))
                        {
                            --_createSelection;
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.Select))
                        {
                            if (_createSelection == PSCreateSelection.ChangeName)
                            {
                                if (!isEditing)
                                {
                                    _changeName = true;
                                    if (_name == "")
                                        _name = GetMaskName(1);
                                    SFX.Play("consoleSelect", 0.4f);
                                }
                                else
                                    SFX.Play("consoleError", 0.8f);
                            }
                            else if (_createSelection == PSCreateSelection.Accept)
                            {
                                SaveSettings(isEditing, true);
                                SFX.Play("consoleSelect", 0.4f);
                            }
                        }
                        if (_inputProfile.Pressed(Triggers.Cancel))
                        {
                            SaveSettings(isEditing, false);
                            if (!isEditing)
                            {
                                _desiredMode = PSMode.SelectProfile;
                            }
                            else
                            {
                                _desiredMode = PSMode.SelectProfile;
                                _mode = PSMode.SelectProfile;
                                _open = false;
                                _selector.fade = 1f;
                                _fade = 0f;
                                _selector.screen.DoFlashTransition();
                            }
                            SFX.Play("consoleCancel", 0.4f);
                        }
                    }
                    else
                    {
                        InputProfile.repeat = true;
                        Keyboard.repeat = true;
                        if (_inputProfile.Pressed(Triggers.Select))
                        {
                            string str = _name.Replace(" ", "");
                            if (str == "")
                            {
                                str = "duckis91";
                                _name = str + " ";
                                _currentLetter = 7;
                            }
                            List<Profile> allCustomProfiles = Profiles.allCustomProfiles;
                            bool flag = false;
                            if (_selector == null || !_selector.isArcadeHatSelector)
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
                                _takenFlash = 1f;
                            else
                                _changeName = false;
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.MenuLeft))
                        {
                            --_currentLetter;
                            if (_currentLetter < 0)
                            {
                                _currentLetter = 0;
                            }
                            else
                            {
                                _name = _name.Remove(_currentLetter + 1, 1);
                                _name = _name.Insert(_currentLetter + 1, " ");
                            }
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.MenuRight))
                        {
                            ++_currentLetter;
                            if (_currentLetter > 8)
                            {
                                _currentLetter = 8;
                            }
                            else
                            {
                                _name = _name.Remove(_currentLetter, 1);
                                if (_currentLetter > 0)
                                    _name = _name.Insert(_currentLetter, _name[_currentLetter - 1].ToString() ?? "");
                            }
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.MenuUp))
                        {
                            int index = GetCharIndex(_name[_currentLetter]) + 1;
                            if (index >= _characters.Count)
                                index = 0;
                            char character = _characters[index];
                            _name = _name.Remove(_currentLetter, 1);
                            _name = _name.Insert(_currentLetter, character.ToString() ?? "");
                            _maskName = _maskName.Remove(_currentLetter, 1);
                            _maskName = _maskName.Insert(_currentLetter, character.ToString() ?? "");
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.MenuDown))
                        {
                            int index = GetCharIndex(_name[_currentLetter]) - 1;
                            if (index < 0)
                                index = _characters.Count - 1;
                            char character = _characters[index];
                            _name = _name.Remove(_currentLetter, 1);
                            _name = _name.Insert(_currentLetter, character.ToString() ?? "");
                            _maskName = _maskName.Remove(_currentLetter, 1);
                            _maskName = _maskName.Insert(_currentLetter, character.ToString() ?? "");
                            SFX.Play("consoleTick");
                        }
                        if (_inputProfile.Pressed(Triggers.Cancel))
                        {
                            _desiredMode = PSMode.SelectProfile;
                            SFX.Play("consoleCancel", 0.4f);
                        }
                    }
                }
                if (_slideTo != 0 && _slide != _slideTo)
                    _slide = Lerp.Float(_slide, _slideTo, 0.1f);
                else if (_slideTo != 0 && _slide == _slideTo)
                {
                    _slide = 0f;
                    _slideTo = 0f;
                    if (_desiredSelectorPosition != -1 && ProfileAlreadySelected(_profiles[_desiredSelectorPosition]))
                    {
                        _selectorPosition = _desiredSelectorPosition;
                        if (_wasDown)
                            SelectDown();
                        else
                            SelectUp();
                    }
                    else
                    {
                        _selectorPosition = _desiredSelectorPosition;
                        if (!(Level.current is TeamSelect2))
                        {
                            if (_selectorPosition != -1)
                            {
                                _box.ChangeProfile(_profiles[_selectorPosition]);
                                _profile = _profiles[_selectorPosition];
                            }
                            else
                            {
                                _box.ChangeProfile(null);
                                _profile = _box.profile;
                            }
                        }
                    }
                }
                _font.alpha = _fade;
                _font.depth = (Depth)0.96f;
                _font.scale = new Vec2(1f, 1f);
                if (_mode == PSMode.EditControlsConfirm)
                {
                    Vec2 position = this.position;
                    this.position = Vec2.Zero;
                    _selector.screen.BeginDraw();
                    string text = "SAVE CHANGES?";
                    _smallFont.Draw(text, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _smallFont.GetWidth(text) / 2), y + 22f)), Colors.MenuOption * (_controlPosition == 0 ? 1f : 0.6f), (Depth)0.95f);
                    Vec2 vec2 = new Vec2((float)(x + width / 2 - 66), y + 18f) + new Vec2(0.5f, 0f);
                    _smallFont.Draw("YES", Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _smallFont.GetWidth("YES") / 2), y + 34f)), Colors.MenuOption * (_editControlSelection == 0 ? 1f : 0.6f), (Depth)0.95f);
                    _smallFont.Draw("NO", Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _smallFont.GetWidth("NO") / 2), (float)(y + 34 + 8))), Colors.MenuOption * (_editControlSelection == 1 ? 1f : 0.6f), (Depth)0.95f);
                    _font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    _font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    this.position = position;
                    _selector.screen.EndDraw();
                }
                else if (_mode == PSMode.EditControls)
                {
                    Vec2 position1 = position;
                    position = Vec2.Zero;
                    _selector.screen.BeginDraw();
                    InputProfile inputProfile = _inputProfile;
                    _smallFont.scale = new Vec2(1f, 1f);
                    float num = 6f;
                    string text = inputProfile.lastActiveDevice.productName;
                    if (text == null)
                    {
                        _desiredMode = PSMode.CreateProfile;
                        SFX.Play("consoleError");
                    }
                    else
                    {
                        if (text == "Joy-Con (L)" || text == "Joy-Con (R)")
                            text = "Joy-Con (L)/(R)";
                        if (text.Length > 15)
                            text = text.Substring(0, 15) + "...";
                        if (_controlPosition == 0)
                            text = "< " + text + " >";
                        _smallFont.Draw(text, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _smallFont.GetWidth(text) / 2), y + num)), Colors.MenuOption * (_controlPosition == 0 ? 1f : 0.6f), (Depth)0.95f);
                        Vec2 vec2 = new Vec2((float)(x + width / 2 - 66), (float)(y + num + 9)) + new Vec2(0.5f, 0f);
                        bool flag = false;
                        foreach (ControlSetting controlSetting in _controlSettingPages[_controlPage])
                        {
                            InputDevice inputDevice = _inputProfile.lastActiveDevice;
                            if (inputDevice is GenericController)
                                inputDevice = (inputDevice as GenericController).device;
                            if (controlSetting.condition == null || controlSetting.condition(inputDevice))
                            {
                                string name = controlSetting.name;
                                Vec2 position2 = controlSetting.position;
                                if (position2.y == 0)
                                    flag = true;
                                else if (!flag && (_controlPage != 0 || controlSetting != _controlSettingPages[_controlPage][_controlSettingPages[_controlPage].Count - 1]))
                                    position2.y -= 12f;
                                if (controlSetting.trigger != Triggers.Any)
                                {
                                    name += ":|DGBLUE|";
                                    if (!_editControl || _selectedSetting != controlSetting)
                                        Graphics.Draw(inputProfile.lastActiveDevice.GetMapImage(_configInputMapping.map[controlSetting.trigger]), (float)(vec2.x + position2.x + _smallFont.GetWidth(name) - 2), (float)(vec2.y + position2.y - 3));
                                    else
                                        name += "_";
                                }
                                _smallFont.Draw(name, Maths.RoundToPixel(new Vec2(position2.x, position2.y) + vec2), Colors.MenuOption * (controlSetting == _selectedSetting ? 1f : 0.6f), (Depth)0.95f);
                            }
                        }
                        if (!_editControl)
                        {
                            _font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                            _font.Draw("@START@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                        }
                        else
                            _font.Draw("@START@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                        position = position1;
                        _selector.screen.EndDraw();
                    }
                }
                else if (_mode == PSMode.SelectProfile)
                {
                    _pendingMaps.Clear();
                    Vec2 position = this.position;
                    this.position = Vec2.Zero;
                    _selector.screen.BeginDraw();
                    string text1 = "@LWING@PICK PROFILE@RWING@";
                    _font.Draw(text1, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _font.GetWidth(text1) / 2), y + 8f)), Color.White, (Depth)0.95f);
                    float num1 = 8f;
                    for (int index1 = 0; index1 < 7; ++index1)
                    {
                        int index = ProfileIndexAdd(_selectorPosition, index1 - 3);
                        string text2 = "NEW PROFILE";
                        bool flag1 = true;
                        bool flag2 = false;
                        if (index != -1)
                        {
                            if (Profiles.IsDefault(_profiles[index]))
                            {
                                text2 = "DEFAULT";
                                flag2 = true;
                            }
                            else
                                text2 = _profiles[index].name;
                            flag1 = false;
                            if (_profiles[index] == Profiles.experienceProfile)
                                text2 = "@RAINBOWICON@|DGBLUE|" + text2 + "|WHITE|";
                            else if (_profiles[index].steamID != 0UL)
                                text2 = "@STEAMICON@|DGBLUE|" + text2 + "|WHITE|";
                        }
                        string text3 = null;
                        if (_desiredSelectorPosition == index && (index1 == 3 || _slideTo > 0 && index1 == 4 || _slideTo < 0 && index1 == 2))
                            text3 = "> " + text2 + " <";
                        float num2 = (float)(this.y + num1 + 33);
                        float y = (float)(this.y + num1 + index1 * 11 + -_slide * 11);
                        float num3 = Maths.Clamp((float)((33 - Math.Abs(y - num2)) / 33), 0f, 1f);
                        float num4 = num3 * Maths.NormalizeSection(num3, 0f, 0.9f);
                        float num5 = 0.2f;
                        float num6 = Maths.Clamp(num3 >= 0.3f ? (num3 >= 0.8f ? Maths.NormalizeSection(num3, 0.8f, 1f) + num5 : num5) : Maths.NormalizeSection(num3, 0f, 0.3f) * num5, 0f, 1f);
                        bool flag3 = false;
                        if ((_selector == null || !_selector.isArcadeHatSelector) && index != -1 && (Profiles.active.Contains(_profiles[index]) || Profiles.active.FirstOrDefault(x => x.linkedProfile == _profiles[index]) != null))
                            flag3 = true;
                        if (flag3)
                            text2 = text2.Replace("|DGBLUE|", "");
                        _font.Draw(text2, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _font.GetWidth(text2) / 2), y)), (flag3 ? Color.Red : (flag1 ? Color.Lime : (flag2 ? Colors.DGYellow : Colors.MenuOption))) * num6, (Depth)0.95f);
                        if (text3 != null)
                            _font.Draw(text3, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _font.GetWidth(text3) / 2), y)), Color.White, (Depth)0.92f);
                    }
                    float y1 = num1 + 32f;
                    Graphics.DrawRect(this.position + new Vec2(2f, y1), this.position + new Vec2(138f, y1 + 9f), new Color(30, 30, 30) * _fade, (Depth)0.8f);
                    _font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    _font.Draw(HoveredProfileIsCustom() ? "@MENU2@" : "@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    this.position = position;
                    _selector.screen.EndDraw();
                }
                else
                {
                    if (_mode != PSMode.CreateProfile)
                        return;
                    Vec2 position = this.position;
                    this.position = Vec2.Zero;
                    _selector.screen.BeginDraw();
                    string str1 = "NONAME";
                    if (_name != "")
                    {
                        str1 = _name;// ParentalControls.RunProfanityCheck(ref this._name) > 0  
                    }
                    Vec2 pos = new Vec2(x + 36f, y + 8f);
                    if (!isEditing)
                    {
                        if (_changeName)
                        {
                            pos.x -= 2f;
                            for (int index = 0; index < 9; ++index)
                            {
                                Graphics.DrawRect(pos + new Vec2(index * 8, 0f), pos + new Vec2(index * 8 + 7, 7f), new Color(60, 60, 60), (Depth)0.8f);
                                if (index == _currentLetter)
                                {
                                    _spinnerArrows.frame = 0;
                                    Vec2 vec2_1 = pos + new Vec2(index * 8, -6f);
                                    Graphics.Draw(_spinnerArrows, vec2_1.x, vec2_1.y, (Depth)0.95f);
                                    _spinnerArrows.frame = 1;
                                    Vec2 vec2_2 = pos + new Vec2(index * 8, 9f);
                                    Graphics.Draw(_spinnerArrows, vec2_2.x, vec2_2.y, (Depth)0.95f);
                                    Graphics.DrawRect(pos + new Vec2(index * 8 - 2, -2f), pos + new Vec2(index * 8 + 9, 9f), Color.White * 0.8f, (Depth)0.97f, false);
                                }
                            }
                            _font.Draw(str1, Maths.RoundToPixel(pos), Color.Lime * (_createSelection == PSCreateSelection.ChangeName ? 1f : 0.6f), (Depth)0.95f);
                            pos.x += 2f;
                            string text4 = ">              <";
                            _font.Draw(text4, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _font.GetWidth(text4) / 2), pos.y)), Color.White * (_createSelection == PSCreateSelection.ChangeName ? 1f : 0.6f), (Depth)0.95f);
                            if (_takenFlash > 0.05f)
                            {
                                string text5 = "Name Taken";
                                _font.Draw(text5, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _font.GetWidth(text5) / 2), pos.y)), Color.Red * _takenFlash, (Depth)0.97f);
                                Graphics.DrawRect(new Vec2(x + 20f, pos.y), new Vec2((float)(x + width - 20), pos.y + 8f), Color.Black, (Depth)0.96f);
                            }
                        }
                        else
                        {
                            string str2 = str1.Replace(" ", "");
                            string text = _createSelection != PSCreateSelection.ChangeName ? "@LWING@" + str2.Reduced(12) + "@RWING@" : "> " + str2.Reduced(12) + " <";
                            _font.Draw(text, Maths.RoundToPixel(new Vec2((float)(x + 2 + width / 2 - _font.GetWidth(text) / 2), pos.y)), Color.White * (_createSelection == PSCreateSelection.ChangeName ? 1f : 0.6f), (Depth)0.95f);
                        }
                    }
                    else
                    {
                        string text = "@LWING@" + str1.Reduced(12) + "@RWING@";
                        _font.Draw(text, Maths.RoundToPixel(new Vec2((float)(x + width / 2 - _font.GetWidth(text) / 2), y + 8f)), Color.White * (1f - Math.Min(1f, _takenFlash * 2f)), (Depth)0.95f);
                    }
                    pos.y += 14f;
                    string text6 = "            ";
                    if (_createSelection == PSCreateSelection.Mood)
                        text6 = "< " + text6 + " >";
                    _font.Draw(text6, (float)(x + width / 2 - _font.GetWidth(text6) / 2), pos.y, Color.White * (_createSelection == PSCreateSelection.Mood ? 1f : 0.6f), (Depth)0.95f);
                    Graphics.DrawLine(new Vec2((float)(x + width / 4 + 4), pos.y + 5f), new Vec2(x + (float)(width / 4 * 3), pos.y + 5f), Colors.MenuOption * (_createSelection == PSCreateSelection.Mood ? 1f : 0.6f), 2f, (Depth)0.95f);
                    float num = 60f;
                    Graphics.DrawLine(new Vec2((float)(x + width / 2 - num / 2 + num * _moodVal + 2), pos.y + 1f), new Vec2((float)(x + width / 2 - num / 2 + num * _moodVal + 2), pos.y + 4f), Colors.MenuOption * (_createSelection == PSCreateSelection.Mood ? 1f : 0.6f), 3f, (Depth)0.95f);
                    Graphics.DrawLine(new Vec2((float)(x + width / 2 - num / 2 + num * _moodVal + 2), pos.y + 6f), new Vec2((float)(x + width / 2 - num / 2 + num * _moodVal + 2), pos.y + 9f), Colors.MenuOption * (_createSelection == PSCreateSelection.Mood ? 1f : 0.6f), 3f, (Depth)0.95f);
                    _happyIcons.color = Color.White * (_createSelection == PSCreateSelection.Mood ? 1f : 0.6f);
                    _happyIcons.alpha = _fade;
                    _happyIcons.frame = (int)Math.Round(_moodVal * 4);
                    _happyIcons.depth = (Depth)0.95f;
                    Graphics.Draw(_happyIcons, (float)(x + width / 6 + 2), pos.y + 4f);
                    _angryIcons.color = Color.White * (_createSelection == PSCreateSelection.Mood ? 1f : 0.6f);
                    _angryIcons.alpha = _fade;
                    _angryIcons.frame = (int)Math.Round((1f - _moodVal) * 4f);
                    _angryIcons.depth = (Depth)0.95f;
                    Graphics.Draw(_angryIcons, x + (float)(width / 6 * 5), pos.y + 4f);
                    pos.y += 16f;
                    string text7 = _preferredColor >= 0 ? "COLOR" : "NO COLOR";
                    if (_createSelection == PSCreateSelection.Color)
                        text7 = "< " + text7 + " >";
                    if (_preferredColor >= 0)
                    {
                        Graphics.DrawRect(new Vec2(x + 20f, pos.y - 2f), new Vec2(x + (width - 20f), pos.y + 9f), Persona.alllist[_preferredColor].colorDark.ToColor() * (_createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.93f, false);
                        _font.Draw(text7, Maths.RoundToPixel(new Vec2((float)(x + 2 + width / 2 - _font.GetWidth(text7) / 2), pos.y)), Persona.alllist[_preferredColor].color.ToColor() * (_createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.95f);
                    }
                    else
                    {
                        Graphics.DrawRect(new Vec2(x + 20f, pos.y - 2f), new Vec2(x + (width - 20f), pos.y + 9f), Colors.BlueGray * (_createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.93f, false);
                        _font.Draw(text7, Maths.RoundToPixel(new Vec2((float)(x + 2 + width / 2 - _font.GetWidth(text7) / 2), pos.y)), Colors.BlueGray * (_createSelection == PSCreateSelection.Color ? 1f : 0.6f), (Depth)0.95f);
                    }
                    pos.y += 12f;
                    string text8 = "CONTROLS";
                    if (_createSelection == PSCreateSelection.Controls)
                        text8 = "> " + text8 + " <";
                    _font.Draw(text8, Maths.RoundToPixel(new Vec2((float)(x + 2 + width / 2 - _font.GetWidth(text8) / 2), pos.y)), Colors.MenuOption * (_createSelection == PSCreateSelection.Controls ? 1f : 0.6f), (Depth)0.95f);
                    string text9 = "OK";
                    if (_createSelection == PSCreateSelection.Accept)
                        text9 = "> " + text9 + " <";
                    pos.y += 12f;
                    _font.Draw(text9, Maths.RoundToPixel(new Vec2((float)(x + 2 + width / 2 - _font.GetWidth(text9) / 2), pos.y)), Colors.MenuOption * (_createSelection == PSCreateSelection.Accept ? 1f : 0.6f), (Depth)0.95f);
                    if (_changeName)
                    {
                        string text10 = "@DPAD@";
                        if (_selector != null && (_selector.profileBoxNumber == 0 || _selector.profileBoxNumber == 2))
                            text10 = "@WASD@";
                        _font.Draw(text10, 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                        _font.Draw("@SELECT@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    }
                    else if (_createSelection == PSCreateSelection.ChangeName)
                    {
                        _font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                        _font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    }
                    else if (_createSelection == PSCreateSelection.Mood)
                    {
                        string text11 = "@DPAD@";
                        if (_selector != null && (_selector.profileBoxNumber == 0 || _selector.profileBoxNumber == 2))
                            text11 = "@WASD@";
                        _font.Draw(text11, 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                        _font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    }
                    else if (_createSelection == PSCreateSelection.Color)
                    {
                        string text12 = "@DPAD@";
                        if (_selector != null && (_selector.profileBoxNumber == 0 || _selector.profileBoxNumber == 2))
                            text12 = "@WASD@";
                        _font.Draw(text12, 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                        _font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    }
                    else
                    {
                        _font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                        _font.Draw("@CANCEL@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, _inputProfile);
                    }
                    this.position = position;
                    _selector.screen.EndDraw();
                }
            }
        }

        public void Open(Profile p)
        {
            _desiredSelectorPosition = _selectorPosition = 0;
            if (_box == null && Level.current is TeamSelect2)
                _box = (Level.current as TeamSelect2).GetBox(p.networkIndex);
            if (_box == null)
                return;
            isEditing = false;
            _inputProfile = p.inputProfile;
            _profile = _starterProfile = p;
            RebuildProfileList();
            for (int index = 0; index < _profiles.Count; ++index)
            {
                if (_profiles[index] == _profile)
                {
                    _selectorPosition = index;
                    break;
                }
            }
            _desiredSelectorPosition = _selectorPosition;
            _open = true;
            _fade = 1f;
        }

        private bool ProfileAlreadySelected(Profile p) => _profile.linkedProfile != null ? p != null && Profiles.active.FirstOrDefault(x => x.linkedProfile == p) != null && p != _profile.linkedProfile : p != null && Profiles.active.Contains(p) && p != _profile;

        public void EditProfile(Profile p)
        {
            Open(p);
            isEditing = true;
            _mode = PSMode.EditProfile;
            _desiredMode = PSMode.EditProfile;
            _name = p.name;
            _desiredMode = PSMode.CreateProfile;
            _changeName = false;
            _currentLetter = 0;
            _moodVal = p.funslider;
            _preferredColor = p.preferredColor;
            _createSelection = PSCreateSelection.Accept;
            _configInputMapping = Input.GetDefaultMapping(_inputProfile.lastActiveDevice.productName, _inputProfile.lastActiveDevice.productGUID, p: (isEditing ? _profile : null)).Clone();
        }

        private int ProfileIndexAdd(int index, int plus)
        {
            if (_profiles.Count == 0)
                return -1;
            int num = index + plus;
            while (num >= _profiles.Count)
                num -= _profiles.Count + 1;
            while (num < -1)
                num += _profiles.Count + 1;
            return num;
        }

        public override void Draw()
        {
            if (_fade < 0.01f)
                return;
            if (_mode == PSMode.EditControlsConfirm)
            {
                _selector.firstWord = "OK";
                _selector.secondWord = "BACK";
            }
            else if (_mode == PSMode.CreateProfile)
            {
                if (_changeName)
                {
                    _selector.firstWord = "MOVE";
                    _selector.secondWord = "OK";
                }
                else if (_createSelection == PSCreateSelection.ChangeName)
                {
                    _selector.firstWord = "ALTER";
                    _selector.secondWord = "BACK";
                }
                else if (_createSelection == PSCreateSelection.Mood)
                {
                    _selector.firstWord = "MOVE";
                    _selector.secondWord = "BACK";
                }
                else
                {
                    _selector.firstWord = "OK";
                    _selector.secondWord = "BACK";
                }
            }
            else if (_mode == PSMode.SelectProfile)
            {
                if (!HoveredProfileIsCustom())
                {
                    _selector.firstWord = "PICK";
                    _selector.secondWord = "BACK";
                }
                else
                {
                    _selector.firstWord = "PICK";
                    _selector.secondWord = "KILL";
                }
            }
            else if (!_editControl)
            {
                _selector.firstWord = "EDIT";
                _selector.secondWord = "SAVE";
            }
            else
            {
                _selector.firstWord = "BACK";
                _selector.secondWord = "";
            }
        }
    }
}
