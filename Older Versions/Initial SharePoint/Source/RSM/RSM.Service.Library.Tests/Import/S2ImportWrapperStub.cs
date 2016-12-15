

namespace RSM.Service.Library.Tests.Import
{
	public class S2ImportTaskWrapper : RSM.Integration.S2.Import.AccessHistory
	{
		public void TestWrapper(object stateinfo)
		{
			ExecuteWrapper(stateinfo);

			Stop();
		}
	}
}
