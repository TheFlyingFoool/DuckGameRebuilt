using DuckGame.ConsoleInterface;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public partial class OnKeybindAttribute
    {
        public static bool IsActive(string bindString)
        {
            TimeSpan multiInputGrace = MallardManager.Config.Keymap.DefaultSettings.MultiInputGraceMilliseconds.Milliseconds();
            
            DateTime now = DateTime.Now;
            DateTime sequenceLowerLimit = DateTime.MaxValue;
            string[] sequence = bindString.Split(',');

            for (int i = 0; i < sequence.Length; i++)
            {
                string input = sequence[sequence.Length - 1 - i];

                EnsureStringIntegrity(input);

                bool isDuckInput = char.IsUpper(input[0]);
                List<InputActivationInfo> searchList = isDuckInput
                    ? KeybindReceptionHandler.LastActivatedDuckInputs
                    : KeybindReceptionHandler.LastActivatedKeyboardInputs;

                if (searchList.Count == 0)
                    return false;

                string[] rawInputs = input.Split('+');

                DateTime lowestActivationTime = DateTime.MaxValue;
                DateTime lastActivationTime = default;
                for (int j = 0; j < rawInputs.Length; j++)
                {
                    string rawInput = rawInputs[rawInputs.Length - 1 - j];

                    EnsureStringIntegrity(rawInput);

                    if (!Match(now, sequenceLowerLimit, i, rawInput, searchList, out InputActivationInfo responsibleActivation))
                        return false;
                    
                    DateTime responsibleActivationTime = responsibleActivation.ActivationTime;

                    // (lastTime - grace > thisTime < lastTime + grace) == false
                    if (lastActivationTime != default &&
                        !(lastActivationTime - multiInputGrace > responsibleActivationTime &&
                         responsibleActivationTime < lastActivationTime + multiInputGrace))
                        break;
                    
                    if (responsibleActivationTime < lowestActivationTime)
                        lowestActivationTime = responsibleActivationTime;

                    lastActivationTime = responsibleActivationTime;
                }

                sequenceLowerLimit = lowestActivationTime;
            }

            KeybindReceptionHandler.Flush();
            return true;
        }

        private static bool Match(
            DateTime now,
            DateTime lowerLimit,
            int sequenceIndex,
            string rawInput,
            IReadOnlyList<InputActivationInfo> activatedInputs,
            out InputActivationInfo responsibleActivation)
        {
            responsibleActivation = default;
            
            int sequenceGrace = MallardManager.Config.Keymap.DefaultSettings.SequenceGraceMilliseconds;
            int multiInputGrace = MallardManager.Config.Keymap.DefaultSettings.MultiInputGraceMilliseconds;

            rawInput = rawInput.Replace("ctrl", "leftcontrol");
            rawInput = rawInput.Replace("alt", "leftalt");
            rawInput = rawInput.Replace("shift", "leftshift");

            List<InputActivationInfo> relevantActivations = activatedInputs
                .Where(x => KeybindReceptionHandler.DownInputs.Contains(x.InputName) ||
                            (x.ActivationTime < lowerLimit &&
                             x.ActivationTime + multiInputGrace.Milliseconds() >
                             now - (sequenceGrace * (sequenceIndex + 1)).Milliseconds()))
                .ToList();

            if (relevantActivations.Count == 0)
                return false;

            for (int i = relevantActivations.Count - 1; i >= 0; i--)
            {
                InputActivationInfo activationInfo = relevantActivations[i];

                string keyId = activationInfo.InputName.Substring(2);
                
                switch (rawInput[0])
                {
                    case '_':
                        if (KeybindReceptionHandler.DownInputs.Contains(activationInfo.InputName) && keyId.CaselessEquals(rawInput.Substring(1)))
                            break;
                        continue;
                    
                    case '^':
                        if (!activationInfo.Pressed && keyId.CaselessEquals(rawInput.Substring(1)))
                            break;
                        continue;
                    
                    default:
                        if (activationInfo.Pressed && keyId.CaselessEquals(rawInput))
                        {
                            if (KeybindReceptionHandler.DownInputs.Contains(activationInfo.InputName))
                                KeybindReceptionHandler.DownInputs.Remove(activationInfo.InputName);
                            
                            break;
                        }
                        continue;
                }

                responsibleActivation = activationInfo;
                return true;
            }

            return false;
        }

        private static void EnsureStringIntegrity(string str)
        {
            if (str.Length > 0 && !string.IsNullOrWhiteSpace(str))
                return;

            throw new ArgumentException($@"Invalid input usage in keybind string: ""{str}""");
        }

        public bool IsActive() => IsActive(KeybindString);
    }
}