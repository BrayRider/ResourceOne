using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using RSM.Artifacts.Log;
using RSM.Service.Library.Interfaces;
using System.Text;
using Logger = RSM.Service.Library.Controllers.Logger;

namespace RSM.Service.Library
{
	public class ServiceProfile
	{
		private static int DefaultSystemId = 0;

		#region Config Reference
		public abstract class Config
		{
			public abstract class App
			{
				public static string Id = "ServiceId";
				public static string ServiceName = "ServiceName";
				public static string Description = "Description";
			}
		}
		#endregion

		public int Id { get; private set; }
		public ServiceBase Service { get; set; }
		public List<ITask> Tasks { get; set; }
		public string Name { get; private set; }
		public string Description { get; private set; }

		private List<UnityFactory> Containers { get; set; }
		private Logger Logger { get; set; }

		public ServiceProfile()
		{
			Tasks = new List<ITask>();
			Containers = new List<UnityFactory>();

			int id;
			Trace.Assert(int.TryParse(ConfigurationManager.AppSettings[ServiceProfile.Config.App.Id], out id), 
				string.Format("{0} must be provided in the configuration.", ServiceProfile.Config.App.Id));
			Id = id;

			Name = ConfigurationManager.AppSettings[ServiceProfile.Config.App.ServiceName];
			Trace.Assert(!string.IsNullOrEmpty(Name), string.Format("{0} must be provided in the configuration.", ServiceProfile.Config.App.ServiceName));

			Description = ConfigurationManager.AppSettings[ServiceProfile.Config.App.Description];
			Description = Description ?? string.Empty;

			Logger = new Logger();
		}

		public void Load(string[] args, ServiceBase service = null)
		{
			LogMessage(EventLogEntryType.Information, "Loading service configuration.");

			Service = service;

			//Load tasks to be hosted
			for (var i = 1; true; i++)
			{
				var name = string.Format("Task{0}", i);
				var value = ConfigurationManager.AppSettings[name];
				if (string.IsNullOrEmpty(value))
					break;

				var task = Task.Create(value, this);
				task.Load(args);

				Tasks.Add(task);
			}
		}

		public string LogException(Exception e, string message, params object[] parms)
		{
			return LogException(DefaultSystemId, e, message, parms);
		}
		public string LogException(int systemId, Exception e, string message, params object[] parms)
		{
			try
			{
				var msg = new StringBuilder();
				msg.AppendFormat(message, parms);

				Logger.LogSystemActivity(systemId, Severity.Error, msg.ToString(), e.ToString());

				msg.Append(e.ToString());

				var text = msg.ToString();
				Service.EventLog.WriteEntry(text, EventLogEntryType.Error);

				return text;
			}
			catch (Exception)
			{ }
			return string.Empty;
		}

		public string LogMessage(EventLogEntryType type, string message, params object[] parms)
		{
			return LogMessage(DefaultSystemId, type, string.Empty, message, parms);
		}

		public string LogMessageDetail(int systemId, EventLogEntryType type, string detail, string message, params object[] parms)
		{
			return LogMessage(systemId, type, detail, message, parms);
		}

		public string LogMessageDetail(EventLogEntryType type, string detail, string message, params object[] parms)
		{
			return LogMessage(DefaultSystemId, type, detail, message, parms);
		}

		public string LogMessage(int systemId, EventLogEntryType type, string detail, string message, params object[] parms)
		{
			var text = string.Empty;

			try
			{
				var msg = new StringBuilder();
				msg.AppendFormat(message, parms);

				var sev = Severity.Debug;
				if (type == EventLogEntryType.Error || type == EventLogEntryType.FailureAudit)
					sev = Severity.Error;

				else if (type == EventLogEntryType.Information || type == EventLogEntryType.SuccessAudit)
					sev = Severity.Informational;

				else if (type == EventLogEntryType.Warning)
					sev = Severity.Warning;

				sev = systemId == 0 ? Severity.Debug : sev;

				Logger.LogSystemActivity(systemId, sev, msg.ToString(), detail);

				text = msg.ToString();

				if (Service != null && Service.EventLog != null)
					Service.EventLog.WriteEntry(text, type);
			}
			catch (Exception)
			{ }

			return text;
		}
	}
}
