//namespace DuckGame
//{
//    internal class CrashLogRoom : Level
//    {
//        private FancyBitmapFont _font = new FancyBitmapFont("smallFont");
//        private string _error;

//        public CrashLogRoom(string error)
//        {
//            this._error = error;
//            this._centeredView = true;
//        }

//        public override void Initialize()
//        {
//            this._startCalled = true;
//            base.Initialize();
//        }

//        public override void Draw()
//        {
//            this._font.scale = new Vec2(0.5f, 0.5f);
//            this._font.Draw(this._error, new Vec2(30f, 30f), Color.White);
//        }
//    }
//}
