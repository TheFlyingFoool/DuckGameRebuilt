using DuckGame;
using System;
using System.Collections.Generic;
using System.Windows.Automation.Peers;
namespace AddedContent.Biverom.TreeSawing
{
    public class FallingTree : Thing
    {
        public FallingTree(float xpos, float ypos, List<Thing> treeList, bool flip, bool explode)
            : base(xpos, ypos)
        {
            sw = new SinWave(this, Rando.Float(0.05f, 0.15f), Rando.Float(-5, 5));

            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);

            _treeList = treeList;
            _flip = flip;

            rotateSpeed = explode ? 0.02f : 0.002f;

            posList = new List<Vec2>();
            for (int i = 0; i < treeList.Count; i++)
            {
                Thing tree = treeList[i];
                Vec2 vec = tree.position - position;
                posList.Add(vec);
                massCenter = Vec2.Lerp(massCenter, vec, 1f / (i+1f));
            }

            if (explode)
            {
                freeFall = true;
                this.vSpeed = -3f;
                fallPos = position + massCenter;
            }
            else
            {
                fallPos = position - massCenter;
            }
        }

        public List<Thing> _treeList;
        public List<Vec2> posList;
        public bool freeFall = false;
        public bool impacted = false;
        public bool _flip;
        public float fallAngle = 0;
        public float rotateSpeed = 0.002f;
        public float opacity = 5f;
        public Vec2 massCenter = Vec2.Zero;
        public Vec2 fallPos;
        public float actualOpacity => MathHelper.Clamp(opacity, 0f, 1f);
        public SinWave sw;

        public override void Update()
        {
            base.Update();
            if (freeFall)
            {
                this.vSpeed += 0.1f;
                fallPos.y += vSpeed;
                fallAngle += rotateSpeed;
                this.angle = fallAngle * (_flip ? -1 : 1);

                this.position = fallPos + massCenter.Rotate(this.angle + (float)Math.PI, Vec2.Zero);
            }
            else
            {
                fallAngle += rotateSpeed;
                if (fallAngle > Math.PI)
                {
                    PlaySFX("treeFall");
                    freeFall = true;
                    fallAngle = (float)Math.PI;
                    rotateSpeed *= 0.5f;
                }
                this.angle = fallAngle * (_flip ? -1 : 1);
                rotateSpeed += (float)Math.Sin(fallAngle) * 0.001f;
                if (rotateSpeed > 0)
                {
                    foreach (Thing tree in _treeList)
                    {
                        Vec2 pos = this.position + posList[_treeList.IndexOf(tree)].Rotate(this.angle, Vec2.Zero);
                        if (Level.CheckPoint<Block>(pos.x, pos.y) != null)
                        {
                            rotateSpeed = -rotateSpeed * 0.25f;
                            if (!impacted)
                            {
                                impacted = true;
                                SFX.Play("treeImpact");
                            }
                        }
                        if (!(tree is AutoPlatform))
                            tree.Update();
                    }
                }
            }
            opacity -= 0.02f;
            if (opacity <= 0f)
                Level.Remove(this);
        }

        public override void Draw()
        {
            foreach (Thing tree in _treeList)
            {
                Vec2 pos = this.position + posList[_treeList.IndexOf(tree)].Rotate(this.angle, Vec2.Zero);
                if (tree is AutoPlatform platform)
                {
                    Sprite s = platform.graphic;
                    s.position = pos;
                    s.angle = this.angle;
                    s.alpha = actualOpacity;
                    s.scale = Vec2.One * 1.02f;
                    s.Draw();
                }
                else
                {
                    tree.position = pos;
                    tree.angle = this.angle + (tree is TreeEnd ? 0 : (sw * 20f * rotateSpeed));
                    tree.alpha = actualOpacity;
                    tree.Draw();
                }
            }
        }
    }
}
