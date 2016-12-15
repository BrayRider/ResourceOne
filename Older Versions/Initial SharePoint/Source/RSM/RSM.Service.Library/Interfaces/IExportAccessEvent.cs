using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;

namespace RSM.Service.Library.Interfaces
{
	public interface IExportAccessEvent
	{
		bool PushEvent(AccessLog log);
		Result<AccessLog> ExportEvent(AccessLog log);
		Result<Person> ExportPerson(Person person, AccessLog log);
		Result<Reader> ExportReader(Reader reader, AccessLog log);
		Result<Portal> ExportPortal(Portal portal, AccessLog log);
		Result<Location> ExportLocation(Location location, AccessLog log);
		Result<string> ExportCompany(AccessLog log);
	}
}
