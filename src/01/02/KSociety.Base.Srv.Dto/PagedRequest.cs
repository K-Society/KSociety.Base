// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Srv.Dto
{
    using ProtoBuf;

    [ProtoContract]
    public class PagedRequest
    {
        [ProtoMember(1)] public int PageNumber { get; set; }

        [ProtoMember(2)] public int PageSize { get; set; }

        public PagedRequest()
        {

        }

        public PagedRequest(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}
