// Decompiled with JetBrains decompiler
// Type: DuckGame.CameraZoomNew
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Arcade|Cameras", EditorItemType.ArcadeNew)]
    public class CameraZoomNew : CameraZoom
    {
        public EditorProperty<float> Zoom = new EditorProperty<float>(1f, min: 0.25f, max: 5f);
        public EditorProperty<bool> Overfollow = new EditorProperty<bool>(false);
        public EditorProperty<bool> Allow_Warps = new EditorProperty<bool>(false);

        public CameraZoomNew()
        {
            _contextMenuFilter.Add("overFollow");
            _contextMenuFilter.Add("allowWarps");
            _editorName = "Camera Zoom";
            editorTooltip = "A camera that follows your Ducks with an adjustable zoom.";
            Zoom._tooltip = "The Zoom factor (Counter-intuitively, Higher values zoom out further!)";
            Allow_Warps._tooltip = "If enabled, the camera will instantly move to follow Faster-Than-Light ducks";
            Overfollow._tooltip = "If enabled, the camera will keep a tighter focus on fast moving Ducks (think Chainsaw challenges!)";
            graphic = new Sprite("cameraIcon");
        }

        public override void Initialize()
        {
            zoomMult = Zoom.value;
            overFollow = (EditorProperty<bool>)Overfollow.value;
            allowWarps = (EditorProperty<bool>)Allow_Warps.value;
            base.Initialize();
        }
    }
}
