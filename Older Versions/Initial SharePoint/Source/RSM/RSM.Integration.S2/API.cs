using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public class API : IAPI, IAuthentication, IImportAccessHistory, IImportPeople
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

			var person = Mapper.ToPerson(xml);

			result.Entity = person;

			return result;
		}

		public Result<Person> RetrievePersonDetail(string id, bool includeImage = false)
		{
			var result = Result<Person>.Success();

			var xml = S2.SearchPersonData(id);

			result.RequiredObject(xml, string.Format("Unable to retrieve detail for S2 person ID {0}.", id));
			if (result.Failed)
				return result;

			var personNode = xml.SelectSingleNode("PEOPLE/PERSON");
			if (personNode == null)
				return result.Fail("Person not found", "NotFound");

			var person = Mapper.ToPerson(personNode);
			//EventLog.WriteEntry("Application", personNode.OuterXml);

			if (includeImage)
			{
				var url = personNode.GetElementValue("PICTUREURL");
				if(!string.IsNullOrWhiteSpace(url))
				{
					person.Image = S2.GetPictureFromUrl(url);
				}
			}

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

		public Result<List<Person>> GetPeople(ref string nextKey)
		{
			return GetPeople(ref nextKey, PersonState.All);
		}
		public Result<List<Person>> GetPeople(ref string nextKey, PersonState state = PersonState.All)
		{
			var result = Result<List<Person>>.Success();
			var list = new List<Person>();

			//Get all records after the From date
			var xml = S2.SearchPersonData(startingKey: nextKey, state: state );
			if (xml == null)
				return result.Fail("Unable retrieve people.");

			if (xml.InnerText == "NOT FOUND")
			{
				return result;
			}

			list.AddRange(Mapper.ToPeople(xml));

			var idNode = xml.SelectSingleNode("NEXTKEY");
			nextKey = (idNode == null || idNode.InnerText == "-1") ? null : idNode.InnerText;

			result.Entity = list.OrderBy(x => x.Updated).ToList();

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
		public static DateTime? GetElementValueAsDate(this XmlNode node, string name)
		{
			var value = GetElementValue(node, name);

			if (string.IsNullOrWhiteSpace(value)) return (DateTime?)null;

			if (value.IndexOf(".", StringComparison.Ordinal) > 0)
			{
				value = value.Substring(0, value.IndexOf(".", StringComparison.Ordinal));
			}

			DateTime returnValue;

			if (DateTime.TryParse(value, out returnValue))
			{
				return (DateTime?)returnValue;
			}
			return (DateTime?)null;
		}
	}
}
