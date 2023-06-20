using DuckGame.ConsoleInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DuckGame
{
    public static class KeybindReceptionHandler
    {
        public static List<InputActivationInfo> LastActivatedKeyboardInputs = new();
        public static List<InputActivationInfo> LastActivatedDuckInputs = new();
        public static List<string> DownInputs = new();
        public static bool HasListUpdated = false;
        
        public static void UpdateKeyboardInputs()
        {
            Array genericArray = typeof(Microsoft.Xna.Framework.Input.Keys).GetEnumValues();
            Microsoft.Xna.Framework.Input.Keys[] allKeys = new Microsoft.Xna.Framework.Input.Keys[genericArray.Length];
            genericArray.CopyTo(allKeys, 0);
            
            foreach (Microsoft.Xna.Framework.Input.Keys key in allKeys)
            {
                bool keyPressed = Keyboard.Pressed((Keys)key);
                bool keyReleased = Keyboard.Released((Keys)key);
                
                if ((keyPressed || keyReleased) == false)
                    continue;

                InputActivationInfo inputActivationInfo = new($"k_{key.ToString()}", keyPressed);
                LastActivatedKeyboardInputs.Add(inputActivationInfo);
                
                if (keyPressed && !DownInputs.Contains(inputActivationInfo.InputName))
                    DownInputs.Add(inputActivationInfo.InputName);
                else if (DownInputs.Contains(inputActivationInfo.InputName)) 
                    DownInputs.Remove(inputActivationInfo.InputName);

                HasListUpdated = true;
            }

            // -- debug viewer --

            List<InputActivationInfo> inputs = LastActivatedDuckInputs.Concat(LastActivatedKeyboardInputs).OrderBy(x => x.ActivationTime.Ticks).ToList();
            
            int inputCount = inputs.Count;
            for (int i = 0; i < Math.Min(inputCount, 25); i++)
            {
                InputActivationInfo input = inputs[(inputCount - 1) - i];

                string drawString = input.InputName + $" {input.ActivationTime:mm:ss.fff}";
                float size = 0.5f;
                SizeF textDimensions = RebuiltMono.GetDimensions(drawString, size);
                Vec2 drawPos = new(Layer.HUD.width, Layer.HUD.height - i * textDimensions.Height);
                Graphics.DrawRect(new Rectangle(drawPos.x - textDimensions.Width, drawPos.y - textDimensions.Height, textDimensions.Width, textDimensions.Height), Color.Black * 0.6f, 1.5f);
                RebuiltMono.Draw(drawString, drawPos, input.Pressed ? Color.Aqua : Color.Maroon, 2f, size, ContentAlignment.BottomRight);
            }
            
            RebuiltMono.DebugDraw(inputs.Count, (0, 0), Color.Red, 2f);
        }
        
        public static void UpdateDuckInputs()
        {
            for (int i = 0; i < Triggers.TriggerList.Count; i++)
            {
                string trigger = Triggers.TriggerList[i];
                
                foreach (Profile localProfile in Profiles.active.Where(x => x.localPlayer))
                {
                    bool triggerPressed = localProfile.inputProfile.Pressed(trigger);
                    bool triggerReleased = localProfile.inputProfile.Released(trigger);

                    if ((triggerPressed || triggerReleased) == false)
                        continue;

                    InputActivationInfo inputActivationInfo = new($"d_{trigger}", triggerPressed);
                    LastActivatedDuckInputs.Add(inputActivationInfo);
                
                    if (triggerPressed && !DownInputs.Contains(inputActivationInfo.InputName))
                        DownInputs.Add(inputActivationInfo.InputName);
                    else if (DownInputs.Contains(inputActivationInfo.InputName)) 
                        DownInputs.Remove(inputActivationInfo.InputName);

                    HasListUpdated = true;
                }
            }
        }

        public static void Flush()
        {
            LastActivatedDuckInputs.Clear();
            LastActivatedKeyboardInputs.Clear();
        }
    }
}