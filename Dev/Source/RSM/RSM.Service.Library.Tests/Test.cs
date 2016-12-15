using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Artifacts;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Model;
using RSMDB = RSM.Support;
using RSM.Service.Library.Tests.Model;
using StageFactory = RSM.Staging.Library.Factory;

namespace RSM.Service.Library.Tests
{
	[TestClass]
	public class Test
	{
		public static ExternalSystem S2In = ExternalSystem.S2In;
		public static ExternalSystem TrackOut = ExternalSystem.TrackOut;

		[TestInitialize]
		public virtual void Initialize()
		{
			Cleanup();

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var sysList = new List<RSMDB.ExternalSystem>()
				{
					StageFactory.CreateExternalSystem(1, Constants.R1SMSystemName, RSMDB.ExternalSystemDirection.None),
					StageFactory.CreateExternalSystem(2, "S2 Import", RSMDB.ExternalSystemDirection.Incoming),
					StageFactory.CreateExternalSystem(3, "Track", RSMDB.ExternalSystemDirection.Outgoing),
					StageFactory.CreateExternalSystem(4, "S2 Export", RSMDB.ExternalSystemDirection.Outgoing),
					StageFactory.CreateExternalSystem(5, "PeopleSoft", RSMDB.ExternalSystemDirection.Incoming),
				};

				var settingsList = new List<RSMDB.Setting>()
				{
					StageFactory.CreateSetting(1, "RuleEngineAllow", "Allow the R1SM rule engine to assign roles.", "false", 0, true, InputTypes.Checkbox, sysList[0]),
					StageFactory.CreateSetting(2, "JobCodesFirst", "Show job codes before job titles when editing rules.", "false", 1, true, InputTypes.Checkbox, sysList[0]),
					StageFactory.CreateSetting(3, "RequireAccessApproval", "Require approval of changes made by the rule engine", "false", 2, false, InputTypes.Checkbox, sysList[0]),
					StageFactory.CreateSetting(4, "AdminPass", "New Admin Password", "Testing", 3, true, InputTypes.Password, sysList[0]),
					StageFactory.CreateSetting(5, "LevelImport", "Allow importing of levels from S2.", "false", 1, true, InputTypes.Text, sysList[1]),
					StageFactory.CreateSetting(6, "S2Import.ServiceAddress", "Appliance Address", "http://localhost", 2, true, InputTypes.Text, sysList[1]),
					StageFactory.CreateSetting(7, "S2Import.PersonImport", "Allow importing of People from S2.", "true", 0, true, InputTypes.Checkbox, sysList[1]),
					StageFactory.CreateSetting(8, "TrackExport.PersonExport", "Allow exporting of People to Track", "true", 0, true, InputTypes.Checkbox, sysList[2]),
					StageFactory.CreateSetting(9, "TrackExport.LocationExport", "Allow exporting of Locations to Track", "true", 1, true, InputTypes.Checkbox, sysList[2]),
					StageFactory.CreateSetting(10, "TrackExport.AccessExport", "Allow exporting of Access History to Track", "true", 2, true, InputTypes.Checkbox, sysList[2]),
					StageFactory.CreateSetting(11, "S2Import.ServiceAccount", "S2 Service User Id", "asdfasasdfasd", 3, true, InputTypes.Text, sysList[1]),
					StageFactory.CreateSetting(12, "S2Import.ServicePassword", "S2 Service Password", "admin", 4, true, InputTypes.Password, sysList[1]),
					StageFactory.CreateSetting(13, "PersonExport", "Allow exporting of user data and roles to S2", "false", 0, false, InputTypes.Checkbox, sysList[1]),

					StageFactory.CreateSetting(14, "S2Import.Repeat", "Allow S2 import task to repeat.", "true", 0, false, InputTypes.Checkbox, sysList[1]),
					StageFactory.CreateSetting(15, "S2Import.RepeatInterval", "S2 import repeat interval in minutes.", "3", 0, false, InputTypes.Text, sysList[1]),
					StageFactory.CreateSetting(16, "S2Import.LastAccessEvent", "Date time of last access record imported from S2.", "", 0, false, InputTypes.Text, sysList[1]),

					StageFactory.CreateSetting(17, "TrackExport.ServiceAddress", "Appliance Address", "http://localhost", 2, true, InputTypes.Text, sysList[2]),
					StageFactory.CreateSetting(18, "TrackExport.ServiceAccount", "Track Service User Id", "asdfasasdfasd", 3, true, InputTypes.Text, sysList[2]),
					StageFactory.CreateSetting(19, "TrackExport.ServicePassword", "Track Service Password", "admin", 4, true, InputTypes.Password, sysList[2]),
					StageFactory.CreateSetting(20, "TrackExport.LastAccessEvent", "Date time of last access record exported to Track.", "", 0, false, InputTypes.Text, sysList[2]),
					StageFactory.CreateSetting(21, "TrackExport.SourceSystem", "System whose data will be exported to Track.", "", 0, false, InputTypes.Text, sysList[2]),
					StageFactory.CreateSetting(22, "TrackExport.Repeat", "Allow Track export task to repeat.", "true", 0, false, InputTypes.Checkbox, sysList[2]),
					StageFactory.CreateSetting(23, "TrackExport.RepeatInterval", "Track export repeat interval in minutes.", "3", 0, false, InputTypes.Text, sysList[2]),
					StageFactory.CreateSetting(24, "TrackExport.LastAccessEvent", "Date time of last access record exported to Track.", "", 0, false, InputTypes.Text, sysList[2]),

                	StageFactory.CreateSetting(25, "S2Import.Contractors", "Comma delimited list of contractors to get from S2.", "S & B,Mustang", 0, false, InputTypes.Text, sysList[1]),
                };

				context.ExternalSystems.InsertAllOnSubmit(sysList);
				context.Settings.InsertAllOnSubmit(settingsList);
				context.SubmitChanges();
			}
		}

		[TestCleanup]
		public virtual void Cleanup()
		{
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				context.AccessHistories.DeleteAllOnSubmit(context.AccessHistories);
				context.ExternalApplicationKeys.DeleteAllOnSubmit(context.ExternalApplicationKeys);
				context.Readers.DeleteAllOnSubmit(context.Readers);
				context.Portals.DeleteAllOnSubmit(context.Portals);
				context.Locations.DeleteAllOnSubmit(context.Locations);
				context.Persons.DeleteAllOnSubmit(context.Persons);
				context.Settings.DeleteAllOnSubmit(context.Settings);
				context.ExternalSystems.DeleteAllOnSubmit(context.ExternalSystems);
				context.SubmitChanges();
			}
		}
	}
}
