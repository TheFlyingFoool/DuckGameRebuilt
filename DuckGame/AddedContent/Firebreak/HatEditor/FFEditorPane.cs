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
            public static MetapixelEditorMode MetapixelEditorMode = MetapixelEditorMode.MyMetapixelList;
            public static CanvasTool CurrentCanvasTool = CanvasTool.Brush;
            public static Color CanvasPrimaryColor = Color.Black;
            public static Color CanvasSecondaryColor = Color.White;
            private static bool s_renderOnionSkin = false;
            private static int s_animationFrameBeingHovered = -1;
            private static bool s_lastPressInPreviousMenu = false;
            private static int s_currentlyEditingMetapixelIndex = -1;
            private static int s_scrollIndex = 0;
            private static int s_scrollLimit = 0;
            public static Dictionary<byte, MetapixelInfo> MetapixelInfo;
            private static bool s_usingSlider1 = false;
            private static bool s_usingSlider2 = false;
            private static bool s_primColPickerOpen = false;
            private static bool s_secColPickerOpen = false;
            private static Rectangle s_colPickSliderBounds;
            private static int s_colPickSliderUsedIndex = -1;

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
                        UpdateCanvas(true, 32, ref HatAnimationBuffer[AnimationFrame], current.camera.center);
                        break;

                    case EditorMode.Cape:
                        UpdateCanvas(true, 32, ref CapeFrameBuffer, current.camera.center);
                        break;

                    case EditorMode.Rock:
                        UpdateCanvas(false, 24, ref RockFrameBuffer, current.camera.center);
                        break;

                    case EditorMode.Particle:
                        UpdateCanvas(false, 12, ref ParticleAnimationBuffer[AnimationFrame], current.camera.center);
                        break;

                    case EditorMode.Metapixel:
                        UpdateMetapixelEditor(current.camera.center);
                        break;

                    default: throw new InvalidOperationException();
                }
            }

            public static void Draw()
            {
                switch (CurrentMode)
                {
                    case EditorMode.Hat:
                        DrawCanvas(true, 32, HatAnimationBuffer[AnimationFrame], current.camera.center);
                        break;

                    case EditorMode.Cape:
                        DrawCanvas(true, 32, CapeFrameBuffer, current.camera.center);
                        break;

                    case EditorMode.Rock:
                        DrawCanvas(false, 24, RockFrameBuffer, current.camera.center);
                        break;

                    case EditorMode.Particle:
                        DrawCanvas(false, 12, ParticleAnimationBuffer[AnimationFrame], current.camera.center);
                        break;

                    case EditorMode.Metapixel:
                        DrawMetapixelEditor(current.camera.center);
                        break;

                    default: throw new InvalidOperationException();
                }
            }

            private static void UpdateMetapixelEditor(Vec2 center)
            {
                if (Mouse.scrollingUp && s_scrollIndex > 0)
                    s_scrollIndex--;
                else if (Mouse.scrollingDown && s_scrollIndex < s_scrollLimit - 1)
                    s_scrollIndex++;

                s_scrollIndex = Maths.Clamp(s_scrollIndex, 0, s_scrollLimit);
                
                UpdateEditorSwitcher();
                // TODO: make interactions and stuff happen in here and not the fucking draw loop
            }
            
            private static void DrawMetapixelEditor(Vec2 center)
            {
                const float totw = 128 + 32;
                const float toth = 128;
                Rectangle editorInnerBounds = new(center - new Vec2(totw / 2, toth / 2), center + new Vec2(totw / 2, toth / 2));
                DrawEditorSwitcher(new Vec2(editorInnerBounds.x - 8, editorInnerBounds.y + (editorInnerBounds.height / 2f)));

                switch (MetapixelEditorMode)
                {
                    case MetapixelEditorMode.MyMetapixelList:
                        DrawMetapixelList(editorInnerBounds);
                        break;

                    case MetapixelEditorMode.EditMetapixel:
                        DrawEditMetapixel(editorInnerBounds);
                        break;

                    case MetapixelEditorMode.NewMetapixel:
                        DrawAddMetapixel(editorInnerBounds);
                        break;
                }

                if (s_usingSlider1 && Mouse.left == InputState.Released)
                    s_usingSlider1 = false;
                if (s_usingSlider2 && Mouse.left == InputState.Released)
                    s_usingSlider2 = false;
            }

            private static void DrawAddMetapixel(Rectangle innerBounds)
            {
                s_scrollLimit = MetapixelInfo.Count;
                Graphics.DrawRect(innerBounds, Color.Black, 0.9f);
                
                float textH = Extensions.GetStringSize("H", 0.6f).Height;
                
                Graphics.DrawFancyString("you can scroll here btw", innerBounds.tl + (Vec2.One * 2), Color.Red, 1.1f, 0.6f);
                Graphics.DrawLine(innerBounds.tl + new Vec2(2, 4 + textH), innerBounds.width - 4, 90, Color.Red, depth: 1f);

                Vec2 posPastLine = innerBounds.tl + new Vec2(2, 6 + textH);

                int i = 0;
                foreach ((byte index, MetapixelInfo info) in MetapixelInfo)
                {
                    if (i < s_scrollIndex)
                    {
                        i++;
                        continue;
                    }
                    
                    if (i - s_scrollIndex >= 17)
                        break;

                    string text = $"[{index:000}] {info.Name}";
                    float textW = Graphics.GetFancyStringWidth(text, scale: 0.6f);
                    Vec2 textDrawPos = posPastLine + new Vec2(0, (i - s_scrollIndex) * (textH + 2));
                    Rectangle textBounds = new(textDrawPos.x, textDrawPos.y, textW, textH);

                    Graphics.DrawFancyString(text, textDrawPos, textBounds.Contains(Mouse.positionScreen) ? Color.Yellow : Color.Red, 1.1f, scale: 0.6f);

                    if (Mouse.left == InputState.Pressed && textBounds.Contains(Mouse.positionScreen))
                    {
                        Metapixels.Add(new Color(index, byte.MinValue, byte.MinValue));
                        s_scrollIndex = 0;
                        MetapixelEditorMode = MetapixelEditorMode.EditMetapixel;
                        s_currentlyEditingMetapixelIndex = Metapixels.Count - 1;
                        break;
                    }
                    
                    i++;
                }
            }

            private static void DrawEditMetapixel(Rectangle innerBounds)
            {
                s_scrollLimit = 0;
                Graphics.DrawRect(innerBounds, Color.Black, 0.9f);
                
                int i = Maths.Clamp(s_currentlyEditingMetapixelIndex, 0, Metapixels.Count - 1);
                Color pixel = Metapixels[i];
                MetapixelInfo info = MetapixelInfo[pixel.r];
                
                string title = $"{pixel.r:000} / {info.Name}";
                    
                float textH = Extensions.GetStringSize("H", 0.6f).Height;
                
                Graphics.DrawFancyString(title, innerBounds.tl + (Vec2.One * 2), Color.Red, 1.1f, 0.6f);
                Graphics.DrawLine(innerBounds.tl + new Vec2(2, 4 + textH), innerBounds.width - 4, 90, Color.Red, depth: 1f);
                
                string mpDescription = $"Type: {info.MDType.Name.Substring(2)}\n\n{string.Join("\n", info.Description.SplitByLength(50))}";
                Graphics.DrawFancyString(mpDescription, innerBounds.tl + new Vec2(2, 6 + textH), Color.Red, 1.1f, scale: 0.6f);

                float yDescriptionLine = innerBounds.y + 10 + (textH * (mpDescription.Count(x => x == '\n') + 2));
                Vec2 posDescriptionLine = new(innerBounds.x + 2, yDescriptionLine);
                Graphics.DrawLine(posDescriptionLine, innerBounds.width - 4, 90, Color.Red, depth: 1f);

                bool hasG = info.MDType == typeof(Team.CustomHatMetadata.MDInt)
                         || info.MDType == typeof(Team.CustomHatMetadata.MDFloat);
                bool hasB = info.MDType == typeof(Team.CustomHatMetadata.MDVec2)
                         || info.MDType == typeof(Team.CustomHatMetadata.MDIntPair)
                         || info.MDType == typeof(Team.CustomHatMetadata.MDRandomizer)
                         || info.MDType == typeof(Team.CustomHatMetadata.MDVec2Normalized);
                
                if (hasG || hasB)
                {
                    string text = $"G {pixel.g:000}";
                    float textW = Graphics.GetStringWidth(text);
                    Graphics.DrawString(text, posDescriptionLine + new Vec2(0, 2), Color.Red, 1.1f);
                    
                    Vec2 sliderTL = posDescriptionLine + new Vec2(textW + 2, 2);
                    Rectangle sliderBounds = new(sliderTL, new Vec2(innerBounds.x + innerBounds.width - 2, sliderTL.y + Graphics._biosFont.height));
                    
                    Graphics.DrawRect(sliderBounds, Color.Red, 1.1f);

                    if (sliderBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                    {
                        s_usingSlider1 = true;
                    }

                    if (s_usingSlider1)
                    {
                        float left = sliderBounds.x;
                        float right = left + sliderBounds.width;

                        float mouseX = Maths.Clamp(Mouse.xScreen, left, right);
                        int nearestValueSelected = (int)(255 * ProgressValue.Normalize(mouseX, left, right));

                        Metapixels[i] = new Color(pixel.r, (byte)Maths.Clamp(nearestValueSelected, 0, 255), pixel.b);
                    }

                    Graphics.DrawLine(new Vec2(sliderTL.x + (float) (sliderBounds.width * ProgressValue.Normalize(Metapixels[i].g, 0, 255)), sliderTL.y), sliderBounds.height, 0, Color.Yellow, depth: 1.4f);
                }

                if (hasB)
                {
                    string text = $"B {pixel.b:000}";
                    float textW = Graphics.GetStringWidth(text);
                    Graphics.DrawString(text, posDescriptionLine + new Vec2(0, 3 + Graphics._biosFont.height), Color.Red, 1.1f);
                    
                    Vec2 sliderTL = posDescriptionLine + new Vec2(textW + 2, 3 + Graphics._biosFont.height);
                    Rectangle sliderBounds = new(sliderTL, new Vec2(innerBounds.x + innerBounds.width - 2, sliderTL.y + Graphics._biosFont.height));
                    
                    Graphics.DrawRect(sliderBounds, Color.Red, 1.1f);
                    
                    if (sliderBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                    {
                        s_usingSlider2 = true;
                    }

                    if (s_usingSlider2)
                    {
                        float left = sliderBounds.x;
                        float right = left + sliderBounds.width;

                        float mouseX = Maths.Clamp(Mouse.xScreen, left, right);
                        int nearestValueSelected = (int)(255 * ProgressValue.Normalize(mouseX, left, right));

                        Metapixels[i] = new Color(pixel.r, pixel.g, (byte)Maths.Clamp(nearestValueSelected, 0, 255));
                    }

                    Graphics.DrawLine(new Vec2(sliderTL.x + (float) (sliderBounds.width * ProgressValue.Normalize(Metapixels[i].b, 0, 255)), sliderTL.y), sliderBounds.height, 0, Color.Yellow, depth: 1.4f);
                }
                
                Rectangle backButtonBounds = new(innerBounds.x - 2 + innerBounds.width, innerBounds.y - 2 + innerBounds.height, -40, -12);
                Graphics.DrawRect(backButtonBounds, backButtonBounds.Contains(Mouse.positionScreen) ? Color.Yellow : Color.Red, 1.1f);
                Graphics.DrawString("ok", backButtonBounds.tl + new Vec2(4), backButtonBounds.Contains(Mouse.positionScreen) ? Color.Red : Color.Yellow, 1.12f, scale: 0.6f);

                if (backButtonBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                {
                    s_currentlyEditingMetapixelIndex = -1;
                    MetapixelEditorMode = MetapixelEditorMode.MyMetapixelList;
                    s_scrollIndex = 0;
                    return;
                }
                
                Rectangle deleteButtonBounds = new(backButtonBounds.x - 2, backButtonBounds.y, -40, 12);
                Graphics.DrawRect(deleteButtonBounds, deleteButtonBounds.Contains(Mouse.positionScreen) ? Color.Yellow : Color.Red, 1.1f);
                Graphics.DrawString("delete", deleteButtonBounds.tl + new Vec2(4), deleteButtonBounds.Contains(Mouse.positionScreen) ? Color.Red : Color.Yellow, 1.12f, scale: 0.6f);

                if (deleteButtonBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                {
                    Metapixels.RemoveAt(s_currentlyEditingMetapixelIndex);
                    s_currentlyEditingMetapixelIndex = -1;
                    MetapixelEditorMode = MetapixelEditorMode.MyMetapixelList;
                    s_scrollIndex = 0;
                    return;
                }
            }

            private static void DrawMetapixelList(Rectangle innerBounds)
            {
                s_scrollLimit = Metapixels.Count;
                Graphics.DrawRect(innerBounds, Color.Black, 0.9f);
                
                Rectangle addMetapixelButtonBounds = new(innerBounds.x + 1, innerBounds.y - 1 + innerBounds.height - 12, 48, 12);
                Graphics.DrawOutlinedRect(addMetapixelButtonBounds, addMetapixelButtonBounds.Contains(Mouse.positionScreen) ? Color.Yellow : Color.Black, Color.Red, 1f);
                Graphics.DrawString("add", addMetapixelButtonBounds.tl + new Vec2(4), Color.Red, 1.1f, scale: 0.6f);
                Graphics.DrawFancyString($"current metapixels: |255,255,0|{Metapixels.Count}", addMetapixelButtonBounds.tr + new Vec2(2, 4), Color.Red, 1f, scale: 0.6f);

                if (addMetapixelButtonBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                {
                    MetapixelEditorMode = MetapixelEditorMode.NewMetapixel;
                    s_scrollIndex = 0;
                    return;
                }
                if (addMetapixelButtonBounds.Contains(Mouse.positionScreen) && Mouse.right == InputState.Pressed)
                {
                    for (int i = 0; i < 32; i++)
                    {
                        Metapixels.Add(new Color(MetapixelInfo.ChooseRandom().Key, Byte.MinValue, Byte.MinValue));
                    }
                    return;
                }

                for (int i = s_scrollIndex; i < Math.Min(Metapixels.Count, 16) + s_scrollIndex; i++)
                {
                    if (i >= Metapixels.Count)
                        break;
                    
                    Color pixel = Metapixels[i];
                    if (pixel == default)
                        break;

                    (byte index, byte g, byte b) = pixel;
                    MetapixelInfo info = MetapixelInfo[index];

                    string text = $"{i + 1} / {info.Name}";
                    
                    float textH = Extensions.GetStringSize("H", 0.6f).Height;
                    float textW = Graphics.GetFancyStringWidth(text, scale: 0.6f);
                    Vec2 textDrawPos = innerBounds.tl + new Vec2(2, 2 + ((textH + 2) * (i - s_scrollIndex)));

                    Rectangle textBounds = new(textDrawPos.x, textDrawPos.y, textW, textH);
                    Graphics.DrawFancyString(text, textDrawPos, textBounds.Contains(Mouse.positionScreen) ? Color.Yellow : Color.Red, 1.1f, scale: 0.6f);
                    Rectangle deleteBoxBounds = new(textDrawPos.x + 2 + textW, textDrawPos.y, textH, textH);
                    Graphics.DrawLine(deleteBoxBounds.tl, deleteBoxBounds.br, !deleteBoxBounds.Contains(Mouse.positionScreen) ? Color.Red : Color.Yellow, 1f, 1.12f);
                    Graphics.DrawLine(deleteBoxBounds.bl, deleteBoxBounds.tr, !deleteBoxBounds.Contains(Mouse.positionScreen) ? Color.Red : Color.Yellow, 1f, 1.12f);
                    
                    if ((deleteBoxBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed) || textBounds.Contains(Mouse.positionScreen) && Mouse.middle == InputState.Pressed)
                    {
                        Metapixels.RemoveAt(i);
                        if (s_scrollIndex >= s_scrollLimit - 1)
                            s_scrollIndex--;
                        break;
                    }
                    else if (textBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                    {
                        MetapixelEditorMode = MetapixelEditorMode.EditMetapixel;
                        s_currentlyEditingMetapixelIndex = i;
                        s_scrollIndex = 0;
                        return;
                    }
                }
            }

            private static void UpdateCanvasToolsSelector()
            {
                (SpriteMap, CanvasTool)[] iconTools = {
                    (FFIcons.Brush, CanvasTool.Brush),
                    (FFIcons.Eraser, CanvasTool.Eraser),
                    (FFIcons.ColorPicker, CanvasTool.ColorPicker),
                };

                foreach ((SpriteMap icon, CanvasTool tool) in iconTools)
                {
                    if (s_iconHoveredID == icon.Namebase && icon.frame == 1 && Mouse.left == InputState.Pressed)
                    {
                        CurrentCanvasTool = tool;
                        break;
                    }
                }

                if (Keyboard.Pressed(Keys.B))
                    CurrentCanvasTool = CanvasTool.Brush;
                else if (Keyboard.Pressed(Keys.E))
                    CurrentCanvasTool = CanvasTool.Eraser;
                else if (Keyboard.Pressed(Keys.C))
                    CurrentCanvasTool = CanvasTool.ColorPicker;
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
                        s_lastPressInPreviousMenu = true;
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
                    CanvasTool.ColorPicker,
                };
                
                Dictionary<CanvasTool, SpriteMap> toolIcons = new()
                {
                    {CanvasTool.Brush, FFIcons.Brush},
                    {CanvasTool.Eraser, FFIcons.Eraser},
                    {CanvasTool.ColorPicker, FFIcons.ColorPicker},
                };
                
                Rectangle canvasToolsBounds = new(center - new Vec2(8, 36), center + new Vec2(8, 36));
                Graphics.Draw(FFIcons.CanvasToolsFrame, canvasToolsBounds.x, canvasToolsBounds.y, 1.1f);
                
                for (int i = 0; i < tools.Length; i++)
                {
                    CanvasTool tool = tools[i];
                    SpriteMap icon = toolIcons[tool];

                    Rectangle iconBounds = new(
                        canvasToolsBounds.x + 4,
                        canvasToolsBounds.y + 4 + ((icon.height+8) * i),
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

                // TODO: seperate this out to the update loop
                Rectangle primaryColorPickerBounds = new(canvasToolsBounds.x + 4, canvasToolsBounds.Bottom - 13, 8, 8);
                Graphics.DrawRect(primaryColorPickerBounds, CanvasPrimaryColor, 1f);
                Rectangle secondaryColorPickerBounds = new(canvasToolsBounds.x + 4, canvasToolsBounds.Bottom + 3, 8, 8);
                Graphics.DrawRect(secondaryColorPickerBounds, CanvasSecondaryColor, 1f);

                if (primaryColorPickerBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                {
                    s_primColPickerOpen = true;
                    s_secColPickerOpen = false;
                    s_colPickSliderUsedIndex = -1;
                }
                else if (secondaryColorPickerBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                {
                    s_primColPickerOpen = false;
                    s_secColPickerOpen = true;
                    s_colPickSliderUsedIndex = -1;
                }
                
                if (s_primColPickerOpen)
                    DrawPickColorWindow(primaryColorPickerBounds, ref CanvasPrimaryColor, ref s_primColPickerOpen);
                else if (s_secColPickerOpen)
                    DrawPickColorWindow(secondaryColorPickerBounds, ref CanvasSecondaryColor, ref s_secColPickerOpen);
            }

            private static void DrawPickColorWindow(Rectangle bounds, ref Color color, ref bool toggle)
            {
                Rectangle pickerBounds = new(bounds.tr + new Vec2(8, -20), bounds.tr + new Vec2(64 + 8, 20));

                if (Mouse.left == InputState.Pressed && !pickerBounds.Contains(Mouse.positionScreen) && !bounds.Contains(Mouse.positionScreen))
                {
                    toggle ^= true;
                    return;
                }
                
                Graphics.DrawRect(pickerBounds, Color.Black, 1.2f);
                (byte r, byte g, byte b) = color;

                byte[] bytes = {r, g, b};
                for (int i = 0; i < bytes.Length; i++)
                {
                    float sH = ((pickerBounds.height - 2) / bytes.Length) - 2;
                    const byte bz = 0;
                    Rectangle sliderBounds = new(pickerBounds.x + 1, pickerBounds.y + 1 + ((sH + 2) * i), pickerBounds.width - 2, sH);

                    float left = sliderBounds.x;
                    float right = left + sliderBounds.width;

                    float mouseX = Maths.Clamp(Mouse.xScreen, left, right);
                    int nearestValueSelected = (int)(255 * ProgressValue.Normalize(mouseX, left, right));

                    Graphics.DrawLine(sliderBounds.tl + new Vec2((float)(sliderBounds.width * ProgressValue.Normalize(bytes[i], 0, 255)), 0), sliderBounds.height, 0, Color.White, 1f, 1.5f);

                    if (s_colPickSliderUsedIndex == i)
                    {
                        switch (i)
                        {
                            case 0:
                                color = new Color((byte)Maths.Clamp(nearestValueSelected, 0, 255), color.g, color.b);
                                break;
                            case 1:
                                color = new Color(color.r, (byte)Maths.Clamp(nearestValueSelected, 0, 255), color.b);
                                break;
                            case 2:
                                color = new Color(color.r, color.g, (byte)Maths.Clamp(nearestValueSelected, 0, 255));
                                break;
                        }
                    }
                    
                    Color pc = i switch
                    {
                        0 => new Color(r, bz, bz),
                        1 => new Color(bz, g, bz),
                        2 => new Color(bz, bz, b),
                        _ => throw new InvalidOperationException("God did not intend for this to happen")
                    };
                    
                    Graphics.DrawRect(sliderBounds, pc, 1.3f);

                    if (sliderBounds.Contains(Mouse.positionScreen) && Mouse.left == InputState.Pressed)
                    {
                        s_colPickSliderBounds = sliderBounds;
                        s_colPickSliderUsedIndex = i;
                    }
                    else if (s_colPickSliderUsedIndex != -1 && Mouse.left == InputState.Released)
                    {
                        s_colPickSliderUsedIndex = -1;
                    }
                }
            }

            private static void UpdateCanvas(bool canvasBig, int canvasSize, ref Color[] buffer, Vec2 center)
            {
                UpdateEditorSwitcher();
                UpdateCanvasToolsSelector();
                if (CurrentMode is EditorMode.Hat or EditorMode.Particle)
                    UpdateAnimationController(canvasBig);

                if (s_lastPressInPreviousMenu)
                {
                    if (Mouse.left == InputState.Released)
                        s_lastPressInPreviousMenu = false;
                    else return;
                }
                
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

                            case CanvasTool.ColorPicker:
                                if (Mouse.left == InputState.Pressed && buffer[i] != default)
                                    CanvasPrimaryColor = buffer[i];
                                else if (Mouse.right == InputState.Pressed && buffer[i] != default)
                                    CanvasSecondaryColor = buffer[i];
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

                Color[]? onionSkinBuffer = null;
                if (CurrentMode is EditorMode.Hat or EditorMode.Particle)
                {
                    DrawAnimationController(canvasBig, new Vec2(canvasSprite.x + (canvasSprite.w / 2f), canvasSprite.y + canvasSprite.h));
                    
                    if (s_renderOnionSkin)
                    {
                        onionSkinBuffer = CurrentMode switch
                        {
                            EditorMode.Hat => AnimationFrame == 0 ? HatAnimationBuffer[1] : HatAnimationBuffer[0],
                            EditorMode.Particle => ParticleAnimationBuffer[AnimationFrame == 0 ? 3 : AnimationFrame - 1] 
                        };
                        
                        DrawOnionSkin(onionSkinBuffer, buffer, innerCanvasBounds, pixelScale, canvasSize);
                    }
                }
                else if (CurrentMode == EditorMode.Rock)
                {
                    DrawRockResetButton(new Vec2(canvasSprite.x + (canvasSprite.w / 2f), canvasSprite.y + canvasSprite.h));
                }

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
                        : x % 2 == y % 2 || (onionSkinBuffer is not null && onionSkinBuffer[i] != default)
                            ? FFColors.CanvasEmpty1
                            : FFColors.CanvasEmpty2;

                    Graphics.DrawRect(pixel, color, 1f);
                    
                    if (pixel.Shrink(0.1f).Contains(Mouse.positionScreen))
                        Graphics.DrawRect(pixel, FFColors.Focus, 1.1f, false, 0.5f);
                }
            }

            private static void DrawRockResetButton(Vec2 topCenter)
            {
                const float w = 56;
                const float h = 12;
                const float yOffset = 2;
                
                Rectangle frameBounds = new(
                    topCenter - new Vec2(w / 2f, -yOffset),
                    topCenter + new Vec2(w / 2f, h + yOffset)
                );
                
                bool hover = frameBounds.Contains(Mouse.positionScreen);
                
                Graphics.DrawOutlinedRect(frameBounds, hover ? Color.Yellow : Color.Black, hover ? Color.Black : Color.White, 1.2f);
                Graphics.DrawString("Reset Rock", frameBounds.tl + new Vec2(4), hover ? Color.Black : Color.White, 1.3f, scale: 0.6f);

                if (hover && Mouse.left == InputState.Pressed)
                {
                    DefaultRockBuffer.CopyTo(RockFrameBuffer, 0);
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

            private static void DrawOnionSkin(Color[] buffer, Color[] drawingBuffer, Rectangle canvasBounds, float pixelScale, int canvasSize)
            {
                for (int i = 0; i < canvasSize * canvasSize; i++)
                {
                    int x = i % canvasSize;
                    int y = i / canvasSize;

                    Rectangle pixelBounds = new(
                        canvasBounds.x + (x * pixelScale),
                        canvasBounds.y + (y * pixelScale),
                        pixelScale, pixelScale
                    );

                    Color pixelColor = buffer[i];
                    if (pixelColor != default && drawingBuffer[i] == default)
                        Graphics.DrawRect(pixelBounds, pixelColor * 0.6f, 1.05f);
                }
            }
            
            public static void ClearBuffers()
            {
                Color[][] buffersToClear =
                {
                    HatAnimationBuffer[0],
                    HatAnimationBuffer[1],
                    CapeFrameBuffer,
                    RockFrameBuffer,
                    ParticleAnimationBuffer[0],
                    ParticleAnimationBuffer[1],
                    ParticleAnimationBuffer[2],
                    ParticleAnimationBuffer[3],
                };

                foreach (Color[] buffer in buffersToClear)
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = default;
                    }
                }
                
                DefaultRockBuffer.CopyTo(RockFrameBuffer, 0);
                Metapixels.Clear();
            }
        }
    }
}