<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: Rules
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


<div id="xsnazzy2">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>Role Mapping Rules</h2>
            
            <p>The list below contains the rules for mapping jobs, departments and facilities to security access roles. &nbsp;Double click on a row to view the details for that rule.</p>
            <p align='right'><%: Html.ActionLink("Create New Rule", "Create") %></p>
            <center>
                <%-- HTML Required--%>
                <table id="list" class="scroll" cellpadding="0" cellspacing="0"></table>
                <div id="pager" class="scroll" style="text-align:center;"></div>
    
            </center>

        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>

    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
<%--CSS Files--%>
    <link href="<%=Url.Content("~/Content") %>/jquery-ui-1.8.9.custom.css" rel="stylesheet" type="text/css" />
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
                url: '../JCLRule/Search/',
                datatype: 'json',
                mtype: 'POST',
                height: 325,
                altRows: true,
                altclass:'ui-widget-contentalt',
                ondblClickRow: gridDoubleClick,
                colNames: ['ID', 'Position', 'Department', 'Facility'],
                colModel: [
                          { name: 'ID', index: 'ID', width: 0, key:true, align: 'left', hidden: true, search: false },
                          { name: 'Position', index: 'Position', width: 250, align: 'left', formatter: formatRule, searchoptions: { sopt: ['cn']} },
                          { name: 'Department', index: 'Department', width: 275, align: 'left', formatter: formatRule, searchoptions: { sopt: ['cn']} },
                          { name: 'Facility', index: 'Facility', width: 175, align: 'left', formatter: formatRule, searchoptions: { sopt: ['cn']} },
                          
                          ],
                pager: jQuery('#pager'),
                rowNum: 20,
                rowList: [20, 40, 80, 100, 500],
                sortname: 'Position',
                sortorder: "asc",
                viewrecords: true,
                imgpath: '../../Content/images',
                caption: 'Role Mapping Rules'
            }).navGrid('#pager', { search: true, edit: false, add: false, del: true, searchtext: "Search" }, {}, {}, { url: "/JCLRule/Delete/" });
        });

        String.prototype.startsWith = function (str) {
            return (this.substr(0, str.length) === str);           
        }

        function gridDoubleClick(rowid, iRow, iCol, e) {
            var row = jQuery(this).jqGrid('getRowData', rowid);
            window.location = '../JCLRule/Edit/' + row.ID;
        }


        function formatRule(cellValue, mask, rowObject) {
            var isGeneric = (cellValue.substr(0, 2) == '--');
            if (isGeneric) {
                return "<span class='ruleAny'>" + cellValue + "</span>";
            }


            return "<span class='ruleSpecific'>" + cellValue + "</span>"; ;
        }
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
    <a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Rules
</asp:Content>
