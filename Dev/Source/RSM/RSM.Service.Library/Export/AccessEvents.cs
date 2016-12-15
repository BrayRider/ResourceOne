using System;
using System.Collections.Generic;
using System.Linq;

using RSM.Service.Library.Controllers;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Import;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;

namespace RSM.Service.Library.Export
{
	public abstract class AccessEvents: Task
	{
		public new class Config : Task.Config
		{
			public static string LastEventName = "LastAccessEvent";
			public static string LinkName = "ServiceAddress";
			public static string UserName = "ServiceAccount";
			public static string PasswordName = "ServicePassword";
			public static string SourceSystemName = "SourceSystem";
			public static string ExportPersonName = "PersonExport";
			public static string ExportLocationName = "LocationExport";
			public static string ExportAccessEventsName = "AccessExport";
			public static string ExportCompanyName = "CompanyExport";
            public static string ExportCompaniesName = "Contractors";

			public string Link { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
			public ExternalSystem SourceSystem { get; set; }

			public bool ExportPerson { get; set; }
			public bool ExportCompany { get; set; }
			public bool ExportAccessEvents { get; set; }
			public DateTime LastEvent { get; set; }
            public string[] ExportCompanies { get; set; }

			public Config(AccessEvents task) : base(task)
			{ }

			public override Result<Task.Config> Load()
			{
				var result = base.Load();
				var settings = new TaskSettings(Task);

				Link = settings.GetValue(Config.LinkName);
				Username = settings.GetValue(Config.UserName);
				Password = settings.GetValue(Config.PasswordName);

				var systemId = settings.GetIntValue(Config.SourceSystemName);
				var criteria = new ExternalSystem { Id = systemId };
				var system = criteria.Get();
				if (system.Failed)
					return result.Fail(Task.LogError("source system id {0} not found.", systemId.ToString()));

				SourceSystem = system.Entity;

				ExportPerson = settings.GetBoolValue(Config.ExportPersonName);
				ExportAccessEvents = settings.GetBoolValue(Config.ExportAccessEventsName);
				ExportCompany = settings.GetBoolValue(Config.ExportCompanyName);

                var companies = settings.GetValue(Config.ExportCompaniesName);
                ExportCompanies = !string.IsNullOrWhiteSpace(companies) ? companies.Split(',') : new string[0];

				var from = settings.GetValue(Config.LastEventName);
				if (from == null)
					return result.Fail(Task.LogError("last event setting {0} not found.", Config.LastEventName));

				DateTime date;
				LastEvent = ((from.Trim().Length == 0) || !DateTime.TryParse(from, out date))
					? DateTime.Now.Subtract(TimeSpan.FromDays(30)) : date;

				result.Entity = this;

				return result;
			}

			public override void Save()
			{
				base.Save();

				var settings = new TaskSettings(Task);
				settings.SaveValue(Config.LastEventName, LastEvent.ToString());
			}
		}

        public Config Configuration { get; set; }
        
        public AccessEvents() :
			base()
		{
			ActivityName = "AccessEvents";
		}

		protected override Result<Task.Config> LoadConfig()
		{
			return new Config(this).Load();
		}

		protected override IAPI CreateAPI(Task.Config config)
		{
			var api = base.CreateAPI(config);

			if (!(api is IAuthentication) || !(api is IExportAccessEvent))
				throw new ApplicationException("The API does not implement one of the required interfaces for this task activity.");

			return api;
		}

		public override Result<string> Execute(object stateInfo)
		{
			var result = Result<string>.Success();

			var load = LoadConfig();
			if (load.Failed)
				return result.Merge(load).Fail(LogError("unable to load dynamic configuration."));

			var config = load.Entity as Config;

			var api = CreateAPI(config) as IAuthentication;
			var apiExport = api as IExportAccessEvent;
            if (apiExport == null)
                return result.Fail(LogError("unable to create into export API."));

			var login = api.Login(config.Username, config.Password, config.Link);
			if (login.Failed)
				return result.Merge(login).Fail(LogError("unable to log into external system."));

			var logCount = 0;
			var locationCount = 0;
			var portalCount = 0;
			var readerCount = 0;
			var personCount = 0;
			var companies = new List<string>();

			try
			{
				if (config.ExportAccessEvents)
				{
					// Get Access Log push list
					var pushFrom = Factory.CreateExternalEntity<ExternalEntity>(EntityType.AccessLog, system: config.SourceSystem);

					var accessList = pushFrom.PushList(Factory.CreateExternalEntity<ExternalEntity>(EntityType.AccessLog, system: ExternalSystem));

					if (accessList.Failed)
						return result.Merge(accessList).Fail(LogError("unable to get push list for access events."));

					foreach (var accessKeys in accessList.Entity)
					{
						//get access record
						var access = new AccessLog();
						var r1Access = ((AccessLog)accessKeys.MapKeys(access)).Get(SelectKeys.Internal);
						if (r1Access.Failed)
							return result.Merge(r1Access).Fail(LogError("unable to locate access record {0}.", access.InternalId));

						access = r1Access.Entity;

						//verify entities
						result.RequiredObject(access.Person, string.Format("Person entity {0} not found for access record {1}.", access.PersonId, access.InternalId));
						result.RequiredObject(access.Reader, string.Format("Reader entity {0} not found for access record {1}.", access.ReaderId, access.InternalId));
						result.RequiredObject(access.Portal, string.Format("Portal entity {0} not found for access record {1}.", access.PortalId, access.InternalId));
						result.RequiredObject(access.Portal.Location, string.Format("Location entity {0} not found for access record {1}.", access.Portal.LocationId, access.InternalId));
						if (result.Failed)
						{
							LogErrorDetail(result.ToString(), "access record {0} missing entity.", access.InternalId);
							continue;
						}

						//get external keys
						access.Person.ExternalSystemId = access.ExternalSystemId;
						result.Merge(access.Person.GetKeys(SelectKeys.Internal, replace: true));

						access.Reader.ExternalSystemId = access.ExternalSystemId;
						result.Merge(access.Reader.GetKeys(SelectKeys.Internal, replace: true));

						access.Portal.ExternalSystemId = access.ExternalSystemId;
						result.Merge(access.Portal.GetKeys(SelectKeys.Internal, replace: true));

						access.Portal.Location.ExternalSystemId = access.ExternalSystemId;
						result.Merge(access.Portal.Location.GetKeys(SelectKeys.Internal, replace: true));
						if (result.Failed)
						{
							LogErrorDetail(result.ToString(), "access record {0} missing entity key(s).", access.InternalId);
							continue;
						}

                        //skip any access type not in filter
                        if (!Filter(access))
                            continue;

						if (config.ExportCompany)
						{
							var export = apiExport.ExportCompany(access);
							if (export.Failed)
								return result.Merge(export).Fail(LogErrorDetail(export.ToString(), "failed to export company for access record {0}.", access.InternalId));

							companies.Add(export.Entity);
							//LogMessage("exported company for access record ({0}).", access.InternalId);
						}

						if (config.ExportPerson)
						{
							var person = access.Person;
							person.ExternalSystemId = ExternalSystem.Id;

						    if (!(person as ExternalEntity).KeysExist(SelectKeys.Internal))
						    {
						        var export = apiExport.ExportPerson(person, access);
						        if (export.Failed)
						            return result.Merge(export).Fail(LogError("failed to export person record {0}.", person.InternalId));

						        var r1Add = (export.Entity as ExternalEntity).Add();
						        if (r1Add.Failed)
						            return
						                result.Merge(r1Add)
						                    .Fail(LogError("unable to save external keys for person record {0}.", person.InternalId));

						        personCount++;
						        //LogMessage("exported person record ({0}).", person.InternalId);
						    }
						    else
						    {
                                // Send person everytime
                                var export = apiExport.ExportPerson(person, access);
                                if (export.Failed)
                                    return result.Merge(export).Fail(LogError("failed to export person record {0}.", person.InternalId));

                                personCount++;
                                //LogMessage("exported person record ({0}).", person.InternalId);
                            }
						}

						//export portal
						var portal = access.Portal;
						portal.ExternalSystemId = ExternalSystem.Id;

						if (!(portal as ExternalEntity).KeysExist(SelectKeys.Internal))
						{
							var export = apiExport.ExportPortal(portal, access);
							if (export.Failed)
							{
								LogError("failed to export portal record {0}.", portal.InternalId);
								continue;
							}

							var r1Add = (export.Entity as ExternalEntity).Add();
							if (r1Add.Failed)
							{
								LogError("unable to save external keys for portal record {0}.", portal.InternalId);
								continue;
							}

							portalCount++;
							//LogMessage("exported portal record ({0}).", portal.InternalId);
						}

						//export reader
						var reader = access.Reader;
						reader.ExternalSystemId = ExternalSystem.Id;

						if (!(reader as ExternalEntity).KeysExist(SelectKeys.Internal))
						{
							var export = apiExport.ExportReader(reader, access);
							if (export.Failed)
							{
								LogError("failed to export reader record {0}.", reader.InternalId);
								continue;
							}

							var r1Add = (export.Entity as ExternalEntity).Add();
							if (r1Add.Failed)
							{
								LogError("unable to save external keys for reader record {0}.", reader.InternalId);
								continue;
							}

							readerCount++;
							//LogMessage("exported reader record ({0}).", reader.InternalId);
						}

						//export access event
						access.ExternalSystemId = ExternalSystem.Id;

						if (!(access as ExternalEntity).KeysExist(SelectKeys.Internal))
						{
							var export = apiExport.ExportEvent(access);
							if (export.Failed)
							{
								LogError("failed to export access record {0}.", access.InternalId);
								continue;
							}

							var r1Add = (export.Entity as ExternalEntity).Add();
							if (r1Add.Failed)
							{
								LogError("unable to save external keys for access record {0}.", access.InternalId);
								continue;
							}

							logCount++;
							//LogMessage("exported access record ({0}).", access.InternalId);
						}

						config.LastEvent = access.Accessed;
					}
				}
			}
			finally
			{
				result.Entity = string.Format("Exported {0} access logs, {1} people, {2} locations, {3} portals, {4} readers.",
					logCount, personCount, locationCount, portalCount, readerCount);

				config.Save();
				api.Logoff(login.Entity);
			}

			return result;
		}

		#region Helpers
        public virtual bool Filter(AccessLog log)
        {
            return true;
        }

		#endregion
	}
}
