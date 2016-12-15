using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using RSM.Service.Library;
using RSM.Service.Library.Controllers;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;
using RSM.Service.Library.Model.Reflection;

using RSM.Integration.Lubrizol.Extensions;

using R1Employee = RSM.Integration.Lubrizol.Model.Lubrizol_Employee;
using LzEmployee = RSM.Integration.Lubrizol.Model.tblzILMData;

namespace RSM.Integration.Lubrizol
{
	public class Employees : Task
	{
		public new class Config : Task.Config
		{
			private const string ConnectionStringSettingName = "SqlConnection";

			public string ConnectionString { get; set; }

			public ModelMapper<R1Employee> EmployeeMapper { get; set; }
			public EntityFilters<R1Employee> EmployeeFilters { get; set; }

			public ModelMapper<Person> PersonMapper { get; set; }
			public EntityFilters<Person> PersonFilters { get; set; }

			public Config(Employees task)
				: base(task)
			{ }

			public override Result<Task.Config> Load()
			{
				var result = base.Load();

				var settings = new TaskSettings(Task);

				//Get field filters, if they exist
				var all = new string[] { };
				EmployeeMapper = new ModelMapper<R1Employee>(all);
				EmployeeFilters = new EntityFilters<R1Employee>(EmployeeMapper).Load(settings);

				PersonMapper = new ModelMapper<Person>(all);
				PersonFilters = new EntityFilters<Person>(PersonMapper).Load(settings);

				ConnectionString = settings.GetValue(ConnectionStringSettingName);

				result.Entity = this;

				return result;
			}

			public override void Save()
			{
				base.Save();
			}
		}

		public Employees() :
			base()
		{
			ActivityName = "EmployeeImport";
			ExternalSystem = ExternalSystem.LubrizolIn;
		}
		protected override IAPI CreateAPI(Task.Config config)
		{
			var api = base.CreateAPI(config);

			if (!(api is API))
				throw new ApplicationException("The API is not a Lubrizol API.");

			//This API needs access to the config

			var lzApi = api as API;
			lzApi.ImportConfig = config as Config;
			lzApi.ExportConfig = config as Export.People.Config;
			lzApi.Mapper = (config as Config).EmployeeMapper;

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
				return result.Fail(LogError("unable to load dynamic configuration with settings."));

			var api = CreateAPI(config) as API;

			if(api == null)
				return result.Fail(LogError("unable to load SQL Connector API."));

			var addCount = 0;
			var updateCount = 0;
			try
			{
				// Get people
				var criteria = Factory.CreatePerson(system: ExternalSystem.S2In);
				var people = criteria.Search();
				if (people.Failed)
					return result.Merge(people).Fail(LogError("unable to get people."));

				// Loop through people
				foreach (var person in people.Entity)
				{
					var apiEmployee = api.GetEmployee(person.ExternalId);
					if (apiEmployee.Failed)
					{
						//result.Merge(apiEmployee).Fail(LogWarning("employee does not exist in Lubrizol datastore (id: {0}).", person.InternalId));
						result.Merge(apiEmployee).Fail(string.Format("employee does not exist in Lubrizol datastore (id: {0}).", person.InternalId));
						continue;
					}

					//check existence in R1SM
					var employee = apiEmployee.Entity;

					//skip any not in filter
					if (!Filter(config, employee, person))
						continue;

					var r1Person = employee.Get();
					if (r1Person.Failed && r1Person.Details.ContainsKey("NotFound"))
					{
						//import person into R1SM.
						employee.LastUpdated = DateTime.Now;
						var r1Add = employee.Add();
						if (r1Add.Failed)
							return result.Merge(r1Add).Fail(LogError("unable to import employee ({0}) ({3}).{1}{2}", employee.EmployeeID, Environment.NewLine, r1Add.Message, person.ExternalId));

						addCount++;
						//LogMessage("added Lubrizol employee ({0}).", employee.EmployeeID);
					}
					else
					{
						var r1Update = employee.Update(config.EmployeeMapper);
						if (r1Update.Failed)
							return result.Merge(r1Update).Fail(LogError("unable to update employee ({0}) ({3}).{1}{2}", employee.EmployeeID, Environment.NewLine, r1Update.Message, person.ExternalId));

						employee = r1Update.Entity;

						updateCount++;
						//LogMessage("updated Lubrizol employee ({0}).", employee.EmployeeID);
					}

					if (person.LastUpdated == employee.LastUpdated) continue;

					person.LastUpdated = employee.LastUpdated;
					person.Update(false);
				}
			}
			finally
			{
				result.Entity = string.Format("Added {0} employees, updated {1} employees.", addCount, updateCount);
				config.Save();
			}
			return result;
		}

		#region Helpers
		public virtual bool Filter(Config config, R1Employee employee, Person person)
		{
			return (config.EmployeeFilters.IsMatch(employee) && config.PersonFilters.IsMatch(person));
		}
		#endregion
	}
}
