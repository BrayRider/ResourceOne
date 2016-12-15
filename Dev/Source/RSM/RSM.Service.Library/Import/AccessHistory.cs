using System;
using System.Diagnostics;
using System.Linq;

using RSM.Service.Library.Controllers;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;

namespace RSM.Service.Library.Import
{
	public abstract class AccessHistory: Task
	{
		public new class Config : Task.Config
		{
			public static string LastEventName = "LastAccessEvent";
			public static string LinkName = "ServiceAddress";
			public static string UserName = "ServiceAccount";
			public static string PasswordName = "ServicePassword";
			public static string ImportPersonName = "PersonImport";
		    public static string ImportCompaniesChoiceName = "IncludeContractors";
            public static string ImportCompaniesName = "Contractors";

			public string Link { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
			public bool ImportPerson { get; set; }
            public bool IncludeContractors { get; set; }
            public string[] ImportCompanies { get; set; }
            public DateTime LastEvent { get; set; }

			public Config(AccessHistory task) : base(task)
			{ }

			public override Result<Task.Config> Load()
			{
				var result = base.Load();

				var settings = new TaskSettings(Task);

				Link = settings.GetValue(Config.LinkName);
				Username = settings.GetValue(Config.UserName);
				Password = settings.GetValue(Config.PasswordName);
				ImportPerson = settings.GetBoolValue(Config.ImportPersonName);
                IncludeContractors = settings.GetBoolValue(Config.ImportCompaniesChoiceName);

			    if (IncludeContractors)
			    {
			        var companies = settings.GetValue(Config.ImportCompaniesName);
			        ImportCompanies = !string.IsNullOrWhiteSpace(companies) ? companies.Split(',') : new string[0];
			    }
			    else
			    {
			        ImportCompanies = new string[0];
			    }

				var from = settings.GetValue(Config.LastEventName);
				if (from == null)
					return result.Fail(Task.LogError("{0} setting does not exist.", Config.LastEventName));

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

		public AccessHistory() :
			base()
		{
			ActivityName = "AccessHistory";
		}

		protected override IAPI CreateAPI(Task.Config config)
		{
			var api = base.CreateAPI(config);

			if (!(api is IAuthentication) || !(api is IAccessHistory))
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
		    Configuration = config;

			var api = CreateAPI(config) as IAuthentication;
			var apiHistory = api as IAccessHistory;


			var login = api.Login(config.Username, config.Password, config.Link);
			if (login.Failed)
				return result.Merge(login).Fail(LogError("unable to log into external system."));

			var logCount = 0;
			var personCount = 0;
		    var rowTotal = 0;
		    var filteredPersons = 0;
		    var filteredReaders = 0;
		    var filteredPortals = 0;
		    var filteredAccess = 0;
            
			try
			{
				//var history = apiHistory.GetAccessHistory(config.LastEvent.Subtract(Profile.Schedule.RepeatInterval));
				var history = apiHistory.GetAccessHistory(config.LastEvent);

				if (history.Failed)
					return result.Merge(history).Fail(LogError("unable to get access history."));

			    rowTotal = history.Entity.Count;

				foreach (var access in history.Entity)
				{
					//LogMessage("Trying to process Access record {0}. Person ({1}), Portal ({2}), Reader({3})", access.ExternalId, access.Person.ExternalId, access.Portal.ExternalId, access.Reader.ExternalId);

					//if access id is already in R1SM then skip
				    if (access.KeysExist())
				    {
				        filteredAccess++;
				        continue;
				    }

				    //get portal
                    var r1Portal = access.Portal.Get();
                    if (r1Portal.Failed)
                    {
                        // Not an error - ignore this row
                        filteredPortals++;
                        // LogError("unable to locate external portal id ({0}).", access.Portal.ExternalId);
                        //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Portal Error ({0}).", access.Portal.ExternalId));
                        continue;
                    }

                    access.PortalId = r1Portal.Entity.InternalId;
                    access.Portal = r1Portal.Entity;

                    //get reader
                    var r1Reader = access.Reader.Get();
                    if (r1Reader.Failed)
                    {
                        // Not an error - ignore this row
                        filteredReaders++;
                        // LogError("unable to locate external reader id ({0}).", access.Reader.ExternalId);
                        //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Reader Error ({0}).", access.Reader.ExternalId));
                        continue;
                    }

                    access.ReaderId = r1Reader.Entity.InternalId;
                    access.Reader = r1Reader.Entity;

                    //get person
					var r1Person = access.Person.Get();
				    if (config.ImportPerson)
				    {
                        // Get person details from S2
                        var apiPerson = apiHistory.RetrievePerson(access.Person.ExternalId);
                        if (apiPerson.Failed)
                        {
                            // By pass and not log
                            //                            LogError("unable to locate external person: {0}. Person ({1}), Portal ({2}), Reader({3})", access.ExternalId, access.Person.ExternalId, access.Portal.ExternalId, access.Reader.ExternalId);
                            continue;
                        }

                        if (!FilterPerson(apiPerson.Entity) && !FilterPerson(r1Person.Entity))
                        {
                            filteredPersons++;
                            //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Access Import Skipped Due to person filter: [{0}] {1}", access.InternalId, apiPerson.Entity.udf4));
                            continue;
                        }

                        if (r1Person.Failed && r1Person.Details.ContainsKey("NotFound"))
				        {
				            // Add person into R1SM
                            r1Person = apiPerson.Entity.Add();
                            if (r1Person.Failed)
                            {
                                LogError("unable to import new person ({0}).", access.Person.ExternalId);
                                continue;
                            }

                            personCount++;
                            // LogMessage("added new Person ({0}).", r1Person.Entity.ExternalId);
                        }
				        else
				        {
                            if (r1Person.Failed)
                            {
                                LogError("person ({0}) not found in R1SM.", access.Person.ExternalId);
                                continue;
                            }

				            // Update person in R1SM.
				            r1Person = apiPerson.Entity.Update();
				            if (r1Person.Failed)
				            {
				                LogError("unable to update existing person ({0}). {1}", access.Person.ExternalId, r1Person.Message);
				                continue;
				            }

				            personCount++;
				            // LogMessage("updated existing Person ({0}).", r1Person.Entity.ExternalId);
				        }
				    }

					access.PersonId = r1Person.Entity.InternalId;
					access.Person = r1Person.Entity;

                    //skip any access type not in filter
                    if (!Filter(access))
                    {
                        filteredAccess++;
                        //EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Access Import Skipped Due to Access filter: [{0}] {1}", access.InternalId, access.Person.udf4));
                        continue;
                    }

					//add access to R1SM
					var add = access.Add();
					if (add.Failed)
					{
						LogError("unable to add access log ({0}).", access.ExternalId);
						continue;
					}

					logCount++;
					//LogMessage("added new access log ({0}).", add.Entity.ExternalId);

					config.LastEvent = access.Accessed;
				}
			}
			catch (Exception ex)
			{
				result.Entity = string.Format("Errors Occurred: {0}: {1}", ex.Message, ex.StackTrace);
                EventLog.WriteEntry("R1SM.S2Import.Debug", string.Format("Errors Occurred: {0}: {1}", ex.Message, ex.StackTrace));
            }
			finally
			{
                result.Entity = string.Format("Read {0} rows || Added {1} logs, {2} people || Ignored {3} people, {4} readers, {5} portals, {6} logs.", rowTotal, logCount, personCount, filteredPersons, filteredReaders, filteredPortals, filteredAccess);
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
        public virtual bool FilterPerson(Person person)
        {
            return true;
        }
        #endregion
	}
}
