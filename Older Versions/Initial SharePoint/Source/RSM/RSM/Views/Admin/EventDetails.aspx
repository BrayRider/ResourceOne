<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.Admin.LogEntryModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Model.PageTitle %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="xsnazzy">
		<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

		<div class="xboxcontent">
			<h2>Event Details</h2>
			<table class="layout">
				<tbody>
					<tr>
							<td align="right"><div class="display-label">Date:</div></td>
							<td><div class="display-field"><%: String.Format("{0:g}", Model.EventDate) %></div></td>
							<td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
							<td align="right"><div class="display-label">Source:</div></td>
							<td><div class="display-field"><%: Model.SourceName %></div></td>
							<td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
							<td align="right"><div class="display-label">Severity:</div></td>
							<td><div class="display-field"><%: Model.SeverityName %></div></td>
					</tr>
				 </tbody>
			</table>
			<p>&nbsp;</p>
			<div class="display-label">Message:</div>
			<div class="display-field"><%: Model.Message %></div>
			<p>&nbsp;</p>
			<% if (Model.ShowDetails)
			   { %>
				<div class="display-label">Additional Information:</div>
				<div class="display-field"><%: Model.Details %></div>
				<p>&nbsp;</p>
				<%} %>
			<p align=right>

				<%: Html.ActionLink("Back to Activity Log", "ActivityLog", "Admin", new { id = Model.SystemFilter }, null ) %>
			</p>
		</div>


		<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

	</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
	<a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - <%: Html.ActionLink("Activity Log", "ActivityLog", new { id = Model.SystemFilter })%> - Event Details
</asp:Content>

