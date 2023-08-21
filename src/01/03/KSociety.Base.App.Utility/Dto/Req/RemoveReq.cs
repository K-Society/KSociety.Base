namespace KSociety.Base.App.Utility.Dto.Req
{
    using Shared;
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class RemoveReq : IRequest
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        public RemoveReq() { }

        public RemoveReq(Guid id)
        {
            this.Id = id;
        }
    }
}
