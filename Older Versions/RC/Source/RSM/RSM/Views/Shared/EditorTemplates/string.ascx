<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
   var htmlAttributes = new HtmlPropertiesAttribute();
   if(ViewData.ModelMetadata.AdditionalValues.ContainsKey("HtmlAttributes"))
      htmlAttributes = (HtmlPropertiesAttribute) ViewData.ModelMetadata.AdditionalValues["HtmlAttributes"];
   htmlAttributes.HtmlAttributes().Add("class", "text-box single-line " + htmlAttributes.CssClass);
 %>
    <span>
          
          <%=Html.TextBox("", ViewData.TemplateInfo.FormattedModelValue,htmlAttributes.HtmlAttributes()) %>                                      
   </span>
   

