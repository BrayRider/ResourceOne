<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Models.People.Edit>"
	ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	R1SM:
	<%:Model.Person.DisplayName %> - Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
	<script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>"
		type="text/javascript"></script>
	<% using (Html.BeginForm(new { autocomplete = "off" }))
	   { %>
	<%= Html.AntiForgeryToken() %>
	<%: Html.ValidationSummary(true)%>
	
	<input type="hidden" name="back" id="back" value="<%= Model.BackView %>" />
	<input type="hidden" name="gridData" id="gridData" value="" />
	<div id="xsnazzy">
		<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4">
		</b></b>
		<div class="xboxcontent">
			<h1>
				<%: Model.Person.DisplayName %></h1>
			<table class="layout">
				<tbody>
					<tr>
						<td>
							<img width="256" src="<%= Url.Action( "DisplayImage", "People", new { id = Model.Person.Id } ) %>" alt="Employee Photo" />
						</td>
						<td valign="top">
							<table class="layout">
								<tbody>
								<tr>
										<td align="right">
											<div class="display-label">
												Nickname:</div>
										</td>
										<td>
											<div class="display-field">
												<%: Html.EditorFor(model => model.Person.NickFirst) %></div>
										</td>
									</tr>
									<tr>
										<td align="right">
											<div class="display-label">
												Department:</div>
										</td>
										<td>
											<div class="display-field">
												<%: Model.Person.DeptDescr + " (" + Model.Person.DeptID + ")" %></div>
										</td>
									</tr>
									<tr>
										<td align="right">
											<div class="display-label">
												Position:</div>
										</td>
										<td>
											<div class="display-field">
												<%: Model.Person.JobDescr + " (" + Model.Person.JobCode + ")"%></div>
										</td>
									</tr>
									<tr>
										<td align="right">
											<div class="display-label">
												Facility:</div>
										</td>
										<td>
											<div class="display-field">
												<%: Model.Person.Facility%></div>
										</td>
									</tr>
									<!--
						<tr>
							<td align="right">Badge #:</td>
							<td><div class="display-field"><%: Model.Person.BadgeNumber %></div></td>
						</tr>
						-->
									<tr>
										<td align="right">
											<div class="display-label">
												Status:</div>
										</td>
										<td>
											<div class="display-field">
												<%: Model.Person.Active ? "Active" : "Inactive"%></div>
										</td>
									</tr>
									<tr>
										<td align="right">
											<div class="display-label">
												Credentials:</div>
										</td>
										<td>
											<div class="display-field">
												<%: Html.EditorFor(model => model.Person.Credentials)%></div>
										</td>
									</tr>
									<tr>
										<td align="right">
											<div class="display-label">
												Last Update:</div>
										</td>
										<td>
											<div class="display-field">
												<%: String.Format("{0:d}", Model.Person.LastUpdated)%></div>
										</td>
									</tr>
									 
								</tbody>
							</table>
						</td>
					</tr>
				</tbody>
			</table>
			<table class="layout">
				<tbody>
					<tr>
							<td>
								&nbsp;
							</td>
							<td>
								&nbsp;
							</td>
							<td align="right">
								<% if (Model.IsReview == false)
								   { %>
								<input type="submit" value="Save" />
								&nbsp;
								<%: Html.ActionLink("Cancel", Model.BackView, new { id = Model.Person.PersonID })%>
								<%}
								   else
								   { %>
								<input type="submit" value="Save" />
								&nbsp; &nbsp;
								<%: Html.ActionLink("Cancel", Model.BackView, new { id = Model.Person.PersonID })%>
								<%} %>
							</td>
						</tr>
<% if (Model.AllowRuleAdministration)
   { %>
						<tr>
						<td>
							<table id="alist" class="scroll" cellpadding="0" cellspacing="0">
							</table>
						</td>
						<td>
							<table class="layout">
								<tbody>
									<tr>
										<td>
											<a href='#'>
												<img alt="Assign" onclick="AssignLevel()" src="../../Content/images/right_arrow.png" /></a>
										</td>
									</tr>
									<tr>
										<td>
											<a href='#'>
												<img alt="Remove" onclick="RemoveLevel()" src="../../Content/images/left_arrow.png" /></a>
										</td>
									</tr>
								</tbody>
							</table>
						</td>
						<td>
							<table id="blist" class="scroll" cellpadding="0" cellspacing="0">
							</table>
						</td>
					</tr>
	<% } %>
				</tbody>
			</table>
		</div>
		<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1">
			</b></b>


		<p></p>
		<% if (Model.IsAdmin)
		   { %>
		<div id="Div1">
			<b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4">
			</b></b>
			<div class="xboxcontent">
				<h2>
					Resource One Security Manager Access</h2>
				<p>
					As an administrator you can grant access to the Resource One Security Manager to
					this associate.</p>
				<table class="layout">
					<tbody>
						<tr>
							<td align="right">
								Username:
							</td>
							<td>
								<div class="display-field">
									<%: Html.EditorFor(model => Model.Person.username)%></div>
							</td>
							<td align="right">
								Password:
							</td>
							<td>
								<div class="display-field">
									<%: Html.PasswordFor(model => Model.Person.password)%></div>
							</td>
						</tr>
						<tr>
							<td align="right">
								Admin:
							</td>
							<td>
								<div class="display-field">
									<%: Html.EditorFor(model => Model.IsAdmin)%></div>
							</td>
							<td align="right">
								Locked Out:
							</td>
							<td>
								<div class="display-field">
									<%: Html.EditorFor(model => model.Person.LockedOut)%></div>
							</td>
						</tr>
					</tbody>
				</table>
			</div>
			<b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1">
			</b></b>
		</div>
		<%} %>
		<% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

	<link href="<%= Url.Content("~/Content/jquery-ui-1.8.9.custom.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%= Url.Content("~/Content/ui.jqgrid.css") %>" rel="stylesheet" type="text/css" />
	<link href="<%= Url.Content("~/Content/ui.multiselect.css") %>" rel="stylesheet" type="text/css" />

	<script src="<%= Url.Content("~/Scripts/json2.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/jquery-1.4.4.min.js") %>" type="text/javascript"></script>    
	<script src="<%= Url.Content("~/Scripts/i18n/grid.locale-en.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/jquery.jqGrid.min.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/levelmgmt.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/rolemgmt.js") %>" type="text/javascript"></script>

<% if (Model.AllowRuleAdministration)
   { %>
	<script type="text/javascript">
		jQuery(document).ready(function () {


			setupRoleGrid("#alist", "Available", '<%: Model.AvailableRolesUrl %>', assignByDoubleClickWithException, function () { });
			$("#alist").loadComplete = function () {
				$("#alist").setGridParam({ datatype: 'local' });
			};


			setupRoleGridWithExceptions("#blist", "Assigned", '<%: Model.AssignedRolesUrl %>', removeByDoubleClick, storeAssignedInForm);
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
	<% if (Model.IsReview == false)
	   { %>
	<a href="<%=Url.Action("Index", "People") %>">People</a> -
	<%: Html.ActionLink(Model.Person.DisplayName, "Details", new { id = Model.Person.PersonID })%>
	- Edit
	<%}
	   else
	   { %>
	<a href="<%=Url.Action("Index", "Home") %>">Review</a> -
	<%: Html.ActionLink(Model.Person.DisplayName, "Review", new { id = Model.Person.PersonID })%>
	- Edit
	<%} %>
</asp:Content>
