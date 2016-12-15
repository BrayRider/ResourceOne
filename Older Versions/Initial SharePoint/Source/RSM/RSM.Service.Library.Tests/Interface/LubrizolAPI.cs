using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using API = RSM.Integration.Lubrizol.API;

using RSM.Artifacts;
using RSM.Service.Library.Interfaces;
using RSMDB = RSM.Support;
using Staged = RSM.Staging.Library.Data;

namespace RSM.Service.Library.Tests.Interface
{
	[TestClass]
	public class LubrizolAPI : Test
	{
		public static string User = "admin";
		public static string Password = "072159245241245031239120017047219193126250124056";
		public static string Url = "http://localhost:8088/mockACS2TrackWebSvcSoap12";

		[TestMethod]
		public void GetEmployee()
		{
			var api = new API();

			var result = api.GetEmployee(Staged.People.R1Person1ExternalId);

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
			Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());
			Assert.IsTrue(result.Entity.FirstName == Staged.People.R1Person1.FirstName, "Incorrect entity returned. {0}", result.Entity.FirstName);
		}
	}
}
