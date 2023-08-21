namespace KSociety.Base.App.Utility.Dto.Res.Control
{
    using Shared;
    using ProtoBuf;

    [ProtoContract]
    public class Migration : IResponse
    {
        [ProtoMember(1)] public bool Result { get; set; }

        public Migration() { }

        public Migration(bool result)
        {
            this.Result = result;
        }
    }
}
