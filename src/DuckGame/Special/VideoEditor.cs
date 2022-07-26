// Decompiled with JetBrains decompiler
// Type: DuckGame.VideoEditor
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class VideoEditor
    {
        public static void Draw() => Graphics.DrawLine(new Vec2(32f, Layer.HUD.camera.height - 16f), new Vec2(Layer.HUD.camera.width - 32f, Layer.HUD.camera.height - 16f), Color.White, depth: ((Depth)1f));
    }
}
