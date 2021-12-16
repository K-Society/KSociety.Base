using KSociety.Base.InfraSub.Shared.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KSociety.Base.Srv.Dto
{
    public class PaginatedList<T> 
        : ObjectList<T> where T : IObject
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; set; }

        public PaginatedList()
        {

        }

        public PaginatedList(InfraSub.Shared.Interface.IList<T> items, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize);
            AddRange(items);
        }

        private PaginatedList(IEnumerable<T> items, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(items.Count() / (double)pageSize);
            AddRange(items);
        }

        public bool PreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool NextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static PaginatedList<T> Create(InfraSub.Shared.Interface.IList<T> source, int pageIndex, int pageSize)
        {
            var items = source.List.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new PaginatedList<T>(items, pageIndex, pageSize);
        }

        public static async Task<PaginatedList<T>> CreateAsync(InfraSub.Shared.Interface.IList<T> source, int pageIndex, int pageSize)
        {
            var items = source.List.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new PaginatedList<T>(items, pageIndex, pageSize);
        }
    }
}
