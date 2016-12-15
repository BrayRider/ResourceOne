using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using RSM.Support;

namespace RSM.Service.Library.Controllers
{
	public class Settings : DataController<Setting>
	{
		public Settings()
			: base()
		{
			DbContext.DeferredLoadingEnabled = false;
			var loadOptions = new DataLoadOptions();
			loadOptions.LoadWith<Setting>(t => t.ExternalSystem);
			DbContext.LoadOptions = loadOptions;
		}

		public Settings(RSMDataModelDataContext context)
			: base(context)
		{
			DbContext.DeferredLoadingEnabled = false;
			var loadOptions = new DataLoadOptions();
			loadOptions.LoadWith<Setting>(t => t.ExternalSystem);
			DbContext.LoadOptions = loadOptions;
		}

		public Result<List<Setting>> Search(Expression<Func<Setting, bool>> filterExpression)
		{
			var results = new Result<List<Setting>>();

			results.RequiredObject(filterExpression, "Missing search criteria");

			var rows = DbContext.Settings.Where(filterExpression.Compile()).ToList();

			results.Entity = rows;

			return results;
		}

		public Result<Setting> Get(string system, string name)
		{
			if (string.IsNullOrWhiteSpace(system))
				return new Result<Setting>(ResultType.ValidationError, "Missing system name");

			if (string.IsNullOrWhiteSpace(name))
				return new Result<Setting>(ResultType.ValidationError, "Missing setting name");

			var systemEntity = DbContext.ExternalSystems.FirstOrDefault(x => x.Name == system);

			if (systemEntity == null)
				return new Result<Setting>(ResultType.ValidationError, "Unknown system name");

			var setting = DbContext.Settings.FirstOrDefault(x => x.Name == name && x.SystemId == systemEntity.Id);

			var results = new Result<Setting> {Entity = setting};

			return results;
		}

		public Result<Setting> Set(int id, string value)
		{
			using (var db = new RSMDataModelDataContext())
			{
				var existing = db.Settings.FirstOrDefault(x => x.Id == id);

				if (existing == null)
				{
					return new Result<Setting>(ResultType.DataError, "Setting does not exist");
				}

				existing.Value = value;

				db.SubmitChanges();

				var results = new Result<Setting> { Entity = existing };

				return results;
			}
		}

		public static string GetValue(string system, string setting)
		{
			var controller = new Settings();

			var results = controller.Get(system, setting);

			if (results.Failed || results.Entity == null)
				return null;

			return results.Entity.Value;
		}

		public static bool GetValueAsBool(string system, string setting)
		{
			var controller = new Settings();

			var results = controller.Get(system, setting);

			var value = false;

			if (results.Failed || results.Entity == null)
				return false;

			bool.TryParse(results.Entity.Value, out value);

			return value;
		}

		public Result<Setting> Set(Setting setting)
		{
			DbContext.Settings.Attach(setting, true);
			DbContext.SubmitChanges();

			setting = DbContext.Settings.FirstOrDefault(x => x.Id == setting.Id);
			var results = new Result<Setting> {Entity = setting};

			return results;
		}
	}
}
