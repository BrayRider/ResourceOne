using System;
using RSM.Service.Library;
using RSM.Service.Library.Controllers;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;
using PeopleBase = RSM.Service.Library.Export.People;

namespace RSM.Integration.Lubrizol.Export
{
	public class People : PeopleBase
	{
		public new class Config : PeopleBase.Config
		{
			private const string ConnectionStringSettingName = "SqlConnection";

			public string ConnectionString { get; set; }

			public static string ActiveEmployeeLibraryName = "ActiveEmployeeLibrary";
			public static string InactiveEmployeeLibraryName = "InactiveEmployeeLibrary";

			public string ActiveEmployeeLibrary { get; set; }
			public string InactiveEmployeeLibrary { get; set; }

			public Config(People task)
				: base(task)
			{ }

			public override Result<Task.Config> Load()
			{
				var result = base.Load();

				var settings = new TaskSettings(Task);

				ConnectionString = settings.GetValue(ConnectionStringSettingName);

				ActiveEmployeeLibrary = settings.GetValue(ActiveEmployeeLibraryName);
				InactiveEmployeeLibrary = settings.GetValue(InactiveEmployeeLibraryName);

				result.Entity = this;

				return result;
			}
		}

		public People()
			: base()
		{
			ExternalSystem = ExternalSystem.LubrizolOut;
		}

		protected override Result<Task.Config> LoadConfig()
		{
			return new Config(this).Load(); //Create config specific to the Lubrizol people export
		}

		protected override IAPI CreateAPI(Task.Config config)
		{
			var api = base.CreateAPI(config);

			if (api != null)
			{
				//This API needs access to the config
				(api as API).ExportConfig = config as Config;
			}
			return api;
		}

		public override Result<string> Execute(object stateInfo)
		{
			var result = Result<string>.Success();

			var load = new Config(this).Load();
			if (load.Failed)
				return result.Merge(load).Fail(LogError("unable to load dynamic configuration."));

			var config = load.Entity as Config;
			if (config == null)
				return result.Fail(LogError("unable to get people. Invalid service configuration."));

			var api = CreateAPI(config) as IAuthentication;
			var apiPeople = api as IExportPeople;

			if(apiPeople == null)
				return result.Fail(LogError("unable to get people. Invalid service configuration."));

			var exportCount = 0;
			var failCount = 0;
			try
			{
				// Get all people that have been modified since last search
				var criteria = new Person { ExternalSystemId = config.SourceSystem.Id };
				var r1People = criteria.Search(config.LastUpdated.Subtract(Profile.Schedule.RepeatInterval));
				if (r1People.Failed)
					return result.Merge(r1People).Fail(LogError("unable to get people."));

				// Loop through people
				foreach (var entity in r1People.Entity)
				{
					//skip any not in filter
					if (!Filter(config, entity))
						continue;

					var apiPerson = apiPeople.ExportPerson(entity);
					if (apiPerson.Failed)
					{
						LogError("unable to export person ({0}) ({3}).{1}{2}", entity.InternalId, Environment.NewLine, apiPerson.Message, entity.ExternalId);
						failCount++;
						continue;
					}

					exportCount++;
					//LogMessage("exported Person ({0}).", entity.InternalId);
					config.LastUpdated = entity.Updated;
				}
			}
			finally
			{
				result.Entity = string.Format("Exported {0} people, {1} others failed.", exportCount, failCount);
				config.Save();
			}
			return result;
		}


	}
}
