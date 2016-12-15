using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace RSM.Service
{
    [RunInstaller(true)]
    public class RSMInstaller : Installer
    {
        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;
        public RSMInstaller()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "R1SM";
            serviceInstaller.Description = "Performs import / export operations for R1SM.";
            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }
    }
}