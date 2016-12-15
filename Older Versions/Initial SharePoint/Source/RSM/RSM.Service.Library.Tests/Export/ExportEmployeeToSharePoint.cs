using System;
using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSM.Service.Library.Extensions;
using RSM.Service.Library.Model;
using RSMModel = RSM.Service.Library.Model;
using RSMDB = RSM.Support;
using StageData = RSM.Staging.Library.Data;

using EntityType = RSM.Service.Library.Model.EntityType;
using StageFactory = RSM.Staging.Library.Factory;
using LubrizolExport = RSM.Staging.Library.Data.Settings.LubrizolExport;

using R1Employee = RSM.Integration.Lubrizol.Model.Lubrizol_Employee;

using LubrizolData = RSM.Integration.Lubrizol.Model.RSMLubrizolDataModelDataContext;
using ExternalDataContext = RSM.Integration.Lubrizol.Model.LubrizolDataModelDataContext;

namespace RSM.Service.Library.Tests.Export
{
	[TestClass]
	public class ExportEmployeeToSharePoint : Test
	{
		[TestMethod]
		public void CreateTask()
		{
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				var name = string.Format("{0}.{1}", LubrizolExport.DefaultPrefix, LubrizolExport.Repeat.Name);
				var setting = context.Settings.First(x => x.Name == name);
				setting.Value = "false";

				name = string.Format("{0}.{1}", LubrizolExport.DefaultPrefix, LubrizolExport.RepeatInterval.Name);
				setting = context.Settings.First(x => x.Name == name);
				setting.Value = "30";

				context.SubmitChanges();
			}

			Task task = null;
			try
			{
				task = Task.Create("LubrizolOut", new ServiceProfile());
			}
			catch (Exception e)
			{
				Assert.Fail("Create task failed! {0}", e.ToString());
			}
			Assert.IsNotNull(task, "Missing task object");
			Assert.IsTrue(task.ActivityName == "LubrizolExport", "Wrong activity name {0}", task.ActivityName);
			Assert.IsTrue(task.Name == "LubrizolOut", "Wrong task name {0}", task.Name);
			Assert.IsTrue(!task.Profile.Schedule.Repeat, "Repeat should be false");
			Assert.IsTrue(task.Profile.Schedule.RepeatInterval.TotalMinutes == 30, "Wrong repeat interval {0}", task.Profile.Schedule.RepeatInterval.TotalMinutes);
		}

		[TestMethod]
		public void Execute()
		{
			//var taskName = "LubrizolImportAPIStub";

			//RSMModel.Person modelPerson;
			//using (var context = new RSMDB.RSMDataModelDataContext())
			//{
			//    //Load settings.
			//    //Includes a field level filter on 
			//    LoadLubrizolImportTestData(context, taskName);

			//    //Create an existing person that will be updated
			//    var person = StageFactory.CreatePerson("OrigFirst2", "OrigLast", "OrigMid");
			//    context.Persons.InsertOnSubmit(person);
			//    context.SubmitChanges();
			//    modelPerson = person.ToModel();

			//    var keys = StageFactory.CreateExternalApplicationKey(EntityType.Person, "2", S2In.Id, person.Id);
			//    context.ExternalApplicationKeys.InsertOnSubmit(keys);
			//    context.SubmitChanges();
			//    keys.ToModel(modelPerson);
			//}

			//Task task = null;
			//Result<string> result = Result<string>.Success();

			//try
			//{
			//    task = Task.Create(taskName, new ServiceProfile());

			//    var import = task as RSM.Integration.S2.Import.People;
			//    result = import.Execute(null);
			//}
			//catch (Exception e)
			//{
			//    Assert.Fail("Test exception! {0}", e.ToString());
			//}

			//Assert.IsNotNull(result, "Missing results");
			//Assert.IsTrue(result.Succeeded, result.ToString());
			//using (var context = new RSMDB.RSMDataModelDataContext())
			//{
			//    var newPerson = context.Persons.FirstOrDefault(x => x.PersonID == modelPerson.InternalId);
			//    Assert.IsTrue(newPerson.MiddleName == modelPerson.MiddleName, "Middle name updated but not in field list to update.");
			//    Assert.IsTrue(newPerson.FirstName != modelPerson.FirstName, "First name should have been updated.");
			//    Assert.IsTrue(newPerson.Image != modelPerson.Image, "Image should have been updated.");

			//    var newKeys = context.ExternalApplicationKeys.FirstOrDefault(x => x.ExternalId == modelPerson.ExternalId && x.InternalId == modelPerson.InternalId && x.SystemId == modelPerson.ExternalSystemId && x.EntityType == Enum.GetName(typeof(EntityType), EntityType.Person));
			//    Assert.IsTrue(newKeys.ExternalEntityLastUpdated != null, "ExternalEntityLastUpdated should have been updated.");

			//    Assert.IsTrue( context.Persons.FirstOrDefault(x => x.UDF4 == null || x.UDF4.Length == 0) == null, "There should be no records with an empty UDF4 field due to field filter.");
			//}
		}

		[TestMethod]
		public void Execute_NoStub()
		{
			const string taskName = "LubrizolExport";
			const string employee1 = "_1";
			const string employee2 = "_2";

			var result = Result<string>.Success();

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				LoadLubrizolImportTestData(context, taskName);

				CreateTestPerson(context, StageData.People.R1Person1.FirstName, StageData.People.R1Person1.MiddleName, StageData.People.R1Person1.LastName, employee1, S2In, Properties.Resources.BadPiggies);
				CreateTestPerson(context, StageData.People.R1Person2.FirstName, StageData.People.R1Person2.MiddleName, StageData.People.R1Person2.LastName, employee2, S2In, Properties.Resources.AngryBirdsRed);
			}

			using (var context = new LubrizolData())
			{
				CreateTestEmployee(context, StageData.People.R1Person1.FirstName, StageData.People.R1Person1.MiddleName, StageData.People.R1Person1.LastName, employee1);
				CreateTestEmployee(context, StageData.People.R1Person2.FirstName, StageData.People.R1Person2.MiddleName, StageData.People.R1Person2.LastName, employee2);
			}

			try
			{
				var task = Task.Create(taskName, new ServiceProfile());

				var import = task as Integration.Lubrizol.Export.People;

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

		public Person CreateTestPerson(RSMDB.RSMDataModelDataContext context, string firstName, string middleName, string lastName, string employeeId, ExternalSystem system, Bitmap imageFile = null)
		{
			var person = StageFactory.CreatePerson(firstName, lastName, middleName);

			if (imageFile != null)
			{
				var ms = new MemoryStream();
				imageFile.Save(ms, ImageFormat.Jpeg);
				person.Image = ms.ToArray();
			}

			context.Persons.InsertOnSubmit(person);
			context.SubmitChanges();
			var modelPerson = person.ToModel();

			var keys = StageFactory.CreateExternalApplicationKey(EntityType.Person, employeeId, system.Id, person.Id);
			context.ExternalApplicationKeys.InsertOnSubmit(keys);
			context.SubmitChanges();
			keys.ToModel(modelPerson);

			return modelPerson;
		}

		public R1Employee CreateTestEmployee(LubrizolData context, string firstName, string middleName, string lastName, string employeeId, string status = "Active")
		{
			var employee = new R1Employee
			               	{
			               		FirstName = firstName,
			               		MiddleName = middleName,
			               		LastName = lastName,
			               		EmployeeID = employeeId,
			               		Name = string.Format("{0} {1}", firstName, lastName),
			               		Initials = string.Format("{0}{1}{2}", string.IsNullOrWhiteSpace(firstName) ? "" : firstName.Substring(0,1)
													, string.IsNullOrWhiteSpace(middleName) ? "" : middleName.Substring(0,1)
													, string.IsNullOrWhiteSpace(lastName) ? "" : lastName.Substring(0,1)),
								LastLoadDate = DateTime.Now,
								LastUpdated = DateTime.Now,
								EmployeeStatus = status == "Active" ? 'A' : 'I',
								EmployeeStatusDesc = status,
								Division = Guid.NewGuid().ToString()
			               	};

			context.Lubrizol_Employees.InsertOnSubmit(employee);
			context.SubmitChanges();

			return employee;
		}

		public static void LoadLubrizolImportTestData(RSMDB.RSMDataModelDataContext context, string prefix)
		{
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
