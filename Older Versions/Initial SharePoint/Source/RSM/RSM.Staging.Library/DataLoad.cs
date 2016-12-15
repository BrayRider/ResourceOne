using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using RSM.Artifacts;
using RSM.Staging.Library.Data;
using RSM.Support;

using EntityType = RSM.Service.Library.Model.EntityType;
using Portal = RSM.Support.Portal;
using Reader = RSM.Support.Reader;
using R1SM = RSM.Staging.Library.Data.Settings.R1SM;
using S2Import = RSM.Staging.Library.Data.Settings.S2Import;
using S2PeopleImport = RSM.Staging.Library.Data.Settings.S2PeopleImport;
using TrackExport = RSM.Staging.Library.Data.Settings.TrackExport;
using LubrizolImport = RSM.Staging.Library.Data.Settings.LubrizolImport;
using LubrizolExport = RSM.Staging.Library.Data.Settings.LubrizolExport;

namespace RSM.Staging.Library
{
	public class DataLoad : Factory
	{
		#region Constructors
		public DataLoad() : base()
		{
			Context = new RSMDataModelDataContext {DeferredLoadingEnabled = false};
		}

		public DataLoad(RSMDataModelDataContext context) : base(context)
		{
			Context = context;
			Context.DeferredLoadingEnabled = false;
		}
		#endregion

		#region Actions
		public void Clean(Models.StagingTools model)
		{
			AccessHistoryClean(model);

			ReaderClean(model);
			PortalClean(model);
			LocationClean(model);
			PersonClean(model);

			ExternalApplicationKeysClean(model);

			SettingsClean(model);
			ExternalSystemsClean(model);
		}

		public void Run(Models.StagingTools model)
		{
			Clean(model);

			ExternalSystemsStage(model);
			SettingsStage(model);

			PersonStage(model);
			LocationStage(model);
			PortalStage(model);
			ReaderStage(model);
		}
		#endregion

		#region ExternalSystems

		public void ExternalSystemsStage(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.R1SM)
			{
				var item = Data.ExternalSystems.R1SM;
				item.Settings = null;
				item.ExternalApplicationKeys = null;
				Context.ExternalSystems.InsertOnSubmit(item);
			}

			if (model.S2)
			{
				if (model.S2Incoming)
				{
					var item = Data.ExternalSystems.S2In;
					item.Settings = null;
					item.ExternalApplicationKeys = null;
					Context.ExternalSystems.InsertOnSubmit(item);
				}
				if (model.S2Outgoing)
				{
					var item = Data.ExternalSystems.S2Out;
					item.Settings = null;
					item.ExternalApplicationKeys = null;
					Context.ExternalSystems.InsertOnSubmit(item);
				}
			}

			if (model.Track)
			{
				if (model.TrackOutgoing)
				{
					var item = Data.ExternalSystems.TrackOut;
					item.Settings = null;
					item.ExternalApplicationKeys = null;
					Context.ExternalSystems.InsertOnSubmit(item);
				}
			}

			if (model.PeopleSoft)
			{
				if (model.PsIncoming)
				{
					var item = Data.ExternalSystems.PsIn;
					item.Settings = null;
					item.ExternalApplicationKeys = null;
					Context.ExternalSystems.InsertOnSubmit(item);
				}
			}

			if (model.Lubrizol)
			{
				if (model.LubrizolIncoming)
				{
					var item = Data.ExternalSystems.LubrizolIn;
					item.Settings = null;
					item.ExternalApplicationKeys = null;
					Context.ExternalSystems.InsertOnSubmit(item);
				}
			}
			Context.SubmitChanges();
		}

		public void ExternalSystemsClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.Lubrizol)
			{
				Delete(Context, Data.ExternalSystems.LubrizolIn);
			}

			if (model.PeopleSoft)
			{
				Delete(Context, Data.ExternalSystems.PsIn);
			}

			if (model.Track)
			{
				Delete(Context, Data.ExternalSystems.TrackOut);
			}

			if (model.S2)
			{
				Delete(Context, Data.ExternalSystems.S2In);
				Delete(Context, Data.ExternalSystems.S2Out);
			}

			if (model.R1SM)
			{
				Delete(Context, Data.ExternalSystems.R1SM);
			}

			Context.SubmitChanges();
		}

		#endregion

		#region Settings

		private static void CreateSetting(Factory factory, ExternalSystem system, string prefix, string name, string label, string value, bool viewable = true, string inputType = InputTypes.Checkbox)
		{
			factory.createSetting(name, label, value, prefix: prefix, viewable: viewable, inputType: inputType, system: system);
		}

		public void R1SMSettings(string validationKey)
		{
			const string prefix = "R1SM";

			var r1SM = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.R1SM.Id);

			if (r1SM == null) return;

			var factory = new Factory(Context);

			factory.createSetting(r1SM, prefix, R1SM.RuleEngineAllow);
			factory.createSetting(r1SM, prefix, R1SM.JobCodesFirst, inputType: InputTypes.Text);
			factory.createSetting(r1SM, prefix, R1SM.RequireAccessApproval, inputType: InputTypes.Text);
			factory.createSetting(r1SM, prefix, R1SM.AdminPass, EncryptPassword("R!Lubriz0l", validationKey));

			Context.SubmitChanges();
		}

		public void S2InSettings(string validationKey)
		{
			var s2 = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.S2In.Id);

			if (s2 == null) return;

			var factory = new Factory(Context);

			string prefix = S2Import.DefaultPrefix;
			factory.createSetting(s2, prefix, S2Import.Available);
			factory.createSetting(s2, prefix, S2Import.Repeat);
			factory.createSetting(s2, prefix, S2Import.RepeatInterval, "1");
			factory.createSetting(s2, prefix, S2Import.ServiceAddress, "http://10.1.1.234/goforms/nbapi");
			factory.createSetting(s2, prefix, S2Import.ServiceAccount, "admin");
			factory.createSetting(s2, prefix, S2Import.ServicePassword, "072159245241245031239120017047219193126250124056");
			factory.createSetting(s2, prefix, S2Import.LastAccessed);
			factory.createSetting(s2, prefix, S2Import.PersonImport);

			prefix = S2PeopleImport.DefaultPrefix;
			factory.createSetting(s2, prefix, S2PeopleImport.Available);
			factory.createSetting(s2, prefix, S2PeopleImport.Repeat, "true");
			factory.createSetting(s2, prefix, S2PeopleImport.RepeatInterval, "120");
			factory.createSetting(s2, prefix, S2PeopleImport.ServiceAddress, "http://10.1.1.234/goforms/nbapi");
			factory.createSetting(s2, prefix, S2PeopleImport.ServiceAccount, "admin");
			factory.createSetting(s2, prefix, S2PeopleImport.ServicePassword, "072159245241245031239120017047219193126250124056");
			factory.createSetting(s2, prefix, S2PeopleImport.ImageImport);
			factory.createSetting(s2, prefix, S2PeopleImport.FieldsToUpdate, "FirstName,MiddleName,LastName,ExternalUpdated,Image,"
				+ "udf1,udf2,udf3,udf4,udf5,udf6,udf7,udf8,udf9,udf10,udf11,udf12,udf13,udf14,udf15,udf16,udf17,udf18,udf19,udf20");

			Context.SubmitChanges();
		}

		public void TrackSettings(string validationKey)
		{
			const string prefix = "TrackExport";

			var trackOut = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.TrackOut.Id);

			if (trackOut == null) return;

			var factory = new Factory(Context);

			factory.createSetting(trackOut, prefix, TrackExport.Available);
			factory.createSetting(trackOut, prefix, TrackExport.PersonExport);
			factory.createSetting(trackOut, prefix, TrackExport.LocationExport);
			factory.createSetting(trackOut, prefix, TrackExport.CompanyExport);
			factory.createSetting(trackOut, prefix, TrackExport.AccessExport);
			factory.createSetting(trackOut, prefix, TrackExport.Repeat);
			factory.createSetting(trackOut, prefix, TrackExport.RepeatInterval, "1");
			factory.createSetting(trackOut, prefix, TrackExport.ServiceAddress, "https://lubrizol.tracksoftware.com/ACSToTrackWebSvc/ACS2TrackWebSvc.asmx");
			factory.createSetting(trackOut, prefix, TrackExport.Account, "admin");
			factory.createSetting(trackOut, prefix, TrackExport.Password, "072159245241245031239120017047219193126250124056");
			factory.createSetting(trackOut, prefix, TrackExport.EventCode, "8");
			factory.createSetting(trackOut, prefix, TrackExport.SysId, "1");
			factory.createSetting(trackOut, prefix, TrackExport.DataSource, "TSTLBZDB");
			factory.createSetting(trackOut, prefix, TrackExport.SourceSystem, ExternalSystems.S2In.Id.ToString(CultureInfo.InvariantCulture));
			factory.createSetting(trackOut, prefix, TrackExport.LastAccessEvent);

			Context.SubmitChanges();
		}

		public void LubrizolInSettings(string validationKey)
		{
			const string prefix = "LubrizolIn";

			var system = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.LubrizolIn.Id);

			if (system == null) return;

			var factory = new Factory(Context);

			factory.createSetting(system, prefix, LubrizolImport.Available);
			factory.createSetting(system, prefix, LubrizolImport.Repeat);
			factory.createSetting(system, prefix, LubrizolImport.RepeatInterval, "1");
			factory.createSetting(system, prefix, LubrizolImport.SqlConnection, "Integrated Security=SSPI;Persist Security Info=False;User ID=r1sm;Initial Catalog=LubrizolConnector;Data Source=.");

			Context.SubmitChanges();
		}

		public void LubrizolOutSettings(string validationKey)
		{
			const string prefix = "LubrizolOut";

			var system = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.LubrizolOut.Id);

			if (system == null) return;

			var factory = new Factory(Context);

			/*
Available = Factory.CreateSetting("Available", "Is the Lubrizol export available.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.LubrizolOut);
Repeat = Factory.CreateSetting("Repeat", "Allow export task to repeat.", "true", 0, true, InputTypes.Checkbox, ExternalSystems.LubrizolOut);
RepeatInterval = Factory.CreateSetting("RepeatInterval", "Export repeat interval in minutes.", "3", 0, true, InputTypes.Text, ExternalSystems.LubrizolOut);
ServiceAddress = Factory.CreateSetting("ServiceAddress", "Service Address", "http://localhost", 2, true, InputTypes.Text, ExternalSystems.LubrizolOut);
Account = Factory.CreateSetting("ServiceAccount", "Service User Id", "admin", 3, true, InputTypes.Text, ExternalSystems.LubrizolOut);
Password = Factory.CreateSetting("ServicePassword", "Service Password", "admin", 4, true, InputTypes.Password, ExternalSystems.LubrizolOut);
LastUpdated = Factory.CreateSetting("LastUpdated", "Last updated timestamp on last person record exported.", "", 0, false, InputTypes.Text, ExternalSystems.LubrizolOut);
SourceSystem = Factory.CreateSetting("SourceSystem", "System whose data will be exported.", ExternalSystems.S2In.Id.ToString(), 0, false, InputTypes.Text, ExternalSystems.LubrizolOut);
ActiveEmployeeLibrary = Factory.CreateSetting("ActiveEmployeeLibrary", "Active Employees Library Url", "http://localhost", 7, true, InputTypes.Text, ExternalSystems.LubrizolOut);
InactiveEmployeeLibrary = Factory.CreateSetting("InactiveEmployeeLibrary", "Inactive Employees Library Url", "http://localhost", 7, true, InputTypes.Text, ExternalSystems.LubrizolOut);
			 */

			var crypt = new QuickAES();

			factory.createSetting(system, prefix, LubrizolExport.Available);
			factory.createSetting(system, prefix, LubrizolExport.Repeat);
			factory.createSetting(system, prefix, LubrizolExport.RepeatInterval, "1");
			factory.createSetting(system, prefix, LubrizolExport.ServiceAddress, "http://cencle06/_vti_bin/copy.asmx");
			factory.createSetting(system, prefix, LubrizolExport.Account, "cencle06\\administrator");
			factory.createSetting(system, prefix, LubrizolExport.Password, crypt.EncryptToString("T@ipan11"));
			factory.createSetting(system, prefix, LubrizolExport.LastUpdated);
			factory.createSetting(system, prefix, LubrizolExport.SourceSystem);
			factory.createSetting(system, prefix, LubrizolExport.ActiveEmployeeLibrary, "http://cencle06:10807/Active Employees");
			factory.createSetting(system, prefix, LubrizolExport.InactiveEmployeeLibrary, "http://cencle06:10807/Inactive Employees");
			factory.createSetting(system, prefix, LubrizolExport.SqlConnection, "Integrated Security=SSPI;Persist Security Info=False;User ID=r1sm;Initial Catalog=LubrizolConnector;Data Source=.");

			Context.SubmitChanges();
		}

		public void SettingsStage(Models.StagingTools model)
		{
			if (model.R1SM)
			{
				R1SMSettings(model.ValidationKey);
			}

			if (model.S2Incoming)
			{
				S2InSettings(model.ValidationKey);
			}

			if (model.TrackOutgoing)
			{
				TrackSettings(model.ValidationKey);
			}

			if (model.LubrizolIncoming)
			{
				LubrizolInSettings(model.ValidationKey);
			}
			Context.SubmitChanges();
		}

		public void SettingsClean(Models.StagingTools model)
		{
			Delete(Context, Context.Settings.ToList());

			Context.SubmitChanges();
		}

		#endregion

		#region Person

		public void PersonStage(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var s2Id = Data.ExternalSystems.S2In.Id;
				var entity = Data.People.R1Person1;
				var item = createPerson(entity.FirstName, entity.LastName, string.Empty, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Person, Data.People.R1Person1ExternalId, s2Id, item.PersonID, EntityAction.InsertAndSubmit);

				entity = Data.People.R1Person2;
				item = createPerson(entity.FirstName, entity.LastName, entity.MiddleName, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Person, Data.People.R1Person2ExternalId, s2Id, item.PersonID, EntityAction.InsertAndSubmit);
			}

			Context.SubmitChanges();
		}

		public void PersonClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var s2Id = Data.ExternalSystems.S2In.Id;

				if (model.CleanAllPeople)
				{
					var entityType = Enum.GetName(typeof(EntityType), EntityType.Person);

					Context.ExternalApplicationKeys.DeleteAllOnSubmit(Context.ExternalApplicationKeys.Where(x => x.EntityType == entityType && x.SystemId == s2Id));

					Context.Persons.DeleteAllOnSubmit(Context.Persons);
				}
				else
				{
					var extId = Data.People.R1Person1ExternalId;
					Factory.DeleteWithKeys<Person>(Context, EntityType.Person, extId, s2Id);

					extId = Data.People.R1Person2ExternalId;
					Factory.DeleteWithKeys<Person>(Context, EntityType.Person, extId, s2Id);
				}
			}

			Context.SubmitChanges();
		}

		#endregion

		#region Location

		public void LocationStage(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var sysId = Data.ExternalSystems.TrackOut.Id;
				var item = createLocation("Company A", action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Location, Data.Locations.Location1ExternalId, sysId, item.LocationID, EntityAction.InsertAndSubmit);

				item = createLocation("Company B", action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Location, Data.Locations.Location2ExternalId, sysId, item.LocationID, EntityAction.InsertAndSubmit);
			}

			Context.SubmitChanges();
		}

		public void LocationClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var sysId = Data.ExternalSystems.TrackOut.Id;
				var extId = Data.Locations.Location1ExternalId;
				Factory.DeleteWithKeys<Location>(Context, EntityType.Location, extId, sysId);

				extId = Data.Locations.Location2ExternalId;
				Factory.DeleteWithKeys<Location>(Context, EntityType.Location, extId, sysId);
			}

			Context.SubmitChanges();
		}

		#endregion

		#region Portal

		public void PortalStage(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var sysId = Data.ExternalSystems.S2In.Id;
				var trackId = Data.ExternalSystems.TrackOut.Id;
				var entityType = Enum.GetName(typeof(EntityType), EntityType.Location);

				// Location 1
				var locId = Context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == Data.Locations.Location1ExternalId
					&& x.SystemId == trackId && x.EntityType == entityType).InternalId;

				var entity = Data.Portal.Location1Portal1;
				var item = createPortal(entity.Name, locId, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Portal, Data.Portal.Location1Portal1ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);

				// Location 2
				locId = Context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == Data.Locations.Location2ExternalId
					&& x.SystemId == trackId && x.EntityType == entityType).InternalId;

				entity = Data.Portal.Location2Portal1;
				item = createPortal(entity.Name, locId, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Portal, Data.Portal.Location2Portal1ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);

				entity = Data.Portal.Location2Portal2;
				item = createPortal(entity.Name, locId, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Portal, Data.Portal.Location2Portal2ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);
			}

			Context.SubmitChanges();
		}

		public void PortalClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var s2Id = Data.ExternalSystems.S2In.Id;
				var extId = Data.Portal.Location1Portal1ExternalId;
				Factory.DeleteWithKeys<Portal>(Context, EntityType.Portal, extId, s2Id);

				extId = Data.Portal.Location2Portal1ExternalId;
				Factory.DeleteWithKeys<Portal>(Context, EntityType.Portal, extId, s2Id);

				extId = Data.Portal.Location2Portal2ExternalId;
				Factory.DeleteWithKeys<Portal>(Context, EntityType.Portal, extId, s2Id);
			}

			Context.SubmitChanges();
		}

		#endregion

		#region Reader

		public void ReaderStage(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var sysId = Data.ExternalSystems.S2In.Id;
				var entityType = Enum.GetName(typeof(EntityType), EntityType.Portal);

				// Location 1 Portal 1
				var portalId = Context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == Data.Portal.Location1Portal1ExternalId && x.SystemId == sysId && x.EntityType == entityType).InternalId;
				var entity = Data.Reader.Location1Reader1;
				var item = createReader(entity.Name, portalId, entity.Direction, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Reader, Data.Reader.Location1Reader1ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);

				entity = Data.Reader.Location1Reader2;
				item = createReader(entity.Name, portalId, entity.Direction, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Reader, Data.Reader.Location1Reader2ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);

				// Location 2 Portal 1
				portalId = Context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == Data.Portal.Location2Portal1ExternalId && x.SystemId == sysId && x.EntityType == entityType).InternalId;
				entity = Data.Reader.Location2Reader1;
				item = createReader(entity.Name, portalId, entity.Direction, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Reader, Data.Reader.Location2Reader1ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);

				entity = Data.Reader.Location2Reader2;
				item = createReader(entity.Name, portalId, entity.Direction, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Reader, Data.Reader.Location2Reader2ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);

				// Location 2 Portal 2
				portalId = Context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == Data.Portal.Location2Portal2ExternalId && x.SystemId == sysId && x.EntityType == entityType).InternalId;
				entity = Data.Reader.Location2Reader3;
				item = createReader(entity.Name, portalId, entity.Direction, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Reader, Data.Reader.Location2Reader3ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);

				entity = Data.Reader.Location2Reader4;
				item = createReader(entity.Name, portalId, entity.Direction, action: EntityAction.InsertAndSubmit);
				createExternalApplicationKey(EntityType.Reader, Data.Reader.Location2Reader4ExternalId, sysId, item.Id, EntityAction.InsertAndSubmit);
			}

			Context.SubmitChanges();
		}

		public void ReaderClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if (model.S2Incoming || model.Track)
			{
				var s2Id = Data.ExternalSystems.S2In.Id;
				var extId = Data.Reader.Location1Reader1ExternalId;
				Factory.DeleteWithKeys<Reader>(Context, EntityType.Reader, extId, s2Id);

				extId = Data.Reader.Location1Reader2ExternalId;
				Factory.DeleteWithKeys<Reader>(Context, EntityType.Reader, extId, s2Id);


				extId = Data.Reader.Location2Reader1ExternalId;
				Factory.DeleteWithKeys<Reader>(Context, EntityType.Reader, extId, s2Id);

				extId = Data.Reader.Location2Reader2ExternalId;
				Factory.DeleteWithKeys<Reader>(Context, EntityType.Reader, extId, s2Id);

				extId = Data.Reader.Location2Reader3ExternalId;
				Factory.DeleteWithKeys<Reader>(Context, EntityType.Reader, extId, s2Id);

				extId = Data.Reader.Location2Reader4ExternalId;
				Factory.DeleteWithKeys<Reader>(Context, EntityType.Reader, extId, s2Id);
			}

			Context.SubmitChanges();
		}

		#endregion

		#region AccessHistory

		public void AccessHistoryClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if ((model.S2Incoming || model.Track)
				&& model.CleanAllAccessHistory)
			{
				Context.AccessHistories.DeleteAllOnSubmit(Context.AccessHistories);
			}

			Context.SubmitChanges();
		}

		#endregion

		#region ExternalApplicationKeys

		public void ExternalApplicationKeysClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

			if ((model.S2Incoming || model.Track)
				&& model.CleanAllExternalAppKeys)
			{
				Context.ExternalApplicationKeys.DeleteAllOnSubmit(Context.ExternalApplicationKeys);
			}

			Context.SubmitChanges();
		}

		#endregion
	}
}
