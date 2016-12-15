using RSM.Models.Enums;

namespace RSM.Models.Admin
{
	public class JobCodeCollection : ViewModel
	{
		public PagingModel PagingModel { get; set; }

		public string DataAction { get; set; }

		public string ReviewAction { get; set; }

		public string GridCaption { get; set; }

		public JobCodeCollection()
		{
		}
	}
}