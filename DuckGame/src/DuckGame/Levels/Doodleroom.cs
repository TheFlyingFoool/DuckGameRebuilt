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
//                DuckGame.Graphics.Draw(this._data.image, (float)(layer.camera.width / 2f - 32f), (float)(layer.camera.height / 2f - 32f), 0.5f, 0.5f, (Depth)0.5f);
//                float stringWidth1 = DuckGame.Graphics.GetStringWidth(this._data.name);
//                DuckGame.Graphics.DrawString(this._data.name, new Vec2((float)(layer.camera.width / 2f - stringWidth1 / 2f), (float)(layer.camera.height / 2f - 43f)), Color.Black, (Depth)1f);
//                string text = "\"" + this._data.quote + "\"";
//                float stringWidth2 = DuckGame.Graphics.GetStringWidth(text);
//                DuckGame.Graphics.DrawString(text, new Vec2((float)(layer.camera.width / 2f - stringWidth2 / 2f), (float)(layer.camera.height / 2f + 38f)), Color.Black, (Depth)1f);
//            }
//            base.PostDrawLayer(layer);
//        }
//    }
//}
