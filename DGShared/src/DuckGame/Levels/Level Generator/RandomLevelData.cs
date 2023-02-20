// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomLevelData
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public Dictionary<Type, int> weapons = new Dictionary<Type, int>();
        public Dictionary<Type, int> spawners = new Dictionary<Type, int>();
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
            if (isMirrored)
                return this;
            return new RandomLevelData()
            {
                data = data,
                alternateData = alternateData,
                flip = !flip,
                left = right,
                right = left,
                up = up,
                down = down,
                chance = chance,
                max = max,
                file = file,
                canMirror = canMirror,
                isMirrored = isMirrored,
                numWeapons = numWeapons,
                numSuperWeapons = numSuperWeapons,
                numFatalWeapons = numFatalWeapons,
                numPermanentWeapons = numPermanentWeapons,
                numPermanentSuperWeapons = numPermanentSuperWeapons,
                numPermanentFatalWeapons = numPermanentFatalWeapons,
                numArmor = numArmor,
                numEquipment = numEquipment,
                numSpawns = numSpawns,
                numTeamSpawns = numTeamSpawns,
                numLockedDoors = numLockedDoors,
                numKeys = numKeys
            };
        }

        public RandomLevelData Symmetric()
        {
            RandomLevelData randomLevelData = new RandomLevelData()
            {
                data = data,
                alternateData = alternateData,
                flip = flip,
                up = up,
                down = down,
                symmetry = true,
                file = file,
                canMirror = true,
                isMirrored = true
            };
            randomLevelData.right = randomLevelData.left;
            randomLevelData.chance = chance;
            if (!isMirrored)
                randomLevelData.chance *= 0.75f;
            randomLevelData.max = max;
            randomLevelData.numWeapons = numWeapons;
            randomLevelData.numSuperWeapons = numSuperWeapons;
            randomLevelData.numFatalWeapons = numFatalWeapons;
            randomLevelData.numPermanentWeapons = numPermanentWeapons;
            randomLevelData.numPermanentSuperWeapons = numPermanentSuperWeapons;
            randomLevelData.numPermanentFatalWeapons = numPermanentFatalWeapons;
            randomLevelData.numArmor = numArmor;
            randomLevelData.numEquipment = numEquipment;
            randomLevelData.numSpawns = 0;
            randomLevelData.numTeamSpawns = 0;
            randomLevelData.numLockedDoors = numLockedDoors;
            randomLevelData.numKeys = numKeys;
            return randomLevelData;
        }

        public void ApplyWeaponData(string data)
        {
            weapons.Clear();
            numWeapons = 0;
            numSuperWeapons = 0;
            numFatalWeapons = 0;
            string str = data;
            char[] chArray = new char[1] { '|' };
            foreach (string name in str.Split(chArray))
            {
                if (name != "")
                {
                    try
                    {
                        Type type = Editor.GetType(name);
                        if (!weapons.ContainsKey(type))
                            weapons[type] = 0;
                        weapons[type]++;
                        ++numWeapons;
                        IReadOnlyPropertyBag bag = ContentProperties.GetBag(type);
                        if (bag.GetOrDefault("isSuperWeapon", false))
                            ++numSuperWeapons;
                        if (bag.GetOrDefault("isFatal", true))
                            ++numFatalWeapons;
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void ApplySpawnerData(string data)
        {
            spawners.Clear();
            numPermanentWeapons = 0;
            numPermanentSuperWeapons = 0;
            numPermanentFatalWeapons = 0;
            string str = data;
            char[] chArray = new char[1] { '|' };
            foreach (string name in str.Split(chArray))
            {
                if (name != "")
                {
                    try
                    {
                        Type type = Editor.GetType(name);
                        if (!spawners.ContainsKey(type))
                            spawners[type] = 0;
                        spawners[type]++;
                        ++numPermanentWeapons;
                        IReadOnlyPropertyBag bag = ContentProperties.GetBag(type);
                        if (bag.GetOrDefault("isSuperWeapon", false))
                            ++numPermanentSuperWeapons;
                        if (bag.GetOrDefault("isFatal", true))
                            ++numPermanentFatalWeapons;
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
                        numArmor = Convert.ToInt32(str);
                        break;
                    case 1:
                        numEquipment = Convert.ToInt32(str);
                        break;
                    case 2:
                        numSpawns = Convert.ToInt32(str);
                        break;
                    case 3:
                        numTeamSpawns = Convert.ToInt32(str);
                        break;
                    case 4:
                        numLockedDoors = Convert.ToInt32(str);
                        break;
                    case 5:
                        numKeys = Convert.ToInt32(str);
                        break;
                }
                ++num;
            }
        }

        public RandomLevelData Combine(RandomLevelData dat)
        {
            RandomLevelData randomLevelData = new RandomLevelData();
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (FieldInfo field in GetType().GetFields(bindingAttr))
            {
                if (field.FieldType == typeof(int))
                    field.SetValue(randomLevelData, (int)field.GetValue(this) + (int)field.GetValue(dat));
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Dictionary<Type, int> dictionary1 = field.GetValue(this) as Dictionary<Type, int>;
                    Dictionary<Type, int> dictionary2 = field.GetValue(dat) as Dictionary<Type, int>;
                    Dictionary<Type, int> dictionary3 = new Dictionary<Type, int>();
                    foreach (KeyValuePair<Type, int> keyValuePair in dictionary1)
                    {
                        if (!dictionary3.ContainsKey(keyValuePair.Key))
                            dictionary3[keyValuePair.Key] = 0;
                        dictionary3[keyValuePair.Key] += keyValuePair.Value;
                    }
                    foreach (KeyValuePair<Type, int> keyValuePair in dictionary2)
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
                if (up)
                    ++numLinkDirections;
                if (down)
                    ++numLinkDirections;
                if (left)
                    ++numLinkDirections;
                if (right)
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
            bool flag = flip;
            if (Level.symmetry)
                flag = false;
            if (Level.loadingOppositeSymmetry)
                flag = !flag;
            if (mainLoad && Level.symmetry && !(t is ThingContainer))
            {
                if (Level.leftSymmetry && t.x > 88f)
                    return null;
                if (!Level.leftSymmetry && t.x < 88f)
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
                        t.SetTranslation(new Vec2(-t.x + (192f - t.x) - 16f, 0f));
                        goto label_14;
                }
            }
            if (t is BackgroundTile && (t as BackgroundTile).isFlipped)
                t.flipHorizontal = true;
            if (t is BackgroundTile && !(t as BackgroundTile).oppositeSymmetry)
            {
                int num = flip ? 1 : 0;
            }
            posBeforeTranslate = t.position;
            if (!(t is BackgroundTile))
                t.SetTranslation(new Vec2(x, y));
            return t;
        }

        public List<PreparedThing> PrepareThings(
          bool symmetric,
          float x,
          float y)
        {
            if (symmetric && isMirrored)
                symmetric = false;
            if (!symmetric && flip)
                Level.flipH = true;
            if (symmetry | symmetric)
                Level.symmetry = true;
            List<PreparedThing> preparedThingList = new List<PreparedThing>();
            List<BinaryClassChunk> binaryClassChunkList = LevelGenerator.openAirMode ? alternateData : data;
            if (binaryClassChunkList == null || binaryClassChunkList.Count == 0)
                binaryClassChunkList = data;
            foreach (BinaryClassChunk node in binaryClassChunkList)
            {
                Thing t1 = Thing.LoadThing(node);
                if (t1 != null)
                {
                    mainLoad = true;
                    Level.leftSymmetry = true;
                    Level.loadingOppositeSymmetry = false;
                    Thing thing1 = ProcessThing(t1, x, y);
                    if (thing1 != null)
                    {
                        if (!(thing1 is ThingContainer) && Level.symmetry && (posBeforeTranslate.x - 8f < 80f || posBeforeTranslate.x - 8f > 96f))
                        {
                            Thing t2 = Thing.LoadThing(node, false);
                            if (t2 != null)
                            {
                                mainLoad = false;
                                Level.leftSymmetry = false;
                                Level.loadingOppositeSymmetry = true;
                                Thing thing2 = ProcessThing(t2, x, y);
                                if (thing2 != null)
                                    preparedThingList.Add(new PreparedThing()
                                    {
                                        thing = thing2,
                                        mirror = true
                                    });
                            }
                        }
                        preparedThingList.Add(new PreparedThing()
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
          List<PreparedThing> pPreparedThings)
        {
            if (symmetric && isMirrored)
                symmetric = false;
            if (!symmetric && flip)
                Level.flipH = true;
            if (symmetry | symmetric)
                Level.symmetry = true;
            if (data != null)
            {
                foreach (PreparedThing pPreparedThing in pPreparedThings)
                {
                    Thing thing1 = pPreparedThing.thing;
                    if (thing1 != null && RandomLevelNode._allPreparedThings.Contains(thing1) && (ContentProperties.GetBag(thing1.GetType()).GetOrDefault("isOnlineCapable", true) || !Network.isActive))
                    {
                        Level.leftSymmetry = true;
                        Level.loadingOppositeSymmetry = false;
                        mainLoad = true;
                        Thing thing2 = AddThing(thing1, level);
                        mainLoad = false;
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
                                    Thing thing4 = ProcessThing(thing3, x, y);
                                    if (thing4 != null)
                                    {
                                        AddThing(thing1, level);
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
