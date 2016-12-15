<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Support.LogEntry>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    EventDetails
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
            <% if (Model.Details.Length > 0)
               { %>
                <div class="display-label">Additional Information:</div>
                <div class="display-field"><%: Model.Details %></div>
                <p>&nbsp;</p>
                <%} %>
            <p align=right>

                <%: Html.ActionLink("Back to Activity Log", "ActivityLog", new  { filter = ViewBag.Filter })%>
            </p>
        </div>


        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>


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
					&nbsp;&nbsp;<a href="<%=Url.Action("JobCodes", "Admin") %>"><span class="label">Job Codes</span></a>
				</li>
                <li class="selected">
					&nbsp;&nbsp;<a href="<%=Url.Action("ActivityLog", "Admin") %>"><span class="label">Activity Log</span></a>
				</li>
                 <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "Roles") %>"><span class="label">Roles</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "JCLRule") %>"><span class="label">Rules</span></a>
				</li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Reports", "Admin") %>"><span class="label">Reports</span></a> </li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Settings", "Admin") %>"><span class="label">Settings</span></a>
				</li>
			</ol>
		</dd>
	</dl>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - <%: Html.ActionLink("Activity Log", "ActivityLog", new  { filter = ViewBag.Filter })%> - Event Details
</asp:Content>

