// Decompiled with JetBrains decompiler
// Type: DuckGame.GoodyNew
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Arcade|Targets", EditorItemType.ArcadeNew)]
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isOnlineCapable", false)]
    [BaggedProperty("previewPriority", true)]
    public class GoodyNew : Goody
    {
        public EditorProperty<int> Order = new EditorProperty<int>(0, min: -1f, max: 256f, increment: 1f, minSpecial: "RANDOM");
        public EditorProperty<int> Style;
        public EditorProperty<int> Style_Group;
        private SpriteMap _sprite;

        public override void EditorPropertyChanged(object property) => this.UpdateFrame();

        public GoodyNew(float xpos, float ypos)
          : base(xpos, ypos, new SpriteMap("challenge/goody", 16, 16))
        {
            this._sprite = this.graphic as SpriteMap;
            this.Style = new EditorProperty<int>(0, this, -1f, 3f, 1f, "RANDOM");
            this.Style_Group = new EditorProperty<int>(0, this, -1f, 4f, 1f, "RANDOM");
            this.Order._tooltip = "All Targets/Goodies with smaller Order numbers must be destroyed/collected before this goody appears.";
            this._editorName = "Goody";
            this._contextMenuFilter.Add("Sequence");
            this.sequence._resetLikelyhood = false;
        }

        public void UpdateFrame()
        {
            int num1 = this.Style.value;
            int num2 = this.Style_Group.value;
            if (num1 == -1)
                num1 = Rando.Int(0, 3);
            if (num2 == -1)
                num2 = Rando.Int(0, 4);
            if (num1 > 3)
                num1 = 3;
            if (num2 > 4)
                num2 = 4;
            this._sprite.frame = num1 + num2 * 4;
        }

        public override void Initialize()
        {
            this.sequence.order = this.Order.value;
            if (this.sequence.order == -1)
                this.sequence.order = Rando.Int(256);
            this.sequence.waitTillOrder = true;
            this.UpdateFrame();
            base.Initialize();
        }
    }
}
