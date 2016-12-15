using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSM.Service.Library.Model
{
	public class ExternalEntity : Entity
	{
		public int InternalId { get; set; }
		public string ExternalId { get; set; }
		public int ExternalSystemId { get; set; }
		public ExternalSystem ExternalSystem { get; set; }
		public DateTime KeysAdded { get; set; }
		public DateTime ExternalUpdated { get; set; }

		public ExternalEntity()
		{
			EntityType = EntityType.ExternalApplicationKey;
		}

	}
}
