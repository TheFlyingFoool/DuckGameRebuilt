// Decompiled with JetBrains decompiler
// Type: DuckGame.VirtualShotgun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Shotguns")]
    [BaggedProperty("isSuperWeapon", true)]
    public class VirtualShotgun : Shotgun
    {
        public StateBinding _roomIndexBinding = new StateBinding(nameof(roomIndex), 4);
        private byte _roomIndex;

        public byte roomIndex
        {
            get => this._roomIndex;
            set
            {
                this._roomIndex = value;
                if (!Network.isClient || !Network.InLobby() || this._roomIndex >= 4)
                    return;
                (Level.current as TeamSelect2).GetBox(this._roomIndex).gun = this;
            }
        }

        public VirtualShotgun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 99;
            this.graphic = new Sprite("virtualShotgun");
            this._loaderSprite = new SpriteMap("virtualShotgunLoader", 8, 8)
            {
                center = new Vec2(4f, 4f)
            };
            this.editorTooltip = "The perfect shotgun for life inside a computer simulation. Virtually infinite ammo.";
        }

        public override void Update()
        {
            this.ammo = 99;
            base.Update();
        }
    }
}
