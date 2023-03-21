// Decompiled with JetBrains decompiler
// Type: DuckGame.ItemBoxOneTime
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Spawns")]
    [BaggedProperty("isInDemo", false)]
    [BaggedProperty("previewPriority", false)]
    public class ItemBoxOneTime : ItemBox
    {
        public ItemBoxOneTime(float xpos, float ypos)
          : base(xpos, ypos)
        {
            editorTooltip = "Spawns the contained item one time when it's used.";
            editorCycleType = typeof(ItemBoxRandom);
        }

        public override void UpdateCharging() => charging = 500;

        public override void Draw()
        {
            _sprite.frame += 4;
            base.Draw();
            _sprite.frame -= 4;
        }
    }
}
