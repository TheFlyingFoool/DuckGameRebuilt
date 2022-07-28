// Decompiled with JetBrains decompiler
// Type: DuckGame.HatSelector
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class HatSelector : Thing, ITakeInput
    {
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
            get => this._profileBoxNumber;
            set
            {
                if (value < 0)
                    return;
                if (this._box != null)
                {
                    int controllerIndex = this._box.controllerIndex;
                }
                bool flag = _profileBoxNumber != value;
                this._profileBoxNumber = value;
                if (Network.isClient)
                {
                    this._profile = DuckNetwork.profiles[_profileBoxNumber];
                    this._inputProfile = this._profile.inputProfile;
                    if (Level.current is TeamSelect2 current)
                    {
                        this._box = current.GetBox((byte)this._profileBoxNumber);
                        this._box.SetHatSelector(this);
                    }
                    else
                        DevConsole.Log(DCSection.General, "!---CRITICAL! Profile box link failure!(" + this._profileBoxNumber.ToString() + ")---!");
                }
                if (this._profile == null || this._profile.connection != DuckNetwork.localConnection)
                    return;
                if (flag)
                    Thing.Fondle(this, DuckNetwork.localConnection);
                this.connection = DuckNetwork.localConnection;
            }
        }

        public new virtual Vec2 netPosition
        {
            get => this.position;
            set => this.position = value;
        }

        public bool flashTransition
        {
            get => this._screen._flashTransition;
            set => this._screen._flashTransition = value;
        }

        public float darken
        {
            get => this._screen._darken;
            set => this._screen._darken = value;
        }

        public float fade
        {
            get => this._fade;
            set => this._fade = value;
        }

        public bool open => this._open;

        public float fadeVal
        {
            get
            {
                if (this.fakefade)
                    return 1f;
                float fadeVal = this._fade;
                if ((double)this._profileSelector.fade > 0.0)
                    fadeVal = 1f;
                if ((double)this._roomEditor.fade > 0.0)
                    fadeVal = 1f;
                return fadeVal;
            }
        }

        public string firstWord
        {
            get => this._firstWord;
            set => this._firstWord = value;
        }

        public string secondWord
        {
            get => this._secondWord;
            set => this._secondWord = value;
        }

        public InputProfile profileInput => this._profile != null ? this._profile.inputProfile : this.inputProfile;

        public InputProfile inputProfile
        {
            get
            {
                if (Network.isActive && this.connection != DuckNetwork.localConnection)
                    return this._blankProfile;
                return this._profile != null ? this._profile.inputProfile : this._inputProfile;
            }
        }

        public Profile profile => this._profile;

        public float lcdFlash => this._lcdFlash;

        public byte selectionInt
        {
            get => (byte)this._selection;
            set => this._selection = (HSSelection)value;
        }

        public ConsoleScreen screen => this._screen;

        public ProfileBox2 box => this._box;

        public HatSelector(float xpos, float ypos, Profile profile, ProfileBox2 box)
          : base(xpos, ypos)
        {
            this._profile = profile;
            this._inputProfile = this._profile.inputProfile;
            this._box = box;
            if (Network.isServer)
                this._profileBoxNumber = (sbyte)box.controllerIndex;
            this.Construct();
        }

        public HatSelector()
          : base()
        {
            this.Construct();
        }

        public void Construct()
        {
            this._font = new BitmapFont("biosFontUI", 8, 7)
            {
                scale = new Vec2(0.5f, 0.5f)
            };
            this._collisionSize = new Vec2(141f, 89f);
            this._oButton = new Sprite("oButton");
            this._demoBox = new SpriteMap("demoCrate", 20, 20);
            this._demoBox.CenterOrigin();
            this._clueHat = new SpriteMap("hats/cluehat", 32, 32);
            this._clueHat.CenterOrigin();
            this._blind = new Sprite("blind");
            this._gettingXPBoard = new SpriteMap("gettingXP", 63, 30);
            this._gettingXPBoard.CenterOrigin();
            this._editingRoomBoard = new SpriteMap("editingRoom", 63, 30);
            this._editingRoomBoard.CenterOrigin();
            this._boardLoader = new SpriteMap("boardLoader", 7, 7);
            this._boardLoader.AddAnimation("idle", 0.2f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            this._boardLoader.CenterOrigin();
            this._boardLoader.SetAnimation("idle");
            this._selectBorder = new Sprite("selectBorder2");
            this._consoleText = new Sprite("corptronConsoleText");
            this._contextArrow = new Sprite("contextArrowRight");
            this._lock = new SpriteMap("arcade/unlockLock", 15, 18);
            this._goldLock = new SpriteMap("arcade/goldUnlockLock", 15, 18);
        }

        public void SetProfile(Profile newProfile) => this._profile = newProfile;

        public void ConfirmProfile() => this._selection = HSSelection.Main;

        public override void Initialize()
        {
            this._profileSelector = new ProfileSelector(this.x, this.y, this._box, this);
            Level.Add(_profileSelector);
            this._roomEditor = new RoomEditor(this.x, this.y, this._box, this);
            Level.Add(_roomEditor);
            this._screen = new ConsoleScreen(this.x, this.y, this);
        }

        private int ControllerNumber()
        {
            if (Network.isActive)
                return Maths.Clamp(_profileBoxNumber, 0, DG.MaxPlayers - 1);
            if (this.inputProfile.name == InputProfile.MPPlayer1)
                return 0;
            if (this.inputProfile.name == InputProfile.MPPlayer2)
                return 1;
            if (this.inputProfile.name == InputProfile.MPPlayer3)
                return 2;
            if (this.inputProfile.name == InputProfile.MPPlayer4)
                return 3;
            if (this.inputProfile.name == InputProfile.MPPlayer5)
                return 4;
            if (this.inputProfile.name == InputProfile.MPPlayer6)
                return 5;
            if (this.inputProfile.name == InputProfile.MPPlayer7)
                return 6;
            return this.inputProfile.name == InputProfile.MPPlayer8 ? 7 : 0;
        }

        private void SelectTeam()
        {
            if (_desiredTeamSelection >= this.AllTeams().Count)
                return;
            this.FilterTeam().Join(this._profile);
        }

        private Team FilterTeam(bool hardFilter = false)
        {
            this._teamWasCustomHat = false;
            this._teamName = "";
            if (Network.isActive)
            {
                if (_desiredTeamSelection >= this.AllTeams().Count)
                    this.ControllerNumber();
                Team allTeam = this.AllTeams()[_desiredTeamSelection];
                this._teamName = allTeam.name.ToUpperInvariant();
                if (Teams.core.extraTeams.Contains(allTeam))
                    this._teamWasCustomHat = true;
                return allTeam;
            }
            List<Team> teamList = this.AllTeams();
            return this._desiredTeamSelection < 0 || _desiredTeamSelection >= teamList.Count ? teamList[0] : teamList[_desiredTeamSelection];
        }

        public override void Terminate() => base.Terminate();

        public void ConfirmTeamSelection()
        {
            Team team = this.FilterTeam(true);
            if (Network.isActive && this._box.duck != null)
            {
                if (this._teamWasCustomHat)
                {
                    foreach (NetworkConnection connection in Network.connections)
                        Send.Message(new NMSpecialHat(team, this._profile, connection.profile != null && connection.profile.muteHat), connection);
                }
                Send.Message(new NMSetTeam(this._box.duck.profile, team, this._teamWasCustomHat));
            }
            if (team.hasHat)
            {
                if (this._box.duck != null)
                {
                    Hat equipment = this._box.duck.GetEquipment(typeof(Hat)) as Hat;
                    Hat hat = new TeamHat(0.0f, 0.0f, team, this._box.duck.profile);
                    Level.Add(hat);
                    this._box.duck.Equip(hat, false);
                    this._box.duck.Fondle(hat);
                    if (this.hat != null)
                        Level.Remove(this.hat);
                    this.hat = hat;
                    if (equipment != null)
                    {
                        Level.Remove(equipment);
                        if (Network.isActive)
                            Send.Message(new NMUnequip(this._box.duck, equipment), NetMessagePriority.ReliableOrdered);
                    }
                    if (Network.isActive)
                        Send.Message(new NMEquip(this._box.duck, this.hat), NetMessagePriority.ReliableOrdered);
                }
                else if (this.hat != null)
                    Level.Remove(hat);
            }
            else
            {
                if (this.hat != null)
                    Level.Remove(hat);
                this.hat = null;
                if (this._box.duck != null && this._box.duck.GetEquipment(typeof(Hat)) is Hat equipment)
                {
                    this._box.duck.Unequip(equipment);
                    Level.Remove(equipment);
                    if (Network.isActive)
                        Send.Message(new NMUnequip(this._box.duck, equipment), NetMessagePriority.ReliableOrdered);
                }
            }
            if (_desiredTeamSelection <= DG.MaxPlayers - 1 || this._box.duck == null)
                return;
            DuckNetwork.OnTeamSwitch(this._box.duck.profile);
        }

        private int TeamIndexAdd(int index, int plus, bool alwaysThree = true)
        {
            if (alwaysThree && index < DG.MaxPlayers && index >= 0)
                index = DG.MaxPlayers - 1;
            int num = index + plus;
            if (num >= this.AllTeams().Count)
                return num - this.AllTeams().Count + (DG.MaxPlayers - 1);
            return num < DG.MaxPlayers - 1 ? this.AllTeams().Count + (num - (DG.MaxPlayers - 1)) : num;
        }

        private int TeamIndexAddSpecial(int index, int plus, bool alwaysThree = true)
        {
            if (alwaysThree && index < DG.MaxPlayers && index >= 0)
                index = DG.MaxPlayers - 1;
            int num = index + plus;
            if (num >= this.AllTeams().Count)
                num = num - this.AllTeams().Count + (DG.MaxPlayers - 1);
            if (num < DG.MaxPlayers - 1)
                num = this.AllTeams().Count + (num - (DG.MaxPlayers - 1));
            if (num <= DG.MaxPlayers - 1)
                num = profileBoxNumber;
            return num;
        }

        private int GetTeamIndex(Team tm)
        {
            int teamIndex = 0;
            foreach (Team allTeam in this.AllTeams())
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
            this._netHoveringTeam = null;
            this._open = false;
            this._closing = true;
            this._selection = HSSelection.Main;
            this._mainSelection = 0;
            this._editingRoom = false;
            this._gettingXP = false;
            this._profileSelector.Reset();
            this._roomEditor.Reset();
        }

        public List<Team> AllTeams()
        {
            if (!Network.isActive)
                return Teams.all;
            if (this._profile == null)
                return Teams.core.teams;
            if (this._profile.connection != DuckNetwork.localConnection)
            {
                List<Team> teamList = new List<Team>(Teams.core.teams);
                foreach (Team customTeam in this._profile.customTeams)
                    teamList.Add(customTeam);
                return teamList;
            }
            List<Team> teamList1 = new List<Team>(Teams.core.teams);
            foreach (Team extraTeam in Teams.core.extraTeams)
                teamList1.Add(extraTeam);
            return teamList1;
        }

        public override void Update()
        {
            bool flag1 = true;
            if (this._profileBoxNumber < 0 || this.inputProfile == null || this._box == null || this._profile == null)
                return;
            if (this.connection == DuckNetwork.localConnection && this.inputProfile.Pressed("ANY"))
            {
                DuckGame.NetIndex8 authority = this.authority;
                this.authority = ++authority;
            }
            if (Network.isActive && this.connection == DuckNetwork.localConnection && Profiles.experienceProfile != null && this.profile.linkedProfile == Profiles.experienceProfile)
            {
                if (MonoMain.pauseMenu != null)
                {
                    if (MonoMain.pauseMenu is UILevelBox)
                    {
                        this._gettingXP = true;
                        UILevelBox pauseMenu = MonoMain.pauseMenu as UILevelBox;
                        this._gettingXPCompletion = (float)((pauseMenu._dayProgress + (double)pauseMenu._xpProgress) / 2.0 * 0.699999988079071);
                    }
                    else
                    {
                        this._gettingXPCompletion = 0.7f;
                        if (MonoMain.pauseMenu is UIFuneral)
                            this._gettingXPCompletion = 0.8f;
                        else if (MonoMain.pauseMenu is UIGachaBox)
                            this._gettingXPCompletion = 0.9f;
                    }
                }
                else
                {
                    this._gettingXP = false;
                    this._gettingXPCompletion = 0.0f;
                }
            }
            if (Network.isActive && (this.connection == null || this.connection.status == ConnectionStatus.Disconnected || this.profile == null || this.profile.connection == null || this.profile.connection.status == ConnectionStatus.Disconnected))
            {
                this._experienceProfileCheck = null;
                this._gettingXP = false;
                this._open = false;
            }
            this._fade = Lerp.Float(this._fade, !this._open || this._profileSelector.open || this._roomEditor.open ? 0.0f : 1f, 0.1f);
            this._blackFade = Lerp.Float(this._blackFade, this._open ? 1f : 0.0f, 0.1f);
            this._screen.Update();
            if (this._screen.transitioning)
                this._experienceProfileCheck = null;
            else if (this._profileSelector.open || this._roomEditor.open)
            {
                this._experienceProfileCheck = null;
            }
            else
            {
                if (Profiles.IsDefault(this._profile))
                    flag1 = false;
                if (Profiles.experienceProfile == null)
                    flag1 = false;
                this._editRoomDisabled = false;
                if (NetworkDebugger.enabled)
                {
                    flag1 = true;
                    this._editRoomDisabled = false;
                }
                else if (!flag1 && Network.isActive)
                {
                    flag1 = true;
                    this._editRoomDisabled = true;
                }
                if (this.isArcadeHatSelector)
                    flag1 = false;
                if (!this._open)
                {
                    if (_fade < 0.00999999977648258 && this._closing)
                    {
                        this._closing = false;
                        if (this._box != null)
                            this._box.ReturnControl();
                    }
                    this._experienceProfileCheck = null;
                }
                else if (this._profile.team == null || this._inputSkip)
                {
                    this._inputSkip = false;
                }
                else
                {
                    this._lcdFlashInc += Rando.Float(0.3f, 0.6f);
                    this._lcdFlash = (float)(0.899999976158142 + (Math.Sin(_lcdFlashInc) + 1.0) / 2.0 * 0.100000001490116);
                    if (this._prevDesiredTeam != _desiredTeamSelection && !this.isServerForObject)
                    {
                        if (this.TeamIndexAddSpecial(_desiredTeamSelection, 5) == this._prevDesiredTeam)
                            this._upSlideTo = -1f;
                        else if (this.TeamIndexAddSpecial(_desiredTeamSelection, -5) == this._prevDesiredTeam)
                            this._upSlideTo = 1f;
                        else if (this.TeamIndexAddSpecial(_desiredTeamSelection, -1) == this._prevDesiredTeam)
                            this._slideTo = 1f;
                        else if (this.TeamIndexAddSpecial(_desiredTeamSelection, 1) == this._prevDesiredTeam)
                            this._slideTo = -1f;
                        else if (this.TeamIndexAddSpecial(_desiredTeamSelection, -6) == this._prevDesiredTeam)
                        {
                            this._slideTo = 1f;
                            this._upSlideTo = 1f;
                        }
                        else if (this.TeamIndexAddSpecial(_desiredTeamSelection, -4) == this._prevDesiredTeam)
                        {
                            this._slideTo = -1f;
                            this._upSlideTo = 1f;
                        }
                        else if (this.TeamIndexAddSpecial(_desiredTeamSelection, 6) == this._prevDesiredTeam)
                        {
                            this._slideTo = -1f;
                            this._upSlideTo = -1f;
                        }
                        else if (this.TeamIndexAddSpecial(_desiredTeamSelection, 4) == this._prevDesiredTeam)
                        {
                            this._slideTo = 1f;
                            this._upSlideTo = -1f;
                        }
                        else
                            this._teamSelection = this._desiredTeamSelection;
                        SFX.Play("consoleTick", 0.6f);
                        List<Team> teamList = this.AllTeams();
                        if (_desiredTeamSelection < teamList.Count)
                        {
                            this._teamName = teamList[_desiredTeamSelection].name.ToUpperInvariant();
                            this._netHoveringTeam = teamList[_desiredTeamSelection];
                        }
                        this._prevDesiredTeam = _desiredTeamSelection;
                    }
                    if (_slideTo != 0.0 && _slide != (double)this._slideTo)
                        this._slide = Lerp.Float(this._slide, this._slideTo, 0.1f);
                    else if (_slideTo != 0.0 && _slide == (double)this._slideTo)
                    {
                        this._slide = 0.0f;
                        this._slideTo = 0.0f;
                        this._teamSelection = this._desiredTeamSelection;
                        if (this.isServerForObject)
                        {
                            Team allTeam = this.AllTeams()[_desiredTeamSelection];
                            //if (!Main.isDemo || allTeam.inDemo) that was under this
                            this.SelectTeam();
                        }
                    }
                    if (_upSlideTo != 0.0 && _upSlide != (double)this._upSlideTo)
                        this._upSlide = Lerp.Float(this._upSlide, this._upSlideTo, 0.1f);
                    else if (_upSlideTo != 0.0 && _upSlide == (double)this._upSlideTo)
                    {
                        this._upSlide = 0.0f;
                        this._upSlideTo = 0.0f;
                        this._teamSelection = this._desiredTeamSelection;
                        if (this.isServerForObject)
                        {
                            Team allTeam = this.AllTeams()[_desiredTeamSelection];
                           //if (!Main.isDemo || allTeam.inDemo) that was under this
                            this.SelectTeam();
                        }
                    }
                    if (this._selection == HSSelection.ChooseTeam)
                    {
                        if (_desiredTeamSelection == _teamSelection)
                        {
                            bool flag2 = false;
                            if (this.inputProfile.Down("MENULEFT"))
                            {
                                if (_desiredTeamSelection < DG.MaxPlayers)
                                    this._desiredTeamSelection = (short)(this.AllTeams().Count - 1);
                                else if (_desiredTeamSelection == DG.MaxPlayers)
                                    this._desiredTeamSelection = (short)this.ControllerNumber();
                                else
                                    --this._desiredTeamSelection;
                                this._slideTo = -1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (this.inputProfile.Down("MENURIGHT"))
                            {
                                if (_desiredTeamSelection >= this.AllTeams().Count - 1)
                                    this._desiredTeamSelection = (short)this.ControllerNumber();
                                else if (_desiredTeamSelection < DG.MaxPlayers)
                                    this._desiredTeamSelection = (short)DG.MaxPlayers;
                                else
                                    ++this._desiredTeamSelection;
                                this._slideTo = 1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (this.inputProfile.Down("MENUUP"))
                            {
                                if (_desiredTeamSelection < DG.MaxPlayers)
                                    this._desiredTeamSelection = 0;
                                else
                                    this._desiredTeamSelection -= (short)(DG.MaxPlayers - 1);
                                this._desiredTeamSelection -= 5;
                                if (this._desiredTeamSelection < 0)
                                    this._desiredTeamSelection += (short)(this.AllTeams().Count - (DG.MaxPlayers - 1));
                                if (this._desiredTeamSelection == 0)
                                    this._desiredTeamSelection = (short)this.ControllerNumber();
                                else
                                    this._desiredTeamSelection += (short)(DG.MaxPlayers - 1);
                                this._upSlideTo = -1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (this.inputProfile.Down("MENUDOWN"))
                            {
                                if (_desiredTeamSelection < DG.MaxPlayers)
                                    this._desiredTeamSelection = 0;
                                else
                                    this._desiredTeamSelection -= (short)(DG.MaxPlayers - 1);
                                this._desiredTeamSelection += 5;
                                if (_desiredTeamSelection >= this.AllTeams().Count - (DG.MaxPlayers - 1))
                                    this._desiredTeamSelection -= (short)(this.AllTeams().Count - (DG.MaxPlayers - 1));
                                if (this._desiredTeamSelection == 0)
                                    this._desiredTeamSelection = (short)this.ControllerNumber();
                                else
                                    this._desiredTeamSelection += (short)(DG.MaxPlayers - 1);
                                this._upSlideTo = 1f;
                                flag2 = true;
                                SFX.Play("consoleTick", 0.7f);
                            }
                            if (this.inputProfile.Pressed("SELECT") && !flag2)
                            {
                                if (this._profile.team.locked)
                                {
                                    SFX.Play("consoleError");
                                }
                                else
                                {
                                    SFX.Play("consoleSelect", 0.4f);
                                    this._selection = HSSelection.Main;
                                    this._screen.DoFlashTransition();
                                    this.ConfirmTeamSelection();
                                }
                            }
                            if (this.inputProfile.Pressed("RAGDOLL"))
                            {
                                if (this.profile.requestedColor == -1)
                                    this.profile.requestedColor = this.profile.currentColor;
                                this.profile.IncrementRequestedColor();
                                SFX.Play("consoleTick", 0.7f);
                                this.profile.UpdatePersona();
                            }
                            if (this.inputProfile.Pressed("CANCEL"))
                            {
                                this._desiredTeamSelection = (short)this.GetTeamIndex(this._startingTeam);
                                this._teamSelection = this._desiredTeamSelection;
                                this.SelectTeam();
                                this.ConfirmTeamSelection();
                                SFX.Play("consoleCancel", 0.4f);
                                this._selection = HSSelection.Main;
                                this._screen.DoFlashTransition();
                            }
                        }
                        Vec2 position = this.position;
                        this.position = Vec2.Zero;
                        this._screen.BeginDraw();
                        float num1 = -18f;
                        this._profile.persona.sprite.alpha = this._fade;
                        this._profile.persona.sprite.color = Color.White;
                        this._profile.persona.sprite.color = new Color(this._profile.persona.sprite.color.r, this._profile.persona.sprite.color.g, this._profile.persona.sprite.color.b);
                        this._profile.persona.sprite.depth = (Depth)0.9f;
                        this._profile.persona.sprite.scale = new Vec2(1f, 1f);
                        DuckGame.Graphics.Draw(_profile.persona.sprite, this.x + 70f, this.y + 60f + num1, (Depth)0.9f);
                        short num2 = 0;
                        bool flag3 = false;
                        if (_teamSelection >= this.AllTeams().Count)
                        {
                            num2 = this._teamSelection;
                            this._teamSelection = (short)(this.AllTeams().Count - 1);
                            flag3 = true;
                        }
                        int count = this.AllTeams().Count;
                        for (int index1 = 0; index1 < 5; ++index1)
                        {
                            for (int index2 = 0; index2 < 7; ++index2)
                            {
                                int plus = index2 - 3 + (index1 - 2) * 5;
                                float x = (float)((double)this.x + 2.0 + index2 * 22 + -(double)this._slide * 20.0);
                                float num3 = (float)((double)this.y + 37.0 + -(double)this._upSlide * 20.0);
                                int index3 = this.TeamIndexAdd(_teamSelection, plus);
                                if (index3 == 3)
                                    index3 = this.ControllerNumber();
                                Team allTeam = this.AllTeams()[index3];
                                float num4 = (float)((double)this.x + ((double)this.x + 2.0 + 154.0 - (double)(this.x + 2f)) / 2.0 - 9.0);
                                float num5 = Maths.Clamp((float)((50.0 - (double)Math.Abs(x - num4)) / 50.0), 0.0f, 1f);
                                float num6 = (float)((double)Maths.NormalizeSection(num5, 0.9f, 1f) * 0.800000011920929 + 0.200000002980232);
                                if ((double)num5 < 0.5)
                                    num6 = Maths.NormalizeSection(num5, 0.1f, 0.2f) * 0.3f;
                                float num7 = Maths.NormalizeSection(num5, 0.0f, 0.1f) * 0.3f;
                                switch (index1)
                                {
                                    case 0:
                                        num3 -= num5 * 3f;
                                        num7 = _upSlide >= 0.0 ? 0.0f : Math.Abs(this._upSlide) * num7;
                                        break;
                                    case 1:
                                        num3 -= num5 * 3f;
                                        if (_upSlide > 0.0)
                                        {
                                            num7 = (1f - Math.Abs(this._upSlide)) * num7;
                                            break;
                                        }
                                        break;
                                    case 2:
                                        float num8 = num3 - (float)((double)num5 * 4.0 * (1.0 - (double)Math.Abs(this._upSlide)));
                                        num3 = _upSlide <= 0.0 ? num8 + num5 * 4f * Math.Abs(this._upSlide) : num8 - num5 * 3f * Math.Abs(this._upSlide);
                                        num7 = Maths.NormalizeSection(num5, 0.9f, 1f) * 0.7f + num7;
                                        break;
                                    case 3:
                                        float num9 = Math.Max(0.0f, this._upSlide);
                                        num3 += (float)((double)num5 * 4.0 * (1.0 - (double)num9) + -(double)num5 * 4.0 * (double)num9);
                                        if (_upSlide < 0.0)
                                        {
                                            num7 = (1f - Math.Abs(this._upSlide)) * num7;
                                            break;
                                        }
                                        break;
                                    case 4:
                                        num3 += num5 * 4f;
                                        num7 = _upSlide <= 0.0 ? 0.0f : Math.Abs(this._upSlide) * num7;
                                        break;
                                }
                                if ((double)num7 >= 0.00999999977648258)
                                {
                                    this._profile.persona.sprite.alpha = this._fade;
                                    this._profile.persona.sprite.color = Color.White;
                                    this._profile.persona.sprite.color = new Color(this._profile.persona.sprite.color.r, this._profile.persona.sprite.color.g, this._profile.persona.sprite.color.b);
                                    this._profile.persona.sprite.depth = (Depth)0.9f;
                                    this._profile.persona.sprite.scale = new Vec2(1f, 1f);
                                    DuckRig.GetHatPoint(this._profile.persona.sprite.imageIndex);
                                    SpriteMap g = allTeam.GetHat(this._profile.persona);
                                    Vec2 vec2 = allTeam.hatOffset;
                                    bool flag4 = allTeam.locked;
                                    int index4 = -1;
                                    if (Network.isActive && !this.isServerForObject && this._profile.networkHatUnlockStatuses != null)
                                    {
                                        index4 = Teams.core.teams.IndexOf(allTeam);
                                        if (index4 >= 0 && index4 < this._profile.networkHatUnlockStatuses.Count)
                                            flag4 = this._profile.networkHatUnlockStatuses[index4];
                                    }
                                    if (flag4)
                                    {
                                        g = this._lock;
                                        if (allTeam.name == "Chancy")
                                            g = this._goldLock;
                                        vec2 = new Vec2(-10f, -10f);
                                    }
                                    //bool flag5 = Main.isDemo && !allTeam.inDemo;
                                    //if (flag5)
                                    //    g = this._demoBox;
                                    g.depth = (Depth)0.95f;
                                    g.alpha = this._profile.persona.sprite.alpha;
                                    g.color = Color.White * num7;
                                    g.scale = new Vec2(1f, 1f);
                                    //if (!flag5) was under
                                    g.center = new Vec2(16f, 16f) + vec2;
                                    if (index3 > DG.MaxPlayers - 1 && _fade > 0.00999999977648258)
                                    {
                                        //Vec2 pos = Vec2.Zero;
                                        Vec2 pos = new Vec2(x, (float)((double)num3 + (double)num1 + index1 * 20 - 20.0)); //!flag5 ? new Vec2(x, (float)((double)num3 + (double)num1 + index1 * 20 - 20.0)) : new Vec2(x + 2f, (float)((double)num3 + (double)num1 + index1 * 20 - 20.0 + 1.0));
                                        Vec2 pixel = Maths.RoundToPixel(pos);
                                        if (index4 != -1 && !flag4 && allTeam.locked)
                                        {
                                            if (this._outlineMaterial == null)
                                                this._outlineMaterial = new MaterialSecretOutline();
                                            DuckGame.Graphics.material = _outlineMaterial;
                                            DuckGame.Graphics.Draw(g, pixel.x, pixel.y);
                                            DuckGame.Graphics.material = null;
                                        }
                                        else
                                        {
                                            if (allTeam.metadata != null && allTeam.metadata.UseDuckColor.value)
                                                DuckGame.Graphics.material = _profile.persona.material;
                                            DuckGame.Graphics.Draw(g, pixel.x, pixel.y);
                                            DuckGame.Graphics.material = null;
                                        }
                                    }
                                    this._profile.persona.sprite.color = Color.White;
                                    g.color = Color.White;
                                    this._profile.persona.sprite.scale = new Vec2(1f, 1f);
                                    g.scale = new Vec2(1f, 1f);
                                }
                            }
                        }
                        this._font.alpha = this._fade;
                        this._font.depth = (Depth)0.96f;
                        string str1 = "NO PROFILE";
                        if (!Profiles.IsDefault(this._profile))
                            str1 = this._profile.name;
                        if (this._selection == HSSelection.ChooseProfile)
                        {
                            string str2 = "> " + str1 + " <";
                        }
                        if (this._selection == HSSelection.ChooseTeam)
                        {
                            string text = "<              >";
                            Vec2 pixel = Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)this._font.GetWidth(text) / 2.0), this.y + 60f + num1));
                            this._font.Draw(text, pixel.x, pixel.y, Color.White, (Depth)0.95f);
                        }
                        string name = this._profile.team.name;
                        string text1 = name == Teams.Player1.name || name == Teams.Player2.name || name == Teams.Player3.name || name == Teams.Player4.name || name == Teams.Player5.name || name == Teams.Player6.name || name == Teams.Player7.name || name == Teams.Player8.name ? "NO TEAM" : this._profile.team.GetNameForDisplay();
                        if (this._teamName != "")
                            text1 = this._teamName;
                        bool flag6 = this._profile.team.locked;
                        bool flag7 = false;
                        if (Network.isActive && !this.isServerForObject && this._profile.networkHatUnlockStatuses != null && _desiredTeamSelection < this._profile.networkHatUnlockStatuses.Count)
                        {
                            flag6 = this._profile.networkHatUnlockStatuses[_desiredTeamSelection];
                            flag7 = true;
                        }
                        if (flag6)
                            text1 = "LOCKED";
                        else if (flag7 && this._netHoveringTeam != null && this._netHoveringTeam.locked)
                            text1 = "UNKNOWN";
                        this._font.scale = new Vec2(1f, 1f);
                        float width = this._font.GetWidth(text1);
                        Vec2 pixel1 = Maths.RoundToPixel(new Vec2((float)((double)this.x + (double)this.width / 2.0 - (double)width / 2.0), this.y + 25f + num1));
                        this._font.Draw(text1, pixel1.x, pixel1.y, Color.LimeGreen * (this._selection == HSSelection.ChooseTeam ? 1f : 0.6f), (Depth)0.95f);
                        DuckGame.Graphics.DrawLine(pixel1 + new Vec2(-10f, 4f), pixel1 + new Vec2(width + 10f, 4f), Color.White * 0.1f, 2f, (Depth)0.93f);
                        this._font.Draw("@SELECT@", this.x + 4f, this.y + 79f, new Color(180, 180, 180), (Depth)0.95f, this.profileInput);
                        this._font.Draw("@RAGDOLL@", this.x + 122f, this.y + 79f, new Color(180, 180, 180), (Depth)0.95f, this.profileInput);
                        this._screen.EndDraw();
                        this.position = position;
                        if (!flag3)
                            return;
                        this._teamSelection = num2;
                    }
                    else
                    {
                        if (this._selection != HSSelection.Main)
                            return;
                        if (Level.current is ArcadeLevel && !Options.Data.defaultAccountMerged)
                        {
                            if (this._experienceProfileCheck != this._profile)
                            {
                                if (this._profile == Profiles.experienceProfile)
                                    HUD.AddCornerControl(HUDCorner.BottomLeft, "@MENU2@MERGE DEFAULT", this.inputProfile);
                                else
                                    HUD.CloseCorner(HUDCorner.BottomLeft);
                                this._experienceProfileCheck = this._profile;
                            }
                            if (this._profile == Profiles.experienceProfile && this.inputProfile.Pressed("MENU2"))
                            {
                                UIMenu profileMergeMenu = Options.CreateProfileMergeMenu();
                                Level.Add(profileMergeMenu);
                                MonoMain.pauseMenu = profileMergeMenu;
                                profileMergeMenu.Open();
                            }
                        }
                        if (this.inputProfile.Pressed("MENUUP"))
                        {
                            if (this._mainSelection > 0)
                            {
                                --this._mainSelection;
                                SFX.Play("consoleTick");
                                if (this._editRoomDisabled && this._mainSelection == 2)
                                    this._mainSelection = 1;
                            }
                        }
                        else if (this.inputProfile.Pressed("MENUDOWN"))
                        {
                            if (_mainSelection < (flag1 ? 3 : 2))
                            {
                                ++this._mainSelection;
                                SFX.Play("consoleTick");
                            }
                            if (this._editRoomDisabled && this._mainSelection == 2)
                                this._mainSelection = 3;
                        }
                        else if (this.inputProfile.Pressed("SELECT"))
                        {
                            if (this._mainSelection == 1 && (!Network.isActive || !Profiles.IsExperience(this._profile)))
                            {
                                this._profileSelector.Open(this._profile);
                                SFX.Play("consoleSelect", 0.4f);
                                this._fade = 0.0f;
                                this._screen.DoFlashTransition();
                            }
                            else if (this._mainSelection == 0)
                            {
                                this._selection = HSSelection.ChooseTeam;
                                SFX.Play("consoleSelect", 0.4f);
                                this._screen.DoFlashTransition();
                            }
                            else if (_mainSelection == (flag1 ? 3 : 2))
                            {
                                this._open = false;
                                this._closing = true;
                                SFX.Play("consoleCancel", 0.4f);
                                this._selection = HSSelection.Main;
                            }
                            else if (flag1 && this._mainSelection == 2)
                            {
                                this._editingRoom = true;
                                this._roomEditor.Open(this._profile);
                                SFX.Play("consoleSelect", 0.4f);
                                this._fade = 0.0f;
                                this._screen.DoFlashTransition();
                            }
                        }
                        else if (this._mainSelection == 1 && this.inputProfile.Pressed("MENU1") && !Profiles.IsDefault(this._profile))
                        {
                            this._profileSelector.EditProfile(this._profile);
                            SFX.Play("consoleSelect", 0.4f);
                            this._fade = 0.0f;
                            this._screen.DoFlashTransition();
                        }
                        else if (this.inputProfile.Pressed("CANCEL"))
                        {
                            this._open = false;
                            this._closing = true;
                            SFX.Play("consoleCancel", 0.4f);
                            this._selection = HSSelection.Main;
                        }
                        this._screen.BeginDraw();
                        this._font.scale = new Vec2(1f, 1f);
                        string text2 = "@LWING@CUSTOM DUCK@RWING@";
                        this._font.Draw(text2, Maths.RoundToPixel(new Vec2((float)((double)this.width / 2.0 - (double)this._font.GetWidth(text2) / 2.0), 10f)), Color.White, (Depth)0.95f);
                        string text3 = !Profiles.IsDefault(this._profile) ? this._profile.name : "PICK PROFILE";
                        Vec2 pixel2 = Maths.RoundToPixel(new Vec2((float)((double)this.width / 2.0 - (double)this._font.GetWidth(text3) / 2.0), 39f));
                        this._font.Draw(text3, pixel2, Colors.MenuOption * (this._mainSelection == 1 ? 1f : 0.6f), (Depth)0.95f);
                        if (this._mainSelection == 1)
                            DuckGame.Graphics.Draw(this._contextArrow, pixel2.x - 8f, pixel2.y);
                        if (flag1)
                        {
                            string text4 = "@RAINBOWICON@EDIT ROOM";
                            Vec2 pixel3 = Maths.RoundToPixel(new Vec2((float)((double)this.width / 2.0 - (double)this._font.GetWidth(text4) / 2.0), 48f));
                            this._font.Draw(text4, pixel3, this._editRoomDisabled ? Colors.SuperDarkBlueGray : Colors.MenuOption * (this._mainSelection == 2 ? 1f : 0.6f), (Depth)0.95f, colorSymbols: true);
                            if (this._mainSelection == 2)
                                DuckGame.Graphics.Draw(this._contextArrow, pixel3.x - 8f, pixel3.y);
                        }
                        string text5 = this._profile.team.hasHat ? "|LIME|" + this._profile.team.GetNameForDisplay() + "|MENUORANGE| HAT" : "|MENUORANGE|CHOOSE HAT";
                        Vec2 pixel4 = Maths.RoundToPixel(new Vec2((float)((double)this.width / 2.0 - (double)this._font.GetWidth(text5) / 2.0), 30f));
                        this._font.Draw(text5, pixel4, Color.White * (this._mainSelection == 0 ? 1f : 0.6f), (Depth)0.95f);
                        if (this._mainSelection == 0)
                            DuckGame.Graphics.Draw(this._contextArrow, pixel4.x - 8f, pixel4.y);
                        string text6 = "EXIT";
                        Vec2 pixel5 = Maths.RoundToPixel(new Vec2((float)((double)this.width / 2.0 - (double)this._font.GetWidth(text6) / 2.0), 50 + (flag1 ? 12 : 9)));
                        this._font.Draw(text6, pixel5, Colors.MenuOption * (_mainSelection == (flag1 ? 3 : 2) ? 1f : 0.6f), (Depth)0.95f);
                        if (_mainSelection == (flag1 ? 3 : 2))
                            DuckGame.Graphics.Draw(this._contextArrow, pixel5.x - 8f, pixel5.y);
                        this._font.Draw("@SELECT@", 4f, 79f, new Color(180, 180, 180), (Depth)0.95f, this.profileInput);
                        this._font.Draw(this._mainSelection != 1 || Profiles.IsDefault(this._profile) ? "@CANCEL@" : "@MENU1@", 122f, 79f, new Color(180, 180, 180), (Depth)0.95f, this.profileInput);
                        this._consoleText.color = new Color(140, 140, 140);
                        DuckGame.Graphics.Draw(this._consoleText, 30f, 18f);
                        this._screen.EndDraw();
                    }
                }
            }
        }

        public void Open(Profile p)
        {
            this._profile = p;
            this._startingTeam = this._profile.team;
            this._open = true;
            this._mainSelection = 0;
            this._editingRoom = false;
            this._gettingXP = false;
            this._selection = HSSelection.Main;
            this._teamSelection = this._desiredTeamSelection = (short)this.GetTeamIndex(this._profile.team);
            this._inputSkip = true;
        }

        public override void Draw()
        {
            if (this._profileBoxNumber < 0 || this._box == null)
                return;
            this.fakefade = false;
            if (Network.isActive && this._box.profile != null && this._box.profile.connection != DuckNetwork.localConnection)
            {
                this._blindLerp = Lerp.Float(this._blindLerp, this._editingRoom || this._gettingXP ? 1f : 0.0f, 0.05f);
                if (_blindLerp > 0.00999999977648258)
                {
                    for (int index = 0; index < 8; ++index)
                    {
                        this._blind.yscale = Math.Max(0.0f, Math.Min((float)(_blindLerp * 3.0 - index * 0.0500000007450581), 1f));
                        this._blind.depth = (Depth)(float)(0.910000026226044 + index * 0.00800000037997961);
                        this._blind.flipH = false;
                        DuckGame.Graphics.Draw(this._blind, (float)((double)this.x - 3.0 + index * (9.0 * _blindLerp)), this.y + 1f);
                        this._blind.flipH = true;
                        DuckGame.Graphics.Draw(this._blind, (float)((double)this.x + 4.0 + 140.0 - index * (9.0 * _blindLerp)), this.y + 1f);
                    }
                    float num = Math.Max((float)((_blindLerp - 0.5) * 2.0), 0.0f);
                    if ((double)num > 0.00999999977648258)
                    {
                        if (this._gettingXP)
                        {
                            this._gettingXPBoard.depth = (Depth)0.99f;
                            this._gettingXPBoard.frame = (int)Math.Round(_gettingXPCompletion * 9.0);
                            DuckGame.Graphics.Draw(_gettingXPBoard, this.x + 71f, this.y + 43f * num);
                            this._boardLoader.depth = (Depth)0.995f;
                            DuckGame.Graphics.Draw(_boardLoader, this.x + 94f, this.y + 52f * num);
                        }
                        else if (this._editingRoom)
                        {
                            this._editingRoomBoard.depth = (Depth)0.99f;
                            DuckGame.Graphics.Draw(_editingRoomBoard, this.x + 71f, this.y + 43f * num);
                            this._boardLoader.depth = (Depth)0.995f;
                            DuckGame.Graphics.Draw(_boardLoader, this.x + 94f, this.y + 52f * num);
                        }
                    }
                }
                if (this._editingRoom)
                    this.fakefade = true;
            }
            if ((double)this.fadeVal < 0.00999999977648258 || this._roomEditor._mode == REMode.Place)
                return;
            DuckGame.Graphics.Draw(_screen.target, this.position + new Vec2(3f, 3f), new Rectangle?(), new Color(this._screen.darken, this._screen.darken, this._screen.darken) * this.fadeVal, 0.0f, Vec2.Zero, new Vec2(0.25f, 0.25f), SpriteEffects.None, (Depth)0.82f);
            this._selectBorder.alpha = this.fadeVal;
            this._selectBorder.depth = (Depth)0.85f;
            DuckGame.Graphics.Draw(this._selectBorder, this.x - 1f, this.y, new Rectangle(0.0f, 0.0f, 4f, _selectBorder.height));
            DuckGame.Graphics.Draw(this._selectBorder, (float)((double)this.x - 1.0 + _selectBorder.width - 4.0), this.y, new Rectangle(this._selectBorder.width - 4, 0.0f, 4f, _selectBorder.height));
            DuckGame.Graphics.Draw(this._selectBorder, (float)((double)this.x - 1.0 + 4.0), this.y, new Rectangle(4f, 0.0f, this._selectBorder.width - 8, 4f));
            DuckGame.Graphics.Draw(this._selectBorder, (float)((double)this.x - 1.0 + 4.0), this.y + (this._selectBorder.height - 25), new Rectangle(4f, this._selectBorder.height - 25, this._selectBorder.width - 8, 25f));
            string firstWord = this._firstWord;
            this._font.scale = new Vec2(1f, 1f);
            this._font.Draw(firstWord, this.x + 25f, this.y + 79f, new Color(163, 206, 39) * this.fadeVal * this._lcdFlash, (Depth)0.9f);
            string secondWord = this._secondWord;
            this._font.scale = new Vec2(1f, 1f);
            this._font.Draw(secondWord, this.x + 116f - this._font.GetWidth(secondWord), this.y + 79f, new Color(163, 206, 39) * this.fadeVal * this._lcdFlash, (Depth)0.9f);
            if (this._selection == HSSelection.ChooseTeam)
            {
                this._firstWord = "OK";
                this._secondWord = "COLOR";
            }
            else
            {
                if (this._selection != HSSelection.Main)
                    return;
                this._firstWord = "PICK";
                if (this._mainSelection == 1 && !Profiles.IsDefault(this._profile))
                    this._secondWord = "EDIT";
                else
                    this._secondWord = "EXIT";
            }
        }
    }
}
