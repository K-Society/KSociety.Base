// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.App.Utility.Dto.Req
{
    using Shared;
    using KSociety.Base.InfraSub.Shared.Interface;
    using ProtoBuf;

    [ProtoContract]
    public class ImportReq : IRequest, IImport
    {
        [ProtoMember(1)]
        public string FileName { get; set; }

        [ProtoMember(2)]
        public byte[] ByteArray { get; set; }

        public ImportReq() { }

        public ImportReq(string fileName, byte[] byteArray)
        {
            this.FileName = fileName;
            this.ByteArray = byteArray;
        }
    }
}
