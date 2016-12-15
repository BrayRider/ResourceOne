<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.Admin.JobModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	R1SM: Job Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <div id="ruletop">
 <% using (Html.BeginForm()) { %>
	<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

	<div class="xboxcontent">

		<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
		<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

		
			<%: Html.ValidationSummary(true) %>
			<fieldset>
			<%: Html.HiddenFor(model => model.Job.JobCode) %>

			<h3>
				<%: Model.Job.JobCode%> - <%: Model.Job.JobDescription %>
			</h3>
		   
			<div class="editor-field">
				Display Description:<%: Html.EditorFor(model => model.Job.DisplayDescription) %>
				<%: Html.ValidationMessageFor(model => model.Job.DisplayDescription) %>
			</div>
			 <div class="editor-field">
				Credentials:<%: Html.EditorFor(model => model.Job.Credentials) %>
				<%: Html.ValidationMessageFor(model => model.Job.Credentials)%>
			</div>
			</fieldset>

			<p>
				<input type="submit" value="Save" />&nbsp;&nbsp;<%: Html.ActionLink("Back to List", "JobCodes") %>
			</p>
	</div>
	<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>
	 <% } %>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
<a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - <a href="<%=Url.Action("JobCodes", "Admin") %>">Job Codes</a> - Job Code Details
</asp:Content>

