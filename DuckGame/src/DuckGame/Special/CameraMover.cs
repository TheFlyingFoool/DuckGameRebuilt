// Decompiled with JetBrains decompiler
// Type: DuckGame.CameraMover
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Arcade|Cameras", EditorItemType.ArcadeNew)]
    [BaggedProperty("previewPriority", true)]
    internal class CameraMover : Thing
    {
        public EditorProperty<float> SpeedX = new EditorProperty<float>(0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> SpeedY = new EditorProperty<float>(0f, min: -10f, max: 10f, increment: 0.25f);
        public EditorProperty<float> MoveDelay = new EditorProperty<float>(0f, max: 120f, increment: 0.25f);

        public CameraMover(float xPos, float yPos)
          : base(xPos, yPos)
        {
            graphic = new Sprite("cameraMover");
            center = new Vec2(8f, 8f);
            collisionSize = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -4f);
        }

        public override void Initialize()
        {
            if (!(Level.current is Editor))
                alpha = 0f;
            base.Initialize();
        }
    }
}
