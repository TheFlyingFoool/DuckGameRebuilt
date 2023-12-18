namespace DuckGame
{
    [ClientOnly]
    //[EditorGroup("Blocks")]
    public class TestingTileset : AutoBlock
    {
        // Token: 0x06001973 RID: 6515 RVA: 0x00113388 File Offset: 0x00111588
        public TestingTileset(float x, float y) : base(x, y, "testTileset")
        {
            thickness = float.PositiveInfinity;
            _editorName = "devblock";
            physicsMaterial = PhysicsMaterial.Metal;
            verticalWidth = 16f;
            verticalWidthThick = 16f;
            horizontalHeight = 16f;
            _hasNubs = false;
        }
        public override void Update()
        {
            shouldWreck = false;
            base.Update();
        }
        public override void Removed()
        {
            if (Level.current is not Editor)
            {
                Level.Add(this);
                return;
            }
            base.Removed();
        }
    }
}