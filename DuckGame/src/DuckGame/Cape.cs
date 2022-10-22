using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class Cape : Thing
    {
        public Cape(float xpos, float ypos, PhysicsObject attach, bool trail = false) : base(xpos, ypos, null)
        {
            graphic = new Sprite("cape", 0f, 0f);
            visible = attach.visible;
            //this.killTimer = this.killTime;
            _attach = attach;
            base.depth = -0.5f;
            _trail = trail;
            _editorCanModify = false;
            if (_trail)
            {
                metadata.CapeTaperStart.value = 0.8f;
                metadata.CapeTaperEnd.value = 0f;
            }
        }

        public override void Update()
        {
            base.Update();
            Vec2 attachOffset = Vec2.Zero;
            if (_attach != null)
            {
                attachOffset = _attach.OffsetLocal(metadata.CapeOffset.value);
                if (_attach.removeFromLevel)
                {
                    Level.Remove(this);
                    return;
                }
            }
            _trail = metadata.CapeIsTrail.value;
            if (_initLastPos)
            {
                //this._lastPos = this._attach.position + attachOffset;
                _initLastPos = false;
            }
            Thing attach = _attach;
            float yOffset = 1f;
            if (attach is TeamHat && attach.owner != null)
            {
                attach = attach.owner;
            }
            if (attach is Duck)
            {
                if ((attach as Duck).ragdoll != null && (attach as Duck).ragdoll.part1 != null)
                {
                    attach = (attach as Duck).ragdoll.part1;
                }
                else
                {
                    if ((attach as Duck).crouch)
                    {
                        yOffset += 5f;
                    }
                    if ((attach as Duck).sliding)
                    {
                        yOffset += 2f;
                    }
                }
            }
            float velLength = attach.velocity.length;
            if (velLength > 3f)
            {
                velLength = 3f;
            }
            float inverseVel = 1f - velLength / 3f;
            _capeWave += velLength * 0.1f;
            _inverseWave += inverseVel * 0.09f;
            _inverseWave2 += inverseVel * 0.06f;
            float sin = (float)Math.Sin(_capeWave);
            float sin2 = (float)Math.Sin(_inverseWave);
            float sin3 = (float)Math.Sin(_inverseWave2);
            if (_trail)
            {
                sin = 0f;
                sin2 = 0f;
                sin3 = 0f;
            }
            _capeWaveMult = velLength * 0.5f;
            float inverseMult = inverseVel * 0.5f;
            offDir = (sbyte)-_attach.offDir;
            Vec2 p = attach.position + attachOffset;
            Vec2 p2 = attach.position + attachOffset;
            base.depth = (metadata.CapeForeground.value ? (attach.depth + 50) : (attach.depth - 50));
            p.y += yOffset;
            p2.y += yOffset;
            if (!_trail)
            {
                p.y += sin * _capeWaveMult * metadata.CapeWiggleModifier.value.y * (attach.velocity.x * 0.5f);
                p.x += sin * _capeWaveMult * metadata.CapeWiggleModifier.value.x * (attach.velocity.y * 0.2f);
            }
            if (capePeices.Count > 0)
            {
                p2 = capePeices[capePeices.Count - 1].p1;
            }
            if (_trail)
            {
                capePeices.Add(new CapePeice(attach.x + attachOffset.x, attach.y + attachOffset.y, metadata.CapeTaperStart.value, p, p2));
            }
            else
            {
                capePeices.Add(new CapePeice(attach.x - offDir * -10 + attachOffset.x, attach.y + 6f + attachOffset.y, metadata.CapeTaperStart.value, p, p2));
            }
            int idx = 0;
            foreach (CapePeice cp in capePeices)
            {
                cp.wide = Lerp.FloatSmooth(metadata.CapeTaperEnd.value, metadata.CapeTaperStart.value, idx / (float)(capePeices.Count - 1), 1f);
                if (!_trail)
                {
                    CapePeice capePeice = cp;
                    capePeice.p1.x += sin2 * inverseMult * metadata.CapeWiggleModifier.value.x * (cp.wide - 0.5f) * 0.9f;
                    CapePeice capePeice2 = cp;
                    capePeice2.p2.x += sin2 * inverseMult * metadata.CapeWiggleModifier.value.x * (cp.wide - 0.5f) * 0.9f;
                    CapePeice capePeice3 = cp;
                    capePeice3.p1.y += sin3 * inverseMult * metadata.CapeWiggleModifier.value.y * (cp.wide - 0.5f) * 0.8f;
                    CapePeice capePeice4 = cp;
                    capePeice4.p2.y += sin3 * inverseMult * metadata.CapeWiggleModifier.value.y * (cp.wide - 0.5f) * 0.8f;
                    CapePeice capePeice5 = cp;
                    capePeice5.p1.y += metadata.CapeSwayModifier.value.y;
                    CapePeice capePeice6 = cp;
                    capePeice6.p2.y += metadata.CapeSwayModifier.value.y;
                    CapePeice capePeice7 = cp;
                    capePeice7.p1.x += metadata.CapeSwayModifier.value.x * offDir;
                    CapePeice capePeice8 = cp;
                    capePeice8.p2.x += metadata.CapeSwayModifier.value.x * offDir;
                    CapePeice capePeice9 = cp;
                    capePeice9.position.x += 0.5f * offDir;
                }
                idx++;
            }
            if (_trail)
            {
                maxLength = 16;
            }
            while (capePeices.Count > maxLength + 1 && capePeices.Count > 0)
            {
                capePeices.RemoveAt(0);
            }
            //this._lastPos = attach.position + attachOffset;
            visible = attach.visible;
            if (attach is Holdable && attach.owner != null)
            {
                visible = attach.owner.visible;
                if (attach.owner.owner != null && attach.owner.owner is Duck)
                {
                    visible = attach.owner.owner.visible;
                }
            }
            if (_capeTexture == null)
            {
                SetCapeTexture(Content.Load<Texture2D>("plainCape"));
            }
        }

        public void SetCapeTexture(Texture2D tex)
        {
            _capeTexture = tex;
            maxLength = _capeTexture.height / 2 - 6;
            if (halfFlag)
            {
                maxLength = (int)(_capeTexture.width * 0.28f) - 6;
            }
        }

        public override void Draw()
        {
            if (_attach != null)
            {
                base.depth = (metadata.CapeForeground.value ? (_attach.depth + 50) : (_attach.depth - 50));
                bool hide = !_attach.visible;
                if (_attach.owner != null)
                {
                    hide &= !_attach.owner.visible;
                    if (_attach.owner.owner != null)
                    {
                        hide &= !_attach.owner.owner.visible;
                    }
                }
                if (hide)
                {
                    return;
                }
            }
            float capeWide = 13f;
            Vec2 lastPart = Vec2.Zero;
            Vec2 lastEdgeOffset = Vec2.Zero;
            bool hasLastPart = false;
            bool bust = false;
            if (_capeTexture != null)
            {
                float deep = Graphics.AdjustDepth(base.depth);
                float uvPart = 1f / (capePeices.Count - 1);
                float uvInc = 0f;
                for (int i = capePeices.Count - 1; i >= 0; i--)
                {
                    Vec2 texTL = new Vec2(0f, Math.Min(uvInc + uvPart, 1f));
                    Vec2 texTR = new Vec2(1f, Math.Min(uvInc + uvPart, 1f));
                    Vec2 texBL = new Vec2(0f, Math.Min(uvInc, 1f));
                    Vec2 texBR = new Vec2(1f, Math.Min(uvInc, 1f));
                    if (halfFlag)
                    {
                        texTL = new Vec2(Math.Min(uvInc + uvPart, 1f), 0f);
                        texTR = new Vec2(Math.Min(uvInc + uvPart, 1f), 1f);
                        texBL = new Vec2(Math.Min(uvInc, 1f), 0f);
                        texBR = new Vec2(Math.Min(uvInc, 1f), 1f);
                    }
                    if (offDir > 0)
                    {
                        Vec2 vec = texTL;
                        Vec2 bbl = texBL;
                        texTL = texTR;
                        texTR = vec;
                        texBL = texBR;
                        texBR = bbl;
                    }
                    CapePeice cp = capePeices[i];
                    Vec2 edgeOffset = lastEdgeOffset;
                    if (i > 0)
                    {
                        Vec2 v = cp.p1 - capePeices[i - 1].p1;
                        v.Normalize();
                        edgeOffset = v.Rotate(Maths.DegToRad(90f), Vec2.Zero);
                    }
                    Vec2 pos = cp.p1;
                    if (hasLastPart)
                    {
                        Vec2 v2 = pos - lastPart;
                        float length = v2.length;
                        v2.Normalize();
                        if (length > 2f)
                        {
                            pos = lastPart + v2 * 2f;
                        }
                        float drawAlpha = Lerp.Float(metadata.CapeAlphaStart.value, metadata.CapeAlphaEnd.value, i / (float)(capePeices.Count - 1));
                        Graphics.screen.DrawQuad(pos - edgeOffset * (capeWide * cp.wide / 2f), pos + edgeOffset * (capeWide * cp.wide / 2f), lastPart - lastEdgeOffset * (capeWide * cp.wide / 2f), lastPart + lastEdgeOffset * (capeWide * cp.wide / 2f), texTL, texTR, texBL, texBR, deep, _capeTexture, Color.White * drawAlpha);
                        if (bust)
                        {
                            break;
                        }
                    }
                    if (hasLastPart)
                    {
                        uvInc += uvPart;
                    }
                    hasLastPart = true;
                    lastPart = pos;
                    lastEdgeOffset = edgeOffset;
                }
            }
        }

        private float killTime = 0.0001f;

        //private float killTimer;

        //private float counter;

        private PhysicsObject _attach;

        private List<CapePeice> capePeices = new List<CapePeice>();

        private int maxLength = 10;

        //private int minLength = 8;

        //private GeometryItemTexture _geo;

        public bool _trail;

        //private float yDistance;

        private float _capeWave;

        private float _inverseWave;

        private float _inverseWave2;

        private float _capeWaveMult;

        //private Vec2 _lastPos;

        private bool _initLastPos = true;

        public Team.CustomHatMetadata metadata = new Team.CustomHatMetadata(null);

        public Tex2D _capeTexture;

        public bool halfFlag;
    }
}
