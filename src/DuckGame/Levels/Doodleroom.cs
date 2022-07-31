//// Decompiled with JetBrains decompiler
//// Type: DuckGame.Doodleroom
//// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//using Microsoft.Xna.Framework.Graphics;

//namespace DuckGame
//{
//    internal class Doodleroom : Level
//    {
//        private Texture2D _image;
//        private MonoMain.WebCharData _data;

//        public override void Initialize()
//        {
//            this._data = MonoMain.RequestRandomCharacter();
//            this.camera.width *= 2f;
//            this.camera.height *= 2f;
//            this.backgroundColor = Color.White;
//            base.Initialize();
//        }

//        public override void Update()
//        {
//            if (Keyboard.Pressed(Keys.R))
//                this._data = MonoMain.RequestRandomCharacter();
//            base.Update();
//        }

//        public override void PostDrawLayer(Layer layer)
//        {
//            if (layer == Layer.HUD && this._data != null)
//            {
//                DuckGame.Graphics.Draw(this._data.image, (float)(layer.camera.width / 2.0 - 32.0), (float)(layer.camera.height / 2.0 - 32.0), 0.5f, 0.5f, (Depth)0.5f);
//                float stringWidth1 = DuckGame.Graphics.GetStringWidth(this._data.name);
//                DuckGame.Graphics.DrawString(this._data.name, new Vec2((float)(layer.camera.width / 2.0 - stringWidth1 / 2.0), (float)(layer.camera.height / 2.0 - 43.0)), Color.Black, (Depth)1f);
//                string text = "\"" + this._data.quote + "\"";
//                float stringWidth2 = DuckGame.Graphics.GetStringWidth(text);
//                DuckGame.Graphics.DrawString(text, new Vec2((float)(layer.camera.width / 2.0 - stringWidth2 / 2.0), (float)(layer.camera.height / 2.0 + 38.0)), Color.Black, (Depth)1f);
//            }
//            base.PostDrawLayer(layer);
//        }
//    }
//}
