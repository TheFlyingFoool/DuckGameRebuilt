// Decompiled with JetBrains decompiler
// Type: DuckGame.FluidData
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public struct FluidData
    {
        public float amount;
        public Vec4 color;
        public float flammable;
        public string sprite;
        public float heat;
        public float transparent;
        public float douseFire;

        public FluidData(float am, Vec4 col, float flam, string spr = "", float h = 0.0f, float trans = 0.7f)
        {
            this.amount = am;
            this.color = col;
            this.flammable = flam;
            this.sprite = spr;
            this.heat = h;
            this.transparent = trans;
            this.douseFire = 1f;
        }

        public void Mix(FluidData with)
        {
            float num1 = with.amount + this.amount;
            if (with.amount > 0.0)
            {
                float num2 = this.amount / num1;
                float num3 = with.amount / num1;
                this.flammable = (float)((double)num2 * flammable + (double)num3 * with.flammable);
                this.color = this.color * num2 + with.color * num3;
                this.heat = (float)(heat * (double)num2 + with.heat * (double)num3);
                this.transparent = (float)(transparent * (double)num2 + with.transparent * (double)num3);
                this.douseFire = (float)(douseFire * (double)num2 + with.douseFire * (double)num3);
            }
            this.amount = num1;
        }

        public FluidData Take(float val)
        {
            if (val > this.amount)
            {
                val = this.amount;
            }
            this.amount -= val;
            FluidData newData = this;
            newData.amount = val;
            return newData;
        }
    }
}
