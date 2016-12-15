
using System.ComponentModel.DataAnnotations;

namespace RSM.Staging.Library.Models
{
	public class StagingTools
	{
		public string ValidationKey { get; set; }

		[Display(Name = "R1SM")]
		public bool R1SM { get; set; }

		[Display(Name = "S2 Incoming")]
		public bool S2Incoming { get; set; }

		[Display(Name = "S2 Outgoing")]
		public bool S2Outgoing { get; set; }

		[Display(Name = "Track Outgoing")]
		public bool TrackOutgoing { get; set; }

		[Display(Name = "PeopleSoft Incoming")]
		public bool PsIncoming { get; set; }

		public bool S2
		{
			get { return S2Incoming || S2Outgoing; }
		}

		public bool Track
		{
			get { return TrackOutgoing; }
		}

		public bool PeopleSoft
		{
			get { return PsIncoming; }
		}

		[Display(Name = "S2 Incoming History")]
		public bool S2IncomingHistory { get; set; }

		[Display(Name = "S2 Outgoing History")]
		public bool S2OutgoingHistory { get; set; }

		[Display(Name = "Track Outgoing History")]
		public bool TrackOutgoingHistory { get; set; }

		[Display(Name = "Clean All People")]
		public bool CleanAllPeople { get; set; }

		[Display(Name = "Clean All Access History")]
		public bool CleanAllAccessHistory { get; set; }

		[Display(Name = "Clean All External App Keys")]
		public bool CleanAllExternalAppKeys { get; set; }

	}
}  