// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.TitleServiceConnection
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

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
            this.ServiceId = serviceId;
            this.Description = description;
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
