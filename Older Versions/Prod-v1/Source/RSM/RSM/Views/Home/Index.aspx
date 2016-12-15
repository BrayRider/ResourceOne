<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Resource One Security Manager
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="HeadContent" runat="server">

    

</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="SidebarContent" runat="server">
    <dl class="sectionFeeds">
		<dt id="SectionsHeader" class="content">
			<div class="label">Menu</div>
		</dt>
			
		<dd id="SectionsContent" class="content" style="display: block; height: 293px; ">
			<ol>
                 <li class="selected">
                 
					<a href="<%=Url.Action("Index", "Home") %>"><class="label">Home</span></a>
				</li>
			
				<li>
                
					<a href="<%=Url.Action("Index", "People") %>"><span class="label">Associates</span></a>
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

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="xsnazzy">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
          <h2>Associates</h2>
          <p>&nbsp;</p>
          <p><a href="<%=Url.Action("Index", "People") %>">All Associates</a> | <a href="<%=Url.Action("NewHires", "People") %>">New Hires</a> | <a href="<%=Url.Action("NewTerms", "People") %>">Terminations</a> | <a href="<%=Url.Action("ReviewQueue", "People") %>">Access Changes</a></p>
          <p>&nbsp;</p>
        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>

    <div id="xsnazzy">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
          <h2>Admin</h2>
          <p>&nbsp;</p>
          <p><a href="<%=Url.Action("ActivityLog", "Admin") %>">Activity Log</a> | <a href="Roles/">Manage Roles</a> | <a href="JCLRule/">Manage Rules</a></p>
          <p>&nbsp;</p>
        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>

    </div>
  
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    Home
</asp:Content>