<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Support.JCLRoleRule>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: Create Rule
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>

<% using (Html.BeginForm()) { %>
    <%: Html.ValidationSummary(true) %>
    <%: Html.AntiForgeryToken() %>
    <%: Html.HiddenFor(model => model.ID) %>
    <input type="hidden" name="gridData" id="gridData" value="" />

     <div id="ruletop">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>Role Assignment Rule</h2>
            
            <p>Using the controls below select the job, department and facility this rule should apply to.</p>

        <table class="layout">
            <tbody>
                <tr>
                    <td>
                        Job:
                    </td>
                    <td>
                        <% if (ViewBag.JobCodesFirst == true) { %>
                            <%: Html.DropDownListFor(model => model.JobCode,
                                                        new SelectList(ViewBag.Jobs, "JobCode", "DisplayNameCodeFirst"))%>
                        <%} else { %>
                           <%: Html.DropDownListFor(model => model.JobCode,
                                                        new SelectList(ViewBag.Jobs, "JobCode", "DisplayName"))%>
                        <%} %>
                    </td>
                    <td>
                        Dept:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.DeptID,
                                             new SelectList(ViewBag.Depts, "DeptID", "DeptDescr", Model.DeptID.ToString()))%>
                    </td>
                    <td>
                        Facility:
                    </td>
                    <td>
                        <%: Html.DropDownListFor(model => model.Location,
                                             new SelectList(ViewBag.Locs, "LocationID", "LocationName", Model.Location))%>
                    </td>
                </tr>
                
            </tbody>
        </table>
        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>
        <p align=right><input type="submit" value="Create" />&nbsp;&nbsp;<%: Html.ActionLink("Back to Rules", "Index") %></p>
    </div>
     <div id="rulebottom">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>Role Selection</h2>
            <p>Using the controls below, assign roles that should be applied to associates who match the rule.</p>
            <p>Double click on a row to assign or remove a role.</p>
            <center>
         <table class="layout">
        <tbody>
            <tr>
                <td>
                    <table id="alist" class="scroll" cellpadding="0" cellspacing="0"></table>
                </td>
                <td>
                <table class="layout">
                    <tbody>
                        <tr>
                            <td><a href='#'><img alt="Assign" onclick="AssignLevel()" src="../../Content/images/right_arrow.png" /></a></td>
                        </tr>
                        <tr>
                            <td><a href='#'><img alt="Remove" onclick="RemoveLevel()" src="../../Content/images/left_arrow.png" /></a></td>
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
    </center>
     </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>


    </fieldset>
<% } %>

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
    <script src="<%=Url.Content("~/Scripts") %>/rolemgmt.js" type="text/javascript"></script>
    <%--jqGrid Code - refer http://www.trirand.com/blog/jqgrid/jqgrid.html --%>
    <script type="text/javascript">
        jQuery(document).ready(function () {


            setupRoleGrid("#alist", "Available", '<%=(string)ViewBag.AvailRolesURL %>', assignByDoubleClick, function () { });
            $("#alist").loadComplete = function () {
                $("#alist").setGridParam({ datatype: 'local' });
            };


            setupRoleGrid("#blist", "Assigned", '<%=(string)ViewBag.AssRolesURL %>', removeByDoubleClick, storeAssignedInForm);
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
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("JobCodes", "Admin") %>"><span class="label">Job Codes</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Index", "Roles") %>"><span class="label">Roles</span></a>
				</li>
                <li class="selected">
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
<a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - <a href="../">Rules</a> - Create
</asp:Content>

