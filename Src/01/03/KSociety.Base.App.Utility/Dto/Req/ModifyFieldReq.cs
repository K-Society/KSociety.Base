using KSociety.Base.App.Shared;
using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;
using System;

namespace KSociety.Base.App.Utility.Dto.Req
{
    [ProtoContract]
    public class ModifyFieldReq : IRequest, IModifyField
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        [ProtoMember(2)] public string FieldName { get; set; }

        [ProtoMember(3)] public string Value { get; set; }

        public ModifyFieldReq() { }

        public ModifyFieldReq(
            Guid id,
            string fieldName,
            string value
        )
        {
            Id = id;
            FieldName = fieldName;
            Value = value;
        }
    }
}