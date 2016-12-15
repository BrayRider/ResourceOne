using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using RSM.Artifacts.Requests;
using RSM.Support;

namespace RSM.Service.Library.Controllers
{
	public class EmpWithNonStatusChanges : DataController<EmpWithNonStatusChange>
	{
		public Result<List<EmpWithNonStatusChange>> Search(PagedRequest request, Expression<Func<EmpWithNonStatusChange, bool>> filterExpression)
		{
			var results = new Result<List<EmpWithNonStatusChange>>(new Dictionary<string, string>());

			DbContext.DeferredLoadingEnabled = false;
			var loadOptions = new DataLoadOptions();
			DbContext.LoadOptions = loadOptions;

			var query = DbContext.EmpWithNonStatusChanges.AsQueryable();
			if(filterExpression != null)
			{
				query = query.Where(filterExpression.Compile()).AsQueryable();

			}

			if(!string.IsNullOrWhiteSpace(request.SortField))
			{
				if(request.SortDirection == SortDirections.Ascending)
					query = query.OrderBy(request.SortOrder);
			}

			var rowCount = query.Count();
			results.RowsReturned = rowCount;

			var rows = query.Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ToList();
			results.Entity = rows;

			return results;
		}
	}
}
