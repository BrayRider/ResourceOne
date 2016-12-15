<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Support.Job>" %>

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
            <%: Html.HiddenFor(model => model.JobCode) %>

            <h3>
                <%: Model.JobCode%> - <%: Model.JobDescription %>
            </h3>
           
            <div class="editor-field">
                Display Description:<%: Html.EditorFor(model => model.DisplayDescription) %>
                <%: Html.ValidationMessageFor(model => model.DisplayDescription) %>
            </div>
             <div class="editor-field">
                Credentials:<%: Html.EditorFor(model => model.Credentials) %>
                <%: Html.ValidationMessageFor(model => model.Credentials)%>
            </div>

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
                <li class="selected">
					&nbsp;&nbsp;<a href="<%=Url.Action("JobCodes", "Admin") %>"><span class="label">Job Codes</span></a>
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
<a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - <a href="<%=Url.Action("JobCodes", "Admin") %>">Job Codes</a> - Job Code Details
</asp:Content>

