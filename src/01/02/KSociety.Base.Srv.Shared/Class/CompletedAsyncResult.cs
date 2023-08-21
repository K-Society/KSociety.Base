namespace KSociety.Base.Srv.Shared.Class
{
    using System;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompletedAsyncResult<T> : IAsyncResult
    {
        public CompletedAsyncResult(T data)
        {
            this.Data = data;
        }

        public T Data { get; }

        #region IAsyncResult Members

        public object AsyncState => this.Data;

        public WaitHandle AsyncWaitHandle => throw new Exception("The method or operation is not implemented.");

        public bool CompletedSynchronously => true;

        public bool IsCompleted => true;

        #endregion
    }
}
