using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
	public class InputProfile
	{
		public static InputProfileCore core
		{
			get
			{
				return InputProfile._core;
			}
			set
			{
				InputProfile._core = value;
			}
		}

		public static string MPPlayer1
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[0];
			}
		}

		public static string MPPlayer2
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[1];
			}
		}

		public static string MPPlayer3
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[2];
			}
		}

		public static string MPPlayer4
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[3];
			}
		}

		public static string MPPlayer5
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[4];
			}
		}

		public static string MPPlayer6
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[5];
			}
		}

		public static string MPPlayer7
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[6];
			}
		}

		public static string MPPlayer8
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings[7];
			}
		}

		public static string[] MPPlayers
		{
			get
			{
				return InputProfile._defaultPlayerMappingStrings;
			}
		}

		public static void ReassignDefaultInputProfiles(bool fullRemap = false)
		{
			if (fullRemap)
			{
				Profiles.DefaultPlayer1.inputProfile = InputProfile.Get(InputProfile.MPPlayer1);
				Profiles.DefaultPlayer2.inputProfile = InputProfile.Get(InputProfile.MPPlayer2);
				Profiles.DefaultPlayer3.inputProfile = InputProfile.Get(InputProfile.MPPlayer3);
				Profiles.DefaultPlayer4.inputProfile = InputProfile.Get(InputProfile.MPPlayer4);
				Profiles.DefaultPlayer5.inputProfile = InputProfile.Get(InputProfile.MPPlayer5);
				Profiles.DefaultPlayer6.inputProfile = InputProfile.Get(InputProfile.MPPlayer6);
				Profiles.DefaultPlayer7.inputProfile = InputProfile.Get(InputProfile.MPPlayer7);
				Profiles.DefaultPlayer8.inputProfile = InputProfile.Get(InputProfile.MPPlayer8);
				return;
			}
			Profiles.DefaultPlayer1.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer1));
			Profiles.DefaultPlayer2.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer2));
			Profiles.DefaultPlayer3.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer3));
			Profiles.DefaultPlayer4.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer4));
			Profiles.DefaultPlayer5.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer5));
			Profiles.DefaultPlayer6.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer6));
			Profiles.DefaultPlayer7.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer7));
			Profiles.DefaultPlayer8.SetInputProfileLink(InputProfile.Get(InputProfile.MPPlayer8));
		}

		public static void SwapDefaultInputStrings(string from, string to)
		{
			int fromIdx = -1;
			int toIdx = -1;
			for (int i = 0; i < DG.MaxPlayers; i++)
			{
				if (InputProfile._defaultPlayerMappingStrings[i] == from)
				{
					fromIdx = i;
				}
				else if (InputProfile._defaultPlayerMappingStrings[i] == to)
				{
					toIdx = i;
				}
			}
			if (fromIdx >= 0 && toIdx >= 0)
			{
				string temp = InputProfile._defaultPlayerMappingStrings[toIdx];
				InputProfile._defaultPlayerMappingStrings[toIdx] = InputProfile._defaultPlayerMappingStrings[fromIdx];
				InputProfile._defaultPlayerMappingStrings[fromIdx] = temp;
			}
		}

		public static InputProfile active
		{
			get
			{
				return InputProfile._active;
			}
			set
			{
				InputProfile._active = value;
			}
		}

		public static List<InputProfile> defaultProfiles
		{
			get
			{
				return InputProfile._core.defaultProfiles;
			}
		}

		public static InputProfile FirstProfileWithDevice
		{
			get
			{
				foreach (InputProfile p in InputProfile.defaultProfiles)
				{
					if (p.lastActiveDevice != null && p.lastActiveDevice.productName != null)
					{
						return p;
					}
				}
				return InputProfile.DefaultPlayer1;
			}
		}

		public static void SetDefaultProfile(int idx, InputProfile p)
		{
			if (idx == 0)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer1] = p;
			}
			if (idx == 1)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer2] = p;
			}
			if (idx == 2)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer3] = p;
			}
			if (idx == 3)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer4] = p;
			}
			if (idx == 4)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer5] = p;
			}
			if (idx == 5)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer6] = p;
			}
			if (idx == 6)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer7] = p;
			}
			if (idx == 7)
			{
				InputProfile._core._profiles[InputProfile.MPPlayer8] = p;
			}
		}

		public static InputProfile DefaultPlayer1
		{
			get
			{
				return InputProfile._core.DefaultPlayer1;
			}
		}

		public static InputProfile DefaultPlayer2
		{
			get
			{
				return InputProfile._core.DefaultPlayer2;
			}
		}

		public static InputProfile DefaultPlayer3
		{
			get
			{
				return InputProfile._core.DefaultPlayer3;
			}
		}

		public static InputProfile DefaultPlayer4
		{
			get
			{
				return InputProfile._core.DefaultPlayer4;
			}
		}

		public static InputProfile DefaultPlayer5
		{
			get
			{
				return InputProfile._core.DefaultPlayer5;
			}
		}

		public static InputProfile DefaultPlayer6
		{
			get
			{
				return InputProfile._core.DefaultPlayer6;
			}
		}

		public static InputProfile DefaultPlayer7
		{
			get
			{
				return InputProfile._core.DefaultPlayer7;
			}
		}

		public static InputProfile DefaultPlayer8
		{
			get
			{
				return InputProfile._core.DefaultPlayer8;
			}
		}

		public GenericController genericController
		{
			get
			{
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in this._mappings)
				{
					if (pair.Key is GenericController)
					{
						return pair.Key as GenericController;
					}
				}
				return null;
			}
		}

		protected virtual void UpdateStickPress()
		{
			this.StickPress(PadButton.DPadLeft, this.leftStick.x < -0.6f);
			this.StickPress(PadButton.DPadRight, this.leftStick.x > 0.6f);
			this.StickPress(PadButton.DPadUp, this.leftStick.y > 0.6f);
			this.StickPress(PadButton.DPadDown, this.leftStick.y < -0.6f);
		}

		protected void StickPress(PadButton b, bool press)
		{
			if (press)
			{
				if (this._leftStickStates[b] == InputState.None)
				{
					this._leftStickStates[b] = InputState.Pressed;
					return;
				}
				this._leftStickStates[b] = InputState.Down;
				return;
			}
			else
			{
				if (this._leftStickStates[b] == InputState.Down || this._leftStickStates[PadButton.DPadLeft] == InputState.Pressed)
				{
					this._leftStickStates[b] = InputState.Released;
					return;
				}
				this._leftStickStates[b] = InputState.None;
				return;
			}
		}

		public static Dictionary<string, InputProfile> profiles
		{
			get
			{
				return InputProfile._core._profiles;
			}
		}

		public static InputProfile Add(string name)
		{
			return InputProfile._core.Add(name);
		}

		public static void Update()
		{
			InputProfile._core.Update();
		}

		public static InputProfile Get(string name)
		{
			return InputProfile._core.Get(name);
		}

		private InputDevice _lastActiveDevice
		{
			[CompilerGenerated]
			get
			{
				return this.kBackingField;
			}
			[CompilerGenerated]
			set
			{
				this.kBackingField = value;
			}
		}

		public InputDevice lastActiveDevice
		{
			get
			{
				if (this.lastActiveOverride != null)
				{
					return this.lastActiveOverride;
				}
				if (!MonoMain.started)
				{
					return new InputDevice(0);
				}
				if (this._lastActiveDevice == null)
				{
					foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in this._mappings)
					{
						if (this._lastActiveDevice == null && pair.Key is Keyboard)
						{
							this._lastActiveDevice = pair.Key;
						}
						else if (pair.Key is GenericController && (pair.Key as GenericController).device is XInputPad)
						{
							this._lastActiveDevice = pair.Key;
						}
					}
					if (this._lastActiveDevice == null)
					{
						return this._defaultLastActiveDevice;
					}
				}
				return this._lastActiveDevice;
			}
			set
			{
				this._lastActiveDevice = value;
			}
		}

		public bool HasAnyConnectedDevice()
		{
			using (Dictionary<InputDevice, MultiMap<string, int>>.KeyCollection.Enumerator enumerator = this._mappings.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.isConnected)
					{
						return true;
					}
				}
			}
			return false;
		}

		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public void ClearMappings()
		{
			this._mappings.Clear();
		}

		public MultiMap<string, int> GetMappings(Type t)
		{
			foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in this._mappings)
			{
				if (pair.Key.GetType() == t)
				{
					return pair.Value;
				}
			}
			return null;
		}

		public InputDevice GetDevice(Type t)
		{
			foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in this._mappings)
			{
				if (pair.Key.GetType() == t)
				{
					return pair.Key;
				}
			}
			return null;
		}

		public int GetMapping(Type t, string trigger)
		{
			MultiMap<string, int> map = this.GetMappings(t);
			if (map == null)
			{
				return -1;
			}
			List<int> val = null;
			map.TryGetValue(trigger, out val);
			if (val != null && val.Count > 0)
			{
				return val[0];
			}
			return -1;
		}

		public string GetMappingString(Type t, string trigger)
		{
			int mapping = this.GetMapping(t, trigger);
			if (mapping == -1)
			{
				return "";
			}
			foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in this._mappings)
			{
				if (pair.Key.GetType() == t)
				{
					Dictionary<int, string> mappings = pair.Key.GetTriggerNames();
					if (mappings != null)
					{
						string name = null;
						if (mappings.TryGetValue(mapping, out name))
						{
							return name;
						}
						return "???";
					}
				}
			}
			return "";
		}

		public virtual Sprite GetTriggerImage(string trigger)
		{
			InputDevice last_device = this.lastActiveDevice;
			int mapping = 9999;
			if (trigger != "DPAD" && trigger != "WASD")
			{
				mapping = this.GetMapping(last_device.GetType(), trigger);
			}
			else if (trigger == "WASD")
			{
				mapping = 9998;
			}
			if (mapping == -1)
			{
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> pair in this._mappings)
				{
					mapping = this.GetMapping(pair.Key.GetType(), trigger);
					if (mapping != -1)
					{
						return pair.Key.DoGetMapImage(mapping, false);
					}
				}
				return null;
			}
			return last_device.DoGetMapImage(mapping, false);
		}

		public InputProfile(string profile = "")
		{
			this._name = profile;
			this._leftStickStates[PadButton.DPadLeft] = InputState.None;
			this._leftStickStates[PadButton.DPadRight] = InputState.None;
			this._leftStickStates[PadButton.DPadUp] = InputState.None;
			this._leftStickStates[PadButton.DPadDown] = InputState.None;
		}

		public static InputProfile GetVirtualInput(int index)
		{
			return InputProfile._core.GetVirtualInput(index);
		}

		public VirtualInput virtualDevice
		{
			get
			{
				if (!this._virtualInputInitialized)
				{
					foreach (KeyValuePair<InputDevice, MultiMap<string, int>> kvp in this._mappings)
					{
						if (kvp.Key is VirtualInput)
						{
							this._virtualInput = (kvp.Key as VirtualInput);
							break;
						}
					}
					this._virtualInputInitialized = true;
				}
				return this._virtualInput;
			}
			set
			{
				this._virtualInput = value;
			}
		}

		public virtual void Map(InputDevice device, string trigger, int mapping, bool clearExisting = false)
		{
			if (!this._mappings.ContainsKey(device))
			{
				this._mappings[device] = new MultiMap<string, int>();
			}
			if (clearExisting && this._mappings[device].ContainsKey(trigger))
			{
				this._mappings[device][trigger].Clear();
			}
			this._mappings[device].Add(trigger, mapping);
		}

		public MultiMap<string, int> GetControllerMap<T>() where T : InputDevice
		{
			MultiMap<string, int> controls = null;
			InputDevice d = Input.GetDevice<T>(0);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			d = Input.GetDevice<T>(1);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			d = Input.GetDevice<T>(2);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			d = Input.GetDevice<T>(3);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			return controls;
		}

		public void SetGenericControllerMapIndex<T>(int i, MultiMap<string, int> controls) where T : InputDevice
		{
			InputDevice d = Input.GetDevice<T>(0);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			d = Input.GetDevice<T>(1);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			d = Input.GetDevice<T>(2);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			d = Input.GetDevice<T>(3);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			if (controls != null)
			{
				this._mappings[Input.GetDevice<T>(i)] = controls;
			}
		}

		public InputProfile Clone()
		{
			InputProfile clone = new InputProfile("");
			clone._name = this.name;
			clone._state = this._state;
			clone._prevState = this._prevState;
			clone.dindex = this.dindex;
			clone.swapBack = this.swapBack;
			foreach (KeyValuePair<InputDevice, MultiMap<string, int>> kvp in this._mappings)
			{
				clone._mappings[kvp.Key] = new MultiMap<string, int>();
				foreach (KeyValuePair<string, List<int>> pair in kvp.Value)
				{
					clone._mappings[kvp.Key].AddRange(pair.Key, pair.Value);
				}
			}
			return clone;
		}

		public virtual void UpdateExtraInput()
		{
		}

		public bool CheckCode(InputCode c)
		{
			return c.Update(this);
		}

		public bool JoinGamePressed()
		{
			return this.Pressed("START", false);
		}

		public virtual bool Pressed(string trigger, bool any = false)
		{
			if (Input.ignoreInput && this._virtualInput == null)
			{
				return false;
			}
			if (trigger == "ANY")
			{
				any = true;
			}
			if (this._repeatList.Contains(trigger))
			{
				return true;
			}
			foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
			{
				InputDevice mapped_device = map.Key;
				if ((!(mapped_device is Keyboard) || !InputProfile.ignoreKeyboard) && (this._virtualInput == null || mapped_device is VirtualInput))
				{
					List<int> vals;
					if (any)
					{
						if (mapped_device.MapPressed(-1, true))
						{
							this.lastPressFrame = Graphics.frame;
							return true;
						}
					}
					else if (map.Value.TryGetValue(trigger, out vals))
					{
						foreach (int val in vals)
						{
							if ((mapped_device is AnalogGamePad || mapped_device is GenericController) && this._leftStickStates.ContainsKey((PadButton)val) && this._leftStickStates[(PadButton)val] == InputState.Pressed)
							{
								return true;
							}
							if (mapped_device.MapPressed(val, any))
							{
								this.lastPressFrame = Graphics.frame;
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public virtual bool Released(string trigger)
		{
			if (Input.ignoreInput && this._virtualInput == null)
			{
				return false;
			}
			foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
			{
				InputDevice mapped_device = map.Key;
				List<int> vals;
				if ((this._virtualInput == null || mapped_device is VirtualInput) && map.Value.TryGetValue(trigger, out vals))
				{
					foreach (int val in vals)
					{
						if ((mapped_device is AnalogGamePad || mapped_device is GenericController) && this._leftStickStates.ContainsKey((PadButton)val) && this._leftStickStates[(PadButton)val] == InputState.Released)
						{
							this.lastPressFrame = Graphics.frame;
							return true;
						}
						if (mapped_device.MapReleased(val))
						{
							this.lastPressFrame = Graphics.frame;
							return true;
						}
					}
				}
			}
			return false;
		}

		public virtual bool Down(string trigger)
		{
			if (Input.ignoreInput && this._virtualInput == null)
			{
				return false;
			}
			foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
			{
				InputDevice mapped_device = map.Key;
				List<int> vals;
				if ((this._virtualInput == null || mapped_device is VirtualInput) && map.Value.TryGetValue(trigger, out vals))
				{
					foreach (int val in vals)
					{
						if ((mapped_device is AnalogGamePad || mapped_device is GenericController) && this._leftStickStates.ContainsKey((PadButton)val) && (this._leftStickStates[(PadButton)val] == InputState.Down || this._leftStickStates[(PadButton)val] == InputState.Pressed))
						{
							this.lastPressFrame = Graphics.frame;
							return true;
						}
						if (mapped_device.MapDown(val, false))
						{
							if ((!(mapped_device is Keyboard) || !DuckNetwork.core.enteringText) && !(mapped_device is VirtualInput))
							{
								this._lastActiveDevice = map.Key;
								Input.lastActiveProfile = this;
							}
							this.lastPressFrame = Graphics.frame;
							return true;
						}
					}
				}
			}
			return false;
		}

		public virtual float leftTrigger
		{
			get
			{
				if (this._virtualInput != null)
				{
					return this._virtualInput.leftTrigger;
				}
				if (Input.ignoreInput)
				{
					return 0f;
				}
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is VirtualInput)
					{
						AnalogGamePad pad = map.Key as AnalogGamePad;
						if (pad == null && map.Key is GenericController)
						{
							pad = (map.Key as GenericController).device;
						}
						if (pad != null)
						{
							List<int> mappings = null;
							if (!map.Value.TryGetValue("LTRIGGER", out mappings) || mappings.Count <= 0)
							{
								return pad.leftTrigger;
							}
							int mapping = mappings[0];
							if (mapping == 8388608)
							{
								return pad.leftTrigger;
							}
							if (mapping == 4194304)
							{
								return pad.rightTrigger;
							}
						}
					}
				}
				return 0f;
			}
		}

		public float rightTrigger
		{
			get
			{
				if (this._virtualInput != null)
				{
					return this._virtualInput.rightTrigger;
				}
				if (Input.ignoreInput)
				{
					return 0f;
				}
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is VirtualInput)
					{
						AnalogGamePad pad = map.Key as AnalogGamePad;
						if (pad == null && map.Key is GenericController)
						{
							pad = (map.Key as GenericController).device;
						}
						if (pad != null)
						{
							List<int> mappings = null;
							if (!map.Value.TryGetValue("RTRIGGER", out mappings) || mappings.Count <= 0)
							{
								return pad.rightTrigger;
							}
							int mapping = mappings[0];
							if (mapping == 8388608)
							{
								return pad.leftTrigger;
							}
							if (mapping == 4194304)
							{
								return pad.rightTrigger;
							}
						}
					}
				}
				return 0f;
			}
		}

		public Vec2 leftStick
		{
			get
			{
				if (this._virtualInput != null)
				{
					return this._virtualInput.leftStick;
				}
				if (Input.ignoreInput)
				{
					return Vec2.Zero;
				}
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is VirtualInput)
					{
						AnalogGamePad pad = map.Key as AnalogGamePad;
						if (pad == null && map.Key is GenericController)
						{
							pad = (map.Key as GenericController).device;
						}
						if (pad != null)
						{
							List<int> mappings = null;
							if (!map.Value.TryGetValue("LSTICK", out mappings) || mappings.Count <= 0)
							{
								return pad.leftStick;
							}
							int mapping = mappings[0];
							if (mapping == 64)
							{
								return pad.leftStick;
							}
							if (mapping == 128)
							{
								return pad.rightStick;
							}
						}
					}
				}
				return new Vec2(0f, 0f);
			}
		}

		public Vec2 rightStick
		{
			get
			{
				if (this._virtualInput != null)
				{
					return this._virtualInput.rightStick;
				}
				if (Input.ignoreInput)
				{
					return Vec2.Zero;
				}
				if (Mouse.left == InputState.Pressed || this._mouseAnchor == Vec2.Zero)
				{
					this._mouseAnchor = Mouse.position;
				}
				else
				{
					if (Mouse.left == InputState.Down)
					{
						Vec2 dif = (Mouse.position - this._mouseAnchor) / 16f;
						dif.y *= -1f;
						float len = dif.length;
						if (len > 1f)
						{
							len = 1f;
						}
						return dif.normalized * len;
					}
					if (Mouse.left == InputState.None)
					{
						this._mouseAnchor = Vec2.Zero;
					}
				}
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is VirtualInput)
					{
						AnalogGamePad pad = map.Key as AnalogGamePad;
						if (pad == null && map.Key is GenericController)
						{
							pad = (map.Key as GenericController).device;
						}
						if (pad != null)
						{
							List<int> mappings = null;
							if (!map.Value.TryGetValue("RSTICK", out mappings) || mappings.Count <= 0)
							{
								return pad.rightStick;
							}
							int mapping = mappings[0];
							if (mapping == 64)
							{
								return pad.leftStick;
							}
							if (mapping == 128)
							{
								return pad.rightStick;
							}
						}
					}
				}
				return new Vec2(0f, 0f);
			}
		}

		public bool hasMotionAxis
		{
			get
			{
				if (Input.ignoreInput)
				{
					return false;
				}
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is VirtualInput)
					{
						AnalogGamePad pad = map.Key as AnalogGamePad;
						if (pad == null && map.Key is GenericController)
						{
							pad = (map.Key as GenericController).device;
						}
						if (pad != null)
						{
							return pad.hasMotionAxis;
						}
					}
				}
				return false;
			}
		}

		public float motionAxis
		{
			get
			{
				if (Input.ignoreInput)
				{
					return 0f;
				}
				foreach (KeyValuePair<InputDevice, MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is VirtualInput)
					{
						AnalogGamePad pad = map.Key as AnalogGamePad;
						if (pad == null && map.Key is GenericController)
						{
							pad = (map.Key as GenericController).device;
						}
						if (pad != null)
						{
							return pad.motionAxis;
						}
					}
				}
				return 0f;
			}
		}

		public ushort state
		{
			get
			{
				return this._state;
			}
		}

		public ushort prevState
		{
			get
			{
				return this._prevState;
			}
		}

		public static bool repeat
		{
			get
			{
				return InputProfile._repeat;
			}
			set
			{
				InputProfile._repeat = value;
			}
		}

		public void UpdateTriggerStates()
		{
			this._repeatList.Clear();
			if (InputProfile._repeat)
			{
				if (this.Pressed("MENULEFT", false) || this.Pressed("MENURIGHT", false) || this.Pressed("MENUUP", false) || this.Pressed("MENUDOWN", false))
				{
					if (!this._repeating)
					{
						this._repeatTime = 1.8f;
						this._repeating = true;
					}
					else
					{
						this._repeatTime = 0.5f;
					}
				}
				if (this._repeatTime > 0f)
				{
					this._repeatTime -= 0.1f;
					bool down = false;
					if (this.Down("MENULEFT"))
					{
						if (this._repeatTime <= 0f)
						{
							this._repeatList.Add("MENULEFT");
						}
						down = true;
					}
					if (this.Down("MENURIGHT"))
					{
						if (this._repeatTime <= 0f)
						{
							this._repeatList.Add("MENURIGHT");
						}
						down = true;
					}
					if (this.Down("MENUUP"))
					{
						if (this._repeatTime <= 0f)
						{
							this._repeatList.Add("MENUUP");
						}
						down = true;
					}
					if (this.Down("MENUDOWN"))
					{
						if (this._repeatTime <= 0f)
						{
							this._repeatList.Add("MENUDOWN");
						}
						down = true;
					}
					if (this._repeatTime <= 0f && down)
					{
						this._repeatTime = 0.3f;
					}
					if (!down)
					{
						this._repeating = false;
						this._repeatTime = 0f;
					}
				}
				else
				{
					this._repeatTime = 0f;
				}
			}
			this.UpdateStickPress();
			this._prevState = this._state;
			this._state = 0;
			foreach (string t in Network.synchronizedTriggers)
			{
				this._state |= (this.Down(t) ? (ushort)1 : (ushort)0);
				this._state = (ushort)(this._state << 1);
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static InputProfile()
		{
		}

		public int mpIndex = -1;

		public bool oldAngles;

		public InputProfile swapBack;

		private static InputProfileCore _core = new InputProfileCore();

		public const string SinglePlayer = "SinglePlayer";

		private static string[] _defaultPlayerMappingStrings = new string[]
		{
			"MPPlayer1",
			"MPPlayer2",
			"MPPlayer3",
			"MPPlayer4",
			"MPPlayer5",
			"MPPlayer6",
			"MPPlayer7",
			"MPPlayer8"
		};

		public const string Blank = "Blank";

		private static InputProfile _active;

		public InputDevice lastSynchronizedDevice;

		private InputDevice _defaultLastActiveDevice = new InputDevice(0);

		[CompilerGenerated]
		private InputDevice kBackingField;

		public InputDevice lastActiveOverride;

		private string _name;

		private Dictionary<InputDevice, MultiMap<string, int>> _mappings = new Dictionary<InputDevice, MultiMap<string, int>>();

		protected Dictionary<PadButton, InputState> _leftStickStates = new Dictionary<PadButton, InputState>(default(PadButtonComparer));

		public static InputProfile Default;

		public int dindex;

		private bool _virtualInputInitialized;

		private VirtualInput _virtualInput;

		public long lastPressFrame;

		public static bool ignoreKeyboard = false;

		private Vec2 _mouseAnchor = Vec2.Zero;

		private ushort _state;

		private ushort _prevState;

		private static bool _repeat = false;

		private float _repeatTime;

		private bool _repeating;

		private List<string> _repeatList = new List<string>();
	}
}
