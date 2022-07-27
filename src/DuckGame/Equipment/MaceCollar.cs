// Decompiled with JetBrains decompiler
// Type: DuckGame.MaceCollar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("isOnlineCapable", false)]
    public class MaceCollar : ChokeCollar
    {
        public MaceCollar(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.editorTooltip = "A heavy ball & chain that can be swung with great force. For profit!";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this._ball = new WeightBall(this.x, this.y, this, this, true);
            this.ReturnItemToWorld(_ball);
            Level.Add(_ball);
        }
    }
}
