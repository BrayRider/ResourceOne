using System.Collections.Generic;

namespace RSM.Models
{
    public class MenuCollection
    {
        public string Title { get; set; }

        public string SelectedMenuText { get; set; }

        public List<MenuRoutedLink> LinkCollection { get; set; }
    }
}