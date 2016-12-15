using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RSM.Integration.S2.Stub.Model
{
	public class AccessHistory
	{
		public string LogId { get; set; }
		public string PersonId { get; set; }
		public string ReaderKey { get; set; }
		public string ReaderName { get; set; }
		public DateTime Dttm { get; set; }
		public string Type { get; set; }
		public string PortalKey { get; set; }
	}
}