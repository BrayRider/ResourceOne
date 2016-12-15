using System;
using System.Data.Linq;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Service.Library.Extensions;
using RSM.Service.Library.Model;
using RSMModel = RSM.Service.Library.Model;
using RSMDB = RSM.Support;
using StageData = RSM.Staging.Library.Data;

using EntityType = RSM.Service.Library.Model.EntityType;
using StageFactory = RSM.Staging.Library.Factory;
using LubrizolImport = RSM.Staging.Library.Data.Settings.LubrizolImport;

using LubrizolData = RSM.Integration.Lubrizol.Model.RSMLubrizolDataModelDataContext;
using ExternalDataContext = RSM.Integration.Lubrizol.Model.LubrizolDataModelDataContext;

namespace RSM.Service.Library.Tests.Import
{
	[TestClass]
	public class ImportEmployee : Test
	{
		[TestMethod]
		public void LubrizolEmployeeImport_CreateTask()
		{
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var name = string.Format("{0}.{1}", LubrizolImport.DefaultPrefix, LubrizolImport.Repeat.Name);
				var setting = context.Settings.First(x => x.Name == name);
				setting.Value = "false";

				name = string.Format("{0}.{1}", LubrizolImport.DefaultPrefix, LubrizolImport.RepeatInterval.Name);
				setting = context.Settings.First(x => x.Name == name);
				setting.Value = "30";

				context.SubmitChanges();
			}

			Task task = null;
			try
			{
				task = Task.Create("LubrizolIn", new ServiceProfile());
			}
			catch (Exception e)
			{
				Assert.Fail("Create task failed! {0}", e.ToString());
			}
			Assert.IsNotNull(task, "Missing task object");
			Assert.IsTrue(task.ActivityName == "LubrizolImport", "Wrong activity name {0}", task.ActivityName);
			Assert.IsTrue(task.Name == "LubrizolIn", "Wrong task name {0}", task.Name);
			Assert.IsTrue(!task.Profile.Schedule.Repeat, "Repeat should be false");
			Assert.IsTrue(task.Profile.Schedule.RepeatInterval.TotalMinutes == 30, "Wrong repeat interval {0}", task.Profile.Schedule.RepeatInterval.TotalMinutes);

		}

		[TestMethod]
		public void LubrizolEmployeeImport_Execute()
		{
			var taskName = "LubrizolImportAPIStub";

			RSMModel.Person modelPerson;
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				//Load settings.
				//Includes a field level filter on 
				LoadLubrizolImportTestData(context, taskName);

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
		public void LubrizolEmployeeImport_Execute_NoStub()
		{
			const string taskName = "LubrizolImport";

			var result = Result<string>.Success();

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				//Load settings.
				//Includes a field level filter on 
				LoadLubrizolImportTestData(context, taskName);

				CreateTestPerson(context, "System", "", "Administrator", "_1", S2In);

				CreateTestPerson(context, "Chris", "", "Milum", "_2", S2In);
			}

			try
			{
				var task = Task.Create(taskName, new ServiceProfile());

				var import = task as Integration.Lubrizol.Employees;

				result = import.Execute(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());
		}

		[TestMethod]
		public void LubrizolEmployeeImportUpdate_Execute_NoStub()
		{
			const string taskName = "LubrizolImport";
			const string employee1 = "_1";
			const string employee2 = "_2";

			var result = Result<string>.Success();

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				//Load settings.
				//Includes a field level filter on 
				LoadLubrizolImportTestData(context, taskName);

				CreateTestPerson(context, StageData.People.R1Person1.FirstName, StageData.People.R1Person1.MiddleName, StageData.People.R1Person1.LastName, employee1, S2In);
				CreateTestPerson(context, StageData.People.R1Person2.FirstName, StageData.People.R1Person2.MiddleName, StageData.People.R1Person2.LastName, employee2, S2In);
			}

			try
			{
				var task = Task.Create(taskName, new ServiceProfile());

				var import = task as Integration.Lubrizol.Employees;

				result = import.Execute(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());

			DateTime empl1Date;
			DateTime empl2Date;
			using (var context = new LubrizolData())
			{
				empl1Date = context.Lubrizol_Employees.First(x => x.EmployeeID == employee1).LastUpdated;
				empl2Date = context.Lubrizol_Employees.First(x => x.EmployeeID == employee2).LastUpdated;

				var employee = context.Lubrizol_Employees.First(x => x.EmployeeID == employee1);
				employee.Division = Guid.NewGuid().ToString();

				context.SubmitChanges();
			}

			Thread.SpinWait(100);

			try
			{
				var task = Task.Create(taskName, new ServiceProfile());

				var import = task as Integration.Lubrizol.Employees;

				result = import.Execute(null);
			}
			catch (Exception e)
			{
				Assert.Fail("Test exception! {0}", e.ToString());
			}

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, result.ToString());

			using (var context = new LubrizolData())
			{
				Assert.AreNotEqual(context.Lubrizol_Employees.First(x => x.EmployeeID == employee1).LastUpdated, empl1Date, "Update date not changed");
				Assert.AreEqual(context.Lubrizol_Employees.First(x => x.EmployeeID == employee2).LastUpdated, empl2Date, "Update date changed");
			}

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var internalKey = context.ExternalApplicationKeys.First(x => x.EntityType == "Person" && x.ExternalId == employee1).InternalId;
				var person = context.Persons.First(x => x.PersonID == internalKey);
				Assert.AreNotEqual(person.LastUpdated, empl1Date, "Update date not changed");

				internalKey = context.ExternalApplicationKeys.First(x => x.EntityType == "Person" && x.ExternalId == employee2).InternalId;
				person = context.Persons.First(x => x.PersonID == internalKey);
				Assert.AreEqual(person.LastUpdated, empl2Date, "Update date changed");
			}

		}

		#region Helpers

		public Person CreateTestPerson(RSMDB.RSMDataModelDataContext context, string firstName, string middleName, string lastName, string employeeId, ExternalSystem system)
		{
			var person = StageFactory.CreatePerson(firstName, lastName, middleName);
			context.Persons.InsertOnSubmit(person);
			context.SubmitChanges();
			var modelPerson = person.ToModel();

			var keys = StageFactory.CreateExternalApplicationKey(EntityType.Person, employeeId, system.Id, person.Id);
			context.ExternalApplicationKeys.InsertOnSubmit(keys);
			context.SubmitChanges();
			keys.ToModel(modelPerson);

			return modelPerson;
		}

		public static void LoadLubrizolImportTestData(RSMDB.RSMDataModelDataContext context, string prefix)
		{
		}

		private void ChangeExternalData(ExternalDataContext context, string employeeId)
		{
			var employee = context.tblzILMDatas.First(x => x.EmployeeID == employeeId);
			employee.Division = DateTime.Now.ToLongDateString();
		}
		#endregion

		[TestCleanup]
		public override void Cleanup()
		{
			using (var context = new LubrizolData())
			{
				context.Lubrizol_Employees.DeleteAllOnSubmit(context.Lubrizol_Employees);
				context.SubmitChanges(ConflictMode.FailOnFirstConflict);
			}

			base.Cleanup();
		}
	}
}
