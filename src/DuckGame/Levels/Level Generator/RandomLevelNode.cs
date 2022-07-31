// Decompiled with JetBrains decompiler
// Type: DuckGame.RandomLevelNode
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.gridX = pX;
            this.gridY = pY;
        }

        public List<RandomLevelNode> nodes => new List<RandomLevelNode>()
    {
      this.up,
      this.down,
      this.left,
      this.right
    };

        public RandomLevelData totalData
        {
            get
            {
                this._combinedData = this.Combine();
                this.ClearFlags();
                return this._combinedData;
            }
        }

        private RandomLevelData Combine()
        {
            this.visited = true;
            RandomLevelData dat = new RandomLevelData();
            if (this.left != null && this.left.data != null && !this.left.visited)
                dat = this.left.data.Combine(dat);
            if (this.right != null && this.right.data != null && !this.right.visited)
                dat = this.right.data.Combine(dat);
            if (this.up != null && this.up.data != null && !this.up.visited)
                dat = this.up.data.Combine(dat);
            if (this.down != null && this.down.data != null && !this.down.visited)
                dat = this.down.data.Combine(dat);
            return dat.Combine(this.data);
        }

        public void ClearFlags()
        {
            this.visited = false;
            if (this.up != null && this.up.visited)
                this.up.ClearFlags();
            if (this.down != null && this.down.visited)
                this.down.ClearFlags();
            if (this.left != null && this.left.visited)
                this.left.ClearFlags();
            if (this.right == null || !this.right.visited)
                return;
            this.right.ClearFlags();
        }

        public bool LoadParts(float x, float y, Level level, int seed = 0)
        {
            Random generator = Rando.generator;
            if (seed != 0)
                Rando.generator = new Random(seed);
            Level.InitChanceGroups();
            RandomLevelNode.processing = true;
            RandomLevelNode.topLeft = new Vec2(-this.gridX * 192, -this.gridY * 144);
            RandomLevelNode._allPreparedThings = new HashSet<Thing>();
            this.PreparePartsRecurse(x, y, level);
            RandomLevelNode.processing = false;
            this.ClearFlags();
            if (null == null)
            {
                List<NGeneratorRule> ngeneratorRuleList = new List<NGeneratorRule>()
        {
          new NGeneratorRule( () =>
          {
            bool flag = false;
            for (int index = 0; index < 4; ++index)
            {
              if (NGeneratorRule.Count(RandomLevelNode._allPreparedThings,  thing =>
              {
                if (thing is IContainAThing)
                {
                  System.Type contains = (thing as IContainAThing).contains;
                  if (contains !=  null)
                  {
                    Thing thing2 = Editor.GetThing(contains);
                    return thing2 is Gun && (thing2 as Gun).isFatal;
                  }
                }
                return false;
              }) == 0)
              {
                foreach (Thing thing in (IEnumerable<Thing>) RandomLevelNode._allPreparedThings.AsEnumerable<Thing>().OrderBy<Thing, float>( t => Rando.Float(1f)))
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
            int num1 = NGeneratorRule.Count(RandomLevelNode._allPreparedThings,  thing => thing is Door && (thing as Door).locked);
            int num2 = NGeneratorRule.Count(RandomLevelNode._allPreparedThings,  thing => thing is Key);
            if (num1 > 0 && num2 == 0)
              RandomLevelNode._allPreparedThings.RemoveWhere( v => v is Door && (v as Door).locked);
            return true;
          }),
          new NGeneratorRule( () => NGeneratorRule.Count(RandomLevelNode._allPreparedThings,  thing => thing is Warpgun) > 0)
        };
            }
            this.LoadPartsRecurse(x, y, level);
            this.ClearFlags();
            Rando.generator = generator;
            if (!LevelGenerator.openAirMode)
            {
                for (int index1 = -1; index1 < this.tilesWide + 1; ++index1)
                {
                    for (int index2 = -1; index2 < this.tilesHigh + 1; ++index2)
                    {
                        RandomLevelNode randomLevelNode = null;
                        if (index1 >= 0 && index1 < this.tilesWide && index2 >= 0 && index2 < this.tilesHigh)
                            randomLevelNode = this.tiles[index1, index2];
                        if (randomLevelNode == null || randomLevelNode.data == null)
                        {
                            Vec2 vec2 = new Vec2(index1 * 192 - 8, index2 * 144 - 8) + RandomLevelNode.topLeft;
                            level.AddThing(new PyramidWall(vec2.x, vec2.y));
                        }
                    }
                }
            }
            level.things.RefreshState();
            for (int index3 = 0; index3 < this.tilesWide; ++index3)
            {
                for (int index4 = 0; index4 < this.tilesHigh; ++index4)
                {
                    RandomLevelNode randomLevelNode = null;
                    if (index3 >= 0 && index3 < this.tilesWide && index4 >= 0 && index4 < this.tilesHigh)
                        randomLevelNode = this.tiles[index3, index4];
                    if (randomLevelNode != null && randomLevelNode.data != null)
                    {
                        Vec2 vec2 = new Vec2(index3 * 192 + 96, index4 * 144 + 72) + RandomLevelNode.topLeft;
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
            if (RandomLevelNode.editorLoad)
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
            this.visited = true;
            if (this.data != null)
            {
                this._preparedThings = this.data.PrepareThings(this.mirror, x, y);
                foreach (RandomLevelData.PreparedThing preparedThing in this._preparedThings)
                    RandomLevelNode._allPreparedThings.Add(preparedThing.thing);
            }
            if (this.up != null && !this.up.visited)
                this.up.PreparePartsRecurse(x, y - 144f, level);
            if (this.down != null && !this.down.visited)
                this.down.PreparePartsRecurse(x, y + 144f, level);
            if (this.left != null && !this.left.visited)
                this.left.PreparePartsRecurse(x - 192f, y, level);
            if (this.right == null || this.right.visited)
                return;
            this.right.PreparePartsRecurse(x + 192f, y, level);
        }

        private void LoadPartsRecurse(float x, float y, Level level)
        {
            this.visited = true;
            if (this.data != null)
                this.data.Load(x, y, level, this.mirror, this._preparedThings);
            if (this.up != null && !this.up.visited)
                this.up.LoadPartsRecurse(x, y - 144f, level);
            if (this.down != null && !this.down.visited)
                this.down.LoadPartsRecurse(x, y + 144f, level);
            if (this.left != null && !this.left.visited)
                this.left.LoadPartsRecurse(x - 192f, y, level);
            if (this.right == null || this.right.visited)
                return;
            this.right.LoadPartsRecurse(x + 192f, y, level);
        }

        public void GenerateTiles(RandomLevelData tile = null, LevGenType type = LevGenType.Any, bool symmetricVal = false)
        {
            RandomLevelNode.firstGetRequiresMultiplePaths = false;
            TileConnection requirement = TileConnection.None;
            if (symmetricVal && this.tilesWide == 3 && this.gridX == 1)
                requirement = TileConnection.Left | TileConnection.Right;
            if (symmetricVal && this.tilesWide == 2 && this.gridX == 0)
                requirement = TileConnection.Right;
            else if (symmetricVal && this.tilesWide == 2 && this.gridX == 1)
                requirement = TileConnection.Left;
            if (tile == null)
                tile = LevelGenerator.GetTile(requirement, tile, false, type, filter: this.GetFilter(), requiresSpawns: true);
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
                    RandomLevelNode.firstGetRequiresMultiplePaths = true;
                }
                if (symmetricVal && !LevelGenerator.openAirMode)
                {
                    if (num == 2 && (double)Rando.Float(1f) < 0.300000011920929)
                        symmetricVal = false;
                    else if (num == 1 && (double)Rando.Float(1f) < 0.800000011920929)
                        symmetricVal = false;
                    else if (num == 1 && tile.right && (this.right == null || this.right.right == null))
                        symmetricVal = false;
                    else if (num == 1 && tile.left && (this.left == null || this.left.left == null))
                        symmetricVal = false;
                }
                this.symmetric = symmetricVal;
                this.GenerateTilesRecurse(tile, type);
                this.ClearFlags();
            }
        }

        private TileConnection GetFilter()
        {
            TileConnection filter = TileConnection.None;
            if (this.left == null)
                filter |= TileConnection.Left;
            if (this.right == null)
                filter |= TileConnection.Right;
            if (this.up == null)
                filter |= TileConnection.Up;
            return filter;
        }

        private void GenerateTilesRecurse(RandomLevelData tile, LevGenType type = LevGenType.Any)
        {
            ++RandomLevel.currentComplexityDepth;
            if (LevelGenerator.complexity > 0 && RandomLevel.currentComplexityDepth >= LevelGenerator.complexity)
                return;
            this.visited = true;
            if (tile == null)
                return;
            this.data = tile;
            this.connectionUp = this.data.up;
            this.connectionDown = this.data.down;
            this.connectionLeft = this.data.left;
            this.connectionRight = this.data.right;
            if (this.symmetric)
            {
                if (this.kingTile)
                {
                    if (this.connectionLeft && this.connectionRight || !this.connectionLeft && !this.connectionRight)
                    {
                        this.mirror = true;
                    }
                    else
                    {
                        if (!this.connectionLeft)
                        {
                            if (this.up != null)
                            {
                                this.up.left = null;
                                this.up.removeRight = true;
                            }
                            if (this.down != null)
                            {
                                this.down.left = null;
                                this.down.removeRight = true;
                            }
                            this.removeRight = true;
                            this.left = null;
                        }
                        if (!this.connectionRight)
                        {
                            if (this.up != null)
                            {
                                this.up.right = null;
                                this.up.removeLeft = true;
                            }
                            if (this.down != null)
                            {
                                this.down.right = null;
                                this.down.removeLeft = true;
                            }
                            this.removeLeft = true;
                            this.right = null;
                        }
                    }
                }
                if (this.mirror)
                    this.connectionRight = this.data.left;
                if (this.up != null)
                    this.up.mirror = this.mirror;
                if (this.down != null)
                    this.down.mirror = this.mirror;
            }
            List<TileConnection> list = new List<TileConnection>()
      {
        TileConnection.Right,
        TileConnection.Left,
        TileConnection.Up,
        TileConnection.Down
      };
            if (this.removeLeft)
                list.Remove(TileConnection.Left);
            if (this.removeRight)
                list.Remove(TileConnection.Right);
            foreach (TileConnection tileConnection in Utils.Shuffle<TileConnection>(list))
            {
                switch (tileConnection)
                {
                    case TileConnection.Left:
                        if (this.connectionLeft && this.left != null && this.left.data == null && (!this.mirror || !this.symmetric || !this.rightSymmetric))
                        {
                            if (this.mirror && this.symmetric)
                            {
                                this.leftSymmetric = true;
                                if (this.down != null)
                                {
                                    this.down.leftSymmetric = this.leftSymmetric;
                                    if (this.down.down != null)
                                        this.down.down.leftSymmetric = this.leftSymmetric;
                                }
                                if (this.up != null)
                                {
                                    this.up.leftSymmetric = this.leftSymmetric;
                                    if (this.up.up != null)
                                        this.up.up.leftSymmetric = this.leftSymmetric;
                                }
                            }
                            this.left.leftSymmetric = this.leftSymmetric;
                            this.left.rightSymmetric = this.rightSymmetric;
                            this.left.symmetric = this.symmetric;
                            this.left.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Right, tile, type: type, filter: this.left.GetFilter(), mirror: this.left.mirror), type);
                            continue;
                        }
                        continue;
                    case TileConnection.Right:
                        if (this.connectionRight && this.right != null && this.right.data == null && (!this.mirror || !this.symmetric || !this.leftSymmetric))
                        {
                            if (this.mirror && this.symmetric)
                            {
                                this.rightSymmetric = true;
                                if (this.down != null)
                                {
                                    this.down.rightSymmetric = this.rightSymmetric;
                                    if (this.down.down != null)
                                        this.down.down.rightSymmetric = this.rightSymmetric;
                                }
                                if (this.up != null)
                                {
                                    this.up.rightSymmetric = this.rightSymmetric;
                                    if (this.up.up != null)
                                        this.up.up.rightSymmetric = this.rightSymmetric;
                                }
                            }
                            this.right.leftSymmetric = this.leftSymmetric;
                            this.right.rightSymmetric = this.rightSymmetric;
                            this.right.symmetric = this.symmetric;
                            this.right.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Left, tile, type: type, filter: this.right.GetFilter(), mirror: this.right.mirror), type);
                            continue;
                        }
                        continue;
                    case TileConnection.Up:
                        if (this.connectionUp && this.up != null && this.up.data == null)
                        {
                            this.up.leftSymmetric = this.leftSymmetric;
                            this.up.rightSymmetric = this.rightSymmetric;
                            this.up.symmetric = this.symmetric;
                            this.up.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Down, tile, type: type, filter: this.up.GetFilter(), mirror: this.mirror), type);
                            continue;
                        }
                        continue;
                    case TileConnection.Down:
                        if (this.connectionDown && this.down != null && this.down.data == null)
                        {
                            this.down.leftSymmetric = this.leftSymmetric;
                            this.down.rightSymmetric = this.rightSymmetric;
                            this.down.symmetric = this.symmetric;
                            this.down.GenerateTilesRecurse(LevelGenerator.GetTile(TileConnection.Up, tile, type: type, filter: this.down.GetFilter(), mirror: this.mirror), type);
                            continue;
                        }
                        continue;
                    default:
                        continue;
                }
            }
            if (!this.kingTile || !this.symmetric)
                return;
            this.SolveSymmetry();
            if (this.up != null)
                this.up.SolveSymmetry();
            if (this.down == null)
                return;
            this.down.SolveSymmetry();
        }

        public void SolveSymmetry()
        {
            if (this.mirror)
            {
                if (this.leftSymmetric)
                {
                    if (this.left == null || this.left.data == null || this.right == null)
                        return;
                    this.right.data = !this.left.data.isMirrored ? this.left.data.Flipped() : this.left.data;
                    this.right.symmetricalPartner = this.left;
                    this.left.symmetricalPartner = this.right;
                    this.right.mirror = this.left.mirror;
                }
                else
                {
                    if (this.right == null || this.right.data == null || this.left == null)
                        return;
                    this.left.data = !this.right.data.isMirrored ? this.right.data.Flipped() : this.right.data;
                    this.right.symmetricalPartner = this.left;
                    this.left.symmetricalPartner = this.right;
                    this.left.mirror = this.right.mirror;
                }
            }
            else
            {
                if (this.data == null)
                    return;
                if (this.removeRight && this.right != null)
                {
                    this.right.data = this.data.Flipped();
                    this.right.symmetricalPartner = this;
                    this.symmetricalPartner = this.right;
                }
                if (!this.removeLeft || this.left == null)
                    return;
                this.left.data = this.data.Flipped();
                this.left.symmetricalPartner = this;
                this.symmetricalPartner = this.left;
            }
        }
    }
}
