﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   R1SM: Activity Log
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="xsnazzy4">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>Activity Log</h2>
            
            <p>The grid below contains activities performed by the Resource One Security Manager.</p>
            <p>Double click on a row to view additional details.</p>
            <p>Show: <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=-1">All</a> | <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=0">S2 Import</a> | <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=1">S2 Export</a> | <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=2">Associate Import</a> | <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=3">User Activity</a></p>
            <center>
                 <table id="list" class="scroll" cellpadding="0" cellspacing="0"></table>
                  <div id="pager" class="scroll" style="text-align:center;"></div>
              </center>  
            
            
            <table class="marginlayout">
            <tr>
               <td align='center'>
                   
                   <span class="smalltext">Bold items indicate items that may require your attention.</span>
                </td>
               <td align='center'>
                   
                   <span class="smalltext">Red items indicate an error or failure.</span>
                </td>
               
            </tr>
          
            </table>
            
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
                url: '/Admin/SysLog?Filter=<%: ViewBag.Filter.ToString() %>',
                datatype: 'json',
                mtype: 'POST',
                height: 400,
                altRows: true,
                altclass: 'ui-widget-contentalt',
                colNames: ['ID', 'Severity', 'Date', 'Source', 'Message'],
                colModel: [
                          { name: 'ID', index: 'ID', width: 0, align: 'left', hidden: true, search: false },
                          { name: 'Severity', index: 'Severity', width: 0, align: 'left', hidden: true, search: false },
                          { name: 'Date', index: 'Date', width: 150, align: 'left', formatter: formatSeverity, search: false },

                          { name: 'Source', index: 'Source', width: 125, formatter: formatSeverity, align: 'left' },
                          { name: 'Message', index: 'Message', width: 600, formatter: formatSeverity, align: 'left' }


                          ],
                pager: jQuery('#pager'),
                rowNum: 20,
                ondblClickRow: gridDoubleClick,
                rowList: [20, 40, 80, 100],
                sortname: 'Date',
                sortorder: "desc",
                viewrecords: true,
                imgpath: '../../Content/images',
                caption: 'Activity Log'
            }).navGrid('#pager', { search: false, edit: false, add: false, del: false, searchtext: "Search" });
        });


        function gridDoubleClick(rowid, iRow, iCol, e) {
            var row = jQuery(this).jqGrid('getRowData', rowid);
            window.location = '../../Admin/EventDetails/' + row.ID + "?Filter=<%: ViewBag.Filter.ToString() %>";
        }
        function formatSeverity(cellValue, mask, rowObject) {
            var sev = parseInt(rowObject[1]);
            if (sev == 0) {
                return cellValue;
            }
            if (sev == 1) {
                return "<span class='sevWarn'>" + cellValue + "</span>";
            }

            if (sev == 2) {
                return "<span class='sevError'>" + cellValue + "</span>";
            }


            return cellValue;
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
                <li class="selected">
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
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("Settings", "Admin") %>"><span class="label">Settings</span></a>
				</li>
			</ol>
		</dd>
	</dl>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Activity Log
</asp:Content>