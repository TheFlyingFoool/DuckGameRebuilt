using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DuckGame;

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
    public static List<MemberAttributePair<MethodInfo, DrawingContextAttribute>> ReflectionMethodsUsing = new();
    public static List<(Action Action, DrawingContextAttribute Attribute)> PrecompiledMethodsUsing = new();
    
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
    public int Priority { get; set; } = 0;
    /// <remarks>
    /// This will default to using the assigned method's name
    /// </remarks>
    public string? CustomID { get; set; } = null;

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
    public bool UsePrecompiledLambda { get; set; } = false;

    /// <summary>
    /// Whether or not this drawing context gets drawn or not
    /// </summary>
    public bool DoDraw { get; set; } = true;

    public DrawingContextAttribute(DrawingLayer layer = DrawingLayer.HUD)
    {
        Layer = layer;
    }

    static DrawingContextAttribute()
    {
        MemberAttributePair<MethodInfo, DrawingContextAttribute>.RequestSearch(all =>
        {
            for (int i = 0; i < all.Count; i++)
            {
                var item = all[i];
                if (!item.Attribute.UsePrecompiledLambda)
                {
                    ReflectionMethodsUsing.Add(item);
                }
                else
                {
                    var precompiled = (Action) Delegate.CreateDelegate(typeof(Action), item.MemberInfo);

                    item.Attribute.CustomID ??= item.MemberInfo.Name;
                
                    PrecompiledMethodsUsing.Add((precompiled, item.Attribute));
                }
            }
        });
    }

    public static DrawingLayer? DrawingLayerFromLayer(Layer layer)
    {
        DrawingLayer? dLayer = null;
        if (layer == DuckGame.Layer.PreDrawLayer)
            dLayer = DrawingLayer.PreDrawLayer;
        else if (layer == DuckGame.Layer.Parallax)
            dLayer = DrawingLayer.Parallax;
        else if (layer == DuckGame.Layer.Virtual)
            dLayer = DrawingLayer.Virtual;
        else if (layer == DuckGame.Layer.Background)
            dLayer = DrawingLayer.Background;
        else if (layer == DuckGame.Layer.Game)
            dLayer = DrawingLayer.Game;
        else if (layer == DuckGame.Layer.Blocks)
            dLayer = DrawingLayer.Blocks;
        else if (layer == DuckGame.Layer.Glow)
            dLayer = DrawingLayer.Glow;
        else if (layer == DuckGame.Layer.Lighting)
            dLayer = DrawingLayer.Lighting;
        else if (layer == DuckGame.Layer.Foreground)
            dLayer = DrawingLayer.Foreground;
        else if (layer == DuckGame.Layer.HUD)
            dLayer = DrawingLayer.HUD;
        else if (layer == DuckGame.Layer.Console)
            dLayer = DrawingLayer.Console;

        return dLayer;
    }
    
    public static void ExecuteAll(DrawingLayer? layer)
    {
        if (layer is null)
            return;

        foreach (var pair in ReflectionMethodsUsing
                     .Select<MemberAttributePair<MethodInfo, DrawingContextAttribute>, 
                         (object ActionOrMethodInfo, DrawingContextAttribute Attribute)>(x => (x.MemberInfo, x.Attribute))
                     .Concat(PrecompiledMethodsUsing
                         .Select<(Action Action, DrawingContextAttribute Attribute), 
                             (object ActionOrMethodInfo, DrawingContextAttribute Attribute)>(x => (x.Action, x.Attribute)))
                     .OrderByDescending(x => x.Attribute.Priority)
                     .ToArray())
        {
            if (!pair.Attribute.DoDraw)
                continue;
            
            if (pair.Attribute.Layer.HasFlag(layer))
            {
                switch (pair.ActionOrMethodInfo)
                {
                    case MethodInfo mi:
                        mi.Invoke(null, null);
                        break;
                    case Action a:
                        a.Invoke();
                        break;
                }
            }
        }
    }
}


//FIIIIRREBREAAAAK learn how flags work pls thanks :(
// SORRY
[Flags]
public enum DrawingLayer
{
    [Obsolete] PreDrawLayer = 1,
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