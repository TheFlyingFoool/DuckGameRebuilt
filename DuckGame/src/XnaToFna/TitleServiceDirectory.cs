using System;

namespace XnaToFna.StubXDK.GamerServices
{
    public class TitleServiceDirectory
    {
        private EventHandler<FindServicesCompletedArgs> _FindServicesCompleted;

        public event EventHandler<FindServicesCompletedArgs> FindServicesCompleted
        {
            add => _FindServicesCompleted += value;
            remove => _FindServicesCompleted -= value;
        }

        public bool IsBusy => false;

        public void FindServicesAsync()
        {
        }
    }
}
