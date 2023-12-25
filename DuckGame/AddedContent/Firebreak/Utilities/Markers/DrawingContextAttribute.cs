using DuckGame;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AddedContent.Firebreak
{
    internal static partial class Marker
    {
        /// <summary>
        /// Makes the method using this attribute a DrawingContext. This
        /// means that whatever is inside the method will be executed in
        /// a global drawing method using the selected layer
        /// </summary>
        /// <example>
        /// <code lang='cs'>
        /// // Will draw "DEBUG MODE" at the coordinates (x:16, y:16)
        /// // on the HUD layer and the Foreground layer.
        /// [DrawingContext(DrawLayer.HUD | DrawLayer.Foreground)]
        /// public static void Foo()
        /// {
        ///     Graphics.DrawString("DEBUG MODE", new Vec2(16, 16), Color.White);
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// This attribute will not work well if you add parameters to
        /// the method. Please just keep it parameter-less, thanks.
        /// </remarks>
        [AttributeUsage(AttributeTargets.Method, Inherited = false)]
        internal sealed class DrawingContextAttribute : MarkerAttribute
        {
            public static List<DrawingContextAttribute> AllDrawingContexts = new();

            public string Name;

            /// <summary>
            /// The layer this method will be invoked at for drawing
            /// </summary>
            /// <remarks>
            /// You can draw to multiple layers by using flags
            /// </remarks>
            public DrawingLayer Layer { get; }

            /// <remarks>
            /// This will default to using the assigned method's name
            /// </remarks>
            public string? CustomID = null;

            /// <summary>
            /// Whether or not this drawing context gets drawn or not
            /// </summary>
            public bool DoDraw = true;

            public DrawingContextAttribute(DrawingLayer layer = DrawingLayer.HUD)
            {
                Layer = layer;
            }

            protected override void Implement()
            {
                Name = CustomID ?? Member.Name;
                AllDrawingContexts.Add(this);
            }

            public static DrawingLayer? DrawingLayerFromLayer(Layer layer)
            {
                return layer.name switch
                {
                    "PREDRAW" => DrawingLayer.PreDrawLayer,
                    "PARALLAX" => DrawingLayer.Parallax,
                    "VIRTUAL" => DrawingLayer.Virtual,
                    "BACKGROUND" => DrawingLayer.Background,
                    "GAME" => DrawingLayer.Game,
                    "BLOCKS" => DrawingLayer.Blocks,
                    "GLOW" => DrawingLayer.Glow,
                    "LIGHTING" => DrawingLayer.Lighting,
                    "FOREGROUND" => DrawingLayer.Foreground,
                    "HUD" => DrawingLayer.HUD,
                    "CONSOLE" => DrawingLayer.Console,
                    _ => null
                };
            }

            public static void ExecuteAll(DrawingLayer layer)
            {
                foreach (DrawingContextAttribute marker in AllDrawingContexts)
                {
                    if (marker.DoDraw && marker.Layer.HasFlag(layer))
                    {
                        ((MethodInfo)marker.Member).Invoke(null, null);
                    }
                }
            }
        }
    }
    
    [Flags]
    public enum DrawingLayer
    {
        PreDrawLayer = 1,
        Parallax = 2,
        Virtual = 4,
        Background = 8,
        Game = 16,
        Blocks = 32,
        Glow = 64,
        Lighting = 128,
        Foreground = 256,
        HUD = 512,
        Console = 1024,
    }
}