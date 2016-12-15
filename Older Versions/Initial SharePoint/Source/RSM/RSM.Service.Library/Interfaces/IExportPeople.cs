using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Service.Library.Model;

namespace RSM.Service.Library.Interfaces
{
	public interface IExportPeople
	{
		Result<Person> ExportPerson(Person entity);
	}
}
