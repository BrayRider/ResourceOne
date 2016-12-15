<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.Admin.JobCodeCollection>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   R1SM: <%: Model.PageTitle %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="xsnazzy4">
		<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

		<div class="xboxcontent">
			<h2>Job Codes</h2>
			
			<p>The grid below job codes discovered by the Resource One Security Manager.</p>
			<p>Double click on a row to view additional details.</p>

			<center>
				<table id="list" class="scroll" cellpadding="0" cellspacing="0"></table>
				<div id="pager" class="scroll" style="text-align:center;"></div>
			</center>  
		</div>

		<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

	</div>
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
		    jQuery("#list").jqGrid({
		        url: '<%: Url.Action(Model.DataAction, "Admin") %>',
		        datatype: 'json',
		        mtype: 'POST',
		        height: 400,
		        altRows: true,
		        altclass: 'ui-widget-contentalt',
		        colNames: ['Job Code', 'Job Description', 'Display Description'],
		        colModel: [
		            { name: 'JobCode', index: 'Job Code', width: 100, align: 'left', hidden: false, search: true },
		            { name: 'JobDescr', index: 'Job Descr', width: 350, align: 'left', hidden: false, search: true },
		            { name: 'DisplayDescr', index: 'Display Descr', width: 350, align: 'left', search: true }
		        ],
		        pager: jQuery('#pager'),
		        rowNum: <%: Model.PagingModel.PageSize %>,
		        ondblClickRow: gridDoubleClick,
		        rowList: [20, 40, 80, 100],
		        sortname: 'JobCode',
		        sortorder: "desc",
		        viewrecords: true,
		        imgpath: '<%: Url.Content("~/Content/images") %>',
		        caption: '<%: Model.GridCaption %>'
		    }).navGrid('#pager', { search: false, edit: false, add: false, del: false, searchtext: "Search" });
		});
		
		function gridDoubleClick(rowid, iRow, iCol, e) {
			var row = jQuery(this).jqGrid('getRowData', rowid);
			window.location = '<%=Url.Action(Model.ReviewAction, "People") %>/' + row.JobCode;
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
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
	<a href="<%=Url.Action("Index", "Admin") %>">Admin</a> - Job Codes
</asp:Content>