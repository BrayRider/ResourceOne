using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using RSM.Service.Library.Controllers;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;
using RSM.Service.Library.Model.Reflection;

namespace RSM.Service.Library.Import
{
	public class People : Task
	{
		public new class Config : Task.Config
		{
			public static string LinkName = "ServiceAddress";
			public static string UserName = "ServiceAccount";
			public static string PasswordName = "ServicePassword";
			public static string ImageImportName = "ImageImport";
			public static string LastUpdatedName = "LastAccessEvent";
			public static string FieldsToUpdateName = "FieldsToUpdate";

			public string Link { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
			public bool ImportImage { get; set; }
			public DateTime LastUpdated { get; set; }
			public string[] FieldsToUpdate { get; set; }

			public ModelMapper<Person> PersonMapper { get; set; }
			public EntityFilters<Person> PersonFilters { get; set; }

			public Config(People task) : base(task)
			{ }

			public override Result<Task.Config> Load()
			{
				var result = base.Load();

				var settings = new TaskSettings(Task);

				Link = settings.GetValue(Config.LinkName);
				Username = settings.GetValue(Config.UserName);
				Password = settings.GetValue(Config.PasswordName);
				ImportImage = settings.GetBoolValue(Config.ImageImportName);
				LastUpdated = settings.GetDateValue(Config.LastUpdatedName);
				LastUpdated = (LastUpdated == DateTime.MinValue) ? DateTime.Now.Subtract(TimeSpan.FromDays(1095)) : LastUpdated;

				var fields = settings.GetValue(Config.FieldsToUpdateName);
				FieldsToUpdate = (fields != null) ? fields.Split(',') : new string[] { };

				//Get field filters, if they exist
				PersonMapper = new ModelMapper<Person>(FieldsToUpdate);
				PersonFilters = new EntityFilters<Person>(PersonMapper).Load(settings);

				result.Entity = this;

				return result;
			}

			public override void Save()
			{
				base.Save();
			}
		}

		public People() :
			base()
		{
			ActivityName = "PeopleImport";
		}

		protected override IAPI CreateAPI(Task.Config config)
		{
			var api = base.CreateAPI(config);

			if (!(api is IAuthentication) || !(api is IImportPeople))
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
			var apiPeople = api as IImportPeople;

			var login = api.Login(config.Username, config.Password, config.Link);
			if (login.Failed)
				return result.Merge(login).Fail(LogError("unable to log into external system."));

			var addCount = 0;
			var updateCount = 0;
			string lastKey = null;
			try
			{
				do
				{
					// Get people
					var people = apiPeople.GetPeople(ref lastKey);
					if (people.Failed)
						return result.Merge(people).Fail(LogError("unable to get people."));

					// Loop through people
					foreach (var entity in people.Entity)
					{

						//get S2 data
						var apiPerson = apiPeople.RetrievePersonDetail(entity.ExternalId, config.ImportImage);
						if (apiPerson.Failed)
							return result.Merge(apiPerson).Fail(LogError("unable to locate external person id {0}.", entity.ExternalId));

						//skip any not in filter
						if (!Filter(config, entity))
						{
							//LogMessage("Person Failed Filter: {1}, {2} ({0}) at {3}.", entity.ExternalId, entity.LastName, entity.FirstName, entity.LastUpdated);
							continue;
						}

						//check R1SM existence
						var r1Person = entity.Get();

						if (r1Person.Failed && r1Person.Details.ContainsKey("NotFound"))
						{
							//import person into R1SM.
							var r1Add = apiPerson.Entity.Add();
							if (r1Add.Failed)
								return result.Merge(r1Add).Fail(LogError("unable to import new person {0}.", entity.ExternalId));

							addCount++;
							LogMessage("added new Person: {1}, {2} ({0}).", entity.ExternalId, entity.LastName, entity.FirstName);
						}
						else
						{
							var internalId = r1Person.Entity.InternalId;
							config.PersonMapper.MapProperties(apiPerson.Entity, r1Person.Entity);
							apiPerson.Entity.InternalId = internalId;
							r1Person.Entity.InternalId = internalId;

							var r1Update = r1Person.Entity.Update();
							if (r1Update.Failed)
							{
								LogErrorDetail(r1Update.ToString(), r1Update.ToString());
								return result.Merge(r1Update).Fail(LogError("unable to update person {0}.", entity.ExternalId));
							}

							updateCount++;
							//LogMessage("updated Person: {1}, {2} ({0}).", entity.ExternalId, entity.LastName, entity.FirstName);
						}
					}
				} while (lastKey != null);
			}
			finally
			{
				result.Entity = string.Format("Added {0} people, updated {1} people.", addCount, updateCount);
				config.Save();
				api.Logoff(login.Entity);
			}
			return result;
		}

		#region Helpers
		public virtual bool Filter(Config config, Person entity)
		{
			return entity.ExternalUpdated >= config.LastUpdated.Subtract(Profile.Schedule.RepeatInterval) && config.PersonFilters.IsMatch(entity);
		}

		#endregion
	}
}
