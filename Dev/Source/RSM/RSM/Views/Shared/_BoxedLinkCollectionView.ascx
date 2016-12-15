<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RSM.Models.MenuCollection>" %>

<% if (Model.LinkCollection != null && Model.LinkCollection.Any())
   {%>
    <div id="xsnazzy">
        <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

        <div class="xboxcontent">
            <h2><%: Model.Title %></h2>
            <p>&nbsp;</p>
            <p>
                <%: Html.Raw(String.Join(" | ", Model.LinkCollection.Select(link => @Html.ActionLink(link.Text, link.Action, link.Controller, link.RouteValuesCollection, new {})))) %>
            </p>
            <p>&nbsp;</p>
        </div>

        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>
    </div>
<% } %>
