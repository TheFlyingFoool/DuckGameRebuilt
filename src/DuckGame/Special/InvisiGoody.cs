// Decompiled with JetBrains decompiler
// Type: DuckGame.InvisiGoody
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special|Goodies", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class InvisiGoody : Goody
    {
        public EditorProperty<bool> valid;
        public EditorProperty<bool> sound;
        public EditorProperty<int> size;

        public override void EditorPropertyChanged(object property)
        {
            this.UpdateHeight();
            this.sequence.isValid = this.valid.value;
            if (this.sound.value)
                this.collectSound = "goody";
            else
                this.collectSound = "";
        }

        public void UpdateHeight()
        {
            float num = size.value;
            this.center = new Vec2(8f, 8f);
            this.collisionSize = new Vec2(num * 16f);
            this.collisionOffset = new Vec2((float)(-((double)num * 16.0) / 2.0));
            this.scale = new Vec2(num);
        }

        public InvisiGoody(float xpos, float ypos)
          : base(xpos, ypos, new Sprite("swirl"))
        {
            this._visibleInGame = false;
            this.sequence.isValid = false;
            this.size = new EditorProperty<int>(1, this, 1f, 16f, 1f);
            this.valid = new EditorProperty<bool>(false, this);
            this.sound = new EditorProperty<bool>(false, this);
        }
    }
}
