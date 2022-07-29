//// Decompiled with JetBrains decompiler
//// Type: DuckGame.WeaponTest
//// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//namespace DuckGame
//{
//    public class WeaponTest : DuckGameTestArea
//    {
//        private System.Type[] _types;

//        public WeaponTest(Editor e, params System.Type[] types)
//          : base(e, "Content\\levels\\weaponTest.lev")
//        {
//            this._types = types;
//        }

//        public override void Initialize()
//        {
//            base.Initialize();
//            int num1 = 0;
//            int num2 = 240;
//            foreach (System.Type type in this._types)
//            {
//                Thing thing1 = Thing.Instantiate(type);
//                thing1.x = num2 + num1 * 22;
//                thing1.y = 200f;
//                Level.Add(thing1);
//                Thing thing2 = Thing.Instantiate(type);
//                thing2.x = num2 + num1 * 22 + 8;
//                thing2.y = 200f;
//                Level.Add(thing2);
//                ++num1;
//            }
//            Duck t1 = new Duck(210f, 200f, Profiles.DefaultPlayer1);
//            Level.Add(t1);
//            (Level.current as DeathmatchLevel).followCam.Add(t1);
//            Duck t2 = new Duck(400f, 200f, Profiles.DefaultPlayer2);
//            Level.Add(t2);
//            (Level.current as DeathmatchLevel).followCam.Add(t2);
//            Level.Add(new PhysicsChain(300f, 100f));
//            Level.Add(new PhysicsRope(350f, 100f));
//        }
//    }
//}
