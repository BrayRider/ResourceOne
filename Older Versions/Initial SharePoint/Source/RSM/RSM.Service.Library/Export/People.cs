using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using RSM.Service.Library.Controllers;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;
using RSM.Service.Library.Model.Reflection;

namespace RSM.Service.Library.Export
{
	public class People : Task
	{
		public new class Config : Task.Config
		{
			public static string LinkName = "ServiceAddress";
			public static string UserName = "ServiceAccount";
			public static string PasswordName = "ServicePassword";
			public static string LastUpdatedName = "LastUpdated";
			public static string SourceSystemName = "SourceSystem";

			public string Link { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
			public DateTime LastUpdated { get; set; }
			public ExternalSystem SourceSystem { get; set; }

			public Config(People task)
				: base(task)
			{ }

			public override Result<Task.Config> Load()
			{
				var result = base.Load();

				var settings = new TaskSettings(Task);

				Link = settings.GetValue(Config.LinkName);
				Username = settings.GetValue(Config.UserName);
				Password = settings.GetValue(Config.PasswordName);
				LastUpdated = settings.GetDateValue(Config.LastUpdatedName);
				LastUpdated = (LastUpdated == DateTime.MinValue) ? DateTime.Now.Subtract(TimeSpan.FromDays(1095)) : LastUpdated;
				SourceSystem = settings.GetExternalSystem(Config.SourceSystemName);

				result.Entity = this;

				return result;
			}

			public override void Save()
			{
				base.Save();

				var settings = new TaskSettings(Task);
				settings.SaveValue(Config.LastUpdatedName, LastUpdated.ToString());
			}
		}

		public People() :
			base()
		{
			ActivityName = "PeopleExport";
		}

		protected override IAPI CreateAPI(Task.Config config)
		{
			var api = base.CreateAPI(config);

			if (!(api is IAuthentication) || !(api is IExportPeople))
				throw new ApplicationException("The API does not implement one of the required interfaces for this task activity.");

			return api;
		}


		public override Result<string> Execute(object stateInfo)
		{
			var result = Result<string>.Success();

			var load = new Config(this).Load();
			if (load.Failed)
				return result.Merge(load).Fail(LogError("unable to load dynamic configuration."));

			var config = load.Entity as Config;

			var api = CreateAPI(config) as IAuthentication;
			var apiPeople = api as IExportPeople;

			var login = api.Login(config.Username, config.Password, config.Link);
			if (login.Failed)
				return result.Merge(login).Fail(LogError("unable to log into external system."));

			var exportCount = 0;
			var failCount = 0;
			try
			{
				// Get all people that have been modified since last search
				var criteria = new Person();
				var r1People = criteria.Search(config.LastUpdated);
				if (r1People.Failed)
				{
					return result.Merge(r1People).Fail(LogError("unable to get people."));
				}

				// Loop through people
				foreach (var entity in r1People.Entity)
				{
					//skip any not in filter
					if (!Filter(config, entity))
						continue;

					var apiPerson = apiPeople.ExportPerson(entity);

					if (apiPerson.Failed)
					{
						LogError("unable to export person ({0}).", entity.InternalId);
						failCount++;
						continue;
					}						

					exportCount++;
					LogMessage("exported Person ({0}).", entity.InternalId);
					config.LastUpdated = entity.Updated;
				}
			}
			finally
			{
				result.Entity = string.Format("Exported {0} people, {1} others failed.", exportCount, failCount);
				config.Save();
				api.Logoff(login.Entity);
			}
			return result;
		}

		#region Helpers
		public virtual bool Filter(Config config, Person entity)
		{
			return true;
		}
		#endregion
	}
}
