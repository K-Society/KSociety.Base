using System.Runtime.Serialization;

namespace KSociety.Base.Srv.Shared.Class;

[DataContract]
public class BusinessFault
{
    [DataMember]
    public string ErrorCode { get; set; }

    [DataMember]
    public string Message { get; set; }

    public BusinessFault(string errorCode, string message)
    {
        ErrorCode = errorCode;
        Message = message;
    }
}