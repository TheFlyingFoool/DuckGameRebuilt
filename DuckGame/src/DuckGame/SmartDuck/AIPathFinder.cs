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
            get => _followObject;
            set => _followObject = value;
        }

        public AIPathFinder(Thing t = null) => _followObject = t;

        public List<PathNodeLink> path
        {
            get
            {
                if (_path == null)
                    return null;
                return _path.Count <= 0 ? null : _path;
            }
        }

        public PathNodeLink target => _path == null || _path.Count == 0 ? null : _path[0];

        public PathNodeLink peek
        {
            get
            {
                if (_path == null || _path.Count == 0)
                    return null;
                return _path.Count > 1 ? _path[1] : _path[0];
            }
        }

        public bool finished => _path == null || _path.Count == 0;

        public void AtTarget()
        {
            if (_path == null || _path.Count <= 0)
                return;
            _revert = _path[0];
            _path.RemoveAt(0);
        }

        public void Revert()
        {
            if (_path == null || _revert == null)
                return;
            _path.Insert(0, _revert);
        }

        public void Refresh()
        {
            if (_path == null)
                return;
            SetTarget(_path.Last());
        }

        public void SetTarget(Vec2 target)
        {
            if (_followObject != null)
                SetTarget(_followObject.position, target);
            else
                SetTarget(target, target);
        }

        public void SetTarget(PathNodeLink target)
        {
            if (_followObject == null)
                return;
            SetTarget(_followObject.position, target.owner.position);
        }

        public void SetTarget(Vec2 position, Vec2 target)
        {
            _revert = null;
            _path = null;
            List<Thing> list = Level.current.things[typeof(PathNode)].ToList();
            list.Sort((a, b) =>
           {
               Vec2 vec2 = a.position - position;
               double lengthSq1 = vec2.lengthSq;
               vec2 = b.position - position;
               double lengthSq2 = vec2.lengthSq;
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
               double lengthSq3 = vec2.lengthSq;
               vec2 = b.position - target;
               double lengthSq4 = vec2.lengthSq;
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
            _path = new List<PathNodeLink>();
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
                    _path.Add(pathNodeLink);
                }
                else
                {
                    if (pathNode2 != null)
                        _path.Add(pathNode2.GetLink(node));
                    pathNode2 = node;
                }
            }
            Thing thing1 = null;
            _path.Add(new PathNodeLink()
            {
                owner = _path.Count <= 0 ? thing1 : _path.Last().link,
                link = thing1
            });
        }
    }
}
