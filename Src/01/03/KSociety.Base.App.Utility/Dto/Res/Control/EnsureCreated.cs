using KSociety.Base.App.Shared;
using ProtoBuf;

namespace KSociety.Base.App.Utility.Dto.Res.Control
{
    [ProtoContract]
    public class EnsureCreated : IResponse
    {
        [ProtoMember(1)] public bool Result { get; set; }

        public EnsureCreated() { }

        public EnsureCreated(bool result)
        {
            Result = result;
        }
    }
}