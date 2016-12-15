<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.Settings.SettingsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	R1SM: Settings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% using (Html.BeginForm()) { %>
	<%: Html.ValidationSummary(true) %>
	
	<% if (Model.GroupingCollection.R1SMGroup != null) {%>
	<%: Html.Partial("_SystemSettings", Model.GroupingCollection.R1SMGroup) %>
	<% } %>
	
	<% foreach (var grouping in Model.GroupingCollection.Groupings)
	   { %>
		   <%: Html.Partial("_SystemSettings", grouping) %>
	   <%} %>
		   <p align="right"><input type="submit" value="Save" />&nbsp;&nbsp;<%: Html.ActionLink("Cancel", "Index") %></p>
	   <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
	<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
	<a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Settings
</asp:Content>
