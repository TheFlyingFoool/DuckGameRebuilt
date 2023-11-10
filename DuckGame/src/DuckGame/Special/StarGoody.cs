namespace DuckGame
{
    [EditorGroup("Special|Goodies", EditorItemType.Arcade)]
    [BaggedProperty("isOnlineCapable", false)]
    public class StarGoody : Goody
    {
        public EditorProperty<bool> valid;

        public override void EditorPropertyChanged(object property) => sequence.isValid = valid.value;

        public StarGoody(float xpos, float ypos)
          : base(xpos, ypos, new Sprite("challenge/star"))
        {
            valid = new EditorProperty<bool>(true, this);
        }
    }
}
