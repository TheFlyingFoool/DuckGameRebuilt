// Decompiled with JetBrains decompiler
// Type: DuckGame.MirrorMode
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Arcade", EditorItemType.ArcadeNew)]
    [BaggedProperty("previewPriority", true)]
    internal class MirrorMode : Thing
    {
        private SpriteMap _sprite;
        public EditorProperty<MirrorMode.Setting> mode;

        public override void EditorPropertyChanged(object property)
        {
            _sprite.frame = (int)mode.value;
            base.EditorPropertyChanged(property);
        }

        public MirrorMode(float pX, float pY)
          : base()
        {
            _sprite = new SpriteMap("mirrorMode", 16, 16);
            graphic = _sprite;
            mode = new EditorProperty<MirrorMode.Setting>(MirrorMode.Setting.Vertical, this, 0f, 1f, 0.1f, null, false, false);
            mode.value = MirrorMode.Setting.Vertical;
            collisionSize = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -4f);
            center = new Vec2(8f, 8f);
            editorOffset = new Vec2(8f, 8f);
            _visibleInGame = false;
        }

        public enum Setting
        {
            Horizontal,
            Vertical,
            Both,
        }
    }
}
