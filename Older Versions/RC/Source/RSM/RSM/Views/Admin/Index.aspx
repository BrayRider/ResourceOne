<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
                The icons below provide the status of the importers and exporters since you last
                looked at the activity log. &nbsp;Click any of the links below to view the activity log for that area.</p>
            <table class="marginlayout">
                <tbody>
                    <tr>
                        <td>
                            <table class="marginlayout">
                                <tbody>
                                    <tr>
                                        <td align="center">
                                            <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=2">
                                                <img width="32" src="<%=Url.Content("~/Content") %>/Images/IEStatus<%:ViewBag.PSImportStatus%>.png" />
                                            </a>
                                        </td>
                                        <td align="center">
                                            <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=0">
                                                <img width="32" src="<%=Url.Content("~/Content") %>/Images/IEStatus<%:ViewBag.S2ImportStatus%>.png" />
                                            </a>
                                        </td>
                                        <td align="center">
                                            <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=1">
                                                <img width="32" src="<%=Url.Content("~/Content") %>/Images/IEStatus<%:ViewBag.S2ExportStatus%>.png" />
                                            </a>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=2">Associate Import</a>
                                        </td>
                                        <td>
                                            <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=0">S2 NetBox Import</a>
                                        </td>
                                        <td>
                                            <a href="<%=Url.Action("ActivityLog", "Admin") %>/?Filter=1">S2 NetBox Export</a>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="right">
                            <table class="marginlayout">
                                <tr>
                                    <td>
                                        <img src='<%=Url.Content("~/Content") %>/Images/IEStatus0.png' alt='active' title='All Good' /><br />
                                        <span class="smalltext">Successful</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <img src='<%=Url.Content("~/Content") %>/Images/IEStatus1.png' alt='active' title='Warning' /><br />
                                        <span class="smalltext">Changes may require attention</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <img src='<%=Url.Content("~/Content") %>/Images/IEStatus2.png' alt='active' title='All Good' /><br />
                                        <span class="smalltext">Errors present</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
            
        </div>
        <b class="xbottom"><b class="xb4"></b><b class="xb3"></b><b class="xb2"></b><b class="xb1">
        </b></b>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SidebarContent" runat="server">
    <dl class="sectionFeeds">
        <dt id="SectionsHeader" class="content">
            <div class="label">
                Menu</div>
        </dt>
        <dd id="SectionsContent" class="content" style="display: block; height: 293px;">
            <ol>
                <li><a href="<%=Url.Action("Index", "Home") %>"><span class="label">Home</span></a> </li>
                <li><a href="<%=Url.Action("Index", "People") %>"><span class="label">Associates</span></a> </li>
                <li class="selected"><a href="<%=Url.Action("Index", "Admin") %>"><span class="label">Admin</span></a>
                </li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("ActivityLog", "Admin") %>"><span class="label">Activity Log</span></a>
                </li>
                 <li>
					&nbsp;&nbsp;<a href="<%=Url.Action("JobCodes", "Admin") %>"><span class="label">Job Codes</span></a>
				</li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Index", "Roles") %>"><span class="label">Roles</span></a> </li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Index", "JCLRule") %>"><span class="label">Rules</span></a> </li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Reports", "Admin") %>"><span class="label">Reports</span></a> </li>
                <li>&nbsp;&nbsp;<a href="<%=Url.Action("Settings", "Admin") %>"><span class="label">Settings</span></a>
                </li>
            </ol>
        </dd>
    </dl>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="BreadCrumbs" runat="server">
    Admin
</asp:Content>
