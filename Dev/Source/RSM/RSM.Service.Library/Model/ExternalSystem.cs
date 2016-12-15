using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Artifacts;
using ExternalSystemDirection = RSM.Support.ExternalSystemDirection;

namespace RSM.Service.Library.Model
{
	public class ExternalSystem : Entity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public ExternalSystemDirection Direction { get; set; }

		public ExternalSystem()
		{
			EntityType = EntityType.ExternalSystem;
		}

		#region Static Definitions
		public static ExternalSystem R1SM = Factory.CreateExternalSystem(1, Constants.R1SMSystemName, ExternalSystemDirection.None);
		public static ExternalSystem S2In = Factory.CreateExternalSystem(2, "S2", ExternalSystemDirection.Incoming);
		public static ExternalSystem TrackOut = Factory.CreateExternalSystem(3, "Track", ExternalSystemDirection.Outgoing);
		public static ExternalSystem S2Out = Factory.CreateExternalSystem(4, "S2", ExternalSystemDirection.Outgoing);
		public static ExternalSystem PsIn = Factory.CreateExternalSystem(5, "PeopleSoft", ExternalSystemDirection.Incoming);
		#endregion
	}
}
