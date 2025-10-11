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
        public bool CanAngleLerp = true;

        private bool UpdatedOnce = false;
        public bool FlipUpdate = false;

        // oh god
        public Interp ParentInterp = null;
        public bool SpecialAngleResetParentTrackingForHat = false;

        //TODO: replace this with a tick counting system instead.
        protected TimeSpan PreviousStateUpdate = TimeSpan.Zero;
        protected TimeSpan CurrentStateUpdate = TimeSpan.Zero;
        private bool RecentLerp
        {
            //LMAO -Lucky
            get => Recorderator.Playing ? true : (CurrentStateUpdate - PreviousStateUpdate).TotalMilliseconds < 30;
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
        private bool ShouldAbortLerp
        {
            get => !DGRSettings.UncappedFPS || !CanLerp || Editor.editorDraw || Duck.renderingIcon;
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
            if (ShouldAbortLerp)
                return;

            if (UpdateState)
            {
                if (CurrentStateUpdate != MonoMain.TotalGameTime)
                {
                    PreviousState = CurrentState;
                    PreviousStateUpdate = CurrentStateUpdate;
                    CurrentStateUpdate = MonoMain.TotalGameTime;
                }
                CurrentState = RealState;
                UpdatedOnce = true;
            }
            if (FlipUpdate)
            {
                PreviousState = CurrentState;
                FlipUpdate = false;
            }

            if (PreviousState.Position != null && RecentLerp && UpdatedOnce)
            {
                LerpState.Position = Lerp.Vec2Smooth(PreviousState.Position, CurrentState.Position, IntraTick);
                LerpState.Angle = LerpAngle(PreviousState.Angle, CurrentState.Angle, IntraTick);
                if(SpecialAngleResetParentTrackingForHat && ParentInterp != null && PreviousState.Angle != CurrentState.Angle)
                {
                    LerpState.Position = CurrentState.Position - (ParentInterp.CurrentState.Position - Lerp.Vec2Smooth(ParentInterp.PreviousState.Position, ParentInterp.CurrentState.Position, IntraTick));
                }
                if (!CanAngleLerp)
                    LerpState.Angle = CurrentState.Angle;
            }
        }
        public void UpdateLerpState(Vec2 Pos, float IntraTick, bool UpdateState)
        {
            LerpState.Position = Pos;
            if (ShouldAbortLerp)
                return;

            if (UpdateState)
            {
                if (CurrentStateUpdate != MonoMain.TotalGameTime)
                {
                    PreviousState.Position = CurrentState.Position;
                    PreviousStateUpdate = CurrentStateUpdate;
                    CurrentStateUpdate = MonoMain.TotalGameTime;
                }
                CurrentState.Position = Pos;
                UpdatedOnce = true;
            }
            if (FlipUpdate)
            {
                PreviousState = CurrentState;
                FlipUpdate = false;
            }

            if (PreviousState.Position != null && RecentLerp && UpdatedOnce)
            {
                LerpState.Position = Lerp.Vec2Smooth(PreviousState.Position, CurrentState.Position, IntraTick);
            }
        }

        public void UpdateLerpState(Vec2 Pos, float Angle, float IntraTick, bool UpdateState)
        {
            InterpState RealState = new InterpState(Pos, Angle);
            LerpState = RealState;
            if (ShouldAbortLerp)
                return;

            if (UpdateState)
            {
                if (CurrentStateUpdate != MonoMain.TotalGameTime)
                {
                    PreviousState = CurrentState;
                    PreviousStateUpdate = CurrentStateUpdate;
                    CurrentStateUpdate = MonoMain.TotalGameTime;
                }
                CurrentState = RealState;
                UpdatedOnce = true;
            }
            if (FlipUpdate)
            {
                PreviousState = CurrentState;
                FlipUpdate = false;
            }

            if (PreviousState.Position != null && RecentLerp && UpdatedOnce)
            {
                LerpState.Position = Lerp.Vec2Smooth(PreviousState.Position, CurrentState.Position, IntraTick);
                LerpState.Angle = LerpAngle(PreviousState.Angle, CurrentState.Angle, IntraTick);
                if (SpecialAngleResetParentTrackingForHat && ParentInterp != null && PreviousState.Angle != CurrentState.Angle)
                {
                    LerpState.Position = CurrentState.Position - (ParentInterp.CurrentState.Position - Lerp.Vec2Smooth(ParentInterp.PreviousState.Position, ParentInterp.CurrentState.Position, IntraTick));
                }
                if (!CanAngleLerp)
                    LerpState.Angle = CurrentState.Angle;
            }
        }
        public void UpdateLerpState(Vec2 Pos, Vec2 Size, float IntraTick, bool UpdateState)
        {
            InterpState RealState = new InterpState(Pos, Size);
            LerpState = RealState;
            if (ShouldAbortLerp)
                return;

            if (UpdateState)
            {
                if (CurrentStateUpdate != MonoMain.TotalGameTime)
                {
                    PreviousState = CurrentState;
                    PreviousStateUpdate = CurrentStateUpdate;
                    CurrentStateUpdate = MonoMain.TotalGameTime;
                }
                CurrentState = RealState;
                UpdatedOnce = true;
            }
            if (FlipUpdate)
            {
                PreviousState = CurrentState;
                FlipUpdate = false;
            }

            if (PreviousState.Position != null && RecentLerp && UpdatedOnce)
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
