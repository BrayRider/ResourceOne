using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace RSM.Service
{
	static class Program
	{
		private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			foreach(var arg in args)
			{
				if (arg.Equals("/install", StringComparison.InvariantCultureIgnoreCase))
				{
					InstallMe();
					return;
				}
				else if (arg.Equals("/uninstall", StringComparison.InvariantCultureIgnoreCase))
				{
					UninstallMe();
					return;
				}
			}

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new RSMService() 
			};
			ServiceBase.Run(ServicesToRun);
		}

		public static bool InstallMe()
		{
			try
			{
				ManagedInstallerClass.InstallHelper(new string[] { _exePath });
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static bool UninstallMe()
		{
			try
			{
				ManagedInstallerClass.InstallHelper(new string[] { "/u", _exePath });
			}
			catch
			{
				return false;
			}
			return true;
		}

	}

}
