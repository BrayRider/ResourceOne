using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library.Model
{
	public class Portal : ExternalEntity
	{
		public string Name { get; set; }
		public DateTime Added { get; set; }
		public int LocationId { get; set; }
		public Location Location { get; set; }
		public string NetworkAddress { get; set; }
		public string Capabilities { get; set; }
		public int? DeviceType { get; set; }
	
		public int ReaderCount { get; set; }
 
		public Portal()
		{
			EntityType = EntityType.Portal;
		}
	}
}
