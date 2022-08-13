//// Decompiled with JetBrains decompiler
//// Type: DuckGame.WeaponBrowser
////removed for regex reasons Culture=neutral, PublicKeyToken=null
//// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
//// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
//// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

//namespace DuckGame
//{
//    public class WeaponBrowser : Level
//    {
//        private BitmapFont _font;

//        public override void Initialize()
//        {
//            Layer.Add(new GridBackground("GRID", 99999));
//            this._font = new BitmapFont("duckFont", 8)
//            {
//                scale = new Vec2(2f, 2f)
//            };
//            Gun gun = new Saxaphone(0f, 0f);
//            gun.scale = new Vec2(2f, 2f);
//            UIMenu uiMenu = new UIMenu(gun.editorName, Layer.HUD.camera.width / 2f, Layer.HUD.camera.height / 2f, 220f);
//            UIBox component1 = new UIBox(false, false);
//            UIImage component2 = new UIImage(gun.GetEditorImage(64, 64, true))
//            {
//                collisionSize = new Vec2(64f, 32f)
//            };
//            component1.Add(component2, true);
//            UIBox component3 = new UIBox(isVisible: false);
//            component3.Add(new UIText("AMMO: " + (gun.ammo > 900 ? "INFINITE" : gun.ammo.ToString()), Color.White, UIAlign.Left), true);
//            string str1 = "SHORT";
//            if (gun.ammoType.range > 150.0)
//                str1 = "MEDIUM";
//            if (gun.ammoType.range > 300.0)
//                str1 = "LONG";
//            if (gun.ammoType.range > 600.0)
//                str1 = "EXTREME";
//            component3.Add(new UIText("RANGE: " + str1, Color.White, UIAlign.Left), true);
//            if (gun.ammoType.penetration > 0.0)
//                component3.Add(new UIText("PENETRATION: " + gun.ammoType.penetration.ToString(), Color.White, UIAlign.Left), true);
//            else
//                component3.Add(new UIText("SPECIAL AMMO", Color.White, UIAlign.Left), true);
//            component1.Add(component3, true);
//            uiMenu.Add(component1, true);
//            UIBox component4 = new UIBox(isVisible: false);
//            component4.Add(new UIText("---------------------", Color.White), true);
//            float num = 190f;
//            string str2 = gun.bio;
//            string textVal = "";
//            string str3 = "";
//            while (true)
//            {
//                if (str2.Length > 0 && str2[0] != ' ')
//                {
//                    str3 += str2[0].ToString();
//                }
//                else
//                {
//                    if ((textVal.Length + str3.Length) * 8 > num)
//                    {
//                        component4.Add(new UIText(textVal, Color.White), true);
//                        textVal = "";
//                    }
//                    if (textVal.Length > 0)
//                        textVal += " ";
//                    textVal += str3;
//                    str3 = "";
//                }
//                if (str2.Length != 0)
//                    str2 = str2.Remove(0, 1);
//                else
//                    break;
//            }
//            if (str3.Length > 0)
//            {
//                if (textVal.Length > 0)
//                    textVal += " ";
//                textVal += str3;
//            }
//            if (textVal.Length > 0)
//                component4.Add(new UIText(textVal, Color.White), true);
//            uiMenu.Add(component4, true);
//            Level.Add(uiMenu);
//        }

//        public override void Update()
//        {
//        }

//        public override void Draw()
//        {
//        }

//        public override void PostDrawLayer(Layer layer)
//        {
//        }
//    }
//}
