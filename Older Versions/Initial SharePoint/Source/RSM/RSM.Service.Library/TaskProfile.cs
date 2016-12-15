using System;

using RSM.Service.Library.Controllers;

namespace RSM.Service.Library
{
	public class TaskProfile
	{
		public string ConfigPrefix { get; private set; }
		public Scheduler Schedule { get; set; }
		public TimeSpan StopTaskTimeout { get; set; }

		public Task Task { get; private set; }

		private ServiceProfile _serviceProfile;

		#region Config Names
		public abstract class Config
		{
			public static string Repeat = "Repeat";
			public static string RepeatInterval = "RepeatInterval";
		}
		#endregion

		public TaskProfile(Task task, string configPrefix = null)
		{
			Task = task;
			task.Profile = this;
			_serviceProfile = task.ServiceProfile;

			StopTaskTimeout = TimeSpan.FromSeconds(45);
			Schedule = new Scheduler();

			ConfigPrefix = configPrefix ?? task.Name;

			var settings = new TaskSettings(task);
			var value = settings.GetValue(Config.Repeat);
			Schedule.Repeat = bool.Parse(value);

			value = settings.GetValue(Config.RepeatInterval);
			var interval = int.Parse(value);
			Schedule.RepeatInterval = TimeSpan.FromMinutes(interval);
		}

		#region Config Helpers
		public string ConfigName(string name, bool usePrefix = true)
		{
			return usePrefix ? string.Format("{0}.{1}", ConfigPrefix, name) : name;
		}

		#endregion

	}
}
