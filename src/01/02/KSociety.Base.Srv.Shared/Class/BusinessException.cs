// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

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
