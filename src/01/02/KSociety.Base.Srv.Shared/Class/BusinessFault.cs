// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Shared.Class
{
    using System.Runtime.Serialization;

    [DataContract]
    public class BusinessFault
    {
        [DataMember] public string ErrorCode { get; set; }

        [DataMember] public string Message { get; set; }

        public BusinessFault(string errorCode, string message)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
        }
    }
}
