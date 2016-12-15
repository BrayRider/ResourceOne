using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;
using System.Xml;

namespace RSM.Integration.S2
{
	public class Mapper
	{
		public static List<AccessLog> AccessLogs(XmlNode details)
		{
			var list = new List<AccessLog>();
			var all = details.SelectNodes("ACCESSES/ACCESS");

			if (all == null)
				return list;

			foreach (XmlNode node in all)
			{
				var access = Factory.CreateAccessLog(node["LOGID"].InnerText, ExternalSystem.S2In);
				access.Person = Factory.CreatePerson(node["PERSONID"].InnerText, ExternalSystem.S2In);
				access.Portal = Factory.CreatePortal(node["PORTALKEY"].InnerText, ExternalSystem.S2In);
				access.Reader = Factory.CreateReader(node["READERKEY"].InnerText, ExternalSystem.S2In);
				access.Accessed = DateTime.Parse(node["DTTM"].InnerText);
				access.AccessType = int.Parse(node["TYPE"].InnerText);

				if (node["REASON"] != null)
				{
					var reason = node["REASON"].InnerText;
					if(!string.IsNullOrWhiteSpace(reason))
						access.Reason = int.Parse(node["REASON"].InnerText);
				}

				list.Add(access);
			}

			return list;
		}


		/*
		 * <DETAILS><PERSONID>3029</PERSONID>
		 * <FIRSTNAME>Virginia</FIRSTNAME>
		 * <MIDDLENAME>L</MIDDLENAME><LASTNAME>Abshear</LASTNAME>
		 * <ACTDATE>2011-03-16 00:01:00</ACTDATE>
		 * <UDF1>403</UDF1><UDF2>Secretary</UDF2><UDF3>5605000</UDF3><UDF4>SRMC Nursing Administration</UDF4><UDF5>SRMC</UDF5>
		 * <DELETED>FALSE</DELETED>
		 * <ACCESSLEVELS><ACCESSLEVEL>GC.1A</ACCESSLEVEL><ACCESSLEVEL>North Stairwell Access</ACCESSLEVEL></ACCESSLEVELS></DETAILS>"
		 */
		public static Person ToPerson(XmlNode xml)
		{
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

			// Conditional fields only available when grabbing details via SearchPersonData
			person.ExternalUpdated = xml.GetElementValueAsDate("LASTMOD") ?? person.ExternalUpdated;

			var deleted = xml.GetElementValue("DELETED");
			if (deleted != null)
				person.Active = deleted.Equals("false", StringComparison.InvariantCultureIgnoreCase);

			return person;
		}

		public static List<Person> ToPeople(XmlNode xml)
		{
			var list = new List<Person>();
			var all = xml.SelectNodes("PEOPLE/PERSON");

			if (all == null)
				return list;

			foreach (XmlNode node in all)
			{
				var person = ToPerson(node);

				list.Add(person);
			}

			return list;
		}
	}
}
