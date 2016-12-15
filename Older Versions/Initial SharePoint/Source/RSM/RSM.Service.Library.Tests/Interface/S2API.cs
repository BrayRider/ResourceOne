using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using API = RSM.Integration.S2.API;
using Staged = RSM.Staging.Library.Data;
using System.Collections.Generic;
using System.Configuration;

namespace RSM.Service.Library.Tests.Interface
{
	[TestClass]
	public class S2API : Test
	{
		public static string User = "admin";
		public static string Password = "072159245241245031239120017047219193126250124056";
		public static string DefaultLink = "http://localhost:8766/RSM.Integration.S2.Stub/RestService.svc/test";

		public string Link { 
			get
			{
				return ConfigurationManager.AppSettings["S2ServiceAddress"];
			}
		}

		[TestMethod]
		public void S2API_Login()
		{
			var api = new API();

			var result = api.Login(User, Password, Link);

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
		}

		[TestMethod]
		public void S2API_GetAccessHistories()
		{
			var api = new API();

			var login = api.Login(User, Password, Link);

			var result = api.GetAccessHistory(DateTime.Now.Subtract(TimeSpan.FromMinutes(10)));

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());

			Assert.IsTrue(result.Entity.Count > 0, "No records returned. {0}", result.ToString());

		}

		[TestMethod]
		public void S2API_RetrievePerson()
		{
			var api = new API();

			var login = api.Login(User, Password, Link);

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

			var login = api.Login(User, Password, Link);

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

			var login = api.Login(User, Password, Link);

			var result = api.RetrieveReader(Staged.Reader.Location1Reader1ExternalId);
			//var result = api.RetrieveReader("17");

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
			Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());
			Assert.IsTrue(result.Entity.Name == Staged.Reader.Location1Reader1.Name, "Incorrect entity returned. {0}", result.Entity.Name);
		}

		[TestMethod]
		public void S2API_RetrievePersonDetail()
		{
			var api = new API();

			var login = api.Login(User, Password, Link);

			//var result = api.RetrievePersonDetail(Staged.People.R1Person1ExternalId);
			var result = api.RetrievePersonDetail("_8", true);

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
			Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());
			Assert.IsTrue(!string.IsNullOrEmpty(result.Entity.FirstName), "Missing first name.");
			Assert.IsTrue(result.Entity.Image != null, "Missing image.");

		}

		[TestMethod]
		public void S2API_GetPeople()
		{
			var api = new API();

			var login = api.Login(User, Password, Link);

			string nextKey = null;
			var list = new List<Library.Model.Person>();
			var iterations = 1;
			do
			{
				var result = api.GetPeople(ref nextKey);

				Assert.IsNotNull(result, "Missing results");
				Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
				Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());

				list.AddRange(result.Entity);
			} while (nextKey != null && iterations++ < 2);
		}

		[TestMethod]
		public void S2API_GetPeople_DeletedOnly()
		{
			var api = new API();

			var login = api.Login(User, Password, Link);

			string nextKey = null;

			var result = api.GetPeople(ref nextKey, Support.S2.PersonState.Deleted);

			Assert.IsNotNull(result, "Missing results");
			Assert.IsTrue(result.Succeeded, "Call Failed. {0}", result.ToString());
			Assert.IsNotNull(result.Entity, "No entity returned. {0}", result.ToString());
			Assert.IsTrue(result.Entity.TrueForAll(x => !x.Active), "Not all people retrieved are deleted.");
		}
	}
}
