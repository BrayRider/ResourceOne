
namespace RSM.Models
{
    public class RoutedLink
    {
        public string Text { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public object RouteValuesCollection { get; set; }
    }
}