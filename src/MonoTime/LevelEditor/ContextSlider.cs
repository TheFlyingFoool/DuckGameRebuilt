// Decompiled with JetBrains decompiler
// Type: DuckGame.ContextSlider
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
        private System.Type _myType;

        public bool adjust
        {
            get => this._adjust;
            set => this._adjust = value;
        }

        public ContextSlider(
          string text,
          IContextListener owner,
          FieldBinding field,
          float step,
          string minSpecial,
          bool time,
          System.Type myType,
          string valTooltip)
          : base(owner)
        {
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            this._radioButton = new SpriteMap("Editor/radioButton", 16, 16);
            this._dragMode = true;
            this._step = step;
            this._minSpecial = minSpecial;
            this._adjusterHand = new SpriteMap("adjusterHand", 18, 17);
            this._time = time;
            this._myType = myType;
            this.tooltip = valTooltip;
            if (this._field == null || this._field.value == null || !this._field.value.GetType().IsEnum)
                return;
            this._step = 1f;
        }

        public ContextSlider(
          string text,
          IContextListener owner,
          FieldBinding field = null,
          float step = 0.25f,
          string minSpecial = null,
          bool time = false,
          System.Type myType = null)
          : base(owner)
        {
            this.itemSize.x = 150f;
            this.itemSize.y = 16f;
            this._text = text;
            this._field = field;
            this._radioButton = new SpriteMap("Editor/radioButton", 16, 16);
            this._dragMode = true;
            this._step = step;
            this._minSpecial = minSpecial;
            this._adjusterHand = new SpriteMap("adjusterHand", 18, 17);
            this._time = time;
            this._myType = myType;
            if (this._field == null || this._field.value == null || !this._field.value.GetType().IsEnum)
                return;
            this._step = 1f;
        }

        protected override void OnClose()
        {
            this._adjust = false;
            this._hover = false;
        }

        public override void Selected()
        {
            if (Editor.inputMode == EditorInput.Mouse || !this._enteringSlideMode && Editor.inputMode == EditorInput.Touch && TouchScreen.GetPress().Check(new Rectangle(this.x, this.y, this.itemSize.x, this.itemSize.y), this.layer.camera))
                this._canEditSlide = true;
            this._enteringSlideMode = false;
            if (this._canEditSlide)
            {
                switch (Editor.inputMode)
                {
                    case EditorInput.Mouse:
                        this._sliding = true;
                        float num1 = Maths.Clamp(Mouse.x - this.position.x, 0f, this.itemSize.x);
                        if (Editor.inputMode == EditorInput.Touch)
                            num1 = Maths.Clamp(TouchScreen.GetTouch().Transform(this.layer.camera).x - this.position.x, 0f, this.itemSize.x);
                        if (this._field.value is List<TypeProbPair>)
                        {
                            if (_step > 0f)
                                num1 = (float)(Math.Round(num1 / itemSize.x * 1f / _step) * _step / 1f) * this.itemSize.x;
                            TypeProbPair typeProbPair1 = null;
                            List<TypeProbPair> typeProbPairList = this._field.value as List<TypeProbPair>;
                            foreach (TypeProbPair typeProbPair2 in typeProbPairList)
                            {
                                if (typeProbPair2.type == this._myType)
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
                                    type = this._myType
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
                            float num2 = Math.Abs(this._field.max - this._field.min);
                            if (_step > 0f)
                                num1 = Maths.Snap(num1 / this.itemSize.x * num2, this._step) / num2 * this.itemSize.x;
                            if (this._field.value is float)
                            {
                                this._field.value = (this._field.min + num1 / itemSize.x * num2);
                                goto label_30;
                            }
                            else if (this._field.value is int)
                            {
                                this._field.value = (int)Math.Round(this._field.min + num1 / itemSize.x * (Math.Abs(this._field.min) + this._field.max));
                                goto label_30;
                            }
                            else if (this._field.value != null && this._field.value.GetType().IsEnum)
                            {
                                int index = (int)Math.Round(this._field.min + num1 / itemSize.x * (Math.Abs(this._field.min) + this._field.max));
                                Array values = Enum.GetValues(this._field.value.GetType());
                                if (index >= 0 && index < values.Length)
                                {
                                    this._field.value = values.GetValue(index);
                                    goto label_30;
                                }
                                else
                                    goto label_30;
                            }
                            else
                                goto label_30;
                        }
                    case EditorInput.Touch:
                        if (!TouchScreen.GetTouch().Check(new Rectangle(this.x, this.y, this.itemSize.x, this.itemSize.y), this.layer.camera))
                            break;
                        goto case EditorInput.Mouse;
                }
            }
            this._sliding = false;
        label_30:
            Editor.hasUnsavedChanges = true;
        }

        public void Increment()
        {
            if (this._field.value is List<TypeProbPair>)
            {
                TypeProbPair typeProbPair1 = null;
                List<TypeProbPair> typeProbPairList = this._field.value as List<TypeProbPair>;
                foreach (TypeProbPair typeProbPair2 in typeProbPairList)
                {
                    if (typeProbPair2.type == this._myType)
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
                        type = this._myType
                    };
                    typeProbPairList.Add(typeProbPair1);
                }
                typeProbPair1.probability += this._step;
                typeProbPair1.probability = Maths.Clamp(typeProbPair1.probability, 0f, 1f);
            }
            else if (this._field.value is float)
                this._field.value = Maths.Clamp((float)this._field.value + this._step, this._field.min, this._field.max);
            else if (this._field.value is int)
                this._field.value = (int)Maths.Clamp((int)this._field.value + (int)this._step, this._field.min, this._field.max);
            else if (this._field.value != null && this._field.value.GetType().IsEnum)
            {
                int index = (int)Maths.Clamp((int)this._field.value + (int)this._step, this._field.min, this._field.max);
                Array values = Enum.GetValues(this._field.value.GetType());
                if (index >= 0 && index < values.Length)
                    this._field.value = values.GetValue(index);
            }
            Editor.hasUnsavedChanges = true;
        }

        public void Decrement()
        {
            if (this._field.value is List<TypeProbPair>)
            {
                TypeProbPair typeProbPair1 = null;
                List<TypeProbPair> typeProbPairList = this._field.value as List<TypeProbPair>;
                foreach (TypeProbPair typeProbPair2 in typeProbPairList)
                {
                    if (typeProbPair2.type == this._myType)
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
                        type = this._myType
                    };
                    typeProbPairList.Add(typeProbPair1);
                }
                if (typeProbPair1.probability == 0.0)
                {
                    typeProbPairList.Remove(typeProbPair1);
                }
                else
                {
                    typeProbPair1.probability -= this._step;
                    typeProbPair1.probability = Maths.Clamp(typeProbPair1.probability, 0f, 1f);
                }
            }
            else if (this._field.value is float)
                this._field.value = Maths.Clamp((float)this._field.value - this._step, this._field.min, this._field.max);
            else if (this._field.value is int)
                this._field.value = (int)Maths.Clamp((int)this._field.value - (int)this._step, this._field.min, this._field.max);
            else if (this._field.value != null && this._field.value.GetType().IsEnum)
            {
                int index = (int)Maths.Clamp((int)this._field.value - (int)this._step, this._field.min, this._field.max);
                Array values = Enum.GetValues(this._field.value.GetType());
                if (index >= 0 && index < values.Length)
                    this._field.value = values.GetValue(index);
            }
            Editor.hasUnsavedChanges = true;
        }

        public override void Update()
        {
            if (Editor.inputMode == EditorInput.Gamepad)
            {
                if (this._hover || this._adjust)
                {
                    if (Input.Pressed("SELECT"))
                        this._adjust = true;
                    if (Input.Released("SELECT"))
                        this._adjust = false;
                }
                if (this._adjust)
                {
                    Editor.tookInput = true;
                    int num = 1;
                    float step = this._step;
                    if (Input.Down("RAGDOLL"))
                        num = 5;
                    if (Input.Down("STRAFE"))
                        this._step *= 0.1f;
                    if (Input.Pressed("MENULEFT"))
                    {
                        for (int index = 0; index < num; ++index)
                            this.Decrement();
                    }
                    if (Input.Pressed("MENURIGHT"))
                    {
                        for (int index = 0; index < num; ++index)
                            this.Increment();
                    }
                    this._step = step;
                }
            }
            else if (this._hover)
            {
                this._adjust = true;
                if (Mouse.scroll > 0f)
                {
                    Editor.didUIScroll = true;
                    this.Decrement();
                    ContextMenu._didContextScroll = true;
                }
                if (Mouse.scroll < 0f)
                {
                    Editor.didUIScroll = true;
                    this.Increment();
                    ContextMenu._didContextScroll = true;
                }
            }
            else
                this._adjust = false;
            base.Update();
        }

        public override void Draw()
        {
            float num1 = 0f;
            string text1 = "";
            if (this._field.value is List<TypeProbPair>)
            {
                TypeProbPair typeProbPair1 = null;
                foreach (TypeProbPair typeProbPair2 in this._field.value as List<TypeProbPair>)
                {
                    if (typeProbPair2.type == this._myType)
                    {
                        typeProbPair1 = typeProbPair2;
                        break;
                    }
                }
                num1 = typeProbPair1 == null ? 0f : typeProbPair1.probability;
                text1 = num1.ToString("0.00", CultureInfo.InvariantCulture);
            }
            else if (this._field.value is float)
            {
                num1 = (float)this._field.value;
                text1 = num1.ToString("0.00", CultureInfo.InvariantCulture);
            }
            else if (this._field.value is int)
            {
                num1 = (int)this._field.value;
                text1 = Change.ToString((int)this._field.value);
            }
            else if (this._field.value != null && this._field.value.GetType().IsEnum)
            {
                num1 = (int)this._field.value;
                text1 = Enum.GetName(this._field.value.GetType(), this._field.value);
            }
            if (this._minSpecial != null && num1 == this._field.min)
                text1 = this._minSpecial;
            else if (this._time)
                text1 = MonoMain.TimeString(TimeSpan.FromSeconds((int)num1), 2);
            if (this._adjust)
            {
                float num2 = this._field.max - this._field.min;
                float x1 = (4f + (num2 - (this._field.max - num1)) / num2 * (itemSize.x - 8f));
                float x2 = 0f;
                float x3 = this.itemSize.x;
                string text2 = this._text + ": " + text1;
                Color color = Color.White;
                if (this._field.value is List<TypeProbPair>)
                    color = num1 != 0f ? (num1 >= 0.3f ? (num1 >= 0.7f ? Color.Green : Color.Orange) : Colors.DGRed) : Color.DarkGray;
                float num3 = 0.1f;
                if (Editor.inputMode == EditorInput.Gamepad)
                    num3 = 0.05f;
                bool flag = false;
                float stringWidth = Graphics.GetStringWidth(text2);
                if (position.x + this.itemSize.x + 8f + stringWidth > this.layer.width)
                    flag = true;
                if (flag)
                {
                    x2 = (float)(-stringWidth - 12f);
                    Graphics.DrawString(text2, this.position + new Vec2((-stringWidth - 8f), 5f), color, (Depth)(0.82f + num3));
                }
                else
                {
                    Graphics.DrawString(text2, this.position + new Vec2(this.itemSize.x + 8f, 5f), color, (Depth)(0.82f + num3));
                    x3 += stringWidth + 10f;
                }
                Graphics.DrawRect(this.position + new Vec2(x1 - 2f, 3f), this.position + new Vec2(x1 + 2f, this.itemSize.y - 3f), new Color(250, 250, 250), (Depth)(0.85f + num3));
                Graphics.DrawRect(this.position + new Vec2(x2, 0f), this.position + new Vec2(x3, this.itemSize.y), new Color(70, 70, 70), (Depth)(0.75f + num3));
                Graphics.DrawRect(this.position + new Vec2(4f, (float)(itemSize.y / 2.0 - 2.0)), this.position + new Vec2(this.itemSize.x - 4f, (float)(itemSize.y / 2.0 + 2.0)), new Color(150, 150, 150), (Depth)(0.82f + num3));
                if (Editor.inputMode != EditorInput.Gamepad)
                    return;
                Vec2 vec2 = this.position + new Vec2(x1, 0f);
                this._adjusterHand.depth = (Depth)0.9f;
                Graphics.Draw(_adjusterHand, vec2.x - 6f, vec2.y - 6f);
            }
            else
            {
                if (this._hover)
                    Graphics.DrawRect(this.position, this.position + this.itemSize, new Color(70, 70, 70), this.depth);
                Color color = Color.White;
                if (this._field.value is List<TypeProbPair>)
                    color = num1 != 0.0 ? (num1 >= 0.3f ? (num1 >= 0.7f ? Color.Green : Color.Orange) : Colors.DGRed) : Color.DarkGray;
                Graphics.DrawString(this._text, this.position + new Vec2(2f, 5f), color, (Depth)0.82f);
                Graphics.DrawString(text1, this.position + new Vec2(this.itemSize.x - 4f - Graphics.GetStringWidth(text1), 5f), Color.White, (Depth)0.82f);
            }
        }
    }
}
