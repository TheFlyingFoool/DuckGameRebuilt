using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public static class CustomKeyBinds
	{
		public static readonly Dictionary<string, Keys> KeyDict = new();
		static CustomKeyBinds()
        {
			foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
			{
				KeyDict.Add(key.ToString().ToLower(), key);
			}
		}
		
		/// <summary>
		/// Translates inputed strings or characters that describe keyboard keys to their Keys variant.
		/// </summary>
		/// <param name="character">the keyboard key character or string to be converted</param>
		/// <returns>the corresponding Keys value of said keyboard character</returns>
		public static Keys CharToKeys(string character)
		{
			if (KeyDict.TryGetValue(character.ToLower(), out Keys key))
			{
				return key;
			}
			else
			{
				throw new Exception($"Key ({character}) does not exist");
			}
		}
		/// <summary>
		/// Translates inputed Keys to the corresponding string or character that describe keyboard keys to their Keys variant.
		/// </summary>
		/// <param name="key">the keyboard key to be converted</param>
		/// <returns>the corresponding string value of said keyboard character</returns>
		public static string KeysToChar(Keys key) => key.ToString().ToLower();
		public static bool CheckInput(Keys key, CheckInputMethod method = CheckInputMethod.Pressed) // Keys, single
		{
			return method switch
			{
				CheckInputMethod.Pressed => Keyboard.Pressed(key),
				CheckInputMethod.Down => Keyboard.Down(key),
				CheckInputMethod.Released => Keyboard.Released(key),
				_ => false
			};
		}
		/// <summary>
		/// Checks if the keyboard key has been inputed
		/// </summary>
		/// <param name="key">the key as a string to be checked</param>
		/// <param name="method">what returns <see langword="true"/> when checked</param>
		/// <returns>a boolean describing if the input has happened</returns>
		public static bool CheckInput(string key, CheckInputMethod method = CheckInputMethod.Pressed) // string, single
		{
			foreach (string item in key.Split('|'))
			{
				if (item == "NONE") 
					return false;
				
				if (CheckInput(item.Split('+').Select(CharToKeys), method)) 
					return true; // keys, multiple
			}
			
			return false;
		}
		/// <summary>
		/// Checks if the duck input or keyboard key has been inputed
		/// </summary>
		/// <param name="str">the string value that describes a duck input or a keyboard key to be checked</param>
		/// <param name="prof">the duck profile to check the inputs of. in the case this is a duck input</param>
		/// <param name="method">what returns <see langword="true"/> when checked</param>
		/// <returns>a boolean describing if the input has happened</returns>
		public static bool CheckInputAuto(string str, Profile prof, CheckInputMethod method = CheckInputMethod.Pressed)
		{
			foreach (string item in str.Split('|'))
			{
				if (item == item.ToUpper())
				{
					if (CheckInput(item, prof, method)) 
						return true;
				}
				else
				{
					if (CheckInput(item, method)) 
						return true;
				}
			}
			
			return false;
		}
		/// <summary>
		/// Checks if the duck input has been inputed
		/// </summary>
		/// <param name="input">the string value that describes a duck input to be checked</param>
		/// <param name="prof">the duck profile to check the inputs of</param>
		/// <param name="method">what returns <see langword="true"/> when checked</param>
		/// <returns>a boolean describing if the input has happened</returns>
		public static bool CheckInput(string input, Profile prof, CheckInputMethod method = CheckInputMethod.Pressed) // input, single
		{
			if (input == "NONE" || prof == null) 
				return false;
			
			return method switch
			{
				CheckInputMethod.Pressed =>  prof.inputProfile.Pressed(input),
				CheckInputMethod.Down =>	 prof.inputProfile.Down(input),
				CheckInputMethod.Released => prof.inputProfile.Released(input),
				_ => false
			};
		}

		/// <summary>
		/// Checks if the duck input(s) ha(ve) been inputed
		/// </summary>
		/// <param name="inputs">the string values that describes the duck inputs to be checked</param>
		/// <param name="prof">the duck profile to check the inputs of</param>
		/// <param name="method">what returns <see langword="true"/> when checked. (<see langword="CheckInputMethod.Pressed"/> would check if every input is down except the last being checked as pressed)</param>
		/// <returns>a boolean describing if the input has happened</returns>
		public static bool CheckInput(IEnumerable<string> inputs, Profile prof, CheckInputMethod method = CheckInputMethod.Pressed) // input, multiple
		{
			if (method != CheckInputMethod.Pressed)
			{
				return inputs.All(x => CheckInput(x, prof, method));
			}
			else
			{
				return inputs.Where((_, i) => i < inputs.Count() - 1).All(x => CheckInput(x, prof, CheckInputMethod.Down)) && CheckInput(inputs.Last(), prof, CheckInputMethod.Pressed);
			}
		}
		/// <summary>
		/// Checks if the keyboard key(s) ha(ve) been inputed
		/// </summary>
		/// <param name="keys">the keys as Keys values to be checked</param>
		/// <param name="method">what returns <see langword="true"/> when checked (<see langword="CheckInputMethod.Pressed"/> would check if every input is down except the last being checked as pressed)</param>
		/// <returns>a boolean describing if the input has happened</returns>
		public static bool CheckInput(IEnumerable<Keys> keys, CheckInputMethod method = CheckInputMethod.Pressed) // keys, multiple
		{
			if (method != CheckInputMethod.Pressed)
			{
				return keys.All(x => CheckInput(x, method));
			}
            else
            {
				return keys.Where((_, i) => i < keys.Count() - 1).All(x => CheckInput(x, CheckInputMethod.Down)) && CheckInput(keys.Last(), CheckInputMethod.Pressed);
			}
		}
		/// <summary>
		/// Checks if the keyboard key(s) ha(ve) been inputed
		/// </summary>
		/// <param name="keys">the keys as a strings to be checked</param>
		/// <param name="method">what returns <see langword="true"/> when checked (<see langword="CheckInputMethod.Pressed"/> would check if every input is down except the last being checked as pressed)</param>
		/// <returns>a boolean describing if the input has happened</returns>
		public static bool CheckInput(IEnumerable<string> keys, CheckInputMethod method = CheckInputMethod.Pressed) // string, multiple
																			=> CheckInput(keys.Select(CharToKeys), method);
		public enum CheckInputMethod
        {
            Pressed,
            Released,
            Down
        }
    }
}