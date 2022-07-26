// Decompiled with JetBrains decompiler
// Type: DuckGame.PathNode
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class PathNode : Thing, IPathNodeBlocker
    {
        private Thing _thing;
        private new bool _initialized;
        public float cost;
        public float heuristic;
        public bool fallLeft;
        public bool fallRight;
        private PathNode _parent;
        private List<PathNodeLink> _links = new List<PathNodeLink>();
        private Dictionary<PathNode, AIPath> _paths = new Dictionary<PathNode, AIPath>();
        public bool specialNode;
        private bool _fallInit;

        public Thing thing => this._thing;

        public PathNode parent
        {
            get => this._parent;
            set => this._parent = value;
        }

        public List<PathNodeLink> links => this._links;

        public PathNode(float xpos = 0.0f, float ypos = 0.0f, Thing t = null)
          : base(xpos, ypos)
        {
            this._thing = t;
            this.graphic = new Sprite("ball");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.scale = new Vec2(0.5f, 0.5f);
            this.editorOffset = new Vec2(0.0f, -8f);
        }

        public override void Update()
        {
            if (!this._fallInit)
            {
                this.InitializeLinks();
                this._fallInit = true;
            }
            else
            {
                if (this._initialized)
                    return;
                this.InitializePaths();
                this._initialized = true;
            }
        }

        public AIPath GetPath(PathNode to) => this._paths.ContainsKey(to) ? this._paths[to] : (AIPath)null;

        public PathNodeLink GetLink(PathNode with) => this._links.FirstOrDefault<PathNodeLink>((Func<PathNodeLink, bool>)(node => node.link == with));

        public void UninitializeLinks()
        {
            this._initialized = false;
            this._links.Clear();
        }

        public static bool LineIsClear(Vec2 from, Vec2 to, Thing ignore = null)
        {
            IEnumerable<IPathNodeBlocker> pathNodeBlockers = Level.current.CollisionLineAll<IPathNodeBlocker>(from, to);
            if ((double)to.y - (double)from.y < -64.0)
                return false;
            bool flag = false;
            foreach (IPathNodeBlocker pathNodeBlocker in pathNodeBlockers)
            {
                if (!(pathNodeBlocker is PathNode) && pathNodeBlocker != ignore)
                {
                    Thing thing = pathNodeBlocker as Thing;
                    switch (thing)
                    {
                        case IPlatform _:
                            if ((!(thing is AutoPlatform) || !(thing as AutoPlatform).HasNoCollision()) && !(thing is AutoPlatform) && (double)thing.y > (double)from.y && (double)to.y > (double)thing.y)
                            {
                                flag = true;
                                goto label_13;
                            }
                            else
                                continue;
                        default:
                            flag = true;
                            goto label_13;
                    }
                }
            }
        label_13:
            return !flag;
        }

        public void InitializePaths()
        {
            this._paths.Clear();
            foreach (PathNode pathNode1 in Level.current.things[typeof(PathNode)])
            {
                List<PathNode> path = AI.GetPath(this, pathNode1);
                if (path != null)
                {
                    float num = 0.0f;
                    Vec2 vec2 = Vec2.Zero;
                    foreach (PathNode pathNode2 in path)
                    {
                        if (vec2 != Vec2.Zero)
                            num += (pathNode2.position - vec2).length;
                        vec2 = pathNode2.position;
                    }
                    this._paths[pathNode1] = new AIPath()
                    {
                        length = num,
                        nodes = path
                    };
                }
                else
                    this._paths[pathNode1] = (AIPath)null;
            }
        }

        public static bool CheckTraversalLimits(Vec2 from, Vec2 to) => (double)from.y - (double)to.y <= 64.0 && (double)Math.Abs(from.x - to.x) <= 128.0 && ((double)from.y - (double)to.y <= 8.0 || (double)Math.Abs(from.x - to.x) <= 64.0);

        public static bool CanTraverse(Vec2 from, Vec2 to, Thing ignore) => PathNode.CheckTraversalLimits(from, to) && !PathNode.PathPhysicallyBlocked(from, to, ignore);

        public bool PathBlocked(PathNode to)
        {
            IEnumerable<IPathNodeBlocker> pathNodeBlockers = Level.current.CollisionLineAll<IPathNodeBlocker>(this.position, to.position);
            bool flag = false;
            foreach (IPathNodeBlocker pathNodeBlocker in pathNodeBlockers)
            {
                if (pathNodeBlocker != this && pathNodeBlocker != to)
                {
                    Thing thing = pathNodeBlocker as Thing;
                    switch (thing)
                    {
                        case IPlatform _:
                            if ((!(thing is AutoPlatform) || !(thing as AutoPlatform).HasNoCollision()) && (double)thing.y > (double)this.y)
                            {
                                flag = true;
                                goto label_11;
                            }
                            else
                                continue;
                        default:
                            flag = true;
                            goto label_11;
                    }
                }
            }
        label_11:
            return flag;
        }

        public static bool PathPhysicallyBlocked(Vec2 from, Vec2 to, Thing ignore = null)
        {
            IEnumerable<IPathNodeBlocker> pathNodeBlockers = Level.current.CollisionLineAll<IPathNodeBlocker>(from, to);
            bool flag = false;
            foreach (IPathNodeBlocker pathNodeBlocker in pathNodeBlockers)
            {
                if (!(pathNodeBlocker is PathNode) && pathNodeBlocker != ignore)
                {
                    Thing thing = pathNodeBlocker as Thing;
                    switch (thing)
                    {
                        case IPlatform _:
                            if ((!(thing is AutoPlatform) || !(thing as AutoPlatform).HasNoCollision()) && (double)thing.y > (double)from.y)
                            {
                                flag = true;
                                goto label_11;
                            }
                            else
                                continue;
                        default:
                            flag = true;
                            goto label_11;
                    }
                }
            }
        label_11:
            return flag;
        }

        public void InitializeLinks()
        {
            foreach (PathNode to in Level.current.things[typeof(PathNode)])
            {
                if (to != this && PathNode.CheckTraversalLimits(this.position, to.position) && !this.PathBlocked(to))
                {
                    PathNodeLink pathNodeLink = new PathNodeLink();
                    pathNodeLink.owner = (Thing)this;
                    pathNodeLink.link = (Thing)to;
                    pathNodeLink.distance = (to.position - this.position).length;
                    if ((double)Math.Abs(this.y - to.y) < 8.0)
                    {
                        Vec2 p1 = (this.position + to.position) / 2f;
                        if (Level.CheckLine<IPathNodeBlocker>(p1, p1 + new Vec2(0.0f, 18f)) == null)
                            pathNodeLink.gap = true;
                    }
                    PathNodeLink link = to.GetLink(this);
                    if (link != null)
                    {
                        link.oneWay = false;
                        pathNodeLink.oneWay = false;
                    }
                    else
                        pathNodeLink.oneWay = true;
                    this._links.Add(pathNodeLink);
                }
            }
        }

        public void Reset()
        {
            this.cost = 0.0f;
            this.heuristic = 0.0f;
            this._parent = (PathNode)null;
        }

        public static float CalculateCost(PathNode who, PathNode parent)
        {
            float num1 = who.x - parent.x;
            float num2 = who.y - parent.y;
            if ((double)parent.y > (double)who.y && (double)Math.Abs(num2) > 48.0)
                num2 *= 100f;
            float num3 = num2 * 2f;
            return parent.cost + (float)((double)num1 * (double)num1 + (double)num3 * (double)num3);
        }

        public static float CalculateHeuristic(PathNode who, PathNode end) => Math.Abs(who.x - end.x + (who.y - end.y));

        public static void CalculateNode(PathNode who, PathNode parent, PathNode end)
        {
            who.cost = PathNode.CalculateCost(who, parent);
            who.heuristic = PathNode.CalculateHeuristic(who, end);
        }

        public override void Draw()
        {
            foreach (PathNodeLink link in this._links)
            {
                Color color = Color.LimeGreen;
                if (link.oneWay)
                    color = Color.Red;
                if (link.gap)
                    color = Color.Blue;
                if (link.oneWay && link.gap)
                    color = Color.LightBlue;
                Graphics.DrawLine(this.position, link.link.position, color * 0.2f, depth: ((Depth)0.9f));
                Vec2 vec2_1 = link.link.position - this.position;
                float length = vec2_1.length;
                vec2_1 = link.link.position - this.position;
                Vec2 normalized = vec2_1.normalized;
                Vec2 vec2_2 = normalized;
                Vec2 vec2_3 = normalized;
                Vec2 vec2_4 = -vec2_2.Rotate(1f, Vec2.Zero);
                Vec2 vec2_5 = -vec2_3.Rotate(-1f, Vec2.Zero);
                Vec2 p1 = this.position + normalized * (length / 1.5f);
                Graphics.DrawLine(p1, p1 + vec2_4 * 4f, color * 0.2f, depth: ((Depth)0.9f));
                Graphics.DrawLine(p1, p1 + vec2_5 * 4f, color * 0.2f, depth: ((Depth)0.9f));
            }
            base.Draw();
        }
    }
}
