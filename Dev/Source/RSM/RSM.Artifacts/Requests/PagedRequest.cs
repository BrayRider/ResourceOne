using System;

namespace RSM.Artifacts.Requests
{
	public class PagedRequest
	{
		// Paging
		public int PageSize { get; set; }
		public int Page { get; set; }
		public int TotalRecords { get; set; }
		public int TotalPages { get; set; }
		public int PageIndex { get; set; }

		// Ordering
		public string SortField { get; set; }

		private string sortDirection;
		public string SortDirection
		{
			get
			{
				if (string.IsNullOrWhiteSpace(sortDirection))
					sortDirection = SortDirections.Ascending;

				if (sortDirection.StartsWith("desc", StringComparison.OrdinalIgnoreCase))
					sortDirection = SortDirections.Descending;

				return sortDirection;
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					sortDirection = SortDirections.Ascending;
					return;
				}

				if (value.StartsWith("desc", StringComparison.OrdinalIgnoreCase))
				{
					sortDirection = SortDirections.Descending;
					return;
				}

				sortDirection = SortDirections.Ascending;
			}
		}

		public string SortOrder
		{
			get
			{
				return string.IsNullOrWhiteSpace(SortField) ? string.Empty : string.Format("{0} {1}", SortField, SortDirection);
			}
		}

		public void ResetPages(int page, int pageSize, int totalRows)
		{
			Page = page;
			PageIndex = Page - 1;
			PageSize = pageSize;
			TotalRecords = totalRows;
			TotalPages = (int)Math.Ceiling((float)TotalRecords / (float)PageSize);
		}

	}
}
