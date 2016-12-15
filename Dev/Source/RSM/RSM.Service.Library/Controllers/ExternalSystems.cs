using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RSM.Support;

namespace RSM.Service.Library.Controllers
{
    public class ExternalSystems : DataController<ExternalSystem>
    {
        public ExternalSystems()
            : base()
        {
        }

        public ExternalSystems(RSMDataModelDataContext context)
            : base(context)
        {
        }

        public Result<List<ExternalSystem>> Search(Expression<Func<ExternalSystem, bool>> filterExpression)
        {
            var results = new Result<List<ExternalSystem>>();

            results.RequiredObject(filterExpression, "Missing search criteria");

            var rows = DbContext.ExternalSystems.Where(filterExpression.Compile()).ToList();

            results.Entity = rows;

            return results;
        }

        public Result<ExternalSystem> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new Result<ExternalSystem>(ResultType.ValidationError, "Missing system name");

            var setting = DbContext.ExternalSystems.FirstOrDefault(x => x.Name == name);

            var results = new Result<ExternalSystem> {Entity = setting};

            return results;
        }
    }
}
