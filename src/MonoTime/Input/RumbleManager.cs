// Decompiled with JetBrains decompiler
// Type: DuckGame.RumbleManager
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>
    /// The RumbleManager keeps a list of RumbleEvents added through AddRumbleEvent calls, and each frame evaluates each event,
    /// looking for profiles it should effect.
    /// 
    /// The intensity of a rumble is affected by duration or distance, depending on the type.
    /// 
    /// The modified intensity of every rumble affecting a Profile is added to that Profile's active controller, and then each controller
    /// is Rumbled at the intensity specified in this accumulated value.
    /// 
    /// RumbleEvents can be added as :
    /// - positional (find all Profiles with ducks nearby to the rumble's position and rumble them, modifying the intensity by distance falloff
    /// - profile (rumble the specified profile. If no profile is set, rumble all active profiles)
    /// </summary>
    public static class RumbleManager
    {
        private const float RUMBLE_DISTANCE_MAX = 512f;
        private const float RUMBLE_DISTANCE_MIN = 32f;
        private static List<RumbleEvent> ListRumbleEvents = new List<RumbleEvent>();

        /// <summary>
        /// Evaluates whether we're in a "game" setting, in regards to rumble
        /// Either in an actual match level, or in team select 2
        /// </summary>
        private static bool isInGameForRumble() => Level.core.gameInProgress || Level.current is TeamSelect2;

        /// <summary>
        ///  Add a Rumble Event after all properties have been set.
        /// </summary>
        public static void AddRumbleEvent(RumbleEvent rumbleEvent)
        {
            if (rumbleEvent.intensityInitial <= 0.0)
                return;
            RumbleManager.ListRumbleEvents.Add(rumbleEvent);
        }

        /// <summary>
        /// Add a Rumble that will affect all Profiles whose duck is near the rumble's position (with distance attenuation)
        /// Positional rumbles are always type Gameplay.
        /// </summary>
        public static void AddRumbleEvent(Vec2 positionToSet, RumbleEvent rumbleEvent)
        {
            rumbleEvent.position = new Vec2?(positionToSet);
            rumbleEvent.type = RumbleType.Gameplay;
            RumbleManager.AddRumbleEvent(rumbleEvent);
        }

        /// <summary>Add a Rumble for a specific profile.</summary>
        public static void AddRumbleEvent(Profile profileToRumble, RumbleEvent rumbleEvent)
        {
            if (profileToRumble == null || !profileToRumble.localPlayer)
                return;
            rumbleEvent.profile = profileToRumble;
            RumbleManager.AddRumbleEvent(rumbleEvent);
        }

        /// <summary>Add a rumble for all profiles</summary>
        public static void AddRumbleEventForAll(RumbleEvent rumbleEvent) => RumbleManager.AddRumbleEvent(rumbleEvent);

        public static void ClearRumbles(RumbleType? rumbleType)
        {
            if (rumbleType.HasValue)
                RumbleManager.ListRumbleEvents.RemoveAll(rumble =>
               {
                   int type = (int)rumble.type;
                   RumbleType? nullable = rumbleType;
                   int valueOrDefault = (int)nullable.GetValueOrDefault();
                   return type == valueOrDefault & nullable.HasValue;
               });
            else
                RumbleManager.ListRumbleEvents.Clear();
        }

        /// <summary>
        /// Each frame we total up the intensities from various rumbles and add them to a device for a total intensity to rumble the controller by.
        /// </summary>
        private static void AddIntensityToDevice(InputDevice controllerToSet, float intensityToAdd)
        {
            if (controllerToSet == null)
                return;
            controllerToSet.rumbleIntensity += intensityToAdd;
        }

        public static void Update()
        {
            if (!Graphics.inFocus)
            {
                RumbleManager.ClearRumbles(new RumbleType?());
            }
            else
            {
                List<Profile> active = Profiles.active;
                foreach (Profile profile in active)
                {
                    if (profile != null && profile.inputProfile != null && profile.inputProfile.lastActiveDevice != null)
                        profile.inputProfile.lastActiveDevice.rumbleIntensity = 0.0f;
                }
                for (int index = RumbleManager.ListRumbleEvents.Count - 1; index >= 0; --index)
                {
                    RumbleEvent listRumbleEvent = RumbleManager.ListRumbleEvents[index];
                    if (listRumbleEvent.type != RumbleType.Gameplay || !MonoMain.shouldPauseGameplay)
                    {
                        if (listRumbleEvent.position.HasValue)
                        {
                            foreach (Profile profile in active)
                            {
                                if (profile != null && profile.localPlayer && profile.duck != null && (listRumbleEvent.profile == null || listRumbleEvent.profile == profile))
                                {
                                    float num1 = Vec2.Distance(listRumbleEvent.position.Value, profile.duck.cameraPosition);
                                    float num2 = 1f;
                                    if ((double)num1 > 32.0)
                                    {
                                        if ((double)num1 <= 512.0)
                                            num2 = (float)(1.0 - ((double)num1 - 32.0 > 0.0 ? (double)num1 - 32.0 : 0.0) / 512.0);
                                        else
                                            continue;
                                    }
                                    RumbleManager.AddIntensityToDevice(profile.inputProfile.lastActiveDevice, listRumbleEvent.intensityCurrent * (num2 * num2));
                                }
                            }
                        }
                        else if (listRumbleEvent.profile == null)
                        {
                            foreach (Profile profile in active)
                            {
                                if (profile != null && profile.inputProfile != null && profile.localPlayer && profile.inputProfile.lastActiveDevice != null)
                                    RumbleManager.AddIntensityToDevice(profile.inputProfile.lastActiveDevice, listRumbleEvent.intensityCurrent);
                            }
                        }
                        else if (listRumbleEvent.profile != null && listRumbleEvent.profile.inputProfile != null && listRumbleEvent.profile.inputProfile.lastActiveDevice != null)
                            RumbleManager.AddIntensityToDevice(listRumbleEvent.profile.inputProfile.lastActiveDevice, listRumbleEvent.intensityCurrent);
                        if (!listRumbleEvent.Update())
                            RumbleManager.ListRumbleEvents.RemoveAt(index);
                    }
                }
                if ((double)Options.Data.rumbleIntensity <= 0.0)
                    return;
                foreach (Profile profile in active)
                {
                    if (profile != null && profile.inputProfile != null)
                    {
                        InputDevice lastActiveDevice = profile.inputProfile.lastActiveDevice;
                        lastActiveDevice?.Rumble(lastActiveDevice.rumbleIntensity * lastActiveDevice.RumbleIntensityModifier(), lastActiveDevice.rumbleIntensity * lastActiveDevice.RumbleIntensityModifier());
                    }
                }
            }
        }
    }
}
