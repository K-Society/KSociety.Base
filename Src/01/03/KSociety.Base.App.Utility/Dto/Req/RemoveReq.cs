using KSociety.Base.App.Shared;
using ProtoBuf;
using System;

namespace KSociety.Base.App.Utility.Dto.Req
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