using System;
using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;

namespace KSociety.Base.Srv.Dto
{
    [ProtoContract]
    public class IdObject : IIdObject
    {

        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        public IdObject()
        {

        }

        public IdObject(Guid id)
        {
            Id = id;
        }
    }
}