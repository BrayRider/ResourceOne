using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Artifacts;
using RSMDB = RSM.Support;

using StageFactory = RSM.Staging.Library.Factory;
using EntityType = RSM.Service.Library.Model.EntityType;
using EntityAction = RSM.Staging.Library.EntityAction;
using StageData = RSM.Staging.Library.Data;

using AccessType = RSM.Integration.S2.AccessType;
using System.Collections.Generic;

namespace RSM.Service.Library.Tests.Export
{
	[TestClass]
	public class ExportAccessEvents : Test
	{
		[TestMethod]
		public void TrackExportAccessHistory_CreateTask()
		{
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var setting = context.Settings.First(x => x.Id == StageData.Settings.TrackExportRepeat.Id);
				setting.Value = "true";

				setting = context.Settings.First(x => x.Id == StageData.Settings.TrackExportRepeatInterval.Id);
				setting.Value = "12";

				context.SubmitChanges();
			}

			Task task = null;
			try
			{
				task = Task.Create("TrackExport", new ServiceProfile());
			}
			catch (Exception e)
			{
				Assert.Fail("Create task failed! {0}", e.ToString());
			}
			Assert.IsNotNull(task, "Missing task object");
			Assert.IsTrue(task.ActivityName == "AccessEvents", "Wrong activity name {0}", task.ActivityName);
			Assert.IsTrue(task.Name == "TrackExport", "Wrong task name {0}", task.Name);
			Assert.IsTrue(task.Profile.Schedule.Repeat, "Repeat should be true");
			Assert.IsTrue(task.Profile.Schedule.RepeatInterval.TotalMinutes == 12, "Wrong repeat interval {0}", task.Profile.Schedule.RepeatInterval.TotalMinutes);

		}

		[TestMethod]
		public void TrackExportAccessHistory_Execute_STUB()
		{
			Task task = null;
			Result<string> result = Result<string>.Success();

			var taskName = "TrackExportSTUB";

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				LoadTrackExportTestData(context, taskName);
			}

			try
			{
				task = Task.Create(taskName, new ServiceProfile());
				var export = task as RSM.Integration.Track.Export.AccessEvents;

				result = export.Execute(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
		}

		[TestMethod]
		public void TrackExportAccessHistory_Execute()
		{
			Task task = null;
			Result<string> result = Result<string>.Success();

			var taskName = "TrackExportTest";

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				LoadTrackExportTestData(context, taskName);
			}

			try
			{
				task = Task.Create(taskName, new ServiceProfile());
				var export = task as RSM.Integration.Track.Export.AccessEvents;

				result = export.Execute(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
		}

		public static void LoadTrackExportTestData(RSMDB.RSMDataModelDataContext context, string prefix)
		{
			var s2 = context.ExternalSystems.FirstOrDefault(x => x.Id == S2In.Id);
			var trackOut = context.ExternalSystems.FirstOrDefault(x => x.Id == TrackOut.Id);

			var factory = new StageFactory(context);
			factory.createSetting(1001, string.Format("{0}.Repeat", prefix), "Allow task to repeat.", "true", 0, false, InputTypes.Checkbox, trackOut);
			factory.createSetting(1002, string.Format("{0}.RepeatInterval", prefix), "repeat interval in minutes.", "3", 0, false, InputTypes.Text, trackOut);
			factory.createSetting(1003, string.Format("{0}.LastAccessEvent", prefix), "Date time on last record exported.", "", 0, false, InputTypes.Text, trackOut);
			factory.createSetting(1004, string.Format("{0}.PersonExport", prefix), "Allow export of People.", "true", 0, false, InputTypes.Checkbox, trackOut);
			factory.createSetting(1005, string.Format("{0}.ServiceAddress", prefix), "Appliance Address", "http://localhost:8088/mockACS2TrackWebSvcSoap12", 2, true, InputTypes.Text, trackOut);
			factory.createSetting(1006, string.Format("{0}.ServiceAccount", prefix), "Service User Id", "asdfasasdfasd", 3, true, InputTypes.Text, trackOut);
			factory.createSetting(1007, string.Format("{0}.ServicePassword", prefix), "Service Password", "admin", 4, true, InputTypes.Password, trackOut);
			factory.createSetting(1008, string.Format("{0}.SourceSystem", prefix), "System whose data will be exported to Track.", s2.Id.ToString(), 0, false, InputTypes.Text, trackOut);

			factory.createSetting(1009, string.Format("{0}.LocationExport", prefix), "Allow exporting of Locations to Track", "true", 1, true, InputTypes.Checkbox, trackOut);
			factory.createSetting(10010, string.Format("{0}.AccessExport", prefix), "Allow exporting of Access History to Track", "true", 2, true, InputTypes.Checkbox, trackOut);
			factory.createSetting(10011, string.Format("{0}.EventCode", prefix), "Event Code value for export to Track.", "8", 0, false, InputTypes.Text, trackOut);
			factory.createSetting(10012, string.Format("{0}.SysId", prefix), "System Id value for export to Track.", "1", 0, false, InputTypes.Text, trackOut);
			factory.createSetting(10013, string.Format("{0}.DataSource", prefix), "DataSource value for export to Track.", "TSTLBZDB", 0, false, InputTypes.Text, trackOut);
			factory.createSetting(10014, string.Format("{0}.CompanyExport", prefix), "Allow exporting of Companies to Track", "true", 1, true, InputTypes.Checkbox, trackOut);

			var location = factory.createLocation("Location1", action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Location, "Location1", s2.Id, location.LocationID);
			factory.createExternalApplicationKey(EntityType.Location, "1", trackOut.Id, location.LocationID);

			var portal = factory.createPortal("Portal 1001", location.LocationID, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Portal, "1001", s2.Id, portal.Id);

			var reader = factory.createReader("Reader1", portal.Id, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Reader, "Reader1", s2.Id, reader.Id);

			var person = factory.createPerson("Jane", "Smith", null, UDFs: new Dictionary<int, string> { { 4, "Contractor Co1" } }, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Person, "Person1", s2.Id, person.Id);

			var start = DateTime.Now;
			for (var i = 0; i < 5; i++)
			{
				var extId = string.Format("access{0}", i);
				var access = factory.createAccessHistory(extId, person.Id, portal.Id, reader.Id, (int)AccessType.Valid, accessed: start.Subtract(TimeSpan.FromMinutes(i)), action: EntityAction.InsertAndSubmit);
				factory.createExternalApplicationKey(EntityType.AccessLog, extId, s2.Id, access.Id);
			}
			context.SubmitChanges();
		}

	}
}
