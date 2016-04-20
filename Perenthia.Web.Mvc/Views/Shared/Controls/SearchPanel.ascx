<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="search">
    <% using (Html.BeginForm("Search", "Armorial", FormMethod.Get, new { id = "frmSearch" }))
       { %>
    <%= Html.Hidden("tab", Server.UrlDecode(Request.QueryString["tab"] ?? "characters")) %>
    <%= Html.Hidden("searchType", Server.UrlDecode(Request.QueryString["searchType"])) %>
    <%= Html.TextBox("query", Server.UrlDecode(Request.QueryString["query"]), new{ length = 64, width = "150px", tabindex = 1}) %>
    &nbsp;
    <span class="button"><a href="javascript:document.forms['frmSearch'].submit();" title="Search" tabindex="2">Search</a></span>
    <%} %>
</div>
<div class="search">
    <ul>
        <li><%= Html.ActionLink("All", "Search", "Armorial", new { searchType = "all" }, null)%></li>
        <li><%= Html.ActionLink("Characters", "Search", "Armorial", new { searchType = "characters" }, null)%></li>
        <li><%= Html.ActionLink("Households", "Search", "Armorial", new { searchType = "households" }, null)%></li>
    </ul>
</div>
