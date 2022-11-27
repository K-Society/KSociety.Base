using System;

namespace KSociety.Base.Srv.Shared.Class
{
    public abstract class BusinessException : Exception
    {
        public string ErrorCode { get; private set; }

        protected BusinessException(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}