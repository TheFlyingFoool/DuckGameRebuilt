using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame.AddedContent.Drake.PolyRender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace DuckGame;

public static class DebugTablet
{
    private static bool _open;

    public static bool Open
    {
        get => _open;
        set
        {
            _open = value switch
            {
                true when LockMovementQueue.Add("tablet") => true,
                false when LockMovementQueue.Remove("tablet") => false,
                _ => _open
            };

            // if (!_open)
            //     return;

            // CaretPosition = new IntVec2(Math.Max(Lines.Last().Length - 1, 0), Lines.Count);
        }
    }

    [DrawingContext(CustomID = "tablet")]
    public static void Draw()
    {
        Graphics.polyBatcher.BlendState = BlendState.Opaque;

        if (CustomKeyBinds.CheckInput(Keys.NumPad9))
            Open ^= true;

        if (!Open)
            return;

        // -- update --
        UpdateInputs();

        // -- draw --
        Rectangle drawRect = new(new Vec2(16, 16), new Vec2(Layer.HUD.width - 16, Layer.HUD.height - 16));

        Graphics.DrawRect(drawRect, Color.Black, 1.1f, false, 2);
        Graphics.DrawRect(drawRect, new Color(45, 42, 46), 1f);

        Vec2 stringDrawPos = drawRect.tl.ButBoth(6f, true).ButX(8f, true);

        const float scale = 0.4f;

        var size = Extensions.GetStringSize("0", scale);

        for (int i = 0; i < Lines.Count; i++)
        {
            var drawPos = stringDrawPos.ButY(i * (size.y + 1f), true);

            Graphics.DrawString($"{i + 1}", drawPos.ButX(-8f, true), new Color(91, 81, 92), 1.2f, scale: scale);
            Graphics.DrawString(Lines.FirstOrDefault() == "highlight=true" && i > 0
                    ? DGCommandLanguage.Highlight(Lines[i]) 
                    : Lines[i], drawPos,
                Color.White, 1.2f, scale: scale);
        }

        Graphics.DrawString($"{CaretPosition.Y}:{CaretPosition.X}", drawRect.br - new Vec2(10, 6), Color.Gray, 1.4f, scale: 0.3f);

        DrawCaret(stringDrawPos, scale);
    }

    private static ProgressValue _caretBlink = new();
    private static bool _caretState = true;
    private static List<string> Lines = new() {""};
    public static IntVec2 CaretPosition = new();
    public const int TAB_SPACE_WIDTH = 4;

    public static void DrawCaret(Vec2 position, float fontSize, Color? caretColor = null, bool blinking = true)
    {
        Color color = caretColor ?? Color.White;

        float fade = 1f;

        if (blinking)
        {
            if (_caretState)
            {
                fade = Ease.In.Quart(++_caretBlink);

                if (_caretBlink.Completed)
                    _caretState = false;
            }
            else
            {
                fade = Ease.Out.Quart(--_caretBlink);

                if ((!_caretBlink).Completed)
                    _caretState = true;
            }
        }

        (float w, float h) = Extensions.GetStringSize("0", fontSize);
        float h2 = h;
        w *= CaretPosition.X;
        h *= CaretPosition.Y;
        h += CaretPosition.Y - 0.3f;

        var stringSize = new Vec2(w, h);
        var singleCharacterSize = Extensions.GetStringSize("0", fontSize);

        Vec2 textEndPos = position + stringSize;

        Graphics.DrawRect(textEndPos, textEndPos.ButY(h2, true).ButX(singleCharacterSize.x - 0.2f, true),
            color * fade, 1.3f);
    }

    private static void UpdateInputs()
    { 
        if (!Open)
            return;

        var pressedKeys = Keyboard.KeyState.GetPressedKeys();
        foreach (Microsoft.Xna.Framework.Input.Keys keys in pressedKeys)
        {
            if (!Keyboard.Pressed((Keys) keys))
                continue;
            
            switch (keys)
            {
                case Microsoft.Xna.Framework.Input.Keys.Escape:
                    break;
                case Microsoft.Xna.Framework.Input.Keys.Space:
                {
                    Lines[CaretPosition.Y] = Lines[CaretPosition.Y].Insert(CaretPosition.X, " ");
                    CaretPosition.X++;
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Tab:
                {
                    Lines[CaretPosition.Y] = Lines[CaretPosition.Y].Insert(CaretPosition.X, "    ");
                    CaretPosition.X += 4;
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Enter:
                {
                    string line = Lines[CaretPosition.Y];
                    string currentLine = line.Substring(0, CaretPosition.X);
                    string newLine = line.Substring(CaretPosition.X, line.Length - CaretPosition.X);
                    
                    Lines[CaretPosition.Y] = currentLine;
                    Lines.Insert(++CaretPosition.Y, newLine);
                    CaretPosition.X = 0;
                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Back:
                {
                    if (CaretPosition.X > 0)
                        Lines[CaretPosition.Y] = Lines[CaretPosition.Y].Remove(--CaretPosition.X, 1);
                    else if (CaretPosition.Y > 0)
                    {
                        string line = Lines[CaretPosition.Y];
                        Lines.RemoveAt(CaretPosition.Y);
                        CaretPosition.Y--;
                        CaretPosition.X = Lines[CaretPosition.Y].Length;
                        Lines[CaretPosition.Y] = Lines[CaretPosition.Y].Insert(CaretPosition.X, line);
                    }

                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Up:
                {
                    if (CaretPosition.Y > 0)
                    {
                        CaretPosition.Y--;
                        if (CaretPosition.Y >= 0 && CaretPosition.X > Lines[CaretPosition.Y].Length)
                            CaretPosition.X = Lines[CaretPosition.Y].Length;
                    }
                    else
                    {
                        CaretPosition.X = 0;
                    }

                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Down:
                {
                    if (CaretPosition.Y + 1 < Lines.Count)
                    {
                        CaretPosition.Y++;
                        if (CaretPosition.Y < Lines.Count && CaretPosition.X > Lines[CaretPosition.Y].Length)
                            CaretPosition.X = Lines[CaretPosition.Y].Length;
                    }
                    else
                    {
                        CaretPosition.X = Lines[CaretPosition.Y].Length;
                    }

                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Left:
                {
                    if (CaretPosition.X > 0)
                        CaretPosition.X--;
                    else if (CaretPosition.Y > 0)
                    {
                        CaretPosition.Y--;
                        CaretPosition.X = Lines[CaretPosition.Y].Length;
                    }

                    break;
                }
                case Microsoft.Xna.Framework.Input.Keys.Right:
                {
                    if (CaretPosition.X < Lines[CaretPosition.Y].Length)
                        CaretPosition.X++;
                    else if (CaretPosition.Y + 1 < Lines.Count)
                    {
                        CaretPosition.Y++;
                        CaretPosition.X = 0;
                    }

                    break;
                }
                default:
                {
                    char charFromKey = Keyboard.GetCharFromKey((Keys) keys);
                    if (charFromKey != ' ')
                        Lines[CaretPosition.Y] = Lines[CaretPosition.Y].Insert(CaretPosition.X++, charFromKey.ToString());

                    break;
                }
            }
        }
    }

    public record struct IntVec2(int X, int Y);
}