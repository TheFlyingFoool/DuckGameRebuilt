// Decompiled with JetBrains decompiler
// Type: DuckGame.Profile
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Profile
    {
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
        private bool isDefaultProfile;

        public void ReportConnectionTrouble(NetworkConnection pFrom) => this.connectionTrouble[pFrom] = 200;

        public void TickConnectionTrouble()
        {
            if (this.connectionTrouble.Count > 0)
            {
                foreach (NetworkConnection connection in Network.connections)
                {
                    if (this.connectionTrouble.ContainsKey(connection) && this.connectionTrouble[connection] > 0)
                        this.connectionTrouble[connection]--;
                }
            }
            if (this._spectatorChangeCooldown <= 0)
                return;
            --this._spectatorChangeCooldown;
        }

        public void HasConnectionFailed()
        {
            foreach (NetworkConnection connection in Network.connections)
            {
                if (this.connectionTrouble.ContainsKey(connection) && this.connectionTrouble[connection] > 0)
                    this.connectionTrouble[connection]--;
            }
        }

        public Team networkDefaultTeam => _networkIndex < DG.MaxPlayers ? Teams.all[_networkIndex] : Teams.all[Rando.Int(7)];

        public DuckPersona networkDefaultPersona => _networkIndex < DG.MaxPlayers ? Persona.all.ElementAt<DuckPersona>(_networkIndex) : Persona.Duck1;

        public Dictionary<string, ChallengeSaveData> challengeData => this._challengeData;

        public ChallengeSaveData GetSaveData(string guid, bool canBeNull = false)
        {
            ChallengeSaveData saveData1;
            if (this._challengeData.TryGetValue(guid, out saveData1))
                return saveData1;
            if (canBeNull)
                return null;
            ChallengeSaveData saveData2 = new ChallengeSaveData
            {
                profileID = this.id,
                challenge = guid
            };
            this._challengeData.Add(guid, saveData2);
            return saveData2;
        }

        private void RefreshBlockStatus()
        {
            this._blockStatusDirty = false;
            this._blocked = Options.Data.blockedPlayers != null && Options.Data.blockedPlayers.Contains(this.steamID);
            this._muteString = Options.GetMuteSettings(this);
        }

        public bool muteChat
        {
            get
            {
                if (this._blockStatusDirty)
                    this.RefreshBlockStatus();
                return this._muteString.Contains("C");
            }
            set
            {
                Options.SetMuteSetting(this, "C", value);
                this.RefreshBlockStatus();
            }
        }

        public bool muteHat
        {
            get
            {
                if (this._blockStatusDirty)
                    this.RefreshBlockStatus();
                return this._muteString.Contains("H");
            }
            set
            {
                Options.SetMuteSetting(this, "H", value);
                this.RefreshBlockStatus();
            }
        }

        public bool muteRoom
        {
            get
            {
                if (this._blockStatusDirty)
                    this.RefreshBlockStatus();
                return this._muteString.Contains("R");
            }
            set
            {
                Options.SetMuteSetting(this, "R", value);
                this.RefreshBlockStatus();
            }
        }

        public bool muteName
        {
            get
            {
                if (this._blockStatusDirty)
                    this.RefreshBlockStatus();
                return this._muteString.Contains("N");
            }
            set
            {
                Options.SetMuteSetting(this, "N", value);
                this.RefreshBlockStatus();
            }
        }

        public bool blocked
        {
            get
            {
                if (this._blockStatusDirty)
                    this.RefreshBlockStatus();
                return this._blocked;
            }
        }

        public bool spectator => this.slotType == SlotType.Spectator;

        public ushort customTeamIndexOffset => (ushort)(Teams.kCustomOffset + fixedGhostIndex * Teams.kCustomSpread);

        public int IndexOfCustomTeam(Team pTeam)
        {
            int num = this.customTeams.IndexOf(pTeam);
            return num >= 0 ? customTeamIndexOffset + num : num;
        }

        public Team GetCustomTeam(ushort pIndex)
        {
            if (this.connection == DuckNetwork.localConnection)
                return Teams.core.extraTeams.Count > pIndex && pIndex >= 0 ? Teams.core.extraTeams[pIndex] : null;
            while (this.customTeams.Count <= pIndex)
                this.customTeams.Add(new Team("CUSTOM", "hats/cluehat")
                {
                    owner = this
                });
            return this.customTeams[pIndex];
        }

        public bool ParentalControlsActive
        {
            get => this.connection == null || this.connection == DuckNetwork.localConnection ? ParentalControls.AreParentalControlsActive() : this._parentalControlsActive;
            set => this._parentalControlsActive = value;
        }

        public BitmapFont font
        {
            get
            {
                if (Profile._defaultFont == null)
                    Profile._defaultFont = new BitmapFont("biosFont", 8);
                BitmapFont font = Profile._defaultFont;
                foreach (FurniturePosition furniturePosition in this._furniturePositions)
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
                if (this._linkedProfile != null)
                    return this._linkedProfile._furniturePositions;
                this._internalFurniturePositions.RemoveAll(x => x == null);
                return this._internalFurniturePositions;
            }
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile._furniturePositions = value;
                else
                    this._internalFurniturePositions = value;
            }
        }

        public List<FurniturePosition> furniturePositions => this._furniturePositions;

        public BitBuffer furniturePositionData
        {
            get
            {
                if (this._linkedProfile != null)
                    return this._linkedProfile.furniturePositionData;
                BitBuffer furniturePositionData = new BitBuffer();
                foreach (FurniturePosition furniturePosition in this._furniturePositions)
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
                if (this._linkedProfile != null)
                {
                    this._linkedProfile.furniturePositionData = value;
                }
                else
                {
                    this._furniturePositions.Clear();
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
                                    this._furniturePositions.Add(furniturePosition);
                                    if (furniture.name == "PERIMETER DEFENCE")
                                        ++num;
                                }
                            }
                        }
                        if (num <= 1)
                            return;
                        this._furniturePositions.RemoveAll(x => x.furniMapping.name == "PERIMETER DEFENCE");
                    }
                    catch (Exception)
                    {
                        DevConsole.Log(DCSection.General, "Failed to load furniture position data.");
                        this._furniturePositions.Clear();
                    }
                }
            }
        }

        public int GetNumFurnituresPlaced(int idx)
        {
            int furnituresPlaced = 0;
            foreach (FurniturePosition furniturePosition in this._furniturePositions)
            {
                if (furniturePosition.id == idx)
                    ++furnituresPlaced;
            }
            return furnituresPlaced;
        }

        public int GetTotalFurnituresPlaced()
        {
            int furnituresPlaced = 0;
            foreach (FurniturePosition furniturePosition in this._furniturePositions)
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
                if (this._linkedProfile != null)
                    return this._linkedProfile.furnitureOwnershipData;
                BitBuffer furnitureOwnershipData = new BitBuffer();
                foreach (KeyValuePair<int, int> furniture in this._furnitures)
                {
                    furnitureOwnershipData.Write(furniture.Key);
                    furnitureOwnershipData.Write(furniture.Value);
                }
                return furnitureOwnershipData;
            }
            set
            {
                if (this._linkedProfile != null)
                {
                    this._linkedProfile.furnitureOwnershipData = value;
                }
                else
                {
                    this._furnitures.Clear();
                    this._availableList = null;
                    try
                    {
                        while (value.position != value.lengthInBytes)
                        {
                            FurniturePosition furniturePosition = new FurniturePosition();
                            this._furnitures[value.ReadInt()] = value.ReadInt();
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
            this._furnitures.Clear();
            this._furniturePositions.Clear();
        }

        public Dictionary<int, int> _furnitures
        {
            get => this._linkedProfile != null ? this._linkedProfile._furnitures : this._internalFurnitures;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile._furnitures = value;
                else
                    this._internalFurnitures = value;
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
            this._furnitures.TryGetValue(idx, out num);
            return furniture.name == "PERIMETER DEFENCE" && num > 1 ? 1 : num;
        }

        public void SetNumFurnitures(int idx, int num)
        {
            this._furnitures[idx] = num;
            this._availableList = null;
        }

        public int GetTotalFurnitures()
        {
            int totalFurnitures = 0;
            foreach (KeyValuePair<int, int> furniture in this._furnitures)
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

        private static string AvailFurniSortKey(Furniture x) => x.group.name + Profile.Stringlonger(RoomEditor._furniGroupMap[x.group].IndexOf(x));

        public List<Furniture> GetAvailableFurnis()
        {
            if (this._availableList == null)
            {
                this._availableList = new List<Furniture>();
                foreach (KeyValuePair<int, int> furniture1 in this._furnitures)
                {
                    if (furniture1.Value > 0)
                    {
                        Furniture furniture2 = RoomEditor.GetFurniture(furniture1.Key);
                        if (furniture2 != null)
                            this._availableList.Add(furniture2);
                    }
                }
                foreach (Furniture allFurni in RoomEditor.AllFurnis())
                {
                    if (allFurni.alwaysHave)
                        this._availableList.Add(allFurni);
                }
                this._availableList.Sort((x, y) => Profile.AvailFurniSortKey(x).CompareTo(Profile.AvailFurniSortKey(y)));
            }
            return this._availableList;
        }

        public int roundsSinceXP
        {
            get => this._linkedProfile != null ? this._linkedProfile.roundsSinceXP : this._roundsSinceXP;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.roundsSinceXP = value;
                this._roundsSinceXP = value;
            }
        }

        public int littleManBucks
        {
            get => this._linkedProfile != null ? this._linkedProfile.littleManBucks : this._littleManBucks;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.littleManBucks = value;
                this._littleManBucks = value;
            }
        }

        public int numLittleMen
        {
            get => this._linkedProfile != null ? this._linkedProfile.numLittleMen : this._numLittleMen;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.numLittleMen = value;
                this._numLittleMen = value;
            }
        }

        public int littleManLevel
        {
            get => this._linkedProfile != null ? this._linkedProfile.littleManLevel : this._littleManLevel;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.littleManLevel = value;
                this._littleManLevel = value;
            }
        }

        public int milkFill
        {
            get => this._linkedProfile != null ? this._linkedProfile.milkFill : this._milkFill;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.milkFill = value;
                this._milkFill = value;
            }
        }

        public int numSandwiches
        {
            get => this._linkedProfile != null ? this._linkedProfile.numSandwiches : this._numSandwiches;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.numSandwiches = value;
                this._numSandwiches = value;
            }
        }

        public int currentDay
        {
            get => this._linkedProfile != null ? this._linkedProfile.currentDay : this._currentDay;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.currentDay = value;
                this._currentDay = value;
            }
        }

        public int punished
        {
            get => this._linkedProfile != null ? this._linkedProfile.punished : this._punished;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.punished = value;
                this._punished = value;
            }
        }

        public string formattedName
        {
            get
            {
                string rawName = this.rawName;
                if (this.steamID != 0UL)
                    rawName = this.steamID.ToString();
                return rawName;
            }
        }

        public string rawName => this._name;

        public string nameUI
        {
            get
            {
                string nameUi = this.name;
                if (this.muteName)
                    nameUi = "Player " + (networkIndex + 1).ToString();
                return nameUi;
            }
        }

        public string name
        {
            get
            {
                if (this.linkedProfile != null && this.connection == DuckNetwork.localConnection && !Profiles.IsExperience(this))
                    return this.linkedProfile.name;
                if (this.keepSetName || this.steamID == 0UL || this.slotType == SlotType.Local)
                    return this._name;
                if (Steam.user != null && (long)this.steamID == (long)Steam.user.id)
                    return Steam.user.name;
                if (this.lastKnownName != null)
                    return this.lastKnownName;
                if (!(this._name == this.steamID.ToString()))
                    return this._name;
                if (Steam.IsInitialized())
                {
                    User user = User.GetUser(this.steamID);
                    if (user != null && user.id != 0UL)
                    {
                        this.lastKnownName = user.name;
                        return this.lastKnownName;
                    }
                }
                return "STEAM PROFILE";
            }
            set => this._name = value;
        }

        public static bool logStats => true;

        public string id => this._id;

        public void SetID(string varID) => this._id = varID;

        private static Color PickColor()
        {
            int index = Rando.Int(Profile._allowedColors.Count - 1);
            Color allowedColor = Profile._allowedColors[index];
            Profile._allowedColors.RemoveAt(index);
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
            DuckGame.Graphics.AddRenderTask(() => s.texture = Profile.GetEggTexture(index, seed));
            return s;
        }

        public static Tex2D GetEggTexture(int index, ulong seed)
        {
            RenderTarget2D t = new RenderTarget2D(16, 16, false, RenderTargetUsage.PreserveContents);
            if (Profile._egg == null)
            {
                Profile._batch = new MTSpriteBatch(DuckGame.Graphics.device);
                Profile._egg = new SpriteMap("online/eggWhite", 16, 16);
                Profile._eggShine = new SpriteMap("online/eggShine", 16, 16);
                Profile._eggBorder = new SpriteMap("online/eggBorder", 16, 16);
                Profile._eggOuter = new SpriteMap("online/eggOuter", 16, 16);
                Profile._eggSymbols = new SpriteMap("online/eggSymbols", 16, 16);
            }
            Random generator = Rando.generator;
            Rando.generator = Profile.GetLongGenerator(seed);
            for (int index1 = 0; index1 < index; ++index1)
                Rando.Int(100);
            bool flag1 = (double)Rando.Float(1f) > 0.0199999995529652;
            bool flag2 = (double)Rando.Float(1f) > 0.899999976158142;
            bool flag3 = (double)Rando.Float(1f) > 0.400000005960464;
            bool flag4 = Rando.Int(8) == 1;
            Profile._allowedColors = new List<Color>()
      {
        Colors.DGBlue,
        Colors.DGYellow,
        Colors.DGRed,
        Color.White,
        new Color(48, 224, 242),
        new Color(199, 234, 96)
      };
            Profile._allowedColors.Add(Colors.DGPink);
            Profile._allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            if (Rando.Int(6) == 1)
            {
                Profile._allowedColors.Add(Colors.DGPurple);
                Profile._allowedColors.Add(Colors.DGEgg);
            }
            else if (Rando.Int(100) == 1)
            {
                Profile._allowedColors.Add(Colors.SuperDarkBlueGray);
                Profile._allowedColors.Add(Colors.BlueGray);
                Profile._allowedColors.Add(Colors.DGOrange);
                Profile._allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            }
            else if (Rando.Int(1200) == 1)
                Profile._allowedColors.Add(Colors.Platinum);
            else if (Rando.Int(100000) == 1)
                Profile._allowedColors.Add(new Color(250, 10, 250));
            else if (Rando.Int(1000000) == 1)
                Profile._allowedColors.Add(new Color(229, 245, 181));
            DuckGame.Graphics.SetRenderTarget(t);
            DuckGame.Graphics.Clear(Color.Black);
            Profile._batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
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
            MTSpriteBatch screen = DuckGame.Graphics.screen;
            DuckGame.Graphics.screen = Profile._batch;
            Profile._batch.Draw(Profile._egg.texture, new Vec2(0.0f, 0.0f), new Rectangle?(new Rectangle(num3 * 16, 0.0f, 16f, 16f)), Color.White, 0.0f, new Vec2(0.0f, 0.0f), 1f, SpriteEffects.None, 1f);
            if (flag3)
            {
                if (flag5)
                {
                    char character = BitmapFont._characters[Rando.Int(33, 59)];
                    if (Rando.Int(5) == 1)
                        character = BitmapFont._characters[Rando.Int(16, 26)];
                    else if (Rando.Int(50) == 1)
                        character = BitmapFont._characters[Rando.Int(BitmapFont._characters.Length - 1)];
                    DuckGame.Graphics.DrawString(character.ToString() ?? "", new Vec2(4f, 6f), new Color(60, 60, 60, 200), (Depth)0.9f);
                }
                else
                    Profile._batch.Draw(Profile._eggSymbols.texture, new Vec2(0.0f, 0.0f), new Rectangle?(new Rectangle(num2 * 16, 0.0f, 16f, 16f)), new Color(60, 60, 60, 200), 0.0f, new Vec2(0.0f, 0.0f), 1f, SpriteEffects.None, 0.9f);
            }
            Profile._batch.Draw(Profile._eggOuter.texture, new Vec2(0.0f, 0.0f), new Rectangle?(new Rectangle(num3 * 16, 0.0f, 16f, 16f)), Color.White, 0.0f, new Vec2(0.0f, 0.0f), 1f, SpriteEffects.None, 1f);
            Profile._batch.End();
            DuckGame.Graphics.screen = screen;
            DuckGame.Graphics.SetRenderTarget(null);
            Color[] data = t.GetData();
            float num4 = 0.09999999f;
            Color color1 = Profile.PickColor();
            Color color2 = Profile.PickColor();
            Profile.PickColor();
            Color color3 = Profile.PickColor();
            float num5 = Rando.Float(100000f);
            float num6 = Rando.Float(100000f);
            for (int index2 = 0; index2 < t.height; ++index2)
            {
                for (int index3 = 0; index3 < t.width; ++index3)
                {
                    float num7 = (index3 + 32) * 0.75f;
                    int num8 = index2 + 32;
                    float num9 = (float)(((double)Noise.Generate((float)(((double)num5 + (double)num7) * ((double)num4 * 1.0)), (float)(((double)num6 + num8) * ((double)num4 * 1.0))) + 1.0) / 2.0 * (flag1 ? 1.0 : 0.0));
                    float num10 = (float)(((double)Noise.Generate(num5 + (float)(((double)num7 + 100.0) * ((double)num4 * 2.0)), (float)(((double)num6 + num8 + 100.0) * ((double)num4 * 2.0))) + 1.0) / 2.0 * (flag2 ? 1.0 : 0.0));
                    float num11 = (double)num9 >= 0.5 ? 1f : 0.0f;
                    float num12 = (double)num10 >= 0.5 ? 1f : 0.0f;
                    Color color4 = data[index3 + index2 * t.width];
                    float num13 = 1f;
                    if ((double)num12 > 0.0)
                        num13 = 0.9f;
                    if (color4.r == 0)
                        data[index3 + index2 * t.width] = new Color(0, 0, 0, 0);
                    else if (color4.r < 110)
                    {
                        if (flag4)
                        {
                            data[index3 + index2 * t.width] = new Color((byte)(color3.r * 0.600000023841858), (byte)(color3.g * 0.600000023841858), (byte)(color3.b * 0.600000023841858));
                        }
                        else
                        {
                            float num14 = (double)num13 != 1.0 ? 1f : 0.9f;
                            if ((double)num11 > 0.0)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.600000023841858 * (double)num14), (byte)(color1.g * 0.600000023841858 * (double)num14), (byte)(color1.b * 0.600000023841858 * (double)num14));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.600000023841858 * (double)num14), (byte)(color2.g * 0.600000023841858 * (double)num14), (byte)(color2.b * 0.600000023841858 * (double)num14));
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
                            float num15 = (double)num13 != 1.0 ? 1f : 0.9f;
                            if ((double)num11 > 0.0)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * (double)num15), (byte)(color1.g * (double)num15), (byte)(color1.b * (double)num15));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * (double)num15), (byte)(color2.g * (double)num15), (byte)(color2.b * (double)num15));
                        }
                    }
                    else if (color4.r < byte.MaxValue)
                    {
                        if ((double)num11 > 0.0)
                            data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.600000023841858 * (double)num13), (byte)(color2.g * 0.600000023841858 * (double)num13), (byte)(color2.b * 0.600000023841858 * (double)num13));
                        else
                            data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.600000023841858 * (double)num13), (byte)(color1.g * 0.600000023841858 * (double)num13), (byte)(color1.b * 0.600000023841858 * (double)num13));
                    }
                    else if ((double)num11 > 0.0)
                        data[index3 + index2 * t.width] = new Color((byte)(color2.r * (double)num13), (byte)(color2.g * (double)num13), (byte)(color2.b * (double)num13));
                    else
                        data[index3 + index2 * t.width] = new Color((byte)(color1.r * (double)num13), (byte)(color1.g * (double)num13), (byte)(color1.b * (double)num13));
                }
            }
            t.SetData(data);
            DuckGame.Graphics.SetRenderTarget(t);
            Profile._batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            Profile._batch.Draw(Profile._eggShine.texture, new Vec2(0.0f, 0.0f), new Rectangle?(new Rectangle(num3 * 16, 0.0f, 16f, 16f)), Color.White, 0.0f, new Vec2(0.0f, 0.0f), 1f, SpriteEffects.None, 1f);
            Profile._batch.Draw(Profile._eggBorder.texture, new Vec2(0.0f, 0.0f), new Rectangle?(new Rectangle(num3 * 16, 0.0f, 16f, 16f)), Color.White, 0.0f, new Vec2(0.0f, 0.0f), 1f, SpriteEffects.None, 1f);
            Profile._batch.End();
            DuckGame.Graphics.SetRenderTarget(null);
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
            DuckGame.Graphics.AddRenderTask(() => Profile.GetPainting(index, seed, s));
            return s;
        }

        public static void GetPainting(int index, ulong seed, Sprite spr)
        {
            Tex2D t = new RenderTarget2D(19, 12, false, RenderTargetUsage.PreserveContents);
            if (Profile._easel == null)
            {
                Profile._batch = new MTSpriteBatch(DuckGame.Graphics.device);
                Profile._easel = new SpriteMap("online/easelWhite", 19, 12);
                Profile._eggShine = new SpriteMap("online/eggShine", 16, 16);
                Profile._eggBorder = new SpriteMap("online/eggBorder", 16, 16);
                Profile._eggOuter = new SpriteMap("online/eggOuter", 16, 16);
                Profile._easelSymbols = new SpriteMap("online/easelPic", 19, 12);
            }
            Random generator = Rando.generator;
            Rando.generator = Profile.GetLongGenerator(seed);
            for (int index1 = 0; index1 < index; ++index1)
                Rando.Int(100);
            bool flag1 = (double)Rando.Float(1f) > 0.0299999993294477;
            bool flag2 = (double)Rando.Float(1f) > 0.800000011920929;
            double num1 = (double)Rando.Float(1f);
            bool flag3 = Rando.Int(6) == 1;
            Profile._allowedColors = new List<Color>()
      {
        Colors.DGBlue,
        Colors.DGYellow,
        Colors.DGRed,
        Color.White,
        new Color(48, 224, 242),
        new Color(199, 234, 96)
      };
            Profile._allowedColors.Add(Colors.DGPink);
            Profile._allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            if (Rando.Int(6) == 1)
            {
                Profile._allowedColors.Add(Colors.DGPurple);
                Profile._allowedColors.Add(Colors.DGEgg);
            }
            else if (Rando.Int(100) == 1)
            {
                Profile._allowedColors.Add(Colors.SuperDarkBlueGray);
                Profile._allowedColors.Add(Colors.BlueGray);
                Profile._allowedColors.Add(Colors.DGOrange);
                Profile._allowedColors.Add(new Color((byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200)), (byte)(54 + Rando.Int(200))));
            }
            else if (Rando.Int(1200) == 1)
                Profile._allowedColors.Add(Colors.Platinum);
            else if (Rando.Int(100000) == 1)
                Profile._allowedColors.Add(new Color(250, 10, 250));
            else if (Rando.Int(1000000) == 1)
                Profile._allowedColors.Add(new Color(229, 245, 181));
            DuckGame.Graphics.SetRenderTarget(t as RenderTarget2D);
            DuckGame.Graphics.Clear(Color.Black);
            Profile._batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            int num2 = 8 + Rando.Int(12);
            Rando.Int(100);
            Rando.Int(15);
            Rando.Int(300);
            MTSpriteBatch screen = DuckGame.Graphics.screen;
            DuckGame.Graphics.screen = Profile._batch;
            Profile._batch.Draw(Profile._easel.texture, new Vec2(0.0f, 0.0f), new Rectangle?(), Color.White, 0.0f, new Vec2(0.0f, 0.0f), 1f, SpriteEffects.None, 1f);
            Profile._batch.Draw(Profile._easelSymbols.texture, new Vec2(0.0f, 0.0f), new Rectangle?(new Rectangle(num2 * 19, 0.0f, 19f, 12f)), new Color(60, 60, 60, 200), 0.0f, new Vec2(0.0f, 0.0f), 1f, SpriteEffects.None, 0.9f);
            Profile._batch.End();
            DuckGame.Graphics.screen = screen;
            DuckGame.Graphics.SetRenderTarget(null);
            Color[] data = t.GetData();
            float num3 = 0.09999999f;
            Color color1 = Profile.PickColor();
            Color color2 = Profile.PickColor();
            Profile.PickColor();
            Color color3 = Profile.PickColor();
            float num4 = Rando.Float(100000f);
            float num5 = Rando.Float(100000f);
            for (int index2 = 0; index2 < t.height; ++index2)
            {
                for (int index3 = 0; index3 < t.width; ++index3)
                {
                    float num6 = (index3 + 32) * 0.75f;
                    int num7 = index2 + 32;
                    float num8 = (float)(((double)Noise.Generate((float)(((double)num4 + (double)num6) * ((double)num3 * 1.0)), (float)(((double)num5 + num7) * ((double)num3 * 1.0))) + 1.0) / 2.0 * (flag1 ? 1.0 : 0.0));
                    float num9 = (float)(((double)Noise.Generate(num4 + (float)(((double)num6 + 100.0) * ((double)num3 * 2.0)), (float)(((double)num5 + num7 + 100.0) * ((double)num3 * 2.0))) + 1.0) / 2.0 * (flag2 ? 1.0 : 0.0));
                    float num10 = (double)num8 >= 0.5 ? 1f : 0.0f;
                    float num11 = (double)num9 >= 0.5 ? 1f : 0.0f;
                    Color color4 = data[index3 + index2 * t.width];
                    float num12 = 1f;
                    if ((double)num11 > 0.0)
                        num12 = 0.9f;
                    if (color4.r == 0)
                        data[index3 + index2 * t.width] = new Color(0, 0, 0, 0);
                    else if (color4.r < 110)
                    {
                        if (flag3)
                        {
                            data[index3 + index2 * t.width] = new Color((byte)(color3.r * 0.600000023841858), (byte)(color3.g * 0.600000023841858), (byte)(color3.b * 0.600000023841858));
                        }
                        else
                        {
                            float num13 = (double)num12 != 1.0 ? 1f : 0.9f;
                            if ((double)num10 > 0.0)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.600000023841858 * (double)num13), (byte)(color1.g * 0.600000023841858 * (double)num13), (byte)(color1.b * 0.600000023841858 * (double)num13));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.600000023841858 * (double)num13), (byte)(color2.g * 0.600000023841858 * (double)num13), (byte)(color2.b * 0.600000023841858 * (double)num13));
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
                            float num14 = (double)num12 != 1.0 ? 1f : 0.9f;
                            if ((double)num10 > 0.0)
                                data[index3 + index2 * t.width] = new Color((byte)(color1.r * (double)num14), (byte)(color1.g * (double)num14), (byte)(color1.b * (double)num14));
                            else
                                data[index3 + index2 * t.width] = new Color((byte)(color2.r * (double)num14), (byte)(color2.g * (double)num14), (byte)(color2.b * (double)num14));
                        }
                    }
                    else if (color4.r < byte.MaxValue)
                    {
                        if ((double)num10 > 0.0)
                            data[index3 + index2 * t.width] = new Color((byte)(color2.r * 0.600000023841858 * (double)num12), (byte)(color2.g * 0.600000023841858 * (double)num12), (byte)(color2.b * 0.600000023841858 * (double)num12));
                        else
                            data[index3 + index2 * t.width] = new Color((byte)(color1.r * 0.600000023841858 * (double)num12), (byte)(color1.g * 0.600000023841858 * (double)num12), (byte)(color1.b * 0.600000023841858 * (double)num12));
                    }
                    else if ((double)num10 > 0.0)
                        data[index3 + index2 * t.width] = new Color((byte)(color2.r * (double)num12), (byte)(color2.g * (double)num12), (byte)(color2.b * (double)num12));
                    else
                        data[index3 + index2 * t.width] = new Color((byte)(color1.r * (double)num12), (byte)(color1.g * (double)num12), (byte)(color1.b * (double)num12));
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
            get => this._unlocks;
            set => this._unlocks = value;
        }

        public int ticketCount
        {
            get => this._ticketCount;
            set
            {
                if (MonoMain.logFileOperations && this._ticketCount != value)
                    DevConsole.Log(DCSection.General, "Profile(" + this.name != null ? this.name : ").ticketCount set(" + this.ticketCount.ToString() + ")");
                this._ticketCount = value;
            }
        }

        public int timesMetVincent
        {
            get => this._linkedProfile != null ? this._linkedProfile.timesMetVincent : this._timesMetVincent;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.timesMetVincent = value;
                this._timesMetVincent = value;
            }
        }

        public int timesMetVincentSale
        {
            get => this._linkedProfile != null ? this._linkedProfile.timesMetVincentSale : this._timesMetVincentSale;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.timesMetVincentSale = value;
                this._timesMetVincentSale = value;
            }
        }

        public int timesMetVincentSell
        {
            get => this._linkedProfile != null ? this._linkedProfile.timesMetVincentSell : this._timesMetVincentSell;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.timesMetVincentSell = value;
                this._timesMetVincentSell = value;
            }
        }

        public int timesMetVincentImport
        {
            get => this._linkedProfile != null ? this._linkedProfile.timesMetVincentImport : this._timesMetVincentImport;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.timesMetVincentImport = value;
                this._timesMetVincentImport = value;
            }
        }

        public int timesMetVincentHint
        {
            get => this._linkedProfile != null ? this._linkedProfile.timesMetVincentHint : this._timesMetVincentHint;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.timesMetVincentHint = value;
                this._timesMetVincentHint = value;
            }
        }

        public int xp
        {
            get
            {
                if (this._linkedProfile != null)
                    return this._linkedProfile.xp;
                if (Steam.user == null || this != Profiles.experienceProfile || (int)Steam.GetStat(nameof(xp)) != 0)
                    return this._xp;
                Steam.SetStat(nameof(xp), this._xp);
                return this._xp;
            }
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.xp = value;
                if (MonoMain.logFileOperations && this._xp != value)
                    DevConsole.Log(DCSection.General, "Profile(" + this.name != null ? this.name : ").xp set(" + this.xp.ToString() + ")");
                if (Steam.user != null && this == Profiles.experienceProfile)
                    Steam.SetStat(nameof(xp), value);
                this._xp = value;
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
            return (byte)((byte)((byte)((uint)(byte)((byte)((uint)(byte)((byte)((uint)(byte)((byte)((uint)(byte)(0 | (flag1 ? 1 : 0)) << 1) | ((int)Global.data.onlineWins >= 50 ? 1 : 0)) << 1) | ((int)Global.data.matchesPlayed >= 100 ? 1 : 0)) << 1) | (flag2 ? 1 : 0)) << 1) | (Options.Data.shennanigans ? 1 : 0)) | ((double)Options.Data.rumbleIntensity > 0.0 ? 1 : 0));
        }

        public bool switchStatus => (flippers & 1U) > 0U;

        public bool GetLightStatus(int index) => (flippers >> index + 1 & 1) != 0;

        public float funslider
        {
            get => this._funSlider;
            set => this._funSlider = value;
        }

        public int preferredColor
        {
            get => this.linkedProfile != null ? this.linkedProfile.preferredColor : this._preferredColor;
            set
            {
                if (this.linkedProfile != null)
                    this.linkedProfile.preferredColor = value;
                else
                    this._preferredColor = value;
            }
        }

        public int requestedColor
        {
            get => this.linkedProfile != null ? this.linkedProfile.requestedColor : this._requestedColor;
            set
            {
                if (this.linkedProfile != null)
                    this.linkedProfile.requestedColor = value;
                else
                    this._requestedColor = value;
            }
        }

        public int currentColor => this.persona.index;

        public void IncrementRequestedColor()
        {
            int index;
            for (index = this.requestedColor + 1; index != this.requestedColor; ++index)
            {
                if (index >= DG.MaxPlayers)
                    index = 0;
                DuckPersona duckPersona = Persona.all.ElementAt<DuckPersona>(index);
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
            this.requestedColor = index;
        }

        public ProfileStats stats
        {
            get
            {
                if (!Profile.logStats)
                {
                    if (this._junkStats == null)
                    {
                        DXMLNode node = this._stats.Serialize();
                        this._junkStats = new ProfileStats();
                        this._junkStats.Deserialize(node);
                    }
                    return this._junkStats;
                }
                this._junkStats = null;
                return this._linkedProfile != null ? this._linkedProfile.stats : this._stats;
            }
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.stats = value;
                this._stats = value;
            }
        }

        public ProfileStats prevStats
        {
            get => this._linkedProfile != null ? this._linkedProfile.prevStats : this._prevStats;
            set
            {
                if (this._linkedProfile != null)
                    this._linkedProfile.prevStats = value;
                this._prevStats = value;
            }
        }

        public void RecordPreviousStats()
        {
            DXMLNode node = this.stats.Serialize();
            this.prevStats = new ProfileStats();
            this.prevStats.Deserialize(node);
            this._endOfRoundStats = null;
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
                this._endOfRoundStats = stats - prevStats as ProfileStats;
                return this._endOfRoundStats;
            }
            set => this._endOfRoundStats = value;
        }

        public CurrentGame currentGame => this._currentGame;

        public ulong steamID
        {
            get
            {
                if (this.connection == DuckNetwork.localConnection && (!Network.isActive || !Network.lanMode))
                    return DG.localID;
                return this.connection != null && this.connection.data is User ? (this.connection.data as User).id : this._steamID;
            }
            set
            {
                if ((long)this._steamID != (long)value)
                    this._blockStatusDirty = true;
                this._steamID = value;
            }
        }

        public NetworkConnection connection
        {
            get => this._connection;
            set
            {
                this._connection = value;
                if (this._connection != null)
                    return;
                this._networkStatus = DuckNetStatus.Disconnected;
            }
        }

        public DuckNetStatus networkStatus
        {
            get => this._networkStatus;
            set
            {
                if (!this.isRemoteLocalDuck && !Profile._networkStatusLooping && this.connection != null)
                {
                    Profile._networkStatusLooping = true;
                    foreach (Profile profile in DuckNetwork.profiles)
                    {
                        if (profile.connection == this.connection)
                            profile.networkStatus = value;
                    }
                    Profile._networkStatusLooping = false;
                }
                else
                {
                    if (value != this._networkStatus)
                    {
                        this._currentStatusTimeout = 1f;
                        this._currentStatusTries = 0;
                    }
                    this._networkStatus = value;
                }
            }
        }

        public float currentStatusTimeout
        {
            get => this._currentStatusTimeout;
            set => this._currentStatusTimeout = value;
        }

        public int currentStatusTries
        {
            get => this._currentStatusTries;
            set => this._currentStatusTries = value;
        }

        public Profile linkedProfile
        {
            get => this._linkedProfile;
            set => this._linkedProfile = value;
        }

        public bool isHost => this.connection == Network.host;

        public bool ready
        {
            get => this._ready;
            set => this._ready = value;
        }

        public byte networkIndex => this._networkIndex;

        public void SetNetworkIndex(byte idx) => this._networkIndex = idx;

        public byte fixedGhostIndex => this._fixedGhostIndex;

        public void SetFixedGhostIndex(byte idx) => this._fixedGhostIndex = idx;

        public bool localPlayer => !Network.isActive || this._connection == DuckNetwork.localConnection;

        public byte remoteSpectatorChangeIndex
        {
            get => this._remoteSpectatorChangeIndex;
            set
            {
                this._remoteSpectatorChangeIndex = value;
                this._spectatorChangeCooldown = 120;
            }
        }

        public bool readyForSpectatorChange => this._spectatorChangeCooldown <= 0 && DuckNetwork.allClientsReady;

        public SlotType slotType
        {
            get => this._slotType;
            set
            {
                if (this._slotType == value)
                    return;
                this._slotType = value;
                if (DuckNetwork.preparingProfiles || this._slotType == SlotType.Spectator)
                    return;
                DuckNetwork.ChangeSlotSettings();
            }
        }

        public void Special_SetSlotType(SlotType pType) => this._slotType = pType;

        public object reservedUser
        {
            get => this._reservedUser;
            set => this._reservedUser = value;
        }

        public Team reservedTeam
        {
            get => this._reservedTeam;
            set => this._reservedTeam = value;
        }

        public sbyte reservedSpectatorPersona
        {
            get => this._reservedSpectatorPersona;
            set => this._reservedSpectatorPersona = value;
        }

        public DuckPersona fallbackPersona => Network.isActive ? this.networkDefaultPersona : this.defaultPersona;

        public DuckPersona desiredPersona
        {
            get
            {
                if (this.requestedColor >= 0 && this.requestedColor < DG.MaxPlayers)
                    return Persona.all.ElementAt<DuckPersona>(this.requestedColor);
                return this.preferredColor >= 0 && this.preferredColor < DG.MaxPlayers ? Persona.all.ElementAt<DuckPersona>(this.preferredColor) : this.fallbackPersona;
            }
        }

        public void UpdatePersona() => DuckNetwork.RequestPersona(this, this.desiredPersona);

        public void PersonaRequestResult(DuckPersona pPersona) => this.persona = pPersona;

        public Duck duck
        {
            get => this._duck;
            set => this._duck = value;
        }

        public DuckPersona persona
        {
            get
            {
                if (this.slotType == SlotType.Spectator)
                {
                    sbyte index = this.netData.Get<sbyte>("spectatorPersona", -1);
                    if (index >= 0 && index < 8)
                        return Persona.all.ElementAt<DuckPersona>(index);
                }
                if (this._persona == null)
                    this._persona = this != Profiles.DefaultPlayer1 ? (this != Profiles.DefaultPlayer2 ? (this != Profiles.DefaultPlayer3 ? (this != Profiles.DefaultPlayer4 ? Persona.Duck1 : Persona.Duck4) : Persona.Duck3) : Persona.Duck2) : Persona.Duck1;
                return this._persona;
            }
            set => this._persona = value;
        }

        public void SetInputProfileLink(InputProfile pLink) => this._inputProfile = pLink;

        public static List<Profile> defaultProfileMappings => Profiles.core.defaultProfileMappings;

        public InputProfile inputProfile
        {
            get
            {
                if (!Network.isActive && this._inputProfile == null)
                    this._inputProfile = this != Profiles.DefaultPlayer1 ? (this != Profiles.DefaultPlayer2 ? (this != Profiles.DefaultPlayer3 ? (this != Profiles.DefaultPlayer4 ? (this != Profiles.DefaultPlayer5 ? (this != Profiles.DefaultPlayer6 ? (this != Profiles.DefaultPlayer7 ? (this != Profiles.DefaultPlayer8 ? InputProfile.Get(InputProfile.MPPlayer1) : InputProfile.Get(InputProfile.MPPlayer8)) : InputProfile.Get(InputProfile.MPPlayer7)) : InputProfile.Get(InputProfile.MPPlayer6)) : InputProfile.Get(InputProfile.MPPlayer5)) : InputProfile.Get(InputProfile.MPPlayer4)) : InputProfile.Get(InputProfile.MPPlayer3)) : InputProfile.Get(InputProfile.MPPlayer2)) : InputProfile.Get(InputProfile.MPPlayer1);
                return this._inputProfile;
            }
            set
            {
                if (this._inputProfile != null && this._inputProfile != value)
                {
                    this._inputProfile.lastActiveDevice.Rumble();
                    Input.ApplyDefaultMapping(this._inputProfile);
                }
                if (value != null && value != this._inputProfile)
                    Input.ApplyDefaultMapping(value, this);
                this._inputProfile = value;
            }
        }

        public Team team
        {
            get => this._team;
            set
            {
                if (value != null)
                {
                    if (this.slotType != SlotType.Spectator)
                        value.Join(this, false);
                    this._team = value;
                }
                else
                {
                    if (this._team == null)
                        return;
                    this._team.Leave(this, false);
                    this._team = null;
                    this.requestedColor = -1;
                }
            }
        }

        public int wins
        {
            get => this._wins;
            set => this._wins = value;
        }

        public bool wasRockThrower
        {
            get => this._wasRockThrower;
            set => this._wasRockThrower = value;
        }

        public List<DeviceInputMapping> inputMappingOverrides => this._linkedProfile != null ? this._linkedProfile.inputMappingOverrides : this._inputMappingOverrides;

        public void ClearCurrentGame() => this._currentGame = new CurrentGame();

        public void ApplyDefaults()
        {
            this.team = this.defaultTeam != null ? this.defaultTeam : Teams.Player1;
            this.UpdatePersona();
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
            this._name = varName;
            this._inputProfile = varProfile;
            if (this._inputProfile != null)
                this._inputProfile.oldAngles = false;
            if (varStartTeam != null)
            {
                varStartTeam.Join(this);
                this.defaultTeam = varStartTeam;
            }
            this._persona = varDefaultPersona;
            this.defaultPersona = varDefaultPersona;
            this._id = varID != null ? varID : Guid.NewGuid().ToString();
            this.isNetworkProfile = network;
            this.isDefaultProfile = pDefaultProfile;
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
