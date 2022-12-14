using System;

namespace DuckGame
{
    public class ScriptStatement
    {
        public static ScriptStatement Parse(string statement, object left = null, object right = null, ScriptOperator operat = ScriptOperator.None, string func = null, bool isChild = false)
        {
            ScriptStatement newStatement = new ScriptStatement
            {
                leftObject = left,
                rightObject = right,
                op = operat,
                functionName = func
            };
            int openParenthesis = 1;
            if (!isChild)
            {
                openParenthesis++;
            }
            string currentWord = "";
            ScriptOperator currentOperator = ScriptOperator.None;
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
                            newStatement.data = Change.ToSingle(currentWord);
                        }
                        else if (isNumber)
                        {
                            newStatement.data = Convert.ToInt32(currentWord);
                        }
                    }
                    if (isOperator)
                    {
                        if (currentWord == "+")
                        {
                            currentOperator = ScriptOperator.Plus;
                        }
                        else if (currentWord == "-")
                        {
                            currentOperator = ScriptOperator.Minus;
                        }
                        else if (currentWord == "*")
                        {
                            currentOperator = ScriptOperator.Multiply;
                        }
                        else if (currentWord == "/")
                        {
                            currentOperator = ScriptOperator.Divide;
                        }
                        else if (currentWord == "==")
                        {
                            currentOperator = ScriptOperator.IsEqual;
                        }
                        else if (currentWord == "!=")
                        {
                            currentOperator = ScriptOperator.IsNotEqual;
                        }
                        else if (currentWord == ">")
                        {
                            currentOperator = ScriptOperator.GreaterThan;
                        }
                        else if (currentWord == "<")
                        {
                            currentOperator = ScriptOperator.LessThan;
                        }
                        else if (currentWord == "&&")
                        {
                            currentOperator = ScriptOperator.And;
                        }
                        else if (currentWord == "and")
                        {
                            currentOperator = ScriptOperator.And;
                        }
                        else if (currentWord == "or")
                        {
                            currentOperator = ScriptOperator.Or;
                        }
                        if (newStatement.op == ScriptOperator.None)
                        {
                            newStatement.op = currentOperator;
                        }
                    }
                    if (c == '(')
                    {
                        ScriptStatement innerStatement = Parse(statement, null, null, ScriptOperator.None, isWord ? currentWord : null, true);
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
                        if (currentOperator > ScriptOperator.COMPARATORS)
                        {
                            ScriptStatement s = Parse(statement, dat, null, ScriptOperator.None, null, true);
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
                                return Parse(statement, newStatement, dat, currentOperator, null, true);
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
                ScriptStatement leftStatement = leftObject as ScriptStatement;
                object leftResult;
                if (leftStatement != null)
                {
                    leftResult = leftStatement.result;
                }
                else
                {
                    leftResult = leftObject;
                }
                ScriptStatement rightStatement = rightObject as ScriptStatement;
                object rightResult;
                if (rightStatement != null)
                {
                    if (leftResult is bool || op <= ScriptOperator.IsNotEqual)
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
                    rightResult = rightObject;
                }
                if (leftResult != null)
                {
                    if (rightResult != null)
                    {
                        if ((leftResult is float || leftResult is int) && (rightResult is float || rightResult is int))
                        {
                            if (leftResult is float || rightResult is float)
                            {
                                float left = Change.ToSingle(leftResult);
                                float right = Change.ToSingle(rightResult);
                                if (op == ScriptOperator.Plus)
                                {
                                    finalResult = left + right;
                                }
                                else if (op == ScriptOperator.Minus)
                                {
                                    finalResult = left - right;
                                }
                                else if (op == ScriptOperator.Multiply)
                                {
                                    finalResult = left * right;
                                }
                                else if (op == ScriptOperator.Divide)
                                {
                                    finalResult = ((right != 0f) ? (left / right) : 0f);
                                }
                                else if (op == ScriptOperator.IsEqual)
                                {
                                    finalResult = (left == right);
                                }
                                else if (op == ScriptOperator.IsNotEqual)
                                {
                                    finalResult = (left != right);
                                }
                                else if (op == ScriptOperator.GreaterThan)
                                {
                                    finalResult = (left > right);
                                }
                                else if (op == ScriptOperator.LessThan)
                                {
                                    finalResult = (left < right);
                                }
                            }
                            else
                            {
                                int left2 = (int)leftResult;
                                int right2 = (int)rightResult;
                                if (op == ScriptOperator.Plus)
                                {
                                    finalResult = left2 + right2;
                                }
                                else if (op == ScriptOperator.Minus)
                                {
                                    finalResult = left2 - right2;
                                }
                                else if (op == ScriptOperator.Multiply)
                                {
                                    finalResult = left2 * right2;
                                }
                                else if (op == ScriptOperator.Divide)
                                {
                                    finalResult = ((right2 != 0) ? (left2 / right2) : 0);
                                }
                                else if (op == ScriptOperator.IsEqual)
                                {
                                    finalResult = (left2 == right2);
                                }
                                else if (op == ScriptOperator.IsNotEqual)
                                {
                                    finalResult = (left2 != right2);
                                }
                                else if (op == ScriptOperator.GreaterThan)
                                {
                                    finalResult = (left2 > right2);
                                }
                                else if (op == ScriptOperator.LessThan)
                                {
                                    finalResult = (left2 < right2);
                                }
                            }
                        }
                        else if (leftResult is string && rightResult is string)
                        {
                            string left3 = (string)leftResult;
                            string right3 = (string)rightResult;
                            if (op == ScriptOperator.Plus)
                            {
                                finalResult = left3 + right3;
                            }
                            else if (op == ScriptOperator.IsEqual)
                            {
                                finalResult = (left3 == right3);
                            }
                            else if (op == ScriptOperator.IsNotEqual)
                            {
                                finalResult = (left3 != right3);
                            }
                        }
                        else if (leftResult is bool && rightResult is bool)
                        {
                            bool left4 = (bool)leftResult;
                            bool right4 = (bool)rightResult;
                            if (op == ScriptOperator.IsEqual)
                            {
                                finalResult = (left4 == right4);
                            }
                            else if (op == ScriptOperator.IsNotEqual)
                            {
                                finalResult = (left4 != right4);
                            }
                            else if (op == ScriptOperator.And)
                            {
                                finalResult = (left4 && right4);
                            }
                            else if (op == ScriptOperator.Or)
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
                if (functionName != null)
                {
                    object res;
                    if (finalResult is string)
                    {
                        res = Script.CallMethod(functionName, finalResult as string);
                    }
                    else if (finalResult is int)
                    {
                        res = Script.CallMethod(functionName, (int)finalResult);
                    }
                    else if (finalResult is float)
                    {
                        res = Script.CallMethod(functionName, (float)finalResult);
                    }
                    else
                    {
                        res = Script.CallMethod(functionName, null);
                    }
                    ScriptObject obj = res as ScriptObject;
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
                        finalResult = (float)finalResult * (obj.negative ? -1 : 1);
                    }
                    if (finalResult is int && obj != null)
                    {
                        finalResult = (int)finalResult * (obj.negative ? -1 : 1);
                    }
                }
                if (rightResult != null && rightStatement != null && rightStatement.op > ScriptOperator.IsNotEqual)
                {
                    return new ScriptStatement
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

        public ScriptOperator op;

        public string functionName;

        public string error;

        public string remainingStatementString;
    }
}
