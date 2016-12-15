using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RSM.Support;

namespace RSM.Models.Admin
{
	public class LogEntryModel : ViewModel
	{
		public string SystemFilter { get; set; }

		public DateTime EventDate { get; set; }

		public string SourceName { get; set; }

		public string SeverityName { get; set; }

		public string Message { get; set; }

		public string Details { get; set; }

		public bool ShowDetails
		{
			get { return !string.IsNullOrWhiteSpace(Details); }
		}

		public LogEntryModel(LogEntry logEntry, string filter)
		{
			SystemFilter = filter;
			EventDate = logEntry.EventDate;
			SourceName = logEntry.SourceName;
			SeverityName = logEntry.SeverityName;
			Message = logEntry.Message;
			Details = logEntry.Details;
		}
	}
}