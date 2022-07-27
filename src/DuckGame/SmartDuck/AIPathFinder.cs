// Decompiled with JetBrains decompiler
// Type: DuckGame.AIPathFinder
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class AIPathFinder
    {
        private Thing _followObject;
        private PathNodeLink _revert;
        private List<PathNodeLink> _path;

        public Thing followObject
        {
            get => this._followObject;
            set => this._followObject = value;
        }

        public AIPathFinder(Thing t = null) => this._followObject = t;

        public List<PathNodeLink> path
        {
            get
            {
                if (this._path == null)
                    return null;
                return this._path.Count <= 0 ? null : this._path;
            }
        }

        public PathNodeLink target => this._path == null || this._path.Count == 0 ? null : this._path[0];

        public PathNodeLink peek
        {
            get
            {
                if (this._path == null || this._path.Count == 0)
                    return null;
                return this._path.Count > 1 ? this._path[1] : this._path[0];
            }
        }

        public bool finished => this._path == null || this._path.Count == 0;

        public void AtTarget()
        {
            if (this._path == null || this._path.Count <= 0)
                return;
            this._revert = this._path[0];
            this._path.RemoveAt(0);
        }

        public void Revert()
        {
            if (this._path == null || this._revert == null)
                return;
            this._path.Insert(0, this._revert);
        }

        public void Refresh()
        {
            if (this._path == null)
                return;
            this.SetTarget(this._path.Last<PathNodeLink>());
        }

        public void SetTarget(Vec2 target)
        {
            if (this._followObject != null)
                this.SetTarget(this._followObject.position, target);
            else
                this.SetTarget(target, target);
        }

        public void SetTarget(PathNodeLink target)
        {
            if (this._followObject == null)
                return;
            this.SetTarget(this._followObject.position, target.owner.position);
        }

        public void SetTarget(Vec2 position, Vec2 target)
        {
            this._revert = null;
            this._path = null;
            List<Thing> list = Level.current.things[typeof(PathNode)].ToList<Thing>();
            list.Sort((a, b) =>
           {
               Vec2 vec2 = a.position - position;
               double lengthSq1 = (double)vec2.lengthSq;
               vec2 = b.position - position;
               double lengthSq2 = (double)vec2.lengthSq;
               return lengthSq1 >= lengthSq2 ? 1 : -1;
           });
            PathNode pathNode1 = null;
            foreach (Thing thing in list)
            {
                if (PathNode.LineIsClear(position, thing.position))
                {
                    pathNode1 = thing as PathNode;
                    break;
                }
            }
            if (pathNode1 == null)
                return;
            list.Sort((a, b) =>
           {
               Vec2 vec2 = a.position - target;
               double lengthSq3 = (double)vec2.lengthSq;
               vec2 = b.position - target;
               double lengthSq4 = (double)vec2.lengthSq;
               return lengthSq3 >= lengthSq4 ? 1 : -1;
           });
            PathNode to = null;
            foreach (Thing thing in list)
            {
                if (PathNode.LineIsClear(target, thing.position))
                {
                    to = thing as PathNode;
                    break;
                }
            }
            if (to == null)
                return;
            AIPath path = pathNode1.GetPath(to);
            if (path == null || path.nodes.Count <= 0)
                return;
            bool flag = false;
            if (path.nodes.Count > 1 && PathNode.LineIsClear(position, path.nodes[1].position))
                flag = true;
            this._path = new List<PathNodeLink>();
            PathNode pathNode2 = null;
            foreach (PathNode node in path.nodes)
            {
                if (!flag)
                {
                    Thing thing = null;
                    PathNodeLink pathNodeLink = new PathNodeLink
                    {
                        owner = thing,
                        link = node
                    };
                    pathNode2 = node;
                    flag = true;
                    this._path.Add(pathNodeLink);
                }
                else
                {
                    if (pathNode2 != null)
                        this._path.Add(pathNode2.GetLink(node));
                    pathNode2 = node;
                }
            }
            Thing thing1 = null;
            this._path.Add(new PathNodeLink()
            {
                owner = this._path.Count <= 0 ? thing1 : this._path.Last<PathNodeLink>().link,
                link = thing1
            });
        }
    }
}
