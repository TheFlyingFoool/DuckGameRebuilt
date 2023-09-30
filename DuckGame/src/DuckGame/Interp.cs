using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    //Sorry the names suck 
    public class Interp
    {
        public Interp(bool Lerp)
        {
            CanLerp = Lerp;
        }
        public Interp()
        {
            
        }

        public struct InterpState
        {
            public Vec2 Position;
            public float Angle;
            public Vec2 Size;
            public InterpState(Vec2 pos, float ang)
            {
                Position = pos;
                Angle = ang;
                Size = new Vec2(0f, 0f);
            }
            public InterpState(Vec2 pos, float ang, Vec2 size)
            {
                Position = pos;
                Angle = ang;
                Size = size;
            }
            public InterpState(Vec2 pos, Vec2 size)
            {
                Position = pos;
                Angle = 0f;
                Size = size;
            }

        }

        protected InterpState PreviousState;
        protected InterpState CurrentState;
        protected InterpState LerpState;
        public bool CanLerp = false;

        //TODO: replace this with a tick counting system instead.
        protected TimeSpan PreviousStateUpdate = TimeSpan.Zero;
        protected TimeSpan CurrentStateUpdate = TimeSpan.Zero;
        private bool RecentLerp
        {
            get => (CurrentStateUpdate - PreviousStateUpdate).TotalMilliseconds < 30;
        }

        public Vec2 Size
        {
            get => LerpState.Size;
        }
        public float Width
        {
            get => LerpState.Size.x;
        }
        public float Height
        {
            get => LerpState.Size.y;
        }

        public Vec2 Position
        {
            get => LerpState.Position;
        }
        public float Angle
        {
            get => LerpState.Angle;
        }
        public float x
        {
            get => LerpState.Position.x;
        }
        public float y
        {
            get => LerpState.Position.y;
        }

        private float LerpAngle(float startAngle, float endAngle, float t)
        {
            float PI = (float)Math.PI;
            float difference = (endAngle - startAngle + PI) % (2 * PI) - PI;
            if (difference < -PI)
                difference += 2 * PI;
            return startAngle + difference * t;
        }
        public void CopyLerpState(Interp From)
        {
            PreviousState = From.PreviousState;
            CurrentState = From.CurrentState;
            LerpState = From.LerpState;
            CanLerp = From.CanLerp;
            PreviousStateUpdate = From.PreviousStateUpdate;
            CurrentStateUpdate = From.CurrentStateUpdate;
        }

        //TODO: clean this up
        public void UpdateLerpState(InterpState RealState, float IntraTick, bool UpdateState)
        {
            LerpState = RealState;
            if (!DGRSettings.UncappedFPS || !CanLerp || Editor.editorDraw)
                return;

            if (UpdateState)
            {
                PreviousState = CurrentState;
                CurrentState = RealState;
                PreviousStateUpdate = CurrentStateUpdate;
                CurrentStateUpdate = MonoMain.TotalGameTime;
            }

            if (PreviousState.Position != null && RecentLerp)
            {
                LerpState.Position = Lerp.Vec2Smooth(PreviousState.Position, CurrentState.Position, IntraTick);
                LerpState.Angle = LerpAngle(PreviousState.Angle, CurrentState.Angle, IntraTick);
            }
        }
        public void UpdateLerpState(Vec2 Pos, float IntraTick, bool UpdateState)
        {
            LerpState.Position = Pos;
            if (!DGRSettings.UncappedFPS || !CanLerp || Editor.editorDraw)
                return;

            if (UpdateState)
            {
                PreviousState.Position = CurrentState.Position;
                CurrentState.Position = Pos;
                PreviousStateUpdate = CurrentStateUpdate;
                CurrentStateUpdate = MonoMain.TotalGameTime;
            }

            if (PreviousState.Position != null && RecentLerp)
            {
                LerpState.Position = Lerp.Vec2Smooth(PreviousState.Position, CurrentState.Position, IntraTick);
            }
        }

        public void UpdateLerpState(Vec2 Pos, float Angle, float IntraTick, bool UpdateState)
        {
            InterpState RealState = new InterpState(Pos, Angle);
            LerpState = RealState;
            if (!DGRSettings.UncappedFPS || !CanLerp || Editor.editorDraw)
                return;

            if (UpdateState)
            {
                PreviousState = CurrentState;
                CurrentState = RealState;
                PreviousStateUpdate = CurrentStateUpdate;
                CurrentStateUpdate = MonoMain.TotalGameTime;
            }

            if (PreviousState.Position != null && RecentLerp)
            {
                LerpState.Position = Lerp.Vec2Smooth(PreviousState.Position, CurrentState.Position, IntraTick);
                LerpState.Angle = LerpAngle(PreviousState.Angle, CurrentState.Angle, IntraTick);
            }
        }
        public void UpdateLerpState(Vec2 Pos, Vec2 Size, float IntraTick, bool UpdateState)
        {
            InterpState RealState = new InterpState(Pos, Size);
            LerpState = RealState;
            if (!DGRSettings.UncappedFPS || !CanLerp || Editor.editorDraw)
                return;

            if (UpdateState)
            {
                PreviousState = CurrentState;
                CurrentState = RealState;
                PreviousStateUpdate = CurrentStateUpdate;
                CurrentStateUpdate = MonoMain.TotalGameTime;
            }

            if (PreviousState.Position != null && RecentLerp)
            {
                LerpState.Position = Lerp.Vec2Smooth(PreviousState.Position, CurrentState.Position, IntraTick);
                LerpState.Size = Lerp.Vec2Smooth(PreviousState.Size, CurrentState.Size, IntraTick);
            }
        }
        //I hate the camera class
        public void SubFrameUpdate(Vec2 Pos, float IntraTick)
        {
            InterpState RealState = new InterpState(Pos, Size);
            LerpState = RealState;
            if (!DGRSettings.UncappedFPS)
                return;

            CurrentState = RealState;

        }
    }
}
