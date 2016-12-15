<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: Associates
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="xsnazzy">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2>Associates</h2>
            
            <p>The grid below contains all associates that have been imported.</p>
            <p>Double click on a row to view the details for that person. Click the search button on the bottom left of the grid to find an associate.</p>
            
            <center>
                <table id="list" class="scroll" cellpadding="0" cellspacing="0"></table>
                <div id="pager" class="scroll" style="text-align:center;"></div>
                
            </center>

            <table class="marginlayout">
            <tr>
               <td align='center'>
                   <img src='<%=Url.Content("~/Content") %>/images/icon_active_new.gif' alt='active' title='Active Associate' /> 
                   <span class="smalltext">Active Associate</span>
                </td>
               <td align='center'>
                   <img src='<%=Url.Content("~/Content") %>/images/icon_inactive_new.gif' alt='inactive' title='Inactive Associate' /> 
                   <span class="smalltext">Inactive Associate</span>
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
                url: '<%=Url.Action("Index", "People") %>/EmployeeListSearch/',
                datatype: 'json',
                mtype: 'POST',
                height: 400,
                altRows: true,
                altclass: 'ui-widget-contentalt',
                ondblClickRow: gridDoubleClick,
                colNames: ['Status', 'Name', 'Badge #', 'Department', 'Job', 'Facility', 'Last Updated', 'UpdateMask', 'ID'],
                colModel: [
                          { name: 'Status', index: 'Status', width: 45, align: 'center', formatter: formatStatus, search: false },
                          { name: 'Name', index: 'Name', width: 150, align: 'left', searchoptions: { sopt: ['cn']} },
                          { name: 'BadgeNumber', index: 'BadgeNumber', width: 80,  align: 'right', formatter: formatBadge,   search: false },
                          { name: 'Department', index: 'Department', width: 190, align: 'left', searchoptions: { sopt: ['cn']} },
                          { name: 'Job', index: 'Job', width: 185, align: 'left', searchoptions: { sopt: ['cn']} },
                          { name: 'Facility', index: 'Facility', width: 150, align: 'left', searchoptions: { sopt: ['cn']} },
                          { name: 'LastUpdated', index: 'LastUpdated', width: 110, align: 'left', search: false },
                          { name: 'ChangeMask', index: 'ChangeMask', width: 0, align: 'left', hidden: true, search: false },
                          { name: 'ID', index: 'ID', width: 0, align: 'left', hidden: true, search: false }
                          ],
                pager: jQuery('#pager'),
                rowNum: <%: ViewBag.PageSize.ToString() %>,
                rowList: [25, 50, 100, 200],
                sortname: 'Name',
                sortorder: "asc",
                viewrecords: true,
                imgpath: '<%=Url.Content("~/Content") %>/images',
                caption: 'All Associates'
            }).navGrid('#pager', { search: true, edit: false, add: false, del: false, searchtext: "Search" });
        });


        function gridDoubleClick(rowid, iRow, iCol, e) {
            var row = jQuery(this).jqGrid('getRowData', rowid);
            window.location='<%=Url.Action("Index", "People") %>/Details/' + row.ID;
        }

        function formatStatus(cellvalue, options, rowObject) {
            var changed = parseInt(rowObject[7]);
            if (cellvalue == "True") {
                //if (changed & 0x10) {

                    return "<img src='<%=Url.Content("~/Content") %>/images/icon_active_new.gif' alt='active' title='Active Associate' />";
                //}
                //return "<img src='<%=Url.Content("~/Content") %>/images/icon_active.png' alt='active' title='Active Associate' />";
            }
            else {
                //if (changed & 0x10) {
                    return "<img src='<%=Url.Content("~/Content") %>/images/icon_inactive_new.gif' alt='Inactive' title='Inactive Associate' />";
                //}
                //return "<img src='<%=Url.Content("~/Content") %>/images/icon_inactive.png' alt='Inactive' title='Inactive Associate' />";
            }

        };


        function boldByMask(cellValue, mask, rowObject) {
            var changed = parseInt(rowObject[7]);
            if (changed & mask) {
                return "<b>" + cellValue + "</b>";
            }

            return cellValue;
        }

        function formatDep(cellvalue, options, rowObject) {
            return boldByMask(cellvalue, 0x01, rowObject);
        }


        function formatJob(cellvalue, options, rowObject) {
            return boldByMask(cellvalue, 0x02, rowObject);
        }

        function formatBadge(cellvalue, options, rowObject) {
            return boldByMask(cellvalue, 0x04, rowObject);
        }

        function formatFacility(cellvalue, options, rowObject) {
            return boldByMask(cellvalue, 0x08, rowObject);
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
				<li ordinal="0">
					<a href="<%=Url.Action("Index", "Home") %>"><span url="/" class="label">Home</span></a>
				</li>
                <li ordinal="2"  class="selected">
					<a href="<%=Url.Action("Index", "People") %>"><span url="/People/" class="label">Associates</span></a>
				</li>
                <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("NewHires", "People") %>"><span class="label">New Hires</span></a>
				</li>
                 <li >
					&nbsp;&nbsp;<a href="<%=Url.Action("NewTerms", "People") %>"><span class="label">Terminations</span></a>
				</li>
                <li >
					&nbsp;&nbsp;<a href="<%=Url.Action("ReviewQueue", "People") %>"><span class="label">Changed</span></a>
				</li>
				<% if (ViewBag.IsAdmin)
                   { %>
				<li ordinal="3">
					<a href="<%=Url.Action("Index", "Admin") %>"><span url="/admin/" class="label">Admin</span></a>
				</li>
                <%} %>
              
			</ol>
		</dd>
	</dl>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <a href="<%=Url.Action("Index", "Home") %>"> Home</a> - Associates
</asp:Content>