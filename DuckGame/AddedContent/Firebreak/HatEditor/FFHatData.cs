using System.Collections.Generic;

namespace DuckGame
{
    public partial class FeatherFashion
    {
        // TODO: update those to use 2D arrays (Color[,]). makes the code a lot less hell
        public static Color[][] HatAnimationBuffer =
        {
            new Color[32 * 32], // hat
            new Color[32 * 32], // quack
        };

        public static Color[] CapeFrameBuffer = new Color[32 * 32];
        public static Color[] RockFrameBuffer = new Color[24 * 24];

        public static Color[][] ParticleAnimationBuffer =
        {
            new Color[12 * 12], // 1
            new Color[12 * 12], // 2
            new Color[12 * 12], // 3
            new Color[12 * 12], // 4
        };

        public static List<Color> Metapixels = new();

        private static Tex2D GetFullHatTexture()
        {
            const int hatWidth = 100;
            const int hatHeight = 56;
                
            Tex2D texture = new(hatWidth, hatHeight);

            Color[] textureData = new Color[hatWidth * hatHeight];
            for (int y = 0; y < hatHeight; y++)
            {
                for (int x = 0; x < hatWidth; x++)
                {
                    int i = y * hatWidth + x;

                    if (y < 32)
                    {
                        if (x < 32)
                        {
                            textureData[i] = HatAnimationBuffer[0][y * 32 + x];
                        }
                        else if (x < 64)
                        {
                            textureData[i] = HatAnimationBuffer[1][y * 32 + (x - 32)];
                        }
                        else if (x < 96)
                        {
                            textureData[i] = CapeFrameBuffer[y * 32 + (x - 64)];
                        }
                    }
                    else if (x < 24)
                    {
                        textureData[i] = RockFrameBuffer[(y - 32) * 24 + x];
                    }
                    else if (x < 36)
                    {
                        if (y < 44)
                        {
                            textureData[i] = ParticleAnimationBuffer[0][(y - 32) * 12 + (x - 24)];
                        }
                        else
                        {
                            textureData[i] = ParticleAnimationBuffer[2][(y - 44) * 12 + (x - 24)];
                        }
                    }
                    else if (x < 48)
                    {
                        if (y < 44)
                        {
                            textureData[i] = ParticleAnimationBuffer[1][(y - 32) * 12 + (x - 36)];
                        }
                        else
                        {
                            textureData[i] = ParticleAnimationBuffer[3][(y - 44) * 12 + (x - 36)];
                        }
                    }
                }
            }

            for (int x = hatWidth - 4, i = 0; x < hatWidth; x++)
            {
                for (int y = 0; y < hatHeight; y++, i++)
                {
                    Color metapixel = i >= Metapixels.Count ? default : Metapixels[i];
                    textureData[y * hatWidth + x] = metapixel;
                }
            }
                
            texture.SetData(textureData);
            return texture;
        }
    }
}