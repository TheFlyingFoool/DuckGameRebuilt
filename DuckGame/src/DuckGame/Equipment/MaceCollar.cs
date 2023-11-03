namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("isOnlineCapable", false)]
    public class MaceCollar : ChokeCollar
    {
        public MaceCollar(float xpos, float ypos)
          : base(xpos, ypos)
        {
            editorTooltip = "A heavy ball & chain that can be swung with great force. For profit!";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            _ball = new WeightBall(x, y, this, this, true);
            ReturnItemToWorld(_ball);
            Level.Add(_ball);
        }
    }
}
