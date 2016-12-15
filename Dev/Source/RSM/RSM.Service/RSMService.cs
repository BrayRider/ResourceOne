using System.Configuration;
using System.ServiceProcess;

using RSM.Service.Library;
using System;

namespace RSM.Service
{
	public partial class RSMService : ServiceBase
	{
		public ServiceProfile Profile { get; set; }

		public RSMService()
		{
			InitializeComponent();

			Profile = new ServiceProfile();
			Profile.Service = this;
			this.ServiceName = Profile.Name;
			this.CanStop = true;
			this.CanPauseAndContinue = true;
			this.AutoLog = true;

		}

		#region Service Overrides
		protected override void OnStart(string[] args)
		{
			try
			{
				Profile.Load(args, service: this);
				Profile.Tasks.ForEach(t => t.Start(args));
				base.OnStart(args);
			}
			catch
			{
				ExitCode = 1;
				throw;
			}
		}

		protected override void OnPause()
		{
			Profile.Tasks.ForEach(t => t.Pause());
			base.OnPause();
		}

		protected override void OnContinue()
		{
			Profile.Tasks.ForEach(t => t.Continue());
			base.OnContinue();
		}

		protected override void OnStop()
		{
			Profile.Tasks.ForEach(t => t.Stop());
			ExitCode = 0;
		}

		protected override void OnShutdown()
		{
			Profile.Tasks.ForEach(t => t.Shutdown());
			base.OnShutdown();
		}
		#endregion

		#region Helpers
		#endregion
	}
}
