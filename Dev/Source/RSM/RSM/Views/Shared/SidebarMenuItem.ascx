<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RSM.Models.MenuRoutedLink>" %>

<%
for (var i = 0; i < Model.Indented; i++)
{
	%>&nbsp;<% 

} 
%><a href="<%: Url.Action(Model.Action, Model.Controller, Model.RouteValuesCollection) %>"><span class="label"><%: Model.Text %></span></a>