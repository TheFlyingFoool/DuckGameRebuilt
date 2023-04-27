using AddedContent.Hyeve.PolyRender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace DuckGame.ConsoleInterface
{
    public static class RebuiltMono
    {
        [PostInitialize]
        public static void Initialize()
        {
            FontSprite = new Sprite("RebuiltMono");
        }

        public const int WIDTH = 6;
        public const int HEIGHT = WIDTH * 2;
        public static Vec2 CharDimensions => (WIDTH, HEIGHT);

        public static SizeF GetDimensions(string text, float size)
        {
            float totalWidth = 0;
            float currentWidth = 0;

            float totalHeight = HEIGHT * size;
            
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                switch (c)
                {
                    case '\n':
                        totalHeight += HEIGHT * size;
                        currentWidth = 0;
                        break;
                    case '\0':
                        break;
                    default:
                        if (CharPositionMapping.ContainsKey(c))
                            currentWidth += WIDTH * size;
                        break;
                }

                if (currentWidth > totalWidth)
                    totalWidth = currentWidth;
            }

            return new SizeF(totalWidth, totalHeight);
        }

        public static void Draw(
            string text,
            Vec2 position,
            Color color,
            Depth depth,
            float size,
            ContentAlignment alignment
            )
        {
            SizeF dimensions = GetDimensions(text, size);
            
            // horizontal
            const float left = 0;
            float center = dimensions.Width / 2;
            float right = dimensions.Width;
            
            // vertical
            const float top = 0;
            float middle = dimensions.Height / 2;
            float bottom = dimensions.Height;
            
            position = alignment switch
            {
                ContentAlignment.TopLeft => position -      (left   , top   ),
                ContentAlignment.TopCenter => position -    (center , top   ),
                ContentAlignment.TopRight => position -     (right  , top   ),
                ContentAlignment.MiddleLeft => position -   (left   , middle),
                ContentAlignment.MiddleCenter => position - (center , middle),
                ContentAlignment.MiddleRight => position -  (right  , middle),
                ContentAlignment.BottomLeft => position -   (left   , bottom),
                ContentAlignment.BottomCenter => position - (center , bottom),
                ContentAlignment.BottomRight => position -  (right  , bottom),
                _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
            };
            
            Draw(text, position, color, depth, size);
        }

        public static void Draw(
            string text,
            Vec2 position,
            Color color,
            Depth depth,
            float size
            )
        {
            float x = position.x;
            float y = position.y;
            
            if (FontSprite is null)
                return;

            int yOffset = 0;
            for (int i = 0, xOffset = 0; i < text.Length; i++)
            {
                char character = text[i];

                if (character is '\n')
                {
                    yOffset++;
                    xOffset = 0;
                    continue;
                }
                
                if (!CharPositionMapping.TryGetValue(character, out Vec2 sourcePos))
                    sourcePos = CharPositionMapping['\0'];

                Rectangle sourceRectangle = new(sourcePos.x, sourcePos.y, WIDTH - 1, HEIGHT - 2);

                Graphics.Draw(
                    FontSprite.texture,
                    (x + (xOffset * WIDTH * size), y + (yOffset * HEIGHT * size)),
                    sourceRectangle,
                    color,
                    0,
                    FontSprite.center,
                    (size, size),
                    SpriteEffects.None, depth
                );

                xOffset++;
            }
        }
        
        public static void DebugDraw(object? expression, Vec2 pos, Color color, Depth depth, float size = 1f, [CallerArgumentExpression(nameof(expression))] string compileTimeExpression = default!)
        {
            Draw($"{compileTimeExpression}: {expression}", pos, color, depth, size);
        }

        public static Sprite FontSprite;
        public static Dictionary<char, Vec2> CharPositionMapping = new()
        {
            {'a', (3, 1)}, // lowercase
            {'b', (9, 1)},
            {'c', (15, 1)},
            {'d', (21, 1)},
            {'e', (27, 1)},
            {'f', (33, 1)},
            {'g', (39, 1)},
            {'h', (45, 1)},
            {'i', (51, 1)},
            {'j', (57, 1)},
            {'k', (63, 1)},
            {'l', (69, 1)},
            {'m', (75, 1)},
            {'n', (81, 1)},
            {'o', (87, 1)},
            {'p', (93, 1)},
            {'q', (99, 1)},
            {'r', (105, 1)},
            {'s', (111, 1)},
            {'t', (117, 1)},
            {'u', (123, 1)},
            {'v', (129, 1)},
            {'w', (135, 1)},
            {'x', (141, 1)},
            {'y', (147, 1)},
            {'z', (153, 1)},
            {'A', (3, 14)}, // uppercase
            {'B', (9, 14)},
            {'C', (15, 14)},
            {'D', (21, 14)},
            {'E', (27, 14)},
            {'F', (33, 14)},
            {'G', (39, 14)},
            {'H', (45, 14)},
            {'I', (51, 14)},
            {'J', (57, 14)},
            {'K', (63, 14)},
            {'L', (69, 14)},
            {'M', (75, 14)},
            {'N', (81, 14)},
            {'O', (87, 14)},
            {'P', (93, 14)},
            {'Q', (99, 14)},
            {'R', (105, 14)},
            {'S', (111, 14)},
            {'T', (117, 14)},
            {'U', (123, 14)},
            {'V', (129, 14)},
            {'W', (135, 14)},
            {'X', (141, 14)},
            {'Y', (147, 14)},
            {'Z', (153, 14)},
            {'0', (3, 27)}, // numbers
            {'1', (9, 27)},
            {'2', (15, 27)},
            {'3', (21, 27)},
            {'4', (27, 27)},
            {'5', (33, 27)},
            {'6', (39, 27)},
            {'7', (45, 27)},
            {'8', (51, 27)},
            {'9', (57, 27)}, // symbols
            {'-', (63, 27)},
            {'_', (69, 27)},
            {'=', (75, 27)},
            {'+', (81, 27)},
            {'{', (87, 27)},
            {'}', (93, 27)},
            {'[', (99, 27)},
            {']', (105, 27)},
            {'(', (111, 27)},
            {')', (117, 27)},
            {'/', (123, 27)},
            {'\\', (129, 27)},
            {'`', (135, 27)},
            {'\'', (141, 27)},
            {'"', (147, 27)},
            {'~', (153, 27)},
            {'?', (3, 40)},
            {'!', (9, 40)},
            {'>', (15, 40)},
            {'<', (21, 40)},
            {'@', (27, 40)},
            {'#', (33, 40)},
            {'$', (39, 40)},
            {'%', (45, 40)},
            {'^', (51, 40)},
            {'&', (57, 40)},
            {'*', (63, 40)},
            {'|', (69, 40)},
            {':', (75, 40)},
            {';', (81, 40)},
            {',', (87, 40)},
            {'.', (93, 40)},
            {' ', (99, 40)},
            {'\0', (105, 40)},
        };
    }
}