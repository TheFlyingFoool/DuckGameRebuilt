using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace DuckGame
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
    public sealed class DrawingContextAttribute : Attribute
    {
        public static List<DrawingContextAttribute> AllDrawingContexts = new List<DrawingContextAttribute>();

        public MethodInfo method;
        public Action action;
        public string Name;

        /// <summary>
        /// The layer this method will be invoked at for drawing
        /// </summary>
        /// <remarks>
        /// You can draw to multiple layers by using flags
        /// </remarks>
        public DrawingLayer Layer { get; }
        /// <summary>
        /// Dictates the order of when this context is drawn
        /// </summary>
        public int Priority = 0;
        /// <remarks>
        /// This will default to using the assigned method's name
        /// </remarks>
        public string? CustomID = null;

        /// <summary>
        /// Will massively speed up the execution at the cost of not
        /// being able to debug what's inside the method
        /// </summary>
        /// <remarks>
        /// This works by caching a delegate containing the actions the
        /// method does instead of using the much more costly <c>MethodInfo.Invoke</c>.
        /// But because it is no longer the original invocation, debugging becomes
        /// harder due to no longer executing your "original" code per se
        /// </remarks>
        public bool UsePrecompiledLambda = false;

        /// <summary>
        /// Whether or not this drawing context gets drawn or not
        /// </summary>
        public bool DoDraw = true;

        public DrawingContextAttribute(DrawingLayer layer = DrawingLayer.HUD)
        {
            Layer = layer;
        }

        static DrawingContextAttribute()
        {

        }
        public static void OnResults(Dictionary<Type, List<(MemberInfo MemberInfo, Attribute Attribute)>> all)
        {
            foreach ((MemberInfo vMemberInfo, Attribute vAttribute) in all[typeof(DrawingContextAttribute)])
            {
                DrawingContextAttribute item = vAttribute as DrawingContextAttribute;
                item.method = vMemberInfo as MethodInfo;
                if (!item.UsePrecompiledLambda)
                {
                    item.Name = item.CustomID ?? item.method.Name;
                    AllDrawingContexts.Add(item);
                    //ReflectionMethodsUsing.Add(item);
                }
                else
                {
                    Action precompiled = (Action)Delegate.CreateDelegate(typeof(Action), item.method);
                    if (item.CustomID == null)
                    {
                        item.CustomID = item.method.Name;
                    }
                    item.action = precompiled;
                    item.Name = item.method.Name;
                    AllDrawingContexts.Add(item);
                }
            }
            AllDrawingContexts = AllDrawingContexts.OrderByDescending(x => x.Priority).ToList();
        }
        public static DrawingLayer? DrawingLayerFromLayer(Layer layer)
        {
            switch(layer.name)
            {
                case "PREDRAW":
                    return DrawingLayer.PreDrawLayer;
                case "PARALLAX":
                    return DrawingLayer.Parallax;
                case "VIRTUAL":
                    return DrawingLayer.Virtual;
                case "BACKGROUND":
                    return DrawingLayer.Background;
                case "GAME":
                    return DrawingLayer.Game;
                case "BLOCKS":
                    return DrawingLayer.Blocks;
                case "GLOW":
                    return DrawingLayer.Glow;
                case "LIGHTING":
                    return DrawingLayer.Lighting;
                case "FOREGROUND":
                    return DrawingLayer.Foreground;
                case "HUD":
                    return DrawingLayer.HUD;
                case "CONSOLE":
                    return DrawingLayer.Console;
                default:
                    return null; // idk man firebreak code returned null in this case so i guess ill do so
            }
        }

        public static void ExecuteAll(DrawingLayer? layer)
        {
            if (layer is null)
                return;
            foreach (DrawingContextAttribute pair in AllDrawingContexts)
            {
                if (pair.DoDraw && pair.Layer.HasFlag(layer))
                {
                    if (pair.method != null)
                    {
                        pair.method.Invoke(null, null);
                    }
                    else if (pair.action != null)
                    {
                        pair.action();
                    }
                }
            }
        }
    }

    //[Flags] no thanks :)))) 
    public enum DrawingLayer
    {
        PreDrawLayer = 1, //[Obsolete]  yea why
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
