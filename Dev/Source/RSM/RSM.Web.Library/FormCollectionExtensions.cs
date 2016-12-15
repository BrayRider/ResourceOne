using System.Web.Mvc;

namespace RSM.Web.Library
{
	public static class FormCollectionExtensions
	{
		public static bool GetValueAsBool(this FormCollection collection, string name)
		{
			return collection.Get(name).ToLower().Contains("t");
		}
	}
}
