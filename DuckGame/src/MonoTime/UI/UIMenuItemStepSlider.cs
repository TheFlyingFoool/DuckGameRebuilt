namespace DuckGame
{
    /// <summary>
    /// A slider for a whole number setting: a bar of <c>segments</c> blocks, as many lit as the value.
    /// Left/right change it by one, between the field's min and max.
    /// </summary>
    public class UIMenuItemStepSlider : UIMenuItem
    {
        private FieldBinding _field;

        public UIMenuItemStepSlider(string text, FieldBinding field, int segments, Color c = default(Color))
          : base((UIMenuAction)null)
        {
            if (c == new Color())
                c = Colors.MenuOption;
            UIDivider divider = new UIDivider(true, 0f);
            divider.leftSection.Add(new UIText(text, c)
            {
                align = UIAlign.Left
            }, true);
            divider.rightSection.Add(new UIStepBar(30f, 7f, field, segments)
            {
                align = UIAlign.Right
            }, true);
            rightSection.Add(divider, true);
            _arrow = new UIImage("contextArrowRight")
            {
                align = UIAlign.Right,
                visible = false
            };
            leftSection.Add(_arrow, true);
            _field = field;
            controlString = "@CANCEL@BACK @WASD@ADJUST";
        }

        public override void Activate(string trigger)
        {
            int value = (int)_field.value;
            int next;
            if (trigger == Triggers.MenuLeft)
                next = value - 1;
            else if (trigger == Triggers.MenuRight)
                next = value + 1;
            else
                return;
            next = (int)Maths.Clamp(next, _field.min, _field.max);
            if (next != value)
            {
                SFX.DontSave = 1;
                SFX.Play("textLetter", 0.7f);
            }
            _field.value = next;
        }
    }

    /// <summary>The bar for <see cref="UIMenuItemStepSlider"/>, drawn like <see cref="UIProgressBar"/>.</summary>
    public class UIStepBar : UIComponent
    {
        private FieldBinding _field;
        private Vec2 _barSize;
        private int _segments;

        public UIStepBar(float wide, float high, FieldBinding field, int segments)
          : base(0f, 0f, 0f, 0f)
        {
            _field = field;
            _barSize = new Vec2(wide, high);
            _collisionSize = _barSize;
            _segments = segments;
        }

        public override void Draw()
        {
            UILerp.UpdateLerpState(position, MonoMain.IntraTick, MonoMain.UpdateLerpState);

            float wide = _barSize.x * scale.x;
            float high = _barSize.y * scale.y;
            int segmentWidth = (int)System.Math.Round(wide / _segments);
            int lit = (int)_field.value;
            for (int index = 0; index < _segments; ++index)
            {
                Vec2 p1 = UILerp.Position - new Vec2(halfWidth, high / 2f) + new Vec2(index * segmentWidth, 0f);
                Vec2 p2 = UILerp.Position - new Vec2(halfWidth, -high / 2f) + new Vec2((index + 1) * segmentWidth - 1f, 0f);
                if ((align & UIAlign.Center) > UIAlign.Center)
                {
                    p1.x += halfWidth - wide / 2f;
                    p2.x += halfWidth - wide / 2f;
                }
                else if ((align & UIAlign.Right) > UIAlign.Center)
                {
                    p1.x += width - wide;
                    p2.x += width - wide;
                }
                Graphics.DrawRect(p1, p2, index < lit ? Color.White : new Color(70, 70, 70), depth);
            }
        }
    }
}
