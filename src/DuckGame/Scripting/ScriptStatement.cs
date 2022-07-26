using System;

namespace DuckGame
{
	public class ScriptStatement
	{
		public static global::DuckGame.ScriptStatement Parse(string statement, object left = null, object right = null, global::DuckGame.ScriptOperator operat = global::DuckGame.ScriptOperator.None, string func = null, bool isChild = false)
		{
			global::DuckGame.ScriptStatement newStatement = new global::DuckGame.ScriptStatement();
			newStatement.leftObject = left;
			newStatement.rightObject = right;
			newStatement.op = operat;
			newStatement.functionName = func;
			int openParenthesis = 1;
			if (!isChild)
			{
				openParenthesis++;
			}
			string currentWord = "";
			global::DuckGame.ScriptOperator currentOperator = global::DuckGame.ScriptOperator.None;
			bool isNumber = false;
			bool isFloat = false;
			bool isWord = false;
			bool isOperator = false;
			bool parsingWhitespace = false;
			while (statement.Length > 0)
			{
				char c = statement[0];
				statement = statement.Remove(0, 1);
				if (!isWord && char.IsNumber(c))
				{
					if (currentWord.Length > 0 && !isNumber)
					{
						if (!isOperator || !(currentWord == "-"))
						{
							newStatement.error = "Found unexpected number.";
							break;
						}
						isOperator = false;
					}
					isNumber = true;
				}
				else if (c == '.' && !isWord)
				{
					if (currentWord.Length > 0 && !isNumber)
					{
						newStatement.error = "Found unexpected Period.";
						break;
					}
					isFloat = true;
				}
				else if (c == ' ' || c == '(' || c == ')')
				{
					parsingWhitespace = true;
					if (isWord && (currentWord == "and" || currentWord == "or"))
					{
						isWord = false;
						isOperator = true;
					}
					if (newStatement.data == null)
					{
						if (isWord)
						{
							newStatement.data = currentWord;
						}
						else if (isFloat)
						{
							newStatement.data = global::DuckGame.Change.ToSingle(currentWord);
						}
						else if (isNumber)
						{
							newStatement.data = global::System.Convert.ToInt32(currentWord);
						}
					}
					if (isOperator)
					{
						if (currentWord == "+")
						{
							currentOperator = global::DuckGame.ScriptOperator.Plus;
						}
						else if (currentWord == "-")
						{
							currentOperator = global::DuckGame.ScriptOperator.Minus;
						}
						else if (currentWord == "*")
						{
							currentOperator = global::DuckGame.ScriptOperator.Multiply;
						}
						else if (currentWord == "/")
						{
							currentOperator = global::DuckGame.ScriptOperator.Divide;
						}
						else if (currentWord == "==")
						{
							currentOperator = global::DuckGame.ScriptOperator.IsEqual;
						}
						else if (currentWord == "!=")
						{
							currentOperator = global::DuckGame.ScriptOperator.IsNotEqual;
						}
						else if (currentWord == ">")
						{
							currentOperator = global::DuckGame.ScriptOperator.GreaterThan;
						}
						else if (currentWord == "<")
						{
							currentOperator = global::DuckGame.ScriptOperator.LessThan;
						}
						else if (currentWord == "&&")
						{
							currentOperator = global::DuckGame.ScriptOperator.And;
						}
						else if (currentWord == "and")
						{
							currentOperator = global::DuckGame.ScriptOperator.And;
						}
						else if (currentWord == "or")
						{
							currentOperator = global::DuckGame.ScriptOperator.Or;
						}
						if (newStatement.op == global::DuckGame.ScriptOperator.None)
						{
							newStatement.op = currentOperator;
						}
					}
					if (c == '(')
					{
						global::DuckGame.ScriptStatement innerStatement = global::DuckGame.ScriptStatement.Parse(statement, null, null, global::DuckGame.ScriptOperator.None, isWord ? currentWord : null, true);
						statement = innerStatement.remainingStatementString;
						if (innerStatement.error != null)
						{
							newStatement.error = innerStatement.error;
							break;
						}
						newStatement.data = innerStatement;
					}
					else if (c == ')')
					{
						openParenthesis--;
					}
					if (newStatement.data != null)
					{
						object dat = newStatement.data;
						newStatement.data = null;
						if (currentOperator > global::DuckGame.ScriptOperator.COMPARATORS)
						{
							ScriptStatement s = global::DuckGame.ScriptStatement.Parse(statement, dat, null, global::DuckGame.ScriptOperator.None, null, true);
							statement = s.remainingStatementString;
							dat = s;
						}
						if (newStatement.leftObject == null)
						{
							newStatement.leftObject = dat;
						}
						else
						{
							if (newStatement.rightObject != null)
							{
								return global::DuckGame.ScriptStatement.Parse(statement, newStatement, dat, currentOperator, null, true);
							}
							newStatement.rightObject = dat;
						}
					}
					currentWord = "";
					isWord = false;
					isFloat = false;
					isNumber = false;
					isOperator = false;
					if (openParenthesis <= 0)
					{
						break;
					}
				}
				else if ((!isWord && c == '+') || c == '-' || c == '*' || c == '/' || c == '=' || c == '<' || c == '!' || c == '>' || c == '&')
				{
					if (currentWord.Length > 0 && !isOperator)
					{
						newStatement.error = "Found unexpected operator.";
						break;
					}
					isOperator = true;
				}
				else if (c == '"')
				{
					parsingWhitespace = true;
					isWord = true;
				}
				else
				{
					if (currentWord.Length > 0 && !isWord)
					{
						newStatement.error = "Found unexpected letter.";
						break;
					}
					isWord = true;
				}
				if (!parsingWhitespace)
				{
					currentWord += c.ToString();
				}
				parsingWhitespace = false;
			}
			newStatement.remainingStatementString = statement;
			return newStatement;
		}

		public object result
		{
			get
			{
				object finalResult = null;
				global::DuckGame.ScriptStatement leftStatement = this.leftObject as global::DuckGame.ScriptStatement;
				object leftResult;
				if (leftStatement != null)
				{
					leftResult = leftStatement.result;
				}
				else
				{
					leftResult = this.leftObject;
				}
				global::DuckGame.ScriptStatement rightStatement = this.rightObject as global::DuckGame.ScriptStatement;
				object rightResult;
				if (rightStatement != null)
				{
					if (leftResult is bool || this.op <= global::DuckGame.ScriptOperator.IsNotEqual)
					{
						rightResult = rightStatement.result;
					}
					else
					{
						rightResult = null;
					}
				}
				else
				{
					rightResult = this.rightObject;
				}
				if (leftResult != null)
				{
					if (rightResult != null)
					{
						if ((leftResult is float || leftResult is int) && (rightResult is float || rightResult is int))
						{
							if (leftResult is float || rightResult is float)
							{
								float left = global::DuckGame.Change.ToSingle(leftResult);
								float right = global::DuckGame.Change.ToSingle(rightResult);
								if (this.op == global::DuckGame.ScriptOperator.Plus)
								{
									finalResult = left + right;
								}
								else if (this.op == global::DuckGame.ScriptOperator.Minus)
								{
									finalResult = left - right;
								}
								else if (this.op == global::DuckGame.ScriptOperator.Multiply)
								{
									finalResult = left * right;
								}
								else if (this.op == global::DuckGame.ScriptOperator.Divide)
								{
									finalResult = ((right != 0f) ? (left / right) : 0f);
								}
								else if (this.op == global::DuckGame.ScriptOperator.IsEqual)
								{
									finalResult = (left == right);
								}
								else if (this.op == global::DuckGame.ScriptOperator.IsNotEqual)
								{
									finalResult = (left != right);
								}
								else if (this.op == global::DuckGame.ScriptOperator.GreaterThan)
								{
									finalResult = (left > right);
								}
								else if (this.op == global::DuckGame.ScriptOperator.LessThan)
								{
									finalResult = (left < right);
								}
							}
							else
							{
								int left2 = (int)leftResult;
								int right2 = (int)rightResult;
								if (this.op == global::DuckGame.ScriptOperator.Plus)
								{
									finalResult = left2 + right2;
								}
								else if (this.op == global::DuckGame.ScriptOperator.Minus)
								{
									finalResult = left2 - right2;
								}
								else if (this.op == global::DuckGame.ScriptOperator.Multiply)
								{
									finalResult = left2 * right2;
								}
								else if (this.op == global::DuckGame.ScriptOperator.Divide)
								{
									finalResult = ((right2 != 0) ? (left2 / right2) : 0);
								}
								else if (this.op == global::DuckGame.ScriptOperator.IsEqual)
								{
									finalResult = (left2 == right2);
								}
								else if (this.op == global::DuckGame.ScriptOperator.IsNotEqual)
								{
									finalResult = (left2 != right2);
								}
								else if (this.op == global::DuckGame.ScriptOperator.GreaterThan)
								{
									finalResult = (left2 > right2);
								}
								else if (this.op == global::DuckGame.ScriptOperator.LessThan)
								{
									finalResult = (left2 < right2);
								}
							}
						}
						else if (leftResult is string && rightResult is string)
						{
							string left3 = (string)leftResult;
							string right3 = (string)rightResult;
							if (this.op == global::DuckGame.ScriptOperator.Plus)
							{
								finalResult = left3 + right3;
							}
							else if (this.op == global::DuckGame.ScriptOperator.IsEqual)
							{
								finalResult = (left3 == right3);
							}
							else if (this.op == global::DuckGame.ScriptOperator.IsNotEqual)
							{
								finalResult = (left3 != right3);
							}
						}
						else if (leftResult is bool && rightResult is bool)
						{
							bool left4 = (bool)leftResult;
							bool right4 = (bool)rightResult;
							if (this.op == global::DuckGame.ScriptOperator.IsEqual)
							{
								finalResult = (left4 == right4);
							}
							else if (this.op == global::DuckGame.ScriptOperator.IsNotEqual)
							{
								finalResult = (left4 != right4);
							}
							else if (this.op == global::DuckGame.ScriptOperator.And)
							{
								finalResult = (left4 && right4);
							}
							else if (this.op == global::DuckGame.ScriptOperator.Or)
							{
								finalResult = (left4 || right4);
							}
						}
					}
					else
					{
						finalResult = leftResult;
					}
				}
				else if (rightResult != null)
				{
					finalResult = rightResult;
				}
				if (this.functionName != null)
				{
					object res;
					if (finalResult is string)
					{
						res = global::DuckGame.Script.CallMethod(this.functionName, finalResult as string);
					}
					else if (finalResult is int)
					{
						res = global::DuckGame.Script.CallMethod(this.functionName, (int)finalResult);
					}
					else if (finalResult is float)
					{
						res = global::DuckGame.Script.CallMethod(this.functionName, (float)finalResult);
					}
					else
					{
						res = global::DuckGame.Script.CallMethod(this.functionName, null);
					}
					global::DuckGame.ScriptObject obj = res as global::DuckGame.ScriptObject;
					if (obj != null)
					{
						finalResult = obj.objectProperty.GetValue(obj.obj, null);
					}
					else
					{
						finalResult = res;
					}
					if (finalResult is float && obj != null)
					{
						finalResult = (float)finalResult * (float)(obj.negative ? -1 : 1);
					}
					if (finalResult is int && obj != null)
					{
						finalResult = (int)finalResult * (obj.negative ? -1 : 1);
					}
				}
				if (rightResult != null && rightStatement != null && rightStatement.op > global::DuckGame.ScriptOperator.IsNotEqual)
				{
					return new global::DuckGame.ScriptStatement
					{
						leftObject = finalResult,
						rightObject = rightStatement.rightObject,
						op = rightStatement.op
					}.result;
				}
				return finalResult;
			}
		}

		public ScriptStatement()
		{
		}

		public object data;

		public object leftObject;

		public object rightObject;

		public global::DuckGame.ScriptOperator op;

		public string functionName;

		public string error;

		public string remainingStatementString;
	}
}
