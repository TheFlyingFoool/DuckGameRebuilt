// Decompiled with JetBrains decompiler
// Type: DuckGame.Block
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class Block : MaterialThing, IPlatform
    {
        public bool shouldWreck;
        public bool skipWreck;
        protected bool _groupedWithNeighbors;
        public BlockGroup group;
        protected bool _neighborsInitialized;
        protected Block _leftBlock;
        protected Block _rightBlock;
        protected Block _upBlock;
        protected Block _downBlock;
        private BlockStructure _structure;
        private List<ConcaveLine> _concaveLines;
        private bool _hit;
        private bool _pathed;
        private ConcaveLine nullLine = new ConcaveLine(Vec2.Zero, Vec2.Zero);
        private List<ConcaveLine> G1;
        private List<ConcaveLine> G2;
        private Dictionary<ConcaveLine, ConcaveLine> pairG1val = new Dictionary<ConcaveLine, ConcaveLine>();
        private Dictionary<ConcaveLine, ConcaveLine> pairG2val = new Dictionary<ConcaveLine, ConcaveLine>();
        private List<KeyValuePair<ConcaveLine, ConcaveLine>> matchingSet = new List<KeyValuePair<ConcaveLine, ConcaveLine>>();
        //private List<ConcaveLine> minimumVertex = new List<ConcaveLine>();
        //private int _initializeWait = 20;

        public bool hit
        {
            get => _hit;
            set => _hit = value;
        }
        public bool groupedWithNeighbors
        {
            get => _groupedWithNeighbors;
            set => _groupedWithNeighbors = value;
        }

        public bool neighborsInitialized
        {
            get => _neighborsInitialized;
            set => _neighborsInitialized = value;
        }

        public Block leftBlock
        {
            get => _leftBlock;
            set => _leftBlock = value;
        }

        public Block rightBlock
        {
            get => _rightBlock;
            set => _rightBlock = value;
        }

        public Block upBlock
        {
            get => _upBlock;
            set => _upBlock = value;
        }

        public Block downBlock
        {
            get => _downBlock;
            set => _downBlock = value;
        }

        public BlockStructure structure
        {
            get => _structure;
            set => _structure = value;
        }

        public bool pathed
        {
            get => _pathed;
            set => _pathed = value;
        }

        public bool BFS(
          Dictionary<ConcaveLine, int> dist,
          Dictionary<ConcaveLine, ConcaveLine> pairG1,
          Dictionary<ConcaveLine, ConcaveLine> pairG2)
        {
            Queue<ConcaveLine> concaveLineQueue = new Queue<ConcaveLine>();
            using (List<ConcaveLine>.Enumerator enumerator = G1.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    ConcaveLine current = enumerator.Current;
                    if (pairG1[current] == nullLine)
                    {
                        dist[current] = 0;
                        concaveLineQueue.Enqueue(current);
                    }
                    else
                        dist[current] = int.MaxValue;
                }
            }
            dist[nullLine] = int.MaxValue;
            while (concaveLineQueue.Count > 0)
            {
                ConcaveLine key = concaveLineQueue.Dequeue();
                if (dist[key] < dist[nullLine])
                {
                    foreach (ConcaveLine intersect in key.intersects)
                    {
                        if (dist[pairG2[intersect]] == int.MaxValue)
                        {
                            dist[pairG2[intersect]] = dist[key] + 1;
                            concaveLineQueue.Enqueue(pairG2[intersect]);
                        }
                    }
                }
            }
            return dist[nullLine] != int.MaxValue;
        }

        public bool DFS(
          Dictionary<ConcaveLine, int> dist,
          Dictionary<ConcaveLine, ConcaveLine> pairG1,
          Dictionary<ConcaveLine, ConcaveLine> pairG2,
          ConcaveLine v)
        {
            if (v == nullLine)
                return true;
            foreach (ConcaveLine intersect in v.intersects)
            {
                if (dist[pairG2[intersect]] == dist[v] + 1 && DFS(dist, pairG1, pairG2, pairG2[intersect]))
                {
                    pairG2[intersect] = v;
                    pairG1[v] = intersect;
                    return true;
                }
            }
            dist[v] = int.MaxValue;
            return false;
        }

        public void Calculate(List<ConcaveLine> G1val, List<ConcaveLine> G2val)
        {
            G1 = G1val;
            G2 = G2val;
            Dictionary<ConcaveLine, int> dist = new Dictionary<ConcaveLine, int>();
            foreach (ConcaveLine key in G1)
            {
                dist[key] = 0;
                pairG1val[key] = nullLine;
            }
            foreach (ConcaveLine key in G2)
            {
                dist[key] = 0;
                pairG2val[key] = nullLine;
            }
            int num = 0;
            while (BFS(dist, pairG1val, pairG2val))
            {
                foreach (ConcaveLine concaveLine in G1)
                {
                    if (pairG1val[concaveLine] == nullLine && DFS(dist, pairG1val, pairG2val, concaveLine))
                    {
                        ++num;
                        matchingSet.Add(new KeyValuePair<ConcaveLine, ConcaveLine>(concaveLine, pairG1val[concaveLine]));
                    }
                }
            }
            List<ConcaveLine> concaveLineList = new List<ConcaveLine>();
            concaveLineList.AddRange(_concaveLines);
            foreach (KeyValuePair<ConcaveLine, ConcaveLine> matching in matchingSet)
            {
                concaveLineList.Remove(matching.Key);
                concaveLineList.Remove(matching.Value);
            }
        }

        public BlockCorner GetNearestCorner(Vec2 to)
        {
            List<BlockCorner> groupCorners = GetGroupCorners();
            float num = 9999999f;
            BlockCorner nearestCorner = null;
            foreach (BlockCorner blockCorner in groupCorners)
            {
                float length = (blockCorner.corner - to).length;
                if (length < num)
                {
                    num = length;
                    nearestCorner = blockCorner;
                }
            }
            return nearestCorner;
        }
        public void CalculateConcaveLines()
        {
            _concaveLines = new List<ConcaveLine>();
            List<ConcaveLine> concaveLineList1 = new List<ConcaveLine>();
            List<ConcaveLine> concaveLineList2 = new List<ConcaveLine>();
            List<BlockCorner> list = _structure.corners.Where(v => v.wallCorner).ToList();
            foreach (BlockCorner blockCorner1 in list)
            {
                foreach (BlockCorner blockCorner2 in list)
                {
                    if (blockCorner1 != blockCorner2 && !blockCorner1.testedCorners.Contains(blockCorner2) && !blockCorner2.testedCorners.Contains(blockCorner1))
                    {
                        blockCorner1.testedCorners.Add(blockCorner2);
                        blockCorner2.testedCorners.Add(blockCorner1);
                        if (blockCorner1.corner.x == blockCorner2.corner.x)
                        {
                            int num = 8;
                            if (blockCorner1.corner.y > blockCorner2.corner.y)
                                num = -8;
                            if (Level.CheckPoint<AutoBlock>(new Vec2(blockCorner1.corner.x + 8f, blockCorner1.corner.y + num)) != null && Level.CheckPoint<AutoBlock>(new Vec2(blockCorner1.corner.x - 8f, blockCorner1.corner.y + num)) != null && Level.CheckPoint<AutoBlock>(new Vec2(blockCorner2.corner.x + 8f, blockCorner2.corner.y - num)) != null && Level.CheckPoint<AutoBlock>(new Vec2(blockCorner2.corner.x - 8f, blockCorner2.corner.y - num)) != null)
                                concaveLineList2.Add(new ConcaveLine(blockCorner1.corner, blockCorner2.corner));
                        }
                        else if (blockCorner1.corner.y == blockCorner2.corner.y)
                        {
                            int num = 8;
                            if (blockCorner1.corner.x > blockCorner2.corner.x)
                                num = -8;
                            if (Level.CheckPoint<AutoBlock>(new Vec2(blockCorner1.corner.x + num, blockCorner1.corner.y - 8f)) != null && Level.CheckPoint<AutoBlock>(new Vec2(blockCorner1.corner.x + num, blockCorner1.corner.y + 8f)) != null && Level.CheckPoint<AutoBlock>(new Vec2(blockCorner2.corner.x - num, blockCorner2.corner.y - 8f)) != null && Level.CheckPoint<AutoBlock>(new Vec2(blockCorner2.corner.x - num, blockCorner2.corner.y + 8f)) != null)
                                concaveLineList1.Add(new ConcaveLine(blockCorner1.corner, blockCorner2.corner));
                        }
                    }
                }
            }
            foreach (ConcaveLine concaveLine1 in concaveLineList1)
            {
                foreach (ConcaveLine concaveLine2 in concaveLineList2)
                {
                    if (Collision.LineIntersect(concaveLine1.p1, concaveLine1.p2, concaveLine2.p1, concaveLine2.p2))
                    {
                        concaveLine1.intersects.Add(concaveLine2);
                        concaveLine2.intersects.Add(concaveLine1);
                    }
                }
            }
            if (concaveLineList2.Count == 4 && concaveLineList1.Count == 4)
            {
                ConcaveLine concaveLine3 = concaveLineList2[2];
                concaveLineList2[2] = concaveLineList2[3];
                concaveLineList2[3] = concaveLine3;
                ConcaveLine concaveLine4 = concaveLineList1[0];
                concaveLineList1[0] = concaveLineList1[1];
                concaveLineList1[1] = concaveLine4;
                ConcaveLine concaveLine5 = concaveLineList1[1];
                concaveLineList1[1] = concaveLineList1[2];
                concaveLineList1[2] = concaveLine5;
                _concaveLines.Add(concaveLineList2[0]);
                _concaveLines.Add(concaveLineList2[1]);
                _concaveLines.Add(concaveLineList1[1]);
                _concaveLines.Add(concaveLineList1[0]);
                _concaveLines.Add(concaveLineList1[2]);
                _concaveLines.Add(concaveLineList2[2]);
                _concaveLines.Add(concaveLineList2[3]);
                _concaveLines.Add(concaveLineList1[3]);
                int num = 1;
                foreach (ConcaveLine concaveLine6 in _concaveLines)
                {
                    concaveLine6.index = num;
                    ++num;
                }
            }
            else
            {
                _concaveLines.AddRange(concaveLineList2);
                _concaveLines.AddRange(concaveLineList1);
            }
            Calculate(concaveLineList2, concaveLineList1);
        }

        public List<ConcaveLine> GetConcaveLines()
        {
            if (_concaveLines == null)
                CalculateConcaveLines();
            return _concaveLines;
        }

        public virtual List<BlockCorner> GetGroupCorners()
        {
            if (_structure != null)
                return _structure.corners;
            _structure = new BlockStructure();
            Stack<Block> blockStack = new Stack<Block>();
            blockStack.Push(this);
            _hit = true;
            while (blockStack.Count > 0)
            {
                Block b = blockStack.Pop();
                b._structure = _structure;
                _structure.blocks.Add(b);
                if (b.leftBlock == null)
                {
                    if (b.upBlock == null)
                        _structure.corners.Add(new BlockCorner(b.topLeft, b));
                    if (b.downBlock == null)
                        _structure.corners.Add(new BlockCorner(b.bottomLeft, b));
                }
                else if (!b.leftBlock._hit)
                {
                    b.leftBlock._hit = true;
                    blockStack.Push(b.leftBlock);
                }
                if (b.rightBlock == null)
                {
                    if (b.upBlock == null)
                        _structure.corners.Add(new BlockCorner(b.topRight, b));
                    if (b.downBlock == null)
                        _structure.corners.Add(new BlockCorner(b.bottomRight, b));
                }
                else if (!b.rightBlock._hit)
                {
                    b.rightBlock._hit = true;
                    blockStack.Push(b.rightBlock);
                }
                if (b.upBlock != null && !b.upBlock._hit)
                {
                    b.upBlock._hit = true;
                    blockStack.Push(b.upBlock);
                }
                if (b.downBlock != null && !b.downBlock._hit)
                {
                    b.downBlock._hit = true;
                    blockStack.Push(b.downBlock);
                }
                if (b.upBlock != null && b.leftBlock != null && b.upBlock.leftBlock == null)
                    _structure.corners.Add(new BlockCorner(b.upBlock.bottomLeft, b, true));
                if (b.upBlock != null && b.rightBlock != null && b.upBlock.rightBlock == null)
                    _structure.corners.Add(new BlockCorner(b.upBlock.bottomRight, b, true));
                if (b.downBlock != null && b.leftBlock != null && b.downBlock.leftBlock == null)
                    _structure.corners.Add(new BlockCorner(b.downBlock.topLeft, b, true));
                if (b.downBlock != null && b.rightBlock != null && b.downBlock.rightBlock == null)
                    _structure.corners.Add(new BlockCorner(b.downBlock.topRight, b, true));
            }
            return _structure.corners;
        }

        public Block(float x, float y)
          : base(x, y)
        {
            collisionSize = new Vec2(16f, 16f);
            thickness = 10f;
        }

        public Block(float x, float y, float wid, float hi, PhysicsMaterial mat = PhysicsMaterial.Default)
          : base(x, y)
        {
            collisionSize = new Vec2(wid, hi);
            thickness = 10f;
            physicsMaterial = mat;
        }

        public override void Update()
        {
            InitializeNeighbors();
            _hit = false;
        }

        public override void DoInitialize()
        {
            base.DoInitialize();
        }

        public virtual void InitializeNeighbors()
        {
        }

        public override void Draw() => base.Draw();
    }
}
