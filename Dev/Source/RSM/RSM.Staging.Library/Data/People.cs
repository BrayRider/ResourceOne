using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSMDB = RSM.Support;

namespace RSM.Staging.Library.Data
{
	public class People
	{
		//R1 people for S2 import
		public static string R1Person1ExternalId = "SR.Person1";
		public static RSMDB.Person R1Person1 = Factory.CreatePerson("Joe", "Smith", string.Empty);

		public static string R1Person2ExternalId = "SR.Person2";
		public static RSMDB.Person R1Person2 = Factory.CreatePerson("Kelly", "Jones", string.Empty);


	}
}
