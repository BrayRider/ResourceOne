<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.Admin.BosiStatusCollectionModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	R1SM: Admin
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="xsnazzy4">
		<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4">
		</b></b>
		<div class="xboxcontent">
			<h2>
				Import / Export Status</h2>
			<p>
				The icons below provide the status of the importers and exporters since that system's last cycle. &nbsp;
				Click any of the links below to view the activity log for that area.</p>
				
			<center>
				 <table id="list" class="scroll" cellpadding="0" cellspacing="0"></table>
				  <div id="pager" class="scroll" style="text-align:center;"></div>
			</center>  
			
		</div>
		<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1">
		</b></b>
	</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<link href="<%=Url.Content("~/Content/jquery-ui-1.8.9.custom.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%=Url.Content("~/Content/ui.jqgrid.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%=Url.Content("~/Content/ui.multiselect.css") %>" rel="stylesheet" type="text/css" />
	
	<style type="text/css">
	    .icon-trigger { margin: 2px; vertical-align: middle; display: inline-block; width: 24px; height: 24px; }
		.action-trigger { cursor: pointer; }

		.info-trigger { background: url('<%: Url.Content("~/Content/images/IEStatus0.png")%>') no-repeat scroll 0px 0px transparent !important; }
		.warn-trigger { background: url('<%: Url.Content("~/Content/images/IEStatus1.png")%>') no-repeat scroll 0px 0px transparent !important; }
		.error-trigger { background: url('<%: Url.Content("~/Content/images/IEStatus2.png")%>') no-repeat scroll 0px 0px transparent !important; }
	</style>

	<script src="<%=Url.Content("~/Scripts/jquery-1.4.4.min.js") %>" type="text/javascript"></script>
	
	<script src="<%=Url.Content("~/Scripts/i18n/grid.locale-en.js") %>" type="text/javascript"></script>
	<script src="<%=Url.Content("~/Scripts/jquery.jqGrid.min.js") %>" type="text/javascript"></script>
  
	<script type="text/javascript">
		jQuery(document).ready(function () {
			jQuery("#list").jqGrid({
				url: '<%: Url.Action("CommunicationStatusCollection", "Admin") %>',
				datatype: 'json',
				mtype: 'POST',
				height: 400,
				altRows: true,
				altclass: 'ui-widget-contentalt',
				colNames: ['Id', 'Severity', 'System', 'In/Out', 'Last Action', 'Outcome', 'Message', 'Logged'],
				colModel: [
						  { name: 'Id', index: 'Id', align: 'left', hidden: true, search: false },
						  { name: 'Severity', index: 'Severity', width: 75, align: 'center', search: false, formatter: 'actionFormatter' },
						  { name: 'SystemName', index: 'SystemName', width: 100, align: 'left', hidden: false, search: false },
						  { name: 'Direction', index: 'Direction', width: 75, align: 'left', hidden: false, search: false },
						  { name: 'LastAction', index: 'LastAction', width: 125, align: 'center', search: false },
						  { name: 'Outcome', index: 'Outcome', width: 150, align: 'left', hidden: false, search: false },
						  { name: 'Message', index: 'Message', width: 260, align: 'left', hidden: false, search: false },
						  { name: 'LogCount', index: 'LogCount', width: 75, align: 'right', search: false, formatter: 'countFormatter' }
						  ],
				pager: jQuery('#pager'),
				rowNum: 20,
				ondblClickRow: gridDoubleClick,
				rowList: [20, 40, 80, 100],
				sortname: 'SystemName',
				sortorder: "desc",
				viewrecords: true,
				imgpath: '<%: Url.Content("~/Content/images") %>',
				caption: 'Communication Status'
			}).navGrid('#pager', { search: false, edit: false, add: false, del: false, searchtext: "Search" });
		});


		function gridDoubleClick(rowid, iRow, iCol, e) {
			var row = jQuery(this).jqGrid('getRowData', rowid);
			window.location = '<%: Url.Action("ActivityLog", "Admin") %>/' + row.Id;
		}

		$.extend($.fn.fmatter, {
			actionFormatter: function (cellvalue, options, rowObject) {
				var retVal = "<span class=\'icon-trigger action-trigger " + cellvalue + "\-trigger' rel=\'" + cellvalue + "\' \/>";
				return retVal;
			}
		});

		$.extend($.fn.fmatter, {
			countFormatter: function (cellvalue, options, rowObject) {
				var retVal = "<span style=\'padding-right:20px !important;\'>" + cellvalue + "<\/span>";
				return retVal;
			}
		});
		</script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
	Admin
</asp:Content>
