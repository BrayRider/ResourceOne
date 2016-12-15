
using RSM.Support;

namespace RSM.Service.Library.Controllers
{
    public class BatchHistories : DataController<BatchHistory>
    {
        public BatchHistories()
            : base()
        {
        }

        public BatchHistories(RSMDataModelDataContext context)
            : base(context)
        {
        }

		public Result<BatchHistory> Add(BatchHistory entity)
		{
			DbContext.BatchHistories.InsertOnSubmit(entity);

			DbContext.SubmitChanges();

			return new Result<BatchHistory> { Entity = entity };
		}

	}
}
