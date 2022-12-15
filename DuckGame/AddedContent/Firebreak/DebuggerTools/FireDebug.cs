using System;
using System.Collections.Generic;

namespace DuckGame
{
    public static class FireDebug
    {
        public static bool Debugging => MonoMain.firebreak;

        [PostInitialize]
        public static void OnPostInitialize()
        {
            if (!Debugging)
                return;
            
            foreach (Furniture furni in RoomEditor.AllFurnis())
            {
                Profiles.experienceProfile.SetNumFurnitures(furni.index, 999);
            }

            RoomEditor.maxFurnitures = int.MaxValue;
            
            if (Profiles.experienceProfile.xp < 100000)
            {
                Profiles.experienceProfile.xp = 100000;
            }
            
            //Profiles.experienceProfile.unlocks = new List<string>()
            //{
            //    "MOOGRAV",
            //    "HELMY",
            //    "EXPLODEYCRATES",
            //    "INFAMMO",
            //    "GUNEXPL",
            //    "HATTY2",
            //    "HATTY1",
            //    "WINPRES",
            //    "SHOESTAR",
            //    "QWOPPY",
            //    "JETTY",
            //    "CORPSEBLOW",
            //    "BASEMENTKEY",
            //    "ULTIMATE"
            //};
        }
    }
}