namespace DuckGame
{
    [EditorGroup("Arcade|Cameras", EditorItemType.ArcadeNew)]
    public class CameraBoundsNew : CameraBounds
    {
        public EditorProperty<int> Wide = new EditorProperty<int>(320, min: 60f, max: 1920f, increment: 1f);
        public EditorProperty<int> High = new EditorProperty<int>(320, min: 60f, max: 1920f, increment: 1f);

        public CameraBoundsNew()
        {
            _contextMenuFilter.Add("wide");
            _contextMenuFilter.Add("high");
            _editorName = "Camera Bounds";
            editorTooltip = "A boundary that keeps moving cameras locked within it!";
            Wide._tooltip = "Width of the boundary area (in Pixels)";
            High._tooltip = "Height of the boundary area (in Pixels)";
            editorCycleType = typeof(CameraFixed);
        }

        public override void Initialize()
        {
            wide = (EditorProperty<int>)Wide.value;
            high = (EditorProperty<int>)High.value;
            base.Initialize();
        }

        public override void Draw()
        {
            wide = (EditorProperty<int>)Wide.value;
            high = (EditorProperty<int>)High.value;
            base.Draw();
        }
    }
}
