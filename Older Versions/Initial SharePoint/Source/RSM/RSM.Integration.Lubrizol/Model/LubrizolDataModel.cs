using System.Configuration;
using RSM.Artifacts;

namespace RSM.Integration.Lubrizol.Model
{
	partial class LubrizolDataModelDataContext
	{
		public LubrizolDataModelDataContext()
			: base(ConfigurationManager.ConnectionStrings[Constants.LubrizolConnectionStringName].ConnectionString)
		{
		}

		partial void OnCreated()
		{
		}
	}
}
