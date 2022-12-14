// Decompiled with JetBrains decompiler
// Type: XnaToFna.DeviceEvents
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Reflection;
using XnaToFna.ProxyForms;

namespace XnaToFna
{
    public static class DeviceEvents
    {
        public static bool[] IsGamepadConnected = new bool[0];

        public static void DeviceChange(Events e, IntPtr data) => PInvoke.CallHooks(Messages.WM_DEVICECHANGE, (IntPtr)(int)e, data, allWindows: true);

        public static void GamepadConnected(int i) => DeviceChange(Events.DBT_DEVICEARRIVAL, IntPtr.Zero);

        public static void GamepadDisconnected(int i) => DeviceChange(Events.DBT_DEVICEREMOVECOMPLETE, IntPtr.Zero);

        public static void Update()
        {
            for (int i = 0; i < IsGamepadConnected.Length; ++i)
            {
                bool isConnected = FNAPlatform.GetGamePadState(i, GamePadDeadZone.IndependentAxes).IsConnected;
                if (isConnected && !IsGamepadConnected[i])
                    GamepadConnected(i);
                else if (!isConnected && IsGamepadConnected[i])
                    GamepadDisconnected(i);
                IsGamepadConnected[i] = isConnected;
            }
        }

        public enum Events
        {
            DBT_DEVNODES_CHANGED = 7,
            DBT_QUERYCHANGECONFIG = 23, // 0x00000017
            DBT_CONFIGCHANGED = 24, // 0x00000018
            DBT_CONFIGCHANGECANCELED = 25, // 0x00000019
            DBT_DEVICEARRIVAL = 32768, // 0x00008000
            DBT_DEVICEQUERYREMOVE = 32769, // 0x00008001
            DBT_DEVICEQUERYREMOVEFAILED = 32770, // 0x00008002
            DBT_DEVICEREMOVEPENDING = 32771, // 0x00008003
            DBT_DEVICEREMOVECOMPLETE = 32772, // 0x00008004
            DBT_DEVICETYPESPECIFIC = 32773, // 0x00008005
            DBT_CUSTOMEVENT = 32774, // 0x00008006
            DBT_USERDEFINED = 65535, // 0x0000FFFF
        }
    }
}
