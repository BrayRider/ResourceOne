<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: Reports
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<p> Select a report from the dropdown below and click the download button.</p>
<% Html.BeginForm(); %>
    <%=Html.DropDownList("ReportName",(SelectList)ViewBag.ReportList)%> <input name='download'  type='submit' value="Download Report"/>

<% Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
<dl class="sectionFeeds">
		<dt id="SectionsHeader" class="content">
			<div class="label">Menu</div>
		</dt>
			
		<dd id="SectionsContent" class="content" style="display: block; height: 293px; ">
			<ol>
				<li>
				
					<a href="<%=Url.Action("Index", "Home") %>"><span class="label">Home</span></a>
				</li>
         		<li>
					<a href="<%=Url.Action("Index", "People") %>"><span class="label">Associates</span></a>
				</li>
				<li>
					<a href="<%=Url.Action("Index", "Admin") %>"><span class="label">Admin</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("ActivityLog", "Admin") %>"><span class="label">Activity Log</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("JobCodes", "Admin") %>"><span class="label">Job Codes</span></a>
				</li>
                 <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "Roles") %>"><span class="label">Roles</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "JCLRule") %>"><span class="label">Rules</span></a>
				</li>
                <li class="selected">&nbsp;&nbsp;<a href="<%=Url.Action("Reports", "Admin") %>"><span class="label">Reports</span></a> </li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Settings", "Admin") %>"><span class="label">Settings</span></a>
				</li>
			</ol>
		</dd>
	</dl>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Reports
</asp:Content>
