using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace XnaToFna.ProxyForms
{
    public sealed class Cursor : IDisposable
    {
        public static List<WeakReference<Cursor>> AllCursors = new List<WeakReference<Cursor>>();
        public int GlobalIndex;
        internal bool INTERNAL_IsNullCursor;
        private bool _IsDisposed;

        public IntPtr Handle => (IntPtr)GlobalIndex;

        public static Cursor Current { get; set; } = new Cursor();

        public static ProxyDrawing.Rectangle Clip
        {
            get
            {
                if (!MouseEvents.Clip.HasValue)
                    return new ProxyDrawing.Rectangle();
                Microsoft.Xna.Framework.Rectangle rectangle = MouseEvents.Clip.Value;
                return new ProxyDrawing.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            }
            set
            {
                ProxyDrawing.Rectangle rectangle = new ProxyDrawing.Rectangle();
                if (value == rectangle)
                    MouseEvents.Clip = new Microsoft.Xna.Framework.Rectangle?();
                else
                    MouseEvents.Clip = new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(value.X, value.Y, value.Width, value.Height));
            }
        }

        public static ProxyDrawing.Point Position
        {
            get
            {
                Microsoft.Xna.Framework.Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
                MouseState state = Mouse.GetState();
                return new ProxyDrawing.Point(state.X + clientBounds.X, state.Y + clientBounds.Y);
            }
            set
            {
                if (Position == value)
                    return;
                Microsoft.Xna.Framework.Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
                Mouse.SetPosition(value.X - clientBounds.X, value.Y - clientBounds.Y);
            }
        }

        public ProxyDrawing.Point HotSpot { get; internal set; }

        public object Tag { get; set; }

        private Cursor()
        {
            GlobalIndex = AllCursors.Count + 1;
            XnaToFnaHelper.Log(string.Format("[ProxyForms] Creating null cursor, globally #{0}", GlobalIndex));
            INTERNAL_IsNullCursor = true;
            AllCursors.Add(new WeakReference<Cursor>(this));
        }

        public Cursor(Type type, string resource) => throw new NotSupportedException("Loading cursors from resources currently not supported!");

        public Cursor(IntPtr handle)
        {
            GlobalIndex = AllCursors.Count + 1;
            XnaToFnaHelper.Log(string.Format("[ProxyForms] Creating reapplied cursor from #{0}, globally #{1}", handle, GlobalIndex));
            _Apply(_FromHandle(handle));
            AllCursors.Add(new WeakReference<Cursor>(this));
        }

        public Cursor(string fileName)
        {
            GlobalIndex = AllCursors.Count + 1;
            XnaToFnaHelper.Log(string.Format("[ProxyForms] Creating cursor from file, globally #{0}", GlobalIndex));
            using (Stream stream = File.OpenRead(fileName))
                _Load(stream);
            AllCursors.Add(new WeakReference<Cursor>(this));
        }

        public Cursor(Stream stream)
        {
            GlobalIndex = AllCursors.Count + 1;
            XnaToFnaHelper.Log(string.Format("[ProxyForms] Creating cursor from stream, globally #{0}", GlobalIndex));
            _Load(stream);
            AllCursors.Add(new WeakReference<Cursor>(this));
        }

        private static Cursor _FromHandle(IntPtr ptr)
        {
            int index = (int)ptr - 1;
            if (index < 0 || AllCursors.Count <= index)
                return null;
            WeakReference<Cursor> allCursor = AllCursors[index];
            Cursor target;
            if (allCursor != null && allCursor.TryGetTarget(out target))
                return target;
            AllCursors[index] = null;
            return null;
        }

        private void _Apply(Cursor other)
        {
            if (other != null)
                return;
            INTERNAL_IsNullCursor = true;
        }

        private void _Load(Stream stream)
        {
        }

        public void Dispose() => Dispose(true);

        private void Dispose(bool disposing)
        {
            if (_IsDisposed)
                return;
            _IsDisposed = true;
        }
    }
}
