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
