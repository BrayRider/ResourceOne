using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;

namespace RSM.Service.Library.Interfaces
{
	public interface IImportPeople
	{
		Result<List<Person>> GetPeople(ref string nextKey);

		Result<Person> RetrievePersonDetail(string id, bool includeImage = false);
	}
}
