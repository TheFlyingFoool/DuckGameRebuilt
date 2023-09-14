using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame
{
    [ClientOnly]
    public class DodgeblockCurse : Thing
    {
        public StateBinding _fallBinding = new StateBinding("bFall");
        public StateBinding _positionBinding = new StateBinding("position");
        public DodgeblockCurse(float xpos, float ypos) : base(xpos, ypos)
        {
            collisionSize = new Vec2(96, 16);
            _collisionOffset = new Vec2(-48, -8);
        }
        public Duck duck;
        public override void Draw()
        {
            Graphics.DrawRect(topLeft - new Vec2(0, 288), bottomRight + new Vec2(0, BlockH * BoardHeight), Color.Black * 0.4f, -1);

            for (int i = 0; i < falling.Count; i++)
            {
                DodgeBlock block = falling[i];
                Vec2 v = new Vec2(block.x * BlockW, block.y * BlockH);

                if (block.special)
                {
                    Graphics.DrawString("^", position + v + new Vec2(0.4f, 8 + (BlockH / 2f) - 5.9f), Color.Black, 1.1f, null, 1.68f); //calculated alignment -othello7
                    Graphics.DrawRect(position + v + new Vec2(0, 8), position + new Vec2(BlockW, BlockH + 8) + v, Color.Yellow, 1);
                }
                else
                {
                    Graphics.DrawString("%", position + v + new Vec2(2, 8 + (BlockH / 2f) - 5.9f), Color.Black, 1.1f, null, 1.68f); //calculated pixel-perfect scale alignment -othello7
                    Graphics.DrawRect(position + v + new Vec2(0, 8), position + new Vec2(BlockW, BlockH + 8) + v, Color.White, 1);
                }
            }
        }

        private bool rewarded = false;
        public int timer;
        public BitBuffer bFall = new BitBuffer();
        public List<DodgeBlock> falling = new List<DodgeBlock>();
        private const int BlockH = 24;
        private const int BlockW = 16;
        private const int BoardHeight = 18;

        public override void Update()
        {
            if (isServerForObject)
            {
                bool inc = timer % 20 == 0;
                for (int i = 0; i < falling.Count; i++)
                {
                    DodgeBlock fall = falling[i];
                    if (inc) fall.y++;
                    if (fall.y == BoardHeight)
                    {
                        falling.RemoveAt(i);
                    }
                }

                if (timer == 1000) falling.Add(new DodgeBlock() { x = Rando.Int(-3, 2), y = -18, special = true });
                else if (timer % 20 == 0) falling.Add(new DodgeBlock() { x = Rando.Int(-3, 2), y = -18, special = false });

                if (duck != null)
                {
                    if (duck.ragdoll == null && duck._trapped == null)
                    {
                        if (duck.left < left) left = duck.left;
                        if (duck.right > right) right = duck.right;
                    }
                    else
                    {
                        Vec2 v = duck.position;
                        if (duck.ragdoll != null) v = duck.ragdoll.position;
                        else if (duck._trapped != null) v = duck._trapped.position;
                        if (v.x < left) x -= 16;
                        if (v.x > right) x += 16;
                    }
                }

                timer++;

                bFall = new BitBuffer();
                bFall.Write((ushort)falling.Count);
                for (int i = 0; i < falling.Count; i++)
                {
                    bFall.Write((byte)(falling[i].x + 3));
                    bFall.Write((byte)(falling[i].y + 18));
                }

                if (timer > 1440 || (duck != null && duck.dead)) Level.Remove(this);
            }
            else
            {
                if (falling != null && bFall != null)
                {
                    try
                    {
                        ushort count = bFall.ReadUShort();
                        if (count > falling.Count)
                        {
                            for (int i = 0; i < count - falling.Count; i++)
                            {
                                falling.Add(new DodgeBlock() { x = 0, y = -18 });
                            }
                        }
                        else if (count < falling.Count) falling.RemoveAt(0);
                        for (int i = 0; i < count; i++)
                        {
                            DodgeBlock dg = falling[i];
                            byte xVal = bFall.ReadByte();
                            byte yVal = bFall.ReadByte();
                            dg.x = xVal - 3;
                            dg.y = yVal - 18;
                        }
                    }
                    catch
                    {
                        //jank
                    }
                }
            }

            if (isServerForObject)
            {
                for (int i = 0; i < falling.Count; i++)
                {
                    DodgeBlock block = falling[i];
                    Vec2 v = new Vec2(block.x * BlockW, block.y * BlockH);

                    IEnumerable<IAmADuck> iaads = Level.CheckRectAll<IAmADuck>(position + v + new Vec2(0, (BlockH - 1) + 8), position + new Vec2(BlockW, BlockH + 8) + v);
                    foreach (IAmADuck iaad in iaads)
                    {
                        MaterialThing mt = (MaterialThing)iaad;
                        Duck d = Duck.GetAssociatedDuck(mt);
                        if (d != null && d == duck && !d.dead)
                        {
                            if (block.special)
                            {
                                if (!rewarded)
                                    for (int j = 0; j < 3; j++)
                                    {
                                        Thing reward = ItemBoxRandom.GetRandomItem();
                                        reward.position = d.position;
                                        Level.Add(reward);
                                    }
                                rewarded = true;

                            }
                            else
                            {
                                d.MakeStars();
                                d.Kill(new DTCrush(null));
                            }
                        }
                    }
                }
            }
        }
    }
    public class DodgeBlock
    {
        public int x;
        public int y;
        public bool special; //for funny ending powerup -othello7
    }
}
