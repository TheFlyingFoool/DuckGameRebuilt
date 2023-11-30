using System;
using System.Collections.Generic;
using System.Globalization;

namespace DuckGame
{
    public class ContextSlider : ContextMenu
    {
        private SpriteMap _radioButton;
        private FieldBinding _field;
        private SpriteMap _adjusterHand;
        private float _step;
        private string _minSpecial;
        private bool _adjust;
        private bool _time;
        private Type _myType;

        public bool adjust
        {
            get => _adjust;
            set => _adjust = value;
        }

        public ContextSlider(
          string text,
          IContextListener owner,
          FieldBinding field,
          float step,
          string minSpecial,
          bool time,
          Type myType,
          string valTooltip)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            _radioButton = new SpriteMap("Editor/radioButton", 16, 16);
            _dragMode = true;
            _step = step;
            _minSpecial = minSpecial;
            _adjusterHand = new SpriteMap("adjusterHand", 18, 17);
            _time = time;
            _myType = myType;
            tooltip = valTooltip;
            if (_field == null || _field.value == null || !_field.value.GetType().IsEnum)
                return;
            _step = 1f;
        }

        public ContextSlider(
          string text,
          IContextListener owner,
          FieldBinding field = null,
          float step = 0.25f,
          string minSpecial = null,
          bool time = false,
          Type myType = null)
          : base(owner)
        {
            itemSize.x = 150f;
            itemSize.y = 16f;
            _text = text;
            _field = field;
            _radioButton = new SpriteMap("Editor/radioButton", 16, 16);
            _dragMode = true;
            _step = step;
            _minSpecial = minSpecial;
            _adjusterHand = new SpriteMap("adjusterHand", 18, 17);
            _time = time;
            _myType = myType;
            if (_field == null || _field.value == null || !_field.value.GetType().IsEnum)
                return;
            _step = 1f;
        }

        protected override void OnClose()
        {
            _adjust = false;
            _hover = false;
        }

        public override void Selected()
        {
            if (Editor.inputMode == EditorInput.Mouse || !_enteringSlideMode && Editor.inputMode == EditorInput.Touch && TouchScreen.GetPress().Check(new Rectangle(x, y, itemSize.x, itemSize.y), layer.camera))
                _canEditSlide = true;
            _enteringSlideMode = false;
            if (_canEditSlide)
            {
                switch (Editor.inputMode)
                {
                    case EditorInput.Mouse:
                        _sliding = true;
                        float num1 = Maths.Clamp(Mouse.x - position.x, 0f, itemSize.x);
                        if (Editor.inputMode == EditorInput.Touch)
                            num1 = Maths.Clamp(TouchScreen.GetTouch().Transform(layer.camera).x - position.x, 0f, itemSize.x);
                        if (_field.value is List<TypeProbPair>)
                        {
                            if (_step > 0f)
                                num1 = (float)(Math.Round(num1 / itemSize.x * 1f / _step) * _step / 1f) * itemSize.x;
                            TypeProbPair typeProbPair1 = null;
                            List<TypeProbPair> typeProbPairList = _field.value as List<TypeProbPair>;
                            foreach (TypeProbPair typeProbPair2 in typeProbPairList)
                            {
                                if (typeProbPair2.type == _myType)
                                {
                                    typeProbPair1 = typeProbPair2;
                                    break;
                                }
                            }
                            if (typeProbPair1 == null)
                            {
                                typeProbPair1 = new TypeProbPair()
                                {
                                    probability = 0f,
                                    type = _myType
                                };
                                typeProbPairList.Add(typeProbPair1);
                            }
                            typeProbPair1.probability = (0f + num1 / itemSize.x * 1f);
                            if (typeProbPair1.probability == 0f)
                            {
                                typeProbPairList.Remove(typeProbPair1);
                                goto label_30;
                            }
                            else
                                goto label_30;
                        }
                        else
                        {
                            float num2 = Math.Abs(_field.max - _field.min);
                            if (_step > 0f)
                                num1 = Maths.Snap(num1 / itemSize.x * num2, _step) / num2 * itemSize.x;
                            if (_field.value is float)
                            {
                                _field.value = (_field.min + num1 / itemSize.x * num2);
                                goto label_30;
                            }
                            else if (_field.value is int)
                            {
                                _field.value = (int)Math.Round(_field.min + num1 / itemSize.x * (Math.Abs(_field.min) + _field.max));
                                goto label_30;
                            }
                            else if (_field.value != null && _field.value.GetType().IsEnum)
                            {
                                int index = (int)Math.Round(_field.min + num1 / itemSize.x * (Math.Abs(_field.min) + _field.max));
                                Array values = Enum.GetValues(_field.value.GetType());
                                if (index >= 0 && index < values.Length)
                                {
                                    _field.value = values.GetValue(index);
                                    goto label_30;
                                }
                                else
                                    goto label_30;
                            }
                            else
                                goto label_30;
                        }
                    case EditorInput.Touch:
                        if (!TouchScreen.GetTouch().Check(new Rectangle(x, y, itemSize.x, itemSize.y), layer.camera))
                            break;
                        goto case EditorInput.Mouse;
                }
            }
            _sliding = false;
        label_30:
            Editor.hasUnsavedChanges = true;
        }

        public void Increment()
        {
            if (_field.value is List<TypeProbPair>)
            {
                TypeProbPair typeProbPair1 = null;
                List<TypeProbPair> typeProbPairList = _field.value as List<TypeProbPair>;
                foreach (TypeProbPair typeProbPair2 in typeProbPairList)
                {
                    if (typeProbPair2.type == _myType)
                    {
                        typeProbPair1 = typeProbPair2;
                        break;
                    }
                }
                if (typeProbPair1 == null)
                {
                    typeProbPair1 = new TypeProbPair()
                    {
                        probability = 0f,
                        type = _myType
                    };
                    typeProbPairList.Add(typeProbPair1);
                }
                typeProbPair1.probability += _step;
                typeProbPair1.probability = Maths.Clamp(typeProbPair1.probability, 0f, 1f);
            }
            else if (_field.value is float)
                _field.value = Maths.Clamp((float)_field.value + _step, _field.min, _field.max);
            else if (_field.value is int)
                _field.value = (int)Maths.Clamp((int)_field.value + (int)_step, _field.min, _field.max);
            else if (_field.value != null && _field.value.GetType().IsEnum)
            {
                int index = (int)Maths.Clamp((int)_field.value + (int)_step, _field.min, _field.max);
                Array values = Enum.GetValues(_field.value.GetType());
                if (index >= 0 && index < values.Length)
                    _field.value = values.GetValue(index);
            }
            Editor.hasUnsavedChanges = true;
        }

        public void Decrement()
        {
            if (_field.value is List<TypeProbPair>)
            {
                TypeProbPair typeProbPair1 = null;
                List<TypeProbPair> typeProbPairList = _field.value as List<TypeProbPair>;
                foreach (TypeProbPair typeProbPair2 in typeProbPairList)
                {
                    if (typeProbPair2.type == _myType)
                    {
                        typeProbPair1 = typeProbPair2;
                        break;
                    }
                }
                if (typeProbPair1 == null)
                {
                    typeProbPair1 = new TypeProbPair()
                    {
                        probability = 0f,
                        type = _myType
                    };
                    typeProbPairList.Add(typeProbPair1);
                }
                if (typeProbPair1.probability == 0)
                {
                    typeProbPairList.Remove(typeProbPair1);
                }
                else
                {
                    typeProbPair1.probability -= _step;
                    typeProbPair1.probability = Maths.Clamp(typeProbPair1.probability, 0f, 1f);
                }
            }
            else if (_field.value is float)
                _field.value = Maths.Clamp((float)_field.value - _step, _field.min, _field.max);
            else if (_field.value is int)
                _field.value = (int)Maths.Clamp((int)_field.value - (int)_step, _field.min, _field.max);
            else if (_field.value != null && _field.value.GetType().IsEnum)
            {
                int index = (int)Maths.Clamp((int)_field.value - (int)_step, _field.min, _field.max);
                Array values = Enum.GetValues(_field.value.GetType());
                if (index >= 0 && index < values.Length)
                    _field.value = values.GetValue(index);
            }
            Editor.hasUnsavedChanges = true;
        }

        public override void Update()
        {
            if (Editor.inputMode == EditorInput.Gamepad)
            {
                if (_hover || _adjust)
                {
                    if (Input.Pressed(Triggers.Select))
                        _adjust = true;
                    if (Input.Released(Triggers.Select))
                        _adjust = false;
                }
                if (_adjust)
                {
                    Editor.tookInput = true;
                    int num = 1;
                    float step = _step;
                    if (Input.Down(Triggers.Ragdoll))
                        num = 5;
                    if (Input.Down(Triggers.Strafe))
                        _step *= 0.1f;
                    if (Input.Pressed(Triggers.MenuLeft))
                    {
                        for (int index = 0; index < num; ++index)
                            Decrement();
                    }
                    if (Input.Pressed(Triggers.MenuRight))
                    {
                        for (int index = 0; index < num; ++index)
                            Increment();
                    }
                    _step = step;
                }
            }
            else if (_hover)
            {
                _adjust = true;
                if (Mouse.scroll > 0f)
                {
                    Editor.didUIScroll = true;
                    Decrement();
                    _didContextScroll = true;
                }
                if (Mouse.scroll < 0f)
                {
                    Editor.didUIScroll = true;
                    Increment();
                    _didContextScroll = true;
                }
            }
            else
                _adjust = false;
            base.Update();
        }

        public override void Draw()
        {
            float num1 = 0f;
            string text1 = "";
            if (_field.value is List<TypeProbPair>)
            {
                TypeProbPair typeProbPair1 = null;
                foreach (TypeProbPair typeProbPair2 in _field.value as List<TypeProbPair>)
                {
                    if (typeProbPair2.type == _myType)
                    {
                        typeProbPair1 = typeProbPair2;
                        break;
                    }
                }
                num1 = typeProbPair1 == null ? 0f : typeProbPair1.probability;
                text1 = num1.ToString("0.00", CultureInfo.InvariantCulture);
            }
            else if (_field.value is float)
            {
                num1 = (float)_field.value;
                text1 = num1.ToString("0.00", CultureInfo.InvariantCulture);
            }
            else if (_field.value is int)
            {
                num1 = (int)_field.value;
                text1 = Change.ToString((int)_field.value);
            }
            else if (_field.value != null && _field.value.GetType().IsEnum)
            {
                num1 = (int)_field.value;
                text1 = Enum.GetName(_field.value.GetType(), _field.value);
            }
            if (_minSpecial != null && num1 == _field.min)
                text1 = _minSpecial;
            else if (_time)
                text1 = MonoMain.TimeString(TimeSpan.FromSeconds((int)num1), 2);
            if (_adjust)
            {
                float num2 = _field.max - _field.min;
                float x1 = (4f + (num2 - (_field.max - num1)) / num2 * (itemSize.x - 8f));
                float x2 = 0f;
                float x3 = itemSize.x;
                string text2 = _text + ": " + text1;
                Color color = Color.White;
                if (_field.value is List<TypeProbPair>)
                    color = num1 != 0f ? (num1 >= 0.3f ? (num1 >= 0.7f ? Color.Green : Color.Orange) : Colors.DGRed) : Color.DarkGray;
                float num3 = 0.1f;
                if (Editor.inputMode == EditorInput.Gamepad)
                    num3 = 0.05f;
                bool flag = false;
                float stringWidth = Graphics.GetStringWidth(text2);
                if (position.x + itemSize.x + 8f + stringWidth > layer.width)
                    flag = true;
                if (flag)
                {
                    x2 = (float)(-stringWidth - 12f);
                    Graphics.DrawString(text2, position + new Vec2((-stringWidth - 8f), 5f), color, (Depth)(0.82f + num3));
                }
                else
                {
                    Graphics.DrawString(text2, position + new Vec2(itemSize.x + 8f, 5f), color, (Depth)(0.82f + num3));
                    x3 += stringWidth + 10f;
                }
                Graphics.DrawRect(position + new Vec2(x1 - 2f, 3f), position + new Vec2(x1 + 2f, itemSize.y - 3f), new Color(250, 250, 250), (Depth)(0.85f + num3));
                Graphics.DrawRect(position + new Vec2(x2, 0f), position + new Vec2(x3, itemSize.y), new Color(70, 70, 70), (Depth)(0.75f + num3));
                Graphics.DrawRect(position + new Vec2(4f, (float)(itemSize.y / 2 - 2)), position + new Vec2(itemSize.x - 4f, (float)(itemSize.y / 2 + 2)), new Color(150, 150, 150), (Depth)(0.82f + num3));
                if (Editor.inputMode != EditorInput.Gamepad)
                    return;
                Vec2 vec2 = position + new Vec2(x1, 0f);
                _adjusterHand.depth = (Depth)0.9f;
                Graphics.Draw(_adjusterHand, vec2.x - 6f, vec2.y - 6f);
            }
            else
            {
                if (_hover)
                    Graphics.DrawRect(position, position + itemSize, new Color(70, 70, 70), depth);
                Color color = Color.White;
                if (_field.value is List<TypeProbPair>)
                    color = num1 != 0 ? (num1 >= 0.3f ? (num1 >= 0.7f ? Color.Green : Color.Orange) : Colors.DGRed) : Color.DarkGray;
                Graphics.DrawString(_text, position + new Vec2(2f, 5f), color, (Depth)0.82f);
                Graphics.DrawString(text1, position + new Vec2(itemSize.x - 4f - Graphics.GetStringWidth(text1), 5f), Color.White, (Depth)0.82f);
            }
        }
    }
}
