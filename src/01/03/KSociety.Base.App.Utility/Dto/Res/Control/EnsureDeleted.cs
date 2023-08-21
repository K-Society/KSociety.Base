namespace KSociety.Base.App.Utility.Dto.Res.Control
{
    using Shared;
    using ProtoBuf;

    [ProtoContract]
    public class EnsureDeleted : IResponse
    {
        [ProtoMember(1)] public bool Result { get; set; }

        public EnsureDeleted() { }

        public EnsureDeleted(bool result)
        {
            this.Result = result;
        }
    }
}
