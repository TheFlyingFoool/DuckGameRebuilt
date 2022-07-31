// Decompiled with JetBrains decompiler
// Type: DuckGame.CityBackground
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Background|Parallax")]
    [BaggedProperty("previewPriority", true)]
    public class CityBackground : BackgroundUpdater
    {
        //private Vec2 backgroundPlanePos;
        private List<CityBackground.Plane> _planes = new List<CityBackground.Plane>();
        private float timeSinceSkySay;

        public CityBackground(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.graphic = new SpriteMap("backgroundIcons", 16, 16)
            {
                frame = 5
            };
            this.center = new Vec2(8f, 8f);
            this._collisionSize = new Vec2(16f, 16f);
            this._collisionOffset = new Vec2(-8f, -8f);
            this.depth = (Depth)0.9f;
            this.layer = Layer.Foreground;
            this._visibleInGame = false;
            this._editorName = "City BG";
        }

        public override void Initialize()
        {
            if (Level.current is Editor)
                return;
            this.backgroundColor = new Color(24, 0, 31);
            Level.current.backgroundColor = this.backgroundColor;
            this._parallax = new ParallaxBackground("background/city", 0f, 0f, 3);
            float speed = 0.4f;
            this._parallax.AddZone(0, 0f, -speed, true);
            this._parallax.AddZone(1, 0f, -speed, true);
            this._parallax.AddZone(2, 0f, -speed, true);
            this._parallax.AddZone(3, 0.2f, -speed, true);
            this._parallax.AddZone(4, 0.2f, -speed, true);
            this._parallax.AddZone(5, 0.4f, -speed, true);
            this._parallax.AddZone(6, 0.6f, -speed, true);
            float distance = 0.8f;
            this._parallax.AddZone(14, distance, speed);
            this._parallax.AddZone(15, distance, speed);
            this._parallax.AddZone(16, distance, speed);
            this._parallax.AddZone(17, distance, speed);
            this._parallax.AddZone(18, distance, speed);
            this._parallax.AddZone(19, distance, speed);
            this._parallax.AddZone(20, distance, speed);
            this._parallax.AddZone(21, distance, speed);
            this._parallax.AddZone(22, distance, speed);
            this._parallax.AddZone(23, distance, speed);
            this._parallax.AddZone(24, distance, speed);
            this._parallax.AddZone(25, distance, speed);
            this._parallax.AddZone(26, distance, speed);
            this._parallax.AddZone(27, distance, speed);
            this._parallax.AddZone(28, distance, speed);
            this._parallax.AddZone(29, distance, speed);
            this.layer = Layer.Parallax;
            Level.Add(_parallax);
            if (Rando.Int(200) == 0)
                this.RandomSkySay();
            this._visibleInGame = true;
            this.visible = true;
        }

        public void SkySay(string text, Vec2 spawn = default(Vec2), bool pFlyLeft = false)
        {
            List<string> stringList = new List<string>();
            string str = "";
            for (int index = 0; index < text.Length; ++index)
            {
                if (text[index] == ' ' && str.Length > 150)
                {
                    stringList.Add(str);
                    str = "";
                }
                else
                    str += text[index].ToString();
            }
            if (!string.IsNullOrWhiteSpace(str))
                stringList.Add(str);
            if (stringList.Count <= 0)
                return;
            bool flag = false;
            if (Rando.Float(1f) > 0.5f || stringList.Count > 1 || stringList[0].Length > 80)
                flag = true;
            Vec2 vec2 = new Vec2(flag ? 350f : -50f, 60f + Rando.Float(80f));
            if (spawn != Vec2.Zero)
            {
                vec2 = spawn;
                flag = pFlyLeft;
            }
            else if (Network.isActive && Network.isServer)
                Send.Message(new NMSkySay(text, vec2, flag));
            foreach (string text1 in stringList)
            {
                CityBackground.Plane plane = new CityBackground.Plane(vec2, text1, flag);
                if (flag)
                    vec2.x += (plane.textWidth * 0.5f + 80f);
                else
                    vec2.x -= (plane.textWidth * 0.5f + 80f);
                this._planes.Add(plane);
            }
        }

        public void RandomSkySay()
        {
            string text = "DUCK GAME";
            if (DateTime.Now.Day == 25 && DateTime.Now.Month == 12)
                text = "HAPPY 25TH!";
            else if (DateTime.Now.Day == 1 && DateTime.Now.Month == 1)
                text = "HAPPY NEW YEARS";
            else if (Teams.active.Count > 0 && Rando.Int(10) == 1 && !(Level.current is ChallengeLevel))
                text = "GO " + Profiles.active[Rando.Int(Profiles.active.Count - 1)].team.name.ToUpperInvariant();
            else if (Teams.active.Count > 0 && Rando.Int(500) == 1 && !(Level.current is ChallengeLevel))
                text = Profiles.active[Rando.Int(Profiles.active.Count - 1)].team.name.ToUpperInvariant() + " ARE GONNA WIN!";
            else if (Rando.Int(100) == 1)
                text = "SHOP AT VINCENT'S";
            else if (Rando.Int(150) == 1)
                text = "HEY KIDS!";
            else if (Rando.Int(150) == 1)
                text = "MOM, HELLO";
            else if (Rando.Int(15000) == 1)
                text = "I FOUND YOUR WALLET JERRY";
            else if (Rando.Int(50) == 1)
                text = "CORPTRON GAMES";
            else if (Rando.Int(1500) == 1)
                text = "ARMATURE STUDIOS";
            else if (Rando.Int(5000) == 1)
                text = "ADULT SWIM GAMES";
            else if (Rando.Int(7000) == 1)
                text = "chancy owns the sky lol";
            else if (Rando.Int(5000) == 1)
                text = "WHERE IS JOHN MALLARD";
            else if (Rando.Int(1000) == 1)
                text = "I SEE YOU";
            else if (Rando.Int(200) == 1)
                text = "LET'S DANCE";
            else if (Global.data.timesSpawned > 300 && Rando.Int(200) == 1)
                text = ":)";
            else if (Global.data.timesSpawned > 500 && Rando.Int(10000) == 1)
                text = "MAY ANGLES LEAD YOU IN";
            else if (Rando.Int(200) == 1 && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                text = "HAPPY TUESDAY!";
            else if (Global.data.timesSpawned > 100 && Rando.Int(50000) == 1)
                text = "HEY DON'T WRITE YOURSELF OFF YET IT'S ONLY IN YOUR HEAD TO FEEL LEFT OUT OR LOOKED DOWN ON JUST TRY YOUR BEST TRY EVERYTHING YOU CAN AND DON'T YOU WORRY WHAT THEY TELL THEMSELVES WHEN YOUR AWAY IT JUST TAKES SOME TIME LITTLE GIRL IN THE MIDDLE OF THE RIDE EVERYTHING EVERYTHING'L BE JUST FINE EVERYTHING EVERYTHING'L BE ALRIGHT ALRIGHT RAY YOU KNOW THEY'RE ALL THE SAME YOU KNOW YOU'RE DOING BETTER ON YOUR OWN SO DON'T BUY IN LIVE RIGHT NOW YEAH JUST BE YOURSELF IT DOESN'T MATTER IF IT'S GOOD ENOUGH FOR SOMEONE ELSE IT JUST TAKES SOME TIME LITTLE GIRL IN THE MIDDLE OF THE RIDE EVERYTHING EVERYTHING'L BE JUST FINE EVERYTHING EVERYTHING'L BE ALRIGHT ALRIGHT HEY DON'T WRITE YOURSELF OFF YET IT'S ONLY IN YOUR HEAD TO FEEL LEFT OUT OR LOOKED DOWN ON JUST DO YOUR BEST DO EVERYTHING YOU CAN AND DON'T YOU WORRY WHAT THEIR BITTER HEARTS ARE GONNA SAY IT JUST TAKES SOME TIME LITTLE GIRL IN THE MIDDLE OF THE RIDE EVERYTHING EVERYTHING'L BE JUST FINE EVERYTHING EVERYTHING'L BE ALRIGHT ALRIGHT IT JUST TAKES SOME TIME LITTLE GIRL IN THE MIDDLE OF THE RIDE EVERYTHING EVERYTHING'L BE JUST FINE EVERYTHING EVERYTHING'L BE ALRIGHT ALRIGHT";
            else if (Global.data.timesSpawned > 1000 && Rando.Int(1000000) == 1)
                text = "FUNNY STORY ONE TIME I WAS JUST CHILLING AT ZELLERS WHEN I NOTICED THE OFF BRAND CHIPS WHERE ON SALE 2 FOR ONE SO I BOUGHT SOME. WHEN I GOT HOME I OPENED THE SALT AND VINEGAR ONES AND THERE WHERE ALL DRESSED CHIPS INSIDE SO I WENT BACK TO ZELLERS AND THE CEO WAS THERE AND I TOLD HIM DIRECTLY. HE CRIED PROFUSELY BEFORE SAYING THAT NEVER SHOULD HAVE HAPPENED, THEN DECLARING THAT HIS STORE WAS A MISTAKE AND THEN HE CLOSED ALL OF THE ZELLERS. I WISH I NEVER BROUGHT MY CHIPS BACK TO ZELLERS. I MISS ZELLERS.";
            else if (Global.data.timesSpawned > 10000 && Rando.Int(1000) == 1)
                text = "WOW YOU PLAY DUCK GAME A LOT!";
            else if (Global.data.timesSpawned < 100 && Rando.Int(100) == 1)
                text = "WELCOME TO DUCK GAME!";
            if (DateTime.Now.Day == 1 && Rando.Int(1500) == 1)
                text = "HAPPY BIRTHDAY!";
            if (Rando.Int(200) == 1 && Network.isActive && DuckNetwork.core.chatMessages.Count > 0)
                text = DuckNetwork.core.chatMessages[Rando.Int(DuckNetwork.core.chatMessages.Count - 1)].text;
            this.SkySay(text);
        }

        public override void Update()
        {
            foreach (CityBackground.Plane plane in this._planes)
                plane.UpdateFlying();
            this._planes.RemoveAll(x => x.finished);
            if (!Network.isActive || Network.isServer)
            {
                this.timeSinceSkySay += Maths.IncFrameTimer();
                if (timeSinceSkySay > 30f && this._planes.Count == 0)
                {
                    if (Rando.Float(1f) > 0.5f)
                        this.RandomSkySay();
                    this.timeSinceSkySay = Rando.Float(15f);
                }
            }
            base.Update();
        }

        public override void Draw()
        {
            if (Level.current is Editor)
                base.Draw();
            foreach (CityBackground.Plane plane in this._planes)
            {
                plane.depth = (Depth)1f;
                plane.Draw();
            }
        }

        public override void Terminate() => Level.Remove(_parallax);

        private class Plane : SpriteMap
        {
            private FancyBitmapFont _font;
            private RenderTarget2D bannerTarget;
            private MaterialWiggle _wiggle;
            //private float originalY;
            //private string _text = "";
            private bool _flyLeft;
            public bool finished;

            public float textWidth => this.bannerTarget != null ? bannerTarget.width : 1f;

            public Plane(Vec2 pos, string text, bool flyLeft)
              : base("plane", 18, 13)
            {
                this._font = new FancyBitmapFont("smallFont");
                this._flyLeft = flyLeft;
                //this._text = text;
                //this.originalY = pos.y;
                this.position = pos;
                this.AddAnimation("idle", 0.8f, true, 0, 1);
                this.SetAnimation("idle");
                this.bannerTarget = new RenderTarget2D((int)(this._font.GetWidth(text) + 4f) + 8, 15);
                this._wiggle = new MaterialWiggle(this);
                Camera camera = new Camera(0f, 0f, bannerTarget.width, bannerTarget.height)
                {
                    position = Vec2.Zero
                };
                DuckGame.Graphics.SetRenderTarget(this.bannerTarget);
                DepthStencilState depthStencilState = new DepthStencilState()
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Always,
                    StencilPass = StencilOperation.Replace,
                    ReferenceStencil = 1,
                    DepthBufferEnable = false
                };
                DuckGame.Graphics.Clear(Color.Transparent);
                DuckGame.Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, depthStencilState, RasterizerState.CullNone, null, camera.getMatrix());
                DuckGame.Graphics.DrawRect(new Vec2(0f, 2f), new Vec2(this.bannerTarget.width - 8, this.bannerTarget.height - 2), Color.Black);
                this._font.Draw(text, new Vec2(1f, 3f), new Color(47, 0, 66), (Depth)1f);
                DuckGame.Graphics.screen.End();
                DuckGame.Graphics.SetRenderTarget(null);
            }

            public void UpdateFlying()
            {
                this.position.x += this._flyLeft ? -0.25f : 0.25f;
                if (this.bannerTarget != null && (this._flyLeft && this.x < -(400 + this.bannerTarget.width) || !this._flyLeft && this.x > 400 + this.bannerTarget.width))
                    this.finished = true;
                double num = -(Level.current.bottomRight.y + Level.current.topLeft.y) / 2f;
            }

            public override void Draw()
            {
                if (this.bannerTarget == null)
                    return;
                this.scale = new Vec2(0.5f, 0.5f);
                if (this._flyLeft)
                {
                    this.flipH = true;
                    base.Draw();
                    DuckGame.Graphics.material = _wiggle;
                    DuckGame.Graphics.Draw(bannerTarget, this.x + 4f, this.y, 0.5f, 0.5f, (Depth)1f);
                    DuckGame.Graphics.material = null;
                }
                else
                {
                    base.Draw();
                    DuckGame.Graphics.material = _wiggle;
                    DuckGame.Graphics.Draw(bannerTarget, this.x - (this.bannerTarget.width / 2 + 4), this.y, 0.5f, 0.5f, (Depth)1f);
                    DuckGame.Graphics.material = null;
                }
            }
        }
    }
}
