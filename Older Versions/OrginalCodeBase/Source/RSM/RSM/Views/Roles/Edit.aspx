<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Support.Role>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: Edit Rule
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

    <div id="xsnazzy3">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>Edit Role</h2>
            
            <p>Enter a name and description below to help you identifiy this role.</p>
                  
<% using (Html.BeginForm()) { %>
    <%= Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <input type="hidden" name="gridData" id="gridData" value="" />
    <input type="hidden" name="roleID" value="<%=ViewBag.Role.RoleID%>" />
    <table class="layout">
        <tbody>
            <tr>
                <td>Role:</td><td><%: Html.EditorFor(model => model.RoleName)%></td>
                <td>Description:</td><td><%: Html.EditorFor(model => model.Description) %></td>
            </tr>
            <tr>
            </tr>
        </tbody>
    </table>
    <%: Html.ValidationMessageFor(model => model.RoleName) %>
    <p>&nbsp;</p><p>Double click on a rows to move them between the grids.</p>
    <table class="layout">
    <tbody>
        <tr>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td align="right"><input type="submit" value="Save" /> <%: Html.ActionLink("Cancel", "Details", new { id = Model.RoleID })%></td>
        </tr>
        
            <tr>
                <td>
                    <table id="alist" class="scroll" cellpadding="0" cellspacing="0"></table>
                </td>
                <td>
                <table class="layout">
                    <tbody>
                        <tr>
                            <td><a href='#'><img onclick="AssignLevel()" src="../../Content/images/right_arrow.png" /></a></td>
                        </tr>
                        <tr>
                            <td><a href='#'><img onclick="RemoveLevel()" src="../../Content/images/left_arrow.png" /></a></td>
                        </tr>
                    </tbody>
                </table>
                </td>
                <td>
                    <table id="blist" class="scroll" cellpadding="0" cellspacing="0"></table>
                </td>
            </tr>
        </tbody>
    </table>
       
     <p>
            
        </p>
<% } %>

 </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>
<div>
    
</div>
 

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <%--CSS Files--%>
    <link href="<%=Url.Content("~/Content") %>/jquery-ui-1.8.9.custom.css" rel="stylesheet" type="text/css" />
    <link href="<%=Url.Content("~/Content") %>/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="<%=Url.Content("~/Content") %>/ui.multiselect.css" rel="stylesheet" type="text/css" />
    <script src="<%=Url.Content("~/Scripts") %>/json2.js" type="text/javascript"></script>

    <%--jQuery Library--%>
    <script src="<%=Url.Content("~/Scripts") %>/jquery-1.4.4.min.js" type="text/javascript"></script>
    
    <%--Must load language tag BEFORE script tag--%>
    <script src="<%=Url.Content("~/Scripts") %>/i18n/grid.locale-en.js" type="text/javascript"></script>
   

    <script src="<%=Url.Content("~/Scripts") %>/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts") %>/levelmgmt.js" type="text/javascript"></script>
    <%--jqGrid Code - refer http://www.trirand.com/blog/jqgrid/jqgrid.html --%>
    <script type="text/javascript">
        jQuery(document).ready(function () {


            setupLevelGrid("#alist", "Available", '<%=(string)ViewBag.AvailLevelsURL %>', assignByDoubleClick, function () { });
            $("#alist").loadComplete = function () {
                $("#alist").setGridParam({ datatype: 'local' });
            };

            //setupLevelGrid("#blist", "Assigned", '../../Roles/AssignedLevels/?ID=<%=ViewBag.Role.RoleID%>', removeByDoubleClick);
            setupLevelGrid("#blist", "Assigned", '<%=(string)ViewBag.AssLevelsURL %>', removeByDoubleClick, storeAssignedInForm);
            $("#blist").loadComplete = function () {
                $("#blist").setGridParam({ datatype: 'local' });
            };

        });

       
        

        </script>
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
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "Roles") %>"><span class="label">Roles</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "JCLRule") %>"><span class="label">Rules</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Settings", "Admin") %>"><span class="label">Settings</span></a>
				</li>
			</ol>
		</dd>
	</dl>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - <a href="<%=Url.Action("Index", "Roles") %>">Roles</a> - <%: Html.ActionLink(Model.RoleName, "Details", new { id = Model.RoleID })%> - Edit
</asp:Content>

