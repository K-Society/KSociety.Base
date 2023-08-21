namespace KSociety.Base.Srv.Shared.Class
{
    using System;

    public abstract class BusinessException : Exception
    {
        public string ErrorCode { get; private set; }

        protected BusinessException(string errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
}
