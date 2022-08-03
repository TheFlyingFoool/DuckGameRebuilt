// Decompiled with JetBrains decompiler
// Type: DuckGame.Page
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Page : Level
    {
        protected CategoryState _state;
        public static float camOffset;

        public virtual void DeactivateAll()
        {
        }

        public virtual void ActivateAll()
        {
        }

        public virtual void TransitionOutComplete()
        {
        }

        public override void Update()
        {
            Layer.HUD.camera.x = Page.camOffset;
            if (_state == CategoryState.OpenPage)
            {
                DeactivateAll();
                Page.camOffset = Lerp.FloatSmooth(Page.camOffset, 360f, 0.1f);
                if (camOffset <= 330.0)
                    return;
                TransitionOutComplete();
            }
            else
            {
                if (_state != CategoryState.Idle)
                    return;
                Page.camOffset = Lerp.FloatSmooth(Page.camOffset, -40f, 0.1f);
                if (camOffset < 0.0)
                    Page.camOffset = 0f;
                if (camOffset == 0.0)
                    ActivateAll();
                else
                    DeactivateAll();
            }
        }
    }
}
