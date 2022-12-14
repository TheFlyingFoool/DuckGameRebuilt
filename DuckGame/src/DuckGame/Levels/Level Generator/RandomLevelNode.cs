// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomLevelNode
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class RandomLevelNode
    {
        private const float _width = 192f;
        private const float _height = 144f;
        public int gridX;
        public int gridY;
        public RandomLevelNode[,] map;
        public int seed;
        public RandomLevelNode up;
        public RandomLevelNode down;
        public RandomLevelNode left;
        public RandomLevelNode right;
        public bool isCentered;
        public bool symmetric;
        public bool mirror;
        public RandomLevelNode symmetricalPartner;
        private RandomLevelData _combinedData;
        public RandomLevelNode[,] tiles;
        public int tilesWide;
        public int tilesHigh;
        public static bool editorLoad = false;
        public static bool processing = false;
        public List<RandomLevelData.PreparedThing> _preparedThings;
        public static HashSet<Thing> _allPreparedThings;
        public static Vec2 topLeft = new Vec2(0f, 0f);
        public static bool firstGetRequiresMultiplePaths = false;
        public bool kingTile;
        private bool leftSymmetric;
        private bool rightSymmetric;
        private bool connectionUp;
        private bool connectionDown;
        private bool connectionLeft;
        private bool connectionRight;
        private bool removeLeft;
        private bool removeRight;
        public RandomLevelData data;
        public bool visited;

        public RandomLevelNode(int pX, int pY)
        {
            gridX = pX;
            gridY = pY;
        }

        public List<RandomLevelNode> nodes => new List<RandomLevelNode>()
    {
      up,
      down,
      left,
      right
    };

        public RandomLevelData totalData
        {
            get
            {
                _combinedData = Combine();
                ClearFlags();
                return _combinedData;
            }
        }

        private RandomLevelData Combine()
        {
            visited = true;
            RandomLevelData dat = new RandomLevelData();
            if (left != null && left.data != null && !left.visited)
                dat = left.data.Combine(dat);
            if (right != null && right.data != null && !right.visited)
                dat = right.data.Combine(dat);
            if (up != null && up.data != null && !up.visited)
                dat = up.data.Combine(dat);
            if (down != null && down.data != null && !down.visited)
                dat = down.data.Combine(dat);
            return dat.Combine(data);
        }

        public void ClearFlags()
        {
            visited = false;
            if (up != null && up.visited)
                up.ClearFlags();
            if (down != null && down.visited)
                down.ClearFlags();
            if (left != null && left.visited)
                left.ClearFlags();
            if (right == null || !right.visited)
                return;
            right.ClearFlags();
        }

        public bool LoadParts(float x, float y, Level level, int seed = 0)
        {
            Random generator = Rando.generator;
            if (seed != 0)
                Rando.generator = new Random(seed);
            Level.InitChanceGroups();
            processing = true;
            topLeft = new Vec2((float)(-(float)this.gridX * 192), (float)(-(float)this.gridY * 144));
            _allPreparedThings = new HashSet<Thing>();
            PreparePartsRecurse(x, y, level);
            processing = false;
            ClearFlags();
            if (null == null)
            {
                List<NGeneratorRule> ngeneratorRuleList = new List<NGeneratorRule>()
        {
          new NGeneratorRule( () =>
          {
            bool flag = false;
            for (int index = 0; index < 4; ++index)
            {
              if (NGeneratorRule.Count(_allPreparedThings,  thing =>
              {
                if (thing is IContainAThing)
                {
                  Type contains = (thing as IContainAThing).contains;
                  if (contains !=  null)
                  {
                    Thing thing2 = Editor.GetThing(contains);
                    return thing2 is Gun && (thing2 as Gun).isFatal;
                  }
                }
                return false;
              }) == 0)
              {
                foreach (Thing thing in (IEnumerable<Thing>) _allPreparedThings.AsEnumerable().OrderBy( t => Rando.Float(1f)))
                {
                  if (thing is IContainPossibleThings)
                    (thing as IContainPossibleThings).PreparePossibilities();
                }
              }
              else
              {
                flag = true;
                break;
              }
            }
            return flag;
          }),
          new NGeneratorRule( () =>
          {
            int num1 = NGeneratorRule.Count(_allPreparedThings,  thing => thing is Door && (thing as Door).locked);
            int num2 = NGeneratorRule.Count(_allPreparedThings,  thing => thing is Key);
            if (num1 > 0 && num2 == 0)
              _allPreparedThings.RemoveWhere( v => v is Door && (v as Door).locked);
            return true;
          }),
          new NGeneratorRule( () => NGeneratorRule.Count(_allPreparedThings,  thing => thing is Warpgun) > 0)
        };
            }
            LoadPartsRecurse(x, y, level);
            ClearFlags();
            Rando.generator = generator;
            if (!LevelGenerator.openAirMode)
            {
                for (int xpos = -1; xpos < tilesWide + 1; ++xpos)
                {
                    for (int ypos = -1; ypos < tilesHigh + 1; ++ypos)
                    {
                        RandomLevelNode randomLevelNode = null;
                        if (xpos >= 0 && xpos < tilesWide && ypos >= 0 && ypos < tilesHigh)
                            randomLevelNode = tiles[xpos, ypos];
                        if (randomLevelNode == null || randomLevelNode.data == null)
                        {
                            Vec2 pyramidWallPos = new Vec2((float)(xpos * 192 - 8), (float)(ypos * 144 - 8)) + topLeft;
                            level.AddThing(new PyramidWall(pyramidWallPos.x, pyramidWallPos.y));
                        }
                    }
                }
            }
            level.things.RefreshState();
            for (int index3 = 0; index3 < tilesWide; ++index3)
            {
                for (int index4 = 0; index4 < tilesHigh; ++index4)
                {
                    RandomLevelNode randomLevelNode = null;
                    if (index3 >= 0 && index3 < tilesWide && index4 >= 0 && index4 < tilesHigh)
                        randomLevelNode = tiles[index3, index4];
                    if (randomLevelNode != null && randomLevelNode.data != null)
                    {
                        Vec2 vec2 = new Vec2((float)(index3 * 192 + 96), (float)(index4 * 144 + 72)) + topLeft;
                        PyramidWall pyramidWall1 = Level.CheckPoint<PyramidWall>(vec2 + new Vec2(-192f, 0f));
                        if (pyramidWall1 != null)
                            pyramidWall1.hasRight = true;
                        PyramidWall pyramidWall2 = Level.CheckPoint<PyramidWall>(vec2 + new Vec2(192f, 0f));
                        if (pyramidWall2 != null)
                            pyramidWall2.hasLeft = true;
                        PyramidWall pyramidWall3 = Level.CheckPoint<PyramidWall>(vec2 + new Vec2(0f, -144f));
                        if (pyramidWall3 != null)
                            pyramidWall3.hasDown = true;
                        PyramidWall pyramidWall4 = Level.CheckPoint<PyramidWall>(vec2 + new Vec2(0f, 144f));
                        if (pyramidWall4 != null)
                            pyramidWall4.hasUp = true;
                    }
                }
            }
            foreach (PyramidWall pyramidWall in level.things[typeof(PyramidWall)])
                pyramidWall.AddExtraWalls();
            foreach (PyramidDoor pyramidDoor in level.things[typeof(PyramidDoor)])
            {
                Block block = level.CollisionPoint<Block>(pyramidDoor.position + new Vec2(-16f, 0f), pyramidDoor) ?? level.CollisionPoint<Block>(pyramidDoor.position + new Vec2(16f, 0f), pyramidDoor);
                if (block != null && !(block is PyramidDoor) && !(block is Door))
                {
                    level.RemoveThing(pyramidDoor);
                    Level.Add(new PyramidTileset(pyramidDoor.x, pyramidDoor.y - 16f));
                    Level.Add(new PyramidTileset(pyramidDoor.x, pyramidDoor.y));
                }
                else
                {
                    Block t1 = level.CollisionPoint<Block>(pyramidDoor.x, pyramidDoor.y, pyramidDoor);
                    if (t1 != null)
                        level.RemoveThing(t1);
                    Block t2 = level.CollisionPoint<Block>(pyramidDoor.x, pyramidDoor.y - 16f, pyramidDoor);
                    if (t2 != null)
                        level.RemoveThing(t2);
                }
            }
            foreach (Door door in level.things[typeof(Door)])
            {
                switch (level.CollisionLine<Block>(door.position + new Vec2(-16f, 0f), door.position + new Vec2(16f, 0f), door))
                {
                    case null:
                    case PyramidDoor _:
                    case Door _:
                        continue;
                    default:
                        level.RemoveThing(door);
                        Level.Add(new PyramidTileset(door.x, door.y - 16f));
                        Level.Add(new PyramidTileset(door.x, door.y));
                        continue;
                }
            }
            foreach (Teleporter t in level.things[typeof(Teleporter)])
            {
                t.InitLinks();
                if (t._link == null)
                {
                    if (t.direction == 2)
                        t.direction = 3;
                    else if (t.direction == 3)
                        t.direction = 2;
                    else if (t.direction == 0)
                        t.direction = 1;
                    else if (t.direction == 1)
                        t.direction = 0;
                    t.InitLinks();
                    if (t._link == null)
                    {
                        level.RemoveThing(t);
                        Level.Add(new PyramidTileset(t.x, t.y - 16f));
                        Level.Add(new PyramidTileset(t.x, t.y));
                    }
                }
            }
            if (editorLoad)
            {
                level.things.RefreshState();
                foreach (AutoBlock autoBlock in level.things[typeof(AutoBlock)])
                {
                    autoBlock.DoPositioning();
                    if (!(autoBlock is BlockGroup))
                        autoBlock.FindFrame();
                }
                foreach (AutoPlatform autoPlatform in level.things[typeof(AutoPlatform)])
                {
                    autoPlatform.DoPositioning();
                    autoPlatform.FindFrame();
                }
            }
            LightingTwoPointOH t3 = new LightingTwoPointOH
            {
                visible = false
            };
            level.AddThing(t3);
            return true;
        }

        private void PreparePartsRecurse(float x, float y, Level level)
        {
            visited = true;
            if (data != null)
            {
                _preparedThings = data.PrepareThings(mirror, x, y);
                foreach (RandomLevelData.PreparedThing preparedThing in _preparedThings)
                    _allPreparedThings.Add(preparedThing.thing);
            }
            if (up != null && !up.visited)
                up.PreparePartsRecurse(x, y - 144f, level);
            if (down != null && !down.visited)
                down.PreparePartsRecurse(x, y + 144f, level);
            if (left != null && !left.visited)
                left.PreparePartsRecurse(x - 192f, y, level);
            if (right == null || right.visited)
                return;
            right.PreparePartsRecurse(x + 192f, y, level);
        }

        private void LoadPartsRecurse(float x, float y, Level level)
        {
            visited = true;
            if (data != null)
                data.Load(x, y, level, mirror, _preparedThings);
            if (up != null && !up.visited)
                up.LoadPartsRecurse(x, y - 144f, level);
            if (down != null && !down.visited)
                down.LoadPartsRecurse(x, y + 144f, level);
            if (left != null && !left.visited)
                left.LoadPartsRecurse(x - 192f, y, level);
            if (right == null || right.visited)
                return;
            right.LoadPartsRecurse(x + 192f, y, level);
        }

        public void GenerateTiles(RandomLevelData tile = null, LevGenType type = LevGenType.Any, bool symmetricVal = false)
        {
            firstGetRequiresMultiplePaths = false;
            TileConnection requirement = TileConnection.None;
            if (symmetricVal && tilesWide == 3 && gridX == 1)
                requirement = TileConnection.Left | TileConnection.Right;
            if (symmetricVal && tilesWide == 2 && gridX == 0)
                requirement = TileConnection.Right;
            else if (symmetricVal && tilesWide == 2 && gridX == 1)
                requirement = TileConnection.Left;
            if (tile == null)
                tile = LevelGenerator.GetTile(requirement, tile, false, type, filter: GetFilter(), requiresSpawns: true);
            if (tile == null)
            {
                DevConsole.Log("|DGRED|RandomLevel.GenerateTiles had a null tile! This should never happen!");
            }
            else
            {
                int num = 0;
                if (tile.left)
                    ++num;
                if (tile.right)
                    ++num;
                if (tile.up)
                    ++num;
                if (tile.down)
                    ++num;
                if (num <= 1)
                {
                    symmetricVal = false;
                    firstGetRequiresMultiplePaths = true;
                }
                if (symmetricVal && !LevelGenerator.openAirMode)
                {
                    if (num == 2 && Rando.Float(1f) < 0.3f)
                        symmetricVal = false;
                    else if (num == 1 && Rando.Float(1f) < 0.8f)
                        symmetricVal = false;
                    else if (num == 1 && tile.right && (right == null || right.right == null))
                        symmetricVal = false;
                    else if (num == 1 && tile.left && (left == null || left.left == null))
                        symmetricVal = false;
                }
                symmetric = symmetricVal;
                GenerateTilesRecurse(tile, type);
                ClearFlags();
            }
        }

        private TileConnection GetFilter()
        {
            TileConnection filter = TileConnection.None;
            if (left == null)
                filter |= TileConnection.Left;
            if (right == null)
                filter |= TileConnection.Right;
            if (up == null)
                filter |= TileConnection.Up;
            return filter;
        }

        private void GenerateTilesRecurse(RandomLevelData tile, LevGenType type = LevGenType.Any)
        {
            ++RandomLevel.currentComplexityDepth;
            if (LevelGenerator.complexity > 0 && RandomLevel.currentComplexityDepth >= LevelGenerator.complexity)
                return;
            visited = true;
            if (tile == null)
                return;
            data = tile;
            connectionUp = data.up;
            connectionDown = data.down;
            connectionLeft = data.left;
            connectionRight = data.right;
            if (symmetric)
            {
                if (kingTile)
                {
                    if (connectionLeft && connectionRight || !connectionLeft && !connectionRight)
                    {
                        mirror = true;
                    }
                    else
                    {
                        if (!connectionLeft)
                        {
                            if (up != null)
                            {
                                up.left = null;
                                up.removeRight = true;
                            }
                            if (down != null)
                            {
                                down.left = null;
                                down.removeRight = true;
                            }
                            removeRight = true;
                            left = null;
                        }
                        if (!connectionRight)
                        {
                            if (up != null)
                            {
                                up.right = null;
                                up.removeLeft = true;
                            }
                            if (down != null)
                            {
                                down.right = null;
                                down.removeLeft = true;
                            }
                            removeLeft = true;
                            right = null;
                        }
                    }
                }
                if (mirror)
                    connectionRight = data.left;
                if (up != null)
                    up.mirror = mirror;
                if (down != null)
                    down.mirror = mirror;
            }
            List<TileConnection> list = new List<TileConnection>()
            {
            TileConnection.Right,
            TileConnection.Left,
            TileConnection.Up,
            TileConnection.Down
            };
            if (removeLeft)
                list.Remove(TileConnection.Left);
            if (removeRight)
                list.Remove(TileConnection.Right);
            foreach (TileConnection tileConnection in Utils.Shuffle(list))
            {
                switch (tileConnection)
                {
                    case TileConnection.Left:
                        if (connectionLeft && left != null && left.data == null && (!mirror || !symmetric || !rightSymmetric))
                        {
                            if (mirror && symmetric)
                            {
                                leftSymmetric = true;
                                if (down != null)
                                {
                                    down.leftSymmetric = leftSymmetric;
                                    if (down.down != null)
                                        down.down.leftSymmetric = leftSymmetric;
                                }
                                if (up != null)
                                {
                                    up.leftSymmetric = leftSymmetric;
                                    if (up.up != null)
                                        up.up.leftSymmetric = leftSymmetric;
                                }
                            }
                            left.leftSymmetric = leftSymmetric;
                            left.rightSymmetric = rightSymmetric;
                            left.symmetric = symmetric;
                            left.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Right, tile, type: type, filter: left.GetFilter(), mirror: left.mirror), type);
                            continue;
                        }
                        continue;
                    case TileConnection.Right:
                        if (connectionRight && right != null && right.data == null && (!mirror || !symmetric || !leftSymmetric))
                        {
                            if (mirror && symmetric)
                            {
                                rightSymmetric = true;
                                if (down != null)
                                {
                                    down.rightSymmetric = rightSymmetric;
                                    if (down.down != null)
                                        down.down.rightSymmetric = rightSymmetric;
                                }
                                if (up != null)
                                {
                                    up.rightSymmetric = rightSymmetric;
                                    if (up.up != null)
                                        up.up.rightSymmetric = rightSymmetric;
                                }
                            }
                            right.leftSymmetric = leftSymmetric;
                            right.rightSymmetric = rightSymmetric;
                            right.symmetric = symmetric;
                            right.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Left, tile, type: type, filter: right.GetFilter(), mirror: right.mirror), type);
                            continue;
                        }
                        continue;
                    case TileConnection.Up:
                        if (connectionUp && up != null && up.data == null)
                        {
                            up.leftSymmetric = leftSymmetric;
                            up.rightSymmetric = rightSymmetric;
                            up.symmetric = symmetric;
                            up.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Down, tile, type: type, filter: up.GetFilter(), mirror: mirror), type);
                            continue;
                        }
                        continue;
                    case TileConnection.Down:
                        if (connectionDown && down != null && down.data == null)
                        {
                            down.leftSymmetric = leftSymmetric;
                            down.rightSymmetric = rightSymmetric;
                            down.symmetric = symmetric;
                            down.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Up, tile, type: type, filter: down.GetFilter(), mirror: mirror), type);
                            continue;
                        }
                        continue;
                    default:
                        continue;
                }
            }
            if (!kingTile || !symmetric)
                return;
            SolveSymmetry();
            if (up != null)
                up.SolveSymmetry();
            if (down == null)
                return;
            down.SolveSymmetry();
        }

        public void SolveSymmetry()
        {
            if (mirror)
            {
                if (leftSymmetric)
                {
                    if (left == null || left.data == null || right == null)
                        return;
                    right.data = !left.data.isMirrored ? left.data.Flipped() : left.data;
                    right.symmetricalPartner = left;
                    left.symmetricalPartner = right;
                    right.mirror = left.mirror;
                }
                else
                {
                    if (right == null || right.data == null || left == null)
                        return;
                    left.data = !right.data.isMirrored ? right.data.Flipped() : right.data;
                    right.symmetricalPartner = left;
                    left.symmetricalPartner = right;
                    left.mirror = right.mirror;
                }
            }
            else
            {
                if (data == null)
                    return;
                if (removeRight && right != null)
                {
                    right.data = data.Flipped();
                    right.symmetricalPartner = this;
                    symmetricalPartner = right;
                }
                if (!removeLeft || left == null)
                    return;
                left.data = data.Flipped();
                left.symmetricalPartner = this;
                symmetricalPartner = left;
            }
        }
    }
}
