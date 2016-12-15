using System;

namespace RSM.Models
{
    public class PagingModel : ViewModel
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }

        public PagingModel(int page, int pageSize, int totalRows)
        {
            Page = page;
            PageIndex = Page - 1;
            PageSize = pageSize;
            TotalRecords = totalRows;
            TotalPages = (int)Math.Ceiling((float)TotalRecords / (float)PageSize);
        }
    }
}