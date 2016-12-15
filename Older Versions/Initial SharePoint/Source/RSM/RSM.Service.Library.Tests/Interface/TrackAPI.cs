using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using API = RSM.Integration.Track.API;

using RSM.Artifacts;
using RSM.Service.Library.Interfaces;
using RSMDB = RSM.Support;

namespace RSM.Service.Library.Tests.Interface
{
	[TestClass]
	public class TrackAPI : Test
	{
		public static string User = "admin";
		public static string Password = "072159245241245031239120017047219193126250124056";
		public static string Url = "http://localhost:8088/mockACS2TrackWebSvcSoap12";

		[TestMethod]
		public void TrackAPI_Login()
		{
			Task task = null;
			var taskName = "TrackAPITest";

			using (var context = new RSMDB.RSMDataModelDataContext())
			{
				Export.ExportAccessEvents.LoadTrackExportTestData(context, taskName);
			}

			task = Task.Create(taskName, new ServiceProfile());
			var export = task as Export.TrackExportWrapperStub;

			var api = export.LoadApiTest() as IAuthentication;

			var result = api.Login(User, Password, Url);

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed");
		}
	}
}
