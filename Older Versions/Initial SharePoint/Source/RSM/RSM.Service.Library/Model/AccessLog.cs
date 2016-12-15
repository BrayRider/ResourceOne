using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Support;

namespace RSM.Service.Library.Model
{
	/// <summary>
	/// Represents a single instance of a portal access attempt.
	/// </summary>
	public class AccessLog : ExternalEntity
	{
		public DateTime Accessed { get; set; }
		public Person Person { get; set; }
		public Portal Portal { get; set; }
		public Reader Reader { get; set; }
		public int PersonId { get; set; }
		public int PortalId { get; set; }
		public int ReaderId { get; set; }
		public int AccessType { get; set; }
		public int Reason { get; set; }

		public AccessLog()
		{
			EntityType = EntityType.AccessLog;
		}
	}
}
