// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Utility.Dto.Res.Control
{
    using Shared;
    using ProtoBuf;

    [ProtoContract]
    public class Migration : IResponse
    {
        [ProtoMember(1)]
        public bool Result { get; set; }

        public Migration() { }

        public Migration(bool result)
        {
            this.Result = result;
        }
    }
}
