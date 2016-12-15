using System;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using RSM.Artifacts.Log;

namespace RSM.Support
{
	public partial class RSMDataModelDataContext
	{
		//public enum LogSources
		//{
		//    S2IMPORT, //0 -> 2 *
		//    S2EXPORT, //1 -> 4 *
		//    PSIMPORT, //2 -> 5 *
		//    USER      //3 -> 0 * 
		//}

		//public enum LogSeverity
		//{
		//    INFO,
		//    WARN,
		//    ERROR,
		//    DEBUG
		//}

		//public int CountLogEntriesWithStatus(LogSources source, LogSeverity sev, DateTime since)
		//{
		//    return (from l in LogEntries
		//            where ((l.Source == (int) source) &&
		//                   (l.EventDate >= since) &&
		//                   (l.Severity == (int) sev))
		//            select l).Count();
		//}

		public int CountLogEntriesWithStatus(ExternalSystem system, Severity sev, DateTime since)
		{
			var severity = (int)sev;

			if (system == null) return 0;

			return LogEntries.Count(l => l.Source == system.Id
			                             && l.EventDate >= since
										 && l.Severity == severity);
		}

		//public int GetSource(LogSources source)
		//{
		//    if (source == LogSources.USER) return 0;

		//    ExternalSystem system;
		//    switch (source)
		//    {
		//        case LogSources.S2EXPORT:
		//            system = ExternalSystems.FirstOrDefault(x => x.Name == "S2");
		//            return system == null ? 0 : system.Id;

		//        case LogSources.S2IMPORT:
		//            system = ExternalSystems.FirstOrDefault(x => x.Name == "S2");
		//            return system == null ? 0 : system.Id;

		//        case LogSources.PSIMPORT:
		//            system = ExternalSystems.FirstOrDefault(x => x.Name == "PeopleSoft");
		//            return system == null ? 0 : system.Id;
		//    }

		//    return (int)source;
		//}

		//public bool GetDirection(LogSources source)
		//{
		//    switch (source)
		//    {
		//        case LogSources.S2EXPORT:
		//            return false;

		//        case LogSources.S2IMPORT:
		//            return true;

		//        case LogSources.PSIMPORT:
		//            return true;

		//        default:
		//            return false;
		//    }
		//}

		//public void Syslog(LogSources src, LogSeverity sev, string message, string details)
		//{
		//    var sourceId = GetSource(src);
		//    var direction = GetDirection(src);

		//    var ent = new LogEntry
		//                       {
		//                           EventDate = DateTime.Now,
		//                           Source = sourceId,
		//                           Severity = (int) sev,
		//                           Message = message,
		//                           Details = details
		//                       };

		//    try
		//    {
		//        LogEntries.InsertOnSubmit(ent);
		//        SubmitChanges();
		//    }
		//    catch (Exception)
		//    {
		//        // Don't choke if we can't log.
		//    }
		//}

		public void Syslog(ExternalSystem system, Severity sev, string message, string details)
		{
			var severity = (int)sev;

			var sourceId = system == null ? 0 : system.Id;

			var ent = new LogEntry
			{
				EventDate = DateTime.Now,
				Source = sourceId,
				Severity = severity,
				Message = message,
				Details = details
			};

			try
			{
				LogEntries.InsertOnSubmit(ent);
				SubmitChanges();
			}
			catch (Exception ex)
			{
				// Don't choke if we can't log.
				EventLog.WriteEntry("R1SM", string.Format("Error saving log.\n{0}", ex.Message), EventLogEntryType.Error);
			}
		}
	}

	public partial class Person
	{
		private Job job;

		public Job Job
		{
			get
			{
				if (job == null && JobCode != null)
				{
					using (var ctx = new RSMDataModelDataContext())
					{
						job = ctx.Jobs.First(c => c.JobCode == JobCode);
					}
				}

				return job;
			}
		}

		public string DisplayName
		{
			get
			{
				return string.IsNullOrWhiteSpace(NickFirst) || NickFirst == FirstName
						   ? string.Format("{0}, {1} {2}", LastName, FirstName, MiddleName).Trim()
						   : string.Format("{0}, {1} ({3}) {2}", LastName, FirstName, MiddleName, NickFirst).Trim();
			}
		}

		public string DisplayCredentials
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(Credentials))
					return Credentials.ToUpper() == "DONOTPRINT" ? " " : Credentials;

				if (Job != null && !string.IsNullOrWhiteSpace(Job.Credentials))
					return Job.Credentials;

				return "";
			}
		}

		public static string SortFieldName(string sort)
		{
			switch (sort)
			{
				case "Department":
					return "DeptDescr";

				case "Name":
					return "LastName";

				case "Job":
					return "JobDescr";

				case "Status":
					return "Active";
			}

			return string.Empty;
		}
	}

	public partial class LogEntry
	{
		public string SourceName
		{
			get
			{
				if (Source == 0) return "User Activity";

				using (var context = new RSMDataModelDataContext())
				{
					var system = context.ExternalSystems.FirstOrDefault(x => x.Id == Source);

					return system == null
							   ? "Unknown"
							   : string.Format("{0} {1}", system.Name,
											   system.Direction == (int) ExternalSystemDirection.Incoming
												   ? "Importer"
												   : system.Direction == (int) ExternalSystemDirection.Outgoing
														 ? "Exporter"
														 : "");
				}
			}
		}

		public string SeverityName
		{
			get
			{
				return GetSeverityName(Severity);
			}
		}

		public static string GetSeverityName(Severity severity)
		{
			switch (severity)
			{
				case Artifacts.Log.Severity.Debug:
					return "Debug Information";

				case Artifacts.Log.Severity.Error:
					return "Error";

				case Artifacts.Log.Severity.Warning:
					return "Warning";

				case Artifacts.Log.Severity.Informational:
					return "Informational";

			}
			return "Unknown";
		}

		public static string GetSeverityName(int severity)
		{
			switch (severity)
			{
				case (int)Artifacts.Log.Severity.Debug:
					return "Debug Information";

				case (int)Artifacts.Log.Severity.Error:
					return "Error";

				case (int)Artifacts.Log.Severity.Warning:
					return "Warning";

				case (int)Artifacts.Log.Severity.Informational:
					return "Informational";

			}
			return "Unknown";
		}
	}

	public partial class PeopleRole
	{
		public PeopleRole(Person p, Role roleIn)
		{
			Role = roleIn;
			Person = p;
		}
	}

	public class RoleMetaData
	{
		[Required(ErrorMessage = "You must supply a name for this role.")]
		public string RoleName { get; set; }
	}

	[MetadataType(typeof (RoleMetaData))]
	public partial class Role
	{
		public string Description
		{
			get { return _RoleDesc; }
			set { _RoleDesc = value; }
		}
	}

	public partial class Job
	{
		public string DisplayName
		{
			get
			{
				return ((JobCode == "0")
							? JobDescription
							: string.Format("{0} ({1})", JobDescription, JobCode));
			}
		}

		public string DisplayNameCodeFirst
		{
			get
			{
				return ((JobCode == "0")
							? JobDescription
							: string.Format("{1} - {0}", JobDescription, JobCode));
			}
		}
	}

	public partial class Department
	{
		public string DisplayName
		{
			get { return ((DeptID == "0") ? DeptDescr : string.Format("{0} ({1})", DeptDescr, DeptID)); }
		}
	}
}
