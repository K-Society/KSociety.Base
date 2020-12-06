using System;
using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;

namespace KSociety.Base.Srv.Dto
{
    [ProtoContract]
    public class KbIdObject : IKbIdObject
    {
        
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        public KbIdObject()
        {

        }

        public KbIdObject(Guid id)
        {
            Id = id;
        }
    }
}
