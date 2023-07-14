using System;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DuckGame;
using Debug = System.Diagnostics.Debug;
using Microsoft.Xna.Framework.Input;

namespace RawInput
{
	public sealed class RawKeyboardC
	{
		private readonly Dictionary<IntPtr,KeyPressEvent> _deviceList = new Dictionary<IntPtr,KeyPressEvent>();
		public delegate void DeviceEventHandler(object sender, RawInputEventArg e);
		public event DeviceEventHandler KeyPressed;
		readonly object _padLock = new object();
		public int NumberOfKeyboards { get; private set; }
		static InputData _rawBuffer;

		public RawKeyboardC(IntPtr hwnd, bool captureOnlyInForeground)
		{
            RawInputDevice[] rid = new RawInputDevice[1];

			rid[0].UsagePage = HidUsagePage.GENERIC;       
			rid[0].Usage = HidUsage.Keyboard;              
            rid[0].Flags = (captureOnlyInForeground ? RawInputDeviceFlags.NONE : RawInputDeviceFlags.INPUTSINK) | RawInputDeviceFlags.DEVNOTIFY;
			rid[0].Target = hwnd;

			if(!Win32.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
			{
				throw new ApplicationException("Failed to register raw input device(s).");
			}
		}

		public void EnumerateDevices()
		{
			lock (_padLock)
			{
				_deviceList.Clear();

                int keyboardNumber = 0;

                KeyPressEvent globalDevice = new KeyPressEvent
				{
					DeviceName = "Global Keyboard",
					DeviceHandle = IntPtr.Zero,
					DeviceType = Win32.GetDeviceType(DeviceType.RimTypekeyboard),
					Name = "Fake Keyboard. Some keys (ZOOM, MUTE, VOLUMEUP, VOLUMEDOWN) are sent to rawinput with a handle of zero.",
					Source = keyboardNumber++.ToString(CultureInfo.InvariantCulture)
				};

				_deviceList.Add(globalDevice.DeviceHandle, globalDevice);

                int numberOfDevices = 0;
				uint deviceCount = 0;
                int dwSize = (Marshal.SizeOf(typeof(Rawinputdevicelist)));

				if (Win32.GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
				{
                    IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
					Win32.GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

					for (int i = 0; i < deviceCount; i++)
					{
						uint pcbSize = 0;

                        // On Window 8 64bit when compiling against .Net > 3.5 using .ToInt32 you will generate an arithmetic overflow. Leave as it is for 32bit/64bit applications
                        Rawinputdevicelist rid = (Rawinputdevicelist)Marshal.PtrToStructure(new IntPtr((pRawInputDeviceList.ToInt64() + (dwSize * i))), typeof(Rawinputdevicelist));

						Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

						if (pcbSize <= 0) continue;

                        IntPtr pData = Marshal.AllocHGlobal((int)pcbSize);
						Win32.GetRawInputDeviceInfo(rid.hDevice, RawInputDeviceInfo.RIDI_DEVICENAME, pData, ref pcbSize);
                        string deviceName = Marshal.PtrToStringAnsi(pData);

                        if (rid.dwType == DeviceType.RimTypekeyboard || rid.dwType == DeviceType.RimTypeHid)
						{
                            string deviceDesc = Win32.GetDeviceDescription(deviceName);

                            KeyPressEvent dInfo = new KeyPressEvent
							{
								DeviceName = Marshal.PtrToStringAnsi(pData),
								DeviceHandle = rid.hDevice,
								DeviceType = Win32.GetDeviceType(rid.dwType),
								Name = deviceDesc,
								Source = keyboardNumber++.ToString(CultureInfo.InvariantCulture)
							};
						   
							if (!_deviceList.ContainsKey(rid.hDevice))
							{
								numberOfDevices++;
								_deviceList.Add(rid.hDevice, dInfo);
							}
						}

						Marshal.FreeHGlobal(pData);
					}

					Marshal.FreeHGlobal(pRawInputDeviceList);

					NumberOfKeyboards = numberOfDevices;
					Debug.WriteLine("EnumerateDevices() found {0} Keyboard(s)", NumberOfKeyboards);
					return;
				}
			}
			
			throw new Win32Exception(Marshal.GetLastWin32Error());
		}
        public void ProcessRawInput(IntPtr hdevice)
		{
			//Debug.WriteLine(_rawBuffer.data.keyboard.ToString());
			//Debug.WriteLine(_rawBuffer.data.hid.ToString());
			//Debug.WriteLine(_rawBuffer.header.ToString());

			if (_deviceList.Count == 0) return;

            int dwSize = 0;
			Win32.GetRawInputData(hdevice, DataCommand.RID_INPUT, IntPtr.Zero, ref dwSize, Marshal.SizeOf(typeof(Rawinputheader)));

			if (dwSize != Win32.GetRawInputData(hdevice, DataCommand.RID_INPUT, out _rawBuffer, ref dwSize, Marshal.SizeOf(typeof (Rawinputheader))))
			{
				Debug.WriteLine("Error getting the rawinput buffer");
				return;
			}

			int virtualKey = _rawBuffer.data.keyboard.VKey;
			int makeCode = _rawBuffer.data.keyboard.Makecode;
			int flags = _rawBuffer.data.keyboard.Flags;

			if (virtualKey == Win32.KEYBOARD_OVERRUN_MAKE_CODE) return;

            bool isE0BitSet = ((flags & Win32.RI_KEY_E0) != 0);

			KeyPressEvent keyPressEvent;

			if (_deviceList.ContainsKey(_rawBuffer.header.hDevice))
			{
				lock (_padLock)
				{
					keyPressEvent = _deviceList[_rawBuffer.header.hDevice];
				}
			}
			else
			{
				Debug.WriteLine("Handle: {0} was not in the device list.", _rawBuffer.header.hDevice);
				return;
			}

            bool isBreakBitSet = ((flags & Win32.RI_KEY_BREAK) != 0);
			
			keyPressEvent.KeyPressState = isBreakBitSet ? "BREAK" : "MAKE"; 
			keyPressEvent.Message = _rawBuffer.data.keyboard.Message;
			keyPressEvent.VKeyName = KeyMapper.GetKeyName(VirtualKeyCorrection(virtualKey, isE0BitSet, makeCode)).ToUpper();
            keyPressEvent.VKey = virtualKey;
            keyPressEvent.makeCode = makeCode;
            keyPressEvent.isE0BitSet = isE0BitSet;
            switch (keyPressEvent.VKey)
            {
                case 16: // shift
                    if (makeCode == 54)
                    {
                        keyPressEvent.DGKey = DuckGame.Keys.RightShift;
                    }
                    else if (makeCode == 42)
                    {
                        keyPressEvent.DGKey = DuckGame.Keys.LeftShift;
                    }
                    else
                    {
                        keyPressEvent.DGKey = (DuckGame.Keys)16; // unsure what would hit this case but just in case
                    }
                    break;
                case 17: // control
                    if (isE0BitSet)
                    {
                        keyPressEvent.DGKey = DuckGame.Keys.RightControl;
                    }
                    else
                    {
                        keyPressEvent.DGKey = DuckGame.Keys.LeftControl;
                    }
                    break;
                case 18: //alt
                    if (isE0BitSet)
                    {
                        keyPressEvent.DGKey = DuckGame.Keys.RightAlt;
                    }
                    else
                    {
                        keyPressEvent.DGKey = DuckGame.Keys.LeftAlt;
                    }
                    break;

                default:
                    keyPressEvent.DGKey = (DuckGame.Keys)keyPressEvent.VKey;
                    break;

            }
			if (KeyPressed != null)
			{
				KeyPressed(this, new RawInputEventArg(keyPressEvent));
			}
		}

		private static int VirtualKeyCorrection(int virtualKey, bool isE0BitSet, int makeCode)
		{
            int correctedVKey = virtualKey;

			if (_rawBuffer.header.hDevice == IntPtr.Zero)
			{
				// When hDevice is 0 and the vkey is VK_CONTROL indicates the ZOOM key
				if (_rawBuffer.data.keyboard.VKey == Win32.VK_CONTROL)
				{
					correctedVKey = Win32.VK_ZOOM;
				}
			}
			else
			{
				switch (virtualKey)
				{
					// Right-hand CTRL and ALT have their e0 bit set 
					case Win32.VK_CONTROL:
						correctedVKey = isE0BitSet ? Win32.VK_RCONTROL : Win32.VK_LCONTROL;
						break;
					case Win32.VK_MENU:
						correctedVKey = isE0BitSet ? Win32.VK_RMENU : Win32.VK_LMENU;
						break;
					case Win32.VK_SHIFT:
						correctedVKey = makeCode == Win32.SC_SHIFT_R ? Win32.VK_RSHIFT : Win32.VK_LSHIFT;
						break;
					default:
						correctedVKey = virtualKey;
						break;
				}
			}

			return correctedVKey;
		}
	}
}
