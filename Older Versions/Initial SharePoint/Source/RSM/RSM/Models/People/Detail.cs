using RSM.Models.Enums;
using RSM.Support;

namespace RSM.Models.People
{
	public class Detail : ViewModel
	{
		public Person Person { get; set; }

		public string PictureUrl { get; set; }

		public bool AllowRuleAdministration { get; set; }

		public string AssignedRolesUrl { get; set; }

		public bool IsReview { get; set; }

		public bool NeedsApproval { get; set; }

		public string ReturnView { get; set; }

		public string ReturnStatus { get; set; }

		public string ReturnUrl { get; set; }

		public string BackView { get; set; }

		public ReviewModes ReviewMode { get; set; }

		public string BreadcrumbStatus { get; set; }

		public string BreadcrumbText { get; set; }
	}
}