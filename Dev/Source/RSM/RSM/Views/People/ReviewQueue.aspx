<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.People.AssociateCollection>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	R1SM: <%: Model.PageTitle %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">
	<link href="<%=Url.Content("~/Content/jquery-ui-1.8.9.custom.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%=Url.Content("~/Content/ui.jqgrid.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%=Url.Content("~/Content/ui.multiselect.css") %>" rel="stylesheet" type="text/css" />

	<script src="<%=Url.Content("~/Scripts/jquery-1.4.4.min.js") %>" type="text/javascript"></script>
	
	<script src="<%=Url.Content("~/Scripts/i18n/grid.locale-en.js") %>" type="text/javascript"></script>
	<script src="<%=Url.Content("~/Scripts/jquery.jqGrid.min.js") %>" type="text/javascript"></script>  

	<script type="text/javascript">
		jQuery(document).ready(function () {
			jQuery("#list").jqGrid({
				url: '<%: Url.Action(Model.DataAction, "People") %>',
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
						  { name: 'Department', index: 'Department', width: 200, align: 'left', formatter: formatDep, searchoptions: { sopt: ['cn']} },
						  { name: 'Job', index: 'Job', width: 200, align: 'left', formatter: formatJob, searchoptions: { sopt: ['cn']} },
						  { name: 'Facility', index: 'Facility', width: 100, align: 'left', formatter: formatFacility, searchoptions: { sopt: ['cn']} },
						  { name: 'LastUpdated', index: 'LastUpdated', width: 110, align: 'left' },
						  { name: 'ChangeMask', index: 'ChangeMask', width: 0, align: 'left', hidden: true },
						  { name: 'ID', index: 'ID', width: 0, align: 'left', hidden: true }
						  ],
				pager: jQuery('#pager'),
				rowNum: <%: Model.PagingModel.PageSize %>,
				rowList: [25, 50, 100, 200],
				sortname: 'LastUpdated',
				sortorder: "desc",
				viewrecords: true,
				imgpath: '<%: Url.Content("~/Content/images") %>',
				caption: '<%: Model.GridCaption %>'
			}).navGrid('#pager', { search: true, edit: false, add: false, del: false, searchtext: "Search" });
		});

		function gridDoubleClick(rowid, iRow, iCol, e) {
			var row = jQuery(this).jqGrid('getRowData', rowid);
			window.location='<%=Url.Action(Model.ReviewAction, "People") %>/' + row.ID;
		}

		function formatStatus(cellvalue, options, rowObject) {
			var changed = parseInt(rowObject[7]);
			if (cellvalue == "True") {
				if (changed & 0x10) {
					return "<img src='<%: Url.Content("~/Content/images/icon_active_new.gif") %>' alt='active' title='Active Associate' />";
				}
				return "<img src='<%: Url.Content("~/Content/images/icon_active.png") %>' alt='active' title='Active Associate' />";
			}
			else {
				if (changed & 0x10) {
					return "<img src='<%=Url.Content("~/Content/images/icon_inactive_new.gif") %>' alt='Inactive' title='Inactive Associate' />";
				}
				return "<img src='<%=Url.Content("/Content/images/icon_inactive.png") %>' alt='Inactive' title='Inactive Associate' />";
			}
		};

		function boldByMask(cellValue, mask, rowObject) {
			var changed = parseInt(rowObject[7]);
			if (changed & mask) {
				return "<span class='dataChanged'>" + cellValue + "</span>";
			}
			return "<span class='dataUnchanged'>" + cellValue + "</span>";
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

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="xsnazzy">
		<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>
		
		<div class="xboxcontent">
			<h2><%: Model.AreaTitle  %></h2>
			
			<p><%: Html.Raw(Model.AreaMessage) %></p>

			<p>Double click on a row to view the details for that associate. Click the search button on the bottom left of the grid to find an associate.</p>
			
			<center>
				<table id="list" class="scroll" cellpadding="0" cellspacing="0"></table>
				<div id="pager" class="scroll" style="text-align:center;"></div></p>
			</center>

			<table class="marginlayout">
			<tr>
				<td align='center'>
				   <img src='<%=Url.Content("~/Content/images/icon_active_new.gif") %>' alt='active' title='New Associate' /><br />
				   <span class="smalltext">New Hire</span>
				</td>
				 <td align='center'>
				   <img src='<%=Url.Content("~/Content/images/icon_active.png") %>' alt='active' title='Active Associate' /> 
				   <span class="smalltext">Active Associate</span>
				</td>
				<td align='center'>
				   <img src='<%=Url.Content("~/Content/images/icon_inactive_new.gif") %>' alt='active' title='New Termination' /> 
				   <span class="smalltext">Recently Terminated</span>
				</td>
				 <td align='center'>
				   <img src='<%=Url.Content("~/Content/images/icon_inactive.png") %>' alt='active' title='New Termination' /> 
				   <span class="smalltext">Inactive Associate</span>
				</td>
				<td align='center'>
				   <span class="smalltext"><strong>Bold</strong> items have changed since the last import</span>
				</td>
			</tr>
		  
			</table>

		</div>

		<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

	</div>
  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="SidebarContent" runat="server">
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
	  <a href="<%=Url.Action("Index", "Home") %>"> Home</a> - <a href="/People">Associates</a>&nbsp;- <%: Model.CurrentMenuItem %>
</asp:Content>