using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RSM.Artifacts;
using RSM.Service.Library.Interfaces;
using RSMDB = RSM.Support;

namespace RSM.Service.Library.Tests.Export
{
	public class TrackExportWrapperStub : RSM.Integration.Track.Export.AccessEvents
	{
		public IAPI LoadApiTest()
		{
			var load = LoadConfig();
			if (load.Failed)
				throw new ApplicationException(load.ToString());

			var config = load.Entity as Config;

			return CreateAPI(config);
		}

		public void TestWrapper(object stateinfo)
		{
			ExecuteWrapper(stateinfo);

			Stop();
		}
	}
}
