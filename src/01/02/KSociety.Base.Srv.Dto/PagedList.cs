namespace KSociety.Base.Srv.Dto
{
    using InfraSub.Shared.Interface;
    using ProtoBuf;
    using System;

    [ProtoContract]
    public class PagedList<T>
        : ObjectList<T> where T : IObject
    {
        [ProtoMember(1)] public int TotalRows { get; set; }

        [ProtoMember(2)] public int PageNumber { get; set; }

        [ProtoMember(3)] public int PageSize { get; set; }

        [ProtoMember(4)] public int TotalPages { get; set; }

        public bool PreviousPage
        {
            get
            {
                return this.PageNumber > 1;
            }
        }

        public bool NextPage
        {
            get
            {
                return this.PageNumber < this.TotalPages;
            }
        }

        public PagedList()
        {

        }

        public PagedList(int totalRows, int pageNumber, int pageSize)
        {
            this.TotalRows = totalRows;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalPages = (int)Math.Ceiling((double)totalRows / pageSize);
        }
    }
}
