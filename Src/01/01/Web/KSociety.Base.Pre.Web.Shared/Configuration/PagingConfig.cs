using System;

namespace KSociety.Base.Pre.Web.Shared.Configuration
{
    public class PagingConfig
    {
        public bool Enabled { get; set; }
        public int PageSize { get; set; }
        public bool CustomPager { get; set; }

        public int NumberToItemsToSkip(int pageNumber)
        {
            if (Enabled)
            {
                return (pageNumber - 1) * PageSize;
            }

            return 0;
        }

        public int NumberOfItemsToTake(int totalItemsCount)
        {
            return Enabled ? PageSize : totalItemsCount;
        }

        public int PreviousPageNumber(int currentPageNumber)
        {
            if (currentPageNumber > 0)
                return currentPageNumber - 1;
            return 1;
        }

        public int NextPageNumber(int currentPageNumber, int totalItemsCount)
        {
            if (currentPageNumber < MaxPageNumber(totalItemsCount))
            {
                return currentPageNumber + 1;
            }

            return currentPageNumber;
        }

        public int MaxPageNumber(int totalItemsCount)
        {
            int maxPageNumber;
            double numberOfPages = totalItemsCount / (double)PageSize;
            if (numberOfPages == Math.Floor(numberOfPages))
            {
                maxPageNumber = (int)numberOfPages;
            }
            else
            {
                maxPageNumber = (int)numberOfPages + 1;
            }

            return maxPageNumber;
        }
    }
}
