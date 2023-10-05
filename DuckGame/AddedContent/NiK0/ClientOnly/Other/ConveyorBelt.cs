using System.Linq;
using System.Collections.Generic;
using static DuckGame.PipeTileset;

namespace DuckGame
{
    [ClientOnly]
    [EditorGroup("Rebuilt|Stuff")]
    public class ConveyorBelt : Block
    {
        public EditorProperty<float> speed = new EditorProperty<float>(1, null, -10, 10, 0.1f);
        public ConveyorBelt(float xpos, float ypos) : base(xpos, ypos)
        {
            //0-6
            sprite = new SpriteMap("conveyorMid", 16, 16);
            sprite.AddAnimation("convey", 0.1f, true, 0, 1, 2, 3, 4, 5, 6);
            sprite.SetAnimation("convey");
            sprite.canMultiframeSkip = true;
            sprite.canRegress = true;
            graphic = sprite;
            center = new Vec2(8);

            _collisionOffset = new Vec2(-8, 1.5f);
            collisionSize = new Vec2(16, 6.5f);

            editorOffset = new Vec2(0, -8);
            depth = -1;

            _editorName = "Conveyor";
            editorTooltip = "Conveyors are ancient technology presumably used in industrial machinery. Nowadays we use teleporters";
        }
        protected virtual Dictionary<Direction, ConveyorBelt> GetNeighbors()
        {
            Dictionary<Direction, ConveyorBelt> neighbors = new Dictionary<Direction, ConveyorBelt>();
            ConveyorBelt pipeTileset3 = Level.CheckPointAll<ConveyorBelt>(x - 16f, y + 6).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset3 != null)
                neighbors[Direction.Left] = pipeTileset3;
            ConveyorBelt pipeTileset4 = Level.CheckPointAll<ConveyorBelt>(x + 16f, y + 6).Where(x => x.group == group).FirstOrDefault();
            if (pipeTileset4 != null)
                neighbors[Direction.Right] = pipeTileset4;
            return neighbors;
        }
        public bool dontExpand;
        public ConveyorBelt pRight;
        public ConveyorBelt pLeft;
        public Dictionary<Direction, ConveyorBelt> neighbors;
        public override void EditorAdded()
        {
            neighbors = GetNeighbors();

            if (pLeft != null)
            {
                if (!neighbors.ContainsKey(Direction.Left)) neighbors.Add(Direction.Left, null);
                neighbors[Direction.Left] = pLeft;
                pLeft = null;
            }
            if (pRight != null)
            {
                if (!neighbors.ContainsKey(Direction.Right)) neighbors.Add(Direction.Right, null);
                neighbors[Direction.Right] = pRight;
                pRight = null;
            }
            bool left = neighbors.ContainsKey(Direction.Left);
            bool right = neighbors.ContainsKey(Direction.Right);

            dontExpand = true;

            if (!left && !right || (left && right))
            {
                sprite = new SpriteMap("conveyorMid", 16, 16);
                sprite.AddAnimation("convey", 0.1f, true, 0, 1, 2, 3, 4, 5, 6);
                sprite.SetAnimation("convey");
                sprite.canMultiframeSkip = true;
                sprite.canRegress = true;
                graphic = sprite;
            }
            else if (left && !right)
            {
                sprite = new SpriteMap("conveyorRight", 16, 16);
                sprite.AddAnimation("convey", 0.1f, true, 0, 1, 2, 3, 4, 5, 6);
                sprite.SetAnimation("convey");
                sprite.canMultiframeSkip = true;
                sprite.canRegress = true;
                graphic = sprite;
            }
            else
            {
                sprite = new SpriteMap("conveyorLeft", 16, 16);
                sprite.AddAnimation("convey", 0.1f, true, 0, 1, 2, 3, 4, 5, 6);
                sprite.SetAnimation("convey");
                sprite.canMultiframeSkip = true;
                sprite.canRegress = true;
                graphic = sprite;
            }
            if (left)
            {
                ConveyorBelt l = neighbors[Direction.Left];
                if (l.speed.value == speed.value) l.speed = speed;
                else speed = l.speed;
                if (!l.dontExpand)
                {
                    l.pRight = this;
                    l.EditorAdded();
                }
            }
            if (right && !neighbors[Direction.Right].dontExpand)
            {
                ConveyorBelt r = neighbors[Direction.Right];
                if (r.speed.value == speed.value) r.speed = speed;
                else speed = r.speed;
                if (!r.dontExpand)
                {
                    r.pLeft = this;
                    r.EditorAdded();
                }
            }
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void EditorRemoved()
        {
            ConveyorRemoved = true;
            base.EditorRemoved();
        }
        public static bool ConveyorRemoved;
        public int tim;
        public override void EditorUpdate()
        {
            if (ConveyorRemoved)
            {
                tim++;
                if (tim > 1)
                {
                    ConveyorRemoved = false;
                    tim = 0;
                }
                else initialize = false;
            }
            else tim = 0;
            if (!initialize)
            {
                EditorAdded();
                initialize = true;
                dontExpand = false;
            }
            dontExpand = false;
            sprite.speed = speed.value;
            base.EditorUpdate();
        }
        public override void Draw()
        {
            base.Draw();
        }
        public SpriteMap sprite;
        public bool initialize;
        public bool grouped;
        public override void Update()
        {
            if (!initialize)
            {
                EditorAdded();
                initialize = true;
                dontExpand = false;

                if (!neighbors.ContainsKey(Direction.Left))
                {
                    ConveyorBelt cv = this;

                    while (cv != null)
                    {
                        if (cv.neighbors == null) cv.GetNeighbors();
                        if (cv.neighbors.ContainsKey(Direction.Right))
                        {
                            cv = cv.neighbors[Direction.Right];
                            cv.grouped = true;
                        }
                        else break;
                    }
                    collisionSize = new Vec2(cv.right - left, 6.5f);
                }
            }
            if (!grouped)
            {
                IEnumerable<PhysicsObject> pos = Level.CheckRectAll<PhysicsObject>(topLeft - new Vec2(0, 5), topRight);
                foreach (PhysicsObject po in pos)
                {
                    if (po.isServerForObject)
                    {
                        if ((po.hSpeed < speed.value && speed.value > 0) || (po.hSpeed > speed.value && speed.value < 0))
                        {
                            po.hSpeed += speed.value * 0.3f;

                        }
                    }
                }
            }
            

            sprite.speed = speed.value;
            base.Update();
        }
    }
}
