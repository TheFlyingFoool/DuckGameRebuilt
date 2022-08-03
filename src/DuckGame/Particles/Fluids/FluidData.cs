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

        public FluidData(float am, Vec4 col, float flam, string spr = "", float h = 0f, float trans = 0.7f)
        {
            amount = am;
            color = col;
            flammable = flam;
            sprite = spr;
            heat = h;
            transparent = trans;
            douseFire = 1f;
        }

        public void Mix(FluidData with)
        {
            float num1 = with.amount + amount;
            if (with.amount > 0.0)
            {
                float num2 = amount / num1;
                float num3 = with.amount / num1;
                flammable = (float)(num2 * flammable + num3 * with.flammable);
                color = color * num2 + with.color * num3;
                heat = (float)(heat * num2 + with.heat * num3);
                transparent = (float)(transparent * num2 + with.transparent * num3);
                douseFire = (float)(douseFire * num2 + with.douseFire * num3);
            }
            amount = num1;
        }

        public FluidData Take(float val)
        {
            if (val > amount)
            {
                val = amount;
            }
            amount -= val;
            FluidData newData = this;
            newData.amount = val;
            return newData;
        }
    }
}
