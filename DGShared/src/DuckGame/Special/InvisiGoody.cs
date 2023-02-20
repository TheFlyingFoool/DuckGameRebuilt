// Decompiled with JetBrains decompiler
// Type: DuckGame.InvisiGoody
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            UpdateHeight();
            sequence.isValid = valid.value;
            if (sound.value)
                collectSound = "goody";
            else
                collectSound = "";
        }

        public void UpdateHeight()
        {
            float num = size.value;
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(num * 16f);
            collisionOffset = new Vec2((float)(-(num * 16.0) / 2.0));
            scale = new Vec2(num);
        }

        public InvisiGoody(float xpos, float ypos)
          : base(xpos, ypos, new Sprite("swirl"))
        {
            _visibleInGame = false;
            sequence.isValid = false;
            size = new EditorProperty<int>(1, this, 1f, 16f, 1f);
            valid = new EditorProperty<bool>(false, this);
            sound = new EditorProperty<bool>(false, this);
        }
    }
}
