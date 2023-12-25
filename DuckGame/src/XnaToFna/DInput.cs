// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyDInput.DInput
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace XnaToFna.ProxyDInput
{
    public static class DInput
    {
        public static bool IsProxy = Environment.GetEnvironmentVariable("XTF_PROXY_DINPUT") == "1";
        public static DInputState[] States = new DInputState[0];
        public static DInputState StateDefault = new DInputState();

        public static bool Initialize()
        {
            if (!IsProxy)
            {
                XnaToFnaHelper.Log("[ProxyDInput] ProxyDInput disabled by default - 'export XTF_PROXY_DINPUT=1' to enable");
                return false;
            }
            XnaToFnaHelper.Log("[ProxyDInput] Initializing ProxyDInput");
            States = new DInputState[XnaToFnaHelper.MaximumGamepadCount];
            for (int index = 0; index < States.Length; ++index)
                States[index] = new DInputState();
            return true;
        }

        public static void Terminate()
        {
        }

        public static void EnumGamepads()
        {
        }

        public static void Update()
        {
            for (int index = 0; index < States.Length; ++index)
            {
                GamePadState state1 = GamePad.GetState(index, GamePadDeadZone.IndependentAxes);
                DInputState state2 = States[index];
                state2.connected = state1.IsConnected;
                if (state2.connected)
                {
                    GamePadThumbSticks thumbSticks = state1.ThumbSticks;
                    state2.leftX = thumbSticks.Left.X;
                    state2.leftY = thumbSticks.Left.Y;
                    state2.leftZ = 0.0f;
                    state2.rightX = thumbSticks.Right.X;
                    state2.rightY = thumbSticks.Right.Y;
                    state2.rightZ = 0.0f;
                    GamePadTriggers triggers = state1.Triggers;
                    state2.slider1 = state1.Triggers.Left;
                    state2.slider2 = state1.Triggers.Right;
                    GamePadDPad dpad = state1.DPad;
                    state2.left = dpad.Left == ButtonState.Pressed;
                    state2.right = dpad.Right == ButtonState.Pressed;
                    state2.up = dpad.Up == ButtonState.Pressed;
                    state2.down = dpad.Down == ButtonState.Pressed;
                    GamePadButtons buttons = state1.Buttons;
                    List<bool> boolList = state2.buttons ?? new List<bool>();
                    for (int count = boolList.Count; count < 13; ++count)
                        boolList.Add(false);
                    while (boolList.Count > 13)
                        boolList.RemoveAt(0);
                    boolList[0] = buttons.X == ButtonState.Pressed;
                    boolList[1] = buttons.A == ButtonState.Pressed;
                    boolList[2] = buttons.B == ButtonState.Pressed;
                    boolList[3] = buttons.Y == ButtonState.Pressed;
                    boolList[4] = buttons.LeftShoulder == ButtonState.Pressed;
                    boolList[5] = buttons.RightShoulder == ButtonState.Pressed;
                    boolList[6] = (double)triggers.Left >= 0.999000012874603;
                    boolList[7] = (double)triggers.Right >= 0.999000012874603;
                    boolList[8] = buttons.Back == ButtonState.Pressed;
                    boolList[9] = buttons.Start == ButtonState.Pressed;
                    boolList[10] = buttons.BigButton == ButtonState.Pressed;
                    boolList[11] = buttons.LeftStick == ButtonState.Pressed;
                    boolList[12] = buttons.RightStick == ButtonState.Pressed;
                    state2.buttons = boolList;
                }
            }
        }

        public static DInputState GetState(int player) => player < States.Length ? States[player] : StateDefault;

        public static string GetProductName(int player) => player >= States.Length ? string.Empty : string.Format("ProxyDInput #{0}", player + 1);

        public static string GetProductGUID(int player) => player < States.Length ? GamePad.GetGUIDEXT(player) : string.Empty;
    }
}
