using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using RSM.Service.Library;
using PeopleBase = RSM.Service.Library.Import.People;
using RSM.Service.Library.Model;

namespace RSM.Integration.S2.Import
{
	public class People : PeopleBase
	{
		public People()
			: base()
		{
			base.ExternalSystem = RSM.Service.Library.Model.ExternalSystem.S2In;
		}
	}
}
