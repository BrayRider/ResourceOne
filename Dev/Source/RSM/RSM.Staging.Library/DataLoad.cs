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

			Context.SubmitChanges();
		}

		public void ExternalSystemsClean(Models.StagingTools model)
		{
			Context.Refresh(RefreshMode.KeepCurrentValues);

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

		private static void CreateSetting(Factory factory, ExternalSystem system, string prefix, int id, string name, string label, string value, bool viewable = true, string inputType = InputTypes.Checkbox)
		{
			var idBase = system.Id * 1000;

			factory.createSetting(idBase + id, string.Format("{0}.{1}", prefix, name), label, value, id, viewable, inputType, system);
		}

		public void R1SMSettings(string validationKey)
		{
			const string prefix = "R1SM";

			var r1SM = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.R1SM.Id);

			if (r1SM == null) return;

			var factory = new Factory(Context);

			CreateSetting(factory, r1SM, prefix, 1, "RuleEngineAllow", "Allow the R1SM rule engine to assign roles", "false", false);
			CreateSetting(factory, r1SM, prefix, 2, "JobCodesFirst", "Show job codes before job titles when editing rules", "false", false, InputTypes.Text);
			CreateSetting(factory, r1SM, prefix, 3, "RequireAccessApproval", "Require approval of changes made by the rule engine", "false", false, InputTypes.Text);
			CreateSetting(factory, r1SM, prefix, 4, "AdminPass", "New Admin Password", EncryptPassword("R!Lubriz0l", validationKey), false, InputTypes.Password);

			Context.SubmitChanges();
		}

		public void S2InSettings(string validationKey)
		{
			const string prefix = "S2Import";

			var s2 = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.S2In.Id);

			if (s2 == null) return;

			var factory = new Factory(Context);

			CreateSetting(factory, s2, prefix, 0, "Available", "Is the S2 system available.", "true");
			CreateSetting(factory, s2, prefix, 1, "PersonImport", "Allow importing of People from S2.", "true");
			CreateSetting(factory, s2, prefix, 2, "Repeat", "Allow S2 import task to repeat.", "true");
			CreateSetting(factory, s2, prefix, 3, "RepeatInterval", "S2 import repeat interval in minutes.", "1", true, InputTypes.Text);
			CreateSetting(factory, s2, prefix, 4, "ServiceAddress", "Appliance Address", "http://10.1.1.234/goforms/nbapi", true, InputTypes.Text);
			CreateSetting(factory, s2, prefix, 5, "ServiceAccount", "S2 Service User Id", "admin", true, InputTypes.Text);
			CreateSetting(factory, s2, prefix, 6, "ServicePassword", "S2 Service Password", "072159245241245031239120017047219193126250124056", true, InputTypes.Password);
			CreateSetting(factory, s2, prefix, 7, "LastAccessEvent", "Date time on last S2 record imported.", "", false, InputTypes.Text);

			Context.SubmitChanges();
		}

		public void TrackSettings(string validationKey)
		{
			const string prefix = "TrackExport";

			var trackOut = Context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.TrackOut.Id);

			if (trackOut == null) return;

			var factory = new Factory(Context);

			CreateSetting(factory, trackOut, prefix, 0, "Available", "Is the track system available.", "true");
			CreateSetting(factory, trackOut, prefix, 1, "PersonExport", "Allow export of People.", "true");
			CreateSetting(factory, trackOut, prefix, 2, "LocationExport", "Allow exporting of Locations to Track", "true");
			CreateSetting(factory, trackOut, prefix, 3, "CompanyExport", "Allow exporting of Companies to Track", "true");
			CreateSetting(factory, trackOut, prefix, 4, "AccessExport", "Allow exporting of Access History to Track", "true");
			CreateSetting(factory, trackOut, prefix, 5, "Repeat", "Allow export task to repeat.", "true");
			CreateSetting(factory, trackOut, prefix, 6, "RepeatInterval", "Export repeat interval in minutes.", "1", true, InputTypes.Text);
			CreateSetting(factory, trackOut, prefix, 7, "ServiceAddress", "Appliance Address", "https://lubrizol.tracksoftware.com/ACSToTrackWebSvc/ACS2TrackWebSvc.asmx", true, InputTypes.Text);
			CreateSetting(factory, trackOut, prefix, 8, "ServiceAccount", "Service User Id", "admin", true, InputTypes.Text);
			CreateSetting(factory, trackOut, prefix, 9, "ServicePassword", "Service Password", "072159245241245031239120017047219193126250124056", true, InputTypes.Password);
			CreateSetting(factory, trackOut, prefix, 10, "EventCode", "Event Code value for export to Track.", "8", true, InputTypes.Text);
			CreateSetting(factory, trackOut, prefix, 11, "SysId", "System Id value for export to Track.", "1", true, InputTypes.Text);
			CreateSetting(factory, trackOut, prefix, 12, "DataSource", "DataSource value for export to Track.", "TSTLBZDB", true, InputTypes.Text);
			CreateSetting(factory, trackOut, prefix, 13, "SourceSystem", "System whose data will be exported to Track.", ExternalSystems.S2In.Id.ToString(CultureInfo.InvariantCulture), false, InputTypes.Text);
			CreateSetting(factory, trackOut, prefix, 14, "LastAccessEvent", "Date time on last record exported.", "", false, InputTypes.Text);

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
