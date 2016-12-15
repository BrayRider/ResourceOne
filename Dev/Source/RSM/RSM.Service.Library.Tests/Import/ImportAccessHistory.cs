using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Artifacts;
using RSMDB = RSM.Support;
using EntityType = RSM.Service.Library.Model.EntityType;
using StageFactory = RSM.Staging.Library.Factory;
using EntityAction = RSM.Staging.Library.EntityAction;
using StageData = RSM.Staging.Library.Data;

namespace RSM.Service.Library.Tests.Import
{
	[TestClass]
	public class ImportAccessHistory : Test
	{
		[TestMethod]
		public void S2ImportAccessHistory_CreateTask()
		{
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var setting = context.Settings.First(x => x.Id == StageData.Settings.S2ImportRepeat.Id);
				setting.Value = "false";

				setting = context.Settings.First(x => x.Id == StageData.Settings.S2ImportRepeatInterval.Id);
				setting.Value = "30";

				context.SubmitChanges();
			}

			Task task = null;
			try
			{
				task = Task.Create("S2Import", new ServiceProfile());
			}
			catch (Exception e)
			{
				Assert.Fail("Create task failed! {0}", e.ToString());
			}
			Assert.IsNotNull(task, "Missing task object");
			Assert.IsTrue(task.ActivityName == "AccessHistory", "Wrong activity name {0}", task.ActivityName);
			Assert.IsTrue(task.Name == "S2Import", "Wrong task name {0}", task.Name);
			Assert.IsTrue(!task.Profile.Schedule.Repeat, "Repeat should be false");
			Assert.IsTrue(task.Profile.Schedule.RepeatInterval.TotalMinutes == 30, "Wrong repeat interval {0}", task.Profile.Schedule.RepeatInterval.TotalMinutes);

		}

		[TestMethod]
		public void S2ImportAccessHistory_Execute()
		{
			var taskName = "S2ImportAccessAPIStub";

			var lastLogId = 0; 
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				LoadS2ImportTestData(context, taskName);

				lastLogId = context.LogEntries.Any() ? context.LogEntries.Max(x => x.ID) : 0;
			}

			Task task = null;
			Result<string> result = Result<string>.Success();

			try
			{
                task = Task.Create(taskName, new ServiceProfile(), "S2Import");

				var import = task as RSM.Integration.S2.Import.AccessHistory;
				result = import.Execute(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var histories = context.AccessHistories;

                // 1 for Mustang Filter, 1 for S & B Filter
                Assert.IsTrue(histories.Count() == 2, "Incorrect number of access logs imported.");

				var logs = context.LogEntries.Where(x => x.ID > lastLogId);

				Assert.IsTrue(logs.Any(x => x.Message.Contains("BadPerson")), "Invalid person logged.");
				Assert.IsTrue(logs.Any(x => x.Message.Contains("BadPortal")), "Invalid portal logged.");
				Assert.IsTrue(logs.Any(x => x.Message.Contains("BadReader")), "Invalid reader logged.");
			}
		}

		[TestMethod]
		public void S2ImportAccessHistory_ExecuteWrapper()
		{
			var taskName = "S2ImportAccessWrapper";
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				LoadS2ImportTestData(context, taskName);
			}

			Task task = null;
			try
			{
				task = Task.Create(taskName, new ServiceProfile());

				var import = task as S2ImportTaskWrapper;
				import.TestWrapper(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}
		}

		#region Helpers

		public static void LoadS2ImportTestData(RSMDB.RSMDataModelDataContext context, string prefix)
		{
			var s2 = context.ExternalSystems.FirstOrDefault(x => x.Id == S2In.Id);

			var factory = new StageFactory(context);

            //factory.createSetting(1001, string.Format("{0}.Repeat", prefix), "Allow S2 import task to repeat.", "true", 0, false, InputTypes.Checkbox, s2);
            //factory.createSetting(1002, string.Format("{0}.RepeatInterval", prefix), "S2 import repeat interval in minutes.", "3", 0, false, InputTypes.Text, s2);
            //factory.createSetting(1003, string.Format("{0}.LastAccessEvent", prefix), "Date time on last S2 record imported.", "", 0, false, InputTypes.Text, s2);
            //factory.createSetting(1004, string.Format("{0}.PersonImport", prefix), "Allow importing of People from S2.", "true", 0, false, InputTypes.Checkbox, s2);
            //factory.createSetting(1005, string.Format("{0}.ServiceAddress", prefix), "Appliance Address", "http://localhost", 2, true, InputTypes.Text, s2);
            //factory.createSetting(1006, string.Format("{0}.ServiceAccount", prefix), "S2 Service User Id", "asdfasasdfasd", 3, true, InputTypes.Text, s2);
            //factory.createSetting(1007, string.Format("{0}.ServicePassword", prefix), "S2 Service Password", "admin", 4, true, InputTypes.Password, s2);
            //factory.createSetting(1007, string.Format("{0}.ServicePassword", prefix), "S2 Service Password", "admin", 4, true, InputTypes.Password, s2);

			var location = factory.createLocation(name: "Location1", id: 1, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Location, "Location1", s2.Id, location.LocationID);

			var portal = factory.createPortal("Portal1", location.LocationID, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Portal, "Portal1", s2.Id, portal.Id);

			var reader = factory.createReader("Reader1", portal.Id, action: EntityAction.InsertAndSubmit);
			factory.createExternalApplicationKey(EntityType.Reader, "Reader1", s2.Id, reader.Id);

			context.SubmitChanges();
		}
		#endregion

	}
}
