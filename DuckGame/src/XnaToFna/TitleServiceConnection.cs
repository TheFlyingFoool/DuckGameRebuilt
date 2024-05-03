using System;
using System.ComponentModel;
using System.Net;

namespace XnaToFna.StubXDK.GamerServices
{
    public class TitleServiceConnection : IDisposable
    {
        private readonly int ServiceId;
        private readonly TitleServiceDescription Description;

        public event EventHandler<AsyncCompletedEventArgs> ConnectCompleted;

        public TitleServiceConnectionStatus Status => 0;

        public TitleServiceConnection(int serviceId, TitleServiceDescription description)
        {
            ServiceId = serviceId;
            Description = description;
        }

        public void ConnectAsync()
        {
        }

        public HttpWebRequest CreateWebRequest(int port, Uri uri)
        {
            if (port != 80)
                throw new NotSupportedException("Creating HTTP requests for port != 80 not supported");
            return WebRequest.CreateHttp(uri);
        }

        public void Dispose()
        {
        }
    }
}
