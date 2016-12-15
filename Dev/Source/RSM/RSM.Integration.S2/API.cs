using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using RSM.Service.Library;
using RSM.Service.Library.Model;
using RSM.Service.Library.Interfaces;
using RSM.Support.S2;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Expressions;

namespace RSM.Integration.S2
{
	/// <summary>
	/// Contains implementations of RSM integration interfaces used for communicating with the S2 external system. 
	/// </summary>
	public class API : IAPI, IAuthentication, IAccessHistory
	{
		public string Name { get; private set; }
		public string Version { get; private set; }

		public UserSession UserSession { get; private set; }
		private S2API S2 { get; set; }

		public API()
		{
			Name = "S2 Import API";
			Version = "1.0";
		}

		public Result<UserSession> Login(string username, string password, string uri)
		{
			return Login(username, password, uri, true);
		}

		public Result<UserSession> Login(string username, string password, string uri, bool encryptedPassword = true)
		{
			var result = Result<UserSession>.Success();

			try
			{
				S2 = new S2API(uri, username, password, encryptedPassword);
			}
			catch (Exception e)
			{
				return result.Fail(e.ToString());
			}

			var session = new UserSession()
			{
				Username = username,
				Id = S2.SessionID
			};

			UserSession = session;
			result.Entity = session;
			return result;
		}

		public void Logoff(UserSession session)
		{
			if (S2 == null)
				return;

			S2.LogOut();
		}

		public Result<List<AccessLog>> GetAccessHistory(DateTime From, DateTime? To = null, string FromId = null)
		{
			var result = Result<List<AccessLog>>.Success();
			var list = new List<AccessLog>();

			//Get all records after the From date
			var more = true;
			string nextId = null;
			do
			{
				var xml = S2.GetAccessHistory(From, nextId: nextId, startFromId: FromId);
				if (xml == null)
					return result.Fail("Unable retrieve Access History.");

				if (xml.InnerText == "NOT FOUND")
				{
					return result;
				}

				list.AddRange(Mapper.AccessLogs(xml));

				var idNode = xml.SelectSingleNode("NEXTLOGID");
				if (idNode == null || idNode.InnerText == "-1")
					break;

				nextId = idNode.InnerText;
			} while (more);

			result.Entity = list.OrderBy(x => x.Accessed).ToList();

			return result;
		}

		public Result<Person> RetrievePerson(string id)
		{
			var result = Result<Person>.Success();
			var xml = S2.GetPerson(id);

			result.RequiredObject(xml, string.Format("Unable to retrieve S2 person ID {0}.", id));
			if (result.Failed)
				return result;

/*
 * <DETAILS><PERSONID>3029</PERSONID>
 * <FIRSTNAME>Virginia</FIRSTNAME>
 * <MIDDLENAME>L</MIDDLENAME><LASTNAME>Abshear</LASTNAME>
 * <ACTDATE>2011-03-16 00:01:00</ACTDATE>
 * <UDF1>403</UDF1><UDF2>Secretary</UDF2><UDF3>5605000</UDF3><UDF4>SRMC Nursing Administration</UDF4><UDF5>SRMC</UDF5>
 * <DELETED>FALSE</DELETED>
 * <ACCESSLEVELS><ACCESSLEVEL>GC.1A</ACCESSLEVEL><ACCESSLEVEL>North Stairwell Access</ACCESSLEVEL></ACCESSLEVELS></DETAILS>"
 */
			var person = Factory.CreatePerson(xml.GetElementValue("PERSONID"), ExternalSystem.S2In);

			person.FirstName = xml.GetElementValue("FIRSTNAME");
			person.MiddleName = xml.GetElementValue("MIDDLENAME");
			person.LastName = xml.GetElementValue("LASTNAME");

			person.udf1 = xml.GetElementValue("UDF1");
			person.udf2 = xml.GetElementValue("UDF2");
			person.udf3 = xml.GetElementValue("UDF3");
			person.udf4 = xml.GetElementValue("UDF4");
			person.udf5 = xml.GetElementValue("UDF5");
			person.udf6 = xml.GetElementValue("UDF6");
			person.udf7 = xml.GetElementValue("UDF7");
			person.udf8 = xml.GetElementValue("UDF8");
			person.udf9 = xml.GetElementValue("UDF9");
			person.udf10 = xml.GetElementValue("UDF10");
			person.udf11 = xml.GetElementValue("UDF11");
			person.udf12 = xml.GetElementValue("UDF12");
			person.udf13 = xml.GetElementValue("UDF13");
			person.udf14 = xml.GetElementValue("UDF14");
			person.udf15 = xml.GetElementValue("UDF15");
			person.udf16 = xml.GetElementValue("UDF16");
			person.udf17 = xml.GetElementValue("UDF17");
			person.udf18 = xml.GetElementValue("UDF18");
			person.udf19 = xml.GetElementValue("UDF19");
			person.udf20 = xml.GetElementValue("UDF20");

			result.Entity = person;

			return result;
		}

		public Result<Portal> RetrievePortal(string id)
		{
			var result = Result<Portal>.Success();
			var xml = S2.GetPortal(id);

			result.RequiredObject(xml, string.Format("S2 portal ID {0} was not found.", id));
			if (result.Failed)
				return result;

			var portal = Factory.CreatePortal(xml["PORTALKEY"].InnerText, ExternalSystem.S2In);
			portal.Name = xml.GetElementValue("NAME");

			result.Entity = portal;

			return result;
		}

		public Result<Reader> RetrieveReader(string id)
		{
			var result = Result<Reader>.Success();
			var xml = S2.GetReader(id);

			result.RequiredObject(xml, string.Format("S2 reader ID {0} was not found.", id));
			if (result.Failed)
				return result;

			var reader = Factory.CreateReader(id, ExternalSystem.S2In);
			reader.Name = xml.GetElementValue("NAME");

			result.Entity = reader;

			return result;
		}
	}

	public static class XmlExtensions
	{
		public static string GetElementValue(this XmlNode node, string name)
		{
			var elem = node[name];
			return elem != null ? elem.InnerText : null;
		}
	}
}
