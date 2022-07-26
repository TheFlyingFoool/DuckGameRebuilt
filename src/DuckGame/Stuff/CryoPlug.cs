// Decompiled with JetBrains decompiler
// Type: DuckGame.CryoPlug
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("survival")]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    public class CryoPlug : Holdable
    {
        private SpriteMap _sprite;
        private Rope _rope;

        public CryoPlug(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("survival/cryoPlug", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(12f, 12f);
            this._collisionOffset = new Vec2(-6f, -6f);
            this._sprite.frame = 0;
            this.depth = (Depth)0.9f;
        }

        public void AttachTo(Thing t)
        {
            this._rope = new Rope(this.x, this.y, t, (Thing)this);
            Level.Add((Thing)this._rope);
        }

        public override void Terminate() => Level.Remove((Thing)this._rope);

        public override void Update()
        {
            if (this.owner == null && this._sprite.frame == 0)
            {
                foreach (PowerSocket powerSocket in Level.current.things[typeof(PowerSocket)])
                {
                    if ((double)(powerSocket.position - this.position).length < 8.0)
                    {
                        SFX.Play("equip");
                        this._sprite.frame = 1;
                        this._enablePhysics = false;
                        this.position = powerSocket.position;
                        this.depth = - 0.8f;
                        return;
                    }
                }
            }
            else if (this.owner != null && this._sprite.frame == 1)
            {
                this._sprite.frame = 0;
                this._enablePhysics = true;
                SFX.Play("equip");
            }
            base.Update();
        }
    }
}
