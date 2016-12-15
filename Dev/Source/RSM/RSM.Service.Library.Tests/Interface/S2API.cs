using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using API = RSM.Service.Library.Tests.Import.S2ImportAPIStub;
using Staged = RSM.Staging.Library.Data;

namespace RSM.Service.Library.Tests.Interface
{
	[TestClass]
	public class S2API : Test
	{
		public static string User = "admin";
		public static string Password = "072159245241245031239120017047219193126250124056";
		public static string link = "http://localhost:8766/RSM.Integration.S2.Stub/RestService.svc/test";
		//public static string link = "http://10.1.1.234/goforms/nbapi";

		[TestMethod]
		public void S2API_Login()
		{
			var api = new API();

			var result = api.Login(User, Password, link);

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
		}

		[TestMethod]
		public void S2API_GetAccessHistories()
		{
			var api = new API();

			var login = api.Login(User, Password, link);

			var result = api.GetAccessHistory(DateTime.Now.Subtract(TimeSpan.FromMinutes(10)));

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());

			Assert.IsTrue(result.Entity.Count > 0, "No records returned. {0}", result.ToString());

		}

        [TestMethod]
		public void S2API_RetrievePerson()
		{
			var api = new API();

			var login = api.Login(User, Password, link);

			var result = api.RetrievePerson(Staged.People.R1Person1ExternalId);
			//var result = api.RetrievePerson("3029");

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
			Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());
			Assert.IsTrue(result.Entity.FirstName == Staged.People.R1Person1.FirstName, "Incorrect entity returned. {0}", result.Entity.FirstName);
		}

		[TestMethod]
		public void S2API_RetrievePortal()
		{
			var api = new API();

			var login = api.Login(User, Password, link);

			var result = api.RetrievePortal(Staged.Portal.Location1Portal1ExternalId);
			//var result = api.RetrievePortal("13");

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
			Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());
			Assert.IsTrue(result.Entity.Name == Staged.Portal.Location1Portal1.Name, "Incorrect entity returned. {0}", result.Entity.Name);
		}

		[TestMethod]
		public void S2API_RetrieveReader()
		{
			var api = new API();

			var login = api.Login(User, Password, link);

			var result = api.RetrieveReader(Staged.Reader.Location1Reader1ExternalId);
			//var result = api.RetrieveReader("17");

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
			Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());
			Assert.IsTrue(result.Entity.Name == Staged.Reader.Location1Reader1.Name, "Incorrect entity returned. {0}", result.Entity.Name);
		}

	}
}
