<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RSM.Models.Settings.Grouping>" %>

<div id="xsnazzy">
    <b class="xtop"><b class="xb1"></b><b class="xb2"></b><b class="xb3"></b><b class="xb4"></b></b>

    <div class="xboxcontent">
        <h2><%: Model.Name %> Settings</h2>
        <p><%: Model.Label %></p>
        <table class="layout">
            <tbody>
                    <% 
                        foreach (var setting in Model.SettingCollection.OrderBy(x => x.OrderBy))
                        {
                            %><tr><%: 
                                  Html.Partial("_SettingEditor", setting)
                                  %></tr><%
                        } %>
            </tbody>
        </table>
    </div>

    <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1"></b></b>
</div>

