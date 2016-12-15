
namespace RSM.Web.Library
{
	public static class ValueTypes
	{
		public static string ToString(this bool value)
		{
			return value ? "true" : "false";
		}
	}
}
