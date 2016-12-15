using System.Configuration;

namespace TestSupport
{
	public class Configuration
	{
		public static void SaveConfigValue(string key, string value)
		{
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var settings = config.AppSettings.Settings;

			if (settings[key] == null)
			{
				settings.Add(key, value);
			}
			else
			{
				settings[key].Value = value;
			}

			config.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
		}
	}
}
