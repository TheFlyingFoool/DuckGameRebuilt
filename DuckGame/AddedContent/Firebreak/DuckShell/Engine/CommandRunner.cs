using AddedContent.Firebreak;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DuckGame.ConsoleEngine
{
    public class CommandRunner
    {
        internal readonly List<Marker.DevConsoleCommandAttribute> Commands = new();
        public readonly List<ITypeInterpreter> TypeInterpreterModules = new();
        public Dictionary<Type, ITypeInterpreter> TypeInterpreterModulesMap = new();
        public Regex ValidNameRegex { get; } = new("^[^ ]+$", RegexOptions.Compiled);

        public const string INLINE_COMMAND_MARKER = "±";

        internal bool TryGetCommand(string from, out Marker.DevConsoleCommandAttribute command)
        {
            return Commands.TryFirst(
                x => x.Name.CaselessEquals(from) || x.Aliases.Any(y => y.CaselessEquals(from)),
                out command);
        }

        public virtual ValueOrException<object?> Run(string input)
        {
            if (string.IsNullOrEmpty(input))
                return ValueOrException<object>.FromValue(null);

            string[] commandSegments;

            try
            {
                commandSegments = Tokenize(input);
            }
            catch (Exception e)
            {
                return e;
            }

            if (commandSegments.Length == 0)
                return ValueOrException<object>.FromValue(null);

            try
            {
                if (commandSegments.Contains(";"))
                {
                    List<string>[] commandSequence = new List<string>[commandSegments.Count(x => x == ";") + 1];

                    for (int i = 0; i < commandSequence.Length; i++)
                    {
                        commandSequence[i] = new List<string>();
                    }

                    for (int i = 0, j = 0; i < commandSegments.Length; i++)
                    {
                        string segment = commandSegments[i];

                        if (segment == ";")
                        {
                            j++;
                            continue;
                        }

                        commandSequence[j].Add(segment);
                    }

                    ValueOrException<string> accumulativeResult = "";
                    foreach (List<string> cmd in commandSequence)
                    {
                        if (cmd.Count == 0)
                            continue;

                        ValueOrException<string[]> cmdParseResult = ParseCodeBlocks(cmd);

                        ValueOrException<object?> result = null!;

                        cmdParseResult.TryUse(
                            tokens => result = RunFromTokens(tokens),
                            exception => result = exception
                        );

                        ValueOrException<string> stringResult = result.Failed
                            ? result.Error
                            : result.Value?.ToString();

                        accumulativeResult.AppendResult(stringResult);
                    }

                    return accumulativeResult.Failed
                        ? accumulativeResult.Error
                        : accumulativeResult.Value;
                }

                ValueOrException<string[]> parseResult = ParseCodeBlocks(commandSegments);

                return parseResult.Failed
                    ? parseResult.Error
                    : RunFromTokens(parseResult.Value);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /* ~Firebreak: TODO
          * handle custom AutoCompls for longer-than-one-word strings
          * no autocompletion for empty strings
        */
        public virtual ValueOrException<string[]> Predict(string partialCommand, int caretPosition)
        {
            if (string.IsNullOrEmpty(partialCommand))
                return Array.Empty<string>();

            if (caretPosition == -1)
                caretPosition = partialCommand.Length;
            
            if (caretPosition < 0 || caretPosition > partialCommand.Length)
                throw new ArgumentOutOfRangeException(nameof(caretPosition), "todev: Caret position cannot be outside the range of the command");
            
            partialCommand = partialCommand.Substring(0, caretPosition);

            string[] tokens = Tokenize(partialCommand + "[]");

            if (tokens.Length == 0)
                return Array.Empty<string>();

            // whether to predict the current incomplete word or the next word
            bool predictNext = tokens[tokens.Length - 1] != string.Empty;
            
            return PredictFromTokens(tokens, predictNext);
        }

        protected virtual ValueOrException<string[]> PredictFromTokens(string[] tokens, bool next)
        {
            string commandName = tokens[0];

            string[] commandArgs = new string[tokens.Length - 1];
            for (int i = 1; i < tokens.Length; i++)
            {
                commandArgs[i - 1] = tokens[i];
            }

            if (commandArgs.Length == 0)
            {
                IList<string> suggestions = CommandAutoCompl.GetCommandNames();
                
                return AutoCompl.FilterAndSortToRelevant(commandName, suggestions);
            }
            else
            {
                string lastArgument = commandArgs[commandArgs.Length - 1];

                if (lastArgument.StartsWith(INLINE_COMMAND_MARKER))
                    return Predict(lastArgument.Substring(1), -1);

                if (!TryGetCommand(commandName, out Marker.DevConsoleCommandAttribute command))
                    return new Exception($"Command not found: {commandName}");

                ShellCommand.Parameter[] parameterInfos = command.Command.Parameters;

                if (parameterInfos.Length == 0 || commandArgs.Length > parameterInfos.Length)
                    return Array.Empty<string>();

                IList<string> rawSuggestions = parameterInfos[commandArgs.Length - 1].Autocompletion.Get(lastArgument);
                
                return AutoCompl.FilterAndSortToRelevant(lastArgument, rawSuggestions);
            }
        }

        protected virtual string SerializeForPrint(object o)
        {
            if (o is null)
                return null;

            if (o is IEnumerable collection)
                return string.Join(",", collection.Cast<object>());
            
            if (FireSerializer.IsSerializable(o.GetType()))
                return FireSerializer.Serialize(o);
            
            return o.ToString();
        }

        protected virtual string ReadableCollectionForPrint(IEnumerable collection)
        {
            if (collection is not string)
                return collection.Cast<object>().ToReadableString();

            return collection.ToString();
        }

        protected virtual ValueOrException<object?> RunFromTokens(string[] tokens)
        {
            if (tokens.Length == 0)
                throw new Exception("in RunFromTokens(string[] tokens), tokens.Length == 0");

            string commandName = tokens[0];

            string[] commandArgs = new string[tokens.Length - 1];
            for (int i = 1; i < tokens.Length; i++)
            {
                commandArgs[i - 1] = tokens[i];
            }
            
            if (!TryGetCommand(commandName, out Marker.DevConsoleCommandAttribute command))
                return new Exception($"Command not found: {commandName}");
            
            if (command.HostOnly && !Network.isServer)
                return new Exception("Only the host can use this command");

            if (command.IsCheat && DevConsole.CheckCheats())
                return new Exception("Can't use cheat commands here");
            
            ShellCommand.Parameter[] parameterInfos = command.Command.Parameters;
            object?[] appliedParameters = new object?[parameterInfos.Length]; // parsed and juiced
            bool lastIsParams = parameterInfos.Length != 0 && parameterInfos[parameterInfos.Length - 1].IsParams;

            for (int i = 0; i < appliedParameters.Length; i++)
            {
                ShellCommand.Parameter parameterInfo = parameterInfos[i];
                object? appliedParameterValue;

                if (i >= commandArgs.Length && !lastIsParams)
                {
                    if (parameterInfo.IsOptional)
                        appliedParameterValue = parameterInfo.DefaultValue;
                    else return new Exception($"Missing Argument: {parameterInfo.Name}");
                }
                else
                {
                    Type parseType = parameterInfo.ParameterType;
                    string argString;
                    ValueOrException<object> parseResult = null;

                    bool isLast = i == appliedParameters.Length - 1;

                    if (isLast && (parameterInfo.IsParams || parseType == typeof(string)))
                    {
                        int length = commandArgs.Length - i;
                        string[] resultArray = new string[length];
                        Array.Copy(commandArgs, i, resultArray, 0, length);

                        if (parseType == typeof(string))
                            argString = string.Join(" ", resultArray);
                        else if (parameterInfo.IsParams)
                        {
                            if (length != 0)
                            {
                                parseResult = ValueOrException<object>.FromValue(resultArray);
                            }

                            argString = null;
                        }
                        else throw new InvalidOperationException();
                    }
                    else if (i >= commandArgs.Length)
                    {
                        return new Exception($"Missing Argument: {parameterInfo.Name}");
                    }
                    else
                    {
                        argString = commandArgs[i];
                    }

                    if (parseResult is null)
                    {
                        if (!TypeInterpreterModules.TryFirst(x => x.ParsingType.IsAssignableFrom(parseType),
                                out ITypeInterpreter interpreter))
                            return new Exception($"No conversion module found: {parseType.Name}");

                        parseResult = interpreter.ParseString(argString, parseType, new TypeInterpreterParseContext(this, parameterInfo));
                    }

                    if (parseResult.Failed)
                        return new Exception($"Parsing Error: {parseResult.Error.Message}");

                    appliedParameterValue = parseResult.Value;
                }

                appliedParameters[i] = appliedParameterValue;
            }

            ValueOrException<object?> result;
            try
            {
                object? invokationValue = command.Command.Invoke(appliedParameters);

                if (command.Member is not null && ((MethodInfo)command.Member).ReturnTypeCustomAttributes
                    .GetCustomAttributes(typeof(PrintSerializedAttribute), true)
                    .Any())
                {
                    invokationValue = SerializeForPrint(invokationValue);
                }
                else if (command.Member is not null && ((MethodInfo)command.Member).ReturnTypeCustomAttributes
                         .GetCustomAttributes(typeof(PrintReadableCollectionAttribute), true)
                         .Any() && invokationValue is IEnumerable)
                {
                    invokationValue = ReadableCollectionForPrint((IEnumerable) invokationValue);
                }

                result = ValueOrException<object?>.FromValue(invokationValue);
            }
            catch (TargetInvocationException e)
            {
                return e.InnerException ?? e;
            }
            catch (Exception e)
            {
                result = ValueOrException<object>.FromError(e);
            }

            return result;
        }

        private ValueOrException<string[]> ParseCodeBlocks(IReadOnlyList<string> commandSegments)
        {
            List<string> segments = new();
            foreach (string segment in commandSegments)
            {
                if (!segment.StartsWith(INLINE_COMMAND_MARKER))
                {
                    segments.Add(segment);
                    continue;
                }

                ValueOrException<object?> runResult = Run(segment.Substring(INLINE_COMMAND_MARKER.Length));
                runResult.TryUse(output =>
                {
                    segments.Add(output?.ToString() ?? "");
                }, _ => { });

                if (runResult.Failed)
                    return runResult.Error;
            }

            return segments.ToArray();
        }

        private static string[] Tokenize(string input)
        {
            List<string> split = new();

            StringBuilder currentSegment = new();
            bool treatNextAsDefault = false;

            bool awaitCloseCodeBlock = false;
            int ignoreCloseCodeBlocks = 0;

            bool awaitCloseSquareBracket = false;
            int ignoreCloseSquareBrackets = 0;

            bool awaitCommentEnd = false;

            bool addEmpty = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (treatNextAsDefault)
                {
                    if (!awaitCommentEnd)
                        currentSegment.Append(c);
                    
                    treatNextAsDefault = false;
                    continue;
                }

                switch (c)
                {
                    case '\\':
                        if (awaitCloseCodeBlock)
                            goto default;
                        
                        treatNextAsDefault = true;
                        continue;
                    
                    case '#':
                        if (awaitCloseCodeBlock || awaitCloseSquareBracket)
                            goto default;

                        awaitCommentEnd ^= true;
                        continue;
                
                    case '[':
                        if (awaitCloseCodeBlock || awaitCommentEnd)
                            goto default;
                    
                        if (awaitCloseSquareBracket)
                        {
                            ignoreCloseSquareBrackets++;
                            goto default;
                        }

                        awaitCloseSquareBracket = true;
                        continue;
                    case ']':
                        if (awaitCloseCodeBlock || awaitCommentEnd)
                            goto default;
                    
                        if (awaitCloseSquareBracket)
                        {
                            if (ignoreCloseSquareBrackets > 0)
                            {
                                ignoreCloseSquareBrackets--;
                                goto default;
                            }
                            else
                            {
                                awaitCloseSquareBracket = false;

                                if (input[i - 1] == '[')
                                    addEmpty = true;
                            }
                        }
                        continue;
                
                    case '{':
                        if (awaitCloseSquareBracket || awaitCommentEnd)
                            goto default;

                        if (awaitCloseCodeBlock)
                        {
                            ignoreCloseCodeBlocks++;
                            goto default;
                        }

                        awaitCloseCodeBlock = true;
                        currentSegment.Append(INLINE_COMMAND_MARKER);
                        break;
                    case '}':
                        if (awaitCloseSquareBracket || awaitCommentEnd)
                            goto default;

                        if (awaitCloseCodeBlock)
                        {
                            if (ignoreCloseCodeBlocks > 0)
                            {
                                ignoreCloseCodeBlocks--;
                                goto default;
                            }
                            else
                            {
                                awaitCloseCodeBlock = false;
                            }
                        }
                        break;

                    case '\n':
                    case '\r':
                        goto case ' ';

                    case ' ':
                        if (awaitCloseSquareBracket || awaitCloseCodeBlock || awaitCommentEnd)
                            goto default;
                    
                        if (currentSegment.Length > 0)
                            split.Add(currentSegment.ToString());
                        else if (addEmpty)
                        {
                            split.Add(string.Empty);
                            addEmpty = false;
                        }
                    
                        currentSegment.Clear();
                        break;
                
                    default:
                        if (awaitCommentEnd)
                            continue;
                        
                        currentSegment.Append(c);
                        break;
                }
            }
        
            if (currentSegment.Length > 0)
                split.Add(currentSegment.ToString());
            else if (addEmpty) 
                split.Add(string.Empty);

            return split.ToArray();
        }

        /// <param name="shellCommand" />
        /// <param name="description">Describes the usage of this command</param>
        public void AddCommand(ShellCommand shellCommand, string? description = null)
        {
            AddCommand(new Marker.DevConsoleCommandAttribute()
            {
                Name = shellCommand.Name,
                Description = description,
                Command = shellCommand,
            });
        }

        public void RemoveCommand(string commandName)
        {
            if (TryGetCommand(commandName, out Marker.DevConsoleCommandAttribute command)) 
                Commands.Remove(command);
        }

        internal void AddCommand(Marker.DevConsoleCommandAttribute command)
        {
            if (!ValidNameRegex.IsMatch(command.Name))
                throw new Exception($"Invalid command name: {command.Name}");
            
            RemoveCommand(command.Name);
            Commands.Add(command);
        }
    }
}