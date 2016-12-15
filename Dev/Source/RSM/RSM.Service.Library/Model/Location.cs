using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library.Model
{
	public class Location : ExternalEntity
	{
		public string Name { get; set; }
		public DateTime Added { get; set; }

		public Location()
		{
			EntityType = EntityType.Location;
		}
	}
}
