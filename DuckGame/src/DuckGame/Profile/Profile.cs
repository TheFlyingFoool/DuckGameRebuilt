using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Profile
    {
        public bool ReplayRebuilt;
        public bool isUsingRebuilt
        {
            get
            {
                if (netData == null) return false;
                return netData.Get("REBUILT", false);
            }
        }
        public bool inSameRebuiltVersion
        {
            get
            {
                if (netData == null) return false;
                return netData.Get("rVer", "?") == Program.CURRENT_VERSION_ID;
            }
        }
        public HatSelector hatSelector;
        public int prevXPsave;
        public string prevFurniPositionData;
        public Dictionary<NetworkConnection, int> connectionTrouble = new Dictionary<NetworkConnection, int>();
        public NetIndex16 latestGhostIndex = (NetIndex16)25;
        private Dictionary<string, ChallengeSaveData> _challengeData = new Dictionary<string, ChallengeSaveData>();
        public int numClientCustomLevels;
        public int flagIndex = -1;
        public bool _blockStatusDirty = true;
        private string _muteString = "";
        private bool _blocked;
        public SlotType pendingSpectatorMode = SlotType.Max;
        public List<Team> customTeams = new List<Team>();
        public List<bool> networkHatUnlockStatuses;
        public ProfileNetData netData = new ProfileNetData();
        public HashSet<string> sentMojis = new HashSet<string>();
        public Holdable carryOverObject;
        public float storeValue;
        private bool _parentalControlsActive;
        private static BitmapFont _defaultFont;
        private List<FurniturePosition> _internalFurniturePositions = new List<FurniturePosition>();
        private Dictionary<int, int> _internalFurnitures = new Dictionary<int, int>();
        private List<Furniture> _availableList;
        private int _roundsSinceXP;
        private int _littleManBucks;
        private int _numLittleMen;
        private int _littleManLevel = 1;
        private int _milkFill = 1;
        private int _numSandwiches = 1;
        private int _currentDay = 1;
        private int _punished;
        public bool keepSetName;
        private string _name = "";
        public static bool loading;
        private string _id;
        private static MTSpriteBatch _batch;
        private static SpriteMap _egg;
        private static SpriteMap _eggShine;
        private static SpriteMap _eggBorder;
        private static SpriteMap _eggOuter;
        private static SpriteMap _eggSymbols;
        private static List<Color> _allowedColors;
        private static SpriteMap _easel;
        private static SpriteMap _easelSymbols;
        private List<string> _unlocks = new List<string>();
        private int _ticketCount;
        private int _timesMetVincent;
        private int _timesMetVincentSale;
        private int _timesMetVincentSell;
        private int _timesMetVincentImport;
        private int _timesMetVincentHint;
        public float timeOfDay;
        private int _xp;
        public byte flippers;
        private float _funSlider = 0.5f;
        private int _preferredColor = -1;
        private int _requestedColor = -1;
        private ProfileStats _stats = new ProfileStats();
        private ProfileStats _junkStats;
        private ProfileStats _prevStats = new ProfileStats();
        private ProfileStats _endOfRoundStats;
        private CurrentGame _currentGame = new CurrentGame();
        public string lastKnownName;
        private ulong _steamID;
        //private ulong _lanProfileID;
        private NetworkConnection _connection;
        private static bool _networkStatusLooping;
        private DuckNetStatus _networkStatus;
        private float _currentStatusTimeout;
        private int _currentStatusTries;
        public bool invited;
        private Profile _linkedProfile;
        private bool _ready = true;
        private byte _networkIndex = byte.MaxValue;
        private byte _fixedGhostIndex = byte.MaxValue;
        public bool isRemoteLocalDuck;
        public byte spectatorChangeIndex;
        private int _spectatorChangeCooldown;
        private byte _remoteSpectatorChangeIndex;
        public Dictionary<NetIndex16, GhostObject> removedGhosts = new Dictionary<NetIndex16, GhostObject>();
        private SlotType _slotType;
        public SlotType originalSlotType;
        private object _reservedUser;
        private Team _reservedTeam;
        private sbyte _reservedSpectatorPersona = -1;
        private Duck _duck;
        private DuckPersona _persona;
        private InputProfile _inputProfile;
        private Team _team;
        private int _wins;
        private bool _wasRockThrower;
        private List<DeviceInputMapping> _inputMappingOverrides = new List<DeviceInputMapping>();
        public Team defaultTeam;
        public DuckPersona defaultPersona;
        public bool isNetworkProfile;
        public string fileName = "";
        public bool isDefaultProfile;

        public void ReportConnectionTrouble(NetworkConnection pFrom) => connectionTrouble[pFrom] = 200;

        public void TickConnectionTrouble()
        {
            if (connectionTrouble.Count > 0)
            {
                foreach (NetworkConnection connection in Network.connections)
                {
                    if (connectionTrouble.ContainsKey(connection) && connectionTrouble[connection] > 0)
                        connectionTrouble[connection]--;
                }
            }
            if (_spectatorChangeCooldown <= 0)
                return;
            --_spectatorChangeCooldown;
        }

        public void HasConnectionFailed()
        {
            foreach (NetworkConnection connection in Network.connections)
            {
                if (connectionTrouble.ContainsKey(connection) && connectionTrouble[connection] > 0)
                    connectionTrouble[connection]--;
            }
        }

        public Team networkDefaultTeam => _networkIndex < DG.MaxPlayers ? Teams.all[_networkIndex] : Teams.all[Rando.Int(7)];

        public DuckPersona networkDefaultPersona => _networkIndex < DG.MaxPlayers ? Persona.alllist[_networkIndex] : Persona.Duck1;

        public Dictionary<string, ChallengeSaveData> challengeData => _challengeData;

        public ChallengeSaveData GetSaveData(string guid, bool canBeNull = false)
        {
            ChallengeSaveData saveData1;
            if (_challengeData.TryGetValue(guid, out saveData1))
                return saveData1;
            if (canBeNull)
                return null;
            ChallengeSaveData saveData2 = new ChallengeSaveData
            {
                profileID = id,
                challenge = guid
            };
            _challengeData.Add(guid, saveData2);
            return saveData2;
        }

        private void RefreshBlockStatus()
        {
            _blockStatusDirty = false;
            _blocked = Options.Data.blockedPlayers != null && Options.Data.blockedPlayers.Contains(steamID);
            _muteString = Options.GetMuteSettings(this);
        }

        public bool muteChat
        {
            get
            {
                if (_blockStatusDirty)
                    RefreshBlockStatus();
                return _muteString.Contains("C");
            }
            set
            {
                Options.SetMuteSetting(this, "C", value);
                RefreshBlockStatus();
            }
        }

        public bool muteHat
        {
            get
            {
                if (_blockStatusDirty)
                    RefreshBlockStatus();
                return _muteString.Contains("H");
            }
            set
            {
                Options.SetMuteSetting(this, "H", value);
                RefreshBlockStatus();
            }
        }

        public bool muteRoom
        {
            get
            {
                if (_blockStatusDirty)
                    RefreshBlockStatus();
                return _muteString.Contains("R");
            }
            set
            {
                Options.SetMuteSetting(this, "R", value);
                RefreshBlockStatus();
            }
        }

        public bool muteName
        {
            get
            {
                if (_blockStatusDirty)
                    RefreshBlockStatus();
                return _muteString.Contains("N");
            }
            set
            {
                Options.SetMuteSetting(this, "N", value);
                RefreshBlockStatus();
            }
        }

        public bool blocked
        {
            get
            {
                if (_blockStatusDirty)
                    RefreshBlockStatus();
                return _blocked;
            }
        }

        public bool ReplaySpectator;
        public bool spectator => slotType == SlotType.Spectator;

        public ushort customTeamIndexOffset => (ushort)(Teams.kCustomOffset + fixedGhostIndex * Teams.kCustomSpread);

        public int IndexOfCustomTeam(Team pTeam)
        {
            int num = customTeams.IndexOf(pTeam);
            return num >= 0 ? customTeamIndexOffset + num : num;
        }

        public Team GetCustomTeam(ushort pIndex)
        {
            if (connection == DuckNetwork.localConnection)
                return Teams.core.extraTeams.Count > pIndex && pIndex >= 0 ? Teams.core.extraTeams[pIndex] : null;
            while (customTeams.Count <= pIndex)
                customTeams.Add(new Team("CUSTOM", "hats/cluehat")
                {
                    owner = this
                });
            return customTeams[pIndex];
        }

        public bool ParentalControlsActive
        {
            get
            {
                if (connection != null && connection != DuckNetwork.localConnection)
                {
                    return _parentalControlsActive;
                }
                return false;
            }
            set
            {
                _parentalControlsActive = value;
            }
        }

        public BitmapFont font
        {
            get
            {
                if (_defaultFont == null)
                    _defaultFont = new BitmapFont("biosFont", 8);
                BitmapFont font = _defaultFont;
                foreach (FurniturePosition furniturePosition in _furniturePositions)
                {
                    if (furniturePosition != null)
                    {
                        Furniture furniture = RoomEditor.GetFurniture(furniturePosition.id);
                        if (furniture != null && furniture.type == FurnitureType.Font && furniture.font != null)
                        {
                            font = furniture.font;
                            break;
                        }
                    }
                }
                return font;
            }
        }

        private List<FurniturePosition> _furniturePositions
        {
            get
            {
                if (_linkedProfile != null)
                    return _linkedProfile._furniturePositions;
                _internalFurniturePositions.RemoveAll(x => x == null);
                return _internalFurniturePositions;
            }
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile._furniturePositions = value;
                else
                    _internalFurniturePositions = value;
            }
        }

        public List<FurniturePosition> furniturePositions => _furniturePositions;

        public BitBuffer furniturePositionData
        {
            get
            {
                if (_linkedProfile != null)
                    return _linkedProfile.furniturePositionData;
                BitBuffer furniturePositionData = new BitBuffer();
                foreach (FurniturePosition furniturePosition in _furniturePositions)
                {
                    furniturePositionData.Write(furniturePosition.x);
                    furniturePositionData.Write(furniturePosition.y);
                    furniturePositionData.Write(furniturePosition.variation);
                    furniturePositionData.WritePacked(furniturePosition.id, 15);
                    furniturePositionData.WritePacked(furniturePosition.flip ? 1 : 0, 1);
                }
                return furniturePositionData;
            }
            set
            {
                if (_linkedProfile != null)
                {
                    _linkedProfile.furniturePositionData = value;
                }
                else
                {
                    _furniturePositions.Clear();
                    try
                    {
                        int num = 0;
                        while (value.position != value.lengthInBytes)
                        {
                            FurniturePosition furniturePosition = new FurniturePosition
                            {
                                x = value.ReadByte(),
                                y = value.ReadByte(),
                                variation = value.ReadByte(),
                                id = value.ReadBits<ushort>(15),
                                flip = value.ReadBits<byte>(1) > 0
                            };
                            Furniture furniture = RoomEditor.GetFurniture(furniturePosition.id);
                            if (furniture != null)
                            {
                                furniturePosition.furniMapping = furniture;
                                if (furniture.type == FurnitureType.Font || furniture.type == FurnitureType.Theme || furniturePosition.y < 80 && furniturePosition.x < RoomEditor.roomSize + 20)
                                {
                                    _furniturePositions.Add(furniturePosition);
                                    if (furniture.name == "PERIMETER DEFENCE")
                                        ++num;
                                }
                            }
                        }
                        if (num <= 1)
                            return;
                        _furniturePositions.RemoveAll(x => x.furniMapping.name == "PERIMETER DEFENCE");
                    }
                    catch (Exception)
                    {
                        DevConsole.Log(DCSection.General, "Failed to load furniture position data.");
                        _furniturePositions.Clear();
                    }
                }
            }
        }

        public int GetNumFurnituresPlaced(int idx)
        {
            int furnituresPlaced = 0;
            foreach (FurniturePosition furniturePosition in _furniturePositions)
            {
                if (furniturePosition.id == idx)
                    ++furnituresPlaced;
            }
            return furnituresPlaced;
        }

        public int GetTotalFurnituresPlaced()
        {
            int furnituresPlaced = 0;
            foreach (FurniturePosition furniturePosition in _furniturePositions)
            {
                Furniture furniture = RoomEditor.GetFurniture(furniturePosition.id);
                if (furniture != null && furniture.type != FurnitureType.Theme && furniture.type != FurnitureType.Font)
                    ++furnituresPlaced;
            }
            return furnituresPlaced;
        }

        public BitBuffer furnitureOwnershipData
        {
            get
            {
                if (_linkedProfile != null)
                    return _linkedProfile.furnitureOwnershipData;
                BitBuffer furnitureOwnershipData = new BitBuffer();
                foreach (KeyValuePair<int, int> furniture in _furnitures)
                {
                    furnitureOwnershipData.Write(furniture.Key);
                    furnitureOwnershipData.Write(furniture.Value);
                }
                return furnitureOwnershipData;
            }
            set
            {
                if (_linkedProfile != null)
                {
                    _linkedProfile.furnitureOwnershipData = value;
                }
                else
                {
                    _furnitures.Clear();
                    _availableList = null;
                    try
                    {
                        while (value.position != value.lengthInBytes)
                        {
                            FurniturePosition furniturePosition = new FurniturePosition();
                            _furnitures[value.ReadInt()] = value.ReadInt();
                        }
                    }
                    catch (Exception)
                    {
                        DevConsole.Log(DCSection.General, "Failed to load furniture ownership data.");
                    }
                }
            }
        }

        public void ClearFurnitures()
        {
            _furnitures.Clear();
            _furniturePositions.Clear();
        }

        public Dictionary<int, int> _furnitures
        {
            get => _linkedProfile != null ? _linkedProfile._furnitures : _internalFurnitures;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile._furnitures = value;
                else
                    _internalFurnitures = value;
            }
        }

        public int GetNumFurnitures(int idx)
        {
            Furniture furniture = RoomEditor.GetFurniture(idx);
            if (furniture != null && Profiles.experienceProfile != null)
            {
                if (furniture.alwaysHave)
                    return 1;
                if (furniture.name == "EGG" || furniture.name == "PHOTO")
                    return Profiles.experienceProfile.numLittleMen + 1;
            }
            int num;
            _furnitures.TryGetValue(idx, out num);
            return furniture.name == "PERIMETER DEFENCE" && num > 1 ? 1 : num;
        }

        public void SetNumFurnitures(int idx, int num)
        {
            _furnitures[idx] = num;
            _availableList = null;
        }

        public int GetTotalFurnitures()
        {
            int totalFurnitures = 0;
            foreach (KeyValuePair<int, int> furniture in _furnitures)
                totalFurnitures += furniture.Value;
            return totalFurnitures;
        }

        private static string Stringlonger(int length)
        {
            string str = "";
            for (int index = 0; index < length; ++index)
                str += "z";
            return str;
        }

        private static string AvailFurniSortKey(Furniture x) => x.group.name + Stringlonger(RoomEditor._furniGroupMap[x.group].IndexOf(x));

        public List<Furniture> GetAvailableFurnis()
        {
            if (_availableList == null)
            {
                _availableList = new List<Furniture>();
                if (!DGRSettings.TemporaryUnlockAll)
                {
                    foreach (KeyValuePair<int, int> furniture1 in _furnitures)
                    {
                        if (furniture1.Value > 0)
                        {
                            Furniture furniture2 = RoomEditor.GetFurniture(furniture1.Key);
                            if (furniture2 != null)
                                _availableList.Add(furniture2);
                        }
                    }
                }
                foreach (Furniture allFurni in RoomEditor.AllFurnis())
                {
                    if (allFurni.alwaysHave || DGRSettings.TemporaryUnlockAll)
                        _availableList.Add(allFurni);
                }
                _availableList.Sort((x, y) => AvailFurniSortKey(x).CompareTo(AvailFurniSortKey(y)));
            }
            return _availableList;
        }

        public int roundsSinceXP
        {
            get => _linkedProfile != null ? _linkedProfile.roundsSinceXP : _roundsSinceXP;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.roundsSinceXP = value;
                _roundsSinceXP = value;
            }
        }

        public int littleManBucks
        {
            get => _linkedProfile != null ? _linkedProfile.littleManBucks : _littleManBucks;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.littleManBucks = value;
                _littleManBucks = value;
            }
        }

        public int numLittleMen
        {
            get => _linkedProfile != null ? _linkedProfile.numLittleMen : _numLittleMen;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.numLittleMen = value;
                _numLittleMen = value;
            }
        }

        public int littleManLevel
        {
            get => _linkedProfile != null ? _linkedProfile.littleManLevel : _littleManLevel;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.littleManLevel = value;
                _littleManLevel = value;
            }
        }

        public int milkFill
        {
            get => _linkedProfile != null ? _linkedProfile.milkFill : _milkFill;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.milkFill = value;
                _milkFill = value;
            }
        }

        public int numSandwiches
        {
            get => _linkedProfile != null ? _linkedProfile.numSandwiches : _numSandwiches;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.numSandwiches = value;
                _numSandwiches = value;
            }
        }

        public int currentDay
        {
            get => _linkedProfile != null ? _linkedProfile.currentDay : _currentDay;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.currentDay = value;
                _currentDay = value;
            }
        }

        public int punished
        {
            get => _linkedProfile != null ? _linkedProfile.punished : _punished;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.punished = value;
                _punished = value;
            }
        }

        public string formattedName
        {
            get
            {
                string rawName = this.rawName;
                if (steamID != 0UL)
                    rawName = steamID.ToString();
                return rawName;
            }
        }

        public string rawName => _name;

        public string nameUI
        {
            get
            {
                string nameUi = name;
                
                if (muteName)
                    nameUi = $"Player {networkIndex + 1}";
                
                if (isUsingRebuilt && !DGRSettings.HSDClearNames)
                {
                    string dgrMojiName = "DGR";

                    if (!inSameRebuiltVersion)
                        dgrMojiName += "DIM";

                    if (DGRDevs.Contributors.Any(x => x.SteamID == steamID))
                        dgrMojiName += $"_{steamID}";
                    
                    nameUi += $"@{dgrMojiName}@";
                }
                
                return nameUi;
            }
        }
        //alright so hear me out, DUCK GAME FUCKING SUCKS -NiK0
        public string nameUIBodge
        {
            get
            {
                string nameUi = name;
                
                if (muteName)
                    nameUi = $"Player {networkIndex + 1}";
                
                if (isUsingRebuilt)
                {
                    string dgrMojiName = "DGRBIG";

                    if (!inSameRebuiltVersion)
                        dgrMojiName += "DIM";

                    if (DGRDevs.Contributors.Any(x => x.SteamID == steamID))
                        dgrMojiName += $"_{steamID}";
                    
                    nameUi += $"@{dgrMojiName}@";
                }
                
                return nameUi;
            }
        }

        public string name
        {
            get
            {
                if (linkedProfile != null && connection == DuckNetwork.localConnection && !Profiles.IsExperience(this))
                    return linkedProfile.name;
                if (keepSetName || steamID == 0UL || slotType == SlotType.Local)
                    return _name;
                if (Steam.user != null && (long)steamID == (long)Steam.user.id)
                    return Steam.user.name;
                if (lastKnownName != null)
                    return lastKnownName;
                if (!(_name == steamID.ToString()))
                    return _name;
                if (Steam.IsInitialized())
                {
                    User user = User.GetUser(steamID);
                    if (user != null && user.id != 0UL)
                    {
                        lastKnownName = user.name;
                        return lastKnownName;
                    }
                }
                return "STEAM PROFILE";
            }
            set => _name = value;
        }

        public static bool logStats => true;

        public string id => _id;

        public void SetID(string varID) => _id = varID;

        private static Color PickColor()
        {
            int index = Rando.Int(_allowedColors.Count - 1);
            Color allowedColor = _allowedColors[index];
            _allowedColors.RemoveAt(index);
            return allowedColor;
        }

        public static Random GetLongGenerator(ulong id)
        {
            Random longGenerator = new Random(Math.Abs((int)(id % int.MaxValue)));
            for (int index = 0; index < (int)(id % 252UL); ++index)
                Rando.Int(100);
            return longGenerator;
        }

        public static Random steamGenerator
        {
            get
            {
                if (Steam.user == null)
                    return new Random(90210);
                Random steamGenerator = new Random(Math.Abs((int)(Steam.user.id % int.MaxValue)));
                for (int index = 0; index < (int)(Steam.user.id % 252UL); ++index)
                    Rando.Int(100);
                return steamGenerator;
            }
        }

        public static Sprite GetEggSprite(int index = 0, ulong seed = 0)
        {
            if (seed == 0UL && Profiles.experienceProfile != null)
                seed = Profiles.experienceProfile.steamID;
            Sprite s = new Sprite();
            Graphics.AddRenderTask(() => s.texture = GetEggTexture(index, seed));
            return s;
        }

        public static Tex2D GetEggTexture(int index, ulong seed)
        {
            RenderTarget2D t = new RenderTarget2D(16, 16, false, RenderTargetUsage.PreserveContents);
            if (_egg == null)
            {
                _batch = new MTSpriteBatch(Graphics.device);
                _egg = new SpriteMap("online/eggWhite", 16, 16);
                _eggShine = new SpriteMap("online/eggShine", 16, 16);
                _eggBorder = new SpriteMap("online/eggBorder", 16, 16);
                _eggOuter = new SpriteMap("online/eggOuter", 16, 16);
                _eggSymbols = new SpriteMap("online/eggSymbols", 16, 16);
            }
            Random generator = Rando.generator;
            Rando.generator = GetLongGenerator(seed);
            for (int index1 = 0; index1 < index; ++index1)
                Rando.Int(100);
            bool flag1 = Rando.Float(1f) > 0.02f;
            bool flag2 = Rando.Float(1f) > 0.9f;
            bool flag3 = Rando.Float(1f) > 0.4f;
            bool flag4 = Rando.Int(8) == 1;
            _allowedColors = new List<Color>()
      {
        Colors.DGBlue,
        Colors.DGYellow,
        Colors.DGRed,
        Color.White,
        new Color(48, 224, 242),
        new Color(199, 234, 96)
      };
            _allowedColors.Add(Colors.DGPink);
            _allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            if (Rando.Int(6) == 1)
            {
                _allowedColors.Add(Colors.DGPurple);
                _allowedColors.Add(Colors.DGEgg);
            }
            else if (Rando.Int(100) == 1)
            {
                _allowedColors.Add(Colors.SuperDarkBlueGray);
                _allowedColors.Add(Colors.BlueGray);
                _allowedColors.Add(Colors.DGOrange);
                _allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            }
            else if (Rando.Int(1200) == 1)
                _allowedColors.Add(Colors.Platinum);
            else if (Rando.Int(100000) == 1)
                _allowedColors.Add(new Color(250, 10, 250));
            else if (Rando.Int(1000000) == 1)
                _allowedColors.Add(new Color(229, 245, 181));
            Graphics.SetRenderTarget(t);
            Graphics.Clear(Color.Black);
            _batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            //int num1 = 0;
            int num2 = 8 + Rando.Int(12);
            Rando.Int(100);
            // num1 = 1;
            int num3 = 3;
            if (Rando.Int(10) == 1)
                num2 = 1;
            if (Rando.Int(30) == 1)
                num2 = 5;
            if (Rando.Int(100) == 1)
                num2 = 2;
            else if (Rando.Int(200) == 1)
                num2 = 3;
            else if (Rando.Int(1000) == 1)
                num2 = 4;
            else if (Rando.Int(10000) == 1)
                num2 = 7;
            else if (Rando.Int(1000000) == 1)
                num2 = 6;
            bool flag5 = Rando.Int(300) == 1;
            MTSpriteBatch screen = Graphics.screen;
            Graphics.screen = _batch;
            _batch.Draw(_egg.texture, new Vec2(0f, 0f), new Rectangle?(new Rectangle(num3 * 16, 0f, 16f, 16f)), Color.White, 0f, new Vec2(0f, 0f), 1f, SpriteEffects.None, 1f);
            if (flag3)
            {
                if (flag5)
                {
                    char character = BitmapFont._characters[Rando.Int(33, 59)];
                    if (Rando.Int(5) == 1)
                        character = BitmapFont._characters[Rando.Int(16, 26)];
                    else if (Rando.Int(50) == 1)
                        character = BitmapFont._characters[Rando.Int(BitmapFont._characters.Length - 1)];
                    Graphics.DrawString(character.ToString() ?? "", new Vec2(4f, 6f), new Color(60, 60, 60, 200), (Depth)0.9f);
                }
                else
                    _batch.Draw(_eggSymbols.texture, new Vec2(0f, 0f), new Rectangle?(new Rectangle(num2 * 16, 0f, 16f, 16f)), new Color(60, 60, 60, 200), 0f, new Vec2(0f, 0f), 1f, SpriteEffects.None, 0.9f);
            }
            _batch.Draw(_eggOuter.texture, new Vec2(0f, 0f), new Rectangle?(new Rectangle(num3 * 16, 0f, 16f, 16f)), Color.White, 0f, new Vec2(0f, 0f), 1f, SpriteEffects.None, 1f);
            _batch.End();
            Graphics.screen = screen;
            Graphics.SetRenderTarget(null);
            Color[] data = t.GetData();
            float num4 = 0.09999999f;
            Color color1 = PickColor();
            Color color2 = PickColor();
            PickColor();
            Color color3 = PickColor();
            float num5 = Rando.Float(100000f);
            float num6 = Rando.Float(100000f);
            for (int index2 = 0; index2 < t.height; ++index2)
            {
                for (int index3 = 0; index3 < t.width; ++index3)
                {
                    float num7 = (index3 + 32) * 0.75f;
                    int num8 = index2 + 32;
                    float num9 = ((Noise.Generate((num5 + num7) * (num4 * 1f), (num6 + num8) * (num4 * 1f)) + 1f) / 2f * (flag1 ? 1f : 0f));
                    float num10 = ((Noise.Generate(num5 + ((num7 + 100f) * (num4 * 2f)), (num6 + num8 + 100f) * (num4 * 2f)) + 1f) / 2f * (flag2 ? 1f : 0f));
                    float num11 = num9 >= 0.5f ? 1f : 0f;
                    float num12 = num10 >= 0.5f ? 1f : 0f;
                    Color color4 = data[index3 + index2 * t.width];
                    float num13 = 1f;
                    if (num12 > 0f)
                        num13 = 0.9f;
                    if (color4.r == 0)
                        data[index3 + index2 * t.width] = Color.Transparent;
                    else if (color4.r < 110)
                    {
                        if (flag4)
                        {
                            data[index3 + index2 * t.width] = new Color((byte)(color3.r * 0.6f), (byte)(color3.g * 0.6f), (byte)(color3.b * 0.6f));
                        }
                        else
                        {
                            float num14 = num13 != 1f ? 1f : 0.9f;
                            if (num11 > 0f)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.6f * num14), (byte)(color1.g * 0.6f * num14), (byte)(color1.b * 0.6f * num14));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.6f * num14), (byte)(color2.g * 0.6f * num14), (byte)(color2.b * 0.6f * num14));
                        }
                    }
                    else if (color4.r < 120)
                    {
                        if (flag4)
                        {
                            data[index3 + index2 * t.width] = new Color(color3.r, color3.g, color3.b);
                        }
                        else
                        {
                            float num15 = num13 != 1f ? 1f : 0.9f;
                            if (num11 > 0f)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * num15), (byte)(color1.g * num15), (byte)(color1.b * num15));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * num15), (byte)(color2.g * num15), (byte)(color2.b * num15));
                        }
                    }
                    else if (color4.r < byte.MaxValue)
                    {
                        if (num11 > 0)
                            data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.6f * num13), (byte)(color2.g * 0.6f * num13), (byte)(color2.b * 0.6f * num13));
                        else
                            data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.6f * num13), (byte)(color1.g * 0.6f * num13), (byte)(color1.b * 0.6f * num13));
                    }
                    else if (num11 > 0)
                        data[index3 + index2 * t.width] = new Color((byte)(color2.r * num13), (byte)(color2.g * num13), (byte)(color2.b * num13));
                    else
                        data[index3 + index2 * t.width] = new Color((byte)(color1.r * num13), (byte)(color1.g * num13), (byte)(color1.b * num13));
                }
            }
            t.SetData(data);
            Graphics.SetRenderTarget(t);
            _batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            _batch.Draw(_eggShine.texture, new Vec2(0f, 0f), new Rectangle?(new Rectangle(num3 * 16, 0f, 16f, 16f)), Color.White, 0f, new Vec2(0f, 0f), 1f, SpriteEffects.None, 1f);
            _batch.Draw(_eggBorder.texture, new Vec2(0f, 0f), new Rectangle?(new Rectangle(num3 * 16, 0f, 16f, 16f)), Color.White, 0f, new Vec2(0f, 0f), 1f, SpriteEffects.None, 1f);
            _batch.End();
            Graphics.SetRenderTarget(null);
            Rando.generator = generator;
            Tex2D eggTexture = new Tex2D(t.width, t.height);
            eggTexture.SetData(t.GetData());
            t.Dispose();
            return eggTexture;
        }

        public static Sprite GetPaintingSprite(int index = 0, ulong seed = 0)
        {
            if (seed == 0UL && Profiles.experienceProfile != null)
                seed = Profiles.experienceProfile.steamID;
            Sprite s = new Sprite();
            Graphics.AddRenderTask(() => GetPainting(index, seed, s));
            return s;
        }

        public static void GetPainting(int index, ulong seed, Sprite spr)
        {
            Tex2D t = new RenderTarget2D(19, 12, false, RenderTargetUsage.PreserveContents);
            if (_easel == null)
            {
                _batch = new MTSpriteBatch(Graphics.device);
                _easel = new SpriteMap("online/easelWhite", 19, 12);
                _eggShine = new SpriteMap("online/eggShine", 16, 16);
                _eggBorder = new SpriteMap("online/eggBorder", 16, 16);
                _eggOuter = new SpriteMap("online/eggOuter", 16, 16);
                _easelSymbols = new SpriteMap("online/easelPic", 19, 12);
            }
            Random generator = Rando.generator;
            Rando.generator = GetLongGenerator(seed);
            for (int index1 = 0; index1 < index; ++index1)
                Rando.Int(100);
            bool flag1 = Rando.Float(1f) > 0.03f;
            bool flag2 = Rando.Float(1f) > 0.8f;
            double num1 = Rando.Float(1f);
            bool flag3 = Rando.Int(6) == 1;
            _allowedColors = new List<Color>()
      {
        Colors.DGBlue,
        Colors.DGYellow,
        Colors.DGRed,
        Color.White,
        new Color(48, 224, 242),
        new Color(199, 234, 96)
      };
            _allowedColors.Add(Colors.DGPink);
            _allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            if (Rando.Int(6) == 1)
            {
                _allowedColors.Add(Colors.DGPurple);
                _allowedColors.Add(Colors.DGEgg);
            }
            else if (Rando.Int(100) == 1)
            {
                _allowedColors.Add(Colors.SuperDarkBlueGray);
                _allowedColors.Add(Colors.BlueGray);
                _allowedColors.Add(Colors.DGOrange);
                _allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            }
            else if (Rando.Int(1200) == 1)
                _allowedColors.Add(Colors.Platinum);
            else if (Rando.Int(100000) == 1)
                _allowedColors.Add(new Color(250, 10, 250));
            else if (Rando.Int(1000000) == 1)
                _allowedColors.Add(new Color(229, 245, 181));
            Graphics.SetRenderTarget(t as RenderTarget2D);
            Graphics.Clear(Color.Black);
            _batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            int num2 = 8 + Rando.Int(12);
            Rando.Int(100);
            Rando.Int(15);
            Rando.Int(300);
            MTSpriteBatch screen = Graphics.screen;
            Graphics.screen = _batch;
            _batch.Draw(_easel.texture, new Vec2(0f, 0f), new Rectangle?(), Color.White, 0f, new Vec2(0f, 0f), 1f, SpriteEffects.None, 1f);
            _batch.Draw(_easelSymbols.texture, new Vec2(0f, 0f), new Rectangle?(new Rectangle(num2 * 19, 0f, 19f, 12f)), new Color(60, 60, 60, 200), 0f, new Vec2(0f, 0f), 1f, SpriteEffects.None, 0.9f);
            _batch.End();
            Graphics.screen = screen;
            Graphics.SetRenderTarget(null);
            Color[] data = t.GetData();
            float num3 = 0.09999999f;
            Color color1 = PickColor();
            Color color2 = PickColor();
            PickColor();
            Color color3 = PickColor();
            float num4 = Rando.Float(100000f);
            float num5 = Rando.Float(100000f);
            for (int index2 = 0; index2 < t.height; ++index2)
            {
                for (int index3 = 0; index3 < t.width; ++index3)
                {
                    float num6 = (index3 + 32) * 0.75f;
                    int num7 = index2 + 32;
                    float num8 = ((Noise.Generate(((num4 + num6) * (num3 * 1f)), ((num5 + num7) * (num3 * 1f))) + 1f) / 2f * (flag1 ? 1f : 0f));
                    float num9 = ((Noise.Generate(num4 + ((num6 + 100f) * (num3 * 2f)), ((num5 + num7 + 100f) * (num3 * 2f))) + 1f) / 2f * (flag2 ? 1f : 0f));
                    float num10 = num8 >= 0.5f ? 1f : 0f;
                    float num11 = num9 >= 0.5f ? 1f : 0f;
                    Color color4 = data[index3 + index2 * t.width];
                    float num12 = 1f;
                    if (num11 > 0f)
                        num12 = 0.9f;
                    if (color4.r == 0)
                        data[index3 + index2 * t.width] = Color.Transparent;
                    else if (color4.r < 110)
                    {
                        if (flag3)
                        {
                            data[index3 + index2 * t.width] = new Color((byte)(color3.r * 0.6f), (byte)(color3.g * 0.6f), (byte)(color3.b * 0.6f));
                        }
                        else
                        {
                            float num13 = num12 != 1f ? 1f : 0.9f;
                            if (num10 > 0f)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.6f * num13), (byte)(color1.g * 0.6f * num13), (byte)(color1.b * 0.6f * num13));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.6f * num13), (byte)(color2.g * 0.6f * num13), (byte)(color2.b * 0.6f * num13));
                        }
                    }
                    else if (color4.r < 120)
                    {
                        if (flag3)
                        {
                            data[index3 + index2 * t.width] = new Color(color3.r, color3.g, color3.b);
                        }
                        else
                        {
                            float num14 = num12 != 1f ? 1f : 0.9f;
                            if (num10 > 0)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * num14), (byte)(color1.g * num14), (byte)(color1.b * num14));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * num14), (byte)(color2.g * num14), (byte)(color2.b * num14));
                        }
                    }
                    else if (color4.r < byte.MaxValue)
                    {
                        if (num10 > 0f)
                            data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.6f * num12), (byte)(color2.g * 0.6f * num12), (byte)(color2.b * 0.6f * num12));
                        else
                            data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.6f * num12), (byte)(color1.g * 0.6f * num12), (byte)(color1.b * 0.6f * num12));
                    }
                    else if (num10 > 0f)
                        data[index3 + index2 * t.width] = new Color((byte)(color2.r * num12), (byte)(color2.g * num12), (byte)(color2.b * num12));
                    else
                        data[index3 + index2 * t.width] = new Color((byte)(color1.r * num12), (byte)(color1.g * num12), (byte)(color1.b * num12));
                }
            }
            t.SetData(data);
            Rando.generator = generator;
            spr.texture = t;
            Tex2D tex2D = new Tex2D(t.width, t.height);
            tex2D.SetData(t.GetData());
            t.Dispose();
            spr.texture = tex2D;
        }

        public List<string> unlocks
        {
            get => _unlocks;
            set => _unlocks = value;
        }

        public int ticketCount
        {
            get => _ticketCount;
            set
            {
                if (MonoMain.logFileOperations && _ticketCount != value)
                    DevConsole.Log(DCSection.General, "Profile(" + name != null ? name : ").ticketCount set(" + ticketCount.ToString() + ")");
                _ticketCount = value;
            }
        }

        public int timesMetVincent
        {
            get => _linkedProfile != null ? _linkedProfile.timesMetVincent : _timesMetVincent;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.timesMetVincent = value;
                _timesMetVincent = value;
            }
        }

        public int timesMetVincentSale
        {
            get => _linkedProfile != null ? _linkedProfile.timesMetVincentSale : _timesMetVincentSale;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.timesMetVincentSale = value;
                _timesMetVincentSale = value;
            }
        }

        public int timesMetVincentSell
        {
            get => _linkedProfile != null ? _linkedProfile.timesMetVincentSell : _timesMetVincentSell;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.timesMetVincentSell = value;
                _timesMetVincentSell = value;
            }
        }

        public int timesMetVincentImport
        {
            get => _linkedProfile != null ? _linkedProfile.timesMetVincentImport : _timesMetVincentImport;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.timesMetVincentImport = value;
                _timesMetVincentImport = value;
            }
        }

        public int timesMetVincentHint
        {
            get => _linkedProfile != null ? _linkedProfile.timesMetVincentHint : _timesMetVincentHint;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.timesMetVincentHint = value;
                _timesMetVincentHint = value;
            }
        }

        public int xp
        {
            get
            {
                if (_linkedProfile != null)
                    return _linkedProfile.xp;
                if (Steam.user == null || this != Profiles.experienceProfile || (int)Steam.GetStat(nameof(xp)) != 0)
                    return _xp;
                Steam.SetStat(nameof(xp), _xp);
                return _xp;
            }
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.xp = value;
                if (MonoMain.logFileOperations && _xp != value)
                    DevConsole.Log(DCSection.General, "Profile(" + name != null ? name : ").xp set(" + xp.ToString() + ")");
                if (Steam.user != null && this == Profiles.experienceProfile)
                    Steam.SetStat(nameof(xp), value);
                _xp = value;
            }
        }

        public static byte CalculateLocalFlippers()
        {
            bool flag1 = true;
            bool flag2 = true;
            foreach (ChallengeData challengeData in Challenges.challengesInArcade)
            {
                bool flag3 = false;
                bool flag4 = false;
                foreach (Profile universalProfile in Profiles.universalProfileList)
                {
                    ChallengeSaveData saveData = universalProfile.GetSaveData(challengeData.levelID, true);
                    if (saveData != null && saveData.trophy > TrophyType.Baseline)
                    {
                        flag4 = true;
                        if (saveData.trophy == TrophyType.Developer)
                        {
                            flag3 = true;
                            break;
                        }
                    }
                }
                if (!flag3)
                    flag1 = false;
                if (!flag4)
                    flag2 = false;
                if (!flag2)
                    break;
            }
            return (byte)((byte)((byte)((uint)(byte)((byte)((uint)(byte)((byte)((uint)(byte)((byte)((uint)(byte)(0 | (flag1 ? 1 : 0)) << 1) | ((int)Global.data.onlineWins >= 50 ? 1 : 0)) << 1) | ((int)Global.data.matchesPlayed >= 100 ? 1 : 0)) << 1) | (flag2 ? 1 : 0)) << 1) | (Options.Data.shennanigans ? 1 : 0)) | (Options.Data.rumbleIntensity > 0 ? 1 : 0));
        }

        public bool switchStatus => (flippers & 1U) > 0U;

        public bool GetLightStatus(int index) => (flippers >> index + 1 & 1) != 0;

        public float funslider
        {
            get => _funSlider;
            set => _funSlider = value;
        }

        public int preferredColor
        {
            get => linkedProfile != null ? linkedProfile.preferredColor : _preferredColor;
            set
            {
                if (linkedProfile != null)
                    linkedProfile.preferredColor = value;
                else
                    _preferredColor = value;
            }
        }

        public int requestedColor
        {
            get => linkedProfile != null ? linkedProfile.requestedColor : _requestedColor;
            set
            {
                if (linkedProfile != null)
                    linkedProfile.requestedColor = value;
                else
                    _requestedColor = value;
            }
        }

        public int currentColor => persona.index;

        public void IncrementRequestedColor()
        {
            int index;
            for (index = requestedColor + 1; index != requestedColor; ++index)
            {
                if (index >= DG.MaxPlayers)
                    index = 0;
                DuckPersona duckPersona = Persona.alllist[index];
                bool flag = false;
                foreach (Profile profile in Profiles.active)
                {
                    if (profile != this && profile.persona == duckPersona)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    break;
            }
            requestedColor = index;
        }

        public ProfileStats stats
        {
            get
            {
                if (!logStats)
                {
                    if (_junkStats == null)
                    {
                        DXMLNode node = _stats.Serialize();
                        _junkStats = new ProfileStats();
                        _junkStats.Deserialize(node);
                    }
                    return _junkStats;
                }
                _junkStats = null;
                return _linkedProfile != null ? _linkedProfile.stats : _stats;
            }
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.stats = value;
                _stats = value;
            }
        }

        public ProfileStats prevStats
        {
            get => _linkedProfile != null ? _linkedProfile.prevStats : _prevStats;
            set
            {
                if (_linkedProfile != null)
                    _linkedProfile.prevStats = value;
                _prevStats = value;
            }
        }

        public void RecordPreviousStats()
        {
            DXMLNode node = stats.Serialize();
            prevStats = new ProfileStats();
            prevStats.Deserialize(node);
            _endOfRoundStats = null;
        }

        public static int totalFansThisGame
        {
            get
            {
                int totalFansThisGame = 0;
                foreach (Profile profile in Profiles.active)
                {
                    if (profile.slotType != SlotType.Spectator)
                        totalFansThisGame += profile.stats.GetFans();
                }
                return totalFansThisGame;
            }
        }

        public ProfileStats endOfRoundStats
        {
            get
            {
                _endOfRoundStats = stats - prevStats as ProfileStats;
                return _endOfRoundStats;
            }
            set => _endOfRoundStats = value;
        }

        public CurrentGame currentGame => _currentGame;

        public ulong steamID
        {
            get
            {
                if (connection == DuckNetwork.localConnection && (!Network.isActive || !Network.lanMode))
                    return DG.localID;
                return connection != null && connection.data is User ? (connection.data as User).id : _steamID;
            }
            set
            {
                if ((long)_steamID != (long)value)
                    _blockStatusDirty = true;
                _steamID = value;
            }
        }

        public NetworkConnection connection
        {
            get => _connection;
            set
            {
                _connection = value;
                if (_connection != null)
                    return;
                _networkStatus = DuckNetStatus.Disconnected;
            }
        }

        public DuckNetStatus networkStatus
        {
            get => _networkStatus;
            set
            {
                if (!isRemoteLocalDuck && !_networkStatusLooping && connection != null)
                {
                    _networkStatusLooping = true;
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.connection == connection)
                            profile.networkStatus = value;
                    }
                    _networkStatusLooping = false;
                }
                else
                {
                    if (value != _networkStatus)
                    {
                        _currentStatusTimeout = 1f;
                        _currentStatusTries = 0;
                    }
                    _networkStatus = value;
                }
            }
        }

        public float currentStatusTimeout
        {
            get => _currentStatusTimeout;
            set => _currentStatusTimeout = value;
        }

        public int currentStatusTries
        {
            get => _currentStatusTries;
            set => _currentStatusTries = value;
        }

        public Profile linkedProfile
        {
            get => _linkedProfile;
            set => _linkedProfile = value;
        }

        public bool ReplayHost;
        public bool isHost => connection == Network.host;

        public bool ready
        {
            get => _ready;
            set => _ready = value;
        }

        public byte networkIndex => _networkIndex;

        public void SetNetworkIndex(byte idx) => _networkIndex = idx;

        public byte fixedGhostIndex => _fixedGhostIndex;

        public void SetFixedGhostIndex(byte idx) => _fixedGhostIndex = idx;

        public bool ReplayLocal;
        public bool localPlayer => !Network.isActive || _connection == DuckNetwork.localConnection;

        public byte remoteSpectatorChangeIndex
        {
            get => _remoteSpectatorChangeIndex;
            set
            {
                _remoteSpectatorChangeIndex = value;
                _spectatorChangeCooldown = 60;
            }
        }

        public bool readyForSpectatorChange => _spectatorChangeCooldown <= 0 && DuckNetwork.allClientsReady;

        public SlotType slotType
        {
            get => _slotType;
            set
            {
                if (_slotType == value)
                    return;
                _slotType = value;
                if (DuckNetwork.preparingProfiles || _slotType == SlotType.Spectator)
                    return;
                DuckNetwork.ChangeSlotSettings();
            }
        }

        public void Special_SetSlotType(SlotType pType) => _slotType = pType;

        public object reservedUser
        {
            get => _reservedUser;
            set => _reservedUser = value;
        }

        public Team reservedTeam
        {
            get => _reservedTeam;
            set => _reservedTeam = value;
        }

        public sbyte reservedSpectatorPersona
        {
            get => _reservedSpectatorPersona;
            set => _reservedSpectatorPersona = value;
        }

        public DuckPersona fallbackPersona => Network.isActive ? networkDefaultPersona : defaultPersona;

        public DuckPersona desiredPersona
        {
            get
            {
                if (requestedColor >= 0 && requestedColor < DG.MaxPlayers)
                    return Persona.alllist[requestedColor];
                return preferredColor >= 0 && preferredColor < DG.MaxPlayers ? Persona.alllist[preferredColor] : fallbackPersona;
            }
        }

        public void UpdatePersona()
        {
            DuckNetwork.RequestPersona(this, desiredPersona);
        }

        public void PersonaRequestResult(DuckPersona pPersona) => persona = pPersona;

        public Duck duck
        {
            get => _duck;
            set => _duck = value;
        }

        public DuckPersona persona
        {
            get
            {
                if (slotType == SlotType.Spectator)
                {
                    sbyte index = netData.Get<sbyte>("spectatorPersona", -1);
                    if (index >= 0 && index < 8)
                        return Persona.alllist[index];
                }
                if (_persona == null)
                    _persona = this != Profiles.DefaultPlayer1 ? (this != Profiles.DefaultPlayer2 ? (this != Profiles.DefaultPlayer3 ? (this != Profiles.DefaultPlayer4 ? Persona.Duck1 : Persona.Duck4) : Persona.Duck3) : Persona.Duck2) : Persona.Duck1;
                return _persona;
            }
            set => _persona = value;
        }

        public void SetInputProfileLink(InputProfile pLink) => _inputProfile = pLink;

        public static List<Profile> defaultProfileMappings
        {
            get
            {
                return Profiles.core.defaultProfileMappings;
            }
        }

        public InputProfile inputProfile
        {
            get
            {
                if (!Network.isActive && _inputProfile == null)
                    _inputProfile = this != Profiles.DefaultPlayer1 ? (this != Profiles.DefaultPlayer2 ? (this != Profiles.DefaultPlayer3 ? (this != Profiles.DefaultPlayer4 ? (this != Profiles.DefaultPlayer5 ? (this != Profiles.DefaultPlayer6 ? (this != Profiles.DefaultPlayer7 ? (this != Profiles.DefaultPlayer8 ? InputProfile.Get(InputProfile.MPPlayer1) : InputProfile.Get(InputProfile.MPPlayer8)) : InputProfile.Get(InputProfile.MPPlayer7)) : InputProfile.Get(InputProfile.MPPlayer6)) : InputProfile.Get(InputProfile.MPPlayer5)) : InputProfile.Get(InputProfile.MPPlayer4)) : InputProfile.Get(InputProfile.MPPlayer3)) : InputProfile.Get(InputProfile.MPPlayer2)) : InputProfile.Get(InputProfile.MPPlayer1);
                return _inputProfile;
            }
            set
            {
                if (_inputProfile != null && _inputProfile != value)
                {
                    _inputProfile.lastActiveDevice.Rumble();
                    Input.ApplyDefaultMapping(_inputProfile);
                }
                if (value != null && value != _inputProfile)
                    Input.ApplyDefaultMapping(value, this);
                _inputProfile = value;
            }
        }

        public Team team
        {
            get => _team;
            set
            {
                if (value != null)
                {
                    if (slotType != SlotType.Spectator)
                        value.Join(this, false);
                    _team = value;
                }
                else
                {
                    if (_team == null)
                        return;
                    _team.Leave(this, false);
                    _team = null;
                    requestedColor = -1;
                }
            }
        }

        public int wins
        {
            get => _wins;
            set => _wins = value;
        }

        public bool wasRockThrower
        {
            get => _wasRockThrower;
            set => _wasRockThrower = value;
        }

        public List<DeviceInputMapping> inputMappingOverrides => _linkedProfile != null ? _linkedProfile.inputMappingOverrides : _inputMappingOverrides;

        public void ClearCurrentGame() => _currentGame = new CurrentGame();

        public void ApplyDefaults()
        {
            team = defaultTeam != null ? defaultTeam : Teams.Player1;
            UpdatePersona();
        }

        public Profile(
          string varName,
          InputProfile varProfile,
          Team varStartTeam,
          DuckPersona varDefaultPersona,
          bool network,
          string varID,
          bool pDefaultProfile)
        {
            Main.SpecialCode = "can";
            _name = varName;
            _inputProfile = varProfile;
            if (_inputProfile != null)
                _inputProfile.oldAngles = false;
            if (varStartTeam != null)
            {
                varStartTeam.Join(this);
                defaultTeam = varStartTeam;
            }
            Main.SpecialCode = "you";
            _persona = varDefaultPersona;
            defaultPersona = varDefaultPersona;
            _id = varID != null ? varID : Guid.NewGuid().ToString();
            isNetworkProfile = network;
            isDefaultProfile = pDefaultProfile;
            if (!MonoMain.logFileOperations)
                return;
            DevConsole.Log(DCSection.General, "new Profile(" + varName + ")");
        }

        public Profile(
          string varName,
          InputProfile varProfile = null,
          Team varStartTeam = null,
          DuckPersona varDefaultPersona = null,
          bool network = false,
          string varID = null)
          : this(varName, varProfile, varStartTeam, varDefaultPersona, network, varID, false)
        {
        }
    }
}
