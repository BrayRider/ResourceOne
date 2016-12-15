using RSM.Models.Enums;

namespace RSM.Models.People
{
	public class AssociateCollection : ViewModel
	{
		public PagingModel PagingModel { get; set; }

		public bool IsReview { get; set; }

		public ReviewModes ReviewMode { get; set; }

		public string DataAction { get; set; }

		public string ReviewAction { get; set; }

		public string AreaTitle { get; set; }

		public string AreaMessage { get; set; }

		public string GridCaption { get; set; }

		public string CurrentMenuItem { get; set; }

		public AssociateCollection()
		{
		}
	}
}