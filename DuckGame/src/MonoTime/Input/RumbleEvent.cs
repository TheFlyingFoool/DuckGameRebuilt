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
                    intensityInitial = 0f;
                    break;
                case RumbleIntensity.Kick:
                    intensityInitial = 0.15f;
                    break;
                case RumbleIntensity.Light:
                    intensityInitial = 0.25f;
                    break;
                case RumbleIntensity.Medium:
                    intensityInitial = 0.5f;
                    break;
                case RumbleIntensity.Heavy:
                    intensityInitial = 0.8f;
                    break;
                default:
                    intensityInitial = 0.25f;
                    break;
            }
            intensityCurrent = intensityInitial;
            switch (durationToSet)
            {
                case RumbleDuration.Pulse:
                    timeDuration = 0.075f;
                    break;
                case RumbleDuration.Short:
                    timeDuration = 0.15f;
                    break;
                case RumbleDuration.Medium:
                    timeDuration = 0.5f;
                    break;
                case RumbleDuration.Long:
                    timeDuration = 1f;
                    break;
                default:
                    timeDuration = 0.1f;
                    break;
            }
            switch (falloffToSet)
            {
                case RumbleFalloff.None:
                    timeFalloff = 0f;
                    break;
                case RumbleFalloff.Short:
                    timeFalloff = 0.1f;
                    break;
                case RumbleFalloff.Medium:
                    timeFalloff = 0.25f;
                    break;
                case RumbleFalloff.Long:
                    timeFalloff = 0.5f;
                    break;
                default:
                    timeFalloff = 0.1f;
                    break;
            }
            type = rumbleTypeToSet;
        }

        /// <summary>Create a RumbleEvent using only enum definitions</summary>
        public RumbleEvent(
          RumbleIntensity intensityToSet,
          RumbleDuration durationToSet,
          RumbleFalloff falloffToSet,
          RumbleType rumbleTypeToSet = RumbleType.Gameplay)
        {
            SetRumbleParameters(intensityToSet, durationToSet, falloffToSet, rumbleTypeToSet);
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
            intensityInitial = intensityToSet;
            intensityCurrent = intensityInitial;
            timeDuration = durationToSet;
            timeFalloff = falloffToSet;
            type = rumbleTypeToSet;
        }

        /// <summary>
        /// Updates the intensity of a RumbleEvent based on the time remaining in the falloff portion of the full duration.
        /// </summary>
        public void FallOffLinear() => intensityCurrent = (float)(1f - (timeElapsed - timeDuration) / timeFalloff) * intensityInitial;

        /// <summary>
        /// Updates the elapsed time and updates the intensity for any falloff. Returns false if the rumble is completed and should be cleaned up by RumbleManager
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            timeElapsed += 0.016f;
            if (timeElapsed >= timeDuration + timeFalloff)
                return false;
            if (timeElapsed > timeDuration)
                FallOffLinear();
            return true;
        }
    }
}
