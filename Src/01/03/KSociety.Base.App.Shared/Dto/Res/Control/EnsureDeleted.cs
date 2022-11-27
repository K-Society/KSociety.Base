using ProtoBuf;

namespace KSociety.Base.App.Shared.Dto.Res.Control
{
    [ProtoContract]
    public class EnsureDeleted : IResponse
    {
        [ProtoMember(1)] public bool Result { get; set; }

        public EnsureDeleted() { }

        public EnsureDeleted(bool result)
        {
            Result = result;
        }
    }
}