using System.Linq;
using Microsoft.Xna.Framework;

namespace DuckGame.AddedContent.Drake.Utils;

public static class InputData
{
    public static bool KeyPressed(Keys key) => Keyboard.Pressed(key);
    public static bool KeyReleased(Keys key) => Keyboard.Released(key);
    public static bool KeyDown(Keys key) => Keyboard.Down(key);

    public static bool MouseLeftPressed() => Mouse.left == InputState.Pressed;
    public static bool MouseLeftReleased() => Mouse.left == InputState.Released;
    public static bool MouseLeftDown() => Mouse.left == InputState.Down;

    public static bool MouseRightPressed() => Mouse.right == InputState.Pressed;
    public static bool MouseRightReleased() => Mouse.right == InputState.Released;
    public static bool MouseRightDown() => Mouse.right == InputState.Down;

    public static bool MouseMiddlePressed() => Mouse.middle == InputState.Pressed;
    public static bool MouseMiddleReleased() => Mouse.middle == InputState.Released;
    public static bool MouseMiddleDown() => Mouse.middle == InputState.Down;

    public static float MouseScroll => Mouse.scroll;

    public static Vector2 MouseScreenPos => Mouse.mousePos;

    public static Vector2 MouseGamePos => Mouse.positionScreen;
    
    public static Vector2 MouseProjectedPosition => Graphics.polyBatcher.TransformVector(MouseScreenPos);

    public static Vector2 ViewportSize => new Vec2(Graphics.device.Viewport.Width, Graphics.device.Viewport.Height);
    public static Vector2 CurrentLayerScreenMax => Graphics.currentLayer.camera.transformScreenVector(new Vec2(Graphics.device.Viewport.Width, Graphics.device.Viewport.Height));
    public static Vector2 CurrentLayerScreenMin => Graphics.currentLayer.camera.transformScreenVector(new Vec2(0));
    public static Keys[] KeysPressedThisFrame => Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys().Cast<Keys>().ToArray();
    private static char[] CharsPressedThisFrame => KeysPressedThisFrame.Select(key => Keyboard.KeyToChar(key)).ToArray();
}