using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace DuckGame
{
    [DebuggerDisplay("{_name}")]
    public class Team
    {
        //YUH YEAH AYUYH YUH YYUH ELETI GUEWHYUH YUH YUH
        public float shake;
        public bool favorited;

        public NetworkConnection customConnection;
        private Dictionary<DuckPersona, SpriteMap> _recolors = new Dictionary<DuckPersona, SpriteMap>();
        public string customHatPath;
        private static readonly long kPngHatKey = 630430737023345;
        public static bool networkDeserialize = false;
        public static List<string> hatSearchPaths = new List<string>()
        {
          Directory.GetCurrentDirectory() + "/Hats",
          DuckFile.saveDirectory + "/Hats",
          DuckFile.saveDirectory + "/Custom/Hats"
        };
        public static List<Team> deserializedTeams = new List<Team>();
        public static int currentLoadHat = 0;
        public static int totalLoadHats = 0;
        public static Team deserializeInto;
        public Texture2D _capeTexture;
        public List<Texture2D> _customParticles = new List<Texture2D>();
        public Texture2D _rockTexture;
        public bool capeRequestSuccess = true;
        private string _name = "";
        private string _description = "";
        private SpriteMap _default;
        private SpriteMap _hat;
        private int _score;
        private int _rockScore;
        private int _wins;
        private int _prevScoreboardScore;
        private Vec2 _hatOffset;
        public bool inDemo;
        private List<Profile> _activeProfiles = new List<Profile>();
        public CustomHatMetadata _basicMetadata;
        public CustomHatMetadata _metadata;
        private string _hatID;
        private byte[] _customData;
        public Vec2 prevTreeDraw = Vec2.Zero;
        private bool _locked;
        public bool isFolder;
        public Team folder;
        public string fullFolderPath;
        public Tex2D folderTexture;
        public bool defaultTeam;
        public Profile owner;
        public bool isTemporaryTeam;
        public bool isHair;
        public bool noCrouchOffset;

        public SpriteMap GetHat(DuckPersona pPersona)
        {
            if (metadata == null || !metadata.UseDuckColor.value)
                return this.hat;
            SpriteMap hat;
            if (!_recolors.TryGetValue(pPersona, out hat))
            {
                hat = new SpriteMap(Graphics.RecolorNew(this.hat.texture, pPersona.color.ToColor(), pPersona.colorDark.ToColor()), 32, 32);
                _recolors[pPersona] = hat;
            }
            return hat;
        }

        public bool filter
        {
            get
            {
                if (customConnection == null)
                    return false;
                Profile profile = customConnection.profile;
                if (profile != null && profile.muteHat)
                    return true;
                bool filter = Options.Data.hatFilter == 2;
                if (Options.Data.hatFilter == 1 && customConnection.data is User && (customConnection.data as User).relationship != FriendRelationship.Friend)
                    filter = true;
                return filter;
            }
        }

        public Team Clone() => new Team(name, (Texture2D)_hat.texture)
        {
            _metadata = _metadata,
            _rockTexture = _rockTexture,
            _capeTexture = _capeTexture,
            _customData = _customData,
            _customParticles = _customParticles,
            customConnection = customConnection
        };

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] buffer1 = new byte[4];
            if (s.Read(buffer1, 0, buffer1.Length) != buffer1.Length)
                throw new SystemException("Stream did not contain properly formatted byte array");
            byte[] buffer2 = new byte[BitConverter.ToInt32(buffer1, 0)];
            if (s.Read(buffer2, 0, buffer2.Length) != buffer2.Length)
                throw new SystemException("Did not read byte array properly");
            return buffer2;
        }

        public static Team Deserialize(string file)
        {
            if (!DuckFile.FileExists(file))
                return null;
            return file.EndsWith(".png") ? DeserializeFromPNG(file) : Deserialize(File.ReadAllBytes(file), file);
        }

        public static Team DeserializeFromPNG(string pFile)
        {
            try
            {
                return pFile.EndsWith("folder_preview.png") ? null : DeserializeFromPNG(File.ReadAllBytes(pFile), Path.GetFileNameWithoutExtension(pFile), pFile);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Team DeserializeFromPNG(
          byte[] pData,
          string pName,
          string pPath,
          bool pIgnoreSizeRestriction = false)
        {
            try
            {
                Texture2D texture2D1 = TextureConverter.LoadPNGWithPinkAwesomeness(Graphics.device, new Bitmap(new MemoryStream(pData)), true);
                double num = texture2D1.Width / 32f % 1f;
                Team pTeam = deserializeInto;
                if (pTeam == null)
                    pTeam = new Team(pName, texture2D1);
                else
                    pTeam.Construct(pName, texture2D1);
                deserializeInto = null;
                pTeam.hatID = CRC32.Generate(pData).ToString();
                BitBuffer bitBuffer = new BitBuffer();
                bitBuffer.Write(kPngHatKey);
                bitBuffer.Write(pName);
                bitBuffer.Write(new BitBuffer(pData), true);
                pTeam.customData = bitBuffer.buffer;
                if (texture2D1.Width >= 96)
                {
                    Color[] data = new Color[1024];
                    texture2D1.GetData(0, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 0, 32, 32)), data, 0, 1024);
                    pTeam._capeTexture = new Texture2D(Graphics.device, 32, 32);
                    pTeam._capeTexture.SetData(data);
                    pTeam.capeRequestSuccess = true;
                }
                if (texture2D1.Height >= 56)
                {
                    Color[] colorArray1 = new Color[576];
                    texture2D1.GetData(0, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 32, 24, 24)), colorArray1, 0, 576);
                    if (CheckForPixelData(colorArray1, 16))
                    {
                        pTeam._rockTexture = new Texture2D(Graphics.device, 24, 24);
                        pTeam._rockTexture.SetData(colorArray1);
                    }
                    if (texture2D1.Width > 32)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            Color[] colorArray2 = new Color[144];
                            texture2D1.GetData(0, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(24 + 12 * (index % 2), 32 + 12 * (index / 2), 12, 12)), colorArray2, 0, 144);
                            if (CheckForPixelData(colorArray2))
                            {
                                Texture2D texture2D2 = new Texture2D(Graphics.device, 12, 12);
                                texture2D2.SetData(colorArray2);
                                pTeam.customParticles.Add(texture2D2);
                            }
                        }
                    }
                }
                ProcessMetadata(texture2D1, pTeam);
                pTeam.customHatPath = pPath;
                return pTeam;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void LoadCustomHatsFromFolder(string pFolder, string pExtension)
        {
            foreach (string file in DuckFile.GetFiles(pFolder, "*." + pExtension))
            {
                string f = file;
                ++totalLoadHats;
                MonoMain.currentActionQueue.Enqueue(new LoadingAction(() =>
               {
                   currentLoadHat++;
                   MonoMain.NloadMessage = "Loading Custom Hats (" + currentLoadHat.ToString() + "/" + totalLoadHats.ToString() + ")";
                   Team team = !(pExtension == "png") ? Deserialize(f) : DeserializeFromPNG(File.ReadAllBytes(f), Path.GetFileNameWithoutExtension(f), f);
                   if (team == null)
                       return;
                   deserializedTeams.Add(team);
               },null, "Loading Custom Hats"));
            }
        }

        public static void DeserializeCustomHats()
        {
            LoadCustomHatsFromFolder(Directory.GetCurrentDirectory(), "hat");
            foreach (string hatSearchPath in hatSearchPaths)
            {
                LoadCustomHatsFromFolder(hatSearchPath, "hat");
                LoadCustomHatsFromFolder(hatSearchPath, "png");
            }
        }

        public static bool CheckForPixelData(Color[] pColors, int pMinimumNumberOfPixels = 1)
        {
            int num = 0;
            for (int index = 0; index < pColors.Length; ++index)
            {
                if ((pColors[index].r != byte.MaxValue || pColors[index].g != 0 || pColors[index].b != byte.MaxValue) && !(pColors[index] == Colors.Transparent))
                    ++num;
            }
            return num >= pMinimumNumberOfPixels;
        }

        public static void ProcessMetadata(Texture2D pTex, Team pTeam)
        {
            int width = pTex.Width % 32;
            if (pTex.Width > 100 || width <= 0)
                return;
            pTeam.metadata = new CustomHatMetadata(pTeam);
            Color[] data = new Color[width * Math.Min(pTex.Height, 56)];
            pTex.GetData(0, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(pTex.Width - width, 0, width, Math.Min(pTex.Height, 56))), data, 0, data.Length);
            for (int index = 0; index < data.Length; ++index)
            {
                Color pColor = data[index];
                if ((pColor.r != byte.MaxValue || pColor.g != 0 || pColor.b != byte.MaxValue) && !(pColor == Colors.Transparent))
                    pTeam.metadata.Deserialize(pColor);
            }
            pTeam.hatOffset = pTeam.metadata.HatOffset.value;
        }

        public static Team Deserialize(byte[] teamData) => Deserialize(teamData, null);

        public static Team Deserialize(byte[] teamData, string pPath)
        {
            try
            {
                if (teamData == null)
                    return null;
                MemoryStream memoryStream = new MemoryStream(teamData);
                if (new BinaryReader(memoryStream).ReadInt64() == kPngHatKey)
                {
                    BitBuffer bitBuffer = new BitBuffer(teamData);
                    bitBuffer.ReadLong();
                    string pName = bitBuffer.ReadString();
                    return DeserializeFromPNG(bitBuffer.ReadBitBuffer().buffer, pName, pPath);
                }
                memoryStream.Seek(0L, SeekOrigin.Begin);
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                byte[] numArray = new byte[16]
                {
                   243,
                   22,
                   152,
                   32,
                   1,
                   244,
                   122,
                   111,
                   97,
                   42,
                   13,
                   2,
                   19,
                   15,
                   45,
                   230
                };
                rijndaelManaged.Key = numArray;
                rijndaelManaged.IV = ReadByteArray(memoryStream);
                BinaryReader binaryReader = new BinaryReader(new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV), CryptoStreamMode.Read));
                long num = binaryReader.ReadInt64();
                switch (num)
                {
                    case 402965919293045:
                    case 465665919293045:
                    case 630430777029345:
                    case 630449177029345:
                        if (num == 630449177029345L || num == 465665919293045L)
                            binaryReader.ReadString();
                        string pName1 = binaryReader.ReadString();
                        int count = binaryReader.ReadInt32();
                        return DeserializeFromPNG(binaryReader.ReadBytes(count), pName1, pPath, true);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private Team GetFacadeTeam()
        {
            if (Network.isActive || HostTable.loop)
            {
                foreach (Profile profile in DuckNetwork.profiles)
                {
                    if (profile.team == this && profile.fixedGhostIndex < 8)
                    {
                        Team facadeTeam;
                        if (profile.connection != null && Teams.core._facadeMap.TryGetValue(profile, out facadeTeam))
                            return facadeTeam;
                    }
                }
            }
            return this;
        }

        public Texture2D capeTexture => filter ? null : GetFacadeTeam()._capeTexture;

        public List<Texture2D> customParticles => filter ? new List<Texture2D>() : GetFacadeTeam()._customParticles;

        public Texture2D rockTexture => filter ? null : GetFacadeTeam()._rockTexture;

        public string name => _name;

        public string description => _description;

        public string GetNameForDisplay() => name.ToUpperInvariant();

        public string currentDisplayName
        {
            get
            {
                string currentDisplayName = "";
                if (activeProfiles != null && activeProfiles.Count > 0)
                    currentDisplayName = activeProfiles.Count <= 1 ? (!Profiles.IsDefault(activeProfiles[0]) || Network.isActive ? activeProfiles[0].nameUI : GetNameForDisplay()) : GetNameForDisplay();
                return currentDisplayName;
            }
        }

        public SpriteMap hat
        {
            get
            {
                if (!filter)
                    return GetFacadeTeam()._hat;
                if (_default == null)
                    _default = new SpriteMap("hats/default", 32, 32);
                return _default;
            }
        }

        public void SetHatSprite(SpriteMap pSprite) => _hat = pSprite;

        public bool hasHat => hat != null && hat.texture.textureName != "hats/noHat";

        public int score
        {
            get => _score;
            set => _score = value;
        }

        public int rockScore
        {
            get => _rockScore;
            set => _rockScore = value;
        }

        public int wins
        {
            get => _wins;
            set => _wins = value;
        }

        public int prevScoreboardScore
        {
            get => _prevScoreboardScore;
            set => _prevScoreboardScore = value;
        }

        public Vec2 hatOffset
        {
            get => filter ? Vec2.Zero : GetFacadeTeam()._hatOffset;
            set => _hatOffset = value;
        }

        public List<Profile> activeProfiles => _activeProfiles;

        public int numMembers => _activeProfiles.Count;

        public void Join(Profile prof, bool set = true)
        {
            if (_activeProfiles.Contains(prof))
                return;
            if (prof.team != null)
                prof.team.Leave(prof, set);
            _activeProfiles.Add(prof);
            if (!set)
                return;
            prof.team = this;
        }

        public void Leave(Profile prof, bool set = true)
        {
            _activeProfiles.Remove(prof);
            if (!set)
                return;
            prof.team = null;
        }

        public void ClearProfiles()
        {
            foreach (Profile prof in new List<Profile>(_activeProfiles))
                Leave(prof);
            _activeProfiles.Clear();
        }

        public void ResetTeam() => _score = 0;

        public CustomHatMetadata metadata
        {
            get
            {
                if (!filter)
                    return GetFacadeTeam()._metadata;
                if (_basicMetadata == null)
                {
                    _basicMetadata = new CustomHatMetadata(this);
                    _basicMetadata.UseDuckColor.value = true;
                }
                return _basicMetadata;
            }
            set => _metadata = value;
        }

        public string hatID
        {
            get => GetFacadeTeam()._hatID;
            set => _hatID = value;
        }

        public Team facade
        {
            get
            {
                Team facadeTeam = GetFacadeTeam();
                return facadeTeam == this ? null : facadeTeam;
            }
        }

        public byte[] customData
        {
            get => GetFacadeTeam()._customData;
            set => _customData = value;
        }

        public bool locked
        {
            get
            {
                if (FireDebug.Debugging && !defaultTeam)
                    return false;
                if (!NetworkDebugger.enabled || NetworkDebugger.currentIndex != 1)
                    return _locked;
                return Teams.all.IndexOf(this) > 15 && Teams.all.IndexOf(this) < 35;
            }
            set => _locked = value;
        }

        public Team(
          string varName,
          string hatTexture,
          bool demo = false,
          bool lockd = false,
          Vec2 hatOff = default(Vec2),
          string desc = "",
          Texture2D capeTex = null)
        {
            _name = varName;
            _hat = new SpriteMap(hatTexture, 32, 32);
            _hatOffset = hatOff;
            inDemo = demo;
            _locked = lockd;
            _description = desc;
            _capeTexture = capeTex;
        }

        public Team(
          bool varHair,
          string varName,
          string hatTexture,
          bool demo = false,
          bool lockd = false,
          Vec2 hatOff = default(Vec2),
          string desc = "",
          Texture2D capeTex = null)
          : this(varName, hatTexture, demo, lockd, hatOff, desc, capeTex)
        {
            _name = varName;
            _hat = new SpriteMap(hatTexture, 32, 32);
            _hatOffset = hatOff;
            inDemo = demo;
            _locked = lockd;
            _description = desc;
            _capeTexture = capeTex;
            isHair = varHair;
        }

        public Team(string varName, string hatTexture, bool demo, bool lockd, Vec2 hatOff)
        {
            _name = varName;
            _hat = new SpriteMap(hatTexture, 32, 32);
            _hatOffset = hatOff;
            inDemo = demo;
            _locked = lockd;
        }

        public Team(
          string varName,
          Texture2D hatTexture,
          bool demo = false,
          bool lockd = false,
          Vec2 hatOff = default(Vec2),
          string desc = "")
        {
            Construct(varName, hatTexture, demo, lockd, hatOff, desc);
        }

        public Team(string varName, Texture2D hatTexture, bool demo, bool lockd, Vec2 hatOff)
        {
            _name = varName;
            _hat = new SpriteMap((Tex2D)hatTexture, 32, 32);
            _hatOffset = hatOff;
            inDemo = demo;
            _locked = lockd;
        }

        public void Construct(
          string varName,
          Texture2D hatTexture,
          bool demo = false,
          bool lockd = false,
          Vec2 hatOff = default(Vec2),
          string desc = "")
        {
            _name = varName;
            _hat = new SpriteMap((Tex2D)hatTexture, 32, 32);
            _hatOffset = hatOff;
            inDemo = demo;
            _locked = lockd;
            _description = desc;
        }

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public sealed class Metapixel : Attribute
        {
            public readonly int index;
            public readonly string name;
            public readonly string description;

            public Metapixel(int pIndex, string pName, string pDescription)
            {
                index = pIndex;
                name = pName;
                description = pDescription;
            }
        }

        public class CustomMetadata
        {
            public static HatMetadataElement kPreviousParameter;
            public static HatMetadataElement kCurrentParameter;
            public Dictionary<int, HatMetadataElement> _fieldMap = new Dictionary<int, HatMetadataElement>();

            public int Index(HatMetadataElement pParameter)
            {
                foreach (KeyValuePair<int, HatMetadataElement> field in _fieldMap)
                {
                    if (field.Value == pParameter)
                        return field.Key;
                }
                return -1;
            }

            public virtual bool Deserialize(Color pColor)
            {
                HatMetadataElement hatMetadataElement;
                if (!_fieldMap.TryGetValue(pColor.r, out hatMetadataElement))
                    return false;
                if (!(hatMetadataElement is CustomHatMetadata.MDRandomizer))
                    kCurrentParameter = hatMetadataElement;
                hatMetadataElement.Parse(pColor);
                if (!(hatMetadataElement is CustomHatMetadata.MDRandomizer))
                    kPreviousParameter = hatMetadataElement;
                return true;
            }

            public static Dictionary<Func<object, object>, Metapixel> PrepareParameterAttributes(
              Type pType)
            {
                Dictionary<Func<object, object>, Metapixel> dictionary = new Dictionary<Func<object, object>, Metapixel>();
                foreach (FieldInfo fieldInfo in pType.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(x => typeof(HatMetadataElement).IsAssignableFrom(x.FieldType)))
                    dictionary[Editor.BuildGetAccessorField(pType, fieldInfo)] = fieldInfo.GetCustomAttribute<Metapixel>();
                return dictionary;
            }

            public static Dictionary<int, HatMetadataElement> PrepareFieldMap(
              Dictionary<Func<object, object>, Metapixel> pParameterAttributes,
              object pFor)
            {
                Dictionary<int, HatMetadataElement> dictionary = new Dictionary<int, HatMetadataElement>();
                foreach (KeyValuePair<Func<object, object>, Metapixel> parameterAttribute in pParameterAttributes)
                    dictionary[parameterAttribute.Value.index] = (HatMetadataElement)parameterAttribute.Key(pFor);
                return dictionary;
            }
        }

        public abstract class HatMetadataElement
        {
            public bool set;
            public Vec2 randomizerX;
            public Vec2 randomizerY;

            public abstract void Parse(Color pColor);
        }

        public class CustomHatMetadata : CustomMetadata
        {
            public string hatPath;
            [Metapixel(1, "Hat Offset", "Hat offset position in pixels")]
            public MDVec2 HatOffset = new MDVec2
            {
                range = 16f
            };

            [Metapixel(2, "Use Duck Color", "If this metapixel exists, White (255, 255, 255) and Grey(157, 157, 157) will be recolored to duck colors.")]
            public MDBool UseDuckColor = new MDBool();

            [Metapixel(3, "Hat No-Flip", "If this metapixel exists, the hat will not be flipped with the direction of the duck.")]
            public MDBool HatNoFlip = new MDBool();

            [Metapixel(10, "Cape Offset", "Cape offset position in pixels")]
            public MDVec2 CapeOffset = new MDVec2
            {
                range = 16f
            };

            [Metapixel(11, "Cape Is Foreground", "If this metapixel exists, the cape will be drawn over the duck.")]
            public MDBool CapeForeground = new MDBool();

            [Metapixel(12, "Cape Sway Modifier", "Affects cape length, and left to right sway.")]
            public MDVec2Normalized CapeSwayModifier = new MDVec2Normalized
            {
                value = new Vec2(0.3f, 1f),
                allowNegative = true
            };

            [Metapixel(13, "Cape Wiggle Modifier", "Affects how much the cape wiggles in the wind.")]
            public MDVec2Normalized CapeWiggleModifier = new MDVec2Normalized
            {
                value = new Vec2(1f, 1f),
                allowNegative = true
            };

            [Metapixel(14, "Cape Taper Start", "Affects how narrow the cape/trail is at the top/beginning.")]
            public MDFloat CapeTaperStart = new MDFloat
            {
                value = 0.5f
            };

            [Metapixel(15, "Cape Taper End", "Affects how narrow the cape/trail is at the bottom/end.")]
            public MDFloat CapeTaperEnd = new MDFloat
            {
                value = 1f
            };

            [Metapixel(16, "Cape Alpha Start", "Affects how transparent the cape/trail is at the top/beginning.")]
            public MDFloat CapeAlphaStart = new MDFloat
            {
                value = 1f
            };

            [Metapixel(17, "Cape Alpha End", "Affects how transparent the cape/trail is at the bottom/end.")]
            public MDFloat CapeAlphaEnd = new MDFloat
            {
                value = 1f
            };

            [Metapixel(20, "Cape Is Trail", "If this metapixel exists, the cape will be a trail instead of a cape (think of the rainbow trail left by the TV object).")]
            public MDBool CapeIsTrail = new MDBool();

            [Metapixel(30, "Particle Emitter Offset", "The offset in pixels from the center of the hat where particles will be emitted.")]
            public MDVec2 ParticleEmitterOffset = new MDVec2
            {
                range = 16f
            };

            [Metapixel(31, "Particle Default Behavior", "B defines a particle behavior from a list of presets: 0 = No Behavior, 1 = Spit, 2 = Burst, 3 = Halo, 4 = Exclamation")]
            public MDInt ParticleDefaultBehavior = new MDInt
            {
                range = 4,
                postParseScript = new Action(ApplyDefaultParticleBehavior)
            };

            [Metapixel(32, "Particle Emit Shape", "G: 0 = Point, 1 = Circle, 2 = Box   B: 0 = Emit Around Shape Border Randomly, 1 = Fill Shape Randomly, 2 = Emit Around Shape Border Uniformly")]
            public MDIntPair ParticleEmitShape = new MDIntPair
            {
                rangeX = 2,
                rangeY = 2
            };

            [Metapixel(33, "Particle Emit Shape Size", "X and Y size of the particle emitter (in pixels)")]
            public MDVec2 ParticleEmitShapeSize = new MDVec2
            {
                range = 32f,
                value = new Vec2(24f, 24f)
            };

            [Metapixel(34, "Particle Count", "The number of particles to emit.")]
            public MDInt ParticleCount = new MDInt
            {
                range = 8,
                value = 4
            };

            [Metapixel(35, "Particle Lifespan", "Life span of the particle, in seconds.")]
            public MDFloat ParticleLifespan = new MDFloat
            {
                range = 2f,
                value = 1f
            };

            [Metapixel(36, "Particle Velocity", "Initial velocity of the particle.")]
            public MDVec2Normalized ParticleVelocity = new MDVec2Normalized
            {
                range = 2f,
                allowNegative = true
            };

            [Metapixel(37, "Particle Gravity", "Gravity applied to the particle.")]
            public MDVec2Normalized ParticleGravity = new MDVec2Normalized
            {
                range = 2f,
                allowNegative = true,
                value = new Vec2(0f, PhysicsObject.gravity)
            };

            [Metapixel(38, "Particle Friction", "Friction applied to the particle (The value it's velocity is multiplied by every frame).")]
            public MDVec2Normalized ParticleFriction = new MDVec2Normalized
            {
                range = 1f,
                allowNegative = false,
                value = new Vec2(1f, 1f)
            };

            [Metapixel(39, "Particle Alpha", "G = Start alpha, B = End alpha")]
            public MDVec2Normalized ParticleAlpha = new MDVec2Normalized
            {
                range = 1f,
                allowNegative = false,
                value = new Vec2(1f, 1f)
            };

            [Metapixel(40, "Particle Scale", "G = Start scale, B = End scale")]
            public MDVec2Normalized ParticleScale = new MDVec2Normalized
            {
                range = 2f,
                allowNegative = false,
                value = new Vec2(1f, 0f)
            };

            [Metapixel(41, "Particle Rotation", "G = Start rotation, B = End rotation")]
            public MDVec2Normalized ParticleRotation = new MDVec2Normalized
            {
                range = 36f,
                value = new Vec2(0f, 0f)
            };

            [Metapixel(42, "Particle Offset", "Additional X Y offset of particle.")]
            public MDVec2 ParticleOffset = new MDVec2
            {
                range = 16f
            };

            [Metapixel(43, "Particle Background", "If this metapixel exists, particles will be rendered behind the duck.")]
            public MDBool ParticleBackground = new MDBool();

            [Metapixel(44, "Particle Anchor", "If this metapixel exists, particles will stay anchored around the hat position when it's moving.")]
            public MDBool ParticleAnchor = new MDBool();

            [Metapixel(45, "Particle Animated", "If this metapixel exists, particles will animate through their frames. Otherwise, a frame will be picked randomly.")]
            public MDBool ParticleAnimated = new MDBool();

            [Metapixel(46, "Particle Animation Loop", "If this metapixel exists, the particle animation will loop.")]
            public MDBool ParticleAnimationLoop = new MDBool();

            [Metapixel(47, "Particle Animation Random Frame", "If this metapixel exists, the particle animation will start on a random frame.")]
            public MDBool ParticleAnimationRandomFrame = new MDBool();

            [Metapixel(48, "Particle Animation Speed", "How quickly the particle animates.")]
            public MDFloat ParticleAnimationSpeed = new MDFloat
            {
                range = 1f,
                value = 0.1f
            };

            [Metapixel(49, "Particle Anchor Orientation", "If this metapixel exists, particles will flip and rotate to orient with the hat.")]
            public MDBool ParticleAnchorOrientation = new MDBool();

            [Metapixel(60, "Quack Delay", "Amount of time in between pressing the quack button and the quack frame appearing.")]
            public MDFloat QuackDelay = new MDFloat
            {
                range = 2f,
                value = 0f
            };

            [Metapixel(61, "Quack Hold", "Minimum amount of time to keep the quack frame held, even if the quack button is released.")]
            public MDFloat QuackHold = new MDFloat
            {
                range = 2f,
                value = 0f
            };

            [Metapixel(62, "Quack Suppress Requack", "If this metapixel exists, a new quack will not be allowed to begin until Quack Delay and Quack Hold are finished.")]
            public MDBool QuackSuppressRequack = new MDBool();

            [Metapixel(70, "Wet Lips", "If this metapixel exists, the hat will have 'wet lips'.")]
            public MDBool WetLips = new MDBool();

            [Metapixel(71, "Mechanical Lips", "If this metapixel exists, the hat will have 'mechanical lips'.")]
            public MDBool MechanicalLips = new MDBool();

            [Metapixel(100, "Randomize Parameter X", "If present, the previously defined metapixel value will have it's X value multiplied by a random normalized number between G and B each time it's used. This will generally only work with particles..")]
            public MDRandomizer RandomizeParameterX = new MDRandomizer
            {
                range = 1f,
                allowNegative = true
            };

            [Metapixel(101, "Randomize Parameter Y", "If present, the previously defined metapixel value will have it's Y value multiplied by a random normalized number between G and B each time it's used. This will generally only work with particles..")]
            public MDRandomizer RandomizeParameterY = new MDRandomizer
            {
                range = 1f,
                allowNegative = true,
                randomizeY = true
            };

            [Metapixel(102, "Randomize Parameter", "If present, the previously defined metapixel value will have a random number between G and B applied to its X and Y values each time it's used. This will generally only work with particles..")]
            public MDRandomizer RandomizeParameter = new MDRandomizer
            {
                range = 1f,
                allowNegative = true,
                randomizeBoth = true
            };
            public Team team;
            private static CustomHatMetadata kCurrentMetadata;
            private static Dictionary<Func<object, object>, Metapixel> kParameterAttributes;

            private static void ApplyDefaultParticleBehavior()
            {
                int num = kCurrentMetadata.ParticleDefaultBehavior.value;
                if (num == 1)
                {
                    kCurrentMetadata.ParticleEmitShape.value = new Vec2(0f, 0f);
                    kCurrentMetadata.ParticleOffset.value = new Vec2(2f, 2f);
                    kCurrentMetadata.ParticleOffset.randomizerX = new Vec2(-1f, 1f);
                    kCurrentMetadata.ParticleOffset.randomizerY = new Vec2(-1f, 1f);
                    kCurrentMetadata.ParticleVelocity.value = new Vec2(3f, 1.5f);
                    kCurrentMetadata.ParticleVelocity.randomizerX = new Vec2(0.3f, 1f);
                    kCurrentMetadata.ParticleVelocity.randomizerY = new Vec2(-1f, 0.3f);
                    kCurrentMetadata.ParticleScale.value = new Vec2(1f, 1f);
                    kCurrentMetadata.ParticleScale.randomizerX = new Vec2(0.7f, 1f);
                    kCurrentMetadata.ParticleScale.randomizerY = Vec2.MaxValue;
                    kCurrentMetadata.ParticleCount.value = 5;
                    kCurrentMetadata.ParticleCount.randomizerX = new Vec2(0.3f, 1f);
                    kCurrentMetadata.ParticleBackground.value = false;
                }
                if (num == 2)
                {
                    kCurrentMetadata.ParticleEmitShape.value = new Vec2(0f, 0f);
                    kCurrentMetadata.ParticleOffset.value = new Vec2(2f, 2f);
                    kCurrentMetadata.ParticleOffset.randomizerX = new Vec2(-1f, 1f);
                    kCurrentMetadata.ParticleOffset.randomizerY = new Vec2(-1f, 1f);
                    kCurrentMetadata.ParticleVelocity.value = new Vec2(1.5f, 2.5f);
                    kCurrentMetadata.ParticleVelocity.randomizerX = new Vec2(-1f, 1f);
                    kCurrentMetadata.ParticleVelocity.randomizerY = new Vec2(-1f, 1f);
                    kCurrentMetadata.ParticleScale.value = new Vec2(1f, 0f);
                    kCurrentMetadata.ParticleScale.randomizerX = new Vec2(0.7f, 1f);
                    kCurrentMetadata.ParticleCount.value = 8;
                    kCurrentMetadata.ParticleCount.randomizerX = new Vec2(0.5f, 1f);
                    kCurrentMetadata.ParticleBackground.value = false;
                }
                if (num == 3)
                {
                    kCurrentMetadata.ParticleEmitShape.value = new Vec2(1f, 2f);
                    kCurrentMetadata.ParticleAlpha.value = new Vec2(1f, 0f);
                    kCurrentMetadata.ParticleCount.value = 8;
                    kCurrentMetadata.ParticleBackground.value = true;
                    kCurrentMetadata.ParticleGravity.value = new Vec2(0f, 0f);
                    kCurrentMetadata.ParticleAnchor.value = true;
                }
                if (num != 4)
                    return;
                kCurrentMetadata.ParticleEmitShape.value = new Vec2(0f, 0f);
                kCurrentMetadata.ParticleScale.value = new Vec2(0.3f, 1.5f);
                kCurrentMetadata.ParticleCount.value = 1;
                kCurrentMetadata.ParticleBackground.value = false;
                kCurrentMetadata.ParticleGravity.value = new Vec2(0f, 0f);
                kCurrentMetadata.ParticleAnchor.value = true;
                kCurrentMetadata.ParticleVelocity.value = new Vec2(1.4f, -1.2f);
                kCurrentMetadata.ParticleFriction.value = new Vec2(0.92f, 0.9f);
                kCurrentMetadata.ParticleLifespan.value = 0.8f;
            }

            public CustomHatMetadata(Team pTeam)
            {
                MDVec2Normalized mdVec2Normalized1 = new MDVec2Normalized
                {
                    value = new Vec2(0.3f, 1f),
                    allowNegative = true
                };
                CapeSwayModifier = mdVec2Normalized1;
                MDVec2Normalized mdVec2Normalized2 = new MDVec2Normalized
                {
                    value = new Vec2(1f, 1f),
                    allowNegative = true
                };
                CapeWiggleModifier = mdVec2Normalized2;
                MDFloat mdFloat1 = new MDFloat
                {
                    value = 0.5f
                };
                CapeTaperStart = mdFloat1;
                MDFloat mdFloat2 = new MDFloat
                {
                    value = 1f
                };
                CapeTaperEnd = mdFloat2;
                MDFloat mdFloat3 = new MDFloat
                {
                    value = 1f
                };
                CapeAlphaStart = mdFloat3;
                MDFloat mdFloat4 = new MDFloat
                {
                    value = 1f
                };
                CapeAlphaEnd = mdFloat4;
                CapeIsTrail = new MDBool();
                ParticleEmitterOffset = new MDVec2()
                {
                    range = 16f
                };
                MDInt mdInt1 = new MDInt
                {
                    range = 4,
                    postParseScript = new Action(ApplyDefaultParticleBehavior)
                };
                ParticleDefaultBehavior = mdInt1;
                ParticleEmitShape = new MDIntPair()
                {
                    rangeX = 2,
                    rangeY = 2
                };
                MDVec2 mdVec2 = new MDVec2
                {
                    range = 32f,
                    value = new Vec2(24f, 24f)
                };
                ParticleEmitShapeSize = mdVec2;
                MDInt mdInt2 = new MDInt
                {
                    range = 8,
                    value = 4
                };
                ParticleCount = mdInt2;
                MDFloat mdFloat5 = new MDFloat
                {
                    range = 2f,
                    value = 1f
                };
                ParticleLifespan = mdFloat5;
                MDVec2Normalized mdVec2Normalized3 = new MDVec2Normalized
                {
                    range = 2f,
                    allowNegative = true
                };
                ParticleVelocity = mdVec2Normalized3;
                MDVec2Normalized mdVec2Normalized4 = new MDVec2Normalized
                {
                    range = 2f,
                    allowNegative = true,
                    value = new Vec2(0f, PhysicsObject.gravity)
                };
                ParticleGravity = mdVec2Normalized4;
                MDVec2Normalized mdVec2Normalized5 = new MDVec2Normalized
                {
                    range = 1f,
                    allowNegative = false,
                    value = new Vec2(1f, 1f)
                };
                ParticleFriction = mdVec2Normalized5;
                MDVec2Normalized mdVec2Normalized6 = new MDVec2Normalized
                {
                    range = 1f,
                    allowNegative = false,
                    value = new Vec2(1f, 1f)
                };
                ParticleAlpha = mdVec2Normalized6;
                MDVec2Normalized mdVec2Normalized7 = new MDVec2Normalized
                {
                    range = 2f,
                    allowNegative = false,
                    value = new Vec2(1f, 0f)
                };
                ParticleScale = mdVec2Normalized7;
                MDVec2Normalized mdVec2Normalized8 = new MDVec2Normalized
                {
                    range = 36f,
                    value = new Vec2(0f, 0f)
                };
                ParticleRotation = mdVec2Normalized8;
                ParticleOffset = new MDVec2()
                {
                    range = 16f
                };
                ParticleBackground = new MDBool();
                ParticleAnchor = new MDBool();
                ParticleAnimated = new MDBool();
                ParticleAnimationLoop = new MDBool();
                ParticleAnimationRandomFrame = new MDBool();
                MDFloat mdFloat6 = new MDFloat
                {
                    range = 1f,
                    value = 0.1f
                };
                ParticleAnimationSpeed = mdFloat6;
                ParticleAnchorOrientation = new MDBool();
                MDFloat mdFloat7 = new MDFloat
                {
                    range = 2f,
                    value = 0f
                };
                QuackDelay = mdFloat7;
                MDFloat mdFloat8 = new MDFloat
                {
                    range = 2f,
                    value = 0f
                };
                QuackHold = mdFloat8;
                QuackSuppressRequack = new MDBool();
                WetLips = new MDBool();
                MechanicalLips = new MDBool();
                MDRandomizer mdRandomizer1 = new MDRandomizer
                {
                    range = 1f,
                    allowNegative = true
                };
                RandomizeParameterX = mdRandomizer1;
                MDRandomizer mdRandomizer2 = new MDRandomizer
                {
                    range = 1f,
                    allowNegative = true,
                    randomizeY = true
                };
                RandomizeParameterY = mdRandomizer2;
                MDRandomizer mdRandomizer3 = new MDRandomizer
                {
                    range = 1f,
                    allowNegative = true,
                    randomizeBoth = true
                };
                RandomizeParameter = mdRandomizer3;
                // ISSUE: explicit constructor call
                // base.\u002Ector(); wtf
                team = pTeam;
                kCurrentMetadata = this;
                if (kParameterAttributes == null)
                    kParameterAttributes = PrepareParameterAttributes(GetType());
                _fieldMap = PrepareFieldMap(kParameterAttributes, this);
                ApplyDefaultParticleBehavior();
            }

            public override bool Deserialize(Color pColor)
            {
                if (base.Deserialize(pColor))
                    return true;
                DevConsole.Log(DCSection.General, "Metapixel with invalid ID value (" + pColor.r.ToString() + ") found in custom hat.");
                return false;
            }

            public abstract class V<T> : HatMetadataElement
            {
                public int defaultCopyIndex;
                protected T _value;
                public Action postParseScript;

                protected V<T> _defaultCopy
                {
                    get
                    {
                        HatMetadataElement hatMetadataElement;
                        return defaultCopyIndex != 0 && kCurrentMetadata != null && kCurrentMetadata._fieldMap.TryGetValue(defaultCopyIndex, out hatMetadataElement) ? hatMetadataElement as V<T> : null;
                    }
                }

                public virtual T value
                {
                    get => _value;
                    set => _value = value;
                }

                public override void Parse(Color pColor)
                {
                    randomizerX = Vec2.Zero;
                    randomizerY = Vec2.Zero;
                    set = true;
                    OnParse(pColor);
                    if (postParseScript == null)
                        return;
                    postParseScript();
                }

                public abstract void OnParse(Color pColor);
            }

            public class MDVec2 : V<Vec2>
            {
                public bool allowNegative = true;
                public float range = 16f;

                public override void OnParse(Color pColor) => _value = new Vec2(Maths.Clamp(pColor.g - 128, -range, range), Maths.Clamp(pColor.b - 128, -range, range));

                public override Vec2 value
                {
                    get
                    {
                        Vec2 vec2 = _value;
                        if (_value == Vec2.MaxValue && _defaultCopy != null)
                            vec2 = _defaultCopy.value;
                        if (randomizerX != Vec2.Zero)
                            vec2.x = Rando.Float(_value.x * randomizerX.x, _value.x * randomizerX.y);
                        if (randomizerY == Vec2.MaxValue)
                            vec2.y = vec2.x;
                        else if (randomizerY != Vec2.Zero)
                            vec2.y = Rando.Float(_value.y * randomizerY.x, _value.y * randomizerY.y);
                        if (allowNegative)
                        {
                            vec2.x = Maths.Clamp(vec2.x, -range, range);
                            vec2.y = Maths.Clamp(vec2.y, -range, range);
                        }
                        else
                        {
                            vec2.x = Maths.Clamp(vec2.x, 0f, range);
                            vec2.y = Maths.Clamp(vec2.y, 0f, range);
                        }
                        return vec2;
                    }
                    set => _value = value;
                }
            }

            public class MDVec2Normalized : MDVec2
            {
                public MDVec2Normalized()
                {
                    range = 1f;
                    allowNegative = false;
                }

                public override void OnParse(Color pColor)
                {
                    if (allowNegative)
                    {
                        float range = this.range;
                        this.range = sbyte.MaxValue;
                        base.OnParse(pColor);
                        _value /= this.range;
                        this.range = range;
                    }
                    else
                        _value = new Vec2(pColor.g / (float)byte.MaxValue, pColor.b / (float)byte.MaxValue);
                    _value *= range;
                }
            }

            public class MDRandomizer : MDVec2Normalized
            {
                public bool randomizeY;
                public bool randomizeBoth;

                public override void OnParse(Color pColor)
                {
                    base.OnParse(pColor);
                    if (kPreviousParameter == null)
                        return;
                    if (!randomizeY)
                        kPreviousParameter.randomizerX = value;
                    else
                        kPreviousParameter.randomizerY = value;
                    if (!randomizeBoth)
                        return;
                    kPreviousParameter.randomizerY = Vec2.MaxValue;
                }
            }

            public class MDBool : V<bool>
            {
                public override void OnParse(Color pColor) => _value = true;
            }

            public class MDFloat : V<float>
            {
                public float range = 1f;
                public bool allowNegative;

                public override float value
                {
                    get
                    {
                        float val = _value;
                        if (_value == 3.40282346638529E+38 && _defaultCopy != null)
                            val = _defaultCopy.value;
                        if (randomizerX != Vec2.Zero)
                            val = Rando.Float(_value * randomizerX.x, _value * randomizerX.y);
                        return allowNegative ? Maths.Clamp(val, -range, range) : Maths.Clamp(val, 0f, range);
                    }
                    set => _value = value;
                }

                public override void OnParse(Color pColor)
                {
                    if (allowNegative)
                        _value = (pColor.g - 128) / 128f;
                    else
                        _value = pColor.g / (float)byte.MaxValue;
                    _value *= range;
                }
            }

            public class MDInt : V<int>
            {
                public int range = byte.MaxValue;
                public bool allowNegative;

                public override int value
                {
                    get
                    {
                        float a = _value;
                        if (_value == int.MaxValue && _defaultCopy != null)
                            a = _defaultCopy.value;
                        if (randomizerX != Vec2.Zero)
                            a = Rando.Float(_value * randomizerX.x, _value * randomizerX.y);
                        return (int)Math.Round(a);
                    }
                    set => _value = value;
                }

                public override void OnParse(Color pColor)
                {
                    if (allowNegative)
                        _value = pColor.g - 128;
                    else
                        _value = pColor.g;
                    _value = Maths.Clamp(value, -range, range);
                }
            }

            public class MDIntPair : V<Vec2>
            {
                public int rangeX = byte.MaxValue;
                public int rangeY = byte.MaxValue;
                public bool allowNegative;

                public override Vec2 value
                {
                    get
                    {
                        Vec2 vec2 = _value;
                        if (_value == Vec2.MaxValue && _defaultCopy != null)
                            vec2 = _defaultCopy.value;
                        if (randomizerX != Vec2.Zero)
                            vec2.x = (float)Math.Round(Rando.Float(_value.x * randomizerX.x, _value.x * randomizerX.y));
                        if (randomizerY == Vec2.MaxValue)
                            vec2.y = vec2.x;
                        else if (randomizerY != Vec2.Zero)
                            vec2.y = (float)Math.Round(Rando.Float(_value.y * randomizerY.x, _value.y * randomizerY.y));
                        return vec2;
                    }
                    set => _value = value;
                }

                public override void OnParse(Color pColor)
                {
                    if (allowNegative)
                    {
                        _value.x = pColor.g - 128;
                        _value.y = pColor.b - 128;
                    }
                    else
                    {
                        _value.x = pColor.g;
                        _value.y = pColor.b;
                    }
                    _value.x = Maths.Clamp(value.x, -rangeX, rangeX);
                    _value.y = Maths.Clamp(value.y, -rangeY, rangeY);
                }
            }
        }
    }
}
