// Decompiled with JetBrains decompiler
// Type: DuckGame.ProfileBox2
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class ProfileBox2 : Thing
    {
        private BitmapFont _font;
        private BitmapFont _fontSmall;
        //private SinWave _pulse = new SinWave(0.05f);
        private bool _playerActive;
        private int _teamSelection;
        //private Sprite _plaque;
        private Sprite _onlineIcon;
        //private Sprite _wirelessIcon;
        private bool _ready;
        private InputProfile _inputProfile;
        private Profile _playerProfile;
        private Sprite _doorLeft;
        private Sprite _doorRight;
        private Sprite _doorLeftBlank;
        private Sprite _doorRightBlank;
        private SpriteMap _doorSpinner;
        private SpriteMap _doorIcon;
        private Sprite _roomLeftBackground;
        private Sprite _roomLeftForeground;
        private SpriteMap _tutorialMessages;
        private Sprite _tutorialTV;
        private SpriteMap _selectConsole;
        private Sprite _consoleHighlight;
        //private Sprite _avatar;
        //private object avatarUser;
        private Sprite _aButton;
        private Sprite _readySign;
        public float _doorX;
        private int _currentMessage;
        private float _screenFade;
        private float _consoleFade;
        private Vec2 _consolePos = Vec2.Zero;
        private TeamProjector _projector;
        private TeamSelect2 _teamSelect;
        private Profile _defaultProfile;
        private Sprite _hostCrown;
        private Sprite _consoleFlash;
        private SpriteMap _lightBar;
        private SpriteMap _roomSwitch;
        private int _controllerIndex;
        private Duck _duck;
        private VirtualShotgun _gun;
        private RoomDefenceTurret _turret;
        //private bool _doorClosing;
        private Window _window;
        private Vec2 _gunSpawnPoint = Vec2.Zero;
        //private bool findDuck;
        private int hostFrames;
        public HatSelector _hatSelector;
        private DuckNetStatus _prevStatus = DuckNetStatus.EstablishingCommunicationWithServer;
        //private const int kDoorIconStartButtonFrame = 10;
        public float _tooManyPulse;
        public float _noMorePulse;
        private float _prevDoorX;

        public bool playerActive => this._playerActive;

        public bool ready => this._ready;

        public bool doorIsOpen => _doorX > 82.0;

        public Profile profile => this._playerProfile;

        public void SetProfile(Profile p)
        {
            if (p == null)
            {
                this.SetProfile(Profiles.all.ElementAt<Profile>(this.controllerIndex));
            }
            else
            {
                this._playerProfile = p;
                if (this._duck != null)
                    this._duck.profile = p;
                if (this._projector != null)
                    this._projector.SetProfile(p);
                if (this._hatSelector == null)
                    return;
                this._hatSelector.SetProfile(p);
            }
        }

        public void SetHatSelector(HatSelector s) => this._hatSelector = s;

        public int controllerIndex => this._controllerIndex;

        public ProfileBox2(
          float xpos,
          float ypos,
          InputProfile pProfile,
          Profile pDefaultProfile,
          TeamSelect2 pTeamSelect,
          int pIndex)
          : base(xpos, ypos)
        {
            this._hostCrown = new Sprite("hostCrown");
            this._hostCrown.CenterOrigin();
            this._lightBar = new SpriteMap("lightBar", 2, 1)
            {
                frame = 0
            };
            this._roomSwitch = new SpriteMap("roomSwitch", 7, 5)
            {
                frame = 0
            };
            this._controllerIndex = pIndex;
            this._font = new BitmapFont("biosFont", 8);
            this._fontSmall = new BitmapFont("smallBiosFont", 7, 6);
            this.layer = Layer.Game;
            this._collisionSize = new Vec2(150f, 87f);
            //this._plaque = new Sprite("plaque")
            //{
            //    center = new Vec2(16f, 16f)
            //};
            this._inputProfile = pProfile;
            this._playerProfile = pDefaultProfile;
            this._teamSelection = this.ControllerNumber();
            this._doorLeft = new Sprite("selectDoorLeftPC")
            {
                depth = (Depth)0.905f
            };
            this._doorRight = new Sprite("selectDoorRight")
            {
                depth = (Depth)0.9f
            };
            this._doorLeftBlank = new Sprite("selectDoorLeftBlank")
            {
                depth = (Depth)0.905f
            };
            this._doorRightBlank = new Sprite("selectDoorRightBlank")
            {
                depth = (Depth)0.9f
            };
            this._doorSpinner = new SpriteMap("doorSpinner", 25, 25);
            this._doorSpinner.AddAnimation("spin", 0.2f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            this._doorSpinner.SetAnimation("spin");
            this._doorIcon = new SpriteMap("doorSpinner", 25, 25);
            this._onlineIcon = new Sprite("gameServerOnline");
            //this._wirelessIcon = new Sprite("gameServerWireless");
            this._teamSelect = pTeamSelect;
            this._defaultProfile = pDefaultProfile;
            if (this.rightRoom)
            {
                this._roomSwitch = new SpriteMap("roomSwitchRight", 7, 5)
                {
                    frame = 0
                };
                this._roomLeftBackground = new Sprite("rightRoomBackground");
                this._roomLeftForeground = new Sprite("rightRoomForeground");
                Level.Add(new InvisibleBlock((float)((double)this.x - 2.0 + 142.0 - 138.0), this.y + 69f, 138f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)((double)this.x - 2.0 + 142.0 - 138.0), this.y - 11f, 138f, 12f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)((double)this.x + 142.0 - 98.0 - 46.0), this.y + 56f, 50f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)((double)this.x + 142.0 + 2.0 - 8.0), this.y, 8f, 100f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)((double)this.x + 142.0 - 136.0 - 9.0), this.y, 8f, 25f, PhysicsMaterial.Metal));
                ScaffoldingTileset scaffoldingTileset = new ScaffoldingTileset(this.x + 126f, this.y + 63f)
                {
                    neverCheap = true
                };
                Level.Add(scaffoldingTileset);
                scaffoldingTileset.depth = - 0.5f;
                scaffoldingTileset.PlaceBlock();
                scaffoldingTileset.UpdateNubbers();
                Level.Add(new Platform(this.x + 49f, this.y + 56f, 3f, 5f));
                this._readySign = new Sprite("readyLeft");
            }
            else
            {
                this._roomLeftBackground = new Sprite("leftRoomBackground");
                this._roomLeftForeground = new Sprite("leftRoomForeground");
                Level.Add(new InvisibleBlock(this.x + 2f, this.y + 69f, 138f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(this.x + 2f, this.y - 11f, 138f, 12f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(this.x + 92f, this.y + 56f, 50f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(this.x - 4f, this.y, 8f, 100f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(this.x + 135f, this.y, 8f, 25f, PhysicsMaterial.Metal));
                ScaffoldingTileset scaffoldingTileset = new ScaffoldingTileset(this.x + 14f, this.y + 63f)
                {
                    neverCheap = true
                };
                Level.Add(scaffoldingTileset);
                scaffoldingTileset.depth = - 0.5f;
                scaffoldingTileset.PlaceBlock();
                scaffoldingTileset.UpdateNubbers();
                Level.Add(new Platform(this.x + 89f, this.y + 56f, 3f, 5f));
                this._readySign = new Sprite("readyRight");
            }
            this._gunSpawnPoint = !this.rightRoom ? new Vec2(this.x + 113f, this.y + 50f) : new Vec2((float)((double)this.x + 142.0 - 118.0), this.y + 50f);
            this._readySign.depth = (Depth)0.2f;
            this._roomLeftBackground.depth = - 0.85f;
            this._roomLeftForeground.depth = (Depth)0.1f;
            this._tutorialMessages = new SpriteMap("tutorialScreensPC", 53, 30);
            this._aButton = new Sprite("aButton");
            this._tutorialTV = new Sprite("tutorialTV");
            this._consoleHighlight = new Sprite("consoleHighlight");
            this._consoleFlash = new Sprite("consoleFlash");
            this._consoleFlash.CenterOrigin();
            this._selectConsole = new SpriteMap("selectConsole", 20, 19);
            this._selectConsole.AddAnimation("idle", 1f, true, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            this._selectConsole.SetAnimation("idle");
            if (Network.isServer)
            {
                this._hatSelector = new HatSelector(this.x, this.y, this._playerProfile, this)
                {
                    profileBoxNumber = (sbyte)pIndex
                };
                Level.Add(_hatSelector);
            }
            if (this.rightRoom)
            {
                this._projector = new TeamProjector(this.x + 80f, this.y + 68f, this._playerProfile);
                Level.Add(new ItemSpawner(this.x + 26f, this.y + 54f));
            }
            else
            {
                this._projector = new TeamProjector(this.x + 59f, this.y + 68f, this._playerProfile);
                Level.Add(new ItemSpawner(this.x + 112f, this.y + 54f));
            }
            Level.Add(_projector);
        }

        public override void Initialize()
        {
            if (Network.isServer && this._playerProfile != null && this._playerProfile.connection != null && Network.isActive)
                this.Spawn();
            base.Initialize();
        }

        public void ReturnControl()
        {
            if (this._duck == null)
                return;
            this._duck.immobilized = false;
        }

        public bool rightRoom => this.ControllerNumber() == 1 || this.ControllerNumber() == 3 || this.ControllerNumber() == 4 || this.ControllerNumber() == 5 || this.ControllerNumber() == 7;

        private int ControllerNumber() => this._controllerIndex;

        private void SelectTeam()
        {
            Teams.all[this._teamSelection].Join(this._playerProfile);
            if (this._playerProfile.inputProfile != null)
                return;
            this._playerProfile.inputProfile = this._inputProfile;
        }

        public void ChangeProfile(Profile p)
        {
            if (p == null)
                p = this._defaultProfile;
            if (p != this._playerProfile)
            {
                if (!Network.isActive && !p.isNetworkProfile)
                {
                    for (int index = 0; index < Profile.defaultProfileMappings.Count; ++index)
                    {
                        if (Profile.defaultProfileMappings[index] == p)
                            Profile.defaultProfileMappings[index] = Profiles.universalProfileList.ElementAt<Profile>(index);
                    }
                    Profile.defaultProfileMappings[this._controllerIndex] = p;
                    if (this._teamSelect != null)
                    {
                        foreach (ProfileBox2 profile in this._teamSelect._profiles)
                        {
                            if (profile != this && profile.profile == p)
                                profile.SetProfile(Profile.defaultProfileMappings[profile._controllerIndex]);
                        }
                    }
                }
                Team team1 = this._playerProfile.team;
                if (team1 != null)
                {
                    this._playerProfile.team.Leave(this._playerProfile);
                    team1.Join(p);
                }
                if (!Network.isActive)
                    p.inputProfile = this._playerProfile.inputProfile;
                p.UpdatePersona();
                if (!Network.isActive)
                    this._playerProfile.inputProfile = null;
                this._playerProfile = p;
                if (this._duck != null)
                {
                    if (this._duck.profile.team != null)
                    {
                        Team team2 = this._duck.profile.team;
                        team2.Leave(this._duck.profile);
                        team2.Join(this._playerProfile);
                    }
                    this._duck.profile = this._playerProfile;
                    if (Network.isActive && DuckNetwork.IndexOf(this._playerProfile) >= 0)
                        this._duck.netProfileIndex = (byte)DuckNetwork.IndexOf(this._playerProfile);
                }
                this._projector.SetProfile(p);
                this._hatSelector.SetProfile(p);
            }
            this.OpenCorners();
        }

        public void OpenCorners()
        {
            if (!(Level.current is ArcadeLevel))
                return;
            HUD.CloseAllCorners();
            HUD.AddCornerCounter(HUDCorner.BottomRight, "@TICKET@ ", new FieldBinding(_playerProfile, "ticketCount"), animateCount: true);
            List<ChallengeSaveData> allSaveData = Challenges.GetAllSaveData(this._playerProfile);
            Dictionary<TrophyType, int> dictionary = new Dictionary<TrophyType, int>()
      {
        {
          TrophyType.Bronze,
          0
        },
        {
          TrophyType.Silver,
          0
        },
        {
          TrophyType.Gold,
          0
        },
        {
          TrophyType.Platinum,
          0
        },
        {
          TrophyType.Developer,
          0
        }
      };
            foreach (ChallengeSaveData challengeSaveData in allSaveData)
            {
                if (challengeSaveData.trophy != TrophyType.Baseline)
                    ++dictionary[challengeSaveData.trophy];
            }
            string text = "";
            int num;
            if (dictionary[TrophyType.Bronze] > 0 || dictionary[TrophyType.Silver] == 0 && dictionary[TrophyType.Gold] == 0 && dictionary[TrophyType.Platinum] == 0 && dictionary[TrophyType.Developer] == 0)
            {
                string str1 = text;
                num = dictionary[TrophyType.Bronze];
                string str2 = num.ToString();
                text = str1 + "@BRONZE@" + str2;
            }
            if (dictionary[TrophyType.Silver] > 0)
            {
                string str3 = text;
                num = dictionary[TrophyType.Silver];
                string str4 = num.ToString();
                text = str3 + " @SILVER@" + str4;
            }
            if (dictionary[TrophyType.Gold] > 0)
            {
                string str5 = text;
                num = dictionary[TrophyType.Gold];
                string str6 = num.ToString();
                text = str5 + " @GOLD@" + str6;
            }
            if (dictionary[TrophyType.Platinum] > 0)
            {
                string str7 = text;
                num = dictionary[TrophyType.Platinum];
                string str8 = num.ToString();
                text = str7 + " @PLATINUM@" + str8;
            }
            if (dictionary[TrophyType.Developer] > 0)
            {
                string str9 = text;
                num = dictionary[TrophyType.Developer];
                string str10 = num.ToString();
                text = str9 + " @DEVELOPER@" + str10;
            }
            HUD.AddCornerControl(HUDCorner.TopRight, text);
        }

        public Duck duck
        {
            get => this._duck;
            set => this._duck = value;
        }

        public VirtualShotgun gun
        {
            get => this._gun;
            set => this._gun = value;
        }

        public RoomDefenceTurret turret
        {
            get => this._turret;
            set => this._turret = value;
        }

        public void CloseDoor()
        {
            if (this._duck != null)
                this._duck.immobilized = true;
            this._playerActive = false;
            if (_doorX != 0.0)
                return;
            this.OnDoorClosed();
        }

        public void OnDoorClosed()
        {
            //this._doorClosing = true;
            if (Network.isServer)
            {
                if (this._playerProfile.team != null)
                    this._playerProfile.team.Leave(this._playerProfile);
                this._playerProfile = Profiles.defaultProfiles[this.ControllerNumber()];
                this._teamSelection = this.ControllerNumber();
                this.SelectTeam();
                this._playerProfile.team.Leave(this._playerProfile);
                if (this._duck != null)
                {
                    this._duck.profile = this._playerProfile;
                    if (this._duck.GetEquipment(typeof(Hat)) is Hat equipment)
                    {
                        this._duck.Unequip(equipment);
                        Level.Remove(equipment);
                    }
                }
                foreach (Thing thing in Level.CheckRectAll<RoomDefenceTurret>(this.topLeft, this.bottomRight))
                    Level.Remove(thing);
                this._turret = null;
            }
            this.Despawn();
            //this._doorClosing = false;
        }

        public void Spawn()
        {
            this.profile.UpdatePersona();
            if (this._duck != null)
            {
                this._teamSelection = this.ControllerNumber();
                this.SelectTeam();
                this.ReturnControl();
            }
            else
            {
                this._gun = new VirtualShotgun(this._gunSpawnPoint.x, this._gunSpawnPoint.y)
                {
                    roomIndex = (byte)this._controllerIndex
                };
                Level.Add(_gun);
                if (this.rightRoom)
                {
                    this._duck = new Duck((float)((double)this.x + 142.0 - 48.0), this.y + 40f, this._playerProfile);
                    this._window = new Window((float)((double)this.x + 142.0 - 141.0), this.y + 49f)
                    {
                        noframe = true
                    };
                    Level.Add(_window);
                }
                else
                {
                    this._duck = new Duck(this.x + 48f, this.y + 40f, this._playerProfile);
                    this._window = new Window(this.x + 139f, this.y + 49f)
                    {
                        noframe = true
                    };
                    Level.Add(_window);
                }
                foreach (Thing thing in Level.CheckRectAll<RoomDefenceTurret>(this.topLeft, this.bottomRight))
                    Level.Remove(thing);
                this._turret = null;
                Level.Add(_duck);
                if (this._duck == null || !this._duck.HasEquipment(typeof(TeamHat)))
                    return;
                this._hatSelector.hat = this._duck.GetEquipment(typeof(TeamHat)) as TeamHat;
            }
        }

        public void Despawn()
        {
            if (!Network.isServer)
                return;
            if (this._duck != null)
            {
                Thing.Fondle(_duck, DuckNetwork.localConnection);
                Level.Remove(_duck);
                if (!Network.isActive && this._duck.ragdoll != null)
                    Level.Remove(_duck.ragdoll);
            }
            if (this._gun != null)
            {
                Thing.Fondle(_gun, DuckNetwork.localConnection);
                Level.Remove(_gun);
            }
            foreach (Window t in Level.CheckRectAll<Window>(this.topLeft, this.bottomRight))
            {
                t.lobbyRemoving = true;
                Thing.Fondle(t, DuckNetwork.localConnection);
                Level.Remove(t);
            }
            this._window = null;
            this._duck = null;
            this._gun = null;
        }

        public void OpenDoor()
        {
            this._playerActive = true;
            this.SelectTeam();
            if (this._duck == null)
                return;
            this._duck.immobilized = false;
        }

        public void PrepareDoor()
        {
            DevConsole.Log(DCSection.DuckNet, "|DGGREEN|Preparing Door..");
            if (!Network.isServer)
                return;
            if (this._duck == null)
            {
                DevConsole.Log(DCSection.DuckNet, "|DGGREEN|Duckspawn!");
                this.Spawn();
            }
            else
                DevConsole.Log(DCSection.DuckNet, "|DGRED|Duck was not NULL!");
        }

        public void OpenDoor(Duck d) => this._duck = d;

        public override void Update()
        {
            if (Network.isActive && Network.isServer && this._duck != null && this.profile.connection == null)
                this.Despawn();
            if (Network.isServer && Network.isActive && this._hatSelector != null && this._hatSelector.isServerForObject)
                this._hatSelector.profileBoxNumber = (sbyte)this._controllerIndex;
            if (this.hostFrames > 0)
            {
                --this.hostFrames;
                if (this.hostFrames == 0)
                {
                    TeamSelect2.FillMatchmakingProfiles();
                    if (NetworkDebugger.CurrentServerIndex() < 0)
                    {
                        Network.lanMode = true;
                        DuckNetwork.Host(4, NetworkLobbyType.LAN);
                        (this.level as TeamSelect2).NetworkDebuggerPrepare();
                    }
                    else
                    {
                        Network.lanMode = true;
                        DuckNetwork.Join("test", "netdebug");
                        Level.current = new ConnectingScreen();
                    }
                }
            }
            if (Network.isActive)
            {
                if (!Network.isServer && this.profile.networkStatus != DuckNetStatus.Disconnected)
                {
                    this._duck = null;
                    foreach (Duck duck in Level.current.things[typeof(Duck)])
                    {
                        if (duck.netProfileIndex == this._controllerIndex)
                            this._duck = duck;
                    }
                }
                this._playerActive = this.profile.networkStatus == DuckNetStatus.Connected;
            }
            if (this._duck != null && this._duck.inputProfile != null)
                this._inputProfile = this._duck.inputProfile;
            if (this._hatSelector == null)
                return;
            if (this._hatSelector.open && this.profile.team == null)
                this._hatSelector.Reset();
            foreach (VirtualShotgun virtualShotgun in Level.current.things[typeof(VirtualShotgun)])
            {
                if (virtualShotgun.roomIndex == this._controllerIndex && virtualShotgun.isServerForObject && (double)virtualShotgun.alpha <= 0.0)
                {
                    virtualShotgun.position = this._gunSpawnPoint;
                    virtualShotgun.alpha = 1f;
                    virtualShotgun.vSpeed = -1f;
                }
            }
            bool flag1 = false;
            if (this._teamSelect != null && (!Network.isActive || this._hatSelector.connection == DuckNetwork.localConnection) && !Network.isActive && this._inputProfile.JoinGamePressed() && !this._hatSelector.open && (!NetworkDebugger.enabled || NetworkDebugger._instances[NetworkDebugger.currentIndex].hover && (Input.Down("SHOOT") || Keyboard.Down(Keys.LeftShift) || Keyboard.Down(Keys.RightShift)) || NetworkDebugger.letJoin))
            {
                if (!this._playerActive)
                {
                    this.OpenDoor();
                    flag1 = true;
                }
                if (NetworkDebugger.letJoin && !Input.Down("SHOOT") && !Keyboard.Down(Keys.LeftShift) && !Keyboard.Down(Keys.RightShift))
                {
                    NetworkDebugger.letJoin = false;
                    this.hostFrames = 2;
                }
            }
            if (this._teamSelect != null && !this.ready && !Network.isActive && this._inputProfile.Pressed("START") && !flag1)
                this._teamSelect.OpenPauseMenu(this);
            if (!Network.isActive && this._duck != null && !this._duck.immobilized)
                this._playerActive = true;
            if (Network.isServer && this._duck == null && this._playerProfile.team != null && (!Network.isActive || this._playerProfile.connection != null))
            {
                int num = 0;
                foreach (Team team in Teams.all)
                {
                    if (!(team.name == this._playerProfile.team.name))
                        ++num;
                    else
                        break;
                }
                this._teamSelection = Teams.all.IndexOf(this._playerProfile.team);
                this._playerActive = true;
                this.SelectTeam();
                this.Spawn();
            }
            this._ready = this.doorIsOpen && this._duck != null && (this._duck.dead || this._duck.beammode || _duck.cameraPosition.y < -100.0 || _duck.cameraPosition.y > 400.0);
            if (this._duck != null)
            {
                this._currentMessage = 0;
                Vec2 vec2 = this._duck.position - this._consolePos;
                bool flag2 = (double)vec2.length < 20.0;
                this._consoleFade = Lerp.Float(this._consoleFade, flag2 ? 1f : 0.0f, 0.1f);
                if (this._teamSelect != null & flag2)
                {
                    this._currentMessage = 4;
                    this._duck.canFire = false;
                    if (this._duck.isServerForObject && this.doorIsOpen && this._inputProfile.Pressed("SHOOT") && !this._hatSelector.open && (double)this._hatSelector.fade < 0.00999999977648258)
                    {
                        this._duck.immobilized = true;
                        this._hatSelector.Open(this._playerProfile);
                        this._duck.Fondle(_hatSelector);
                        SFX.Play("consoleOpen", 0.5f);
                    }
                }
                else
                    this._duck.canFire = true;
                if (this._hatSelector.hat != null && (double)this._hatSelector.hat.alpha < 0.00999999977648258 && !this._duck.HasEquipment(_hatSelector.hat))
                {
                    this._hatSelector.hat.alpha = 1f;
                    this._duck.Equip(_hatSelector.hat, false);
                }
                if (this.ready)
                {
                    this._currentMessage = 3;
                    this._readySign.color = Lerp.Color(this._readySign.color, Color.LimeGreen, 0.1f);
                    if (this._hatSelector.hat != null && !this._duck.HasEquipment(_hatSelector.hat))
                    {
                        this._hatSelector.hat.alpha = 1f;
                        this._duck.Equip(_hatSelector.hat, false);
                    }
                }
                else
                {
                    this._readySign.color = Lerp.Color(this._readySign.color, Color.Red, 0.1f);
                    if (this._gun != null)
                    {
                        vec2 = this._gun.position - this._duck.position;
                        if ((double)vec2.length < 30.0)
                        {
                            if (this._duck.holdObject != null)
                            {
                                this._currentMessage = 2;
                                if (flag2)
                                    this._currentMessage = 5;
                            }
                            else
                                this._currentMessage = 1;
                        }
                    }
                }
            }
            this._prevDoorX = this._doorX;
            bool flag3 = this._playerActive && (this._playerProfile.team != null || Network.isActive && Network.connections.Count == 0);
            if (this._playerProfile.connection != null && _playerProfile.connection.levelIndex != DuckNetwork.levelIndex)
                flag3 = false;
            if (flag3 && this._hatSelector != null && this._hatSelector.isServerForObject)
            {
                if (this.profile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("PERIMETER DEFENCE").index) > 0)
                {
                    if (this._turret == null)
                    {
                        foreach (FurniturePosition furniturePosition in this.profile.furniturePositions)
                        {
                            if (RoomEditor.GetFurniture(furniturePosition.id).name == "PERIMETER DEFENCE")
                            {
                                Vec2 vec2 = new Vec2(furniturePosition.x - 2, furniturePosition.y + 2);
                                if (this.rightRoom)
                                {
                                    vec2.x = RoomEditor.roomSize - vec2.x;
                                    vec2.x += 2f;
                                }
                                Vec2 pPosition = vec2 + this.position;
                                if (pPosition.x > (double)this.x)
                                {
                                    if (pPosition.y > (double)this.y)
                                    {
                                        if (pPosition.x < (double)this.x + RoomEditor.roomSize)
                                        {
                                            if (pPosition.y < (double)this.y + RoomEditor.roomSize)
                                            {
                                                this._turret = new RoomDefenceTurret(pPosition, this.duck)
                                                {
                                                    offDir = this.rightRoom ? (sbyte)-1 : (sbyte)1
                                                };
                                                Level.Add(_turret);
                                                break;
                                            }
                                            break;
                                        }
                                        break;
                                    }
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }
                else if (this._turret != null)
                {
                    Level.Remove(_turret);
                    this._turret = null;
                }
            }
            if (this._turret != null)
                this._turret._friendly = this.duck;
            this._doorX = Maths.LerpTowards(this._doorX, flag3 ? 83f : 0.0f, 4f);
            if (Network.isActive && (this.profile.networkStatus == DuckNetStatus.Disconnected && this._prevStatus != DuckNetStatus.Disconnected || this.profile.slotType == SlotType.Spectator) || _doorX == 0.0 && _prevDoorX != 0.0)
                this.OnDoorClosed();
            if (this._playerActive && this.controllerIndex > 3 && !(Level.current.camera is FollowCam))
                TeamSelect2.growCamera = true;
            if (this._currentMessage != this._tutorialMessages.frame)
            {
                this._screenFade = Maths.LerpTowards(this._screenFade, 0.0f, 0.15f);
                if (_screenFade < 0.00999999977648258)
                    this._tutorialMessages.frame = this._currentMessage;
            }
            else
                this._screenFade = Maths.LerpTowards(this._screenFade, 1f, 0.15f);
            this._prevStatus = this.profile.networkStatus;
        }

        public override void Draw()
        {
            if (this._hatSelector != null && (double)this._hatSelector.fadeVal > 0.899999976158142 && this._hatSelector._roomEditor._mode != REMode.Place)
            {
                this._projector.visible = false;
                if (this._duck == null)
                    return;
                this._duck.mindControl = new InputProfile();
            }
            else
            {
                if (this._duck != null)
                    this._duck.mindControl = null;
                this._projector.visible = true;
                if (_tooManyPulse > 0.00999999977648258)
                    Graphics.DrawStringOutline("ROOM FULL", this.position + new Vec2(0.0f, 36f), Color.Red * this._tooManyPulse, Color.Black * this._tooManyPulse, (Depth)0.95f, scale: 2f);
                if (_noMorePulse > 0.00999999977648258)
                    Graphics.DrawStringOutline(" NO MORE ", this.position + new Vec2(0.0f, 36f), Color.Red * this._noMorePulse, Color.Black * this._noMorePulse, (Depth)0.95f, scale: 2f);
                this._tooManyPulse = Lerp.Float(this._tooManyPulse, 0.0f, 0.05f);
                this._noMorePulse = Lerp.Float(this._noMorePulse, 0.0f, 0.05f);
                bool flag1 = this.profile.networkStatus != 0;
                if (_doorX < 82.0)
                {
                    Sprite sprite1 = this._doorLeft;
                    Sprite sprite2 = this._doorRight;
                    bool flag2 = this.profile.slotType == SlotType.Closed;
                    bool flag3 = this.profile.slotType == SlotType.Friend;
                    bool flag4 = this.profile.slotType == SlotType.Invite;
                    bool flag5 = this.profile.slotType == SlotType.Reserved;
                    bool flag6 = this.profile.slotType == SlotType.Local;
                    if (Network.isActive)
                    {
                        sprite1 = this._doorLeftBlank;
                        sprite2 = this._doorRightBlank;
                    }
                    else
                    {
                        flag2 = false;
                        flag3 = false;
                        flag4 = false;
                        flag5 = false;
                        flag1 = false;
                    }
                    Sprite doorLeftBlank = this._doorLeftBlank;
                    Sprite doorRightBlank = this._doorRightBlank;
                    if (this.rightRoom)
                    {
                        Rectangle sourceRectangle1 = new Rectangle((int)this._doorX, 0.0f, doorLeftBlank.width, _doorLeft.height);
                        Graphics.Draw(doorLeftBlank, this.x - 1f, this.y, sourceRectangle1);
                        Rectangle sourceRectangle2 = new Rectangle((int)-(double)this._doorX, 0.0f, _doorRight.width, _doorRight.height);
                        Graphics.Draw(doorRightBlank, (float)((double)this.x - 1.0 + 68.0), this.y, sourceRectangle2);
                        if (_doorX == 0.0)
                        {
                            this._fontSmall.depth = doorLeftBlank.depth + 10;
                            if (!Network.isActive || flag6 && Network.isServer)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 10;
                                Graphics.Draw(_doorIcon, (int)this.x + 57, this.y + 31f);
                                this._fontSmall.DrawOutline("PRESS", new Vec2(this.x + 19f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("START", new Vec2(this.x + 85f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag2)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 8;
                                Graphics.Draw(_doorIcon, (int)this.x + 57, this.y + 31f);
                            }
                            else if (flag1)
                            {
                                this._doorSpinner.depth = doorLeftBlank.depth + 10;
                                Graphics.Draw(_doorSpinner, (int)this.x + 57, this.y + 31f);
                            }
                            else if (flag3)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 11;
                                Graphics.Draw(_doorIcon, (int)this.x + 57, this.y + 31f);
                                this._fontSmall.DrawOutline("PALS", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("ONLY", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag4)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)this.x + 57, this.y + 31f);
                                this._fontSmall.DrawOutline("VIPS", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("ONLY", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag5 && this.profile.reservedUser != null)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                                float num = 120f;
                                float x = this.x + 10f;
                                Graphics.DrawRect(new Vec2(x, this.y + 35f), new Vec2(x + num, this.y + 52f), Color.Black, doorLeftBlank.depth + 20);
                                string text1 = "WAITING FOR";
                                this._fontSmall.Draw(text1, new Vec2((float)((double)x + (double)num / 2.0 - (double)this._fontSmall.GetWidth(text1) / 2.0), this.y + 36f), Color.White, doorLeftBlank.depth + 30);
                                string text2 = this.profile.nameUI;
                                if (text2.Length > 16)
                                    text2 = text2.Substring(0, 16);
                                this._fontSmall.Draw(text2, new Vec2((float)((double)x + (double)num / 2.0 - (double)this._fontSmall.GetWidth(text2) / 2.0), this.y + 44f), Color.White, doorLeftBlank.depth + 30);
                            }
                            else if (flag6)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 13;
                                Graphics.Draw(_doorIcon, (int)this.x + 57, this.y + 31f);
                                this._fontSmall.DrawOutline("HOST", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("SLOT", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 9;
                                Graphics.Draw(_doorIcon, (int)this.x + 57, this.y + 31f);
                                this._fontSmall.DrawOutline("OPEN", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("SLOT", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                        }
                    }
                    else
                    {
                        Rectangle sourceRectangle3 = new Rectangle((int)this._doorX, 0.0f, _doorLeft.width, _doorLeft.height);
                        Graphics.Draw(doorLeftBlank, this.x, this.y, sourceRectangle3);
                        Rectangle sourceRectangle4 = new Rectangle((int)-(double)this._doorX, 0.0f, _doorRight.width, _doorRight.height);
                        Graphics.Draw(doorRightBlank, this.x + 68f, this.y, sourceRectangle4);
                        if (_doorX == 0.0)
                        {
                            this._fontSmall.depth = doorLeftBlank.depth + 10;
                            if (!Network.isActive || flag6 && Network.isServer)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 10;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                                this._fontSmall.DrawOutline("PRESS", new Vec2(this.x + 20f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("START", new Vec2(this.x + 86f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag2)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 8;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                            }
                            else if (flag1)
                            {
                                this._doorSpinner.depth = doorLeftBlank.depth + 10;
                                Graphics.Draw(_doorSpinner, (int)this.x + 58, this.y + 31f);
                            }
                            else if (flag3)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 11;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                                this._fontSmall.DrawOutline("PALS", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("ONLY", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag4)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                                this._fontSmall.DrawOutline("VIPS", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("ONLY", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag5 && this.profile.reservedUser != null)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                                float num = 120f;
                                float x = this.x + 10f;
                                Graphics.DrawRect(new Vec2(x, this.y + 35f), new Vec2(x + num, this.y + 52f), Color.Black, doorLeftBlank.depth + 20);
                                string text3 = "WAITING FOR";
                                this._fontSmall.Draw(text3, new Vec2((float)((double)x + (double)num / 2.0 - (double)this._fontSmall.GetWidth(text3) / 2.0), this.y + 36f), Color.White, doorLeftBlank.depth + 30);
                                string text4 = this.profile.nameUI;
                                if (text4.Length > 16)
                                    text4 = text4.Substring(0, 16);
                                this._fontSmall.Draw(text4, new Vec2((float)((double)x + (double)num / 2.0 - (double)this._fontSmall.GetWidth(text4) / 2.0), this.y + 44f), Color.White, doorLeftBlank.depth + 30);
                            }
                            else if (flag6)
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 13;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                                this._fontSmall.DrawOutline("HOST", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("SLOT", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else
                            {
                                this._doorIcon.depth = doorLeftBlank.depth + 10;
                                this._doorIcon.frame = 9;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, this.y + 31f);
                                this._fontSmall.DrawOutline("OPEN", new Vec2(this.x + 22f, this.y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                this._fontSmall.DrawOutline("SLOT", new Vec2(this.x + 90f, this.y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                        }
                    }
                }
                if (this._playerProfile.team == null || _doorX <= 0.0)
                    return;
                Furniture furniture1 = null;
                if (Profiles.experienceProfile != null)
                {
                    bool flag7 = true;
                    if (Network.isActive && this.profile.connection != DuckNetwork.localConnection && (this.profile.ParentalControlsActive || ParentalControls.AreParentalControlsActive() || this.profile.muteRoom))
                        flag7 = false;
                    if (flag7)
                    {
                        List<FurniturePosition> furniturePositionList = new List<FurniturePosition>();
                        List<FurniturePosition> source1 = new List<FurniturePosition>();
                        foreach (FurniturePosition furniturePosition in this.profile.furniturePositions)
                        {
                            Furniture furniture2 = RoomEditor.GetFurniture(furniturePosition.id);
                            if (furniture2 != null)
                            {
                                if (furniture2.group == Furniture.Characters)
                                {
                                    if (!furniturePosition.flip && this.rightRoom)
                                    {
                                        furniturePosition.furniMapping = furniture2;
                                        source1.Add(furniturePosition);
                                        continue;
                                    }
                                    if (furniturePosition.flip && !this.rightRoom)
                                    {
                                        furniturePosition.furniMapping = furniture2;
                                        furniturePositionList.Add(furniturePosition);
                                        continue;
                                    }
                                }
                                if (furniture2.type == FurnitureType.Theme)
                                    furniture1 = furniture2;
                                else if (furniture2.type != FurnitureType.Font)
                                {
                                    furniture2.sprite.depth = (Depth)(float)(furniture2.deep * (1.0 / 1000.0) - 0.560000002384186);
                                    furniture2.sprite.frame = furniturePosition.variation;
                                    Vec2 pos = new Vec2(furniturePosition.x, furniturePosition.y);
                                    furniture2.sprite.flipH = furniturePosition.flip;
                                    if (this.rightRoom)
                                    {
                                        pos.x = RoomEditor.roomSize - pos.x;
                                        furniture2.sprite.flipH = !furniture2.sprite.flipH;
                                        --pos.x;
                                    }
                                    pos += this.position;
                                    if (furniture2.visible)
                                        furniture2.Draw(pos, furniture2.sprite.depth, furniturePosition.variation, this.profile);
                                    furniture2.sprite.frame = 0;
                                    furniture2.sprite.flipH = false;
                                }
                            }
                        }
                        if (source1.Count > 0)
                        {
                            IOrderedEnumerable<FurniturePosition> source2 = source1.OrderBy<FurniturePosition, int>(furni => furni.x + furni.y * 100);
                            IEnumerable<FurniturePosition> source3 = source1.OrderBy<FurniturePosition, int>(furni => -furni.x + furni.y * 100);
                            int index1 = 0;
                            for (int index2 = 0; index2 < source2.Count<FurniturePosition>(); ++index2)
                            {
                                FurniturePosition furniturePosition1 = source2.ElementAt<FurniturePosition>(index2);
                                Furniture furniMapping1 = furniturePosition1.furniMapping;
                                FurniturePosition furniturePosition2 = source3.ElementAt<FurniturePosition>(index1);
                                Furniture furniMapping2 = furniturePosition2.furniMapping;
                                furniMapping1.sprite.depth = (Depth)(float)(furniMapping2.deep * (1.0 / 1000.0) - 0.560000002384186);
                                furniMapping1.sprite.frame = furniturePosition1.variation;
                                Vec2 pos = new Vec2(furniturePosition2.x, furniturePosition2.y);
                                furniMapping1.sprite.flipH = furniturePosition1.flip;
                                if (this.rightRoom)
                                {
                                    pos.x = RoomEditor.roomSize - pos.x;
                                    furniMapping1.sprite.flipH = !furniMapping1.sprite.flipH;
                                    --pos.x;
                                }
                                pos += this.position;
                                if (furniMapping1.visible)
                                    furniMapping1.Draw(pos, furniMapping1.sprite.depth, furniMapping1.sprite.frame, this.profile);
                                furniMapping1.sprite.frame = 0;
                                furniMapping1.sprite.flipH = false;
                                ++index1;
                            }
                        }
                        if (furniturePositionList.Count > 0)
                        {
                            IOrderedEnumerable<FurniturePosition> source4 = source1.OrderBy<FurniturePosition, int>(furni => -furni.x + furni.y * 100);
                            IEnumerable<FurniturePosition> source5 = source1.OrderBy<FurniturePosition, int>(furni => furni.x + furni.y * 100);
                            int index3 = 0;
                            for (int index4 = 0; index4 < source4.Count<FurniturePosition>(); ++index4)
                            {
                                FurniturePosition furniturePosition3 = source4.ElementAt<FurniturePosition>(index4);
                                Furniture furniMapping3 = furniturePosition3.furniMapping;
                                FurniturePosition furniturePosition4 = source5.ElementAt<FurniturePosition>(index3);
                                Furniture furniMapping4 = furniturePosition4.furniMapping;
                                furniMapping3.sprite.depth = (Depth)(float)(furniMapping4.deep * (1.0 / 1000.0) - 0.560000002384186);
                                furniMapping3.sprite.frame = furniturePosition4.variation;
                                Vec2 pos = new Vec2(furniturePosition3.x, furniturePosition3.y);
                                furniMapping3.sprite.flipH = furniturePosition3.flip;
                                if (this.rightRoom)
                                {
                                    pos.x = RoomEditor.roomSize - pos.x;
                                    furniMapping3.sprite.flipH = !furniMapping3.sprite.flipH;
                                    --pos.x;
                                }
                                pos += this.position;
                                if (furniMapping3.visible)
                                    furniMapping3.Draw(pos, furniMapping3.sprite.depth, furniMapping3.sprite.frame, this.profile);
                                furniMapping3.sprite.frame = 0;
                                furniMapping3.sprite.flipH = false;
                                ++index3;
                            }
                        }
                    }
                    if (this._hatSelector._roomEditor._mode == REMode.Place && this._hatSelector._roomEditor.CurFurni().type == FurnitureType.Theme)
                        furniture1 = this._hatSelector._roomEditor.CurFurni();
                }
                if (this.rightRoom)
                {
                    if (furniture1 == null)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            if (this.profile.GetLightStatus(index))
                            {
                                this._lightBar.depth = this._tutorialTV.depth;
                                this._lightBar.frame = index;
                                Graphics.Draw(_lightBar, this.x + 38f + index * 3, this.y + 49f);
                            }
                        }
                        this._roomSwitch.depth = this._tutorialTV.depth;
                        this._roomSwitch.frame = this.profile.switchStatus ? 1 : 0;
                        Graphics.Draw(_roomSwitch, this.x + 52f, this.y + 47f);
                    }
                    if (furniture1 != null)
                    {
                        Furniture furniture3 = furniture1;
                        furniture3.sprite.flipH = true;
                        furniture3.sprite.depth = this._roomLeftForeground.depth;
                        furniture3.background.depth = this._roomLeftBackground.depth;
                        furniture3.sprite.scale = new Vec2(1f);
                        furniture3.background.scale = new Vec2(1f);
                        Graphics.Draw(furniture3.sprite, this.x + 70f, this.y + 44f, new Rectangle(0.0f, 0.0f, 4f, 87f));
                        Graphics.Draw(furniture3.sprite, this.x + 70f, (float)((double)this.y + 44.0 + 68.0), new Rectangle(0.0f, 68f, 141f, 19f));
                        Graphics.Draw(furniture3.sprite, this.x + 70f, this.y + 44f, new Rectangle(0.0f, 0.0f, 141f, 16f));
                        Graphics.Draw(furniture3.sprite, this.x + 21f, this.y + 44f, new Rectangle(49f, 0.0f, 92f, 68f));
                        furniture3.sprite.depth = this._selectConsole.depth - 20;
                        Graphics.Draw(furniture3.sprite, (float)((double)this.x + 70.0 - 4.0), this.y + 44f, new Rectangle(4f, 0.0f, 44f, 54f));
                        furniture3.sprite.depth = (Depth)0.31f;
                        Graphics.Draw(furniture3.sprite, (float)((double)this.x + 70.0 - 4.0), (float)((double)this.y + 44.0 + 54.0), new Rectangle(4f, 54f, 44f, 14f));
                        furniture3.sprite.flipH = false;
                        furniture3.background.flipH = true;
                        Graphics.Draw(furniture3.background, this.x + 70f, this.y + 45f);
                        furniture3.background.flipH = false;
                    }
                    else
                    {
                        Graphics.Draw(this._roomLeftBackground, this.x - 1f, this.y + 1f);
                        Graphics.Draw(this._roomLeftForeground, this.x - 1f, this.y + 1f, new Rectangle(0.0f, 0.0f, 49f, 16f));
                        Graphics.Draw(this._roomLeftForeground, this.x - 1f, (float)((double)this.y + 1.0 + 16.0), new Rectangle(0.0f, 16f, 6f, 8f));
                        Graphics.Draw(this._roomLeftForeground, this.x - 1f, (float)((double)this.y + 1.0 + 55.0), new Rectangle(0.0f, 55f, 53f, 13f));
                        Graphics.Draw(this._roomLeftForeground, this.x - 1f, (float)((double)this.y + 1.0 + 68.0), new Rectangle(0.0f, 68f, 141f, 19f));
                        Graphics.Draw(this._roomLeftForeground, (float)((double)this.x - 1.0 + 137.0), this.y + 1f, new Rectangle(137f, 0.0f, 4f, 87f));
                    }
                    if (Network.isActive && (Network.isServer && this.profile.connection == DuckNetwork.localConnection || this.profile.connection == Network.host))
                    {
                        this._hostCrown.depth = - 0.5f;
                        Graphics.Draw(this._hostCrown, this.x + 126f, this.y + 23f);
                    }
                }
                else
                {
                    if (furniture1 == null)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            if (this.profile.GetLightStatus(index))
                            {
                                this._lightBar.depth = this._tutorialTV.depth;
                                this._lightBar.frame = index;
                                Graphics.Draw(_lightBar, this.x + 91f + index * 3, this.y + 49f);
                            }
                        }
                        this._roomSwitch.depth = this._tutorialTV.depth;
                        this._roomSwitch.frame = this.profile.switchStatus ? 1 : 0;
                        Graphics.Draw(_roomSwitch, this.x + 81f, this.y + 47f);
                    }
                    if (furniture1 != null)
                    {
                        Furniture furniture4 = furniture1;
                        furniture4.sprite.depth = this._roomLeftForeground.depth;
                        furniture4.background.depth = this._roomLeftBackground.depth;
                        furniture4.sprite.scale = new Vec2(1f);
                        furniture4.background.scale = new Vec2(1f);
                        Graphics.Draw(furniture4.sprite, this.x + 70f, this.y + 44f, new Rectangle(0.0f, 0.0f, 4f, 87f));
                        Graphics.Draw(furniture4.sprite, this.x + 70f, (float)((double)this.y + 44.0 + 68.0), new Rectangle(0.0f, 68f, 141f, 19f));
                        Graphics.Draw(furniture4.sprite, this.x + 70f, this.y + 44f, new Rectangle(0.0f, 0.0f, 141f, 16f));
                        Graphics.Draw(furniture4.sprite, (float)((double)this.x + 70.0 + 49.0), this.y + 44f, new Rectangle(49f, 0.0f, 92f, 68f));
                        furniture4.sprite.depth = this._selectConsole.depth - 20;
                        Graphics.Draw(furniture4.sprite, (float)((double)this.x + 70.0 + 4.0), this.y + 44f, new Rectangle(4f, 0.0f, 44f, 54f));
                        furniture4.sprite.depth = (Depth)0.31f;
                        Graphics.Draw(furniture4.sprite, (float)((double)this.x + 70.0 + 4.0), (float)((double)this.y + 44.0 + 54.0), new Rectangle(4f, 54f, 44f, 14f));
                        Graphics.Draw(furniture4.background, this.x + 70f, this.y + 45f);
                    }
                    else
                    {
                        Graphics.Draw(this._roomLeftBackground, this.x + 4f, this.y + 1f);
                        Graphics.Draw(this._roomLeftForeground, this.x, this.y + 1f, new Rectangle(0.0f, 0.0f, 4f, 87f));
                        Graphics.Draw(this._roomLeftForeground, this.x + 4f, (float)((double)this.y + 1.0 + 68.0), new Rectangle(4f, 68f, 137f, 19f));
                        Graphics.Draw(this._roomLeftForeground, this.x + 92f, this.y + 1f, new Rectangle(92f, 0.0f, 49f, 16f));
                        Graphics.Draw(this._roomLeftForeground, this.x + 135f, (float)((double)this.y + 1.0 + 16.0), new Rectangle(135f, 16f, 6f, 8f));
                        Graphics.Draw(this._roomLeftForeground, this.x + 89f, (float)((double)this.y + 1.0 + 55.0), new Rectangle(89f, 55f, 52f, 13f));
                    }
                    if (Network.isActive && (Network.isServer && this.profile.connection == DuckNetwork.localConnection || this.profile.connection == Network.host))
                    {
                        this._hostCrown.depth = - 0.5f;
                        Graphics.Draw(this._hostCrown, this.x + 14f, this.y + 23f);
                    }
                }
                this._tutorialTV.depth = - 0.58f;
                this._tutorialMessages.depth = - 0.5f;
                this._tutorialMessages.alpha = this._screenFade;
                this._font.alpha = 1f;
                this._font.depth = (Depth)0.6f;
                if (furniture1 != null)
                {
                    this._tutorialTV.depth = - 0.8f;
                    this._tutorialMessages.depth = - 0.8f;
                }
                string currentDisplayName = this._playerProfile.team.currentDisplayName;
                this._selectConsole.depth = - 0.5f;
                this._consoleHighlight.depth = - 0.49f;
                float num1 = 8f;
                if (this.rightRoom)
                {
                    this._consolePos = new Vec2(this.x + 116f, this.y + 30f);
                    this._consoleFlash.scale = new Vec2(0.75f, 0.75f);
                    if (this._selectConsole.imageIndex == 0)
                        this._consoleFlash.alpha = 0.3f;
                    else if (this._selectConsole.imageIndex == 1)
                        this._consoleFlash.alpha = 0.1f;
                    else if (this._selectConsole.imageIndex == 2)
                        this._consoleFlash.alpha = 0.0f;
                    Graphics.Draw(this._consoleFlash, this._consolePos.x + 9f, this._consolePos.y + 7f);
                    Graphics.Draw(_selectConsole, this._consolePos.x, this._consolePos.y);
                    if (_consoleFade > 0.00999999977648258)
                    {
                        this._consoleHighlight.alpha = this._consoleFade;
                        Graphics.Draw(this._consoleHighlight, this._consolePos.x, this._consolePos.y);
                    }
                    Graphics.Draw(this._readySign, this.x + 1f, this.y + 3f);
                    float num2 = -0.57f;
                    if (furniture1 != null)
                        num2 = -0.8f;
                    bool flag8 = true;
                    if (furniture1 == null)
                    {
                        Graphics.Draw(this._tutorialTV, this.x + 57f - num1, this.y + 8f);
                        float num3 = 27f;
                        if (flag8)
                        {
                            if (this._tutorialMessages.frame == 0)
                            {
                                this._font.Draw("@DPAD@MOVE", new Vec2(this.x + 28f + num3, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@JUMP@JUMP", new Vec2(this.x + 28f + num3, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 1)
                            {
                                this._font.Draw("@GRAB@", new Vec2(this.x + 45f + num3, this.y + 17f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("PICKUP", new Vec2(this.x + 29f + num3, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 2)
                            {
                                this._font.Draw("@GRAB@TOSS", new Vec2(this.x + 28f + num3, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@SHOOT@FIRE", new Vec2(this.x + 28f + num3, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 3)
                            {
                                this._font.Draw("@CANCEL@", new Vec2(this.x + 45f + num3, this.y + 17f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("CANCEL", new Vec2(this.x + 29f + num3, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 4)
                            {
                                this._font.Draw("@DPAD@MOVE", new Vec2(this.x + 28f + num3, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@SHOOT@TEAM", new Vec2(this.x + 28f + num3, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 5)
                            {
                                this._font.Draw("@GRAB@TOSS", new Vec2(this.x + 28f + num3, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@SHOOT@TEAM", new Vec2(this.x + 28f + num3, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                        }
                        else
                            Graphics.Draw(this._onlineIcon, (int)this.x + 72, this.y + 19f, (Depth)num2);
                    }
                    this._font.depth = (Depth)0.6f;
                    float num4 = 0.0f;
                    float num5 = 0.0f;
                    Vec2 vec2 = new Vec2(1f, 1f);
                    if (currentDisplayName.Length > 9)
                    {
                        vec2 = new Vec2(0.75f, 0.75f);
                        num4 = 1f;
                        num5 = 1f;
                    }
                    if (currentDisplayName.Length > 12)
                    {
                        vec2 = new Vec2(0.5f, 0.5f);
                        num4 = 2f;
                        num5 = 1f;
                    }
                    this._font.scale = vec2;
                    if (this._hatSelector._roomEditor._mode == REMode.Place)
                    {
                        float num6 = 0.0f;
                        float num7 = 0.0f;
                        string text = "PLAYER 1";
                        float num8 = 47f;
                        this.x += num8;
                        Furniture furniture5 = this._hatSelector._roomEditor.CurFurni();
                        if (furniture5.type == FurnitureType.Font)
                        {
                            furniture5.font.scale = new Vec2(0.5f, 0.5f);
                            furniture5.font.spriteScale = new Vec2(0.5f, 0.5f);
                            furniture5.font.Draw("@SELECT@ACCEPT @CANCEL@CANCEL", (float)((double)this.x + 24.0 - (double)furniture5.font.GetWidth(text) / 2.0) - num6, this.y + 75f + num7, Color.White, (Depth)0.7f, this.profile.inputProfile);
                            furniture5.font.scale = new Vec2(1f, 1f);
                        }
                        else if (furniture5.type == FurnitureType.Theme)
                        {
                            this.profile.font.scale = new Vec2(0.5f, 0.5f);
                            this.profile.font.spriteScale = new Vec2(0.5f, 0.5f);
                            this.profile.font.Draw("@SELECT@ACCEPT @CANCEL@CANCEL", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num6, this.y + 75f + num7, Color.White, (Depth)0.7f, this.profile.inputProfile);
                        }
                        else if (furniture5.name == "CLEAR ROOM")
                        {
                            this.profile.font.scale = new Vec2(0.5f, 0.5f);
                            this.profile.font.spriteScale = new Vec2(0.5f, 0.5f);
                            this.profile.font.Draw("@MENU2@CLEAR @CANCEL@BACK", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num6, this.y + 75f + num7, Color.White, (Depth)0.7f, this.profile.inputProfile);
                        }
                        else
                        {
                            this.profile.font.scale = new Vec2(0.5f, 0.5f);
                            this.profile.font.spriteScale = new Vec2(0.5f, 0.5f);
                            if (this._hatSelector._roomEditor._hover != null)
                                this.profile.font.Draw("@SELECT@DEL @MENU2@GRAB @CANCEL@DONE", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num6, this.y + 75f + num7, Color.White, (Depth)0.7f, this.profile.inputProfile);
                            else
                                this.profile.font.Draw("@SELECT@ADD @MENU2@MOD @CANCEL@DONE", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num6, this.y + 75f + num7, Color.White, (Depth)0.7f, this.profile.inputProfile);
                            this.profile.font.scale = new Vec2(0.25f, 0.25f);
                            int num9 = Profiles.experienceProfile.GetNumFurnitures(furniture5.index) - this.profile.GetNumFurnituresPlaced(furniture5.index);
                            this.profile.font.Draw(furniture5.name + (num9 > 0 ? " |DGGREEN|" : " |DGRED|") + "x" + num9.ToString(), (float)((double)this.x + 17.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num6, (float)((double)this.y + 75.0 + 6.5) + num7, Color.White, (Depth)0.7f);
                            int furnituresPlaced = this.profile.GetTotalFurnituresPlaced();
                            float num10 = furnituresPlaced / (float)RoomEditor.maxFurnitures;
                            this.profile.font.Draw(furnituresPlaced.ToString() + "/" + RoomEditor.maxFurnitures.ToString(), (float)((double)this.x + 68.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num6, (float)((double)this.y + 75.0 + 6.5) + num7, Color.Black, (Depth)0.7f);
                            Vec2 p1 = new Vec2((float)((double)this.x + 56.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num6, (float)((double)this.y + 75.0 + 6.0) + num7);
                            Graphics.DrawRect(p1, p1 + new Vec2(37f, 3f), Colors.BlueGray, (Depth)0.66f, borderWidth: 0.5f);
                            Graphics.DrawRect(p1, p1 + new Vec2(37f * num10, 3f), (double)num10 < 0.400000005960464 ? Colors.DGGreen : ((double)num10 < 0.800000011920929 ? Colors.DGYellow : Colors.DGRed), (Depth)0.68f, borderWidth: 0.5f);
                        }
                        this.profile.font.spriteScale = new Vec2(1f, 1f);
                        this.profile.font.scale = new Vec2(1f, 1f);
                        this.x -= num8;
                    }
                    else
                    {
                        this._playerProfile.font.scale = vec2;
                        this._playerProfile.font.Draw(currentDisplayName, (float)((double)this.x + 94.0 - (double)this._playerProfile.font.GetWidth(currentDisplayName) / 2.0) - num5, this.y + 75f + num4, Color.White, (Depth)0.7f);
                        this._font.scale = new Vec2(1f, 1f);
                    }
                }
                else
                {
                    this._consolePos = new Vec2(this.x + 4f, this.y + 30f);
                    this._consoleFlash.scale = new Vec2(0.75f, 0.75f);
                    if (this._selectConsole.imageIndex == 0)
                        this._consoleFlash.alpha = 0.3f;
                    else if (this._selectConsole.imageIndex == 1)
                        this._consoleFlash.alpha = 0.1f;
                    else if (this._selectConsole.imageIndex == 2)
                        this._consoleFlash.alpha = 0.0f;
                    Graphics.Draw(this._consoleFlash, this._consolePos.x + 9f, this._consolePos.y + 7f);
                    Graphics.Draw(_selectConsole, this._consolePos.x, this._consolePos.y);
                    if (_consoleFade > 0.00999999977648258)
                    {
                        this._consoleHighlight.alpha = this._consoleFade;
                        Graphics.Draw(this._consoleHighlight, this._consolePos.x, this._consolePos.y);
                    }
                    Graphics.Draw(this._readySign, this.x + 96f, this.y + 3f);
                    float num11 = -0.57f;
                    if (furniture1 != null)
                        num11 = -0.8f;
                    bool flag9 = true;
                    if (furniture1 == null)
                    {
                        Graphics.Draw(this._tutorialTV, this.x + 22f + num1, this.y + 8f);
                        if (flag9)
                        {
                            if (this._tutorialMessages.frame == 0)
                            {
                                this._font.Draw("@WASD@MOVE", new Vec2(this.x + 28f + num1, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@JUMP@JUMP", new Vec2(this.x + 28f + num1, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 1)
                            {
                                this._font.Draw("@GRAB@", new Vec2(this.x + 45f + num1, this.y + 17f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("PICKUP", new Vec2(this.x + 29f + num1, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 2)
                            {
                                this._font.Draw("@GRAB@TOSS", new Vec2(this.x + 28f + num1, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@SHOOT@FIRE", new Vec2(this.x + 28f + num1, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 3)
                            {
                                this._font.Draw("@CANCEL@", new Vec2(this.x + 45f + num1, this.y + 17f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("CANCEL", new Vec2(this.x + 29f + num1, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 4)
                            {
                                this._font.Draw("@WASD@MOVE", new Vec2(this.x + 28f + num1, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@SHOOT@TEAM", new Vec2(this.x + 28f + num1, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                            else if (this._tutorialMessages.frame == 5)
                            {
                                this._font.Draw("@GRAB@TOSS", new Vec2(this.x + 28f + num1, this.y + 16f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                                this._font.Draw("@SHOOT@TEAM", new Vec2(this.x + 28f + num1, this.y + 30f), Color.White * this._screenFade, this._tutorialTV.depth + 20, this._inputProfile);
                            }
                        }
                        else
                            Graphics.Draw(this._onlineIcon, (int)this.x + 53, this.y + 19f, (Depth)num11);
                    }
                    this._font.depth = (Depth)0.6f;
                    this._aButton.position = new Vec2(this.x + 39f, this.y + 71f);
                    float num12 = 0.0f;
                    float num13 = 0.0f;
                    Vec2 vec2 = new Vec2(1f, 1f);
                    if (currentDisplayName.Length > 9)
                    {
                        vec2 = new Vec2(0.75f, 0.75f);
                        num12 = 1f;
                        num13 = 1f;
                    }
                    if (currentDisplayName.Length > 12)
                    {
                        vec2 = new Vec2(0.5f, 0.5f);
                        num12 = 2f;
                        num13 = 1f;
                    }
                    if (this._hatSelector._roomEditor._mode == REMode.Place && Profiles.experienceProfile != null)
                    {
                        string text = "PLAYER 1";
                        float num14 = 0.0f;
                        float num15 = 0.0f;
                        Furniture furniture6 = this._hatSelector._roomEditor.CurFurni();
                        if (furniture6.type == FurnitureType.Font)
                        {
                            furniture6.font.scale = new Vec2(0.5f, 0.5f);
                            furniture6.font.spriteScale = new Vec2(0.5f, 0.5f);
                            furniture6.font.Draw("@SELECT@ACCEPT @CANCEL@CANCEL", (float)((double)this.x + 24.0 - (double)furniture6.font.GetWidth(text) / 2.0) - num14, this.y + 75f + num15, Color.White, (Depth)0.7f, this.profile.inputProfile);
                            furniture6.font.scale = new Vec2(1f, 1f);
                        }
                        else if (furniture6.type == FurnitureType.Theme)
                        {
                            this.profile.font.scale = new Vec2(0.5f, 0.5f);
                            this.profile.font.spriteScale = new Vec2(0.5f, 0.5f);
                            this.profile.font.Draw("@SELECT@ACCEPT @CANCEL@CANCEL", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num14, this.y + 75f + num15, Color.White, (Depth)0.7f, this.profile.inputProfile);
                        }
                        else if (furniture6.name == "CLEAR ROOM")
                        {
                            this.profile.font.scale = new Vec2(0.5f, 0.5f);
                            this.profile.font.spriteScale = new Vec2(0.5f, 0.5f);
                            this.profile.font.Draw("@MENU2@CLEAR @CANCEL@BACK", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num14, this.y + 75f + num15, Color.White, (Depth)0.7f, this.profile.inputProfile);
                        }
                        else
                        {
                            this.profile.font.scale = new Vec2(0.5f, 0.5f);
                            this.profile.font.spriteScale = new Vec2(0.5f, 0.5f);
                            if (this._hatSelector._roomEditor._hover != null)
                                this.profile.font.Draw("@SELECT@DEL @MENU2@GRAB @CANCEL@DONE", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num14, this.y + 75f + num15, Color.White, (Depth)0.7f, this.profile.inputProfile);
                            else
                                this.profile.font.Draw("@SELECT@ADD @MENU2@MOD @CANCEL@DONE", (float)((double)this.x + 24.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num14, this.y + 75f + num15, Color.White, (Depth)0.7f, this.profile.inputProfile);
                            this.profile.font.scale = new Vec2(0.25f, 0.25f);
                            int num16 = Profiles.experienceProfile.GetNumFurnitures(furniture6.index) - this.profile.GetNumFurnituresPlaced(furniture6.index);
                            this.profile.font.Draw(furniture6.name + (num16 > 0 ? " |DGGREEN|" : " |DGRED|") + "x" + num16.ToString(), (float)((double)this.x + 17.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num14, (float)((double)this.y + 75.0 + 6.5) + num15, Color.White, (Depth)0.7f);
                            int furnituresPlaced = this.profile.GetTotalFurnituresPlaced();
                            float num17 = furnituresPlaced / (float)RoomEditor.maxFurnitures;
                            this.profile.font.Draw(furnituresPlaced.ToString() + "/" + RoomEditor.maxFurnitures.ToString(), (float)((double)this.x + 68.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num14, (float)((double)this.y + 75.0 + 6.5) + num15, Color.Black, (Depth)0.7f);
                            Vec2 p1 = new Vec2((float)((double)this.x + 56.0 - (double)this.profile.font.GetWidth(text) / 2.0) - num14, (float)((double)this.y + 75.0 + 6.0) + num15);
                            Graphics.DrawRect(p1, p1 + new Vec2(37f, 3f), Colors.BlueGray, (Depth)0.66f, borderWidth: 0.5f);
                            Graphics.DrawRect(p1, p1 + new Vec2(37f * num17, 3f), (double)num17 < 0.400000005960464 ? Colors.DGGreen : ((double)num17 < 0.800000011920929 ? Colors.DGYellow : Colors.DGRed), (Depth)0.68f, borderWidth: 0.5f);
                        }
                        this.profile.font.spriteScale = new Vec2(1f, 1f);
                        this.profile.font.scale = new Vec2(1f, 1f);
                    }
                    else
                    {
                        this._playerProfile.font.scale = vec2;
                        this._playerProfile.font.Draw(currentDisplayName, (float)((double)this.x + 48.0 - (double)this._playerProfile.font.GetWidth(currentDisplayName) / 2.0) - num13, this.y + 75f + num12, Color.White, (Depth)0.7f);
                        this._font.scale = new Vec2(1f, 1f);
                    }
                }
            }
        }
    }
}
