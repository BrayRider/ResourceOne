<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RSM.Support.Person>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    R1SM: <%: Model.LastName + ", " + Model.FirstName + " " + Model.MiddleName %> Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">




 <div id="persondisplay">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h1><%: Model.DisplayName %></h1>
            
        <table class="layout">
        <tbody>
    
        <tr>
            <td>
                <img width="256" src='<%= ViewBag.PicURL %>' alt="Employee Photo"/>
            </td>
            <td valign="top">
                <table class="layout">
                    <tbody>
                        <tr>
                            <td align="right"><div class="display-label">Department:</div></td>
                            <td><div class="display-field"><%: Model.DeptDescr + " (" + Model.DeptID + ")" %></div></td>
                        </tr>
                        <tr>
                            <td align="right"><div class="display-label">Position:</div></td>
                            <td><div class="display-field"><%: Model.JobDescr  + " (" + Model.JobCode + ")" %></div></td>
                        </tr>
                        <tr>
                            <td align="right"><div class="display-label">Facility:</div></td>
                            <td><div class="display-field"><%: Model.Facility %></div></td>
                        </tr>
                        
                        <tr>
                            <td align="right"><div class="display-label">Badge #:</div></td>
                            <td><div class="display-field"><%: Model.BadgeNumber %></div></td>
                        </tr>
                        <tr>
                            <td align="right">
                                <div class="display-label">
                                    Credentials:</div>
                            </td>
                            <td>
                                <div class="display-field"><%: Model.DisplayCredentials %></div>
                            </td>
                        </tr>
                        <tr>
                            <td align="right"><div class="display-label">Status:</div></td>
                            <td><div class="display-field"><%: Model.Active ? "Active" : "Inactive" %></div></td>
                        </tr>
                        <tr>
                            <td align="right"><div class="display-label">Last Update:</div></td>
                            <td><div class="display-field"><%: String.Format("{0:d}", Model.LastUpdated) %></div></td>
                        </tr>
                        
                    </tbody>
                </table>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            
                        <% if (ViewBag.IsReview && Model.NeedsApproval)
                           { %>
   <td align="right">
<% using (Html.BeginForm(new { autocomplete = "off" }))
   { %>
   <%= Html.AntiForgeryToken()%>
   <%: Html.ValidationSummary(true)%>
   <input type="submit" value="Approve Access" />
   <input type="hidden" name="returnView" id="returnView" value="<%= ViewBag.ReturnView %>" />
   
<% } %>
</td>
<%}
                           else
                           { %>
<td>&nbsp;</td>
<%} %>
            <td align="right">
                <%: Html.ActionLink("Edit", "Edit", new { id = Model.PersonID, back = (string)ViewBag.BackView })%> &nbsp;
                <%: Html.ActionLink("Back to List", (string)ViewBag.ReturnView)%>
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
<%--CSS Files--%>
    <link href="<%=Url.Content("~/Content") %>/jquery-ui-1.8.9.custom.css" rel="stylesheet" type="text/css" />
    <link href="<%=Url.Content("~/Content") %>/ui.jqgrid.css" rel="stylesheet" type="text/css" />
    <link href="<%=Url.Content("~/Content") %>/ui.multiselect.css" rel="stylesheet" type="text/css" />
    <script src="<%=Url.Content("~/Scripts") %>/json2.js" type="text/javascript"></script>

    <%--jQuery Library--%>
    <script src="<%=Url.Content("~/Scripts") %>/jquery-1.4.4.min.js" type="text/javascript"></script>
    
    <%--Must load language tag BEFORE script tag--%>
    <script src="<%=Url.Content("~/Scripts") %>/i18n/grid.locale-en.js" type="text/javascript"></script>
   

    <script src="<%=Url.Content("~/Scripts") %>/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts") %>/levelmgmt.js" type="text/javascript"></script>
    <script src="<%=Url.Content("~/Scripts") %>/rolemgmt.js" type="text/javascript"></script>
    <%--jqGrid Code - refer http://www.trirand.com/blog/jqgrid/jqgrid.html --%>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            setupRoleGridWithExceptions("#blist", "Assigned", '<%=(string)ViewBag.AssRolesURL %>', noOpDoubleClick, function () { });
            $("#blist").loadComplete = function () {
                $("#blist").setGridParam({ datatype: 'local' });

            };

        });
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
					<a href="<%=Url.Action("Index", "Home") %>"><span url="/" class="label">Home</span></a>
				</li>
                <li>
					<a href="<%=Url.Action("Index", "People") %>"><span class="label">Associates</span></a>
				</li>
                <% if (ViewBag.ReviewMode == RSM.Controllers.PeopleController.ReviewModes.HIRE){  %>
                    <li  class="selected">
                 <% } else { %>
                    <li>
                 <% }%>
                    
					    &nbsp;&nbsp;<a href="<%=Url.Action("NewHires", "People") %>"><span class="label">New Hires</span></a>
                   
				</li>
                 <% if (ViewBag.ReviewMode == RSM.Controllers.PeopleController.ReviewModes.FIRE){  %>
                    <li  class="selected">
                 <% } else { %>
                    <li>
                    <% }%>
					&nbsp;&nbsp;<a href="<%=Url.Action("NewTerms", "People") %>"><span class="label">Terminations</span></a>
				</li>
                <% if (ViewBag.ReviewMode == RSM.Controllers.PeopleController.ReviewModes.CHANGE ){  %>
                    <li  class="selected">
                 <% } else { %>
                    <li>
                    <% }%>
					&nbsp;&nbsp;<a href="<%=Url.Action("ReviewQueue", "People") %>"><span class="label">Changed</span></a>
				</li>

				<% if (ViewBag.IsAdmin)
                   { %>
				<li>
					<a href="<%=Url.Action("Index", "Admin") %>"><span class="label">Admin</span></a>
				</li>
                <%} %>
              
			</ol>
		</dd>
	</dl>
</asp:Content>


<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    <% if (ViewBag.IsReview == false)  { %>
        <a href="<%=Url.Action("Index", "Home") %>">Home</a> - <a href="<%=Url.Action("Index", "People") %>">Associates</a> - <%: Model.LastName + ", " + Model.FirstName + " " + Model.MiddleName%>
    <%} else { %>
        <a href="<%=Url.Action("Index", "Home") %>">Home</a> - <a href="<%=Url.Action("Index", "People") %>">Associates</a>
        <% if (ViewBag.ReviewMode == RSM.Controllers.PeopleController.ReviewModes.HIRE){  %>
      &nbsp;- <a href="<%=Url.Action("NewHires", "People") %>">New Hires</a> - 
      <% } else if (ViewBag.ReviewMode == RSM.Controllers.PeopleController.ReviewModes.FIRE){  %>
      &nbsp;- <a href="<%=Url.Action("NewTerms", "People") %>">Terminations</a> -
      <% } else {  %>
      &nbsp;- <a href="<%=Url.Action("ReviewQueue", "People") %>">Changed</a> -
      <% }  %>
      <%: Model.LastName + ", " + Model.FirstName + " " + Model.MiddleName%>
    <%} %>
</asp:Content>