using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DuckGame
{
    public partial class FeatherFashion
    {
        public static class FFEditorPane
        {

            public static int AnimationFrame;
            public static EditorMode CurrentMode = EditorMode.Hat;
            public static CanvasTool CurrentCanvasTool = CanvasTool.Brush;
            public static Color CanvasPrimaryColor = Color.Black;
            public static Color CanvasSecondaryColor = Color.White;
            private static bool s_renderOnionSkin = false;
            private static int s_animationFrameBeingHovered = -1;

            public static Color[][] HatAnimationBuffer =
            {
                new Color[1024], // hat
                new Color[1024], // quack
            };
            public static Color[] CapeFrameBuffer = new Color[1024];
            public static Color[] RockFrameBuffer = new Color[576];
            public static Color[][] ParticleAnimationBuffer =
            {
                new Color[144], // 1
                new Color[144], // 2
                new Color[144], // 3
                new Color[144], // 4
            };
            public static Color[] Metapixels = new Color[224];

            public static Tex2D FullHatTexture
            {
                get
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
                                else
                                {
                                    // metapixels
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
                            Color metapixel = Metapixels[i];
                            textureData[y * hatWidth + x] = metapixel;
                        }
                    }
                    
                    texture.SetData(textureData);
                    return texture;
                }
            }

            public static void OnSwitch()
            {
                
            }
            
            public static void OnSwitchOutOf()
            {
                
            }

            public static void Update()
            {
                switch (CurrentMode)
                {
                    case EditorMode.Hat:
                        UpdateCanvas(true, 32, ref HatAnimationBuffer[AnimationFrame], Level.current.camera.center);
                        break;

                    case EditorMode.Cape:
                        UpdateCanvas(true, 32, ref CapeFrameBuffer, Level.current.camera.center);
                        break;

                    case EditorMode.Rock:
                        UpdateCanvas(false, 24, ref RockFrameBuffer, Level.current.camera.center);
                        break;

                    case EditorMode.Particle:
                        UpdateCanvas(false, 12, ref ParticleAnimationBuffer[AnimationFrame], Level.current.camera.center);
                        break;

                    case EditorMode.Metapixel:
                        break;

                    default: throw new InvalidOperationException();
                }
            }

            public static void Draw()
            {
                switch (CurrentMode)
                {
                    case EditorMode.Hat:
                        DrawCanvas(true, 32, HatAnimationBuffer[AnimationFrame], Level.current.camera.center);
                        break;

                    case EditorMode.Cape:
                        DrawCanvas(true, 32, CapeFrameBuffer, Level.current.camera.center);
                        break;

                    case EditorMode.Rock:
                        DrawCanvas(false, 24, RockFrameBuffer, Level.current.camera.center);
                        break;

                    case EditorMode.Particle:
                        DrawCanvas(false, 12, ParticleAnimationBuffer[AnimationFrame], Level.current.camera.center);
                        break;

                    case EditorMode.Metapixel:
                        // METAPIXEL EDITOR WIP
                        break;

                    default: throw new InvalidOperationException();
                }
            }

            private static void UpdateCanvasToolsSelector()
            {
                (SpriteMap, CanvasTool)[] iconTools = {
                    (FFIcons.Brush, CanvasTool.Brush),
                    (FFIcons.Eraser, CanvasTool.Eraser),
                };

                foreach ((SpriteMap icon, CanvasTool tool) in iconTools)
                {
                    if (s_iconHoveredID == icon.Namebase && icon.frame == 1 && Mouse.left == InputState.Pressed)
                    {
                        CurrentCanvasTool = tool;
                        break;
                    }
                }
            }

            private static void UpdateAnimationController(bool canvasBig)
            {
                SpriteMap onionSkinIcon = FFIcons.Copy;

                if (Mouse.left == InputState.Pressed)
                {
                    if (s_iconHoveredID == onionSkinIcon.Namebase && onionSkinIcon.frame == 1)
                        s_renderOnionSkin ^= true;

                    int frameCount = canvasBig ? 2 : 4;
                    for (int i = 0; i < frameCount; i++)
                    {
                        if (s_iconHoveredID == $"frameSwitcher,{i}" && s_animationFrameBeingHovered == i)
                            AnimationFrame = i;
                    }
                }
            }
            
            private static void UpdateEditorSwitcher()
            {
                (SpriteMap, EditorMode)[] iconModes = {
                    (FFIcons.Hat, EditorMode.Hat),
                    (FFIcons.Cape, EditorMode.Cape),
                    (FFIcons.Rock, EditorMode.Rock),
                    (FFIcons.Particles, EditorMode.Particle),
                    (FFIcons.Metadata, EditorMode.Metapixel),
                };

                foreach ((SpriteMap icon, EditorMode mode) in iconModes)
                {
                    if (s_iconHoveredID == icon.Namebase && icon.frame == 1 && Mouse.left == InputState.Pressed)
                    {
                        CurrentMode = mode;
                        AnimationFrame = 0;
                        break;
                    }
                }
            }

            private static void DrawEditorSwitcher(Vec2 center)
            {
                EditorMode[] modes = {
                    EditorMode.Hat,
                    EditorMode.Cape,
                    EditorMode.Rock,
                    EditorMode.Particle,
                    EditorMode.Metapixel
                };
                
                Dictionary<EditorMode, SpriteMap> modeIcons = new()
                {
                    {EditorMode.Hat, FFIcons.Hat},
                    {EditorMode.Cape, FFIcons.Cape},
                    {EditorMode.Rock, FFIcons.Rock},
                    {EditorMode.Particle, FFIcons.Particles},
                    {EditorMode.Metapixel, FFIcons.Metadata}
                };

                Rectangle editorSwitcherBounds = new(center - new Vec2(8, 40), center + new Vec2(8, 40));
                Graphics.Draw(FFIcons.EditorSwitchersFrame, editorSwitcherBounds.x, editorSwitcherBounds.y, 1.1f);
                
                for (int i = 0; i < modes.Length; i++)
                {
                    EditorMode mode = modes[i];
                    SpriteMap icon = modeIcons[mode];

                    Rectangle iconBounds = new(
                        editorSwitcherBounds.x + 4,
                        editorSwitcherBounds.y + 4 + ((icon.height + 8) * i),
                        icon.w,
                        icon.h
                    );

                    if (iconBounds.Contains(Mouse.positionScreen))
                    {
                        icon.frame = 1;
                        RegisterButtonHover(icon.Namebase, $"Switch to {mode} editor");
                    }
                    else if (CurrentMode == mode)
                    {
                        icon.frame = 1;
                    }
                    else
                    {
                        icon.frame = 0;
                    }
                    
                    Graphics.Draw(icon, iconBounds.x, iconBounds.y);
                }
            }
            
            private static void DrawCanvasTools(Vec2 center)
            {
                CanvasTool[] tools = {
                    CanvasTool.Brush,
                    CanvasTool.Eraser,
                };
                
                Dictionary<CanvasTool, SpriteMap> toolIcons = new()
                {
                    {CanvasTool.Brush, FFIcons.Brush},
                    {CanvasTool.Eraser, FFIcons.Eraser},
                };
                
                Rectangle canvasToolsBounds = new(center - new Vec2(8, 36), center + new Vec2(8, 36));
                Graphics.Draw(FFIcons.CanvasToolsFrame, canvasToolsBounds.x, canvasToolsBounds.y, 1.1f);
                
                for (int i = 0; i < tools.Length; i++)
                {
                    CanvasTool tool = tools[i];
                    SpriteMap icon = toolIcons[tool];

                    Rectangle iconBounds = new(
                        canvasToolsBounds.x + 4,
                        canvasToolsBounds.y + 4 + ((icon.height + 8) * i),
                        icon.w,
                        icon.h
                    );

                    if (iconBounds.Contains(Mouse.positionScreen))
                    {
                        icon.frame = 1;
                        RegisterButtonHover(icon.Namebase, $"Use {tool} tool");
                    }
                    else if (CurrentCanvasTool == tool)
                    {
                        icon.frame = 1;
                    }
                    else
                    {
                        icon.frame = 0;
                    }
                    
                    Graphics.Draw(icon, iconBounds.x, iconBounds.y);
                }
                
                Graphics.DrawRect(new Rectangle(canvasToolsBounds.x + 4, canvasToolsBounds.Bottom - 28, 8, 8), CanvasPrimaryColor, 1f);
                Graphics.DrawRect(new Rectangle(canvasToolsBounds.x + 4, canvasToolsBounds.Bottom - 12, 8, 8), CanvasSecondaryColor, 1f);
            }

            private static void UpdateCanvas(bool canvasBig, int canvasSize, ref Color[] buffer, Vec2 center)
            {
                UpdateEditorSwitcher();
                UpdateCanvasToolsSelector();
                if (CurrentMode is EditorMode.Hat or EditorMode.Particle)
                    UpdateAnimationController(canvasBig);
                
                float canvasScale = canvasBig ? 1f : 0.75f;
                float pixelScale = canvasSize == 12 ? 8f : 4f;

                Rectangle innerCanvasBounds = new(
                    center - new Vec2(64f * canvasScale),
                    center + new Vec2(64f * canvasScale)
                );

                for (int i = 0; i < canvasSize * canvasSize; i++)
                {
                    int x = i % canvasSize;
                    int y = i / canvasSize;

                    Rectangle pixel = new(
                        innerCanvasBounds.x + (x * pixelScale),
                        innerCanvasBounds.y + (y * pixelScale),
                        pixelScale, pixelScale
                    );

                    if (pixel.Shrink(0.1f).Contains(Mouse.positionScreen))
                    {
                        switch (CurrentCanvasTool)
                        {
                            case CanvasTool.Brush:
                                if (Mouse.left == InputState.Down)
                                    buffer[i] = CanvasPrimaryColor;
                                else if (Mouse.right == InputState.Down)
                                    buffer[i] = CanvasSecondaryColor;
                                break;

                            case CanvasTool.Eraser:
                                if (Mouse.left == InputState.Down
                                    || Mouse.right == InputState.Down)
                                    buffer[i] = default;
                                break;

                            default: throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
            
            private static void DrawCanvas(bool canvasBig, int canvasSize, Color[] buffer, Vec2 center)
            {
                float canvasScale = canvasBig ? 1f : 0.75f;
                Sprite canvasSprite = canvasBig ? FFIcons.CanvasBig : FFIcons.CanvasSmall;
                float pixelScale = canvasSize == 12 ? 8f : 4f;

                Graphics.Draw(canvasSprite, center.x - canvasSprite.w / 2f, center.y - canvasSprite.h / 2f, 1f);
                Rectangle innerCanvasBounds = new(
                    center - new Vec2(64f * canvasScale),
                    center + new Vec2(64f * canvasScale)
                );
                DrawEditorSwitcher(new Vec2(canvasSprite.x - 8, canvasSprite.y + (canvasSprite.h / 2f)));
                DrawCanvasTools(new Vec2(canvasSprite.x + canvasSprite.w + 8, canvasSprite.y + (canvasSprite.h / 2f)));

                if (CurrentMode is EditorMode.Hat or EditorMode.Particle)
                    DrawAnimationController(canvasBig, new Vec2(canvasSprite.x + (canvasSprite.w / 2f), canvasSprite.y + canvasSprite.h));

                for (int i = 0; i < canvasSize * canvasSize; i++)
                {
                    int x = i % canvasSize;
                    int y = i / canvasSize;

                    Rectangle pixel = new(
                        innerCanvasBounds.x + (x * pixelScale),
                        innerCanvasBounds.y + (y * pixelScale),
                        pixelScale, pixelScale
                    );
                    Color color = buffer[i] != default
                        ? buffer[i]
                        : x % 2 == y % 2
                            ? FFColors.CanvasEmpty1
                            : FFColors.CanvasEmpty2;

                    Graphics.DrawRect(pixel, color, 1f);
                    
                    if (pixel.Shrink(0.1f).Contains(Mouse.positionScreen))
                        Graphics.DrawRect(pixel, FFColors.Focus, 1.1f, false, 0.5f);
                }
            }

            private static void DrawAnimationController(bool canvasBig, Vec2 topCenter)
            {
                int frameCount = canvasBig ? 2 : 4;
                string[] hatFrameNames = {"IDLE", "QUACK"};
                string[] particleFrameNames = {"1", "2", "3", "4"};
                
                Sprite frameSprite = canvasBig ? FFIcons.AnimationControlsBig : FFIcons.AnimationControlsSmall;
                Rectangle frameBounds = new(
                    topCenter - new Vec2(frameSprite.w / 2f, 0),
                    topCenter + new Vec2(frameSprite.w / 2f, frameSprite.h)
                );
                Graphics.Draw(frameSprite, frameBounds.x, frameBounds.y, 1.1f);
                
                SpriteMap onionSkinIcon = FFIcons.Copy;
                
                Rectangle iconBounds = new(
                    frameBounds.x + 4,
                    frameBounds.y,
                    onionSkinIcon.w,
                    onionSkinIcon.h
                );

                if (iconBounds.Contains(Mouse.positionScreen))
                {
                    onionSkinIcon.frame = 1;
                    RegisterButtonHover(onionSkinIcon.Namebase, "Toggle OnionSkin");
                }
                else if (s_renderOnionSkin)
                {
                    onionSkinIcon.frame = 1;
                }
                else
                {
                    onionSkinIcon.frame = 0;
                }
                    
                Graphics.Draw(onionSkinIcon, iconBounds.x, iconBounds.y);

                Rectangle animationFrameControlBounds = new(
                    new Vec2(frameBounds.x + 24, frameBounds.y),
                    frameBounds.br - new Vec2(4)
                );

                bool didHover = false;
                for (int i = 0; i < frameCount; i++)
                {
                    float frameSwitchWidth = animationFrameControlBounds.width / frameCount;
                    Rectangle frameSwitchBounds = new(
                        animationFrameControlBounds.x + (frameSwitchWidth * i),
                        animationFrameControlBounds.y,
                        frameSwitchWidth,
                        animationFrameControlBounds.height
                    );

                    bool beingHovered = frameSwitchBounds.Shrink(i > 0 ? 0.5f : 0, i < frameCount - 1 ? 0.5f : 0, 0f, 0f).Contains(Mouse.positionScreen);
                    string frameName = canvasBig ? hatFrameNames[i] : particleFrameNames[i];

                    if (beingHovered)
                    {
                        didHover = true;
                        s_animationFrameBeingHovered = i;
                        RegisterButtonHover($"frameSwitcher,{i}", $"Switch to frame: {frameName}");
                    }
                    else if (AnimationFrame == i)
                    {
                        beingHovered = true;
                    }
                    
                    Color textColor = beingHovered ? FFColors.PrimaryDim : FFColors.Focus;
                    Color bgColor = beingHovered ? FFColors.Focus : FFColors.PrimaryDim;
                    
                    Graphics.DrawRect(frameSwitchBounds, bgColor, 0.9f);
                    if (i < frameCount - 1)
                        Graphics.DrawLine(frameSwitchBounds.tr, frameSwitchBounds.br, FFColors.Outline, 1f, 0.95f);
                    Graphics.DrawString(frameName, frameSwitchBounds.tl + Vec2.One, textColor, 1f, scale: 0.5f);
                }

                if (!didHover)
                    s_animationFrameBeingHovered = -1;
            }
        }
    }
}