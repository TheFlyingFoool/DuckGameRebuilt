// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomLevelData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Reflection;

namespace DuckGame
{
    public class RandomLevelData
    {
        public Dictionary<System.Type, int> weapons = new Dictionary<System.Type, int>();
        public Dictionary<System.Type, int> spawners = new Dictionary<System.Type, int>();
        public int numWeapons;
        public int numSuperWeapons;
        public int numFatalWeapons;
        public int numPermanentWeapons;
        public int numPermanentSuperWeapons;
        public int numPermanentFatalWeapons;
        public bool up;
        public bool down;
        public bool left;
        public bool right;
        public float chance;
        public float boostChance;
        public int max = 2;
        public bool single;
        public bool multi;
        public bool canMirror = true;
        public bool isMirrored;
        public int numArmor;
        public int numEquipment;
        public int numSpawns;
        public int numTeamSpawns;
        public int numLockedDoors;
        public int numKeys;
        public List<BinaryClassChunk> data;
        public List<BinaryClassChunk> alternateData;
        public bool flip;
        public bool symmetry;
        public string file = "";
        private Vec2 posBeforeTranslate = Vec2.Zero;
        private bool mainLoad = true;

        public RandomLevelData Flipped()
        {
            if (this.isMirrored)
                return this;
            return new RandomLevelData()
            {
                data = this.data,
                alternateData = this.alternateData,
                flip = !this.flip,
                left = this.right,
                right = this.left,
                up = this.up,
                down = this.down,
                chance = this.chance,
                max = this.max,
                file = this.file,
                canMirror = this.canMirror,
                isMirrored = this.isMirrored,
                numWeapons = this.numWeapons,
                numSuperWeapons = this.numSuperWeapons,
                numFatalWeapons = this.numFatalWeapons,
                numPermanentWeapons = this.numPermanentWeapons,
                numPermanentSuperWeapons = this.numPermanentSuperWeapons,
                numPermanentFatalWeapons = this.numPermanentFatalWeapons,
                numArmor = this.numArmor,
                numEquipment = this.numEquipment,
                numSpawns = this.numSpawns,
                numTeamSpawns = this.numTeamSpawns,
                numLockedDoors = this.numLockedDoors,
                numKeys = this.numKeys
            };
        }

        public RandomLevelData Symmetric()
        {
            RandomLevelData randomLevelData = new RandomLevelData()
            {
                data = this.data,
                alternateData = this.alternateData,
                flip = this.flip,
                up = this.up,
                down = this.down,
                symmetry = true,
                file = this.file,
                canMirror = true,
                isMirrored = true
            };
            randomLevelData.right = randomLevelData.left;
            randomLevelData.chance = this.chance;
            if (!this.isMirrored)
                randomLevelData.chance *= 0.75f;
            randomLevelData.max = this.max;
            randomLevelData.numWeapons = this.numWeapons;
            randomLevelData.numSuperWeapons = this.numSuperWeapons;
            randomLevelData.numFatalWeapons = this.numFatalWeapons;
            randomLevelData.numPermanentWeapons = this.numPermanentWeapons;
            randomLevelData.numPermanentSuperWeapons = this.numPermanentSuperWeapons;
            randomLevelData.numPermanentFatalWeapons = this.numPermanentFatalWeapons;
            randomLevelData.numArmor = this.numArmor;
            randomLevelData.numEquipment = this.numEquipment;
            randomLevelData.numSpawns = 0;
            randomLevelData.numTeamSpawns = 0;
            randomLevelData.numLockedDoors = this.numLockedDoors;
            randomLevelData.numKeys = this.numKeys;
            return randomLevelData;
        }

        public void ApplyWeaponData(string data)
        {
            this.weapons.Clear();
            this.numWeapons = 0;
            this.numSuperWeapons = 0;
            this.numFatalWeapons = 0;
            string str = data;
            char[] chArray = new char[1] { '|' };
            foreach (string name in str.Split(chArray))
            {
                if (name != "")
                {
                    try
                    {
                        System.Type type = Editor.GetType(name);
                        if (!this.weapons.ContainsKey(type))
                            this.weapons[type] = 0;
                        this.weapons[type]++;
                        ++this.numWeapons;
                        IReadOnlyPropertyBag bag = ContentProperties.GetBag(type);
                        if (bag.GetOrDefault("isSuperWeapon", false))
                            ++this.numSuperWeapons;
                        if (bag.GetOrDefault("isFatal", true))
                            ++this.numFatalWeapons;
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void ApplySpawnerData(string data)
        {
            this.spawners.Clear();
            this.numPermanentWeapons = 0;
            this.numPermanentSuperWeapons = 0;
            this.numPermanentFatalWeapons = 0;
            string str = data;
            char[] chArray = new char[1] { '|' };
            foreach (string name in str.Split(chArray))
            {
                if (name != "")
                {
                    try
                    {
                        System.Type type = Editor.GetType(name);
                        if (!this.spawners.ContainsKey(type))
                            this.spawners[type] = 0;
                        this.spawners[type]++;
                        ++this.numPermanentWeapons;
                        IReadOnlyPropertyBag bag = ContentProperties.GetBag(type);
                        if (bag.GetOrDefault("isSuperWeapon", false))
                            ++this.numPermanentSuperWeapons;
                        if (bag.GetOrDefault("isFatal", true))
                            ++this.numPermanentFatalWeapons;
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void ApplyItemData(string data)
        {
            string[] strArray = data.Split('|');
            int num = 0;
            foreach (string str in strArray)
            {
                switch (num)
                {
                    case 0:
                        this.numArmor = Convert.ToInt32(str);
                        break;
                    case 1:
                        this.numEquipment = Convert.ToInt32(str);
                        break;
                    case 2:
                        this.numSpawns = Convert.ToInt32(str);
                        break;
                    case 3:
                        this.numTeamSpawns = Convert.ToInt32(str);
                        break;
                    case 4:
                        this.numLockedDoors = Convert.ToInt32(str);
                        break;
                    case 5:
                        this.numKeys = Convert.ToInt32(str);
                        break;
                }
                ++num;
            }
        }

        public RandomLevelData Combine(RandomLevelData dat)
        {
            RandomLevelData randomLevelData = new RandomLevelData();
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (FieldInfo field in this.GetType().GetFields(bindingAttr))
            {
                if (field.FieldType == typeof(int))
                    field.SetValue(randomLevelData, (int)field.GetValue(this) + (int)field.GetValue(dat));
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Dictionary<System.Type, int> dictionary1 = field.GetValue(this) as Dictionary<System.Type, int>;
                    Dictionary<System.Type, int> dictionary2 = field.GetValue(dat) as Dictionary<System.Type, int>;
                    Dictionary<System.Type, int> dictionary3 = new Dictionary<System.Type, int>();
                    foreach (KeyValuePair<System.Type, int> keyValuePair in dictionary1)
                    {
                        if (!dictionary3.ContainsKey(keyValuePair.Key))
                            dictionary3[keyValuePair.Key] = 0;
                        dictionary3[keyValuePair.Key] += keyValuePair.Value;
                    }
                    foreach (KeyValuePair<System.Type, int> keyValuePair in dictionary2)
                    {
                        if (!dictionary3.ContainsKey(keyValuePair.Key))
                            dictionary3[keyValuePair.Key] = 0;
                        dictionary3[keyValuePair.Key] += keyValuePair.Value;
                    }
                    field.SetValue(randomLevelData, dictionary3);
                }
            }
            return randomLevelData;
        }

        public int numLinkDirections
        {
            get
            {
                int numLinkDirections = 0;
                if (this.up)
                    ++numLinkDirections;
                if (this.down)
                    ++numLinkDirections;
                if (this.left)
                    ++numLinkDirections;
                if (this.right)
                    ++numLinkDirections;
                return numLinkDirections;
            }
        }

        private Thing AddThing(Thing t, Level level)
        {
            level.AddThing(t);
            return t;
        }

        private Thing ProcessThing(Thing t, float x, float y)
        {
            if (!t.visibleInGame)
                t.visible = false;
            bool flag = this.flip;
            if (Level.symmetry)
                flag = false;
            if (Level.loadingOppositeSymmetry)
                flag = !flag;
            if (this.mainLoad && Level.symmetry && !(t is ThingContainer))
            {
                if (Level.leftSymmetry && (double)t.x > 88.0)
                    return null;
                if (!Level.leftSymmetry && (double)t.x < 88.0)
                    return null;
            }
            if (flag)
            {
                switch (t)
                {
                    case ThingContainer _:
                        break;
                    case BackgroundTile _:
                    label_14:
                        t.flipHorizontal = true;
                        if (t is Teleporter)
                        {
                            if ((t as Teleporter).direction == 2)
                            {
                                (t as Teleporter).direction = 3;
                                break;
                            }
                            if ((t as Teleporter).direction == 3)
                            {
                                (t as Teleporter).direction = 2;
                                break;
                            }
                            break;
                        }
                        break;
                    default:
                        t.SetTranslation(new Vec2((float)(-(double)t.x + (192.0 - (double)t.x) - 16.0), 0.0f));
                        goto label_14;
                }
            }
            if (t is BackgroundTile && (t as BackgroundTile).isFlipped)
                t.flipHorizontal = true;
            if (t is BackgroundTile && !(t as BackgroundTile).oppositeSymmetry)
            {
                int num = this.flip ? 1 : 0;
            }
            this.posBeforeTranslate = t.position;
            if (!(t is BackgroundTile))
                t.SetTranslation(new Vec2(x, y));
            return t;
        }

        public List<RandomLevelData.PreparedThing> PrepareThings(
          bool symmetric,
          float x,
          float y)
        {
            if (symmetric && this.isMirrored)
                symmetric = false;
            if (!symmetric && this.flip)
                Level.flipH = true;
            if (this.symmetry | symmetric)
                Level.symmetry = true;
            List<RandomLevelData.PreparedThing> preparedThingList = new List<RandomLevelData.PreparedThing>();
            List<BinaryClassChunk> binaryClassChunkList = LevelGenerator.openAirMode ? this.alternateData : this.data;
            if (binaryClassChunkList == null || binaryClassChunkList.Count == 0)
                binaryClassChunkList = this.data;
            foreach (BinaryClassChunk node in binaryClassChunkList)
            {
                Thing t1 = Thing.LoadThing(node);
                if (t1 != null)
                {
                    this.mainLoad = true;
                    Level.leftSymmetry = true;
                    Level.loadingOppositeSymmetry = false;
                    Thing thing1 = this.ProcessThing(t1, x, y);
                    if (thing1 != null)
                    {
                        if (!(thing1 is ThingContainer) && Level.symmetry && (posBeforeTranslate.x - 8.0 < 80.0 || posBeforeTranslate.x - 8.0 > 96.0))
                        {
                            Thing t2 = Thing.LoadThing(node, false);
                            if (t2 != null)
                            {
                                this.mainLoad = false;
                                Level.leftSymmetry = false;
                                Level.loadingOppositeSymmetry = true;
                                Thing thing2 = this.ProcessThing(t2, x, y);
                                if (thing2 != null)
                                    preparedThingList.Add(new RandomLevelData.PreparedThing()
                                    {
                                        thing = thing2,
                                        mirror = true
                                    });
                            }
                        }
                        preparedThingList.Add(new RandomLevelData.PreparedThing()
                        {
                            thing = thing1
                        });
                    }
                }
            }
            Level.flipH = false;
            Level.symmetry = false;
            return preparedThingList;
        }

        public void Load(
          float x,
          float y,
          Level level,
          bool symmetric,
          List<RandomLevelData.PreparedThing> pPreparedThings)
        {
            if (symmetric && this.isMirrored)
                symmetric = false;
            if (!symmetric && this.flip)
                Level.flipH = true;
            if (this.symmetry | symmetric)
                Level.symmetry = true;
            if (this.data != null)
            {
                foreach (RandomLevelData.PreparedThing pPreparedThing in pPreparedThings)
                {
                    Thing thing1 = pPreparedThing.thing;
                    if (thing1 != null && RandomLevelNode._allPreparedThings.Contains(thing1) && (ContentProperties.GetBag(thing1.GetType()).GetOrDefault("isOnlineCapable", true) || !Network.isActive))
                    {
                        Level.leftSymmetry = true;
                        Level.loadingOppositeSymmetry = false;
                        this.mainLoad = true;
                        Thing thing2 = this.AddThing(thing1, level);
                        this.mainLoad = false;
                        if (Network.isActive && thing2.isStateObject)
                        {
                            GhostManager.context.MakeGhost(thing2, initLevel: true);
                            thing2.ghostType = Editor.IDToType[thing2.GetType()];
                            DuckNetwork.AssignToHost(thing2);
                        }
                        if (thing1 is ThingContainer)
                        {
                            foreach (Thing thing3 in (thing1 as ThingContainer).things)
                            {
                                if (thing3 is BackgroundTile || thing3 is ForegroundTile)
                                {
                                    Thing thing4 = this.ProcessThing(thing3, x, y);
                                    if (thing4 != null)
                                    {
                                        this.AddThing(thing1, level);
                                        if (Network.isActive && thing4.isStateObject)
                                        {
                                            GhostManager.context.MakeGhost(thing4, initLevel: true);
                                            thing4.ghostType = Editor.IDToType[thing4.GetType()];
                                            DuckNetwork.AssignToHost(thing4);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                level.things.RefreshState();
            }
            Level.flipH = false;
            Level.symmetry = false;
        }

        public class PreparedThing
        {
            public Thing thing;
            public bool mirror;
        }
    }
}
