namespace DuckGame
{
    public class NMFuneral : NMEvent
    {
        public Profile profile;
        public Duck lay;

        public NMFuneral()
        {
        }

        public NMFuneral(Profile pProfile, Duck pLay)
        {
            profile = pProfile;
            lay = pLay;
        }

        public override void Activate()
        {
            if (lay != null && profile != null)
            {
                lay.isConversionMessage = true;
                lay.LayToRest(profile);
                lay.isConversionMessage = false;
                if (!Music.currentSong.Contains("MarchOfDuck"))
                    Music.Play("MarchOfDuck", false);
            }
            base.Activate();
        }
    }
}
