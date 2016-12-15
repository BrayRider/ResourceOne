<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.People.Detail>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	R1SM: <%: Model.Person.DisplayName %> Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	 <div id="persondisplay">
		<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

		<div class="xboxcontent">
			<h1><%: Model.Person.DisplayName %></h1>
			
		<table class="layout">
		<tbody>
	
		<tr>
			<td>
				<img width="256" src="<%= Url.Action( "DisplayImage", "People", new { id = Model.Person.Id } ) %>" alt="Employee Photo"/>
			</td>
			<td valign="top">
				<table class="layout">
					<tbody>
						<tr>
							<td align="right"><div class="display-label">Department:</div></td>
							<td><div class="display-field"><%: Model.Person.DeptDescr + " (" + Model.Person.DeptID + ")"%></div></td>
						</tr>
						<tr>
							<td align="right"><div class="display-label">Position:</div></td>
							<td><div class="display-field"><%: Model.Person.JobDescr + " (" + Model.Person.JobCode + ")"%></div></td>
						</tr>
						<tr>
							<td align="right"><div class="display-label">Facility:</div></td>
							<td><div class="display-field"><%: Model.Person.Facility%></div></td>
						</tr>
						
						<tr>
							<td align="right"><div class="display-label">Badge #:</div></td>
							<td><div class="display-field"><%: Model.Person.BadgeNumber%></div></td>
						</tr>
						<tr>
							<td align="right">
								<div class="display-label">
									Credentials:</div>
							</td>
							<td>
								<div class="display-field"><%: Model.Person.DisplayCredentials%></div>
							</td>
						</tr>
						<tr>
							<td align="right"><div class="display-label">Status:</div></td>
							<td><div class="display-field"><%: Model.Person.Active ? "Active" : "Inactive"%></div></td>
						</tr>
						<tr>
							<td align="right"><div class="display-label">Last Update:</div></td>
							<td><div class="display-field"><%: String.Format("{0:d}", Model.Person.LastUpdated)%></div></td>
						</tr>
						
					</tbody>
				</table>
			</td>
		</tr>
		<tr>
			<td>&nbsp;</td>
			<% if (Model.IsReview && Model.NeedsApproval)
				{ %>
			<td align="right">
<% using (Html.BeginForm(new { autocomplete = "off" }))
   { %>
   <%= Html.AntiForgeryToken()%>
   <%: Html.ValidationSummary(true)%>
   <input type="submit" value="Approve Access" />
   <input type="hidden" name="returnView" id="returnView" value="<%= Model.ReturnView %>" />
   <input type="hidden" name="returnStatus" id="returnStatus" value="<%= Model.ReturnStatus %>" />
   
<% } %>
</td>
<%}
						   else
						   { %>
<td>&nbsp;</td>
<%} %>
			<td align="right">
				<%: Html.ActionLink("Edit", "Edit", new { id = Model.Person.PersonID, back = Model.BackView })%> &nbsp;
				<a href="<%: Model.ReturnUrl %>">Back to List</a>	
			</td>

		</tr>
		</tbody>
	</table>

	<table class="layout">
		<tbody>
		<tr>
		<td width="310">
					&nbsp;
				</td>
		 
		</tr>
		   
			<tr>
				<td width="310">
					&nbsp;
				</td>
				<td>
					&nbsp;
				</td>
				<td>
					<table id="blist" class="scroll" cellpadding="0" cellspacing="0"></table>
				</td>
			</tr>
		</tbody>
	</table>

		</div>

		<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

	</div>



<fieldset>
	
</fieldset>
<p>

	
	
</p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
<% if (Model.AllowRuleAdministration)
   { %>
	<link href="<%= Url.Content("~/Content/jquery-ui-1.8.9.custom.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%= Url.Content("~/Content/ui.jqgrid.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%= Url.Content("~/Content/ui.multiselect.css") %>" rel="stylesheet" type="text/css" />

	<script src="<%= Url.Content("~/Scripts/json2.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/jquery-1.4.4.min.js") %>" type="text/javascript"></script>    
	<script src="<%= Url.Content("~/Scripts/i18n/grid.locale-en.js") %>" type="text/javascript"></script>
   <script src="<%= Url.Content("~/Scripts/jquery.jqGrid.min.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/levelmgmt.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/rolemgmt.js") %>" type="text/javascript"></script>

	<script type="text/javascript">
		jQuery(document).ready(function () {
			setupRoleGridWithExceptions("#blist", "Assigned", '<%= Model.AssignedRolesUrl %>', noOpDoubleClick, function () { });
			$("#blist").loadComplete = function () {
				$("#blist").setGridParam({ datatype: 'local' });
			};
			  });
	</script>
	<% } %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
	<%: Html.Partial("SidebarMenu", Model.SidebarMenu) %>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
	<% if (Model.IsReview == false)  { %>
		<%=Html.ActionLink("Home", "Index", "Home") %> - <%=Html.ActionLink("Associates", "Index", "People") %> - <%: Model.Person.DisplayName %>
	<%} else
   { %>
		<%=Html.ActionLink("Home", "Index", "Home")%> - <%=Html.ActionLink("Associates", "Index", "People") %>&nbsp;-&nbsp;<%=Html.ActionLink(Model.BreadcrumbText, "Index", "People", new { status = Model.BreadcrumbStatus })%> - <%: Model.Person.DisplayName  %>
	<%} %>
</asp:Content>