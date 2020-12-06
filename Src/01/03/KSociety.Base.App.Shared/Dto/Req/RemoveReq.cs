using System;
using ProtoBuf;

namespace KSociety.Base.App.Shared.Dto.Req
{
    [ProtoContract]
    public class RemoveReq : IRequest
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        public RemoveReq() { }

        public RemoveReq(Guid id)
        {
            Id = id;
        }
    }
}
