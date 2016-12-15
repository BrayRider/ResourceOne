using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using RSM.Artifacts;
using RSM.Integration.S2;
using RSM.Service.Library;
using RSM.Service.Library.Model;
using RSM.Staging.Library;
using RSM.Staging.Library.Data;
using RSM.Support;
using RSM.Support.S2;
using RSM.Support.SRMC;
using StageFactory = RSM.Staging.Library.Factory;

namespace TestSupport
{
	public partial class Form1 : Form
	{
		#region Properties
		public string S2APIUrl { get; set; }
		public string S2APIUser { get; set; }
		public string S2APIPassword { get; set; }
		#endregion

		#region Constructors
		public Form1()
		{
			InitializeComponent();

			S2APIUrl = ConfigurationManager.AppSettings.Get("S2APIUrl");
			S2APIUser = ConfigurationManager.AppSettings.Get("S2APIUser");
			S2APIPassword = ConfigurationManager.AppSettings.Get("S2APIPassword");
		}
		#endregion

		#region Helpers
		private static string GetUniqueName(string file, string folder)
		{
			var fullFile = Path.Combine(folder, file);

			var x = 1;
			if (!File.Exists(fullFile))
				return fullFile;

			while (x < int.MaxValue)
			{
				fullFile = Path.Combine(folder, file + "." + x.ToString());
				if (!File.Exists(fullFile))
					return fullFile;

				x++;
			}

			// No way in hell this is ever getting hit.
			return null;
		}

		private static void ImportAssociates(string path, bool requireApproval)
		{
			var numFiles = 0;

			var arcFolder = Path.Combine(path, "archive");

			if (!(Directory.Exists(arcFolder)))
			{
				//WriteToEventLog(string.Format("Created archive folder at: {0}", ArcFolder));
				Directory.CreateDirectory(arcFolder);
			}

			var errFolder = Path.Combine(path, "error");

			if (!(Directory.Exists(errFolder)))
			{
				//WriteToEventLog(string.Format("Created error folder at: {0}", ErrFolder));
				Directory.CreateDirectory(errFolder);
			}

			var files = Directory.GetFiles(path);

			var imp = new SRMCImporter();

			foreach (var filename in files)
			{
				try
				{
					var f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
					// If we could open it with exclusive rights then it's done transferring
					f.Close();

					string newFile;
					if (imp.ImportCSV(filename, requireApproval))
					{
						//WriteToEventLog(string.Format("Imported {0}.", filename));
						numFiles++;
						newFile = GetUniqueName(Path.GetFileName(filename), arcFolder);
					}
					else
					{
						//WriteToEventLogError(string.Format("Failed to import {0}.", filename));
						newFile = GetUniqueName(Path.GetFileName(filename), errFolder);
					}

					if (newFile != null)
						File.Move(filename, newFile);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.ToString());
				}
			}
		}

		private static string ConvertStringArrayToString(string[] array)
		{
			var builder = new StringBuilder();

			foreach (var value in array)
			{
				builder.Append(value);
				builder.Append('.');
			}
			return builder.ToString();
		}

		private void ExportAll()
		{
			var api = new S2API(S2APIUrl, S2APIUser, S2APIPassword, false);

			using (var db = new RSMDataModelDataContext())
			{
				foreach (var person in db.Persons)
				{
					api.SavePerson(person);
				}
			}

			txtOutput.Text = "Done exporting";
		}

		private static string IndentXMLString(string xml)
		{
			var outXml = string.Empty;
			var ms = new MemoryStream();
			// Create a XMLTextWriter that will send its output to a memory stream (file)
			var xtw = new XmlTextWriter(ms, Encoding.Unicode);
			var doc = new XmlDocument();

			try
			{
				// Load the unformatted XML text string into an instance 
				// of the XML Document Object Model (DOM)
				doc.LoadXml(xml);

				// Set the formatting property of the XML Text Writer to indented
				// the text writer is where the indenting will be performed
				xtw.Formatting = Formatting.Indented;

				// write dom xml to the xmltextwriter
				doc.WriteContentTo(xtw);
				// Flush the contents of the text writer
				// to the memory stream, which is simply a memory file
				xtw.Flush();

				// set to start of the memory stream (file)
				ms.Seek(0, SeekOrigin.Begin);
				// create a reader to read the contents of 
				// the memory stream (file)
				var sr = new StreamReader(ms);
				// return the formatted string to caller
				return sr.ReadToEnd();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return string.Empty;
			}
		}
		
		private void AssignView(string textArea, Image image = null)
		{
			txtOutput.Text = textArea;

			personPicture.Image = image;
			this.Refresh();
		}
		#endregion

		#region Buttons
		#region From PS
		private void ImportPeopleFromPSClick(object sender, EventArgs e)
		{
			ImportAssociates("C:\\temp", false);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			var imp = new SRMCImporter();

			var count = imp.ImportNewDepts();
			txtOutput.Text = string.Format("Imported {0} departments.", count);
		}

		private void button6_Click(object sender, EventArgs e)
		{
			var imp = new SRMCImporter();

			var count = imp.ImportNewLocations();
			txtOutput.Text = string.Format("Imported {0} locations.", count);
		}

		private void ImportJobsFromPSClick(object sender, EventArgs e)
		{
			var imp = new SRMCImporter();

			var count = imp.ImportNewJobs();
			txtOutput.Text = string.Format("Imported {0} jobs.", count);
		}

		#endregion

		#region From S2
		private void GetLevelIDsFromS2Click(object sender, EventArgs e)
		{
			var api = new S2API(S2APIUrl, S2APIUser, S2APIPassword, false);

			var text = ConvertStringArrayToString(api.GetLevelIDS());

			AssignView(text, null);
		}

		private void GetLevelOneFromS2Click(object sender, EventArgs e)
		{
			var api = new S2API(S2APIUrl, S2APIUser, S2APIPassword, false);

			var text = api.GetAccessLevel("2").InnerXml;

			AssignView(text, null);
		}

		private void ImportLevelsFromS2Click(object sender, EventArgs e)
		{
			var api = new S2API(S2APIUrl, S2APIUser, S2APIPassword, false);

			var importer = new S2Importer(api);
			importer.ImportLevels();
		}

		private void GetPersonClick(object sender, EventArgs e)
		{
			var api = new S2API(S2APIUrl, S2APIUser, S2APIPassword, false);

			var nd = api.GetPerson("0");
			var imgUrl = nd["PICTUREURL"].InnerText;

			var text = IndentXMLString(nd.OuterXml);

			AssignView(text, null);
		}

		private void GetPictureFromS2Click(object sender, EventArgs e)
		{
			var api = new S2API(S2APIUrl, S2APIUser, S2APIPassword, false);

			var nd = api.GetPicture("0");
			var fileName = nd["PICTUREURL"].InnerText;
			var imageData = Convert.FromBase64String(nd["PICTURE"].InnerText);

			var text = fileName;

			Image image;
			using (var imageStream = new MemoryStream(imageData))
			{
				image = Image.FromStream(imageStream);
			}

			//using (var stream = new FileStream(fileName, FileMode.Create))
			//{
			//    using (var writer = new BinaryWriter(stream))
			//    {
			//        writer.Write(imageData);
			//        writer.Close();
			//    }
			//}

			AssignView(text, image);
		}

		#endregion

		#region To S2
		private void ExportPersonClick(object sender, EventArgs e)
		{
			ExportAll();
		}

		#endregion

		#region R1SM
		private void button7_Click(object sender, EventArgs e)
		{
			var engine = new RoleAssignmentEngine();

			engine.ProcessPerson(6380);

			AssignView("Done processing person", null);
		}

		private void button8_Click(object sender, EventArgs e)
		{
			var engine = new RoleAssignmentEngine();

			engine.ProcessDirtyPeople(null);

			AssignView("Done processing dirty", null);
		}

		private void button10_Click(object sender, EventArgs e)
		{
			var ID = 2;
			var context = new RSMDataModelDataContext();
			var person = (from p in context.Persons
							 where p.PersonID == ID
							 select p).Single();

			AssignView(person.DisplayCredentials, null);
		}

		#endregion

		private void GetHistoriesClick(object sender, EventArgs e)
		{
			var api = new RSM.Integration.S2.API();

			var results = api.Login(S2APIUser, S2APIPassword, S2APIUrl, false);

			if (results.Failed)
			{
				AssignView("Failed login!");
				return;
			}

			var session = results.Entity;

			var accessResult = api.GetAccessHistory(DateTime.Parse("01/01/2012"));

			if (accessResult.Failed)
			{ 
				AssignView("Failed access request!");
				return;
			}

			foreach (var access in accessResult.Entity)
			{
				var apiPerson = api.RetrievePerson(access.Person.ExternalId);
				if (apiPerson.Succeeded)
					access.Person = apiPerson.Entity;

				var apiPortal = api.RetrievePortal(access.Portal.ExternalId);
				if (apiPortal.Succeeded)
					access.Portal = apiPortal.Entity;

				var apiReader = api.RetrieveReader(access.Reader.ExternalId);
				if (apiReader.Succeeded)
					access.Reader = apiReader.Entity;
			}

			var sb = new StringBuilder();
			foreach (var accessLog in accessResult.Entity)
			{
				sb.AppendFormat("{0} ({1}, {2})", accessLog.Person.ExternalId, accessLog.Person.LastName, accessLog.Person.FirstName);
				sb.AppendFormat(" | {0} ({1})", accessLog.Portal.ExternalId, accessLog.Portal.Name);
				sb.AppendFormat(" | {0} ({1})", accessLog.Reader.ExternalId, accessLog.Reader.Name);
				sb.AppendFormat(" | {0}", accessLog.Accessed);
				sb.Append(Environment.NewLine);
			}

			AssignView(sb.ToString());
		}

		#endregion

		private void StageSettings_Click(object sender, EventArgs e)
		{

		}

		private void People_Click(object sender, EventArgs e)
		{
			var context = new RSMDataModelDataContext();

			var s2 = context.ExternalSystems.FirstOrDefault(x => x.Id == ExternalSystems.S2In.Id);

			var factory = new StageFactory(context);

			var person = factory.createPerson("John", "Doe", middleName: null, id: null, isAdmin: false, isLockedOut: false,
			                                  username: null, password: null, credentials: null, nickname: null,
			                                  externalId: "John Doe", system: s2, action: EntityAction.InsertAndSubmit);

			var person2 = factory.createPerson("Brayton", "Rider", middleName: null, id: null, isAdmin: true, isLockedOut: false,
											  username: "brider", password: "Abcd1234", credentials: null, nickname: null,
											  externalId: "Brayton Rider", system: s2, action: EntityAction.InsertAndSubmit);

			var person3 = factory.createPerson("Janet", "Smith", middleName: null, id: null, isAdmin: true, isLockedOut: false,
											  username: "admin2", password: "admin", credentials: null, nickname: null,
											  externalId: "Smith, Jane", system: s2, action: EntityAction.InsertAndSubmit);

			var person4 = factory.createPerson("Bill", "Nye", middleName: null, id: null, isAdmin: false, isLockedOut: false,
											  username: null, password: null, credentials: null, nickname: null,
											  externalId: "The Science Guy", system: s2, action: EntityAction.InsertAndSubmit);

		}

		private void StagerClick(object sender, EventArgs e)
		{
			var form = new Staging();
			form.Show();
		}

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

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void ExportHistoryToTrackClick(object sender, EventArgs e)
		{
			MessageBox.Show("Not Implemented");
			//var result = Result<string>.Success();

			//var taskName = "TrackExportTest";

			//try
			//{
			//    var task = Task.Create(taskName, new ServiceProfile());

			//    var export = task as RSM.Integration.Track.Export.AccessEvents;

			//    result = export.Execute(null);
			//}
			//catch (Exception e)
			//{
			//    Assert.Fail("Test exception! {0}", e.ToString());
			//}

			//Assert.IsNotNull(result, "Missing results");
			//Assert.IsTrue(result.Succeeded, result.ToString());

		}

		private void btnGetPeople_Click(object sender, EventArgs e)
		{
			var api = new RSM.Integration.S2.API();

			var results = api.Login(S2APIUser, S2APIPassword, S2APIUrl, false);

			if (results.Failed)
			{
				AssignView("Failed login!");
				return;
			}

			var session = results.Entity;

			var sb = new StringBuilder();
			string nextKey = null;

			var peopleResult = api.GetPeople(ref nextKey);

			if (peopleResult.Failed)
			{
				AssignView(string.Format("Failed people request!{0}{1}", Environment.NewLine, peopleResult.ToString()));
				return;
			}

			var imageShown = false;
			foreach (var person in peopleResult.Entity)
			{
				var apiPerson = api.RetrievePersonDetail(person.ExternalId, true);
				if (apiPerson.Succeeded)
				{
					var detail = apiPerson.Entity;

					sb.AppendFormat("{0} ({1}, {2})", detail.ExternalId, detail.LastName, detail.FirstName);
					sb.AppendFormat(" | {0}", detail.Active ? "Active" : "Inactive");
					sb.AppendFormat(" | last updated: {0}", detail.ExternalUpdated);
					sb.AppendFormat(" | image: {0}", detail.Image != null);

					if (detail.Image != null)
					{
						Image image;
						try
						{
							using (var imageStream = new MemoryStream(detail.Image))
							{
								image = Image.FromStream(imageStream);
							}
							sb.Append(", image valid");

							//display first valid image retreived
							if (!imageShown)
							{
								AssignView("first image", image);
								imageShown = true;
							}
						}
						catch (Exception ex)
						{
							sb.AppendFormat(", image invalid: {0}", ex.Message);
						}
					}

					sb.Append(Environment.NewLine);
				}
				AssignView(sb.ToString());
			}
		}

		private void OpenServiceTester_Click(object sender, EventArgs e)
		{
			var form = new ServiceTester();
			form.Show();
		}
	}
}
