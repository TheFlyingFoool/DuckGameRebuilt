// Decompiled with JetBrains decompiler
// Type: DuckGame.CameraBoundsNew
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Arcade|Cameras", EditorItemType.ArcadeNew)]
    public class CameraBoundsNew : CameraBounds
    {
        public EditorProperty<int> Wide = new EditorProperty<int>(320, min: 60f, max: 1920f, increment: 1f);
        public EditorProperty<int> High = new EditorProperty<int>(320, min: 60f, max: 1920f, increment: 1f);

        public CameraBoundsNew()
        {
            this._contextMenuFilter.Add("wide");
            this._contextMenuFilter.Add("high");
            this._editorName = "Camera Bounds";
            this.editorTooltip = "A boundary that keeps moving cameras locked within it!";
            this.Wide._tooltip = "Width of the boundary area (in Pixels)";
            this.High._tooltip = "Height of the boundary area (in Pixels)";
        }

        public override void Initialize()
        {
            this.wide = (EditorProperty<int>)this.Wide.value;
            this.high = (EditorProperty<int>)this.High.value;
            base.Initialize();
        }

        public override void Draw()
        {
            this.wide = (EditorProperty<int>)this.Wide.value;
            this.high = (EditorProperty<int>)this.High.value;
            base.Draw();
        }
    }
}
