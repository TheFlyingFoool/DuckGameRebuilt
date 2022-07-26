using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DuckGame
{
	public class InputProfile
	{
		public static global::DuckGame.InputProfileCore core
		{
			get
			{
				return global::DuckGame.InputProfile._core;
			}
			set
			{
				global::DuckGame.InputProfile._core = value;
			}
		}

		public static string MPPlayer1
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[0];
			}
		}

		public static string MPPlayer2
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[1];
			}
		}

		public static string MPPlayer3
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[2];
			}
		}

		public static string MPPlayer4
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[3];
			}
		}

		public static string MPPlayer5
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[4];
			}
		}

		public static string MPPlayer6
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[5];
			}
		}

		public static string MPPlayer7
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[6];
			}
		}

		public static string MPPlayer8
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings[7];
			}
		}

		public static string[] MPPlayers
		{
			get
			{
				return global::DuckGame.InputProfile._defaultPlayerMappingStrings;
			}
		}

		public static void ReassignDefaultInputProfiles(bool fullRemap = false)
		{
			if (fullRemap)
			{
				global::DuckGame.Profiles.DefaultPlayer1.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer1);
				global::DuckGame.Profiles.DefaultPlayer2.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer2);
				global::DuckGame.Profiles.DefaultPlayer3.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer3);
				global::DuckGame.Profiles.DefaultPlayer4.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer4);
				global::DuckGame.Profiles.DefaultPlayer5.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer5);
				global::DuckGame.Profiles.DefaultPlayer6.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer6);
				global::DuckGame.Profiles.DefaultPlayer7.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer7);
				global::DuckGame.Profiles.DefaultPlayer8.inputProfile = global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer8);
				return;
			}
			global::DuckGame.Profiles.DefaultPlayer1.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer1));
			global::DuckGame.Profiles.DefaultPlayer2.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer2));
			global::DuckGame.Profiles.DefaultPlayer3.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer3));
			global::DuckGame.Profiles.DefaultPlayer4.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer4));
			global::DuckGame.Profiles.DefaultPlayer5.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer5));
			global::DuckGame.Profiles.DefaultPlayer6.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer6));
			global::DuckGame.Profiles.DefaultPlayer7.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer7));
			global::DuckGame.Profiles.DefaultPlayer8.SetInputProfileLink(global::DuckGame.InputProfile.Get(global::DuckGame.InputProfile.MPPlayer8));
		}

		public static void SwapDefaultInputStrings(string from, string to)
		{
			int fromIdx = -1;
			int toIdx = -1;
			for (int i = 0; i < global::DuckGame.DG.MaxPlayers; i++)
			{
				if (global::DuckGame.InputProfile._defaultPlayerMappingStrings[i] == from)
				{
					fromIdx = i;
				}
				else if (global::DuckGame.InputProfile._defaultPlayerMappingStrings[i] == to)
				{
					toIdx = i;
				}
			}
			if (fromIdx >= 0 && toIdx >= 0)
			{
				string temp = global::DuckGame.InputProfile._defaultPlayerMappingStrings[toIdx];
				global::DuckGame.InputProfile._defaultPlayerMappingStrings[toIdx] = global::DuckGame.InputProfile._defaultPlayerMappingStrings[fromIdx];
				global::DuckGame.InputProfile._defaultPlayerMappingStrings[fromIdx] = temp;
			}
		}

		public static global::DuckGame.InputProfile active
		{
			get
			{
				return global::DuckGame.InputProfile._active;
			}
			set
			{
				global::DuckGame.InputProfile._active = value;
			}
		}

		public static global::System.Collections.Generic.List<global::DuckGame.InputProfile> defaultProfiles
		{
			get
			{
				return global::DuckGame.InputProfile._core.defaultProfiles;
			}
		}

		public static global::DuckGame.InputProfile FirstProfileWithDevice
		{
			get
			{
				foreach (global::DuckGame.InputProfile p in global::DuckGame.InputProfile.defaultProfiles)
				{
					if (p.lastActiveDevice != null && p.lastActiveDevice.productName != null)
					{
						return p;
					}
				}
				return global::DuckGame.InputProfile.DefaultPlayer1;
			}
		}

		public static void SetDefaultProfile(int idx, global::DuckGame.InputProfile p)
		{
			if (idx == 0)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer1] = p;
			}
			if (idx == 1)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer2] = p;
			}
			if (idx == 2)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer3] = p;
			}
			if (idx == 3)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer4] = p;
			}
			if (idx == 4)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer5] = p;
			}
			if (idx == 5)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer6] = p;
			}
			if (idx == 6)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer7] = p;
			}
			if (idx == 7)
			{
				global::DuckGame.InputProfile._core._profiles[global::DuckGame.InputProfile.MPPlayer8] = p;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer1
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer1;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer2
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer2;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer3
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer3;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer4
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer4;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer5
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer5;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer6
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer6;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer7
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer7;
			}
		}

		public static global::DuckGame.InputProfile DefaultPlayer8
		{
			get
			{
				return global::DuckGame.InputProfile._core.DefaultPlayer8;
			}
		}

		public global::DuckGame.GenericController genericController
		{
			get
			{
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> pair in this._mappings)
				{
					if (pair.Key is global::DuckGame.GenericController)
					{
						return pair.Key as global::DuckGame.GenericController;
					}
				}
				return null;
			}
		}

		protected virtual void UpdateStickPress()
		{
			this.StickPress(global::DuckGame.PadButton.DPadLeft, this.leftStick.x < -0.6f);
			this.StickPress(global::DuckGame.PadButton.DPadRight, this.leftStick.x > 0.6f);
			this.StickPress(global::DuckGame.PadButton.DPadUp, this.leftStick.y > 0.6f);
			this.StickPress(global::DuckGame.PadButton.DPadDown, this.leftStick.y < -0.6f);
		}

		protected void StickPress(global::DuckGame.PadButton b, bool press)
		{
			if (press)
			{
				if (this._leftStickStates[b] == global::DuckGame.InputState.None)
				{
					this._leftStickStates[b] = global::DuckGame.InputState.Pressed;
					return;
				}
				this._leftStickStates[b] = global::DuckGame.InputState.Down;
				return;
			}
			else
			{
				if (this._leftStickStates[b] == global::DuckGame.InputState.Down || this._leftStickStates[global::DuckGame.PadButton.DPadLeft] == global::DuckGame.InputState.Pressed)
				{
					this._leftStickStates[b] = global::DuckGame.InputState.Released;
					return;
				}
				this._leftStickStates[b] = global::DuckGame.InputState.None;
				return;
			}
		}

		public static global::System.Collections.Generic.Dictionary<string, global::DuckGame.InputProfile> profiles
		{
			get
			{
				return global::DuckGame.InputProfile._core._profiles;
			}
		}

		public static global::DuckGame.InputProfile Add(string name)
		{
			return global::DuckGame.InputProfile._core.Add(name);
		}

		public static void Update()
		{
			global::DuckGame.InputProfile._core.Update();
		}

		public static global::DuckGame.InputProfile Get(string name)
		{
			return global::DuckGame.InputProfile._core.Get(name);
		}

		private global::DuckGame.InputDevice _lastActiveDevice
		{
			[global::System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.kBackingField;
			}
			[global::System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.kBackingField = value;
			}
		}

		public global::DuckGame.InputDevice lastActiveDevice
		{
			get
			{
				if (this.lastActiveOverride != null)
				{
					return this.lastActiveOverride;
				}
				if (!global::DuckGame.MonoMain.started)
				{
					return new global::DuckGame.InputDevice(0);
				}
				if (this._lastActiveDevice == null)
				{
					foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> pair in this._mappings)
					{
						if (this._lastActiveDevice == null && pair.Key is global::DuckGame.Keyboard)
						{
							this._lastActiveDevice = pair.Key;
						}
						else if (pair.Key is global::DuckGame.GenericController && (pair.Key as global::DuckGame.GenericController).device is global::DuckGame.XInputPad)
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
			using (global::System.Collections.Generic.Dictionary<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>>.KeyCollection.Enumerator enumerator = this._mappings.Keys.GetEnumerator())
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

		public global::DuckGame.MultiMap<string, int> GetMappings(global::System.Type t)
		{
			foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> pair in this._mappings)
			{
				if (pair.Key.GetType() == t)
				{
					return pair.Value;
				}
			}
			return null;
		}

		public global::DuckGame.InputDevice GetDevice(global::System.Type t)
		{
			foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> pair in this._mappings)
			{
				if (pair.Key.GetType() == t)
				{
					return pair.Key;
				}
			}
			return null;
		}

		public int GetMapping(global::System.Type t, string trigger)
		{
			global::DuckGame.MultiMap<string, int> map = this.GetMappings(t);
			if (map == null)
			{
				return -1;
			}
			global::System.Collections.Generic.List<int> val = null;
			map.TryGetValue(trigger, out val);
			if (val != null && val.Count > 0)
			{
				return val[0];
			}
			return -1;
		}

		public string GetMappingString(global::System.Type t, string trigger)
		{
			int mapping = this.GetMapping(t, trigger);
			if (mapping == -1)
			{
				return "";
			}
			foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> pair in this._mappings)
			{
				if (pair.Key.GetType() == t)
				{
					global::System.Collections.Generic.Dictionary<int, string> mappings = pair.Key.GetTriggerNames();
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

		public virtual global::DuckGame.Sprite GetTriggerImage(string trigger)
		{
			global::DuckGame.InputDevice last_device = this.lastActiveDevice;
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
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> pair in this._mappings)
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
			this._leftStickStates[global::DuckGame.PadButton.DPadLeft] = global::DuckGame.InputState.None;
			this._leftStickStates[global::DuckGame.PadButton.DPadRight] = global::DuckGame.InputState.None;
			this._leftStickStates[global::DuckGame.PadButton.DPadUp] = global::DuckGame.InputState.None;
			this._leftStickStates[global::DuckGame.PadButton.DPadDown] = global::DuckGame.InputState.None;
		}

		public static global::DuckGame.InputProfile GetVirtualInput(int index)
		{
			return global::DuckGame.InputProfile._core.GetVirtualInput(index);
		}

		public global::DuckGame.VirtualInput virtualDevice
		{
			get
			{
				if (!this._virtualInputInitialized)
				{
					foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> kvp in this._mappings)
					{
						if (kvp.Key is global::DuckGame.VirtualInput)
						{
							this._virtualInput = (kvp.Key as global::DuckGame.VirtualInput);
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

		public virtual void Map(global::DuckGame.InputDevice device, string trigger, int mapping, bool clearExisting = false)
		{
			if (!this._mappings.ContainsKey(device))
			{
				this._mappings[device] = new global::DuckGame.MultiMap<string, int>();
			}
			if (clearExisting && this._mappings[device].ContainsKey(trigger))
			{
				this._mappings[device][trigger].Clear();
			}
			this._mappings[device].Add(trigger, mapping);
		}

		public global::DuckGame.MultiMap<string, int> GetControllerMap<T>() where T : global::DuckGame.InputDevice
		{
			global::DuckGame.MultiMap<string, int> controls = null;
			global::DuckGame.InputDevice d = global::DuckGame.Input.GetDevice<T>(0);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			d = global::DuckGame.Input.GetDevice<T>(1);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			d = global::DuckGame.Input.GetDevice<T>(2);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			d = global::DuckGame.Input.GetDevice<T>(3);
			if (d != null && this._mappings.ContainsKey(d))
			{
				controls = this._mappings[d];
			}
			return controls;
		}

		public void SetGenericControllerMapIndex<T>(int i, global::DuckGame.MultiMap<string, int> controls) where T : global::DuckGame.InputDevice
		{
			global::DuckGame.InputDevice d = global::DuckGame.Input.GetDevice<T>(0);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			d = global::DuckGame.Input.GetDevice<T>(1);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			d = global::DuckGame.Input.GetDevice<T>(2);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			d = global::DuckGame.Input.GetDevice<T>(3);
			if (d != null && this._mappings.ContainsKey(d))
			{
				this._mappings.Remove(d);
			}
			if (controls != null)
			{
				this._mappings[global::DuckGame.Input.GetDevice<T>(i)] = controls;
			}
		}

		public global::DuckGame.InputProfile Clone()
		{
			global::DuckGame.InputProfile clone = new global::DuckGame.InputProfile("");
			clone._name = this.name;
			clone._state = this._state;
			clone._prevState = this._prevState;
			clone.dindex = this.dindex;
			clone.swapBack = this.swapBack;
			foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> kvp in this._mappings)
			{
				clone._mappings[kvp.Key] = new global::DuckGame.MultiMap<string, int>();
				foreach (global::System.Collections.Generic.KeyValuePair<string, global::System.Collections.Generic.List<int>> pair in kvp.Value)
				{
					clone._mappings[kvp.Key].AddRange(pair.Key, pair.Value);
				}
			}
			return clone;
		}

		public virtual void UpdateExtraInput()
		{
		}

		public bool CheckCode(global::DuckGame.InputCode c)
		{
			return c.Update(this);
		}

		public bool JoinGamePressed()
		{
			return this.Pressed("START", false);
		}

		public virtual bool Pressed(string trigger, bool any = false)
		{
			if (global::DuckGame.Input.ignoreInput && this._virtualInput == null)
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
			foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
			{
				global::DuckGame.InputDevice mapped_device = map.Key;
				if ((!(mapped_device is global::DuckGame.Keyboard) || !global::DuckGame.InputProfile.ignoreKeyboard) && (this._virtualInput == null || mapped_device is global::DuckGame.VirtualInput))
				{
					global::System.Collections.Generic.List<int> vals;
					if (any)
					{
						if (mapped_device.MapPressed(-1, true))
						{
							this.lastPressFrame = global::DuckGame.Graphics.frame;
							return true;
						}
					}
					else if (map.Value.TryGetValue(trigger, out vals))
					{
						foreach (int val in vals)
						{
							if ((mapped_device is global::DuckGame.AnalogGamePad || mapped_device is global::DuckGame.GenericController) && this._leftStickStates.ContainsKey((global::DuckGame.PadButton)val) && this._leftStickStates[(global::DuckGame.PadButton)val] == global::DuckGame.InputState.Pressed)
							{
								return true;
							}
							if (mapped_device.MapPressed(val, any))
							{
								this.lastPressFrame = global::DuckGame.Graphics.frame;
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
			if (global::DuckGame.Input.ignoreInput && this._virtualInput == null)
			{
				return false;
			}
			foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
			{
				global::DuckGame.InputDevice mapped_device = map.Key;
				global::System.Collections.Generic.List<int> vals;
				if ((this._virtualInput == null || mapped_device is global::DuckGame.VirtualInput) && map.Value.TryGetValue(trigger, out vals))
				{
					foreach (int val in vals)
					{
						if ((mapped_device is global::DuckGame.AnalogGamePad || mapped_device is global::DuckGame.GenericController) && this._leftStickStates.ContainsKey((global::DuckGame.PadButton)val) && this._leftStickStates[(global::DuckGame.PadButton)val] == global::DuckGame.InputState.Released)
						{
							this.lastPressFrame = global::DuckGame.Graphics.frame;
							return true;
						}
						if (mapped_device.MapReleased(val))
						{
							this.lastPressFrame = global::DuckGame.Graphics.frame;
							return true;
						}
					}
				}
			}
			return false;
		}

		public virtual bool Down(string trigger)
		{
			if (global::DuckGame.Input.ignoreInput && this._virtualInput == null)
			{
				return false;
			}
			foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
			{
				global::DuckGame.InputDevice mapped_device = map.Key;
				global::System.Collections.Generic.List<int> vals;
				if ((this._virtualInput == null || mapped_device is global::DuckGame.VirtualInput) && map.Value.TryGetValue(trigger, out vals))
				{
					foreach (int val in vals)
					{
						if ((mapped_device is global::DuckGame.AnalogGamePad || mapped_device is global::DuckGame.GenericController) && this._leftStickStates.ContainsKey((global::DuckGame.PadButton)val) && (this._leftStickStates[(global::DuckGame.PadButton)val] == global::DuckGame.InputState.Down || this._leftStickStates[(global::DuckGame.PadButton)val] == global::DuckGame.InputState.Pressed))
						{
							this.lastPressFrame = global::DuckGame.Graphics.frame;
							return true;
						}
						if (mapped_device.MapDown(val, false))
						{
							if ((!(mapped_device is global::DuckGame.Keyboard) || !global::DuckGame.DuckNetwork.core.enteringText) && !(mapped_device is global::DuckGame.VirtualInput))
							{
								this._lastActiveDevice = map.Key;
								global::DuckGame.Input.lastActiveProfile = this;
							}
							this.lastPressFrame = global::DuckGame.Graphics.frame;
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
				if (global::DuckGame.Input.ignoreInput)
				{
					return 0f;
				}
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is global::DuckGame.VirtualInput)
					{
						global::DuckGame.AnalogGamePad pad = map.Key as global::DuckGame.AnalogGamePad;
						if (pad == null && map.Key is global::DuckGame.GenericController)
						{
							pad = (map.Key as global::DuckGame.GenericController).device;
						}
						if (pad != null)
						{
							global::System.Collections.Generic.List<int> mappings = null;
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
				if (global::DuckGame.Input.ignoreInput)
				{
					return 0f;
				}
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is global::DuckGame.VirtualInput)
					{
						global::DuckGame.AnalogGamePad pad = map.Key as global::DuckGame.AnalogGamePad;
						if (pad == null && map.Key is global::DuckGame.GenericController)
						{
							pad = (map.Key as global::DuckGame.GenericController).device;
						}
						if (pad != null)
						{
							global::System.Collections.Generic.List<int> mappings = null;
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

		public global::DuckGame.Vec2 leftStick
		{
			get
			{
				if (this._virtualInput != null)
				{
					return this._virtualInput.leftStick;
				}
				if (global::DuckGame.Input.ignoreInput)
				{
					return global::DuckGame.Vec2.Zero;
				}
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is global::DuckGame.VirtualInput)
					{
						global::DuckGame.AnalogGamePad pad = map.Key as global::DuckGame.AnalogGamePad;
						if (pad == null && map.Key is global::DuckGame.GenericController)
						{
							pad = (map.Key as global::DuckGame.GenericController).device;
						}
						if (pad != null)
						{
							global::System.Collections.Generic.List<int> mappings = null;
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
				return new global::DuckGame.Vec2(0f, 0f);
			}
		}

		public global::DuckGame.Vec2 rightStick
		{
			get
			{
				if (this._virtualInput != null)
				{
					return this._virtualInput.rightStick;
				}
				if (global::DuckGame.Input.ignoreInput)
				{
					return global::DuckGame.Vec2.Zero;
				}
				if (global::DuckGame.Mouse.left == global::DuckGame.InputState.Pressed || this._mouseAnchor == global::DuckGame.Vec2.Zero)
				{
					this._mouseAnchor = global::DuckGame.Mouse.position;
				}
				else
				{
					if (global::DuckGame.Mouse.left == global::DuckGame.InputState.Down)
					{
						global::DuckGame.Vec2 dif = (global::DuckGame.Mouse.position - this._mouseAnchor) / 16f;
						dif.y *= -1f;
						float len = dif.length;
						if (len > 1f)
						{
							len = 1f;
						}
						return dif.normalized * len;
					}
					if (global::DuckGame.Mouse.left == global::DuckGame.InputState.None)
					{
						this._mouseAnchor = global::DuckGame.Vec2.Zero;
					}
				}
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is global::DuckGame.VirtualInput)
					{
						global::DuckGame.AnalogGamePad pad = map.Key as global::DuckGame.AnalogGamePad;
						if (pad == null && map.Key is global::DuckGame.GenericController)
						{
							pad = (map.Key as global::DuckGame.GenericController).device;
						}
						if (pad != null)
						{
							global::System.Collections.Generic.List<int> mappings = null;
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
				return new global::DuckGame.Vec2(0f, 0f);
			}
		}

		public bool hasMotionAxis
		{
			get
			{
				if (global::DuckGame.Input.ignoreInput)
				{
					return false;
				}
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is global::DuckGame.VirtualInput)
					{
						global::DuckGame.AnalogGamePad pad = map.Key as global::DuckGame.AnalogGamePad;
						if (pad == null && map.Key is global::DuckGame.GenericController)
						{
							pad = (map.Key as global::DuckGame.GenericController).device;
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
				if (global::DuckGame.Input.ignoreInput)
				{
					return 0f;
				}
				foreach (global::System.Collections.Generic.KeyValuePair<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> map in this._mappings)
				{
					if (this._virtualInput == null || map.Key is global::DuckGame.VirtualInput)
					{
						global::DuckGame.AnalogGamePad pad = map.Key as global::DuckGame.AnalogGamePad;
						if (pad == null && map.Key is global::DuckGame.GenericController)
						{
							pad = (map.Key as global::DuckGame.GenericController).device;
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
				return global::DuckGame.InputProfile._repeat;
			}
			set
			{
				global::DuckGame.InputProfile._repeat = value;
			}
		}

		public void UpdateTriggerStates()
		{
			this._repeatList.Clear();
			if (global::DuckGame.InputProfile._repeat)
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
			foreach (string t in global::DuckGame.Network.synchronizedTriggers)
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

		public global::DuckGame.InputProfile swapBack;

		private static global::DuckGame.InputProfileCore _core = new global::DuckGame.InputProfileCore();

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

		private static global::DuckGame.InputProfile _active;

		public global::DuckGame.InputDevice lastSynchronizedDevice;

		private global::DuckGame.InputDevice _defaultLastActiveDevice = new global::DuckGame.InputDevice(0);

		[global::System.Runtime.CompilerServices.CompilerGenerated]
		private global::DuckGame.InputDevice kBackingField;

		public global::DuckGame.InputDevice lastActiveOverride;

		private string _name;

		private global::System.Collections.Generic.Dictionary<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>> _mappings = new global::System.Collections.Generic.Dictionary<global::DuckGame.InputDevice, global::DuckGame.MultiMap<string, int>>();

		protected global::System.Collections.Generic.Dictionary<global::DuckGame.PadButton, global::DuckGame.InputState> _leftStickStates = new global::System.Collections.Generic.Dictionary<global::DuckGame.PadButton, global::DuckGame.InputState>(default(global::DuckGame.PadButtonComparer));

		public static global::DuckGame.InputProfile Default;

		public int dindex;

		private bool _virtualInputInitialized;

		private global::DuckGame.VirtualInput _virtualInput;

		public long lastPressFrame;

		public static bool ignoreKeyboard = false;

		private global::DuckGame.Vec2 _mouseAnchor = global::DuckGame.Vec2.Zero;

		private ushort _state;

		private ushort _prevState;

		private static bool _repeat = false;

		private float _repeatTime;

		private bool _repeating;

		private global::System.Collections.Generic.List<string> _repeatList = new global::System.Collections.Generic.List<string>();
	}
}
