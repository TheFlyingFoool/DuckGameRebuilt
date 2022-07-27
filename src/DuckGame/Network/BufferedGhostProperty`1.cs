// Decompiled with JetBrains decompiler
// Type: DuckGame.BufferedGhostProperty`1
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class BufferedGhostProperty<T> : BufferedGhostProperty
    {
        private T _value;

        public override object value
        {
            get => _value;
            set => this._value = (T)value;
        }

        public override bool Refresh()
        {
            T newVal;
            if (this.binding.Compare<T>(this._value, out newVal))
                return false;
            this._value = newVal;
            return true;
        }

        public override void UpdateFrom(StateBinding bind) => this._value = bind.getTyped<T>();

        public override void Apply(float lerp)
        {
            if ((double)lerp < 1.0)
            {
                if (this.binding is CompressedVec2Binding)
                {
                    Vec2 typed = this.binding.getTyped<Vec2>();
                    Vec2 to = (Vec2)this.value;
                    if ((double)(typed - to).lengthSq > 1024.0)
                        this.binding.setTyped<Vec2>(to);
                    else
                        this.binding.setTyped<Vec2>(Lerp.Vec2Smooth(typed, to, lerp));
                }
                else if (this.binding.isRotation)
                    this.binding.setTyped<float>(Maths.DegToRad(Maths.PointDirection(Vec2.Zero, BufferedGhostProperty.Slerp(Maths.AngleToVec(this.binding.getTyped<float>()), Maths.AngleToVec((float)this.value), lerp))));
                else
                    this.binding.setTyped<T>(this._value);
            }
            else
            {
                if (this.binding.name == "netPosition")
                {
                    double x = ((Vec2)value).x;
                }
                this.binding.setTyped<T>(this._value);
            }
        }
    }
}
