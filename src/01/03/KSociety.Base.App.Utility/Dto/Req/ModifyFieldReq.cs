// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Utility.Dto.Req
{
    using Shared;
    using KSociety.Base.InfraSub.Shared.Interface;
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class ModifyFieldReq : IRequest, IModifyField
    {
        [ProtoMember(1), CompatibilityLevel(CompatibilityLevel.Level200)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public string? FieldName { get; set; }

        [ProtoMember(3)]
        public string? Value { get; set; }

        public ModifyFieldReq() { }

        public ModifyFieldReq(
            Guid id,
            string fieldName,
            string value
        )
        {
            this.Id = id;
            this.FieldName = fieldName;
            this.Value = value;
        }
    }
}
