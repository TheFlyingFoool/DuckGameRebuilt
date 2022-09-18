using System;
using System.Threading;

internal class Lock : IDisposable {

    private object m_pObject;

    public Lock(object pObject) {
        m_pObject = pObject;
        Monitor.Enter(m_pObject);
    }

    ~Lock() {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing)
            Monitor.Exit(m_pObject);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


}
