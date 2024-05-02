using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public static class AI
    {
        public static void InitializeLevelPaths()
        {
            foreach (PathNode pathNode in Level.current.things[typeof(PathNode)])
            {
                pathNode.UninitializeLinks();
                pathNode.InitializeLinks();
            }
            foreach (PathNode pathNode in Level.current.things[typeof(PathNode)])
                pathNode.InitializePaths();
        }

        private static Thing FilterBlocker(Thing b)
        {
            switch (b)
            {
                case Door _:
                case Window _:
                    return null;
                default:
                    return b;
            }
        }

        public static T Nearest<T>(Vec2 position, Thing ignore = null)
        {
            PathNode pathNode = NearestNode(position);
            if (pathNode == null)
                return default(T);
            System.Type key = typeof(T);
            float num = 99999.9f;
            T obj = default(T);
            foreach (Thing thing in Level.current.things[key])
            {
                if (thing != ignore)
                {
                    PathNode to = NearestNode(thing.position);
                    if (to != null)
                    {
                        AIPath path = pathNode.GetPath(to);
                        if (path != null && path.length < num)
                        {
                            num = path.length;
                            obj = (T)(object)thing;
                        }
                    }
                }
            }
            return obj;
        }

        public static bool CanReach(PathNode from, Thing what) => PathNode.CanTraverse(from.position, what.position, what);

        public static Thing Nearest(Vec2 position, List<Thing> things)
        {
            PathNode pathNode = NearestNode(position);
            if (pathNode == null)
                return null;
            float num = 99999.9f;
            Thing thing1 = null;
            foreach (Thing thing2 in things)
            {
                PathNode to = NearestNode(thing2.position, thing2);
                if (to != null)
                {
                    AIPath path = pathNode.GetPath(to);
                    if (path != null && path.nodes.Count > 0 && path.length < num && CanReach(path.nodes.Last(), thing2))
                    {
                        num = path.length;
                        thing1 = thing2;
                    }
                }
            }
            return thing1;
        }

        public static PathNode NearestNode(Vec2 pos, Thing ignore = null)
        {
            List<Thing> list = Level.current.things[typeof(PathNode)].ToList();
            list.Sort((a, b) =>
           {
               Vec2 vec2 = a.position - pos;
               double lengthSq1 = vec2.lengthSq;
               vec2 = b.position - pos;
               double lengthSq2 = vec2.lengthSq;
               return lengthSq1 >= lengthSq2 ? 1 : -1;
           });
            PathNode pathNode = null;
            foreach (Thing thing in list)
            {
                if (PathNode.LineIsClear(pos, thing.position, ignore))
                {
                    pathNode = thing as PathNode;
                    break;
                }
            }
            return pathNode;
        }

        private static Thing GetHighest(List<IPlatform> things)
        {
            Thing highest = null;
            foreach (IPlatform thing1 in things)
            {
                if (!(thing1 is PhysicsObject))
                {
                    Thing thing2 = thing1 as Thing;
                    if (highest == null || thing2.y < highest.y)
                        highest = thing2;
                }
            }
            return highest;
        }

        private static Thing GetHighestNotGlass(List<IPlatform> things)
        {
            Thing highestNotGlass = null;
            foreach (IPlatform thing1 in things)
            {
                switch (thing1)
                {
                    case PhysicsObject _:
                    case Window _:
                        continue;
                    default:
                        Thing thing2 = thing1 as Thing;
                        if (highestNotGlass == null || thing2.y < highestNotGlass.y)
                        {
                            highestNotGlass = thing2;
                            continue;
                        }
                        continue;
                }
            }
            return highestNotGlass;
        }

        public static List<PathNode> GetPath(PathNode start, PathNode end)
        {
            List<PathNode> path = new List<PathNode>();
            List<PathNode> pathNodeList1 = new List<PathNode>();
            foreach (PathNode pathNode in Level.current.things[typeof(PathNode)])
            {
                pathNode.Reset();
                pathNodeList1.Add(pathNode);
            }
            List<PathNode> pathNodeList2 = new List<PathNode>();
            List<PathNode> pathNodeList3 = new List<PathNode>();
            pathNodeList2.Add(start);
            PathNode.CalculateNode(start, start, end);
            while (pathNodeList2.Count != 0)
            {
                PathNode parent = null;
                foreach (PathNode pathNode in pathNodeList2)
                {
                    if (parent == null)
                        parent = pathNode;
                    else if (pathNode.cost + pathNode.heuristic < parent.cost + parent.heuristic)
                        parent = pathNode;
                }
                if (parent != null)
                {
                    if (parent == end)
                    {
                        PathNode pathNode1 = parent;
                        path.Clear();
                        for (; pathNode1 != null; pathNode1 = pathNode1.parent)
                            path.Add(pathNode1);
                        foreach (PathNode pathNode2 in pathNodeList1)
                            pathNode2.Reset();
                        path.Reverse();
                        return path;
                    }
                    pathNodeList3.Add(parent);
                    foreach (PathNodeLink link1 in parent.links)
                    {
                        if (link1.link is PathNode link2 && !pathNodeList3.Contains(link2))
                        {
                            if (!pathNodeList2.Contains(link2))
                            {
                                link2.parent = parent;
                                PathNode.CalculateNode(link2, parent, end);
                                pathNodeList2.Add(link2);
                            }
                            else
                            {
                                float cost = PathNode.CalculateCost(link2, parent);
                                if (cost < link2.cost)
                                {
                                    link2.cost = cost;
                                    link2.parent = parent;
                                }
                            }
                        }
                    }
                    pathNodeList2.Remove(parent);
                }
            }
            return path;
        }
    }
}
