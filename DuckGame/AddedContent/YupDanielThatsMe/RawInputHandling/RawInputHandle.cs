using System;
using System.Diagnostics;
//using System.Windows.Forms;
using System.Runtime.InteropServices;
using DuckGame;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Windows.Forms;

namespace RawInput
{
    public class RawInputHandle/* : NativeWindow  */
    {
        static RawKeyboardC _keyboardDriver;
        readonly IntPtr _devNotifyHandle;
        static readonly Guid DeviceInterfaceHid = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        //private PreMessageFilter _filter;

        public  event RawKeyboardC.DeviceEventHandler KeyPressed
        {
            add { _keyboardDriver.KeyPressed += value; }
            remove { _keyboardDriver.KeyPressed -= value;}
        }

        public int NumberOfKeyboards
        {
            get { return _keyboardDriver.NumberOfKeyboards; } 
        }

        //public void AddMessageFilter()
        //{
        //    if (null != _filter) return;

        //    _filter = new PreMessageFilter();
        //    //Application.AddMessageFilter(_filter);
        //}

        //private void RemoveMessageFilter()
        //{
        //    if (null == _filter) return;

        //    Application.RemoveMessageFilter(_filter);
        //}

        private IntPtr parentHandle;
        private WndProc _wndProcDelegate;
        private IntPtr _oldWndProc;

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        public static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            else
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        public RawInputHandle(IntPtr parentHandle, bool captureOnlyInForeground)
        {


            Console.WriteLine($"Window Handle: {parentHandle}");

            _keyboardDriver = new RawKeyboardC(parentHandle, false);
            _keyboardDriver.EnumerateDevices();

            //_devNotifyHandle = RegisterForDeviceNotifications(parentHandle);
            
            /* _rawinput.AddMessageFilter();  */ // Adding a message filter will cause keypresses to be handled
                                                 // Win32.DeviceAudit();            // Writes a file DeviceAudit.txt to the current directory

            //_rawinput.KeyPressed += OnKeyPressed;


            // Create a new WndProc delegate instance.
            _wndProcDelegate = WndProcHook;

            // Get the old window procedure.
            _oldWndProc = SetWindowLongPtr(parentHandle, -4, Marshal.GetFunctionPointerForDelegate(_wndProcDelegate));
            Console.WriteLine($"Old WndProc: {_oldWndProc}");

            // Verify the current window procedure
            IntPtr currentWndProc = SetWindowLongPtr(parentHandle, -4, Marshal.GetFunctionPointerForDelegate(_wndProcDelegate));
            Console.WriteLine($"Current WndProc: {currentWndProc}");

            //AssignHandle(parentHandle);



            _keyboardDriver = new RawKeyboardC(parentHandle, captureOnlyInForeground);
            _keyboardDriver.EnumerateDevices();
            _keyboardDriver.KeyPressed += OnKeyPressed;
            //_devNotifyHandle = RegisterForDeviceNotifications(parentHandle);
        }
        private byte[] internalkeystate = new byte[256];
        public static readonly Dictionary<IntPtr, byte[]> deviceinput = new Dictionary<IntPtr, byte[]>();
        public static readonly Dictionary<int, IntPtr> devicenumbermap = new Dictionary<int, IntPtr>();
        private void OnKeyPressed(object sender, RawInputEventArg e)
        {
            //lbHandle.Text = e.KeyPressEvent.DeviceHandle.ToString();
            //lbType.Text = e.KeyPressEvent.DeviceType;
            //lbName.Text = e.KeyPressEvent.DeviceName;
            //lbDescription.Text = e.KeyPressEvent.Name;
            //lbKey.Text = e.KeyPressEvent.VKey.ToString(CultureInfo.InvariantCulture);
            //lbNumKeyboards.Text = _rawinput.NumberOfKeyboards.ToString(CultureInfo.InvariantCulture);
            //lbVKey.Text = e.KeyPressEvent.VKeyName;
            //lbSource.Text = e.KeyPressEvent.Source;
            //lbKeyPressState.Text = e.KeyPressEvent.KeyPressState;
            //lbMessage.Text = string.Format("0x{0:X4} ({0})", e.KeyPressEvent.Message);

            //DevConsole.Log(e.KeyPressEvent.DeviceName);
            //DevConsole.Log(e.KeyPressEvent.DeviceHandle.ToString());ffffffsdfse

            //e.KeyPressEvent.DeviceHandle
            if (!deviceinput.ContainsKey(e.KeyPressEvent.DeviceHandle))
            {
                deviceinput.Add(e.KeyPressEvent.DeviceHandle,new byte[256]);
                try
                {
                    int keyboardnumber = int.Parse(e.KeyPressEvent.Source.Replace("Keyboard_", ""));
                    devicenumbermap.Add(keyboardnumber, e.KeyPressEvent.DeviceHandle);
                }
                catch
                {

                }
               
            }
            try
            {
                int keyboardnumber = int.Parse(e.KeyPressEvent.Source.Replace("Keyboard_", ""));
                devicenumbermap[keyboardnumber] = e.KeyPressEvent.DeviceHandle;
            }
            catch
            {

            }
            if (DansTestArea.showkeys)
            {
                DevConsole.Log("-------------------------");
                DevConsole.Log(e.KeyPressEvent.Source + " : " + e.KeyPressEvent.Name.Split('.')[0]);
                DevConsole.Log(e.KeyPressEvent.VKey.ToString(CultureInfo.InvariantCulture) + " " + e.KeyPressEvent.VKeyName + " " + ((DuckGame.Keys)e.KeyPressEvent.VKey).ToString());
                DevConsole.Log(e.KeyPressEvent.DGKey.ToString() + " : " + e.KeyPressEvent.makeCode.ToString() + " : " + e.KeyPressEvent.isE0BitSet.ToString());
                //DevConsole.Log(_rawinput.NumberOfKeyboards.ToString(CultureInfo.InvariantCulture));
                //DevConsole.Log(e.KeyPressEvent.VKeyName);
                //DevConsole.Log(e.KeyPressEvent.Source);
            }
            if (e.KeyPressEvent.KeyPressState == "MAKE")
            {

                deviceinput[e.KeyPressEvent.DeviceHandle][e.KeyPressEvent.VKey] = 0x1;
                if (DansTestArea.showkeys)
                    DevConsole.Log("Key Down");
            }
            else if (e.KeyPressEvent.KeyPressState == "BREAK")
            {
                deviceinput[e.KeyPressEvent.DeviceHandle][e.KeyPressEvent.VKey] = 0x0;
                if (DansTestArea.showkeys)
                    DevConsole.Log("Key Up");
            }
            
            //DevConsole.Log(string.Format("0x{0:X4} ({0})", e.KeyPressEvent.Message));


        }
        public static KeyboardState GetState(IntPtr intPtr)
        {
            //IntPtr targetkeyboard = (IntPtr)0x3f2f1213;
            List<Microsoft.Xna.Framework.Input.Keys> keys = new List<Microsoft.Xna.Framework.Input.Keys>();
            if (deviceinput.ContainsKey(intPtr))
            {
                for (int i = 0; i < deviceinput[intPtr].Length; i++)
                {
                    byte b = deviceinput[intPtr][i];
                    Microsoft.Xna.Framework.Input.Keys key = (Microsoft.Xna.Framework.Input.Keys)i;
                    if (b == 0x1 && !keys.Contains(key))
                    {
                        keys.Add(key);
                    }
                }
            }

            return new KeyboardState(keys.ToArray());
        }
        public static KeyboardState GetStates()
        {
            //IntPtr targetkeyboard = (IntPtr)0x3f2f1213;
            List<Microsoft.Xna.Framework.Input.Keys> keys = new List<Microsoft.Xna.Framework.Input.Keys>();
            foreach(IntPtr id in deviceinput.Keys)
            {
                //if (id != targetkeyboard)
                //{
                //    continue;
                //}
                for (int i = 0; i < deviceinput[id].Length; i++)
                {
                    byte b = deviceinput[id][i];
                    Microsoft.Xna.Framework.Input.Keys key = (Microsoft.Xna.Framework.Input.Keys)i;
                    if (b == 0x1 && !keys.Contains(key))
                    {
                        keys.Add(key);
                    }
                }
            }
           
            return new KeyboardState(keys.ToArray());
        }
        static IntPtr RegisterForDeviceNotifications(IntPtr parent)
        {
            IntPtr usbNotifyHandle = IntPtr.Zero;
            BroadcastDeviceInterface bdi = new BroadcastDeviceInterface();
            bdi.DbccSize = Marshal.SizeOf(bdi);
            bdi.BroadcastDeviceType = BroadcastDeviceType.DBT_DEVTYP_DEVICEINTERFACE;
            bdi.DbccClassguid = DeviceInterfaceHid;

            IntPtr mem = IntPtr.Zero;
            try
            {
                mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BroadcastDeviceInterface)));
                Marshal.StructureToPtr(bdi, mem, false);
                usbNotifyHandle = Win32.RegisterDeviceNotification(parent, mem, DeviceNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            catch (Exception e)
            {
                //Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
                //Debug.Print(e.StackTrace);
            }
            finally
            {
                Marshal.FreeHGlobal(mem);
            }

            //if (usbNotifyHandle == IntPtr.Zero)
            //{
            //    Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
            //}
            
            return usbNotifyHandle;
        }

        private IntPtr WndProcHook(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            // Handle window messages here...
            //if (Win32.WM_IME_SETCONTEXT == msg)
            //{
            //    return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
            //}
            //if (msg != 0x007F && msg != 512 && msg != Win32.WM_INPUT && msg != 8 && msg != 70 && msg != 71 && msg != 274 && msg != 642 && msg != 29 && msg != 28 && msg != 160 && msg != 132 && msg != 32 && msg != 257 && msg != 258 && msg != 256 && msg != 675 && msg != 674 && msg != 33 && msg != 134 && msg != 522 && msg != 6 && msg != 161)
            //{
            //    DevConsole.Log($"Received message: {msg}");
            //}
            // Convert the message to an integer for comparison
            int intMsg = Convert.ToInt32(msg);
            // Handle specific messages
            switch (intMsg)
            {
                case RawInput.Win32.WM_INPUT:
                    {
                        _keyboardDriver.ProcessRawInput(lParam);
                    }
                    break;

                case RawInput.Win32.WM_USB_DEVICECHANGE:
                    {
                        DevConsole.Log("USB Device Arrival / Removal");
                        _keyboardDriver.EnumerateDevices();
                    }
                    break;
            }
            // Call the old window procedure to not break the existing behavior.
            return CallWindowProc(_oldWndProc, hWnd, msg, wParam, lParam);
        }

        ~RawInputHandle()
        {
            //Win32.UnregisterDeviceNotification(_devNotifyHandle);
            //RemoveMessageFilter();
        }
    }
}
