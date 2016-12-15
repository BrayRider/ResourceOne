<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: Roles
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="xsnazzy2">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4">
        </b></b>
        <div class="xboxcontent">
            <h2>
                Role Definition</h2>
            <p>
                Roles allow you to group access levels into groups based on activities.</p>
            <p>
                Double click on a row to view the details for that role.</p>
            <p align="right">
                <%: Html.ActionLink("Create New Role", "Create") %></p>
            <center>
                <table id="list" class="scroll" cellpadding="0" cellspacing="0">
                </table>
                <div id="pager" class="scroll" style="text-align: center;">
                </div>
            </center>
        </div>
        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1">
        </b></b>
    </div>
    <%-- HTML Required--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <%--CSS Files--%>
    <link href="<%=Url.Content("~/Content") %>/jquery-ui-1.8.9.custom.css" rel="stylesheet"
        type="text/css" />
    <link href="<%=Url.Content("~/Content") %>/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="<%=Url.Content("~/Content") %>/ui.multiselect.css" rel="stylesheet" type="text/css" />
    <%--jQuery Library--%>
    <script src="<%=Url.Content("~/Scripts") %>/jquery-1.4.4.min.js" type="text/javascript"></script>
    <%--Must load language tag BEFORE script tag--%>
    <script src="<%=Url.Content("~/Scripts") %>/i18n/grid.locale-en.js" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts") %>/jquery.jqGrid.min.js" type="text/javascript"></script>
    <%--jqGrid Code - refer http://www.trirand.com/blog/jqgrid/jqgrid.html --%>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            jQuery("#list").jqGrid({
                url: '<%=Url.Action("RoleList", "Roles") %>',

                datatype: 'json',
                mtype: 'POST',
                height: 300,
                altRows: true,
                altclass: 'ui-widget-contentalt',
                ondblClickRow: gridDoubleClick,
                colNames: ['ID', 'Role Name', 'Description'],
                colModel: [
                          { name: 'ID', index: 'ID', width: 0, align: 'left', key: true, hidden: true, search: false },
                          { name: 'Role', index: 'Name', width: 300, align: 'left', searchoptions: { sopt: ['cn']} },
                          { name: 'Description', index: 'Description', width: 400, align: 'left', searchoptions: { sopt: ['cn']} }
                          ],
                pager: jQuery('#pager'),
                rowNum: 20,
                rowList: [20, 40, 80, 100],
                sortname: 'Name',
                sortorder: "asc",
                viewrecords: true,
                imgpath: '../../Content/images',
                caption: 'Roles'
            }).navGrid('#pager', { search: true, edit: false, add: false, del: true, searchtext: "Search" }, {}, {}, { url: "/Roles/Delete/" });
        });


        function gridDoubleClick(rowid, iRow, iCol, e) {
            var row = jQuery(this).jqGrid('getRowData', rowid);
            window.location = '<%=Url.Action("Details", "Roles") %>/' + row.ID;
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
    <dl class="sectionFeeds">
        <dt id="SectionsHeader" class="content">
            <div class="label">
                Menu</div>
        </dt>
        <dd id="SectionsContent" class="content" style="display: block; height: 293px;">
            <ol>
                <li><a href="<%=Url.Action("Index", "Home") %>"><span class="label">Home</span></a>
                </li>
                <li><a href="<%=Url.Action("Index", "People") %>"><span class="label">Associates</span></a>
                </li>
                <li><a href="<%=Url.Action("Index", "Admin") %>"><span class="label">Admin</span></a>
                </li>
                
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("ActivityLog", "Admin") %>"><span class="label">Activity
                    Log</span></a> </li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("JobCodes", "Admin") %>"><span class="label">Job Codes</span></a>
				</li>
                <li class="selected">&nbsp;&nbsp;<a href="<%=Url.Action("Index", "Roles") %>"><span
                    class="label">Roles</span></a> </li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Index", "JCLRule") %>"><span class="label">Rules</span></a>
                </li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Reports", "Admin") %>"><span class="label">Reports</span></a> </li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Settings", "Admin") %>"><span class="label">Settings</span></a>
                </li>
            </ol>
        </dd>
    </dl>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Roles
</asp:Content>
