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
            this._contextMenuFilter.Add("overFollow");
            this._contextMenuFilter.Add("allowWarps");
            this._editorName = "Camera Zoom";
            this.editorTooltip = "A camera that follows your Ducks with an adjustable zoom.";
            this.Zoom._tooltip = "The Zoom factor (Counter-intuitively, Higher values zoom out further!)";
            this.Allow_Warps._tooltip = "If enabled, the camera will instantly move to follow Faster-Than-Light ducks";
            this.Overfollow._tooltip = "If enabled, the camera will keep a tighter focus on fast moving Ducks (think Chainsaw challenges!)";
            this.graphic = new Sprite("cameraIcon");
        }

        public override void Initialize()
        {
            this.zoomMult = this.Zoom.value;
            this.overFollow = (EditorProperty<bool>)this.Overfollow.value;
            this.allowWarps = (EditorProperty<bool>)this.Allow_Warps.value;
            base.Initialize();
        }
    }
}
