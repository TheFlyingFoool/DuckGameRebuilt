using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

//Direct port from HiK0Client
//Includes MathParser.cs

namespace DuckGame
{
    public class ChangePlus
    {
        public static float ToSingle(object value)
        {
            if (value is not string vol)
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);

            try
            {
                MathParser stf = new MathParser();
                return (float)stf.Parse(vol);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't convert from {vol} to float!", e);
            }
        }
        public static Vec2 ToVec2(string value)
        {
            try
            {
                if (value == "mouse") return Mouse.positionScreen;
                Vec2 val = Vec2.Zero;
                string[] p = value.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                val.x = ToSingle(p[0]);
                val.y = ToSingle(p[1]);
                return val;
            }
            catch (Exception e)
            {
                throw new Exception($"Can't convert from {value} to Vec2!", e);
            }
        }
        public static int ToInt32(string value)
        {
            try
            {
                MathParser stf = new MathParser();
                return (int)Math.Round(stf.Parse(value));
            }
            catch (Exception e)
            {
                throw new Exception($"Can't convert from {value} to int!", e);
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
                        string[] p = value.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        Color val = new Color(ToInt32(p[0]), ToInt32(p[1]), ToInt32(p[2]));
                        return val;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Can't convert from {value} to Color!", e);
            }
        }

        /// <summary>Matches two sides (floats) between a comparison operator.</summary>
        /// <example>
        ///	<![CDATA[4 > 4]]> <br />
        /// <![CDATA[56 != 57]]> <br />
        /// <![CDATA[8 <= 2]]>
        /// </example>
        /// <remarks>
        /// An operator is valid with this RegEx if it uses one of these
        /// 4 characters (<![CDATA[<, >, =, !]]>) in any combination with
        /// a length of 1 or 2. 
        /// </remarks>
        private static readonly Regex _twoSideOperationRegex = new(@"([\d.]+?)\s*([<>=!]{1,2})\s*([\d.]+)", RegexOptions.Compiled);

        public static bool ToBoolean(string value)
        {
            try
            {
                if (value.Last() == '%')
                {
                    float chance = ToSingle(value.Substring(0, value.Length - 1));
                    return chance > Rando.Float(100.00f);
                }
                else if (_twoSideOperationRegex.Match(value)
                         is var m && m.Success)
                {
                    float comp1 = ToSingle(m.Groups[1].Value);
                    float comp2 = ToSingle(m.Groups[3].Value);
                    string operation = m.Groups[2].Value;
                    switch (operation)
                    {
                        case ">":
                            return comp1 > comp2;
                        case "<":
                            return comp1 < comp2;
                        case ">=":
                            return comp1 >= comp2;
                        case "<=":
                            return comp1 <= comp2;
                        case "=":
                        case "==":
                            return comp1 == comp2;
                        case "!=":
                            return comp1 != comp2;
                        default:
                            throw new Exception($"Invalid Operation: {operation}");
                    }
                }

                return Convert.ToBoolean(value);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't convert from {value} to boolean!", e);
            }
        }
    }
}