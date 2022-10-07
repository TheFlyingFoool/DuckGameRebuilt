using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

//Direct port from HiK0Client
//Includes ChangePlus.cs

namespace DuckGame
{
    public class MathParser
    {
        private const char GeqSign = (char)8805;
        private const char LeqSign = (char)8804;
        private const char NeqSign = (char)8800;

        #region Properties

        public Dictionary<string, Func<double, double, double>> Operators { get; set; }
        public Dictionary<string, Func<double[], double>> LocalFunctions { get; set; }
        public Dictionary<string, double> LocalVariables { get; set; }
        public string VariableDeclarator = "let";

        #endregion

        public MathParser(
            bool loadPreDefinedFunctions = true,
            bool loadPreDefinedOperators = true,
            bool loadPreDefinedVariables = true,
            CultureInfo cultureInfo = null)
        {
            if (loadPreDefinedOperators)
            {
                Operators = new Dictionary<string, Func<double, double, double>>
                {
                    {
                        "|",
                        (a, b) => (int)a | (int)b
                    },
                    {
                        "&",
                        (a, b) => (int)a & (int)b
                    },
                    {
                        "^",
                        (a, b) => Math.Pow(a, b)
                    },
                    {
                        "%",
                        (a, b) => a%b
                    },
                    {
                        ";",
                        (a, b) => Rando.Float((float)a, (float)b)
                    },
                    {
                        "/",
                        (a, b) =>
                        {
                            if (b != 0)
                                return a / b;
                            else if (a > 0)
                                return 9999999;
                            else if (a < 0)
                                return -9999999;
                            else
                                return double.NaN;
                        }
                    },
                    {
                        "+",
                        (a, b) => a+b
                    },
                    {
                        "-",
                        (a, b) => a-b
                    },
                    {
                        "*",
                        (a, b) => a*b
                    },
                    {
                        ">",
                        (a, b) => a>b ? 1:0
                    },
                    {
                        "<",
                        (a, b) => a<b ? 1:0
                    },
                    {
                        "=",
                        (a, b) => Math.Abs(a - b) < 0.00000001 ? 1 : 0
                    },
                    {
                        "/#" + GeqSign,
                        (a, b) => a > b || Math.Abs(a - b) < 0.00000001 ? 1 : 0
                    },
                    {
                        "#/" + LeqSign,
                        (a, b) => a < b || Math.Abs(a - b) < 0.00000001 ? 1 : 0
                    },
                    {
                        "##" + NeqSign,
                        (a, b) => Math.Abs(a - b) < 0.00000001 ? 0 : 1
                    }
                };
            }
            else
            {
                Operators = new Dictionary<string, Func<double, double, double>>();
            }

            if (loadPreDefinedFunctions)
            {
                LocalFunctions = new Dictionary<string, Func<double[], double>>
                {
                    {
                        "rando",
                        inputs => Rando.Float((float)inputs[0], (float)inputs[1])
                    },
                    {
                        "abs",
                        inputs => Math.Abs(inputs[0])
                    },
                    {
                        "cos",
                        inputs => Math.Cos(inputs[0])
                    },
                    {
                        "cosh",
                        inputs => Math.Cosh(inputs[0])
                    },
                    {
                        "acos",
                        inputs => Math.Acos(inputs[0])
                    },
                    {
                        "arccos",
                        inputs => Math.Acos(inputs[0])
                    },
                    {
                        "sin",
                        inputs => Math.Sin(inputs[0])
                    },
                    {
                        "sinh",
                        inputs => Math.Sinh(inputs[0])
                    },
                    {
                        "asin",
                        inputs => Math.Asin(inputs[0])
                    },
                    {
                        "arcsin",
                        inputs => Math.Asin(inputs[0])
                    },
                    {
                        "tan",
                        inputs => Math.Tan(inputs[0])
                    },
                    {
                        "tanh",
                        inputs => Math.Tanh(inputs[0])
                    },
                    {
                        "atan",
                        inputs => Math.Atan(inputs[0])
                    },
                    {
                        "arctan",
                        inputs => Math.Atan(inputs[0])
                    },
                    {
                        "sqrt",
                        inputs => Math.Sqrt(inputs[0])
                    },
                    {
                        "lerp",
                        inputs => Lerp.Float((float)inputs[0], (float)inputs[1], (float)inputs[2])
                    },
                    {
                        "lerpsmooth",
                        inputs => Lerp.FloatSmooth((float)inputs[0], (float)inputs[1], (float)inputs[2])
                    },
                    {
                        "clamp",
                        inputs => Maths.Clamp((float)inputs[0], (float)inputs[1], (float)inputs[2])
                    },
                    {
                        "pow",
                        inputs => Math.Pow(inputs[0], inputs[1])
                    },
                    {
                        "root",
                        inputs => Math.Pow(inputs[0], 1 / inputs[1])
                    },
                    {
                        "rem",
                        inputs => Math.IEEERemainder(inputs[0], inputs[1])
                    },
                    {
                        "sign",
                        inputs => Math.Sign(inputs[0])
                    },
                    {
                        "exp",
                        inputs => Math.Exp(inputs[0])
                    },
                    {
                        "floor",
                        inputs => Math.Floor(inputs[0])
                    },
                    {
                        "ceil",
                        inputs => Math.Ceiling(inputs[0])
                    },
                    {
                        "ceiling",
                        inputs => Math.Ceiling(inputs[0])
                    },
                    {
                        "round",
                        inputs => Math.Round(inputs[0], MidpointRounding.AwayFromZero)
                    },
                    {
                        "truncate",
                        inputs => Math.Truncate(inputs[0])
                    }
                };
            }
            else
            {
                LocalFunctions = new Dictionary<string, Func<double[], double>>();
            }

            if (loadPreDefinedVariables)
            {
                float f = 0;
                LocalVariables = new Dictionary<string, double>
                {
                    {
                        "nan",
                        double.NaN
                    },
                    {
                        "mx", //mouse x
                        Mouse.positionScreen.x
                    },
                    {
                        "near",
                        f
                    },
                    {
                        "my", //mouse y
                        Mouse.positionScreen.y
                    },
                    {
                        "pi",
                        3.14159265358979
                    },
                    {
                        "tao",
                        6.28318530717959
                    }
                };
                for (int i = 0; i < Profiles.all.Count(); i++)
                {
                    Duck d = Profiles.all[i].duck;
                    if (d != null)
                    {
                        LocalVariables.Add("x" + i, d.x);
                        LocalVariables.Add("y" + i, d.y);
                    }
                }
            }
            else
            {
                LocalVariables = new Dictionary<string, double>();
            }
        }

        public double Parse(string mathExpression)
        {
            return MathParserLogic(Lexer(mathExpression));
        }
        public double Parse(List<string> tokens)
        {
            return MathParserLogic(new List<string>(tokens));
        }

        public List<string> GetTokens(string mathExpression)
        {
            return Lexer(mathExpression);
        }

        #region Core

        private List<string> Lexer(string expr)
        {
            string token = "";
            List<string> tokens = new List<string>();

            expr = expr.Replace("+-", "-");
            expr = expr.Replace("-+", "-");
            expr = expr.Replace("--", "+");
            expr = expr.Replace("==", "=");
            expr = expr.Replace(">=", "" + GeqSign);
            expr = expr.Replace("<=", "" + LeqSign);
            expr = expr.Replace("!=", "" + NeqSign);

            for (int i = 0; i < expr.Length; i++)
            {
                char ch = expr[i];

                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                if (char.IsLetter(ch))
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                    }

                    token += ch;

                    while (i + 1 < expr.Length && char.IsLetterOrDigit(expr[i + 1]))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (char.IsDigit(ch))
                {
                    token += ch;

                    while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (ch == '.')
                {
                    token += ch;

                    while (i + 1 < expr.Length && char.IsDigit(expr[i + 1]))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (i + 1 < expr.Length &&
                    (ch == '-' || ch == '+') &&
                    char.IsDigit(expr[i + 1]) &&
                    (i == 0 || (tokens.Count > 0 && Operators.ContainsKey(tokens.Last())) || i - 1 > 0 && expr[i - 1] == '('))
                {
                    // if the above is true, then the token for that negative number will be "-1", not "-","1".
                    // to sum up, the above will be true if the minus sign is in front of the number, but
                    // at the beginning, for example, -1+2, or, when it is inside the brakets (-1), or when it comes after another operator.
                    // NOTE: this works for + as well!

                    token += ch;

                    while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                    {
                        token += expr[++i];
                    }

                    tokens.Add(token);
                    token = "";

                    continue;
                }

                if (ch == '(')
                {
                    if (i != 0 && (char.IsDigit(expr[i - 1]) || char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
                    {
                        tokens.Add("*");
                        tokens.Add("(");
                    }
                    else
                    {
                        tokens.Add("(");
                    }
                }
                else
                {
                    tokens.Add(ch.ToString());
                }
            }

            return tokens;
        }

        private double MathParserLogic(List<string> tokens)
        {
            // get vars
            for (int i = 0; i < tokens.Count; i++)
            {
                if (LocalVariables.Keys.Contains(tokens[i]))
                {
                    tokens[i] = LocalVariables[tokens[i]].ToString();
                }
            }

            while (tokens.IndexOf("(") != -1)
            {
                // getting data between "(" and ")"
                int open = tokens.LastIndexOf("(");
                int close = tokens.IndexOf(")", open);

                if (open >= close)
                {
                    throw new ArithmeticException("No closing bracket/parenthesis. Token: " + open.ToString());
                }

                List<string> roughExpr = new List<string>();

                for (int i = open + 1; i < close; i++)
                {
                    roughExpr.Add(tokens[i]);
                }

                double tmpResult;

                List<double> args = new List<double>();
                string functionName = tokens[open == 0 ? 0 : open - 1];

                if (LocalFunctions.Keys.Contains(functionName))
                {
                    if (roughExpr.Contains(","))
                    {
                        // converting all arguments into a double array
                        for (int i = 0; i < roughExpr.Count; i++)
                        {
                            List<string> defaultExpr = new List<string>();
                            int firstCommaOrEndOfExpression =
                                roughExpr.IndexOf(",", i) != -1
                                    ? roughExpr.IndexOf(",", i)
                                    : roughExpr.Count;

                            while (i < firstCommaOrEndOfExpression)
                            {
                                defaultExpr.Add(roughExpr[i++]);
                            }

                            args.Add(defaultExpr.Count == 0 ? 0 : BasicArithmeticalExpression(defaultExpr));
                        }

                        // PPPPPPPARSEEEEEEEEEEEEE
                        tmpResult = double.Parse(LocalFunctions[functionName](args.ToArray()).ToString());
                    }
                    else
                    {
                        if (roughExpr.Count == 0)
                            tmpResult = LocalFunctions[functionName](new double[0]);
                        else
                        {
                            tmpResult = double.Parse(LocalFunctions[functionName](new[]
                            {
                                BasicArithmeticalExpression(roughExpr)
                            }).ToString());
                        }
                    }
                }
                else
                {
                    // if no function is need to execute following expression, pass it
                    // to the "BasicArithmeticalExpression" method.
                    tmpResult = BasicArithmeticalExpression(roughExpr);
                }

                // when all the calculations have been done
                // we replace the "opening bracket with the result"
                // and removing the rest.
                tokens[open] = tmpResult.ToString();
                tokens.RemoveRange(open + 1, close - open);

                if (LocalFunctions.Keys.Contains(functionName))
                {
                    // if we also executed a function, removing
                    // the function name as well.
                    tokens.RemoveAt(open - 1);
                }
            }

            // CCCCCCALCulatE
            return BasicArithmeticalExpression(tokens);
        }

        private double BasicArithmeticalExpression(List<string> tokens)
        {
            // PERFORMING A BASIC ARITHMETICAL EXPRESSION CALCULATION
            // THIS METHOD CAN ONLY OPERATE WITH NUMBERS AND OPERATORS
            // AND WILL NOT UNDERSTAND ANYTHING BEYOND THAT.

            double token0;
            double token1;

            switch (tokens.Count)
            {
                case 1:
                    if (!double.TryParse(tokens[0], out token0))
                    {
                        throw new Exception("local variable " + tokens[0] + " is undefined");
                    }

                    return token0;
                case 2:
                    string op = tokens[0];

                    if (op == "-" || op == "+")
                    {
                        string first = op == "+" ? "" : (tokens[1].Substring(0, 1) == "-" ? "" : "-");

                        if (!double.TryParse(first + tokens[1], out token1))
                        {
                            throw new Exception("local variable " + first + tokens[1] + " is undefined");
                        }

                        return token1;
                    }

                    if (!Operators.ContainsKey(op))
                    {
                        throw new Exception("operator " + op + " is not defined");
                    }

                    if (!double.TryParse(tokens[1], out token1))
                    {
                        throw new Exception("local variable " + tokens[1] + " is undefined");
                    }

                    return Operators[op](0, token1);
                case 0:
                    return 0;
            }

            foreach (KeyValuePair<string, Func<double, double, double>> op in Operators)
            {
                int opPlace;

                while ((opPlace = tokens.IndexOf(op.Key)) != -1)
                {
                    double rhs;

                    if (!double.TryParse(tokens[opPlace + 1], out rhs))
                    {
                        throw new Exception("local variable " + tokens[opPlace + 1] + " is undefined");
                    }

                    if (op.Key == "-" && opPlace == 0)
                    {
                        double result = op.Value(0.0, rhs);
                        tokens[0] = result.ToString();
                        tokens.RemoveRange(opPlace + 1, 1);
                    }
                    else
                    {
                        double lhs;

                        if (!double.TryParse(tokens[opPlace - 1], out lhs))
                        {
                            throw new Exception("local variable " + tokens[opPlace - 1] + " is undefined");
                        }

                        double result = op.Value(lhs, rhs);
                        tokens[opPlace - 1] = result.ToString();
                        tokens.RemoveRange(opPlace, 2);
                    }
                }
            }

            if (!double.TryParse(tokens[0], out token0))
            {
                throw new Exception("local variable " + tokens[0] + " is undefined");
            }

            return token0;
        }

        #endregion
    }
}