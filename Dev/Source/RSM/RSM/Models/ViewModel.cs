namespace RSM.Models
{
    public class ViewModel
    {
    	public string PageTitle { get; set; }

        public bool IsAdmin { get; set; }

        public MenuCollection SidebarMenu { get; set; }
    }
}