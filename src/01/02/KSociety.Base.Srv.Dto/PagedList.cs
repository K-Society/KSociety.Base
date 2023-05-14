using KSociety.Base.InfraSub.Shared.Interface;
using ProtoBuf;
using System;

namespace KSociety.Base.Srv.Dto
{
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
                return PageNumber > 1;
            }
        }

        public bool NextPage
        {
            get
            {
                return PageNumber < TotalPages;
            }
        }

        public PagedList()
        {

        }

        public PagedList(int totalRows, int pageNumber, int pageSize)
        {
            TotalRows = totalRows;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalRows / pageSize);
        }
    }
}