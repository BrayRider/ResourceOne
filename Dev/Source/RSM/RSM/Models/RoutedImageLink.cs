
namespace RSM.Models
{
    public class RoutedImageLink
    {
        public string ImageSrc { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public object RouteValuesCollection { get; set; }
        public object ImageHtmlValuesCollection { get; set; }
    }
}