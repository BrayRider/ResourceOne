using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Artifacts;
using RSMDB = RSM.Support;

using StageFactory = RSM.Staging.Library.Factory;
using EntityType = RSM.Service.Library.Model.EntityType;
using EntityAction = RSM.Staging.Library.EntityAction;
using StageData = RSM.Staging.Library.Data;

using AccessType = RSM.Integration.S2.AccessType;
using TrackExport = RSM.Staging.Library.Data.Settings.TrackExport;

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
				var name = string.Format("{0}.{1}", TrackExport.DefaultPrefix, TrackExport.Repeat.Name);
				var setting = context.Settings.First(x => x.Name == name);
				setting.Value = "true";

				name = string.Format("{0}.{1}", TrackExport.DefaultPrefix, TrackExport.RepeatInterval.Name);
				setting = context.Settings.First(x => x.Name == name);
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
			factory.createSetting(trackOut, prefix, TrackExport.Repeat);
			factory.createSetting(trackOut, prefix, TrackExport.RepeatInterval);
			factory.createSetting(trackOut, prefix, TrackExport.LastAccessEvent);
			factory.createSetting(trackOut, prefix, TrackExport.PersonExport);
			factory.createSetting(trackOut, prefix, TrackExport.ServiceAddress, "http://localhost:8088/mockACS2TrackWebSvcSoap12");
			factory.createSetting(trackOut, prefix, TrackExport.Account);
			factory.createSetting(trackOut, prefix, TrackExport.Password);
			factory.createSetting(trackOut, prefix, TrackExport.SourceSystem, s2.Id.ToString());

			factory.createSetting(trackOut, prefix, TrackExport.LocationExport);
			factory.createSetting(trackOut, prefix, TrackExport.AccessExport);
			factory.createSetting(trackOut, prefix, TrackExport.EventCode);
			factory.createSetting(trackOut, prefix, TrackExport.SysId);
			factory.createSetting(trackOut, prefix, TrackExport.DataSource);
			factory.createSetting(trackOut, prefix, TrackExport.CompanyExport);

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
