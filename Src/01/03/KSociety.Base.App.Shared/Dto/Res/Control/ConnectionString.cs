using ProtoBuf;

namespace KSociety.Base.App.Shared.Dto.Res.Control
{
    [ProtoContract]
    public class ConnectionString : IResponse
    {
        [ProtoMember(1)]
        public string Result { get; set; }

        public ConnectionString() { }

        public ConnectionString(string result)
        {
            Result = result;
        }
    }
}
