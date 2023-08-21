namespace KSociety.Base.App.Utility.Dto.Res.Control
{
    using Shared;
    using ProtoBuf;

    [ProtoContract]
    public class ConnectionString : IResponse
    {
        [ProtoMember(1)] public string Result { get; set; }

        public ConnectionString() { }

        public ConnectionString(string result)
        {
            this.Result = result;
        }
    }
}
