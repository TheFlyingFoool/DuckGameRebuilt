//// Decompiled with JetBrains decompiler
//// Type: DuckGame.BuyScreen
//// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//using System;
//using System.Globalization;

//namespace DuckGame
//{
//    public class BuyScreen : Level
//    {
//        private BitmapFont _font;
//        private Sprite _payScreen;
//        private SpriteMap _moneyType;
//        private bool _buy;
//        private bool _demo;
//        private float _wave;
//        private bool _fade;
//        private string _currencyType = "USD";
//        private string _currencyCharacter = "$";
//        private float _price;

//        public BuyScreen(string currency, float price)
//        {
//            this._centeredView = true;
//            this._currencyType = currency;
//            this._price = price;
//        }

//        public override void Initialize()
//        {
//            Graphics.fade = 0.0f;
//            this._payScreen = new Sprite("payScreen");
//            this._payScreen.CenterOrigin();
//            this._moneyType = new SpriteMap("moneyTypes", 14, 18);
//            this._font = new BitmapFont("moneyFont", 8);
//            if (this._currencyType == "USD")
//            {
//                this._currencyCharacter = "$";
//                this._moneyType.frame = 0;
//            }
//            else if (this._currencyType == "EUR")
//            {
//                this._currencyCharacter = "%";
//                this._moneyType.frame = 1;
//            }
//            else if (this._currencyType == "GBP")
//            {
//                this._currencyCharacter = "&";
//                this._moneyType.frame = 2;
//            }
//            HUD.AddCornerControl(HUDCorner.BottomLeft, "@DPAD@Select");
//            HUD.AddCornerControl(HUDCorner.BottomRight, "@SELECT@Confirm");
//            base.Initialize();
//        }

//        public override void Update()
//        {
//            if (this._fade)
//            {
//                Graphics.fade = Lerp.Float(Graphics.fade, 0.0f, 0.02f);
//                if ((double)Graphics.fade > 0.0)
//                    return;
//                Main.isDemo = this._demo;
//                Level.current = new TitleScreen();
//            }
//            else
//            {
//                Graphics.fade = Lerp.Float(Graphics.fade, 1f, 0.02f);
//                this._wave += 0.1f;
//                if (Input.Pressed("MENUUP"))
//                {
//                    this._buy = true;
//                    SFX.Play("textLetter", 0.9f);
//                }
//                if (Input.Pressed("MENUDOWN"))
//                {
//                    this._buy = false;
//                    SFX.Play("textLetter", 0.9f);
//                }
//                if (!Input.Pressed("SELECT"))
//                    return;
//                if (this._buy)
//                {
//                    this._fade = true;
//                    this._demo = false;
//                }
//                else
//                {
//                    this._fade = true;
//                    this._demo = true;
//                }
//                SFX.Play("rockHitGround", 0.9f);
//            }
//        }

//        public override void PostDrawLayer(Layer layer)
//        {
//            if (layer == Layer.Game)
//            {
//                this._payScreen.depth = (Depth)0.5f;
//                this._moneyType.depth = (Depth)0.6f;
//                Graphics.Draw(this._payScreen, layer.width / 2f, layer.height / 2f);
//                Graphics.Draw(_moneyType, (float)((double)layer.width / 2.0 - 79.0), (float)((double)layer.height / 2.0 - 23.0));
//                string text1 = "Buy Game (" + this._currencyCharacter + this._price.ToString("0.00", CultureInfo.InvariantCulture) + ")";
//                this._font.Draw(text1, (float)((double)layer.width / 2.0 - (double)this._font.GetWidth(text1) / 2.0 + 15.0), (float)((double)layer.height / 2.0 - 18.0), Color.White, (Depth)0.8f);
//                if (this._buy)
//                {
//                    Vec2 p1 = new Vec2((float)((double)layer.width / 2.0 - this._payScreen.width / 2 + 6.0), (float)((double)layer.height / 2.0 - 25.0));
//                    Graphics.DrawRect(p1, p1 + new Vec2(_payScreen.width - 11.5f, 22f), Color.White, (Depth)0.9f, false);
//                }
//                else
//                {
//                    Vec2 p1 = new Vec2((float)((double)layer.width / 2.0 - this._payScreen.width / 2 + 6.0), (float)((double)layer.height / 2.0 + 3.0));
//                    Graphics.DrawRect(p1, p1 + new Vec2(_payScreen.width - 11.5f, 22f), Color.White, (Depth)0.9f, false);
//                }
//                string text2 = "PLAY DEMO";
//                this._font.Draw(text2, (float)((double)layer.width / 2.0 - (double)this._font.GetWidth(text2) / 2.0 + 12.0), (float)((double)layer.height / 2.0 + 10.0), Color.White, (Depth)0.8f);
//            }
//            base.PostDrawLayer(layer);
//        }
//    }
//}
