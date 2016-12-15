using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Artifacts;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Model;
using RSM.Staging.Library.Data;
using RSMDB = RSM.Support;
using RSM.Service.Library.Tests.Model;

using StageFactory = RSM.Staging.Library.Factory;
using R1Setting = RSM.Staging.Library.Data.Settings.R1SM;
using S2Import = RSM.Staging.Library.Data.Settings.S2Import;
using S2PeopleImport = RSM.Staging.Library.Data.Settings.S2PeopleImport;
using S2Export = RSM.Staging.Library.Data.Settings.S2Export;
using TrackExport = RSM.Staging.Library.Data.Settings.TrackExport;
using LubrizolImport = RSM.Staging.Library.Data.Settings.LubrizolImport;
using LubrizolExport = RSM.Staging.Library.Data.Settings.LubrizolExport;


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
				              		StageFactory.CreateExternalSystem(6, "Lubrizol In", RSMDB.ExternalSystemDirection.Incoming),
				              		StageFactory.CreateExternalSystem(7, "Lubrizol Out", RSMDB.ExternalSystemDirection.Outgoing),
				              	};

				var factory = new StageFactory(context);
				var settingsList = new List<RSMDB.Setting>()
				{
					factory.createSetting(sysList[0], R1Setting.DefaultPrefix, R1Setting.RuleEngineAllow),
					factory.createSetting(sysList[0], R1Setting.DefaultPrefix, R1Setting.JobCodesFirst),
					factory.createSetting(sysList[0], R1Setting.DefaultPrefix, R1Setting.RequireAccessApproval),
					factory.createSetting(sysList[0], R1Setting.DefaultPrefix, R1Setting.AdminPass, "Testing"),

					factory.createSetting(sysList[1], S2Import.DefaultPrefix, S2Import.ServiceAddress, "http://localhost"),
					factory.createSetting(sysList[1], S2Import.DefaultPrefix, S2Import.PersonImport),
					factory.createSetting(sysList[1], S2Import.DefaultPrefix, S2Import.ServiceAccount, "asdfasasdfasd"),
					factory.createSetting(sysList[1], S2Import.DefaultPrefix, S2Import.ServicePassword),
					factory.createSetting(sysList[1], S2Import.DefaultPrefix, S2Import.Repeat),
					factory.createSetting(sysList[1], S2Import.DefaultPrefix, S2Import.RepeatInterval, "3"),
					factory.createSetting(sysList[1], S2Import.DefaultPrefix, S2Import.LastAccessed),

					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.Available),
					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.Repeat),
					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.RepeatInterval),
					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.ServiceAddress, "http://10.1.1.234/goforms/nbapi"),
					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.ServiceAccount, "admin"),
					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.ServicePassword, "072159245241245031239120017047219193126250124056"),
					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.ImageImport),
					factory.createSetting(sysList[1], S2PeopleImport.DefaultPrefix, S2PeopleImport.FieldsToUpdate, "FirstName,MiddleName,LastName,ExternalUpdated,Image,"
						+ "udf1,udf2,udf3,udf4,udf5,udf6,udf7,udf8,udf9,udf10,udf11,udf12,udf13,udf14,udf15,udf16,udf17,udf18,udf19,udf20"),

					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.Available),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.Repeat),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.RepeatInterval),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.ServiceAddress, "http://localhost"),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.Account),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.Password),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.SourceSystem, ""),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.PersonExport),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.LocationExport),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.AccessExport),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.LastAccessEvent),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.LocationId),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.EventCode, ""),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.SysId, ""),
					factory.createSetting(sysList[2], TrackExport.DefaultPrefix, TrackExport.DataSource, ""),

					factory.createSetting(sysList[5], LubrizolImport.DefaultPrefix, LubrizolImport.Available),
					factory.createSetting(sysList[5], LubrizolImport.DefaultPrefix, LubrizolImport.Repeat),
					factory.createSetting(sysList[5], LubrizolImport.DefaultPrefix, LubrizolImport.RepeatInterval),
					factory.createSetting(sysList[5], LubrizolImport.DefaultPrefix, LubrizolImport.SqlConnection),

					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.Available),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.Repeat),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.RepeatInterval),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.ServiceAddress),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.Account),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.Password),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.LastUpdated),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.SourceSystem),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.ActiveEmployeeLibrary),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.InactiveEmployeeLibrary),
					factory.createSetting(sysList[6], LubrizolExport.DefaultPrefix, LubrizolExport.SqlConnection),
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
