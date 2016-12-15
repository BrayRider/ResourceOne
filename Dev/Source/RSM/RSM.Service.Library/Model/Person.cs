using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library.Model
{
	public class Person : ExternalEntity
	{
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public DateTime Added { get; set; }
		public string BadgeNumber { get; set; }

		public string udf1 { get; set; }
		public string udf2 { get; set; }
		public string udf3 { get; set; }
		public string udf4 { get; set; }
		public string udf5 { get; set; }
		public string udf6 { get; set; }
		public string udf7 { get; set; }
		public string udf8 { get; set; }
		public string udf9 { get; set; }
		public string udf10 { get; set; }
		public string udf11 { get; set; }
		public string udf12 { get; set; }
		public string udf13 { get; set; }
		public string udf14 { get; set; }
		public string udf15 { get; set; }
		public string udf16 { get; set; }
		public string udf17 { get; set; }
		public string udf18 { get; set; }
		public string udf19 { get; set; }
		public string udf20 { get; set; }

		public Person()
		{
			EntityType = EntityType.Person;
		}
	}
}
