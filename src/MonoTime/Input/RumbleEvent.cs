// Decompiled with JetBrains decompiler
// Type: DuckGame.RumbleEvent
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    /// <summary>
    /// Create a RumbleEvent and call RumbleManager.AddRumbleEvent with the event to add a rumble.
    /// </summary>
    public class RumbleEvent
    {
        public Vec2? position;
        public Profile profile;
        public float intensityInitial;
        public float intensityCurrent;
        public float timeDuration;
        public float timeElapsed;
        public float timeFalloff;
        public RumbleType type;

        public void SetRumbleParameters(
          RumbleIntensity intensityToSet,
          RumbleDuration durationToSet,
          RumbleFalloff falloffToSet,
          RumbleType rumbleTypeToSet)
        {
            switch (intensityToSet)
            {
                case RumbleIntensity.None:
                    this.intensityInitial = 0.0f;
                    break;
                case RumbleIntensity.Kick:
                    this.intensityInitial = 0.15f;
                    break;
                case RumbleIntensity.Light:
                    this.intensityInitial = 0.25f;
                    break;
                case RumbleIntensity.Medium:
                    this.intensityInitial = 0.5f;
                    break;
                case RumbleIntensity.Heavy:
                    this.intensityInitial = 0.8f;
                    break;
                default:
                    this.intensityInitial = 0.25f;
                    break;
            }
            this.intensityCurrent = this.intensityInitial;
            switch (durationToSet)
            {
                case RumbleDuration.Pulse:
                    this.timeDuration = 0.075f;
                    break;
                case RumbleDuration.Short:
                    this.timeDuration = 0.15f;
                    break;
                case RumbleDuration.Medium:
                    this.timeDuration = 0.5f;
                    break;
                case RumbleDuration.Long:
                    this.timeDuration = 1f;
                    break;
                default:
                    this.timeDuration = 0.1f;
                    break;
            }
            switch (falloffToSet)
            {
                case RumbleFalloff.None:
                    this.timeFalloff = 0.0f;
                    break;
                case RumbleFalloff.Short:
                    this.timeFalloff = 0.1f;
                    break;
                case RumbleFalloff.Medium:
                    this.timeFalloff = 0.25f;
                    break;
                case RumbleFalloff.Long:
                    this.timeFalloff = 0.5f;
                    break;
                default:
                    this.timeFalloff = 0.1f;
                    break;
            }
            this.type = rumbleTypeToSet;
        }

        /// <summary>Create a RumbleEvent using only enum definitions</summary>
        public RumbleEvent(
          RumbleIntensity intensityToSet,
          RumbleDuration durationToSet,
          RumbleFalloff falloffToSet,
          RumbleType rumbleTypeToSet = RumbleType.Gameplay)
        {
            this.SetRumbleParameters(intensityToSet, durationToSet, falloffToSet, rumbleTypeToSet);
        }

        /// <summary>
        /// Create a RumbleEvent with float-specified intensity, duration, and falloff
        /// </summary>
        public RumbleEvent(
          float intensityToSet,
          float durationToSet,
          float falloffToSet,
          RumbleType rumbleTypeToSet = RumbleType.Gameplay)
        {
            this.intensityInitial = intensityToSet;
            this.intensityCurrent = this.intensityInitial;
            this.timeDuration = durationToSet;
            this.timeFalloff = falloffToSet;
            this.type = rumbleTypeToSet;
        }

        /// <summary>
        /// Updates the intensity of a RumbleEvent based on the time remaining in the falloff portion of the full duration.
        /// </summary>
        public void FallOffLinear() => this.intensityCurrent = (float)(1.0 - ((double)this.timeElapsed - (double)this.timeDuration) / (double)this.timeFalloff) * this.intensityInitial;

        /// <summary>
        /// Updates the elapsed time and updates the intensity for any falloff. Returns false if the rumble is completed and should be cleaned up by RumbleManager
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            this.timeElapsed += 0.016f;
            if ((double)this.timeElapsed >= (double)this.timeDuration + (double)this.timeFalloff)
                return false;
            if ((double)this.timeElapsed > (double)this.timeDuration)
                this.FallOffLinear();
            return true;
        }
    }
}
