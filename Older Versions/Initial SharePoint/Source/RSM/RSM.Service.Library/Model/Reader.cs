using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library.Model
{
	public class Reader : ExternalEntity
	{
		public string Name { get; set; }
		public int PortalId { get; set; }
		public Portal Portal { get; set; }
		public DateTime Added { get; set; }
		public int Direction { get; set; }

		public Reader()
		{
			EntityType = EntityType.Reader;
		}
	}
}
