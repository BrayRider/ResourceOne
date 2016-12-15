using System;
using System.Diagnostics;
using System.Linq;
using RSM.Artifacts.Log;
using RSM.Support;

namespace RSM.Service.Library.Controllers
{
	public class Logger : DataController<LogEntry>
	{
		public Logger() : base()
		{
		}

		public Logger(RSMDataModelDataContext context) : base(context)
		{
			
		}

		public int CountLogEntriesWithStatus(int systemId, Severity severity, DateTime since)
		{
			return DbContext.LogEntries.Count(
				x => x.Source == systemId && x.EventDate >= since && x.Severity == (int) severity);
		}

		public void LogUserActivity(Severity sev, string message, string details)
		{
			var ent = new LogEntry
						  {
							  EventDate = DateTime.Now,
							  Source = 0,
							  Severity = (int) sev,
							  Message = message,
							  Details = details
						  };

			try
			{
				DbContext.LogEntries.InsertOnSubmit(ent);
				DbContext.SubmitChanges();
			}
			catch (Exception)
			{
				// Don't choke if we can't log.
				WriteToEventLog(ent.SourceName, message);
			}
		}

		public void LogSystemActivity(ExternalSystem system, Severity sev, string message, string details)
		{
			LogSystemActivity(system == null ? 0 : system.Id, sev, message, details);
		}
		public void LogSystemActivity(int systemId, Severity sev, string message, string details)
		{
			var ent = new LogEntry
						  {
							  EventDate = DateTime.Now,
							  Source = systemId,
							  Severity = (int) sev,
							  Message = message,
							  Details = details
						  };

			try
			{
				DbContext.LogEntries.InsertOnSubmit(ent);
				DbContext.SubmitChanges();
			}
			catch (Exception)
			{
				// Don't choke if we can't log.
				WriteToEventLog(ent.SourceName, message);
			}
		}

		//public void LogSystemActivity(RSMDataModelDataContext.LogSources source, RSMDataModelDataContext.LogSeverity sev, string message, string details)
		//{
		//    var ent = new LogEntry
		//                  {
		//                      EventDate = DateTime.Now,
		//                      Severity = (int) sev,
		//                      Message = message,
		//                      Details = details
		//                  };

		//    var sourceId = DbContext.GetSource(source);

		//    ent.Source = sourceId;

		//    try
		//    {
		//        DbContext.LogEntries.InsertOnSubmit(ent);
		//        DbContext.SubmitChanges();
		//    }
		//    catch (Exception)
		//    {
		//        // Don't choke if we can't log.
		//        WriteToEventLog(ent.SourceName, message);
		//    }
		//}

		private static void WriteToEventLog(string source, string message)
		{
			var elog = new EventLog {Source = source, EnableRaisingEvents = true};

			elog.WriteEntry(message);
		}

	}
}
