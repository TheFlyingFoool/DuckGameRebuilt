using System;
using System.Globalization;
using System.Collections.Generic;

//Direct port from HiK0Client
//Includes MathParser.cs

namespace DuckGame
{
	public class ChangePlus
	{
		public static float ToSingle(object value)
		{
			if (!(value is string)) return Convert.ToSingle(value, CultureInfo.InvariantCulture);
			try
			{
				string vol = (string)value;
				MathParser stf = new MathParser();
				return (float)stf.Parse(vol);
			}
			catch
			{
				throw new Exception("Can't convert from " + (string)value + " to float!");
			}
		}
		public static Vec2 ToVec2(string value)
		{
			try
			{
				if (value == "mouse") return Mouse.positionScreen;
				Vec2 val = Vec2.Zero;
				string[] p = value.Split(':');
				val.x = ToSingle(p[0]);
				val.y = ToSingle(p[1]);
				return val;
			}
			catch
			{
				throw new Exception("Can't convert from " + value + " to Vec2!");
			}
		}
		public static int ToInt32(string value)
		{
			try
			{
				MathParser stf = new MathParser();
				return (int)Math.Round(stf.Parse(value));
			}
			catch
			{
				throw new Exception("Can't convert from " + value + " to int!");
			}
		}
		public static Color ToColor(string value)
		{
			try
			{
				/*
				 *  helper.Add("white");
                                        helper.Add("grey");
                                        helper.Add("yellow");
                                        helper.Add("orange");
                                        helper.Add("green");
                                        helper.Add("red");
                                        helper.Add("blue");
                                        helper.Add("purple");
				 */
				switch (value)
				{
					case "white":
						return Color.White;
					case "grey":
					case "gray":
						return Color.Gray;
					case "yellow":
						return Color.Yellow;
					case "orange":
						return Color.Orange;
					case "green":
						return Color.Green;
					case "red":
						return Color.Red;
					case "black":
						return Color.Black;
					case "blue":
						return Color.Blue;
					case "purple":
						return Color.Purple;
					default:
						if (value.Contains(":"))
						{
							string[] p = value.Split(':');
							Color val = new Color(ToInt32(p[0]), ToInt32(p[1]), ToInt32(p[2]));
							return val;
						}
						else
						{
							string[] p = value.Split(',');
							Color val = new Color(ToInt32(p[0]), ToInt32(p[1]), ToInt32(p[2]));
							return val;
						}
				}
			}
			catch
			{
				throw new Exception("Can't convert from " + value + " to Color!");
			}
		}
		public static bool ToBoolean(string value)
		{
			try
			{
				if (value.Contains("%"))
				{
					float chance = ToSingle(value.Replace("%", string.Empty));
					if (Rando.Float(100.00f) <= chance) return false;
				}
				if (value.Contains(">"))
				{
					float comp1 = ToSingle(value.Split('>')[0]);
					float comp2 = ToSingle(value.Split('>')[1]);
					if (comp1 > comp2) return true;
					else return false;
				}
				else if (value.Contains("<"))
				{
					float comp1 = ToSingle(value.Split('<')[0]);
					float comp2 = ToSingle(value.Split('<')[1]);
					if (comp1 < comp2) return true;
					else return false;
				}
				else if (value.Contains("=") && !value.Contains("!"))
				{
					float comp1 = ToSingle(value.Split('=')[0]);
					float comp2 = ToSingle(value.Split('=')[1]);
					if (comp1 == comp2) return true;
					else return false;
				}
				else if (value.Contains("!="))
				{
					value = value.Replace("!", string.Empty);
					float comp1 = ToSingle(value.Split('=')[0]);
					float comp2 = ToSingle(value.Split('=')[1]);
					if (comp1 != comp2) return true;
					else return false;
				}
				return Convert.ToBoolean(value);
			}
			catch
			{
				throw new Exception("Can't convert from " + value + " to boolean!");
			}
		}
	}
}