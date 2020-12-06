using System;
using System.Threading;

namespace KSociety.Base.Srv.Shared.Class
{
    public class CompletedAsyncResult<T> : IAsyncResult
    {
        public CompletedAsyncResult(T data)
        { Data = data; }

        public T Data { get; }

        #region IAsyncResult Members
        public object AsyncState => Data;

        public WaitHandle AsyncWaitHandle => throw new Exception("The method or operation is not implemented.");

        public bool CompletedSynchronously => true;

        public bool IsCompleted => true;

        #endregion
    }
}
