using System.ComponentModel;
using System.Configuration;
using System.ServiceProcess;

using RSM.Service.Library;

namespace RSM.Service
{
	[RunInstaller(true)]
	public partial class RSMInstaller : System.Configuration.Install.Installer
	{
		private ServiceProcessInstaller processInstaller;
		private ServiceInstaller serviceInstaller;

		public RSMInstaller()
		{
			InitializeComponent();
			processInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();

			var profile = new ServiceProfile();

			processInstaller.Account = ServiceAccount.LocalSystem;
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			serviceInstaller.ServiceName = profile.Name;
			serviceInstaller.Description = profile.Description;

			Installers.Add(serviceInstaller);
			Installers.Add(processInstaller);
		}
	}
}
