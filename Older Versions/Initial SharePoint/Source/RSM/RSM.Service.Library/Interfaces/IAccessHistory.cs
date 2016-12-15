using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;

namespace RSM.Service.Library.Interfaces
{
	public interface IImportAccessHistory
	{
		Result<List<AccessLog>> GetAccessHistory(DateTime From, DateTime? To = null, string LastKey = null);

		Result<Person> RetrievePerson(string id);

		Result<Portal> RetrievePortal(string id);

		Result<Reader> RetrieveReader(string id);
	}
}
