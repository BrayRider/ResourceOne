using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using RSM.Service.Library;

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

namespace TestSupport
{
	public partial class ServiceTester : Form
	{
		public static ExternalSystem S2In = ExternalSystem.S2In;
		public static ExternalSystem TrackOut = ExternalSystem.TrackOut;

		public ServiceTester()
		{
			InitializeComponent();
		}

		#region Helper Buttons
		private void ClearButtonClick(object sender, EventArgs e)
		{
			txtOutput.Text = "";
			txtOutput.Refresh();
		}

		private void CloseButtonClick(object sender, EventArgs e)
		{
			Close();
		}

		private void ExitAllButtonClick(object sender, EventArgs e)
		{
			Application.Exit();
		}
		#endregion

		private void CallLubrizolExport_Click(object sender, EventArgs e)
		{
			const string taskName = "LubrizolExport";
			const string employee1 = "TEST_1";
			const string employee2 = "TEST_2";

			Configuration.SaveConfigValue("ServiceId", "6");
			Configuration.SaveConfigValue("ServiceName", "R1SM.LubrizolExport");
			Configuration.SaveConfigValue("Description", "Performs export operations for R1SM's SharePoint integration.");
			Configuration.SaveConfigValue("Task1", "LubrizolExport");

			var result = Result<string>.Success();
			
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				CreateTestPerson(context, StageData.People.R1Person1.FirstName, StageData.People.R1Person1.MiddleName, StageData.People.R1Person1.LastName, employee1, S2In, RSM.Service.Library.Tests.Properties.Resources.BadPiggies);
				CreateTestPerson(context, StageData.People.R1Person2.FirstName, StageData.People.R1Person2.MiddleName, StageData.People.R1Person2.LastName, employee2, S2In, RSM.Service.Library.Tests.Properties.Resources.AngryBirdsRed);
			}

			using (var context = new LubrizolData())
			{
				CreateTestEmployee(context, StageData.People.R1Person1.FirstName, StageData.People.R1Person1.MiddleName, StageData.People.R1Person1.LastName, employee1, "Active");
				CreateTestEmployee(context, StageData.People.R1Person2.FirstName, StageData.People.R1Person2.MiddleName, StageData.People.R1Person2.LastName, employee2, "Inactive");
			}

			try
			{
				var task = Task.Create(taskName, new ServiceProfile());

				var import = task as RSM.Integration.Lubrizol.Export.People;

				result = import.Execute(null);

				txtOutput.Text += Environment.NewLine + string.Format(result.Message);
			}
			catch (Exception ex)
			{
				txtOutput.Text += Environment.NewLine + string.Format("Test exception! {0}", ex.ToString());
			}

			// Clean out test data
			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				DeleteTestPerson(context, employee1, S2In);
				DeleteTestPerson(context, employee2, S2In);
			}

			using (var context = new LubrizolData())
			{
				DeleteTestEmployee(context, employee1);
				DeleteTestEmployee(context, employee2);
			}

		}

		private void CallS2Import_Click(object sender, EventArgs e)
		{
			const string taskName = "S2ToSharePoint.S2Import";

			Configuration.SaveConfigValue("ServiceId", "2");
			Configuration.SaveConfigValue("ServiceName", "R1SM.S2ToSharePoint.S2Import");
			Configuration.SaveConfigValue("Description", "Performs import operations for R1SM's S2 integration.");
			Configuration.SaveConfigValue("Task1", taskName);

			try
			{
				var task = Task.Create(taskName, new ServiceProfile());

				var import = task as RSM.Integration.S2.Import.People;

				var result = import.Execute(null);

				txtOutput.Text += Environment.NewLine + string.Format(result.Message);
			}
			catch (Exception ex)
			{
				txtOutput.Text += Environment.NewLine + string.Format("Test exception! {0}", ex.ToString());
			}
	
		}

		#region Helpers
		public Person CreateTestPerson(RSMDB.RSMDataModelDataContext context, string firstName, string middleName, string lastName, string employeeId, ExternalSystem system, Bitmap imageFile = null)
		{
			var key = context.ExternalApplicationKeys.FirstOrDefault(x => x.EntityType == "Person" && x.ExternalId == employeeId);
			if (key != null)
			{
				DeleteTestPerson(context, employeeId, system);
			}

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
			var employee = context.Lubrizol_Employees.FirstOrDefault(x => x.EmployeeID == employeeId);

			if (employee != null)
			{
				DeleteTestEmployee(context, employeeId);
			}

			employee = new R1Employee
			{
				FirstName = firstName,
				MiddleName = middleName,
				LastName = lastName,
				EmployeeID = employeeId,
				Name = string.Format("{0} {1}", firstName, lastName),
				Initials = string.Format("{0}{1}{2}", string.IsNullOrWhiteSpace(firstName) ? "" : firstName.Substring(0, 1)
									, string.IsNullOrWhiteSpace(middleName) ? "" : middleName.Substring(0, 1)
									, string.IsNullOrWhiteSpace(lastName) ? "" : lastName.Substring(0, 1)),
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

		public void DeleteTestPerson(RSMDB.RSMDataModelDataContext context, string employeeId, ExternalSystem system)
		{
			var key = context.ExternalApplicationKeys.FirstOrDefault(x => x.EntityType == "Person" && x.ExternalId == employeeId);

			if(key == null)
			{
				txtOutput.Text += Environment.NewLine + string.Format("ERROR: EmployeeId ({0}) Not Found in Keys.", employeeId);
				return;
			}
			var person = context.Persons.FirstOrDefault(x => x.PersonID == key.InternalId);

			if (person == null)
			{
				txtOutput.Text += Environment.NewLine + string.Format("ERROR: EmployeeId ({0}) Not Found in Persons.", employeeId);
				return;
			}

			context.ExternalApplicationKeys.DeleteOnSubmit(key);
			context.Persons.DeleteOnSubmit(person);
			context.SubmitChanges();
		}

		public void DeleteTestEmployee(LubrizolData context, string employeeId)
		{
			var employee = context.Lubrizol_Employees.FirstOrDefault(x => x.EmployeeID == employeeId);
			if (employee == null)
			{
				txtOutput.Text += Environment.NewLine + string.Format("ERROR: Employee ({0}) Not Found in Employees.", employeeId);
				return;
			}

			context.Lubrizol_Employees.DeleteOnSubmit(employee);
			context.SubmitChanges();
		}
		#endregion


	}
}
