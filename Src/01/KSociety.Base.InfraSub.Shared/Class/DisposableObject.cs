using System;

namespace KSociety.Base.InfraSub.Shared.Class
{
    public abstract class DisposableObject : IDisposable
    {
        protected bool Disposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableObject()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (disposing)
            {
                DisposeManagedResources();
            }

            DisposeUnmanagedResources();
            Disposed = true;
        }

        /// <summary>
        /// DisposeManagedResources
        /// </summary>
        protected virtual void DisposeManagedResources() { }

        /// <summary>
        /// DisposeUnmanagedResources
        /// </summary>
        protected virtual void DisposeUnmanagedResources() { }
    }
}