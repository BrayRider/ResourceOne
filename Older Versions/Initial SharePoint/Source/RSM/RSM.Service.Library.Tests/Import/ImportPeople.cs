using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Artifacts;
using RSM.Service.Library.Extensions;
using RSM.Service.Library.Model.Reflection;
using RSMModel = RSM.Service.Library.Model;
using RSMDB = RSM.Support;
using StageData = RSM.Staging.Library.Data;

using EntityType = RSM.Service.Library.Model.EntityType;
using StageFactory = RSM.Staging.Library.Factory;
using EntityAction = RSM.Staging.Library.EntityAction;
using S2PeopleImport = RSM.Staging.Library.Data.Settings.S2PeopleImport;
using S2Import = RSM.Staging.Library.Data.Settings.S2Import;

namespace RSM.Service.Library.Tests.Import
{
	[TestClass]
	public class ImportPeople : Test
	{
		[TestMethod]
		public void S2PeopleImport_CreateTask()
		{
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var name = string.Format("{0}.{1}", S2PeopleImport.DefaultPrefix, S2PeopleImport.Repeat.Name);
				var setting = context.Settings.First(x => x.Name == name);
				setting.Value = "false";

				name = string.Format("{0}.{1}", S2PeopleImport.DefaultPrefix, S2PeopleImport.RepeatInterval.Name);
				setting = context.Settings.First(x => x.Name == name);
				setting.Value = "30";

				context.SubmitChanges();
			}

			Task task = null;
			try
			{
				task = Task.Create("S2PeopleImport", new ServiceProfile());
			}
			catch (Exception e)
			{
				Assert.Fail("Create task failed! {0}", e.ToString());
			}
			Assert.IsNotNull(task, "Missing task object");
			Assert.IsTrue(task.ActivityName == "PeopleImport", "Wrong activity name {0}", task.ActivityName);
			Assert.IsTrue(task.Name == "S2PeopleImport", "Wrong task name {0}", task.Name);
			Assert.IsTrue(!task.Profile.Schedule.Repeat, "Repeat should be false");
			Assert.IsTrue(task.Profile.Schedule.RepeatInterval.TotalMinutes == 30, "Wrong repeat interval {0}", task.Profile.Schedule.RepeatInterval.TotalMinutes);

		}

		[TestMethod]
		public void S2PeopleImport_Execute()
		{
			var taskName = "S2PeopleImportAPIStub";

			RSMModel.Person modelPerson;
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				//Load settings.
				//Includes a field level filter on 
				LoadS2ImportTestData(context, taskName);

				//Create an existing person that will be updated
				var person = StageFactory.CreatePerson("OrigFirst2", "OrigLast", "OrigMid");
				context.Persons.InsertOnSubmit(person);
				context.SubmitChanges();
				modelPerson = person.ToModel();

				var keys = StageFactory.CreateExternalApplicationKey(EntityType.Person, "2", S2In.Id, person.Id);
				context.ExternalApplicationKeys.InsertOnSubmit(keys);
				context.SubmitChanges();
				keys.ToModel(modelPerson);
			}

			Task task = null;
			Result<string> result = Result<string>.Success();

			try
			{
				task = Task.Create(taskName, new ServiceProfile());

				var import = task as RSM.Integration.S2.Import.People;
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
				var newPerson = context.Persons.FirstOrDefault(x => x.PersonID == modelPerson.InternalId);
				Assert.IsTrue(newPerson.MiddleName == modelPerson.MiddleName, "Middle name updated but not in field list to update.");
				Assert.IsTrue(newPerson.FirstName != modelPerson.FirstName, "First name should have been updated.");
				Assert.IsTrue(newPerson.Image != modelPerson.Image, "Image should have been updated.");

				var newKeys = context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == modelPerson.ExternalId && x.InternalId == modelPerson.InternalId && x.SystemId == modelPerson.ExternalSystemId && x.EntityType == Enum.GetName(typeof(EntityType), EntityType.Person));
				Assert.IsTrue(newKeys.ExternalEntityLastUpdated != null, "ExternalEntityLastUpdated should have been updated.");

				Assert.IsTrue( context.Persons.FirstOrDefault(x => x.UDF4 == null || x.UDF4.Length == 0) == null, "There should be no records with an empty UDF4 field due to field filter.");
			}
		}

		[TestMethod]
		public void S2PeopleImport_Execute_NoStub()
		{
			var taskName = "S2PeopleImport";

			Task task = null;
			Result<string> result = Result<string>.Success();

			try
			{
				task = Task.Create(taskName, new ServiceProfile(), "S2PeopleImport");

				var import = task as RSM.Integration.S2.Import.People;
				result = import.Execute(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
		}

		#region Helpers

		public static void LoadS2ImportTestData(RSMDB.RSMDataModelDataContext context, string prefix)
		{
			var s2 = context.ExternalSystems.FirstOrDefault(x => x.Id == S2In.Id);

			var factory = new StageFactory(context);

			factory.createSetting(s2, prefix, S2PeopleImport.Available);
			factory.createSetting(s2, prefix, S2PeopleImport.Repeat);
			factory.createSetting(s2, prefix, S2PeopleImport.RepeatInterval);
			factory.createSetting(s2, prefix, S2PeopleImport.ServiceAccount);
			factory.createSetting(s2, prefix, S2PeopleImport.ServicePassword);
			factory.createSetting(s2, prefix, S2PeopleImport.ImageImport);
			factory.createSetting(s2, prefix, S2PeopleImport.ServiceAddress, value: "http://localhost:8766/RSM.Integration.S2.Stub/RestService.svc/test");

			//Note: MiddleName intentionally left out for tests
			factory.createSetting(s2, prefix, S2PeopleImport.FieldsToUpdate, value: "FirstName,LastName,ExternalUpdated,Image,"
				+ "udf1,udf2,udf3,udf4,udf5,udf6,udf7,udf8,udf9,udf10,udf11,udf12,udf13,udf14,udf15,udf16,udf17,udf18,udf19,udf20");

			//Field filter to exclude unwanted person records
			factory.createSetting("Filter.Person.udf4" , "Field filter that excludes records with an empty udf4 value.", ".+", prefix, viewable: false, system: s2);

			context.SubmitChanges();
		}
		#endregion

	}
}
