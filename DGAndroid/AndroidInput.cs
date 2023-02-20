using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Keycode = Android.Views.Keycode;

namespace DuckGame
{
    internal class AndroidInput : InputProfile
    {
        public static AndroidInput instance;
        public static TouchCollection State;
        public static bool First;
        public static bool NotTouching;
        static List<AButton> Buttons = new List<AButton>();
        static bool Inited;

        static int MoveStickId = -1;
        static Vec2 MoveStickPos;
        static Rectangle MoveStickRect;

        static bool SwitchInput;
        static Rectangle SwitchRect;
        static int SwitchWait;

        public AndroidInput()
        {
            instance = this;
        }

        static AndroidInput()
        {
            var rect = Graphics.viewport;
            float h = rect.Height / 2;
            float w = rect.Width / 2 - 40;
            float s = 40;
            MoveStickRect = new Rectangle(0, h - h / 3, h / 3, h / 3);
            SwitchRect = new Rectangle(0, 0, s, s);
            Buttons = new List<AButton>()
            {
                new AButton("LEFT", Keycode.A, new Rectangle(0, h - s * 2, s, s)),
                new AButton("DOWN",Keycode.S, new Rectangle(s, h - s, s, s)),
                new AButton("RIGHT",Keycode.D, new Rectangle(s * 2 ,h - s * 2, s, s)),
                new AButton("UP",Keycode.W,new Rectangle(s, h - s * 3, s, s)),
                new AButton("LEFT","UP",Keycode.W,new Rectangle(0, h - s * 3, s, s)),
                new AButton("RIGHT","UP",Keycode.W,new Rectangle(s * 2, h - s * 3, s, s)),
                new AButton("LEFT","DOWN",Keycode.W,new Rectangle(0,h - s, s, s)),
                new AButton("RIGHT","DOWN",Keycode.W,new Rectangle(s * 2,h - s, s, s)),

                new AButton("START",Keycode.Enter, new Rectangle(w / 2 - s * 2, h - s, s, s)),
                new AButton("CANCEL",Keycode.Enter, new Rectangle(w / 2,h - s,s, s)),
                new AButton("SELECT",Keycode.ShiftLeft, new Rectangle(w / 2 + s * 2,h - s,s, s)),

                new AButton("JUMP",Keycode.Space, new Rectangle(w - s * 2,h - s,s, s)),
                new AButton("SHOOT",Keycode.Space, new Rectangle(w - s,h - s,s, s)),
                new AButton("GRAB",Keycode.Space, new Rectangle(w,h - s,s, s)),
                new AButton("QUACK",Keycode.Space, new Rectangle(w,h - s * 2,s, s)),

                new AButton("MENU1",Keycode.Space, new Rectangle(w - s / 2 * 2,0,s, s)),
                new AButton("MENU2",Keycode.Space, new Rectangle(w,0,s, s)),
            };
        }

        //Bugged! Letter keys are released instanly after pressing making them unusable
        public static bool HardwareKeyboard = false;

        public static GestureSample sample;
        public static bool Gestured;
        Vec2 _leftStick;
        public override Vec2 leftStick => _leftStick;

        public static void Update()
        {
            if (HardwareKeyboard)
            {
                if (Keyboard.Pressed(Keys.A))
                {
                    MainActivity.Pressed.Add(Android.Views.Keycode.A);
                }
                foreach (var b in Buttons)
                {
                    b.State = InputState.Released;
                }
                foreach (var k in MainActivity.Pressed)
                {
                    First = true;
                    foreach (var b in Buttons)
                    {
                        if (b.Key == k)
                        {
                            b.State = InputState.Pressed;
                        }
                        if (b.State == InputState.Pressed && b.PrevState == InputState.Released)
                        {
                            b.PrevState = b.State;
                            b.State = InputState.Pressed;
                            continue;
                        }
                        else if (b.State == InputState.Pressed && b.PrevState == InputState.Pressed)
                        {
                            b.PrevState = b.State;
                            b.State = InputState.Down;
                            continue;
                        }
                    }
                }
                return;
            }
            State = TouchPanel.GetState();
            if (--SwitchWait <= 0)
            {
                foreach (var touch in State)
                {
                    var pos = ToScreen(touch.Position);//Layer.Console.camera.transformScreenVector(touch.Position);
                    if (SwitchRect.Contains((pos)))
                    {
                        SwitchInput = !SwitchInput;
                        SwitchWait = 60;
                        break;
                    }
                }
            }
            bool idSame = false;
            int idTemp = -1;
            Vec2 newPos = default;
            Vec2 posTemp = default;
            int ignoreIndex = -1;
            if (SwitchInput)
            {
                for (int i = 0; i < State.Count; i++)
                {
                    TouchLocation touch = State[i];
                    var pos = ToScreen(touch.Position);//Layer.Console.camera.transformScreenVector(touch.Position);
                    //pos.x += 40;
                    if (touch.Id == MoveStickId)
                    {
                        idSame = true;
                        newPos = pos;
                        ignoreIndex = i;
                        break;
                    }
                    if (MoveStickRect.Contains(pos))
                    {
                        idTemp = touch.Id;
                        posTemp = pos;
                        ignoreIndex = i;
                    }
                }

                if (!idSame && idTemp != -1)
                {
                    MoveStickId = idTemp;
                    MoveStickPos = posTemp;
                }
                else if (!idSame)
                {
                    MoveStickId = -1;
                }
                else
                {
                    MoveStickPos = newPos;
                }
                if (MoveStickId >= 0)
                {
                    if (Maths.Intersects(MoveStickRect.Center, MoveStickPos, MoveStickRect.tl, MoveStickRect.tr, out Vec2 inter))
                    {
                        MoveStickPos = inter;
                    }
                    else if (Maths.Intersects(MoveStickRect.Center, MoveStickPos, MoveStickRect.bl, MoveStickRect.br, out Vec2 inter2))
                    {
                        MoveStickPos = inter2;
                    }
                    else if (Maths.Intersects(MoveStickRect.Center, MoveStickPos, MoveStickRect.tl, MoveStickRect.bl, out Vec2 inter3))
                    {
                        MoveStickPos = inter3;
                    }
                    else if (Maths.Intersects(MoveStickRect.Center, MoveStickPos, MoveStickRect.tr, MoveStickRect.br, out Vec2 inter4))
                    {
                        MoveStickPos = inter4;
                    }
                    instance._leftStick = new Vec2(2 * (MoveStickPos.x - MoveStickRect.Left) / (MoveStickRect.Right - MoveStickRect.Left) - 1,
                        2 * (MoveStickPos.y - MoveStickRect.Bottom) / (MoveStickRect.Top - MoveStickRect.Bottom) - 1);
                }
                else
                {
                    instance._leftStick = new Vec2(0, 0);
                }
                float sec = MoveStickRect.height / 3 + MoveStickRect.Top;
                float third = (MoveStickRect.height / 3) * 2 + MoveStickRect.Top;
                if (MoveStickPos.y > sec && MoveStickPos.y < third)// middle
                {
                    Buttons[1].State = InputState.Released;
                    Buttons[3].State = InputState.Released;
                } 
                else if(MoveStickPos.y > third) // bottom
                {
                    Buttons[1].State = InputState.Down;
                    Buttons[3].State = InputState.Released;
                }
                else // top
                {
                    Buttons[1].State = InputState.Released;
                    Buttons[3].State = InputState.Down;
                }
            }
            else
            {
                instance._leftStick = new Vec2(0, 0);
                Buttons[1].State = InputState.Released;
                Buttons[3].State = InputState.Released;
            }
            foreach (var b in Buttons.Skip(SwitchInput ? 8 : 0))
            {
                bool set = false;
                for (int i = 0; i < State.Count; i++)
                {
                    TouchLocation touch = State[i];
                    First = true;
                    if (ignoreIndex == i) continue;
                    var pos = ToScreen(touch.Position);
                    if (b.Rect.Contains(pos))
                    {
                        b.State = InputState.Pressed;
                    }
                    if (b.State == InputState.Pressed && b.PrevState == InputState.Released)
                    {
                        b.PrevState = b.State;
                        b.State = InputState.Pressed;
                        set = true;
                        continue;
                    }
                    else if (b.State == InputState.Pressed && b.PrevState == InputState.Pressed)
                    {
                        b.PrevState = b.State;
                        b.State = InputState.Down;
                        set = true;
                        continue;
                    }
                }
                if (set) continue;
                b.State = InputState.Released;
                b.PrevState = InputState.Released;
            }
        }
        public static Vec2 ToScreen(Vec2 pos)
        {
            var cam = Layer.Console.camera;
            var dim = MainActivity.GetDimensions();
            var dx = MathHelper.Lerp(cam.x, cam.width, pos.x / (dim.Item1 - 140));
            var dy = MathHelper.Lerp(cam.y, cam.height, pos.y / dim.Item2);
            return new Vec2(dx, dy);
        }

        public static string MenuToTrigger(string trigger)
        {
            switch (trigger)
            {
                case "MENULEFT":
                    return "LEFT";
                    break;
                case "MENUDOWN":
                    return "DOWN";
                    break;
                case "MENURIGHT":
                    return "RIGHT";
                    break;
                case "MENUUP":
                    return "UP";
                    break;
                default:
                    return trigger;
            }
        }

        public override bool Pressed(string trigger, bool any = false)
        {
            if (NotTouching) return false;
            if (_repeatList.Contains(trigger))
            {
                return true;
            }
            trigger = MenuToTrigger(trigger);
            var b = Buttons.FindAll(x => x.trigger == trigger || x.trigger2 == trigger);
            return b != null && b.Any(x => x.State == InputState.Pressed);
        }

        public override bool Down(string trigger)
        {
            if (NotTouching) return false;
            trigger = MenuToTrigger(trigger);
            var b = Buttons.FindAll(x => x.trigger == trigger || x.trigger2 == trigger);
            return b != null && b.Any(x => x.State == InputState.Down);
        }

        public override bool Released(string trigger)
        {
            if (NotTouching) return true;
            trigger = MenuToTrigger(trigger);
            var b = Buttons.FindAll(x => x.trigger == trigger || x.trigger2 == trigger);
            return b != null && b.Any(x => x.State == InputState.Released);
        }
        public static void Draw()
        {
            if (HUD.hide) return;
            if (!First) return;
            int i = 0;
            //foreach (var k in MainActivity.Pressed)
            //{
            //    Graphics.DrawString(k.ToString(), new Vec2(i + 5, 20), Color.White);
            //    i += (int)Graphics.GetStringWidth(k.ToString());
            //}
            //Graphics.DrawString( $"x:{instance._leftStick.x:F1} y:{instance._leftStick.y:F1}", MoveStickRect.tl + new Vec2(5, -20), Color.Blue);
            Graphics.DrawRect(SwitchRect, Color.Blue, 0, SwitchInput);
            if (SwitchInput)
            {
                Graphics.DrawRect(MoveStickRect, Color.Blue, 0, false);
                if (MoveStickId >= 0)
                {
                    Graphics.DrawCircle(MoveStickPos, 20, Color.AliceBlue);
                }
                else
                {
                    Graphics.DrawCircle(MoveStickRect.Center, 20, Color.AliceBlue);
                }
            }
            foreach (var b in Buttons.Skip(SwitchInput ? 8 : 0))
            {
                var r = b.Rect;
                Graphics.DrawRect(r, Color.Blue, 0, !NotTouching && b.State == InputState.Down);
            }
            if (Gestured)
            {
                Graphics.DrawLine(sample.Position, sample.Position2, Color.White);
            }
            if (State.Count == 0) return;
            Vec2 tpos = Vec2.Zero;
            foreach (var s in State)
            {
                Graphics.DrawCircle(ToScreen(s.Position), 20, Color.White, 1, 0.001f);
            }
        }
    }

    internal class AButton
    {
        public InputState State = InputState.Released;
        public InputState PrevState = InputState.Released;
        public string trigger;
        public string trigger2;
        public bool Single;
        public Rectangle Rect;
        public Keycode Key;

        public AButton(string trigger, Keycode key, Rectangle rect)
        {
            this.trigger = trigger;
            this.trigger2 = null;
            Rect = rect;
            Key = key;
            Single = true;
        }

        public AButton(string trigger, string trigger2, Keycode key, Rectangle rect)
        {
            this.trigger = trigger;
            this.trigger2 = trigger2;
            Rect = rect;
            Key = key;
        }
    }
}