using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using RSM.Artifacts.Requests;
using RSM.Support;

namespace RSM.Service.Library.Controllers
{
	public class People : DataController<Person>
	{
		public Result<List<Person>> Search(PagedRequest request, Expression<Func<Person, bool>> filterExpression)
		{
			var results = new Result<List<Person>>(new Dictionary<string, string>());

			DbContext.DeferredLoadingEnabled = false;
			var loadOptions = new DataLoadOptions();
			DbContext.LoadOptions = loadOptions;

			var query = DbContext.Persons.AsQueryable();
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

		public Result<Person> Get(int id)
		{
			var results = new Result<Person>();

			try
			{
				var person = DbContext.Persons.FirstOrDefault(p => p.PersonID == id);

				results.RequiredObject(person, "Person was not found.");

				results.Entity = person;

				return results;
			}
			catch (Exception ex)
			{
				return results.Fail(ex.Message);
			}
		}

		public People(RSMDataModelDataContext context) : base(context)
		{
		}

		public People() : base()
		{
			
		}
	}
}
