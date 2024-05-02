using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XnaToFna.StubXDK.GamerServices
{
    public class FindServicesCompletedArgs : EventArgs
    {
        public ReadOnlyCollection<TitleServiceDescription> Services => new ReadOnlyCollection<TitleServiceDescription>(new List<TitleServiceDescription>());
    }
}
