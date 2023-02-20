// Decompiled with JetBrains decompiler
// Type: DuckGame.LevelGenerator
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class LevelGenerator
    {
        public const int tileWidth = 12;
        public const int tileHeight = 9;
        private static List<RandomLevelData> _tiles = new List<RandomLevelData>();
        private static MultiMap<TileConnection, RandomLevelData> _connections = new MultiMap<TileConnection, RandomLevelData>();
        private static Dictionary<string, int> _used = new Dictionary<string, int>();
        public static bool openAirMode = false;
        public static int complexity = 0;

        public static List<RandomLevelData> tiles => _tiles;

        public static List<RandomLevelData> GetTiles(
          TileConnection requirement,
          TileConnection filter,
          bool mirror)
        {
            if (requirement == TileConnection.None && filter == TileConnection.None)
                return new List<RandomLevelData>(_tiles);
            bool flag1 = (requirement & TileConnection.Left) != 0;
            bool flag2 = (requirement & TileConnection.Right) != 0;
            bool flag3 = (requirement & TileConnection.Up) != 0;
            bool flag4 = (requirement & TileConnection.Down) != 0;
            bool flag5 = (filter & TileConnection.Left) != 0;
            bool flag6 = (filter & TileConnection.Right) != 0;
            bool flag7 = (filter & TileConnection.Up) != 0;
            bool flag8 = (filter & TileConnection.Down) != 0;
            List<RandomLevelData> tiles = new List<RandomLevelData>();
            foreach (RandomLevelData tile in _tiles)
            {
                if ((!mirror || tile.isMirrored) && (tile.left || !flag1) && (tile.right || !flag2) && (tile.up || !flag3) && (tile.down || !flag4) && !(tile.left & flag5) && !(tile.right & flag6) && !(tile.up & flag7) && !(tile.down & flag8))
                    tiles.Add(tile);
            }
            return tiles;
        }

        public static RandomLevelData GetTile(
          TileConnection requirement,
          RandomLevelData current,
          bool canBeNull = true,
          LevGenType type = LevGenType.Any,
          Func<RandomLevelData, bool> lambdaReq = null,
          TileConnection filter = TileConnection.None,
          bool requiresSpawns = false,
          bool mirror = false,
          int numLinksRequired = -1)
        {
            List<RandomLevelData> tiles = GetTiles(requirement, filter, mirror);
            RandomLevelData tile1 = new RandomLevelData();
            bool flag = false;
            int num1 = 0;
            bool requiresMultiplePaths = RandomLevelNode.firstGetRequiresMultiplePaths;
            RandomLevelNode.firstGetRequiresMultiplePaths = false;
            while (tiles.Count != 0)
            {
                RandomLevelData tile2 = tiles[Rando.Int(tiles.Count - 1)];
                if (tile2.numSpawns <= 0 & requiresSpawns)
                    tiles.Remove(tile2);
                else if (lambdaReq != null && !lambdaReq(tile2))
                    tiles.Remove(tile2);
                else if (tile2.numLinkDirections == 0 || requiresMultiplePaths && tile2.numLinkDirections == 1)
                {
                    tiles.Remove(tile2);
                }
                else
                {
                    int num2;
                    if (_used.TryGetValue(tile2.file, out num2) && num2 >= tile2.max)
                    {
                        tiles.Remove(tile2);
                    }
                    else
                    {
                        float chance = tile2.chance;
                        if (tile2.numLinkDirections == 1)
                            chance *= 0.65f;
                        if (tiles.Count == 1 && !canBeNull)
                        {
                            if (flag)
                                return tile1;
                            tile2 = tiles[0];
                        }
                        else if (tile2.chance != 1f && Rando.Float(1f) > chance)
                        {
                            tile1 = tile2;
                            tiles.Remove(tile2);
                            flag = true;
                            tile2 = null;
                        }
                        if (tile2 != null)
                        {
                            if (tile2.chance == 1f && Rando.Float(1f) < 0.3f && num1 < 4)
                            {
                                tile1 = tile2;
                                ++num1;
                            }
                            else
                            {
                                if (_used.ContainsKey(tile2.file))
                                    ++_used[tile2.file];
                                else
                                    _used[tile2.file] = 1;
                                return tile2;
                            }
                        }
                        else if (tiles.Count == 0)
                            return flag ? tile1 : null;
                    }
                }
            }
            return flag ? tile1 : null;
        }

        public static void ReInitialize()
        {
            _tiles.Clear();
            _connections.Clear();
            Content.ReloadLevels("pyramid");
            Initialize();
        }

        public static RandomLevelData LoadInTile(string tile, string realName = null)
        {
            RandomLevelData element = new RandomLevelData
            {
                file = tile
            };
            if (realName != null)
                element.file = realName;
            LevelData levelData = Content.GetLevel(tile) ?? DuckFile.LoadLevel(tile);
            int sideMask = levelData.proceduralData.sideMask;
            if (sideMask != 0)
            {
                if ((sideMask & 1) != 0)
                    element.up = true;
                if ((sideMask & 2) != 0)
                    element.right = true;
                if ((sideMask & 4) != 0)
                    element.down = true;
                if ((sideMask & 8) != 0)
                    element.left = true;
            }
            element.chance = levelData.proceduralData.chance;
            element.max = levelData.proceduralData.maxPerLevel;
            element.single = levelData.proceduralData.enableSingle;
            element.multi = levelData.proceduralData.enableMulti;
            element.ApplyWeaponData(levelData.proceduralData.weaponConfig);
            element.ApplySpawnerData(levelData.proceduralData.spawnerConfig);
            element.numArmor = levelData.proceduralData.numArmor;
            element.numEquipment = levelData.proceduralData.numEquipment;
            element.numKeys = levelData.proceduralData.numKeys;
            element.numLockedDoors = levelData.proceduralData.numLockedDoors;
            element.numSpawns = levelData.proceduralData.numSpawns;
            element.numTeamSpawns = levelData.proceduralData.numTeamSpawns;
            element.canMirror = levelData.proceduralData.canMirror;
            element.isMirrored = levelData.proceduralData.isMirrored;
            element.data = levelData.objects.objects;
            element.alternateData = levelData.proceduralData.openAirAlternateObjects.objects;
            _tiles.Add(element);
            if (element.up)
                _connections.Add(TileConnection.Up, element);
            if (element.down)
                _connections.Add(TileConnection.Down, element);
            if (element.left)
            {
                _connections.Add(TileConnection.Left, element);
                _connections.Add(TileConnection.Right, element.Flipped());
            }
            if (element.right)
            {
                _connections.Add(TileConnection.Right, element);
                _connections.Add(TileConnection.Left, element.Flipped());
            }
            _tiles.Add(element.Flipped());
            if (element.canMirror)
                _tiles.Add(element.Symmetric());
            return element;
        }

        public static void Initialize()
        {
            foreach (string level in Content.GetLevels("pyramid", LevelLocation.Content))
                LoadInTile(level);
        }

        public static RandomLevelNode MakeLevel(
          RandomLevelData tile = null,
          bool allowSymmetry = true,
          int seed = 0,
          LevGenType type = LevGenType.Any,
          int varwide = 0,
          int varhigh = 0,
          int genX = 1,
          int genY = 1)
        {
            Random generator = Rando.generator;
            if (seed == 0)
                seed = Rando.Int(2147483646);
            Rando.generator = new Random(seed);
            varwide = 0;
            varhigh = 0;
            openAirMode = Rando.Float(1f) > 0.75f;
            bool flag = false;
            _used.Clear();
            int length1 = varwide;
            int length2 = varhigh;
            if (varwide == 0)
                length1 = Rando.Float(1f) <= 0.6f ? 3 : 2;
            if (varhigh == 0)
            {
                float num = Rando.Float(1f);
                if (num > 0.85f && length1 == 3)
                    ;
                length2 = num <= 0.45f ? 3 : 2;
            }
            if (flag)
                length1 = length2 = 3;
            genX = Rando.Int(length1 - 2);
            genY = Rando.Int(length2 - 2);
            if (Rando.Float(1f) > 0.75f)
                ++genX;
            if (Rando.Float(1f) > 0.75f)
                ++genY;
            bool symmetricVal = true;
            if (symmetricVal)
            {
                switch (length1)
                {
                    case 2:
                        genX = 0;
                        break;
                    case 3:
                        genX = 1;
                        break;
                }
                switch (length2)
                {
                    case 2:
                        genY = 0;
                        break;
                    case 3:
                        genY = 1;
                        break;
                }
            }
            else
            {
                if (length1 == 3 && Rando.Float(1f) < 0.5f)
                    genX = 1;
                if (length2 == 3 && Rando.Float(1f) < 0.5f)
                    genY = 1;
            }
            if (length1 == 1)
            {
                length1 = Rando.Float(1f) > 0.5f ? 2 : 3;
                genX = 0;
                genY = 1;
            }
            RandomLevelNode[,] randomLevelNodeArray = new RandomLevelNode[length1, length2];
            for (int pX = 0; pX < length1; ++pX)
            {
                for (int pY = 0; pY < length2; ++pY)
                {
                    randomLevelNodeArray[pX, pY] = new RandomLevelNode(pX, pY)
                    {
                        map = randomLevelNodeArray
                    };
                }
            }
            for (int index1 = 0; index1 < length1; ++index1)
            {
                for (int index2 = 0; index2 < length2; ++index2)
                {
                    RandomLevelNode randomLevelNode = randomLevelNodeArray[index1, index2];
                    if (index1 > 0)
                        randomLevelNode.left = randomLevelNodeArray[index1 - 1, index2];
                    if (index1 < length1 - 1)
                        randomLevelNode.right = randomLevelNodeArray[index1 + 1, index2];
                    if (index2 > 0)
                        randomLevelNode.up = randomLevelNodeArray[index1, index2 - 1];
                    if (index2 < length2 - 1)
                        randomLevelNode.down = randomLevelNodeArray[index1, index2 + 1];
                }
            }
            if (tile != null)
                _used[tile.file] = 1;
            RandomLevel.currentComplexityDepth = 0;
            randomLevelNodeArray[genX, genY].tilesWide = length1;
            randomLevelNodeArray[genX, genY].tilesHigh = length2;
            randomLevelNodeArray[genX, genY].kingTile = true;
            randomLevelNodeArray[genX, genY].GenerateTiles(tile, type, symmetricVal);
            List<RandomLevelNode> randomLevelNodeList = new List<RandomLevelNode>();
            for (int index3 = 0; index3 < length1; ++index3)
            {
                for (int index4 = 0; index4 < length2; ++index4)
                {
                    RandomLevelNode randomLevelNode = randomLevelNodeArray[index3, index4];
                    if (randomLevelNode.data != null)
                        randomLevelNodeList.Add(randomLevelNode);
                }
            }
            Rando.generator = generator;
            randomLevelNodeArray[genX, genY].seed = seed;
            randomLevelNodeArray[genX, genY].tiles = randomLevelNodeArray;
            return randomLevelNodeArray[genX, genY];
        }
    }
}
