using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using RSM.Support;
using System.Collections;

namespace RSM.Integration.S2.Stub.Handlers
{
	public class Handler : ICommands
	{
		public static string DefaultSessionId = "123456789";

		public virtual XElement Login(XDocument request)
		{
			var response = NewResponse();
			response.Root.SetAttributeValue("sessionid", DefaultSessionId);
			SetCode("SUCCESS", response);
			return response.Root;
		}

		public virtual XElement Logout(XDocument request)
		{
			var response = NewResponse();

			if (IsValidSessionId(request))
				SetCode("SUCCESS", response);
			else
				SetCode("FAIL", response, "Unknown session id specified.");

			return response.Root;
		}

		public virtual XElement GetAccessHistory(XDocument request)
		{
			var response = NewResponse();

			var fromLogId = request.Descendants().FirstOrDefault(x => x.Name == "STARTLOGID");
			if (fromLogId == null)
				Data.Content.ReloadAccessHistory();

			SetCode("SUCCESS", response);

			response.Root.Element("RESPONSE").Add(new XElement("DETAILS", new XElement("ACCESSES")));
			var accesses = response.Root.Element("RESPONSE").Element("DETAILS").Element("ACCESSES");


			// Order records by ascending date 
			var list = fromLogId != null
				? Data.Content.AccessHistories.Where(x => x.Key.CompareTo(fromLogId.Value) <= 0).OrderByDescending(x => x.Value.Dttm).ToList()
				: Data.Content.AccessHistories.OrderByDescending(x => x.Value.Dttm).ToList();

			var count = 0;
			foreach (var entry in list)
			{
				var entity = entry.Value;

				var access = new XElement("ACCESS");
				access.SetElementValue("LOGID", entity.LogId);
				access.SetElementValue("PERSONID", entity.PersonId);
				access.SetElementValue("READER", entity.ReaderName);
				access.SetElementValue("DTTM", entity.Dttm.ToString("yyyy-MM-dd HH:mm:ss"));
				access.SetElementValue("TYPE", entity.Type);
				access.SetElementValue("READERKEY", entity.ReaderKey);
				access.SetElementValue("PORTALKEY", entity.PortalKey);
				accesses.Add(access);

				if (++count == 10)
					break;
			}

			accesses.AddAfterSelf(new XElement("NEXTLOGID", (count < list.Count) ? list.ElementAt(count).Key : "-1"));

			return response.Root;
		}

		/// <summary>
		/// Should mimic this format... 
		///<DETAILS> <PERSONID>ID of Person record</PERSONID>
		///<FIRSTNAME>person’s first name</FIRSTNAME>
		///<LASTNAME>person’s last name</LASTNAME>
		///<UDF1>User Defined Field</UDF1> <UDF2>User Defined Field</UDF1> … <UDF20>User Defined Field</UDF20> 
		///<PICTUREURL>Filename for picture data</PICTUREURL> <
		///    DELETED>TRUE/FALSE</DELETED> 
		///<ACCESSLEVELS> <ACCESSLEVEL>access level 1</ACCESSLEVEL> <ACCESSLEVEL>access level 2</ACCESSLEVEL> . . . <ACCESSLEVEL>access level 32</ACCESSLEVEL> </ACCESSLEVELS> 
		//    </DETAILS>
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public virtual XElement GetPerson(XDocument request)
		{
			var response = NewResponse();

			var id = request.Descendants().FirstOrDefault(x => x.Name == "PERSONID" && x.Parent.Name == "PARAMS");

			if (Data.Content.People.ContainsKey(id.Value))
			{
				SetCode("SUCCESS", response);

				var entity = Data.Content.People[id.Value];

				response.Root.Element("RESPONSE").SetElementValue("DETAILS", string.Empty);
				var detail = response.Root.Element("RESPONSE").Element("DETAILS");

				detail.SetElementValue("PERSONID", id.Value);
				detail.SetElementValue("FIRSTNAME", entity.FirstName);
				detail.SetElementValue("LASTNAME", entity.LastName);
			}
			else
				SetCode("FAIL", response, string.Format("Unknown person {0}.", id != null ? id.Value : "?"));

			return response.Root;
		}

		
		/// <summary>
		///
		/// <DETAILS> <PORTALS> <PORTAL> <NAME>name of portal</NAME> <PORTALKEY>unique id for portal</PORTALKEY> <READERS> <READER> <READERKEY>unique id for reader</READERKEY> <NAME>name of reader</NAME> <PORTALORDER>1 or 2</PORTALORDER> </READER> <READER> … </READER> </READERS> </PORTAL> <PORTAL> … </PORTAL> </PORTALS> <NEXTKEY>key of next portal</NEXTKEY> </DETAILS>
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public virtual XElement GetPortals(XDocument request)
		{
			var response = NewResponse();

			var id = request.Descendants().FirstOrDefault(x => x.Name == "STARTFROMKEY" && x.Parent.Name == "PARAMS");

			if (Data.Content.Portals.ContainsKey(id.Value))
			{
				SetCode("SUCCESS", response);

				var entity = Data.Content.Portals[id.Value];

				response.Root.Element("RESPONSE").SetElementValue("DETAILS", string.Empty);
				var detail = response.Root.Element("RESPONSE").Element("DETAILS");

				detail.Add(new XElement("PORTALS", new XElement("PORTAL")));
				var portal = detail.Element("PORTALS").Element("PORTAL");
				portal.SetElementValue("NAME", entity.Name);
				portal.SetElementValue("PORTALKEY", id.Value);

				portal.Add(new XElement("READERS", new XElement("READER")));
				var reader = portal.Element("READERS").Element("READER");

				//TODO: determine whether we really need to mock the readers here 
			}
			else
				SetCode("FAIL", response, string.Format("Unknown portal {0}.", id != null ? id.Value : "?"));

			return response.Root;
		}

		public virtual XElement GetReader(XDocument request)
		{
			var response = NewResponse();

			var id = request.Descendants().FirstOrDefault(x => x.Name == "READERKEY" && x.Parent.Name == "PARAMS");

			if (id != null && Data.Content.Readers.ContainsKey(id.Value))
			{
				SetCode("SUCCESS", response);

				var entity = Data.Content.Readers[id.Value];

				response.Root.Element("RESPONSE").SetElementValue("DETAILS", string.Empty);
				var detail = response.Root.Element("RESPONSE").Element("DETAILS");

				detail.SetElementValue("READER", string.Empty);
				detail = detail.Element("READER");

				detail.SetElementValue("NAME", entity.Name);
				detail.SetElementValue("DESCRIPTION", entity.Name);
			}
			else
				SetCode("FAIL", response, string.Format("Unknown reader {0}.", id != null ? id.Value : "?"));

			return response.Root;
		}

		#region Helpers
		public static XDocument NewResponse()
		{
			return new XDocument(new XElement("NETBOX", new XElement("RESPONSE")));
		}
		public static bool IsValidSessionId(XDocument request)
		{
			var id = request.Element("NETBOX-API").Attribute("sessionid");
			return id != null && id.Value == DefaultSessionId;
		}
		public static XDocument SetCode(string code, XDocument response = null, string msg = null)
		{
			response = response ?? NewResponse();

			var resp = response.Root.Element("RESPONSE");
			resp.SetElementValue("CODE", code);

			if (msg != null)
			{
				resp.SetElementValue("DETAILS", string.Empty);
				resp.Element("DETAILS").SetElementValue("ERRMSG", msg);
			}
			return response;
		}
		#endregion
	}
}