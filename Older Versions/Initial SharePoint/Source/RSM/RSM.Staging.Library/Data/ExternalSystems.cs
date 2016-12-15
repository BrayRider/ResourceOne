using RSM.Artifacts;
using RSM.Support;

namespace RSM.Staging.Library.Data
{
    public class ExternalSystems
    {
        public static ExternalSystem R1SM = Factory.CreateExternalSystem(1, Constants.R1SMSystemName, ExternalSystemDirection.None);
        public static ExternalSystem S2In = Factory.CreateExternalSystem(2, "S2 Import", ExternalSystemDirection.Incoming);
        public static ExternalSystem TrackOut = Factory.CreateExternalSystem(3, "Track", ExternalSystemDirection.Outgoing);
        public static ExternalSystem S2Out = Factory.CreateExternalSystem(4, "S2 Export", ExternalSystemDirection.Outgoing);
        public static ExternalSystem PsIn = Factory.CreateExternalSystem(5, "PeopleSoft", ExternalSystemDirection.Incoming);
		public static ExternalSystem LubrizolIn = Factory.CreateExternalSystem(6, "Lubrizol Import", ExternalSystemDirection.Incoming);
		public static ExternalSystem LubrizolOut = Factory.CreateExternalSystem(7, "Lubrizol Export", ExternalSystemDirection.Outgoing);
	}
}
