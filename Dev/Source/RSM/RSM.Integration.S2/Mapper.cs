using System;
using System.Collections.Generic;
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

	}
}
