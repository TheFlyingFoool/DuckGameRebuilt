using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame
{
    public class HatSelector : Thing, ITakeInput
    {
        public const string ButtonSprite = "iVBORw0KGgoAAAANSUhEUgAAABkAAAAYCAMAAAA4a6b0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAPUExURQAAANkCAoMBARsmMgAAAJo1rnoAAAAFdFJOU/////8A+7YOUwAAAAlwSFlzAAAOwwAADsMBx2+oZAAAAGBJREFUKFOtjlsKwEAIA7Np73/mJtY+FUphZz8EZ41iFUCUJ25hDFaljkSnIEG/amKincHQ/spulgIUESb/nchQxSajExvv+GvmptmwNfOv7vd83/biSiscMw1pMv4GSGw7tAWYP02v3wAAAABJRU5ErkJggg==";
        public Sprite extraButton;

        public StateBinding _profileBoxNumberBinding = new StateBinding(nameof(profileBoxNumber));
        private sbyte _profileBoxNumber = -1;
        public StateBinding _positionBinding = new StateBinding(nameof(netPosition));
        public StateBinding _openBinding = new StateBinding(nameof(_open));
        public StateBinding _selectorPositionBinding = new StateBinding(nameof(_selectorPosition));
        public StateBinding _desiredTeamSelectionBinding = new StateBinding(nameof(_desiredTeamSelection));
        public StateBinding _mainSelectionBinding = new StateBinding(nameof(_mainSelection));
        public StateBinding _selectionBinding = new StateBinding(nameof(selectionInt));
        public StateBinding _lcdFlashBinding = new StateBinding(nameof(_lcdFlash));
        public StateBinding _lcdFlashIncBinding = new StateBinding(nameof(_lcdFlashInc));
        public StateBinding _editingRoomBinding = new StateBinding(nameof(_editingRoom));
        public StateBinding _gettingXPBinding = new StateBinding(nameof(_gettingXP));
        public StateBinding _gettingXPCompletionBinding = new StateBinding(nameof(_gettingXPCompletion));
        public StateBinding _flashTransitionBinding = new StateBinding(nameof(flashTransition));
        public StateBinding _darkenBinding = new StateBinding(nameof(darken));
        public float _fade;
        private float _blackFade;
        public bool _open;
        private bool _closing;
        public bool _gettingXP;
        public float _gettingXPCompletion;
        public bool _editingRoom;
        public short _selectorPosition;
        public short _teamSelection;
        public short _desiredTeamSelection;
        public short _mainSelection;
        public float _slide;
        public float _slideTo;
        public float _upSlide;
        public float _upSlideTo;
        private bool fakefade;
        private string _firstWord = "";
        private string _secondWord = "";
        private InputProfile _blankProfile = new InputProfile();
        private InputProfile _inputProfile;
        private Profile _profile;
        private BitmapFont _font;
        public float _lcdFlash;
        public float _lcdFlashInc;
        private HSSelection _selection = HSSelection.ChooseProfile;
        private ConsoleScreen _screen;
        private ProfileSelector _profileSelector;
        public RoomEditor _roomEditor;
        private Sprite _oButton;
        private ProfileBox2 _box;
        private SpriteMap _demoBox;
        private Sprite _selectBorder;
        private Sprite _consoleText;
        private Sprite _contextArrow;
        private SpriteMap _clueHat;
        private SpriteMap _boardLoader;
        private SpriteMap _lock;
        private SpriteMap _goldLock;
        private SpriteMap _gettingXPBoard;
        private SpriteMap _editingRoomBoard;
        private Sprite _blind;
        public Hat hat;
        private bool _teamWasCustomHat;
        private string _teamName = "";
        private Team _netHoveringTeam;
        private MaterialSecretOutline _outlineMaterial;
        private int _prevDesiredTeam;
        private Profile _experienceProfileCheck;
        public bool isArcadeHatSelector;
        private bool _editRoomDisabled;
        private Team _startingTeam;
        private bool _inputSkip;
        private float _blindLerp;

        public sbyte profileBoxNumber
        {
            get => _profileBoxNumber;
            set
            {
                if (value < 0)
                    return;
                if (_box != null)
                {
                    int controllerIndex = _box.controllerIndex;
                }
                bool flag = _profileBoxNumber != value;
                _profileBoxNumber = value;
                if (Network.isClient)
                {
                    _profile = DuckNetwork.profiles[_profileBoxNumber];
                    _inputProfile = _profile.inputProfile;
                    if (Level.current is TeamSelect2 current)
                    {
                        _box = current.GetBox((byte)_profileBoxNumber);
                        _box.SetHatSelector(this);
                    }
                    else
                        DevConsole.Log(DCSection.General, "!---CRITICAL! Profile box link failure!(" + _profileBoxNumber.ToString() + ")---!");
                }
                if (_profile == null || _profile.connection != DuckNetwork.localConnection)
                    return;
                if (flag)
                    Fondle(this, DuckNetwork.localConnection);
                connection = DuckNetwork.localConnection;
            }
        }

        public new virtual Vec2 netPosition
        {
            get => position;
            set => position = value;
        }

        public bool flashTransition
        {
            get => _screen._flashTransition;
            set => _screen._flashTransition = value;
        }

        public float darken
        {
            get => _screen._darken;
            set => _screen._darken = value;
        }

        public float fade
        {
            get => _fade;
            set => _fade = value;
        }

        public bool open => _open;

        public float fadeVal
        {
            get
            {
                if (fakefade)
                    return 1f;
                float fadeVal = _fade;
                if (_profileSelector.fade > 0)
                    fadeVal = 1f;
                if (_roomEditor.fade > 0)
                    fadeVal = 1f;
                return fadeVal;
            }
        }

        public string firstWord
        {
            get => _firstWord;
            set => _firstWord = value;
        }

        public string secondWord
        {
            get => _secondWord;
            set => _secondWord = value;
        }

        public ProfileSelector profileSelector
        {
            get
            {
                return _profileSelector;
            }
        }

        public InputProfile profileInput => _profile != null ? _profile.inputProfile : inputProfile;

        public InputProfile inputProfile
        {
            get
            {
                if (Network.isActive && connection != DuckNetwork.localConnection)
                    return _blankProfile;
                return _profile != null ? _profile.inputProfile : _inputProfile;
            }
        }

        public Profile profile => _profile;

        public float lcdFlash => _lcdFlash;

        public byte selectionInt
        {
            get => (byte)_selection;
            set => _selection = (HSSelection)value;
        }

        public ConsoleScreen screen => _screen;

        public ProfileBox2 box => _box;

        public HatSelector(float xpos, float ypos, Profile profile, ProfileBox2 box)
          : base(xpos, ypos)
        {
            _profile = profile;
            _inputProfile = _profile.inputProfile;
            _box = box;
            if (Network.isServer)
                _profileBoxNumber = (sbyte)box.controllerIndex;
            Construct();
        }

        public HatSelector()
          : base()
        {
            Construct();
        }

        public void Construct()
        {
            _font = new BitmapFont("biosFontUI", 8, 7)
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            _collisionSize = new Vec2(141f, 89f);
            extraButton = new Sprite(new Tex2D(Texture2D.FromStream(Graphics.device, new MemoryStream(Convert.FromBase64String(ButtonSprite))), "button"))
            {
                center = new Vec2(12.5f, 12),
                Namebase = "nikoextraButton"
            };
            Content.textures[extraButton.Namebase] = extraButton.texture;
            _oButton = new Sprite("oButton");
            _demoBox = new SpriteMap("demoCrate", 20, 20);
            _demoBox.CenterOrigin();
            _clueHat = new SpriteMap("hats/cluehat", 32, 32);
            _clueHat.CenterOrigin();
            _blind = new Sprite("blind");
            _gettingXPBoard = new SpriteMap("gettingXP", 63, 30);
            _gettingXPBoard.CenterOrigin();
            _editingRoomBoard = new SpriteMap("editingRoom", 63, 30);
            _editingRoomBoard.CenterOrigin();
            _boardLoader = new SpriteMap("boardLoader", 7, 7);
            _boardLoader.AddAnimation("idle", 0.2f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            _boardLoader.CenterOrigin();
            _boardLoader.SetAnimation("idle");
            _selectBorder = new Sprite("selectBorder2");
            _consoleText = new Sprite("corptronConsoleText");
            _contextArrow = new Sprite("contextArrowRight");
            _lock = new SpriteMap("arcade/unlockLock", 15, 18);
            _goldLock = new SpriteMap("arcade/goldUnlockLock", 15, 18);
        }

        public void SetProfile(Profile newProfile) => _profile = newProfile;

        public void ConfirmProfile() => _selection = HSSelection.Main;

        public override void Initialize()
        {
            _profileSelector = new ProfileSelector(x, y, _box, this);
            Level.Add(_profileSelector);
            _roomEditor = new RoomEditor(x, y, _box, this);
            Level.Add(_roomEditor);
            _screen = new ConsoleScreen(x, y, this);
        }

        private int ControllerNumber()
        {
            if (Network.isActive)
                return Maths.Clamp(_profileBoxNumber, 0, DG.MaxPlayers - 1);
            int index = Array.IndexOf(InputProfile.MPPlayers, inputProfile.name);
            if (index == -1)
                index = 0;
            return index;
        }

        private void SelectTeam()
        {
            if (_desiredTeamSelection >= AllTeams().Count)
                return;
            FilterTeam().Join(_profile);
        }

        private Team FilterTeam(bool hardFilter = false)
        {
            _teamWasCustomHat = false;
            _teamName = "";
            if (Network.isActive)
            {
                if (_desiredTeamSelection >= AllTeams().Count)
                    ControllerNumber();
                Team allTeam = AllTeams()[_desiredTeamSelection];
                _teamName = allTeam.name.ToUpperInvariant();
                if (Teams.core.extraTeams.Contains(allTeam))
                    _teamWasCustomHat = true;
                return allTeam;
            }
            List<Team> teamList = AllTeams();
            return _desiredTeamSelection < 0 || _desiredTeamSelection >= teamList.Count ? teamList[0] : teamList[_desiredTeamSelection];
        }

        public override void Terminate() => base.Terminate();

        public void ConfirmTeamSelection()
        {
            Team team = FilterTeam(true);
            if (Network.isActive && _box.duck != null)
            {
                if (_teamWasCustomHat)
                {
                    foreach (NetworkConnection connection in Network.connections)
                        Send.Message(new NMSpecialHat(team, _profile, connection.profile != null && connection.profile.muteHat), connection);
                }

                if (Network.isServer)
                {
                    if (!TeamSelect2.CheckForCTeams(_box.duck.profile))
                    {
                        Send.Message(new NMSetTeam(_box.duck.profile, team, _teamWasCustomHat));
                    }
                }
                else
                {
                    Send.Message(new NMSetTeam(_box.duck.profile, team, _teamWasCustomHat));
                }
            }

            DGRSettings.arcadeDuckColor = _profile.persona.index;

            if (team.hasHat)
            {
                if (_box.duck != null)
                {
                    if (isArcadeHatSelector) DGRSettings.arcadeHat = team.name;
                    Hat equipment = _box.duck.GetEquipment(typeof(Hat)) as Hat;
                    Hat hat = new TeamHat(0f, 0f, team, _box.duck.profile);
                    Level.Add(hat);
                    _box.duck.Equip(hat, false);
                    _box.duck.Fondle(hat);
                    if (this.hat != null)
                        Level.Remove(this.hat);
                    this.hat = hat;
                    if (equipment != null)
                    {
                        Level.Remove(equipment);
                        if (Network.isActive)
                            Send.Message(new NMUnequip(_box.duck, equipment), NetMessagePriority.ReliableOrdered);
                    }
                    if (Network.isActive)
                        Send.Message(new NMEquip(_box.duck, this.hat), NetMessagePriority.ReliableOrdered);
                }
                else if (hat != null)
                    Level.Remove(hat);
            }
            else
            {
                if (hat != null)
                    Level.Remove(hat);
                hat = null;
                if (_box.duck != null && _box.duck.GetEquipment(typeof(Hat)) is Hat equipment)
                {
                    _box.duck.Unequip(equipment);
                    Level.Remove(equipment);
                    if (Network.isActive)
                        Send.Message(new NMUnequip(_box.duck, equipment), NetMessagePriority.ReliableOrdered);
                }
            }
            if (_desiredTeamSelection <= DG.MaxPlayers - 1 || _box.duck == null)
                return;
            DuckNetwork.OnTeamSwitch(_box.duck.profile);
        }

        public List<Team> indexedAllTeams = new List<Team>();
        private int TeamIndexAdd(int index, int plus, bool alwaysThree = true)
        {
            if (alwaysThree && index < DG.MaxPlayers && index >= 0)
                index = DG.MaxPlayers - 1;
            int num = index + plus;
            if (num >= indexedAllTeams.Count)
                return num - indexedAllTeams.Count + (DG.MaxPlayers - 1);
            return num < DG.MaxPlayers - 1 ? indexedAllTeams.Count + (num - (DG.MaxPlayers - 1)) : num;
        }

        private int TeamIndexAddSpecial(int index, int plus, bool alwaysThree = true)
        {
            if (alwaysThree && index < DG.MaxPlayers && index >= 0)
                index = DG.MaxPlayers - 1;
            int num = index + plus;
            if (num >= AllTeams().Count)
                num = num - AllTeams().Count + (DG.MaxPlayers - 1);
            if (num < DG.MaxPlayers - 1)
                num = AllTeams().Count + (num - (DG.MaxPlayers - 1));
            if (num <= DG.MaxPlayers - 1)
                num = profileBoxNumber;
            return num;
        }

        private int GetTeamIndex(Team tm)
        {
            int teamIndex = 0;
            foreach (Team allTeam in AllTeams())
            {
                if (allTeam != tm)
                    ++teamIndex;
                else
                    break;
            }
            return teamIndex;
        }

        public void Reset()
        {
            _netHoveringTeam = null;
            if (_profile != null)
            {
                _profile.hatSelector = null;
            }
            _open = false;
            _closing = true;
            _selection = HSSelection.Main;
            _mainSelection = 0;
            _editingRoom = false;
            _gettingXP = false;
            _profileSelector.Reset();
            _roomEditor.Reset();
        }

        public static List<Team> remember;
        public List<Team> AllTeams()
        {
            if (!Network.isActive)
            {
                if (DGRSettings.favoriteHats.Count == 0)
                {
                    List<Team> filtered = new List<Team>();
                    for (int i = 0; i < Teams.all.Count; i++)
                    {
                        Team t = Teams.all[i];
                        if (t.NoDisplay) continue;
                        filtered.Add(t);
                    }
                    return filtered;
                }
                if (remember == null)
                {
                    List<Team> tts = new List<Team>();

                    List<Team> laterer = new List<Team>();
                    for (int i = 0; i < Teams.all.Count; i++)
                    {
                        Team t = Teams.all[i];
                        if (t.NoDisplay) continue;
                        if (t.favorited)
                        {
                            laterer.Add(t);
                        }
                        else
                        {
                            tts.Add(t);
                        }
                    }
                    tts.AddRange(laterer);

                    remember = tts;
                    return tts;
                }
                else return remember;
            }
            if (_profile == null)
                return Teams.core.teams;
            if (_profile.connection != DuckNetwork.localConnection)
            {
                List<Team> teamList = new List<Team>(Teams.core.teams);
                foreach (Team customTeam in _profile.customTeams)
                {
                    if (customTeam.NoDisplay) continue;
                    teamList.Add(customTeam);
                }
                return teamList;
            }

            if (DGRSettings.favoriteHats.Count == 0)
            {
                List<Team> list2 = new List<Team>(Teams.core.teams);
                foreach (Team item2 in Teams.core.extraTeams)
                {
                    if (item2.NoDisplay) continue;
                    list2.Add(item2);
                }
                return list2;
            }
            List<Team> ttss = new List<Team>();
            List<Team> later = new List<Team>();
            for (int i = 0; i < Teams.core.teams.Count; i++)
            {
                Team t = Teams.core.teams[i];
                if (t.NoDisplay) continue;
                if (t.favorited)
                {
                    later.Add(t);
                }
                else
                {
                    ttss.Add(t);
                }
            }
            for (int i = 0; i < Teams.core.extraTeams.Count; i++)
            {
                Team t = Teams.core.extraTeams[i];
                if (t.NoDisplay) continue;
                if (t.favorited)
                {
                    later.Add(t);
                }
                else
                {
                    ttss.Add(t);
                }
            }
            ttss.AddRange(later);

            return ttss;
        }

        public override void Update()
        {
            bool flag1 = true;
            if (_profileBoxNumber < 0 || inputProfile == null || _box == null || _profile == null) return;
            if (connection == DuckNetwork.localConnection && inputProfile.Pressed(Triggers.Any))
            {
                NetIndex8 authority = this.authority;
                this.authority = ++authority;
            }
            if (Network.isActive && connection == DuckNetwork.localConnection && Profiles.experienceProfile != null && profile.linkedProfile == Profiles.experienceProfile)
            {
                if (MonoMain.pauseMenu != null)
                {
                    if (MonoMain.pauseMenu is UILevelBox)
                    {
                        _gettingXP = true;
                        UILevelBox pauseMenu = MonoMain.pauseMenu as UILevelBox;
                        _gettingXPCompletion = ((pauseMenu._dayProgress + pauseMenu._xpProgress) / 2f * 0.7f);
                    }
                    else
                    {
                        _gettingXPCompletion = 0.7f;
                        if (MonoMain.pauseMenu is UIFuneral) _gettingXPCompletion = 0.8f;
                        else if (MonoMain.pauseMenu is UIGachaBox) _gettingXPCompletion = 0.9f;
                    }
                }
                else
                {
                    _gettingXP = false;
                    _gettingXPCompletion = 0f;
                }
            }
            if (Network.isActive && (connection == null || connection.status == ConnectionStatus.Disconnected || profile == null || profile.connection == null || profile.connection.status == ConnectionStatus.Disconnected))
            {
                _experienceProfileCheck = null;
                _gettingXP = false;
                _open = false;
                if (_profile != null) _profile.hatSelector = null;
            }
            _fade = Lerp.Float(_fade, !_open || _profileSelector.open || _roomEditor.open ? 0f : 1f, 0.1f);
            _blackFade = Lerp.Float(_blackFade, _open ? 1f : 0f, 0.1f);
            _screen.Update();
            if (_screen.transitioning) _experienceProfileCheck = null;
            else if (_profileSelector.open || _roomEditor.open) _experienceProfileCheck = null;
            else
            {
                if (Profiles.IsDefault(_profile)) flag1 = false;
                if (Profiles.experienceProfile == null) flag1 = false;
                _editRoomDisabled = false;
                if (NetworkDebugger.enabled)
                {
                    flag1 = true;
                    _editRoomDisabled = false;
                }
                else if (!flag1 && Network.isActive)
                {
                    flag1 = true;
                    _editRoomDisabled = true;
                }
                if (isArcadeHatSelector) flag1 = false;
                if (!_open)
                {
                    if (_fade < 0.01f && _closing)
                    {
                        _closing = false;
                        if (_box != null) _box.ReturnControl();
                    }
                    _experienceProfileCheck = null;
                }
                else if (_profile.team == null || _inputSkip) _inputSkip = false;
                else
                {
                    _lcdFlashInc += Rando.Float(0.3f, 0.6f);
                    _lcdFlash = (0.9f + (float)(Math.Sin(_lcdFlashInc) + 1f) / 2f * 0.1f);
                    if (_prevDesiredTeam != _desiredTeamSelection && !isServerForObject)
                    {
                        if (TeamIndexAddSpecial(_desiredTeamSelection, 5) == _prevDesiredTeam) _upSlideTo = -1f;
                        else if (TeamIndexAddSpecial(_desiredTeamSelection, -5) == _prevDesiredTeam) _upSlideTo = 1f;
                        else if (TeamIndexAddSpecial(_desiredTeamSelection, -1) == _prevDesiredTeam) _slideTo = 1f;
                        else if (TeamIndexAddSpecial(_desiredTeamSelection, 1) == _prevDesiredTeam) _slideTo = -1f;
                        else if (TeamIndexAddSpecial(_desiredTeamSelection, -6) == _prevDesiredTeam)
                        {
                            _slideTo = 1f;
                            _upSlideTo = 1f;
                        }
                        else if (TeamIndexAddSpecial(_desiredTeamSelection, -4) == _prevDesiredTeam)
                        {
                            _slideTo = -1f;
                            _upSlideTo = 1f;
                        }
                        else if (TeamIndexAddSpecial(_desiredTeamSelection, 6) == _prevDesiredTeam)
                        {
                            _slideTo = -1f;
                            _upSlideTo = -1f;
                        }
                        else if (TeamIndexAddSpecial(_desiredTeamSelection, 4) == _prevDesiredTeam)
                        {
                            _slideTo = 1f;
                            _upSlideTo = -1f;
                        }
                        else _teamSelection = _desiredTeamSelection;
                        SFX.Play("consoleTick", 0.6f);
                        List<Team> teamList = AllTeams();
                        if (_desiredTeamSelection < teamList.Count)
                        {
                            _teamName = teamList[_desiredTeamSelection].name.ToUpperInvariant();
                            _netHoveringTeam = teamList[_desiredTeamSelection];
                        }
                        _prevDesiredTeam = _desiredTeamSelection;
                    }
                    if (_slideTo != 0 && _slide != _slideTo) _slide = Lerp.Float(_slide, _slideTo, 0.1f);
                    else if (_slideTo != 0 && _slide == _slideTo)
                    {
                        _slide = 0f;
                        _slideTo = 0f;
                        _teamSelection = _desiredTeamSelection;
                        if (isServerForObject) SelectTeam();
                    }
                    if (_upSlideTo != 0 && _upSlide != _upSlideTo) _upSlide = Lerp.Float(_upSlide, _upSlideTo, 0.1f);
                    else if (_upSlideTo != 0 && _upSlide == _upSlideTo)
                    {
                        _upSlide = 0f;
                        _upSlideTo = 0f;
                        _teamSelection = _desiredTeamSelection;
                        if (isServerForObject) SelectTeam();
                    }
                    if (_selection == HSSelection.ChooseTeam)
                    {
                        string name = _profile.team.name;//Canewton's balls
                        string text1 = name == Teams.Player1.name || name == Teams.Player2.name || name == Teams.Player3.name || name == Teams.Player4.name || name == Teams.Player5.name || name == Teams.Player6.name || name == Teams.Player7.name || name == Teams.Player8.name ? "NO TEAM" : _profile.team.GetNameForDisplay();

                        if (_desiredTeamSelection == _teamSelection)
                        {
                            bool flag2 = false;
                            if (inputProfile.Down(Triggers.MenuLeft))
                            {
                                if (_desiredTeamSelection < DG.MaxPlayers) _desiredTeamSelection = (short)(AllTeams().Count - 1);
                                else if (_desiredTeamSelection == DG.MaxPlayers) _desiredTeamSelection = (short)ControllerNumber();
                                else _desiredTeamSelection--;
                                _slideTo = -1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (inputProfile.Down(Triggers.MenuRight))
                            {
                                if (_desiredTeamSelection >= AllTeams().Count - 1) _desiredTeamSelection = (short)ControllerNumber();
                                else if (_desiredTeamSelection < DG.MaxPlayers) _desiredTeamSelection = (short)DG.MaxPlayers;
                                else _desiredTeamSelection++;
                                _slideTo = 1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (inputProfile.Down(Triggers.MenuUp))
                            {
                                if (_desiredTeamSelection < DG.MaxPlayers) _desiredTeamSelection = 0;
                                else _desiredTeamSelection -= (short)(DG.MaxPlayers - 1);
                                _desiredTeamSelection -= 5;
                                if (_desiredTeamSelection < 0) _desiredTeamSelection += (short)(AllTeams().Count - (DG.MaxPlayers - 1));
                                if (_desiredTeamSelection == 0) _desiredTeamSelection = (short)ControllerNumber();
                                else _desiredTeamSelection += (short)(DG.MaxPlayers - 1);
                                _upSlideTo = -1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (inputProfile.Down(Triggers.MenuDown))
                            {
                                if (_desiredTeamSelection < DG.MaxPlayers) _desiredTeamSelection = 0;
                                else _desiredTeamSelection -= (short)(DG.MaxPlayers - 1);
                                _desiredTeamSelection += 5;
                                if (_desiredTeamSelection >= AllTeams().Count - (DG.MaxPlayers - 1))
                                    _desiredTeamSelection -= (short)(AllTeams().Count - (DG.MaxPlayers - 1));
                                if (_desiredTeamSelection == 0)
                                    _desiredTeamSelection = (short)ControllerNumber();
                                else
                                    _desiredTeamSelection += (short)(DG.MaxPlayers - 1);
                                _upSlideTo = 1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (inputProfile.Pressed(Triggers.Select) && !flag2)
                            {
                                if (_profile.team.locked)
                                {
                                    _profile.team.shake = 2;
                                    SFX.Play("consoleError");
                                }
                                else
                                {
                                    SFX.Play("consoleSelect", 0.4f);
                                    _selection = HSSelection.Main;
                                    _screen.DoFlashTransition();
                                    ConfirmTeamSelection();
                                }
                            }
                            //NiK0's personal hell
                            if (inputProfile.Pressed(Triggers.Menu2) && text1 != "NO TEAM")
                            {
                                Team t = FilterTeam();
                                if (t.locked)
                                {
                                    SFX.Play("consoleError");
                                    t.shake = 2;
                                }
                                else
                                {
                                    SFX.Play("click");
                                    t.favorited = !t.favorited;

                                    DGRSettings.ReloadFavHats();
                                    _desiredTeamSelection = (short)AllTeams().IndexOf(t);
                                    _slideTo = float.Epsilon;
                                }
                            }
                            if (inputProfile.Pressed(Triggers.Ragdoll))
                            {
                                if (profile.requestedColor == -1) profile.requestedColor = profile.currentColor;
                                profile.IncrementRequestedColor();
                                SFX.Play("consoleTick", 0.7f);
                                profile.UpdatePersona();
                            }
                            if (inputProfile.Pressed(Triggers.Cancel))
                            {
                                //this if check is here because if the host has Custom Hat Teams enabled and this player is currently in a custom hat team then
                                //they'll instantly crash because GetTeamIndex() cant find the index of their currently wore hat as for that hat doesnt belong to them
                                //but it has been forcefully set to them by the host so teams can happen
                                //-NiK0
                                if (_startingTeam.defaultTeam || (_startingTeam.activeProfiles.Count == 1 && _startingTeam.activeProfiles.Contains(_box.duck.profile)))
                                {
                                    _desiredTeamSelection = (short)GetTeamIndex(_startingTeam);
                                    _teamSelection = _desiredTeamSelection;
                                    SelectTeam();
                                    ConfirmTeamSelection();
                                }
                                else
                                {
                                    _box.profile.team = _startingTeam;
                                    SFX.Play("consoleCancel", 0.4f);
                                    _selection = HSSelection.Main;
                                    _screen.DoFlashTransition();
                                    return;
                                }
                                SFX.Play("consoleCancel", 0.4f);
                                _selection = HSSelection.Main;
                                _screen.DoFlashTransition();
                            }
                        }
                        Vec2 position = this.position;
                        this.position = Vec2.Zero;
                        _screen.BeginDraw();
                        float num1 = -18f;
                        _profile.persona.sprite.alpha = _fade;
                        _profile.persona.sprite.color = Color.White;
                        _profile.persona.sprite.color = new Color(_profile.persona.sprite.color.r, _profile.persona.sprite.color.g, _profile.persona.sprite.color.b);
                        _profile.persona.sprite.depth = (Depth)0.9f;
                        _profile.persona.sprite.scale = new Vec2(scaleMultiplier);
                        Graphics.Draw(_profile.persona.sprite, x + 70f, y + 60f + num1, (Depth)0.9f);
                        short num2 = 0;
                        bool flag3 = false;
                        if (_teamSelection >= AllTeams().Count)
                        {
                            num2 = _teamSelection;
                            _teamSelection = (short)(AllTeams().Count - 1);
                            flag3 = true;
                        }

                        if (isServerForObject)
                        {
                            //do not question the elevated one's broken code
                            //as for it is PERFECT
                            extraButton.depth = 1;
                            extraButton.alpha = _fade;
                            Graphics.Draw(extraButton, x + 15, y + 61, (Depth)1);
                            _font.Draw("@MENU2@", x + 7.5f, y + 61, new Color(180, 180, 180), (Depth)1, profileInput);
                            _font.Draw("@STARGOODY@", x + 27, y + 61, new Color(180, 180, 180) * 0.3f, (Depth)1, profileInput);
                        }

                        HatsDrawLogic();
                        _font.alpha = _fade;
                        _font.depth = (Depth)0.96f;
                        if (_selection == HSSelection.ChooseTeam)
                        {
                            string text = "<              >";
                            Vec2 pixel = Maths.RoundToPixel(new Vec2((float)(x + this.width / 2 - _font.GetWidth(text) / 2), y + 60f + num1));
                            _font.Draw(text, pixel.x, pixel.y, Color.White, (Depth)0.95f);
                        }

                        if (_teamName != "")
                            text1 = _teamName;
                        bool flag6 = _profile.team.locked;
                        bool flag7 = false;
                        if (Network.isActive && !isServerForObject && _profile.networkHatUnlockStatuses != null && _desiredTeamSelection < _profile.networkHatUnlockStatuses.Count)
                        {
                            flag6 = _profile.networkHatUnlockStatuses[_desiredTeamSelection];
                            flag7 = true;
                        }
                        if (flag6) text1 = "LOCKED";
                        else if (flag7 && _netHoveringTeam != null && _netHoveringTeam.locked) text1 = "UNKNOWN";
                        _font.scale = new Vec2(1f, 1f);
                        float width = _font.GetWidth(text1);
                        Vec2 pixel1 = Maths.RoundToPixel(new Vec2((float)(x + this.width / 2 - width / 2), y + 25f + num1));
                        _font.Draw(text1, pixel1.x, pixel1.y, Color.LimeGreen * (_selection == HSSelection.ChooseTeam ? 1f : 0.6f), (Depth)0.95f);
                        Graphics.DrawLine(pixel1 + new Vec2(-10f, 4f), pixel1 + new Vec2(width + 10f, 4f), Color.White * 0.1f, 2f, (Depth)0.93f);
                        _font.Draw("@SELECT@", x + 4f, y + 79f, new Color(180, 180, 180), (Depth)0.95f, profileInput);
                        _font.Draw("@RAGDOLL@", x + 122f, y + 79f, new Color(180, 180, 180), (Depth)0.95f, profileInput);
                        _screen.EndDraw();
                        this.position = position;
                        if (!flag3)
                            return;
                        _teamSelection = num2;
                    }
                    else
                    {
                        if (_selection != HSSelection.Main)
                            return;
                        if (Level.current is ArcadeLevel && !Options.Data.defaultAccountMerged)
                        {
                            if (_experienceProfileCheck != _profile)
                            {
                                if (_profile == Profiles.experienceProfile)
                                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@MENU2@MERGE DEFAULT", inputProfile);
                                else
                                    HUD.CloseCorner(HUDCorner.BottomLeft);
                                _experienceProfileCheck = _profile;
                            }
                            if (_profile == Profiles.experienceProfile && inputProfile.Pressed(Triggers.Menu2))
                            {
                                UIMenu profileMergeMenu = Options.CreateProfileMergeMenu();
                                Level.Add(profileMergeMenu);
                                MonoMain.pauseMenu = profileMergeMenu;
                                profileMergeMenu.Open();
                            }
                        }
                        if (inputProfile.Pressed(Triggers.MenuUp))
                        {
                            if (_mainSelection > 0)
                            {
                                --_mainSelection;
                                SFX.Play("consoleTick");
                                if (_editRoomDisabled && _mainSelection == 2)
                                    _mainSelection = 1;
                            }
                        }
                        else if (inputProfile.Pressed(Triggers.MenuDown))
                        {
                            if (_mainSelection < (flag1 ? 3 : 2))
                            {
                                ++_mainSelection;
                                SFX.Play("consoleTick");
                            }
                            if (_editRoomDisabled && _mainSelection == 2)
                                _mainSelection = 3;
                        }
                        else if (inputProfile.Pressed(Triggers.Select))
                        {
                            if (_mainSelection == 1 && (!Network.isActive || !Profiles.IsExperience(_profile)))
                            {
                                _profileSelector.Open(_profile);
                                SFX.Play("consoleSelect", 0.4f);
                                _fade = 0f;
                                _screen.DoFlashTransition();
                            }
                            else if (_mainSelection == 0)
                            {
                                _selection = HSSelection.ChooseTeam;
                                SFX.Play("consoleSelect", 0.4f);
                                _screen.DoFlashTransition();
                            }
                            else if (_mainSelection == (flag1 ? 3 : 2))
                            {
                                if (_profile != null)
                                {
                                    _profile.hatSelector = null;
                                }
                                _open = false;
                                _closing = true;
                                SFX.Play("consoleCancel", 0.4f);
                                _selection = HSSelection.Main;
                            }
                            else if (flag1 && _mainSelection == 2)
                            {
                                _editingRoom = true;
                                _roomEditor.Open(_profile);
                                SFX.Play("consoleSelect", 0.4f);
                                _fade = 0f;
                                _screen.DoFlashTransition();
                            }
                        }
                        else if (_mainSelection == 1 && inputProfile.Pressed(Triggers.Menu1) && !Profiles.IsDefault(_profile))
                        {
                            _profileSelector.EditProfile(_profile);
                            SFX.Play("consoleSelect", 0.4f);
                            _fade = 0f;
                            _screen.DoFlashTransition();
                        }
                        else if (inputProfile.Pressed(Triggers.Cancel))
                        {
                            if (_profile != null)
                            {
                                _profile.hatSelector = null;
                            }
                            _open = false;
                            _closing = true;
                            SFX.Play("consoleCancel", 0.4f);
                            _selection = HSSelection.Main;
                        }
                        _screen.BeginDraw();
                        _font.scale = new Vec2(1f, 1f);
                        string text2 = "@LWING@CUSTOM DUCK@RWING@";
                        _font.Draw(text2, Maths.RoundToPixel(new Vec2((float)(width / 2f - _font.GetWidth(text2) / 2f), 10f)), Color.White, (Depth)0.95f);
                        string text3 = !Profiles.IsDefault(_profile) ? _profile.name : "PICK PROFILE";
                        Vec2 pixel2 = Maths.RoundToPixel(new Vec2((float)(width / 2f - _font.GetWidth(text3) / 2f), 39f));
                        _font.Draw(text3, pixel2, Colors.MenuOption * (_mainSelection == 1 ? 1f : 0.6f), (Depth)0.95f);
                        if (_mainSelection == 1)
                            Graphics.Draw(_contextArrow, pixel2.x - 8f, pixel2.y);
                        if (flag1)
                        {
                            string text4 = "@RAINBOWICON@EDIT ROOM";
                            Vec2 pixel3 = Maths.RoundToPixel(new Vec2((float)(width / 2f - _font.GetWidth(text4) / 2f), 48f));
                            _font.Draw(text4, pixel3, _editRoomDisabled ? Colors.SuperDarkBlueGray : Colors.MenuOption * (_mainSelection == 2 ? 1f : 0.6f), (Depth)0.95f, colorSymbols: true);
                            if (_mainSelection == 2)
                                Graphics.Draw(_contextArrow, pixel3.x - 8f, pixel3.y);
                        }
                        string text5 = _profile.team.hasHat ? "|LIME|" + _profile.team.GetNameForDisplay() + "|MENUORANGE| HAT" : "|MENUORANGE|CHOOSE HAT";
                        Vec2 pixel4 = Maths.RoundToPixel(new Vec2((float)(width / 2f - _font.GetWidth(text5) / 2f), 30f));
                        _font.Draw(text5, pixel4, Color.White * (_mainSelection == 0 ? 1f : 0.6f), (Depth)0.95f);
                        if (_mainSelection == 0)
                            Graphics.Draw(_contextArrow, pixel4.x - 8f, pixel4.y);
                        string text6 = "EXIT";
                        Vec2 pixel5 = Maths.RoundToPixel(new Vec2((float)(width / 2f - _font.GetWidth(text6) / 2f), 50 + (flag1 ? 12 : 9)));
                        _font.Draw(text6, pixel5, Colors.MenuOption * (_mainSelection == (flag1 ? 3 : 2) ? 1f : 0.6f), (Depth)0.95f);
                        if (_mainSelection == (flag1 ? 3 : 2))
                            Graphics.Draw(_contextArrow, pixel5.x - 8f, pixel5.y);
                        _font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, profileInput);
                        _font.Draw(_mainSelection != 1 || Profiles.IsDefault(_profile) ? "@CANCEL@" : "@MENU1@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, profileInput);
                        _consoleText.color = new Color(140, 140, 140);
                        Graphics.Draw(_consoleText, 30f, 18f);
                        _screen.EndDraw();
                    }
                }
            }
        }

        public void Open(Profile p)
        {
            p.hatSelector = this;
            _profile = p;
            _startingTeam = _profile.team;
            _open = true;
            _mainSelection = 0;
            _editingRoom = false;
            _gettingXP = false;
            _selection = HSSelection.Main;
            _teamSelection = _desiredTeamSelection = (short)GetTeamIndex(_profile.team);
            _inputSkip = true;
        }

        public override void Draw()
        {
            if (_profileBoxNumber < 0 || _box == null)
                return;
            fakefade = false;
            if (Network.isActive && _box.profile != null && _box.profile.connection != DuckNetwork.localConnection)
            {
                _blindLerp = Lerp.Float(_blindLerp, _editingRoom || _gettingXP ? 1f : 0f, 0.05f);
                if (_blindLerp > 0.01f)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        _blind.yscale = Math.Max(0f, Math.Min(_blindLerp * 3f - i * 0.05f, 1f));
                        _blind.depth = 0.91f + i * 0.008f;
                        _blind.flipH = false;
                        Graphics.Draw(_blind, x - 3f + i * (9f * _blindLerp), y + 1f);
                        _blind.flipH = true;
                        Graphics.Draw(_blind, x + 4f + 140f - i * (9f * _blindLerp), y + 1f);
                    }
                    float num = Math.Max((_blindLerp - 0.5f) * 2f, 0f);
                    if (num > 0.01f)
                    {
                        if (_gettingXP)
                        {
                            _gettingXPBoard.depth = (Depth)0.99f;
                            _gettingXPBoard.frame = (int)Math.Round(_gettingXPCompletion * 9f);
                            Graphics.Draw(_gettingXPBoard, x + 71f, y + 43f * num);
                            _boardLoader.depth = (Depth)0.995f;
                            Graphics.Draw(_boardLoader, x + 94f, y + 52f * num);
                        }
                        else if (_editingRoom)
                        {
                            _editingRoomBoard.depth = (Depth)0.99f;
                            Graphics.Draw(_editingRoomBoard, x + 71f, y + 43f * num);
                            _boardLoader.depth = (Depth)0.995f;
                            Graphics.Draw(_boardLoader, x + 94f, y + 52f * num);
                        }
                    }
                }
                if (_editingRoom)
                    fakefade = true;
            }
            if (fadeVal < 0.01f || _roomEditor._mode == REMode.Place)
                return;
            Graphics.Draw(_screen.target, position + new Vec2(3f, 3f), new Rectangle?(), new Color(_screen.darken, _screen.darken, _screen.darken) * fadeVal, 0f, Vec2.Zero, new Vec2(0.25f, 0.25f), SpriteEffects.None, (Depth)0.82f);
            _selectBorder.alpha = fadeVal;
            _selectBorder.depth = (Depth)0.85f;
            Graphics.Draw(_selectBorder, x - 1f, y, new Rectangle(0f, 0f, 4f, _selectBorder.height));
            Graphics.Draw(_selectBorder, (x - 1f + _selectBorder.width - 4f), y, new Rectangle(_selectBorder.width - 4, 0f, 4f, _selectBorder.height));
            Graphics.Draw(_selectBorder, (x - 1f + 4f), y, new Rectangle(4f, 0f, _selectBorder.width - 8, 4f));
            Graphics.Draw(_selectBorder, (x - 1f + 4f), y + (_selectBorder.height - 25), new Rectangle(4f, _selectBorder.height - 25, _selectBorder.width - 8, 25f));
            string firstWord = _firstWord;
            _font.scale = new Vec2(1f, 1f);
            _font.Draw(firstWord, x + 25f, y + 79f, new Color(163, 206, 39) * fadeVal * _lcdFlash, (Depth)0.9f);
            string secondWord = _secondWord;
            _font.scale = new Vec2(1f, 1f);
            _font.Draw(secondWord, x + 116f - _font.GetWidth(secondWord), y + 79f, new Color(163, 206, 39) * fadeVal * _lcdFlash, (Depth)0.9f);
            if (_selection == HSSelection.ChooseTeam)
            {
                _firstWord = "OK";
                _secondWord = "COLOR";
            }
            else
            {
                if (_selection != HSSelection.Main)
                    return;
                _firstWord = "PICK";
                if (_mainSelection == 1 && !Profiles.IsDefault(_profile))
                    _secondWord = "EDIT";
                else
                    _secondWord = "EXIT";
            }
        }

        public float scaleMultiplier
        {
            get
            {
                return isServerForObject ? DGRSettings.ActualHatSelectorSize : 1;
            }
        }
        public void HatsDrawLogic()
        {
            float yAdd2 = -18f;
            float xAdd2 = 0;
            float shineAdd = 0;

            float dissen = 50;

            int lowerxIndex = 0;
            int loweryIndex = 0;
            int xIndex = 7;
            int yIndex = 5;
            if (scaleMultiplier == 0.7f)
            {
                shineAdd = 0.2f;
                dissen = 100;
                xAdd2 = 29.85f;
                yAdd2 += 30;
                xIndex = 8;
            }
            else if (scaleMultiplier == 0.5f)
            {
                shineAdd = 0.2f;
                xIndex = 9;
                loweryIndex = -1;
                yIndex = 6;
                lowerxIndex = -3;
                dissen = 140;
                xAdd2 = 70;
                yAdd2 += 60;
            }


            indexedAllTeams = AllTeams();
            for (int yPos = loweryIndex; yPos < yIndex; yPos++)
            {
                for (int xPos = lowerxIndex; xPos < xIndex; xPos++)
                {
                    int plus = xPos - 3 + (yPos - 2) * 5;
                    float x = this.x + 2f + xPos * 22 + -_slide * 20f;
                    float yAdd1 = (float)(y + 37f + -_upSlide * 20f) * scaleMultiplier;
                    int index3 = TeamIndexAdd(_teamSelection, plus);
                    if (index3 == 3)
                        index3 = ControllerNumber();
                    Team allTeam = indexedAllTeams[index3];
                    float num4 = (this.x + (this.x + 2f + 154f - (this.x + 2f)) / 2f - 9f);
                    float num5 = Maths.Clamp((float)((dissen - Math.Abs(x - num4)) / dissen), 0, 1);
                    float num7 = Maths.NormalizeSection(num5, 0f, 0.1f) * 0.3f;
                    switch (yPos)
                    {
                        case 0:
                            yAdd1 -= num5 * 3f;
                            num7 = _upSlide >= 0f ? 0f : Math.Abs(_upSlide) * num7;
                            break;
                        case 1:
                            yAdd1 -= num5 * 3f;
                            if (_upSlide > 0f)
                            {
                                num7 = (1f - Math.Abs(_upSlide)) * num7;
                                break;
                            }
                            break;
                        case 2:
                            float num8 = yAdd1 - (num5 * 4f * (1f - Math.Abs(_upSlide)));
                            yAdd1 = _upSlide <= 0f ? num8 + num5 * 4f * Math.Abs(_upSlide) : num8 - num5 * 3f * Math.Abs(_upSlide);
                            num7 = Maths.NormalizeSection(num5, 0.9f, 1f) * 0.7f + num7;
                            break;
                        case 3:
                            float num9 = Math.Max(0f, _upSlide);
                            yAdd1 += (num5 * 4f * (1f - num9) + -num5 * 4f * num9);
                            if (_upSlide < 0f)
                            {
                                num7 = (1f - Math.Abs(_upSlide)) * num7;
                                break;
                            }
                            break;
                        case 4:
                            yAdd1 += num5 * 4f;
                            num7 = _upSlide <= 0f ? 0f : Math.Abs(_upSlide) * num7;
                            break;
                    }
                    num7 += shineAdd;
                    if (num7 >= 0.01f)
                    {
                        _profile.persona.sprite.alpha = _fade;
                        _profile.persona.sprite.color = Color.White;
                        _profile.persona.sprite.color = new Color(_profile.persona.sprite.color.r, _profile.persona.sprite.color.g, _profile.persona.sprite.color.b);
                        _profile.persona.sprite.depth = (Depth)0.9f;
                        _profile.persona.sprite.scale = new Vec2(1);
                        //DuckRig.GetHatPoint(_profile.persona.sprite.imageIndex);
                        SpriteMap g = allTeam.GetHat(_profile.persona);
                        Vec2 vec2 = allTeam.hatOffset;
                        bool isLocked = allTeam.locked;
                        int allTeamIndex = -1;
                        if (Network.isActive && !isServerForObject && _profile.networkHatUnlockStatuses != null)
                        {
                            allTeamIndex = Teams.core.teams.IndexOf(allTeam);
                            if (allTeamIndex >= 0 && allTeamIndex < _profile.networkHatUnlockStatuses.Count)
                                isLocked = _profile.networkHatUnlockStatuses[allTeamIndex];
                        }
                        if (isLocked)
                        {
                            g = _lock;
                            if (allTeam.name == "Chancy")
                                g = _goldLock;
                            vec2 = new Vec2(-10f, -10f);
                        }
                        g.depth = (Depth)0.95f;
                        g.alpha = _profile.persona.sprite.alpha;
                        g.color = Color.White * num7;
                        g.center = new Vec2(16f, 16f) + vec2;
                        if (index3 > DG.MaxPlayers - 1 && _fade > 0.01f)
                        {
                            Vec2 pos = new Vec2(x + xAdd2, yAdd1 + yAdd2 + yPos * 20 - 20f) * scaleMultiplier;
                            Vec2 pixel = Maths.RoundToPixel(pos);
                            if (allTeam.shake > 0)
                            {
                                pixel.x += Rando.Float(-allTeam.shake, allTeam.shake);
                                allTeam.shake -= 0.1f;
                            }
                            if (allTeamIndex != -1 && !isLocked && allTeam.locked)
                            {
                                if (_outlineMaterial == null)
                                    _outlineMaterial = new MaterialSecretOutline();
                                Graphics.material = _outlineMaterial;
                                g.scale = new Vec2(scaleMultiplier);
                                Graphics.Draw(g, pixel.x, pixel.y);
                                g.scale = new Vec2(1);
                                Graphics.material = null;
                            }
                            else
                            {
                                if (allTeam.favorited) Graphics.DrawDottedRect(pixel - new Vec2(16 * scaleMultiplier), pixel + new Vec2(16 * scaleMultiplier), Color.White, 0.95f, 1, 6 * scaleMultiplier);
                                if (allTeam.metadata != null && allTeam.metadata.UseDuckColor.value) Graphics.material = _profile.persona.material;
                                g.scale = new Vec2(scaleMultiplier);
                                Graphics.Draw(g, pixel.x, pixel.y);
                                g.scale = new Vec2(1);
                                Graphics.material = null;
                            }
                        }
                        _profile.persona.sprite.color = Color.White;
                        g.color = Color.White;
                        _profile.persona.sprite.scale = new Vec2(scaleMultiplier);
                        g.scale = new Vec2(scaleMultiplier);
                    }
                }
            }
        }
    }
}
