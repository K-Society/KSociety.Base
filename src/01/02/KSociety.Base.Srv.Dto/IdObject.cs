// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Dto
{
    using System;
    using InfraSub.Shared.Interface;
    using ProtoBuf;

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
            this.Id = id;
        }
    }
}
