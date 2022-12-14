// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyForms.Control
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Collections.Generic;
using XnaToFna.ProxyDrawing;

namespace XnaToFna.ProxyForms
{
    public class Control : IDisposable
    {
        public static List<WeakReference<Control>> AllControls = new List<WeakReference<Control>>();
        public int GlobalIndex;
        public Form Form;
        protected bool _IsDisposed;

        public IntPtr Handle => (IntPtr)GlobalIndex;

        public static Point MousePosition => Cursor.Position;

        public virtual Rectangle Bounds { get; set; }

        protected virtual Rectangle _ClientRectangle { get; set; }

        public Rectangle ClientRectangle => _ClientRectangle;

        public virtual Point Location { get; set; }

        public virtual Cursor Cursor { get; set; }

        public virtual bool Focused { get; protected set; }

        public event EventHandler MouseEnter;

        public event MouseEventHandler MouseMove;

        public event EventHandler MouseHover;

        public event MouseEventHandler MouseDown;

        public event MouseEventHandler MouseWheel;

        public event MouseEventHandler MouseUp;

        public event EventHandler MouseLeave;

        public bool IsDisposed => _IsDisposed;

        public Control()
        {
            GlobalIndex = AllControls.Count + 1;
            XnaToFnaHelper.Log(string.Format("[ProxyForms] Creating control {0}, globally #{1}", GetType().Name, GlobalIndex));
            AllControls.Add(new WeakReference<Control>(this));
        }

        public static Control FromHandle(IntPtr ptr)
        {
            int index = (int)ptr - 1;
            if (index < 0 || AllControls.Count <= index)
                return null;
            WeakReference<Control> allControl = AllControls[index];
            Control target;
            if (allControl != null && allControl.TryGetTarget(out target))
                return target;
            AllControls[index] = null;
            return null;
        }

        public Form FindForm() => Form ?? GameForm.Instance;

        public void SetBounds(int x, int y, int w, int h) => Bounds = new Rectangle(x, y, w, h);

        protected virtual void CreateHandle()
        {
        }

        public object Invoke(Delegate method) => method.DynamicInvoke();

        public object Invoke(Delegate method, params object[] args) => method.DynamicInvoke(args);

        public IAsyncResult BeginInvoke(Delegate method) => new SyncResult(method.DynamicInvoke());

        public IAsyncResult BeginInvoke(Delegate method, params object[] args) => new SyncResult(method.DynamicInvoke(args));

        public object EndInvoke(IAsyncResult result) => result.AsyncState;

        public Rectangle RectangleToScreen(Rectangle r)
        {
            Rectangle bounds = Bounds;
            return new Rectangle(r.X + bounds.X, r.Y + bounds.Y, r.Width, r.Height);
        }

        public Rectangle RectangleToClient(Rectangle r)
        {
            Rectangle bounds = Bounds;
            return new Rectangle(r.X - bounds.X, r.Y - bounds.Y, r.Width, r.Height);
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_IsDisposed)
                return;
            _IsDisposed = true;
        }

        protected virtual void SetVisibleCore(bool visible)
        {
        }

        protected virtual void WndProc(ref Message msg)
        {
        }
    }
}
