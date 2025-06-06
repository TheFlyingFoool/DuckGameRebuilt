﻿using System;
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
        private float _roomRelatedXAdjust = 0f;
        private float _roomRelatedYAdjust = 0f;

        public bool playerActive => _playerActive;

        public bool ready => _ready;

        public bool doorIsOpen => _doorX > 82f;

        public Profile profile => _playerProfile;

        public void SetProfile(Profile p)
        {
            if (p == null)
            {
                SetProfile(Profiles.alllist[controllerIndex]);
            }
            else
            {
                _playerProfile = p;
                if (_duck != null)
                    _duck.profile = p;
                if (_projector != null)
                    _projector.SetProfile(p);
                if (_hatSelector == null)
                    return;
                _hatSelector.SetProfile(p);
            }
        }

        public void SetHatSelector(HatSelector s) => _hatSelector = s;

        public int controllerIndex => _controllerIndex;

        public ProfileBox2(
          float xpos,
          float ypos,
          InputProfile pProfile,
          Profile pDefaultProfile,
          TeamSelect2 pTeamSelect,
          int pIndex)
          : base(xpos, ypos)
        {
            sin = new SinWave(this, 0.05f, Rando.Float(3.14f));
            _hostCrown = new Sprite("hostCrown");
            _hostCrown.CenterOrigin();
            _lightBar = new SpriteMap("lightBar", 2, 1)
            {
                frame = 0
            };
            _roomSwitch = new SpriteMap("roomSwitch", 7, 5)
            {
                frame = 0
            };
            _controllerIndex = pIndex;
            _font = new BitmapFont("biosFont", 8);
            _fontSmall = new BitmapFont("smallBiosFont", 7, 6);
            layer = Layer.Game;
            _collisionSize = new Vec2(150f, 87f);
            //this._plaque = new Sprite("plaque")
            //{
            //    center = new Vec2(16f, 16f)
            //};
            _inputProfile = pProfile;
            _playerProfile = pDefaultProfile;
            _teamSelection = ControllerNumber();
            _doorLeft = new Sprite("selectDoorLeftPC")
            {
                depth = (Depth)0.905f
            };
            _doorRight = new Sprite("selectDoorRight")
            {
                depth = (Depth)0.9f
            };
            _doorLeftBlank = new Sprite("selectDoorLeftBlank")
            {
                depth = (Depth)0.905f
            };
            _doorRightBlank = new Sprite("selectDoorRightBlank")
            {
                depth = (Depth)0.9f
            };
            _doorSpinner = new SpriteMap("doorSpinner", 25, 25);
            _doorSpinner.AddAnimation("spin", 0.2f, true, 0, 1, 2, 3, 4, 5, 6, 7);
            _doorSpinner.SetAnimation("spin");
            _doorIcon = new SpriteMap("doorSpinner", 25, 25);
            _onlineIcon = new Sprite("gameServerOnline");
            //this._wirelessIcon = new Sprite("gameServerWireless");
            _teamSelect = pTeamSelect;
            _defaultProfile = pDefaultProfile;
            if (rightRoom)
            {
                _roomSwitch = new SpriteMap("roomSwitchRight", 7, 5)
                {
                    frame = 0
                };
                _roomLeftBackground = new Sprite("rightRoomBackground");
                _roomLeftForeground = new Sprite("rightRoomForeground");
                Level.Add(new InvisibleBlock((float)(x - 2 + 142 - 138), y + 69f, 138f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)(x - 2 + 142 - 138), y - 11f, 138f, 12f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)(x + 142 - 98 - 46), y + 56f, 50f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)(x + 142 + 2 - 8), y, 8f, 100f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock((float)(x + 142 - 136 - 9), y, 8f, 25f, PhysicsMaterial.Metal));
                ScaffoldingTileset scaffoldingTileset = new ScaffoldingTileset(x + 126f, y + 63f)
                {
                    neverCheap = true
                };
                Level.Add(scaffoldingTileset);
                scaffoldingTileset.depth = -0.5f;
                scaffoldingTileset.PlaceBlock();
                scaffoldingTileset.UpdateNubbers();
                Level.Add(new Platform(x + 49f, y + 56f, 3f, 5f));
                _readySign = new Sprite("readyLeft");
            }
            else
            {
                _roomLeftBackground = new Sprite("leftRoomBackground");
                _roomLeftForeground = new Sprite("leftRoomForeground");
                Level.Add(new InvisibleBlock(x + 2f, y + 69f, 138f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(x + 2f, y - 11f, 138f, 12f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(x + 92f, y + 56f, 50f, 16f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(x - 4f, y, 8f, 100f, PhysicsMaterial.Metal));
                Level.Add(new InvisibleBlock(x + 135f, y, 8f, 25f, PhysicsMaterial.Metal));
                ScaffoldingTileset scaffoldingTileset = new ScaffoldingTileset(x + 14f, y + 63f)
                {
                    neverCheap = true
                };
                Level.Add(scaffoldingTileset);
                scaffoldingTileset.depth = -0.5f;
                scaffoldingTileset.PlaceBlock();
                scaffoldingTileset.UpdateNubbers();
                Level.Add(new Platform(x + 89f, y + 56f, 3f, 5f));
                _readySign = new Sprite("readyRight");
            }
            _gunSpawnPoint = !rightRoom ? new Vec2(x + 113f, y + 50f) : new Vec2((float)(x + 142f - 118f), y + 50f);
            _readySign.depth = (Depth)0.2f;
            _readySign.center = new Vec2(21.5f, 3.5f);
            _roomLeftBackground.depth = -0.85f;
            _roomLeftForeground.depth = (Depth)0.1f;
            _tutorialMessages = new SpriteMap("tutorialScreensPC", 53, 30);
            _aButton = new Sprite("aButton");
            _tutorialTV = new Sprite("tutorialTV");
            _consoleHighlight = new Sprite("consoleHighlight");
            _consoleFlash = new Sprite("consoleFlash");
            _consoleFlash.CenterOrigin();
            _selectConsole = new SpriteMap("selectConsole", 20, 19);
            _selectConsole.AddAnimation("idle", 1f, true, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            _selectConsole.SetAnimation("idle");
            if (Network.isServer)
            {
                _hatSelector = new HatSelector(x, y, _playerProfile, this)
                {
                    profileBoxNumber = (sbyte)pIndex
                };
                Level.Add(_hatSelector);
            }
            if (rightRoom)
            {
                _projector = new TeamProjector(x + 80f, y + 68f, _playerProfile);
                Level.Add(new ItemSpawner(x + 26f, y + 54f));
            }
            else
            {
                _projector = new TeamProjector(x + 59f, y + 68f, _playerProfile);
                Level.Add(new ItemSpawner(x + 112f, y + 54f));
            }
            Level.Add(_projector);
        }

        public override void Initialize()
        {
            if (Network.isServer && _playerProfile != null && _playerProfile.connection != null && Network.isActive)
                Spawn();
            base.Initialize();
        }

        public void ReturnControl()
        {
            if (_duck == null)
                return;
            _duck.immobilized = false;
        }

        public bool rightRoom => ControllerNumber() == 1 || ControllerNumber() == 3 || ControllerNumber() == 4 || ControllerNumber() == 5 || ControllerNumber() == 7;

        private int ControllerNumber() => _controllerIndex;

        private void SelectTeam()
        {
            Teams.all[_teamSelection].Join(_playerProfile);
            if (_playerProfile.inputProfile != null)
                return;
            _playerProfile.inputProfile = _inputProfile;
        }

        public void ChangeProfile(Profile p)
        {
            if (p == null)
                p = _defaultProfile;
            if (p != _playerProfile)
            {
                if (!Network.isActive && !p.isNetworkProfile)
                {
                    for (int index = 0; index < Profile.defaultProfileMappings.Count; ++index)
                    {
                        if (Profile.defaultProfileMappings[index] == p)
                            Profile.defaultProfileMappings[index] = Profiles.universalProfileList.ElementAt(index);
                    }
                    Profile.defaultProfileMappings[_controllerIndex] = p;
                    if (_teamSelect != null)
                    {
                        foreach (ProfileBox2 profile in _teamSelect._profiles)
                        {
                            if (profile != this && profile.profile == p)
                                profile.SetProfile(Profile.defaultProfileMappings[profile._controllerIndex]);
                        }
                    }
                }
                Team team1 = _playerProfile.team;
                if (team1 != null)
                {
                    _playerProfile.team.Leave(_playerProfile);
                    team1.Join(p);
                }
                if (!Network.isActive)
                    p.inputProfile = _playerProfile.inputProfile;
                p.UpdatePersona();
                if (!Network.isActive)
                    _playerProfile.inputProfile = null;
                _playerProfile = p;
                if (_duck != null)
                {
                    if (_duck.profile.team != null)
                    {
                        Team team2 = _duck.profile.team;
                        team2.Leave(_duck.profile);
                        team2.Join(_playerProfile);
                    }
                    _duck.profile = _playerProfile;
                    if (Network.isActive && DuckNetwork.IndexOf(_playerProfile) >= 0)
                        _duck.netProfileIndex = (byte)DuckNetwork.IndexOf(_playerProfile);
                }
                _projector.SetProfile(p);
                _hatSelector.SetProfile(p);
            }
            OpenCorners();
        }

        public void OpenCorners()
        {
            if (!(Level.current is ArcadeLevel))
                return;
            HUD.CloseAllCorners();
            HUD.AddCornerCounter(HUDCorner.BottomRight, "@TICKET@ ", new FieldBinding(_playerProfile, "ticketCount"), animateCount: true);
            List<ChallengeSaveData> allSaveData = Challenges.GetAllSaveData(_playerProfile);
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
            get => _duck;
            set => _duck = value;
        }

        public VirtualShotgun gun
        {
            get => _gun;
            set => _gun = value;
        }

        public RoomDefenceTurret turret
        {
            get => _turret;
            set => _turret = value;
        }

        public void CloseDoor()
        {
            if (_duck != null)
                _duck.immobilized = true;
            _playerActive = false;
            if (_doorX != 0f)
                return;
            OnDoorClosed();
        }

        public void OnDoorClosed()
        {
            //this._doorClosing = true;
            if (Network.isServer)
            {
                if (_playerProfile.team != null)
                    _playerProfile.team.Leave(_playerProfile);
                _playerProfile = Profiles.defaultProfiles[ControllerNumber()];
                _teamSelection = ControllerNumber();
                SelectTeam();
                _playerProfile.team.Leave(_playerProfile);
                if (_duck != null)
                {
                    _duck.profile = _playerProfile;
                    if (_duck.GetEquipment(typeof(Hat)) is Hat equipment)
                    {
                        _duck.Unequip(equipment);
                        Level.Remove(equipment);
                    }
                }
                foreach (Thing thing in Level.CheckRectAll<RoomDefenceTurret>(topLeft, bottomRight))
                    Level.Remove(thing);
                foreach (Thing thing in Level.CheckRectAll<TeamHat>(topLeft, bottomRight))
                    if (!thing.enablePhysics) Level.Remove(thing);
                _turret = null;
            }
            Despawn();
            //this._doorClosing = false;
        }

        public void Spawn()
        {
            profile.UpdatePersona();
            if (_duck != null)
            {
                _teamSelection = ControllerNumber();
                SelectTeam();
                ReturnControl();
            }
            else
            {
                _gun = new VirtualShotgun(_gunSpawnPoint.x, _gunSpawnPoint.y)
                {
                    roomIndex = (byte)_controllerIndex
                };
                Level.Add(_gun);
                if (rightRoom)
                {
                    _duck = new Duck((float)(x + 142f - 48f), y + 40f, _playerProfile);
                    _window = new Window((float)(x + 142f - 141f), y + 49f)
                    {
                        noframe = true
                    };
                    Level.Add(_window);
                }
                else
                {
                    _duck = new Duck(x + 48f, y + 40f, _playerProfile);
                    _window = new Window(x + 139f, y + 49f)
                    {
                        noframe = true
                    };
                    Level.Add(_window);
                }
                foreach (Thing thing in Level.CheckRectAll<RoomDefenceTurret>(topLeft, bottomRight))
                    Level.Remove(thing);
                _turret = null;
                Level.Add(_duck);
                if (_duck == null || !_duck.HasEquipment(typeof(TeamHat)))
                    return;
                _hatSelector.hat = _duck.GetEquipment(typeof(TeamHat)) as TeamHat;
            }
        }

        public void Despawn()
        {
            if (!Network.isServer)
                return;
            if (_duck != null)
            {
                Fondle(_duck, DuckNetwork.localConnection);
                Level.Remove(_duck);
                if (!Network.isActive && _duck.ragdoll != null)
                    Level.Remove(_duck.ragdoll);
            }
            if (_gun != null)
            {
                Fondle(_gun, DuckNetwork.localConnection);
                Level.Remove(_gun);
            }
            foreach (Window t in Level.CheckRectAll<Window>(topLeft, bottomRight))
            {
                t.lobbyRemoving = true;
                Fondle(t, DuckNetwork.localConnection);
                Level.Remove(t);
            }
            _window = null;
            _duck = null;
            _gun = null;
        }

        public void OpenDoor()
        {
            _playerActive = true;
            SelectTeam();
            if (_duck == null)
                return;
            _duck.immobilized = false;
        }

        public void PrepareDoor()
        {
            DevConsole.Log(DCSection.DuckNet, "|DGGREEN|Preparing Door..");
            if (!Network.isServer)
                return;
            if (_duck == null)
            {
                DevConsole.Log(DCSection.DuckNet, "|DGGREEN|Duckspawn!");
                Spawn();
            }
            else
                DevConsole.Log(DCSection.DuckNet, "|DGRED|Duck was not NULL!");
        }

        public void OpenDoor(Duck d) => _duck = d;


        public SinWave sin;
        public override void Update()
        {
            if (Network.isActive && Network.isServer && _duck != null && profile.connection == null)
                Despawn();
            if (Network.isServer && Network.isActive && _hatSelector != null && _hatSelector.isServerForObject)
                _hatSelector.profileBoxNumber = (sbyte)_controllerIndex;
            if (hostFrames > 0)
            {
                --hostFrames;
                if (hostFrames == 0)
                {
                    TeamSelect2.FillMatchmakingProfiles();
                    if (NetworkDebugger.CurrentServerIndex() < 0)
                    {
                        Network.lanMode = true;
                        DuckNetwork.Host(4, NetworkLobbyType.LAN);
                        (level as TeamSelect2).NetworkDebuggerPrepare();
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
                if (!Network.isServer && profile.networkStatus != DuckNetStatus.Disconnected)
                {
                    _duck = null;
                    foreach (Duck duck in Level.current.things[typeof(Duck)])
                    {
                        if (duck.netProfileIndex == _controllerIndex)
                            _duck = duck;
                    }
                }
                _playerActive = profile.networkStatus == DuckNetStatus.Connected;
            }
            if (_duck != null && _duck.inputProfile != null)
                _inputProfile = _duck.inputProfile;
            if (_hatSelector == null)
                return;
            if (_hatSelector.open && profile.team == null)
                _hatSelector.Reset();
            foreach (VirtualShotgun virtualShotgun in Level.current.things[typeof(VirtualShotgun)])
            {
                if (virtualShotgun.roomIndex == _controllerIndex && virtualShotgun.isServerForObject && virtualShotgun.alpha <= 0)
                {
                    virtualShotgun.position = _gunSpawnPoint;
                    virtualShotgun.alpha = 1f;
                    virtualShotgun.vSpeed = -1f;
                }
            }
            bool flag1 = false; //  
            if (_teamSelect != null && (!Network.isActive || _hatSelector.connection == DuckNetwork.localConnection) && !Network.isActive)
            {
                if (!_playerActive && (Program.testServer || MonoMain.startInLobby) && _playerProfile == Profiles.DefaultPlayer1)
                {
                    OpenDoor();
                    flag1 = true;
                }
                if (_inputProfile.JoinGamePressed() && !_hatSelector.open && (!NetworkDebugger.enabled || NetworkDebugger._instances[NetworkDebugger.currentIndex].hover && (Input.Down(Triggers.Shoot) || Keyboard.Down(Keys.LeftShift) || Keyboard.Down(Keys.RightShift)) || NetworkDebugger.letJoin))
                {
                    if (!_playerActive)
                    {
                        OpenDoor();
                        flag1 = true;
                    }
                    if (NetworkDebugger.letJoin && !Input.Down(Triggers.Shoot) && !Keyboard.Down(Keys.LeftShift) && !Keyboard.Down(Keys.RightShift))
                    {
                        NetworkDebugger.letJoin = false;
                        hostFrames = 2;
                    }
                }
            }
            if (_teamSelect != null && !ready && !Network.isActive && _inputProfile.Pressed(Triggers.Start) && !flag1 && !_teamSelect._returnToMenu.value)
                _teamSelect.OpenPauseMenu(this);
            if (!Network.isActive && _duck != null && !_duck.immobilized)
                _playerActive = true;
            if (Network.isServer && _duck == null && _playerProfile.team != null && (!Network.isActive || _playerProfile.connection != null))
            {
                int num = 0;
                foreach (Team team in Teams.all)
                {
                    if (!(team.name == _playerProfile.team.name))
                        ++num;
                    else
                        break;
                }
                _teamSelection = Teams.all.IndexOf(_playerProfile.team);
                if (_teamSelection == -1) //fall back incase team is wonky
                {
                    _playerProfile.team = Teams.all[0];
                    _teamSelection = 0;
                }
                _playerActive = true;
                SelectTeam();
                Spawn();
            }
            _ready = doorIsOpen && _duck != null && (_duck.dead || _duck.beammode); // || _duck.cameraPosition.y < -100f || _duck.cameraPosition.y > 400f
            if (_duck != null)
            {
                _currentMessage = 0;
                Vec2 vec2 = _duck.position - _consolePos;
                bool flag2 = vec2.length < 20f;
                _consoleFade = Lerp.Float(_consoleFade, flag2 ? 1f : 0f, 0.1f);
                if (_teamSelect != null & flag2)
                {
                    _currentMessage = 4;
                    _duck.canFire = false;
                    if (_duck.isServerForObject && doorIsOpen && _inputProfile.Pressed(Triggers.Shoot) && !_hatSelector.open && _hatSelector.fade < 0.01f)
                    {
                        _duck.immobilized = true;
                        _hatSelector.Open(_playerProfile);
                        _duck.Fondle(_hatSelector);
                        SFX.Play("consoleOpen", 0.5f);
                    }
                }
                else _duck.canFire = true;
                if (_hatSelector.hat != null && _hatSelector.hat.alpha < 0.01f && !_duck.HasEquipment(_hatSelector.hat))
                {
                    _hatSelector.hat.alpha = 1f;
                    _duck.Equip(_hatSelector.hat, false);
                }
                if (ready)
                {
                    _currentMessage = 3;
                    _readySign.color = Lerp.Color(_readySign.color, Color.LimeGreen, 0.1f);
                    if (_hatSelector.hat != null && !_duck.HasEquipment(_hatSelector.hat))
                    {
                        _hatSelector.hat.alpha = 1f;
                        _duck.Equip(_hatSelector.hat, false);
                    }
                }
                else
                {
                    _readySign.color = Lerp.Color(_readySign.color, Color.Red, 0.1f);
                    if (_gun != null)
                    {
                        vec2 = _gun.position - _duck.position;
                        if (vec2.length < 30)
                        {
                            if (_duck.holdObject != null)
                            {
                                _currentMessage = 2;
                                if (flag2)
                                    _currentMessage = 5;
                            }
                            else
                                _currentMessage = 1;
                        }
                    }
                }
            }
            _prevDoorX = _doorX;
            bool flag3 = _playerActive && (_playerProfile.team != null || Network.isActive && Network.connections.Count == 0);
            if (_playerProfile.connection != null && _playerProfile.connection.levelIndex != DuckNetwork.levelIndex)
                flag3 = false;
            if (flag3 && _hatSelector != null && _hatSelector.isServerForObject)
            {
                if (profile.GetNumFurnituresPlaced(RoomEditor.GetFurniture("PERIMETER DEFENCE").index) > 0)
                {
                    if (_turret == null)
                    {
                        foreach (FurniturePosition furniturePosition in profile.furniturePositions)
                        {
                            if (RoomEditor.GetFurniture(furniturePosition.id).name == "PERIMETER DEFENCE")
                            {
                                Vec2 vec2 = new Vec2(furniturePosition.x - 2, furniturePosition.y + 2);
                                if (rightRoom)
                                {
                                    vec2.x = RoomEditor.roomSize - vec2.x;
                                    vec2.x += 2f;
                                }
                                Vec2 pPosition = vec2 + position;
                                if (pPosition.x > x)
                                {
                                    if (pPosition.y > y)
                                    {
                                        if (pPosition.x < x + RoomEditor.roomSize)
                                        {
                                            if (pPosition.y < y + RoomEditor.roomSize)
                                            {
                                                _turret = new RoomDefenceTurret(pPosition, duck)
                                                {
                                                    offDir = rightRoom ? (sbyte)-1 : (sbyte)1
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
                else if (_turret != null)
                {
                    Level.Remove(_turret);
                    _turret = null;
                }
            }
            if (_turret != null)
                _turret._friendly = duck;
            _doorX = Maths.LerpTowards(_doorX, flag3 ? 83f : 0f, 4f);
            if (Network.isActive && (profile.networkStatus == DuckNetStatus.Disconnected && _prevStatus != DuckNetStatus.Disconnected || profile.slotType == SlotType.Spectator) || _doorX == 0 && _prevDoorX != 0)
                OnDoorClosed();
            if (_playerActive && controllerIndex > 3 && !(Level.current.camera is FollowCam))
                TeamSelect2.growCamera = true;
            if (_currentMessage != _tutorialMessages.frame)
            {
                _screenFade = Maths.LerpTowards(_screenFade, 0f, 0.15f);
                if (_screenFade < 0.01f)
                    _tutorialMessages.frame = _currentMessage;
            }
            else
                _screenFade = Maths.LerpTowards(_screenFade, 1f, 0.15f);
            _prevStatus = profile.networkStatus;
        }

        public override void Draw()
        {
            if (_hatSelector != null && _hatSelector.fadeVal > 0.9f && _hatSelector._roomEditor._mode != REMode.Place)
            {
                _projector.visible = false;
                if (_duck == null)
                    return;
                _duck.mindControl = new InputProfile();
            }
            else
            {
                if (_duck != null)
                    _duck.mindControl = null;
                _projector.visible = true;
                if (_tooManyPulse > 0.01f)
                    Graphics.DrawStringOutline("ROOM FULL", position + new Vec2(0f, 36f), Color.Red * _tooManyPulse, Color.Black * _tooManyPulse, (Depth)0.95f, scale: 2f);
                if (_noMorePulse > 0.01f)
                    Graphics.DrawStringOutline(" NO MORE ", position + new Vec2(0f, 36f), Color.Red * _noMorePulse, Color.Black * _noMorePulse, (Depth)0.95f, scale: 2f);
                _tooManyPulse = Lerp.Float(_tooManyPulse, 0f, 0.05f);
                _noMorePulse = Lerp.Float(_noMorePulse, 0f, 0.05f);
                bool flag1 = profile.networkStatus != 0;
                if (_doorX < 82f)
                {
                    Sprite sprite1 = _doorLeft;
                    Sprite sprite2 = _doorRight;
                    bool flag2 = profile.slotType == SlotType.Closed;
                    bool flag3 = profile.slotType == SlotType.Friend;
                    bool flag4 = profile.slotType == SlotType.Invite;
                    bool flag5 = profile.slotType == SlotType.Reserved;
                    bool flag6 = profile.slotType == SlotType.Local;
                    if (Network.isActive)
                    {
                        sprite1 = _doorLeftBlank;
                        sprite2 = _doorRightBlank;
                    }
                    else
                    {
                        flag2 = false;
                        flag3 = false;
                        flag4 = false;
                        flag5 = false;
                        flag1 = false;
                    }
                    Sprite doorLeftBlank = _doorLeftBlank;
                    Sprite doorRightBlank = _doorRightBlank;
                    doorLeftBlank.texture.skipSpriteAtlas = true;
                    doorRightBlank.texture.skipSpriteAtlas = true;

                    if (rightRoom)
                    {
                        Rectangle leftRect = new Rectangle(_doorX, 0f, doorLeftBlank.width - _doorX, _doorLeft.height);
                        Graphics.Draw(doorLeftBlank, x - 1f, y, leftRect);
                        Rectangle rightRect = new Rectangle(-_doorX, 0f, _doorRight.width, _doorRight.height);
                        Graphics.Draw(doorRightBlank, x - 1f + 68f, y, rightRect);
                        if (_doorX == 0)
                        {
                            _fontSmall.depth = doorLeftBlank.depth + 10;

                            if (!Network.isActive || flag6 && Network.isServer)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 10;
                                Graphics.Draw(_doorIcon, (int)x + 57, y + 31f);
                                _fontSmall.DrawOutline("PRESS", new Vec2(x + 19f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                _fontSmall.DrawOutline(Triggers.Start, new Vec2(x + 85f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag2)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 8;
                                Graphics.Draw(_doorIcon, (int)x + 57, y + 31f);
                            }
                            else if (flag1)
                            {
                                _doorSpinner.depth = doorLeftBlank.depth + 10;
                                Graphics.Draw(_doorSpinner, (int)x + 57, y + 31f);
                            }
                            else if (flag3)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 11;
                                Graphics.Draw(_doorIcon, (int)x + 57, y + 31f);
                                if (LangHandler.reverse)
                                {
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("PALS", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                                else
                                {
                                    _fontSmall.DrawOutline("PALS", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                            }
                            else if (flag4)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)x + 57, y + 31f);
                                if (LangHandler.reverse)
                                {
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("VIPS", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                                else
                                {
                                    _fontSmall.DrawOutline("VIPS", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                            }
                            else if (flag5 && profile.reservedUser != null)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, y + 31f);
                                float num = 120f;
                                float x = this.x + 10f;
                                Graphics.DrawRect(new Vec2(x, y + 35f), new Vec2(x + num, y + 52f), Color.Black, doorLeftBlank.depth + 20);
                                string text1 = "WAITING FOR";
                                _fontSmall.Draw(text1, new Vec2((x + num / 2f - _fontSmall.GetWidth(text1) / 2f), y + 36f), Color.White, doorLeftBlank.depth + 30);
                                string text2 = profile.name;

                                if (profile.muteName)
                                    text2 = "Player " + (profile.networkIndex + 1).ToString();
                                if (profile.isUsingRebuilt) text2 += "|PINK|♥|WHITE|";

                                if (text2.Length > 16)
                                    text2 = text2.Substring(0, 16);
                                _fontSmall.Draw(text2, new Vec2((x + num / 2f - _fontSmall.GetWidth(text2) / 2f), y + 44f), Color.White, doorLeftBlank.depth + 30);
                            }
                            else if (flag6)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 13;
                                Graphics.Draw(_doorIcon, (int)x + 57, y + 31f);
                                if (LangHandler.reverse)
                                {
                                    _fontSmall.DrawOutline("SLOT", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("HOST", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                                else
                                {
                                    _fontSmall.DrawOutline("HOST", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("SLOT", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                            }
                            else
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 9;
                                Graphics.Draw(_doorIcon, (int)x + 57, y + 31f);
                                _fontSmall.DrawOutline("OPEN", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                _fontSmall.DrawOutline("SLOT", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                        }
                    }
                    else
                    {
                        Rectangle sourceRectangle3 = new Rectangle(_doorX, 0f, _doorLeft.width - _doorX, _doorLeft.height);
                        Graphics.Draw(doorLeftBlank, x, y, sourceRectangle3);
                        Rectangle sourceRectangle4 = new Rectangle(0f, 0f, _doorRight.width - _doorX, _doorRight.height);
                        Graphics.Draw(doorRightBlank, x + 68f + _doorX, y, sourceRectangle4);
                        if (_doorX == 0)
                        {
                            _fontSmall.depth = doorLeftBlank.depth + 10;
                            if (!Network.isActive || flag6 && Network.isServer)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 10;
                                Graphics.Draw(_doorIcon, (int)x + 58, y + 31f);
                                _fontSmall.DrawOutline("PRESS", new Vec2(x + 20f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                _fontSmall.DrawOutline(Triggers.Start, new Vec2(x + 86f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                            else if (flag2)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 8;
                                Graphics.Draw(_doorIcon, (int)x + 58, y + 31f);
                            }
                            else if (flag1)
                            {
                                _doorSpinner.depth = doorLeftBlank.depth + 10;
                                Graphics.Draw(_doorSpinner, (int)x + 58, y + 31f);
                            }
                            else if (flag3)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 11;
                                Graphics.Draw(_doorIcon, (int)x + 58, y + 31f);
                                if (LangHandler.reverse)
                                {
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("PALS", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                                else
                                {
                                    _fontSmall.DrawOutline("PALS", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }

                            }
                            else if (flag4)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)x + 58, y + 31f);
                                if (LangHandler.reverse)
                                {
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("VIPS", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                                else
                                {
                                    _fontSmall.DrawOutline("VIPS", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("ONLY", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                            }
                            else if (flag5 && profile.reservedUser != null)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 12;
                                Graphics.Draw(_doorIcon, (int)this.x + 58, y + 31f);
                                float num = 120f;
                                float x = this.x + 10f;
                                Graphics.DrawRect(new Vec2(x, y + 35f), new Vec2(x + num, y + 52f), Color.Black, doorLeftBlank.depth + 20);
                                string text3 = "WAITING FOR";
                                _fontSmall.Draw(text3, new Vec2((float)(x + num / 2f - _fontSmall.GetWidth(text3) / 2f), y + 36f), Color.White, doorLeftBlank.depth + 30);
                                string text4 = profile.name;

                                if (profile.muteName)
                                    text4 = "Player " + (profile.networkIndex + 1).ToString();
                                if (profile.isUsingRebuilt) text4 += "|PINK|♥|WHITE|";
                                if (text4.Length > 16)
                                    text4 = text4.Substring(0, 16);
                                _fontSmall.Draw(text4, new Vec2((float)(x + num / 2f - _fontSmall.GetWidth(text4) / 2f), y + 44f), Color.White, doorLeftBlank.depth + 30);
                            }
                            else if (flag6)
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 13;
                                Graphics.Draw(_doorIcon, (int)x + 58, y + 31f);
                                if (LangHandler.reverse)
                                {
                                    _fontSmall.DrawOutline("SLOT", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("HOST", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                                else
                                {
                                    _fontSmall.DrawOutline("HOST", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                    _fontSmall.DrawOutline("SLOT", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                                }
                            }
                            else
                            {
                                _doorIcon.depth = doorLeftBlank.depth + 10;
                                _doorIcon.frame = 9;
                                Graphics.Draw(_doorIcon, (int)x + 58, y + 31f);
                                _fontSmall.DrawOutline("OPEN", new Vec2(x + 22f, y + 40f), Color.White, Colors.BlueGray, doorLeftBlank.depth + 10);
                                _fontSmall.DrawOutline("SLOT", new Vec2(x + 90f, y + 40f), Color.White, Colors.BlueGray, doorRightBlank.depth + 10);
                            }
                        }
                    }
                }
                if (_playerProfile.team == null || _doorX <= 0)
                    return;
                Furniture roomTheme = null;
                if (Profiles.experienceProfile != null)
                {
                    bool shouldDisplayFurniture = true;
                    if (Network.isActive && profile.connection != DuckNetwork.localConnection && (profile.ParentalControlsActive || profile.muteRoom)) // || ParentalControls.AreParentalControlsActive() || 
                        shouldDisplayFurniture = false;
                    if (shouldDisplayFurniture)
                    {
                        List<FurniturePosition> furniturePositionList = new List<FurniturePosition>();
                        List<FurniturePosition> letterMapRight = new List<FurniturePosition>();
                        foreach (FurniturePosition furniturePosition in profile.furniturePositions)
                        {
                            Furniture furniture2 = RoomEditor.GetFurniture(furniturePosition.id);
                            if (furniture2 != null)
                            {
                                if (furniture2.group == Furniture.Characters)
                                {
                                    if (!furniturePosition.flip && rightRoom)
                                    {
                                        furniturePosition.furniMapping = furniture2;
                                        letterMapRight.Add(furniturePosition);
                                        continue;
                                    }
                                    if (furniturePosition.flip && !rightRoom)
                                    {
                                        furniturePosition.furniMapping = furniture2;
                                        furniturePositionList.Add(furniturePosition);
                                        continue;
                                    }
                                }
                                if (furniture2.type == FurnitureType.Theme)
                                    roomTheme = furniture2;
                                else if (furniture2.type != FurnitureType.Font)
                                {
                                    furniture2.sprite.depth = -0.56f + (furniture2.deep * 0.001f);
                                    furniture2.sprite.frame = furniturePosition.variation;
                                    Vec2 pos = new Vec2(furniturePosition.x, furniturePosition.y);
                                    furniture2.sprite.flipH = furniturePosition.flip;
                                    if (rightRoom)
                                    {
                                        pos.x = RoomEditor.roomSize - pos.x;
                                        furniture2.sprite.flipH = !furniture2.sprite.flipH;
                                        --pos.x;
                                    }
                                    pos += position;
                                    if (furniture2.visible)
                                        furniture2.Draw(pos, furniture2.sprite.depth, furniturePosition.variation, profile);
                                    furniture2.sprite.frame = 0;
                                    furniture2.sprite.flipH = false;
                                }
                            }
                        }
                        if (letterMapRight.Count > 0)
                        {
                            IOrderedEnumerable<FurniturePosition> letters = letterMapRight.OrderBy(furni => furni.x + furni.y * 100);
                            IEnumerable<FurniturePosition> lettersReverse = letterMapRight.OrderBy(furni => -furni.x + furni.y * 100);
                            int index1 = 0;
                            for (int index2 = 0; index2 < letters.Count(); ++index2)
                            {
                                FurniturePosition p = letters.ElementAt(index2);
                                Furniture f = p.furniMapping;
                                FurniturePosition opposite = lettersReverse.ElementAt(index1);
                                Furniture fOpposite = opposite.furniMapping;
                                f.sprite.depth = -0.56f + (fOpposite.deep * 0.001f);
                                f.sprite.frame = p.variation;
                                Vec2 pos = new Vec2(opposite.x, opposite.y);
                                f.sprite.flipH = p.flip;
                                if (rightRoom)
                                {
                                    pos.x = RoomEditor.roomSize - pos.x;
                                    f.sprite.flipH = !f.sprite.flipH;
                                    --pos.x;
                                }
                                pos += position;
                                if (f.visible)
                                    f.Draw(pos, f.sprite.depth, f.sprite.frame, profile);
                                f.sprite.frame = 0;
                                f.sprite.flipH = false;
                                ++index1;
                            }
                        }
                        if (furniturePositionList.Count > 0)
                        {
                            IOrderedEnumerable<FurniturePosition> letters = letterMapRight.OrderBy(furni => -furni.x + furni.y * 100);
                            IEnumerable<FurniturePosition> lettersReverse = letterMapRight.OrderBy(furni => furni.x + furni.y * 100);
                            int index3 = 0;
                            for (int index4 = 0; index4 < letters.Count(); ++index4)
                            {
                                FurniturePosition p = letters.ElementAt(index4);
                                Furniture f = p.furniMapping;
                                FurniturePosition opposite = lettersReverse.ElementAt(index3);
                                Furniture fOpposite = opposite.furniMapping;
                                f.sprite.depth = -0.56f + (fOpposite.deep * 0.001f);
                                f.sprite.frame = opposite.variation;
                                Vec2 pos = new Vec2(p.x, p.y);
                                f.sprite.flipH = p.flip;
                                if (rightRoom)
                                {
                                    pos.x = RoomEditor.roomSize - pos.x;
                                    f.sprite.flipH = !f.sprite.flipH;
                                    --pos.x;
                                }
                                pos += position;
                                if (f.visible)
                                    f.Draw(pos, f.sprite.depth, f.sprite.frame, profile);
                                f.sprite.frame = 0;
                                f.sprite.flipH = false;
                                ++index3;
                            }
                        }
                    }
                    if (_hatSelector._roomEditor._mode == REMode.Place && _hatSelector._roomEditor.CurFurni().type == FurnitureType.Theme)
                        roomTheme = _hatSelector._roomEditor.CurFurni();
                    _roomRelatedXAdjust = 0f;
                    _roomRelatedYAdjust = 0f;
                    if (roomTheme != null)
                    {
                        if (roomTheme.name == "BAR ROOM" ||
                            roomTheme.name == "GREENHOUSE ROOM" ||
                            roomTheme.name == "MUSIC ROOM" ||
                            roomTheme.name == "OFFICE ROOM")
                        {
                            _roomRelatedXAdjust = 1f;
                            if (rightRoom)
                                _roomRelatedXAdjust -= 3f;
                        }
                        if (roomTheme.name == "BAR ROOM" ||
                            roomTheme.name == "BATH ROOM")
                            _roomRelatedYAdjust = 1f;
                    }
                }
                if (rightRoom)
                {
                    if (roomTheme == null)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            if (profile.GetLightStatus(index))
                            {
                                _lightBar.depth = _tutorialTV.depth;
                                _lightBar.frame = index;
                                Graphics.Draw(_lightBar, x + 38f + index * 3, y + 49f);
                            }
                        }
                        _roomSwitch.depth = _tutorialTV.depth;
                        _roomSwitch.frame = profile.switchStatus ? 1 : 0;
                        Graphics.Draw(_roomSwitch, x + 52f, y + 47f);
                    }
                    if (roomTheme != null)
                    {
                        Furniture theme = roomTheme;
                        theme.sprite.flipH = true;
                        theme.sprite.depth = _roomLeftForeground.depth;
                        theme.background.depth = _roomLeftBackground.depth;
                        theme.sprite.scale = new Vec2(1f);
                        theme.background.scale = new Vec2(1f);
                        Graphics.Draw(theme.sprite, x + 70f, y + 44f, new Rectangle(0f, 0f, 4f, 87f));
                        Graphics.Draw(theme.sprite, x + 70f, (float)(y + 44f + 68f), new Rectangle(0f, 68f, 141f, 19f));
                        Graphics.Draw(theme.sprite, x + 70f, y + 44f, new Rectangle(0f, 0f, 141f, 16f));
                        Graphics.Draw(theme.sprite, x + 21f, y + 44f, new Rectangle(49f, 0f, 92f, 68f));
                        theme.sprite.depth = _selectConsole.depth - 20;
                        Graphics.Draw(theme.sprite, (float)(x + 70f - 4f), y + 44f, new Rectangle(4f, 0f, 44f, 54f));
                        theme.sprite.depth = (Depth)0.31f;
                        Graphics.Draw(theme.sprite, (float)(x + 70f - 4f), (float)(y + 44f + 54f), new Rectangle(4f, 54f, 44f, 14f));
                        theme.sprite.flipH = false;
                        theme.background.flipH = true;
                        Graphics.Draw(theme.background, x + 70f, y + 45f);
                        theme.background.flipH = false;
                    }
                    else
                    {
                        Graphics.Draw(_roomLeftBackground, x - 1f, y + 1f);
                        Graphics.Draw(_roomLeftForeground, x - 1f, y + 1f, new Rectangle(0f, 0f, 49f, 16f));
                        Graphics.Draw(_roomLeftForeground, x - 1f, (float)(y + 1f + 16f), new Rectangle(0f, 16f, 6f, 8f));
                        Graphics.Draw(_roomLeftForeground, x - 1f, (float)(y + 1f + 55f), new Rectangle(0f, 55f, 53f, 13f));
                        Graphics.Draw(_roomLeftForeground, x - 1f, (float)(y + 1f + 68f), new Rectangle(0f, 68f, 141f, 19f));
                        Graphics.Draw(_roomLeftForeground, (float)(x - 1f + 137f), y + 1f, new Rectangle(137f, 0f, 4f, 87f));
                    }
                    if (Network.isActive && (Network.isServer && profile.connection == DuckNetwork.localConnection || profile.connection == Network.host))
                    {
                        _hostCrown.depth = -0.5f;
                        Graphics.Draw(_hostCrown, x + 126f, y + 23f);
                    }
                }
                else
                {
                    if (roomTheme == null)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            if (profile.GetLightStatus(index))
                            {
                                _lightBar.depth = _tutorialTV.depth;
                                _lightBar.frame = index;
                                Graphics.Draw(_lightBar, x + 91f + index * 3, y + 49f);
                            }
                        }
                        _roomSwitch.depth = _tutorialTV.depth;
                        _roomSwitch.frame = profile.switchStatus ? 1 : 0;
                        Graphics.Draw(_roomSwitch, x + 81f, y + 47f);
                    }
                    if (roomTheme != null)
                    {
                        Furniture theme = roomTheme;
                        theme.sprite.depth = _roomLeftForeground.depth;
                        theme.background.depth = _roomLeftBackground.depth;
                        theme.sprite.scale = new Vec2(1f);
                        theme.background.scale = new Vec2(1f);
                        Graphics.Draw(theme.sprite, x + 70f, y + 44f, new Rectangle(0f, 0f, 4f, 87f));
                        Graphics.Draw(theme.sprite, x + 70f, (float)(y + 44f + 68f), new Rectangle(0f, 68f, 141f, 19f));
                        Graphics.Draw(theme.sprite, x + 70f, y + 44f, new Rectangle(0f, 0f, 141f, 16f));
                        Graphics.Draw(theme.sprite, (float)(x + 70f + 49f), y + 44f, new Rectangle(49f, 0f, 92f, 68f));
                        theme.sprite.depth = _selectConsole.depth - 20;
                        Graphics.Draw(theme.sprite, (float)(x + 70f + 4f), y + 44f, new Rectangle(4f, 0f, 44f, 54f));
                        theme.sprite.depth = (Depth)0.31f;
                        Graphics.Draw(theme.sprite, (float)(x + 70f + 4f), (float)(y + 44f + 54f), new Rectangle(4f, 54f, 44f, 14f));
                        Graphics.Draw(theme.background, x + 70f, y + 45f);
                    }
                    else
                    {
                        Graphics.Draw(_roomLeftBackground, x + 4f, y + 1f);
                        Graphics.Draw(_roomLeftForeground, x, y + 1f, new Rectangle(0f, 0f, 4f, 87f));
                        Graphics.Draw(_roomLeftForeground, x + 4f, (float)(y + 1f + 68f), new Rectangle(4f, 68f, 137f, 19f));
                        Graphics.Draw(_roomLeftForeground, x + 92f, y + 1f, new Rectangle(92f, 0f, 49f, 16f));
                        Graphics.Draw(_roomLeftForeground, x + 135f, (float)(y + 1f + 16f), new Rectangle(135f, 16f, 6f, 8f));
                        Graphics.Draw(_roomLeftForeground, x + 89f, (float)(y + 1f + 55f), new Rectangle(89f, 55f, 52f, 13f));
                    }
                    if (Network.isActive && (Network.isServer && profile.connection == DuckNetwork.localConnection || profile.connection == Network.host))
                    {
                        _hostCrown.depth = -0.5f;
                        Graphics.Draw(_hostCrown, x + 14f, y + 23f);
                    }
                }
                _tutorialTV.depth = -0.58f;
                _tutorialMessages.depth = -0.5f;
                _tutorialMessages.alpha = _screenFade;
                _font.alpha = 1f;
                _font.depth = (Depth)0.6f;
                if (roomTheme != null)
                {
                    _tutorialTV.depth = -0.8f;
                    _tutorialMessages.depth = -0.8f;
                }
                string currentDisplayName = _playerProfile.team.currentDisplayName;
                _selectConsole.depth = -0.5f;
                _consoleHighlight.depth = -0.49f;
                float num1 = 8f;
                if (rightRoom)
                {
                    _consolePos = new Vec2(x + 116f, y + 30f);
                    _consoleFlash.scale = new Vec2(0.75f, 0.75f);
                    if (_selectConsole.imageIndex == 0)
                        _consoleFlash.alpha = 0.3f;
                    else if (_selectConsole.imageIndex == 1)
                        _consoleFlash.alpha = 0.1f;
                    else if (_selectConsole.imageIndex == 2)
                        _consoleFlash.alpha = 0f;
                    Graphics.Draw(_consoleFlash, _consolePos.x + 9f, _consolePos.y + 7f);
                    Graphics.Draw(_selectConsole, _consolePos.x, _consolePos.y);
                    if (_consoleFade > 0.01f)
                    {
                        _consoleHighlight.alpha = _consoleFade;
                        Graphics.Draw(_consoleHighlight, _consolePos.x, _consolePos.y);
                    }
                    if (_hatSelector._roomEditor._mode != REMode.Place)
                        Graphics.Draw(_readySign, x + 22.5f, y + 6.5f);
                    float num2 = -0.57f;
                    if (roomTheme != null)
                        num2 = -0.8f;
                    bool flag8 = true;
                    if (roomTheme == null)
                    {
                        Graphics.Draw(_tutorialTV, x + 57f - num1, y + 8f);
                        float num3 = 27f;
                        if (flag8)
                        {
                            if (_tutorialMessages.frame == 0)
                            {
                                _font.Draw("@DPAD@MOVE", new Vec2(x + 28f + num3, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@JUMP@JUMP", new Vec2(x + 28f + num3, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 1)
                            {
                                _font.Draw("@GRAB@", new Vec2(x + 45f + num3, y + 17f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("PICKUP", new Vec2(x + 29f + num3, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 2)
                            {
                                _font.Draw("@GRAB@TOSS", new Vec2(x + 28f + num3, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@SHOOT@FIRE", new Vec2(x + 28f + num3, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 3)
                            {
                                _font.Draw("@CANCEL@", new Vec2(x + 45f + num3, y + 17f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw(Triggers.Cancel, new Vec2(x + 29f + num3, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 4)
                            {
                                _font.Draw("@DPAD@MOVE", new Vec2(x + 28f + num3, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@SHOOT@TEAM", new Vec2(x + 28f + num3, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 5)
                            {
                                _font.Draw("@GRAB@TOSS", new Vec2(x + 28f + num3, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@SHOOT@TEAM", new Vec2(x + 28f + num3, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                        }
                        else
                            Graphics.Draw(_onlineIcon, (int)x + 72, y + 19f, (Depth)num2);
                    }
                    _font.depth = (Depth)0.6f;
                    Vec2 displayNameScale = new Vec2(1f);
                    float displayNameXAdjust = 0f;
                    float displayNameYAdjust = 0f;
                    int displayNameLength = _playerProfile.font.GetLength(currentDisplayName);

                    if (displayNameLength > 27) // That is really long...
                    {
                        displayNameScale = new Vec2(0.25f);
                        displayNameXAdjust = 1f;
                        displayNameYAdjust = 3f;
                    }
                    else if (displayNameLength > 20)
                    {
                        displayNameScale = new Vec2(0.32f);
                        displayNameXAdjust = 1f;
                        displayNameYAdjust = 3f;
                    }
                    else if (displayNameLength > 12)
                    {
                        displayNameScale = new Vec2(0.5f);
                        displayNameXAdjust = 1f;
                        displayNameYAdjust = 2f;
                    }
                    else if (displayNameLength > 9)
                    {
                        displayNameScale = new Vec2(0.75f);
                        displayNameXAdjust = 1f;
                        displayNameYAdjust = 1f;
                    }
                    _font.scale = displayNameScale;
                    if (_hatSelector._roomEditor._mode == REMode.Place)
                    {
                        const float tempShift = 47f;
                        x += tempShift;
                        float cenX = x + 47f;
                        float cenY = y + 80f;
                        Furniture curFurni = _hatSelector._roomEditor.CurFurni();
                        if (curFurni.type == FurnitureType.Font)
                        {
                            Graphics.Draw(_readySign, x - tempShift + 22.5f, y + 6.5f);
                            curFurni.font.scale = new Vec2(0.5f);
                            curFurni.font.characterYOffset = -1;
                            string displayText = "@SELECT@ACCEPT @CANCEL@CANCEL";
                            curFurni.font.Draw(displayText, cenX - curFurni.font.GetWidth(displayText) / 2f + _roomRelatedXAdjust, cenY - curFurni.font.height / 2f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);
                            curFurni.font.scale = new Vec2(1f);
                            curFurni.font.characterYOffset = 0;
                        }
                        else if (curFurni.type == FurnitureType.Theme)
                        {
                            Graphics.Draw(_readySign, x - tempShift + 22.5f, y + 6.5f);
                            profile.font.scale = new Vec2(0.5f);
                            profile.font.characterYOffset = -1;
                            string displayText = "@SELECT@ACCEPT @CANCEL@CANCEL";
                            profile.font.Draw(displayText, cenX - profile.font.GetWidth(displayText) / 2f + _roomRelatedXAdjust, cenY - profile.font.height / 2f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);
                            profile.font.characterYOffset = 0;
                        }
                        else if (curFurni.name == "CLEAR ROOM")
                        {
                            Graphics.Draw(_readySign, x - tempShift + 22.5f, y + 6.5f);
                            profile.font.scale = new Vec2(0.5f);
                            profile.font.characterYOffset = -1;
                            string displayText = "@MENU2@CLEAR @CANCEL@BACK";
                            profile.font.Draw(displayText, cenX - profile.font.GetWidth(displayText) / 2f + _roomRelatedXAdjust, cenY - profile.font.height / 2f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);
                            profile.font.characterYOffset = 0;
                        }
                        else
                        {
                            profile.font.scale = new Vec2(0.5f);
                            profile.font.spriteScale = new Vec2(0.8f);
                            profile.font.characterYOffset = -1;
                            string instruction = "";
                            if (_hatSelector._roomEditor._hover != null)
                                instruction = "@SELECT@DEL @MENU2@GRAB @CANCEL@DONE";
                            else
                                instruction = "@SELECT@ADD @MENU2@VARY @CANCEL@DONE";
                            float instructionWidth = profile.font.GetWidth("@SELECT@ADD @MENU2@VARY @CANCEL@XYZ");
                            profile.font.Draw(instruction, cenX - instructionWidth / 2f + _roomRelatedXAdjust, cenY - 4f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);

                            profile.font.characterYOffset = 0;
                            profile.font.scale = new Vec2(0.32f);
                            int numFur = Profiles.experienceProfile.GetNumFurnitures(curFurni.index) - profile.GetNumFurnituresPlaced(curFurni.index);
                            if (DGRSettings.TemporaryUnlockAll)
                                numFur = 99;
                            string numFurText = curFurni.name + (numFur > 0 ? " |DGGREEN|" : " |DGRED|") + "x" + numFur.ToString();
                            float numFurTextWidth = profile.font.GetWidth(numFurText);
                            profile.font.Draw(numFurText, cenX - numFurTextWidth / 2f + _roomRelatedXAdjust, cenY + 1f + _roomRelatedYAdjust, Color.White, (Depth)0.7f);

                            profile.font.scale = new Vec2(0.5f);
                            int numFurPlaced = profile.GetTotalFurnituresPlaced();
                            float ratio = numFurPlaced / (float)RoomEditor.maxFurnitures;
                            float barWidth = 44f;
                            float barHeight = 9f;
                            string ratioText = numFurPlaced.ToString() + "/" + RoomEditor.maxFurnitures.ToString();
                            float ratioTextWidth = profile.font.GetWidth(ratioText);
                            float ratioTextHeight = profile.font.height;
                            Vec2 barLeftTop = new Vec2(x - 46f, y + 2f);
                            profile.font.Draw(ratioText, barLeftTop.x + barWidth / 2 - ratioTextWidth / 2, barLeftTop.y + barHeight / 2f - ratioTextHeight / 2f, Color.White, (Depth)0.7f);
                            Graphics.DrawRect(barLeftTop, barLeftTop + new Vec2(barWidth * ratio, barHeight), (ratio < 0.4f ? Colors.DGGreen : (ratio < 0.8f ? Colors.DGYellow : Colors.DGRed)) * 0.8f, (Depth)0.68f, borderWidth: 0.5f);
                        }
                        profile.font.spriteScale = new Vec2(1f, 1f);
                        profile.font.scale = new Vec2(1f, 1f);
                        profile.font.spriteScale = new Vec2(1f, 1f);
                        profile.font.scale = new Vec2(1f, 1f);
                        x -= tempShift;
                    }
                    else
                    {
                        _playerProfile.font.scale = displayNameScale;
                        _playerProfile.font.Draw(currentDisplayName, (x + 94f - _playerProfile.font.GetWidth(currentDisplayName) / 2f) + displayNameXAdjust + _roomRelatedXAdjust, y + 75f + displayNameYAdjust + _roomRelatedYAdjust, Color.White, (Depth)0.7f);
                        _font.scale = new Vec2(1f, 1f);
                    }
                }
                else
                {
                    _consolePos = new Vec2(x + 4f, y + 30f);
                    _consoleFlash.scale = new Vec2(0.75f, 0.75f);
                    if (_selectConsole.imageIndex == 0)
                        _consoleFlash.alpha = 0.3f;
                    else if (_selectConsole.imageIndex == 1)
                        _consoleFlash.alpha = 0.1f;
                    else if (_selectConsole.imageIndex == 2)
                        _consoleFlash.alpha = 0f;
                    Graphics.Draw(_consoleFlash, _consolePos.x + 9f, _consolePos.y + 7f);
                    Graphics.Draw(_selectConsole, _consolePos.x, _consolePos.y);
                    if (_consoleFade > 0.01f)
                    {
                        _consoleHighlight.alpha = _consoleFade;
                        Graphics.Draw(_consoleHighlight, _consolePos.x, _consolePos.y);
                    }
                    if (_hatSelector._roomEditor._mode != REMode.Place)
                        Graphics.Draw(_readySign, x + 117.5f, y + 6.5f);
                    float num11 = -0.57f;
                    if (roomTheme != null)
                        num11 = -0.8f;
                    bool flag9 = true;
                    if (roomTheme == null)
                    {
                        Graphics.Draw(_tutorialTV, x + 22f + num1, y + 8f);
                        if (flag9)
                        {
                            if (_tutorialMessages.frame == 0)
                            {
                                _font.Draw("@WASD@MOVE", new Vec2(x + 28f + num1, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@JUMP@JUMP", new Vec2(x + 28f + num1, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 1)
                            {
                                _font.Draw("@GRAB@", new Vec2(x + 45f + num1, y + 17f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("PICKUP", new Vec2(x + 29f + num1, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 2)
                            {
                                _font.Draw("@GRAB@TOSS", new Vec2(x + 28f + num1, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@SHOOT@FIRE", new Vec2(x + 28f + num1, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 3)
                            {
                                _font.Draw("@CANCEL@", new Vec2(x + 45f + num1, y + 17f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw(Triggers.Cancel, new Vec2(x + 29f + num1, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 4)
                            {
                                _font.Draw("@WASD@MOVE", new Vec2(x + 28f + num1, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@SHOOT@TEAM", new Vec2(x + 28f + num1, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                            else if (_tutorialMessages.frame == 5)
                            {
                                _font.Draw("@GRAB@TOSS", new Vec2(x + 28f + num1, y + 16f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                                _font.Draw("@SHOOT@TEAM", new Vec2(x + 28f + num1, y + 30f), Color.White * _screenFade, _tutorialTV.depth + 20, _inputProfile);
                            }
                        }
                        else
                            Graphics.Draw(_onlineIcon, (int)x + 53, y + 19f, (Depth)num11);
                    }
                    _font.depth = (Depth)0.6f;
                    _aButton.position = new Vec2(x + 39f, y + 71f);
                    Vec2 displayNameScale = new Vec2(1f);
                    float displayNameXAdjust = 0f;
                    float displayNameYAdjust = 0f;
                    int displayNameLength = _playerProfile.font.GetLength(currentDisplayName);
                    if (displayNameLength > 28) // That is really long...
                    {
                        displayNameScale = new Vec2(0.25f);
                        displayNameXAdjust = -1f;
                        displayNameYAdjust = 3f;
                    }
                    else if (displayNameLength > 20)
                    {
                        displayNameScale = new Vec2(0.32f);
                        displayNameXAdjust = -1f;
                        displayNameYAdjust = 3f;
                    }
                    else if (displayNameLength > 12)
                    {
                        displayNameScale = new Vec2(0.5f);
                        displayNameXAdjust = -1f;
                        displayNameYAdjust = 2f;
                    }
                    else if (displayNameLength > 9)
                    {
                        displayNameScale = new Vec2(0.75f);
                        displayNameXAdjust = -1f;
                        displayNameYAdjust = 1f;
                    }
                    if (_hatSelector._roomEditor._mode == REMode.Place && Profiles.experienceProfile != null)
                    {
                        float cenX = x + 47f;
                        float cenY = y + 80f;
                        Furniture curFurni = _hatSelector._roomEditor.CurFurni();
                        if (curFurni.type == FurnitureType.Font)
                        {
                            Graphics.Draw(_readySign, x + 117.5f, y + 6.5f);
                            curFurni.font.scale = new Vec2(0.5f);
                            curFurni.font.characterYOffset = -1;
                            string displayText = "@SELECT@ACCEPT @CANCEL@CANCEL";
                            curFurni.font.Draw(displayText, cenX - curFurni.font.GetWidth(displayText) / 2f + _roomRelatedXAdjust, cenY - curFurni.font.height / 2f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);
                            curFurni.font.scale = new Vec2(1f);
                            curFurni.font.characterYOffset = 0;
                        }
                        else if (curFurni.type == FurnitureType.Theme)
                        {
                            Graphics.Draw(_readySign, x + 117.5f, y + 6.5f);
                            profile.font.scale = new Vec2(0.5f);
                            profile.font.characterYOffset = -1;
                            string displayText = "@SELECT@ACCEPT @CANCEL@CANCEL";
                            profile.font.Draw(displayText, cenX - profile.font.GetWidth(displayText) / 2f + _roomRelatedXAdjust, cenY - profile.font.height / 2f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);
                            profile.font.characterYOffset = 0;
                        }
                        else if (curFurni.name == "CLEAR ROOM")
                        {
                            Graphics.Draw(_readySign, x + 117.5f, y + 6.5f);
                            profile.font.scale = new Vec2(0.5f);
                            profile.font.characterYOffset = -1;
                            string displayText = "@MENU2@CLEAR @CANCEL@BACK";
                            profile.font.Draw(displayText, cenX - profile.font.GetWidth(displayText) / 2f + _roomRelatedXAdjust, cenY - profile.font.height / 2f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);
                            profile.font.characterYOffset = 0;
                        }
                        else
                        {
                            profile.font.scale = new Vec2(0.5f);
                            profile.font.spriteScale = new Vec2(0.8f);
                            profile.font.characterYOffset = -1;
                            string instruction = "";
                            if (_hatSelector._roomEditor._hover != null)
                                instruction = "@SELECT@DEL @MENU2@GRAB @CANCEL@DONE";
                            else
                                instruction = "@SELECT@ADD @MENU2@VARY @CANCEL@DONE";
                            float instructionWidth = profile.font.GetWidth("@SELECT@ADD @MENU2@VARY @CANCEL@XYZ");
                            profile.font.Draw(instruction, cenX - instructionWidth / 2f + _roomRelatedXAdjust, cenY - 4f + _roomRelatedYAdjust, Color.White, (Depth)0.7f, profile.inputProfile);

                            profile.font.characterYOffset = 0;
                            profile.font.scale = new Vec2(0.32f);
                            int numFur = Profiles.experienceProfile.GetNumFurnitures(curFurni.index) - profile.GetNumFurnituresPlaced(curFurni.index);
                            if (DGRSettings.TemporaryUnlockAll)
                                numFur = 99;
                            string numFurText = curFurni.name + (numFur > 0 ? " |DGGREEN|" : " |DGRED|") + "x" + numFur.ToString();
                            float numFurTextWidth = profile.font.GetWidth(numFurText);
                            profile.font.Draw(numFurText, cenX - numFurTextWidth / 2f + _roomRelatedXAdjust, cenY + 1f + _roomRelatedYAdjust, Color.White, (Depth)0.7f);

                            profile.font.scale = new Vec2(0.5f);
                            int numFurPlaced = profile.GetTotalFurnituresPlaced();
                            float ratio = numFurPlaced / (float)RoomEditor.maxFurnitures;
                            float barWidth = 44f;
                            float barHeight = 9f;
                            string ratioText = numFurPlaced.ToString() + "/" + RoomEditor.maxFurnitures.ToString();
                            float ratioTextWidth = profile.font.GetWidth(ratioText);
                            float ratioTextHeight = profile.font.height;
                            Vec2 barLeftTop = new Vec2(x + 95f, y + 2f);
                            profile.font.Draw(ratioText, barLeftTop.x + barWidth / 2 - ratioTextWidth / 2, barLeftTop.y + barHeight / 2f - ratioTextHeight / 2f, Color.White, (Depth)0.7f);
                            Graphics.DrawRect(barLeftTop, barLeftTop + new Vec2(barWidth * ratio, barHeight), (ratio < 0.4f ? Colors.DGGreen : (ratio < 0.8f ? Colors.DGYellow : Colors.DGRed)) * 0.8f, (Depth)0.68f, borderWidth: 0.5f);
                        }
                        profile.font.spriteScale = new Vec2(1f, 1f);
                        profile.font.scale = new Vec2(1f, 1f);
                    }
                    else
                    {
                        _playerProfile.font.scale = displayNameScale;
                        _playerProfile.font.Draw(currentDisplayName, (float)(x + 48f - _playerProfile.font.GetWidth(currentDisplayName) / 2f) + displayNameXAdjust + _roomRelatedXAdjust, y + 75f + displayNameYAdjust + _roomRelatedYAdjust, Color.White, (Depth)0.7f);
                        _font.scale = new Vec2(1f, 1f);
                    }
                }
            }
        }
    }
}
