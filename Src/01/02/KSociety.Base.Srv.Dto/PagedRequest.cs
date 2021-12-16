using ProtoBuf;

namespace KSociety.Base.Srv.Dto
{
    [ProtoContract]
    public class PagedRequest
    {
        [ProtoMember(1)]
        public int PageNumber { get; set; }

        [ProtoMember(2)]
        public int PageSize { get; set; }

        public PagedRequest()
        {

        }

        public PagedRequest(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
