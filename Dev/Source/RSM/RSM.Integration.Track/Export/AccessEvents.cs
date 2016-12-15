using System.Linq;
using RSM.Service.Library;
using RSM.Service.Library.Interfaces;
using RSM.Service.Library.Model;
using RSM.Service.Library.Controllers;
using RSM.Service.Library.Export;
using AccessEventsBase = RSM.Service.Library.Export.AccessEvents;
using RSM.Service.Library.Model;

namespace RSM.Integration.Track.Export
{
	public class AccessEvents : AccessEventsBase
	{
		public new class Config : AccessEventsBase.Config
		{
			public static string EventCodeName = "EventCode";
			public static string SysIdName = "SysId";
			public static string DataSourceName = "DataSource";

			public int EventCode { get; set; }
			public int SysId { get; set; }
			public string DataSource { get; set; }

			public Config(AccessEvents task) : base(task)
			{ }

			public override Result<Task.Config> Load()
			{
				var result = base.Load();

				var settings = new TaskSettings(Task);

				EventCode = settings.GetIntValue(Config.EventCodeName);
				SysId = settings.GetIntValue(Config.SysIdName);
				DataSource = settings.GetValue(Config.DataSourceName);

				result.Entity = this;

				return result;
			}
		}

		public AccessEvents()
			: base()
		{
			ExternalSystem = RSM.Service.Library.Model.ExternalSystem.TrackOut;
		}

		protected override Result<Task.Config> LoadConfig()
		{
			return new Config(this).Load(); //Create config specific to the Track export
		}

		protected override IAPI CreateAPI(Task.Config config)
		{
			var api = base.CreateAPI(config);

			if (api != null)
			{
				//This API needs access to the config
				(api as API).Config = config as Config;
			}
			return api;
		}

        public override bool Filter(AccessLog log)
        {
            if (string.IsNullOrWhiteSpace(log.Person.ExternalId) ||
                string.IsNullOrWhiteSpace(log.Portal.ExternalId) ||
                string.IsNullOrWhiteSpace(log.Reader.ExternalId))
            {
                LogError("cannot process Access record {0}. It has an invalid ID. Person ({1}), Portal ({2}), Reader({3})", log.ExternalId, log.Person.ExternalId, log.Portal.ExternalId, log.Reader.ExternalId);
                return false;
            }

            if (log.Person == null || log.Person.InternalId == 0) return true;

            //Only contractor activity matters. Any value in UDF4 means it is contractor
            return !string.IsNullOrWhiteSpace(log.Person.udf4);
        }

	}
}
