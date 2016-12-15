using System.Web.Mvc;

namespace RSM.Web.Library
{
    public static class MvcExtensions
    {
        // Extension method
        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, string controller, object routeValues,
                                                string imagePath, object htmlAttributes)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));

            foreach (var attr in attributes)
            {
                imgBuilder.MergeAttribute(attr.Key, attr.Value.ToString());
            }
            
            var imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, controller, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            var anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }
    }
}
