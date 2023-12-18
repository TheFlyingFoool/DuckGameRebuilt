using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DuckGame
{
    internal class updateCorderator : IUpdateable
    {
        public bool Enabled
        {
            get
            {
                return true;
            }
        }
        public int UpdateOrder
        {
            get
            {
                return 0;
            }
        }
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public static bool Patched;
        public static void OnLevelChange()
        {
            if (zeCorder != null && Recorderator.RecordType == 2 || (Recorderator.RecordType == 1 && Recorderator.Clipped))
            {
                zeCorder.SaveToFile(lastLevel);
                zeCorder = null;

                Recorderator.Clipped = false;
            }
            if (!(Level.current is ReplayLevel)) Corderator.instance = null;
            SomethingSomethingVessel.somethingIndex = 0;
            Recorderator.Playing = false;
        }
        public static Corderator zeCorder;
        public static Level lastLevel;
        public void Update(GameTime gameTime)
        {
            //uncomment this when recorderator is a thing
            if (Level.current != null)
            {
                if (lastLevel != Level.current) OnLevelChange();
                if (Recorderator.RecordType != 0)
                {
                    bool modded = ModLoader.modHash != "nomods";
                    if (Level.current is GameLevel && Level.current.things.OfType<Corderator>().Count() == 0 && (!modded || Recorderator.DoRecordMods))
                    {
                        zeCorder = new Corderator();
                        Level.Add(zeCorder);
                    }
                }
                lastLevel = Level.current;
            }
        }
    }
}