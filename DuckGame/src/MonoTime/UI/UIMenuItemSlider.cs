namespace DuckGame
{
    public class UIMenuItemSlider : UIMenuItem
    {
        private FieldBinding _field;
        private float _step;

        public UIMenuItemSlider(
          string text,
          UIMenuAction action = null,
          FieldBinding field = null,
          float step = 0.1f,
          Color c = default(Color))
          : base(action)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            UIDivider component1 = new UIDivider(true, 0f);
            UIText component2 = new UIText(text, c)
            {
                align = UIAlign.Left
            };
            component1.leftSection.Add(component2, true);
            UIProgressBar component3 = new UIProgressBar(step < 0.05263158f ? 26f : 30f, 7f, field, step)
            {
                align = UIAlign.Right
            };
            component1.rightSection.Add(component3, true);
            rightSection.Add(component1, true);
            _arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            leftSection.Add(_arrow, true);
            _field = field;
            _step = step;
            controlString = "@CANCEL@BACK @WASD@ADJUST";
        }

        public override void Activate(string trigger)
        {
            float num;
            if (trigger == Triggers.MenuLeft)
            {
                num = Maths.Clamp((float)_field.value - _step, _field.min, _field.max);
            }
            else
            {
                if (!(trigger == Triggers.MenuRight))
                    return;
                num = Maths.Clamp((float)_field.value + _step, _field.min, _field.max);
            }
            if (num != (float)_field.value)
            {
                SFX.DontSave = 1;
                SFX.Play("textLetter", 0.7f);
            }
            _field.value = num;
        }
    }
}
