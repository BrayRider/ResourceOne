<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<RSM.Models.Settings.SettingModel>" %>
<%@ Import Namespace="RSM.Artifacts" %>

<% 
    if (Model.InputType == InputTypes.Checkbox)
   { %>
        <td>
            <%: Html.CheckBox(Model.FullName, Model.Value.AsBool()) %>
            <%: Html.Label(Model.FullName, Model.Label)%>
        </td>
    <% }
   else
   { %>
        <td>
            <%: Html.Label(Model.FullName, Model.Label + ":")%>
            <% if (Model.InputType == InputTypes.Password)
               { %>
                <%: Html.Password(Model.FullName, null)%>
            <% } else { %>
                <%: Html.TextBox(Model.FullName, Model.Value)%>
            <% } %>
        </td>
        <td>
            <%: Html.ValidationMessage(Model.FullName, Model.ValidationMessage)%>
        </td>
<% } %>
