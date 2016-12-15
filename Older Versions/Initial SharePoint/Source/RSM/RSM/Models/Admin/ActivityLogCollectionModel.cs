using System.Collections.Generic;
using System.Web.Mvc;
using RSM.Support;

namespace RSM.Models.Admin
{
	public class ActivityLogCollectionModel : ViewModel
	{
		public string System { get; set; }

		public SelectList SystemCollection { get; set; }

		public string SystemFilters { get; set; }
	}
}