namespace KSociety.Base.App.Utility.Dto.Res.Control
{
    using Shared;
    using ProtoBuf;

    [ProtoContract]
    public class EnsureCreated : IResponse
    {
        [ProtoMember(1)] public bool Result { get; set; }

        public EnsureCreated() { }

        public EnsureCreated(bool result)
        {
            this.Result = result;
        }
    }
}
