<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.Admin.ActivityLogCollectionModel>" %>

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
			
			<table class="layout">
				<tbody>
					<tr>
						<td>System:</td>
						<td><%: Html.DropDownList("SystemFilter", Model.SystemCollection, new { onchange = @"FilterRouting(this.value);" })%></td>
					</tr>
				</tbody>
			</table>

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

<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
	<a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Activity Log
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<link href="<%=Url.Content("~/Content/jquery-ui-1.8.9.custom.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%=Url.Content("~/Content/ui.jqgrid.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%=Url.Content("~/Content/ui.multiselect.css") %>" rel="stylesheet" type="text/css" />

	<script src="<%=Url.Content("~/Scripts/jquery-1.4.4.min.js") %>" type="text/javascript"></script>
	
	<script src="<%=Url.Content("~/Scripts/i18n/grid.locale-en.js") %>" type="text/javascript"></script>
	<script src="<%=Url.Content("~/Scripts/jquery.jqGrid.min.js") %>" type="text/javascript"></script>

	<script type="text/javascript">
		jQuery(document).ready(function () {
			var systems = "<%: Model.SystemFilters %>";

			jQuery("#list").jqGrid({
				url: '<%: Url.Action("SysLog", "Admin") %>/<%: Model.System %>',
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

					{ name: 'Source', index: 'Source', width: 125, search: true, formatter: formatSeverity, align: 'left',
						stype: 'select', searchoptions: { sopt: ['eq'], value: systems }
					},
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
			}).navGrid('#pager', { search: true, edit: false, add: false, del: false, searchtext: "Search" });
		});

		function gridDoubleClick(rowid, iRow, iCol, e) {
			var row = jQuery(this).jqGrid('getRowData', rowid);
			window.location = '<%: Url.Action("EventDetails", "Admin") %>/' + row.ID + "?Filter=<%: Model.System %>";
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

		function FilterRouting(id) {
			window.location = '<%: Url.Action("ActivityLog", "Admin", new { id = ""} ) %>/' + id;
		}
	</script>
</asp:Content>

