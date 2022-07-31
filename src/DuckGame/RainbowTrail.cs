// Decompiled with JetBrains decompiler
// Type: DuckGame.RainbowTrail
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class RainbowTrail : Thing
    {
        //private float killTime = 0.0001f;
        //private float killTimer;
        //private float counter;
        private PhysicsObject _attach;
        private List<TrailPiece> capePeices = new List<TrailPiece>();
        private int maxLength = 10;
        //private int minLength = 8;
        //private GeometryItemTexture _geo;
        //private float yDistance;
        private float _capeWave;
        private float _inverseWave;
        private float _inverseWave2;
        private float _capeWaveMult;
        //private Vec2 _lastPos;
        private bool _initLastPos = true;
        private Tex2D _capeTexture;

        public RainbowTrail(float xpos, float ypos, PhysicsObject attach)
          : base(xpos, ypos)
        {
            this.graphic = new Sprite("cape");
            this.visible = attach.visible;
            //this.killTimer = this.killTime;
            this._attach = attach;
            this.depth = -0.5f;
        }

        public override void Update()
        {
            base.Update();
            if (this._initLastPos)
            {
                //this._lastPos = this._attach.position;
                this._initLastPos = false;
            }
            Thing thing = _attach;
            float num1 = 1f;
            if (thing is TeamHat && thing.owner != null)
                thing = thing.owner;
            if (thing is Duck)
            {
                if ((thing as Duck).ragdoll != null && (thing as Duck).ragdoll.part1 != null)
                {
                    thing = (thing as Duck).ragdoll.part1;
                }
                else
                {
                    if ((thing as Duck).crouch)
                        num1 += 5f;
                    if ((thing as Duck).sliding)
                        num1 += 2f;
                }
            }
            float num2 = thing.velocity.length;
            if (num2 > 3.0)
                num2 = 3f;
            float num3 = (float)(1.0 - num2 / 3.0);
            this._capeWave += num2 * 0.1f;
            this._inverseWave += num3 * 0.09f;
            this._inverseWave2 += num3 * 0.06f;
            float num4 = (float)Math.Sin(_capeWave);
            float num5 = (float)Math.Sin(_inverseWave);
            float num6 = (float)Math.Sin(_inverseWave2);
            this._capeWaveMult = num2 * 0.5f;
            float num7 = num3 * 0.5f;
            this.offDir = (sbyte)-this._attach.offDir;
            Vec2 position = thing.position;
            Vec2 _p2 = thing.position;
            this.depth = thing.depth - 18;
            position.y += num1;
            _p2.y += num1;
            position.y += (float)(num4 * _capeWaveMult * (thing.velocity.x * 0.5));
            position.x += (float)(num4 * _capeWaveMult * (thing.velocity.y * 0.200000002980232));
            if (this.capePeices.Count > 0)
                _p2 = this.capePeices[this.capePeices.Count - 1].p1;
            this.capePeices.Add(new TrailPiece(thing.x - offDir * -10, thing.y + 6f, 0.5f, position, _p2));
            foreach (TrailPiece capePeice in this.capePeices)
            {
                if (capePeice.wide < 1.0)
                    capePeice.wide += 0.05f;
                capePeice.p1.x += (float)(num5 * num7 * (capePeice.wide - 0.5) * 0.899999976158142);
                capePeice.p2.x += (float)(num5 * num7 * (capePeice.wide - 0.5) * 0.899999976158142);
                capePeice.p1.y += (float)(num6 * num7 * (capePeice.wide - 0.5) * 0.800000011920929);
                capePeice.p2.y += (float)(num6 * num7 * (capePeice.wide - 0.5) * 0.800000011920929);
                ++capePeice.p1.y;
                ++capePeice.p2.y;
                capePeice.p1.x += 0.3f * offDir;
                capePeice.p2.x += 0.3f * offDir;
                capePeice.position.x += 0.5f * offDir;
            }
            while (this.capePeices.Count > this.maxLength)
                this.capePeices.RemoveAt(0);
            //this._lastPos = thing.position;
            this.visible = thing.visible;
            if (this._capeTexture != null)
                return;
            this._capeTexture = (Tex2D)Content.Load<Texture2D>("plainCape");
        }

        public override void Draw()
        {
            float num1 = 22f;
            float num2 = 0f;
            float num3 = 13f;
            Vec2 vec2_1 = Vec2.Zero;
            Vec2 vec2_2 = Vec2.Zero;
            bool flag1 = false;
            bool flag2 = false;
            Vec2 t1 = new Vec2(0f, 0f);
            Vec2 t2 = new Vec2(1f, 0f);
            Vec2 t3 = new Vec2(0f, 1f);
            Vec2 t4 = new Vec2(1f, 1f);
            if (this._capeTexture == null)
                return;
            float depth = DuckGame.Graphics.AdjustDepth(this.depth);
            for (int index = this.capePeices.Count - 1; index >= 0; --index)
            {
                TrailPiece capePeice = this.capePeices[index];
                Vec2 vec2_3 = vec2_2;
                if (index > 0)
                {
                    Vec2 vec2_4 = capePeice.p1 - this.capePeices[index - 1].p1;
                    vec2_4.Normalize();
                    vec2_3 = vec2_4.Rotate(Maths.DegToRad(90f), Vec2.Zero);
                }
                Vec2 vec2_5 = capePeice.p1;
                if (flag1)
                {
                    Vec2 vec2_6 = vec2_5 - vec2_1;
                    float length = vec2_6.length;
                    num2 += length;
                    vec2_6.Normalize();
                    if (num2 > num1)
                    {
                        vec2_5 = vec2_1 + vec2_6 * (length - (num2 - num1));
                        flag2 = true;
                    }
                    DuckGame.Graphics.screen.DrawQuad(vec2_5 - vec2_3 * (float)(num3 * capePeice.wide / 2.0), vec2_5 + vec2_3 * (float)(num3 * capePeice.wide / 2.0), vec2_1 - vec2_2 * (float)(num3 * capePeice.wide / 2.0), vec2_1 + vec2_2 * (float)(num3 * capePeice.wide / 2.0), t1, t2, t3, t4, depth, this._capeTexture, Color.White);
                    if (flag2)
                        break;
                }
                flag1 = true;
                vec2_1 = vec2_5;
                vec2_2 = vec2_3;
            }
        }
    }
}
