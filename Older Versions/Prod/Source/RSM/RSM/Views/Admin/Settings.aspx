<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.SettingsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: Settings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">



<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true) %>

    <div id="xsnazzy">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>R1SM Settings</h2>
            <p>The settings below control how the Resource One Security Manager operates.</p>
            <table class="layout">
                <tbody>
                    <tr>
                        <td><%: Html.EditorFor(model => model.RuleEngineAllow) %>Allow the R1SM rule engine to assign roles.</td>
                        <td><%: Html.EditorFor(model => model.JobCodesFirst) %>Show job codes before job titles when editing rules.</td>
                       <!-- <td><%: Html.EditorFor(model => model.RequireApproval) %>Require approval of changes made by the rule engine</td> -->
                        
                    </tr>
                    
                    <tr>
                        <td>New Admin Password:<%: Html.PasswordFor(model => model.AdminPass) %></td>
                    </tr>
                </tbody>
            </table>
            <%: Html.ValidationMessageFor(model => model.AdminPass)%>
          
       
        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>



    <div id="xsnazzy">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>S2 Settings</h2>
            <p>The settings below control how the Resource One Security Manager interfaces with your S2 security appliance.</p>
            <table class="layout">
                <tbody>
                    <tr>
                        <td><%: Html.EditorFor(model => model.S2AllowImport) %>Allow importing of levels from S2.</td>
                        <td><%: Html.EditorFor(model => model.S2AllowExport) %>Allow exporting of user data and roles to S2.</td>
                        
                    </tr>
                </tbody>
            </table>
            
          
          <table class="layout">
            <tbody>
            <tr>
                <td>Appliance Address:</td><td><%: Html.EditorFor(model => model.S2Address) %></td>
            </tr>
            <tr>
                <td align="right">R1SM S2 User ID:</td>
                <td><%: Html.EditorFor(model => model.S2RSMAccountName) %></td>
                <td>New R1SM S2 Password:</td>
                <td><%: Html.PasswordFor(model => model.S2RSMAccountPassword)%></td>
            </tr>
            </tbody>
          </table>
          <%: Html.ValidationMessageFor(model => model.S2Address)%><%: Html.ValidationMessageFor(model => model.S2RSMAccountName)%><%: Html.ValidationMessageFor(model => model.S2RSMAccountPassword)%>
        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>

    <div id="xsnazzy">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>Associate Import Settings</h2>
            <p>The settings below control how the Resource One Security Manager imports associate information.</p>
            <table class="layout">
                <tbody>
                    <tr>
                        <td><%: Html.EditorFor(model => model.PSAllowImport) %>Allow R1SM to import associate information.</td>
                    </tr>
                </tbody>
            </table>
            <table class="layout">
                <tbody>
                    <tr>
                        <td>"Green"Import Folder:</td><td><%: Html.EditorFor(model => model.GreenFolder) %></td>
                        <td align="right">"Red"Import Folder:</td><td><%: Html.EditorFor(model => model.RedFolder) %></td>
                    </tr>
                </tbody>
            </table>
            <%: Html.ValidationMessageFor(model => model.GreenFolder)%><%: Html.ValidationMessageFor(model => model.RedFolder)%>
          
       
        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>

    <p align="right"><input type="submit" value="Save" />&nbsp;&nbsp;<%: Html.ActionLink("Cancel", "Index") %></p>

  <% } %>

<div>
    
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
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("JobCodes", "Admin") %>"><span class="label">Job Codes</span></a>
				</li>
                 <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "Roles") %>"><span class="label">Roles</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "JCLRule") %>"><span class="label">Rules</span></a>
				</li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Reports", "Admin") %>"><span class="label">Reports</span></a> </li>
                <li class="selected">
					&nbsp;&nbsp;<a href="<%=Url.Action("Settings", "Admin") %>"><span class="label">Settings</span></a>
				</li>
			</ol>
		</dd>
	</dl>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Settings
</asp:Content>
