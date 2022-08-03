// Decompiled with JetBrains decompiler
// Type: DuckGame.SpawnAimer
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class SpawnAimer : Thing
    {
        private float _moveSpeed;
        private float _thickness;
        private Color _color;
        private DuckPersona _persona;
        private Vec2 targetPos;
        private float distOut = 100f;
        private float distLen = 64f;
        private float sizeWaver;
        private float rot;
        private float streamAlpha = 1f;
        private SinWaveManualUpdate _sin = (SinWaveManualUpdate)0.25f;
        public Duck dugg;
        private float aimerScale = 1f;
        private List<Vec2> prevPos = new List<Vec2>();

        public SpawnAimer(
          float xpos,
          float ypos,
          int dir,
          float moveSpeed,
          Color color,
          DuckPersona person,
          float thickness)
          : base(xpos, ypos)
        {
            _moveSpeed = moveSpeed;
            _color = color;
            _thickness = thickness;
            offDir = (sbyte)dir;
            _persona = person;
            targetPos = new Vec2(xpos, ypos);
            if (_persona == Persona.Duck1)
                position = new Vec2(0f, 0f);
            else if (_persona == Persona.Duck2)
                position = new Vec2(320f, 0f);
            else if (_persona == Persona.Duck3)
                position = new Vec2(0f, 180f);
            else if (_persona == Persona.Duck4)
                position = new Vec2(320f, 180f);
            prevPos.Add(position);
            layer = Layer.Foreground;
        }

        public override void Update()
        {
            _sin.Update();
            if (GameMode.started)
                Level.Remove(this);
            distOut = Lerp.FloatSmooth(distOut, 16f, 0.08f, 1.2f);
            distLen = Lerp.FloatSmooth(distLen, 10f, 0.08f, 1.2f);
            rot = Lerp.FloatSmooth(rot, 45f, 0.08f, 1.1f);
            if (Math.Abs(rot - 45f) < 20.0)
            {
                streamAlpha -= 0.03f;
                if (streamAlpha < 0.0)
                    streamAlpha = 0f;
            }
            Level.current.camera.getMatrix();
            Vec2 targetPos = this.targetPos;
            aimerScale = layer.camera.width / Layer.HUD.width;
            position = Lerp.Vec2Smooth(position, targetPos, 0.2f);
            if ((position - targetPos).length > 16.0)
                prevPos.Add(position);
            sizeWaver += 0.2f;
        }

        public override void Draw()
        {
            float num1 = (this.distOut + (float)(Math.Sin(sizeWaver) * 2.0)) * aimerScale;
            float distOut = this.distOut;
            if (Network.isActive && dugg != null && dugg.profile != null && dugg.profile.connection == DuckNetwork.localConnection)
                distOut += _sin.value * 2f;
            _thickness = 2f;
            for (int index = 0; index < 4; ++index)
            {
                float deg = rot + index * 90f;
                Vec2 vec2 = new Vec2((float)Math.Cos(Maths.DegToRad(deg)), (float)-Math.Sin(Maths.DegToRad(deg)));
                Graphics.DrawLine(position + vec2 * distOut, position + vec2 * (distOut + distLen * aimerScale), _color * alpha, _thickness * aimerScale, (Depth)0.9f);
                Graphics.DrawLine(position + vec2 * (distOut - 1f * aimerScale), position + vec2 * (float)(distOut + 1.0 * aimerScale + distLen * aimerScale), Color.Black, (_thickness + 2f) * aimerScale, (Depth)0.8f);
            }
            if (streamAlpha <= 0.00999999977648258)
                return;
            int num2 = 0;
            Vec2 vec2_1 = Vec2.Zero;
            bool flag = false;
            foreach (Vec2 prevPo in prevPos)
            {
                if (flag)
                {
                    Vec2 normalized = (vec2_1 - prevPo).normalized;
                    Graphics.DrawLine(vec2_1 - normalized, prevPo + normalized, _color * streamAlpha, (float)(4.0 + num2 * 2.0) * aimerScale, (Depth)0.9f);
                }
                vec2_1 = prevPo;
                flag = true;
                ++num2;
            }
        }
    }
}
