using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Artifacts;
using RSMDB = RSM.Support;
using StageData = RSM.Staging.Library.Data;

using EntityType = RSM.Service.Library.Model.EntityType;
using StageFactory = RSM.Staging.Library.Factory;
using EntityAction = RSM.Staging.Library.EntityAction;
using S2Import = RSM.Staging.Library.Data.Settings.S2Import;

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
				var name = string.Format("{0}.{1}", S2Import.DefaultPrefix, S2Import.Repeat.Name);
				var setting = context.Settings.First(x => x.Name == name);
				setting.Value = "false";

				name = string.Format("{0}.{1}", S2Import.DefaultPrefix, S2Import.RepeatInterval.Name);
				setting = context.Settings.First(x => x.Name == name);
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
				task = Task.Create(taskName, new ServiceProfile());

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
				Assert.IsTrue(histories.Count() == 1, "Incorrect number of access logs imported.");

				var logs = context.LogEntries.Where(x => x.ID > lastLogId);

				Assert.IsTrue(logs.Any(x => x.Message.Contains("It has an invalid ID")), "Invalid access history not logged.");
				Assert.IsTrue(logs.Any(x => x.Message.Contains("BadPerson")), "Invalid person not logged.");
				Assert.IsTrue(logs.Any(x => x.Message.Contains("BadPortal")), "Invalid portal not logged.");
				Assert.IsTrue(logs.Any(x => x.Message.Contains("BadReader")), "Invalid reader not logged.");
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

			factory.createSetting(s2, prefix, S2Import.Repeat);
			factory.createSetting(s2, prefix, S2Import.RepeatInterval, "3");
			factory.createSetting(s2, prefix, S2Import.LastAccessed);
			factory.createSetting(s2, prefix, S2Import.PersonImport, "true");
			factory.createSetting(s2, prefix, S2Import.ServiceAddress, "http://localhost");
			factory.createSetting(s2, prefix, S2Import.ServiceAccount, "asdfasasdfasd");
			factory.createSetting(s2, prefix, S2Import.ServicePassword, "admin");

			var location = factory.createLocation("Location1", action: EntityAction.InsertAndSubmit);
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
