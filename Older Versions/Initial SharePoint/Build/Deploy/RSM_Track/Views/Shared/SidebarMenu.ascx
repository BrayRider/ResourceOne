<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RSM.Models.MenuCollection>" %>

<dl class="sectionFeeds">
	<dt id="SectionsHeader" class="content">
		<div class="label"><%: Model.Title %></div>
	</dt>
			
	<dd id="SectionsContent" class="content" style="display: block; height: 293px; ">
		<ol>
			<% foreach (var item in Model.LinkCollection)
			   { %><li class="<%: item.Text.Equals(Model.SelectedMenuText) ? "selected" : "" %>"><%: Html.Partial("SidebarMenuItem", item) %></li><%
			   }%>
		</ol>
	</dd>
</dl>
