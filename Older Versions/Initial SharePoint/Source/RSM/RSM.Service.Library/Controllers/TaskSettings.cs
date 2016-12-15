using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Configuration;

using RSM.Support;
using RSM.Service.Library.Interfaces;

using RSM.Service.Library.Extensions;
using Model = RSM.Service.Library.Model;


namespace RSM.Service.Library.Controllers
{
	/// <summary>
	/// Actions taken via this object are within the scope of the external system assigned to the TaskProfile provided. 
	/// </summary>
    public class TaskSettings
    {
		#region Config Reference
		public abstract class Config
		{
			public abstract class App
			{
				public static string UseDbSettings = "_UseDbSettings";
			}
		}
		#endregion

		private Task _task;
		private TaskProfile _profile;
		private bool _dbMode;

		public TaskSettings(Task task, bool dbMode = true)
			: base()
		{
			_task = task;
			_profile = _task.Profile;
			_dbMode = dbMode;

			var fullName = _task.Profile.ConfigName(Config.App.UseDbSettings, true);
			var mode = ConfigurationManager.AppSettings[fullName];
			if (!string.IsNullOrEmpty(mode))
			{
				_dbMode = bool.Parse(mode);
			}
		}

		public Setting Get(string name, bool usePrefix = true)
		{
			var fullName = _task.Profile.ConfigName(name, usePrefix);
			var results = new Settings().Search(s => s.ExternalSystem.Id == _task.ExternalSystem.Id && s.Name.Equals(fullName));
			return (results.Entity != null && results.Entity.Count > 0) ? results.Entity.FirstOrDefault() : null;
		}

		public string GetValue(string name, bool usePrefix = true)
		{
			if (_dbMode)
			{
				var setting = Get(name);
				return (setting != null) ? setting.Value : null;
			}

			//check if app config has the value
			var fullName = _task.Profile.ConfigName(name, usePrefix);
			return ConfigurationManager.AppSettings[fullName];
		}

		public bool GetBoolValue(string name, bool usePrefix = true)
		{
			bool value;
			if (!bool.TryParse(GetValue(name), out value))
				throw new ArgumentException(_task.LogError("{0} setting is not valid.", name));

			return value;
		}

		public int GetIntValue(string name, bool usePrefix = true)
		{
			int value;
			if (!int.TryParse(GetValue(name), out value))
				throw new ArgumentException(_task.LogError("{0} setting is not valid.", name));

			return value;
		}

		public DateTime GetDateValue(string name, bool usePrefix = true)
		{
			DateTime value;
			if (!DateTime.TryParse(GetValue(name), out value))
				throw new ArgumentException(_task.LogError("{0} setting is not valid.", name));

			return value;
		}

		public Model.ExternalSystem GetExternalSystem(string name, bool usePrefix = true)
		{
			var systemId = GetIntValue(name);
			var criteria = new Model.ExternalSystem { Id = systemId };
			var system = criteria.Get();
			if (system.Failed)
				throw new ArgumentException(_task.LogError("{0} setting is not a valid external system.", name));

			return system.Entity;
		}

		public void SaveValue(string name, string value, bool usePrefix = true)
		{
			if (_dbMode)
			{
				var setting = Get(name, usePrefix);
				if (setting != null)
				{
					new Settings().Set(setting.Id, value);
				}
				else
				{
					throw new ApplicationException(string.Format("{0} setting must already exist", name));
				}
				return;
			}

			var fullName = _task.Profile.ConfigName(name, usePrefix);
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.AppSettings.Settings.Remove(fullName);
			config.AppSettings.Settings.Add(fullName, value);
			config.Save(ConfigurationSaveMode.Modified, false);
			ConfigurationManager.RefreshSection("appSettings");
		}

	}
}
