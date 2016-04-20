<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Mobile.Master" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.ArmorialViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Perenthia Armorial</h1>
	
	<div>
		<p>The Perenthia Armorial allows you to search for and view details about Characters, Households, Items and Creatures.</p>
		<div class="search">
			<ul>
				<li><%= Model.ActionLink(Html, "All", new { searchType = "all" })%></li>
				<li><%= Model.ActionLink(Html, "Characters", new { searchType = "characters" })%></li>
				<li><%= Model.ActionLink(Html, "Households", new { searchType = "households" })%></li>
			</ul>
		</div>
		<div class="search">
		<% using (Html.BeginForm("Search", "Armorial", FormMethod.Get)){  %>
			<%= Html.TextBox("query", Request.Params["query"], new{ length = 64, width = "150", tabindex = 1}) %>
			&nbsp;&nbsp;
			<span class="button"><a href="javascript:document.forms[0].submit();" title="Search" tabindex="2">Search</a></span>
		<%} %>
		</div>
	</div>

<div class="search">
	<% if (Model.Results != null)
	{
		%>
		<div class="pager"><%= Html.Pager("Search", "Armorial", Model.PageIndex, Model.MaxRows, Model.TotalRowCount, 5,
			Model.ToRouteValues(), new { }, new { @class = "next-prev" })%></div>
		<%
		switch (Model.Tab)
		{
			case ArmorialTab.Characters:
				%>
				<table rules="none" cellpadding="4" cellspacing="0" border="0" class="avatar-list" style="border-collapse:collapse;">
					<tr>
						<th><%= Model.ActionSortLink(Html, "Level")%></th>
						<th><%= Model.ActionSortLink(Html, "Name")%></th>
						<th>Online</th>
					</tr>
					<% foreach (Radiance.Contract.AvatarData player in Model.Results)
		{	
					%>
					<tr>
						<td valign="top" class="level"><%= player.Level%></td>
						<td valign="top" class="avatarImg"><img src="/common/images/races/avatar-<%= player.Race %>-<%= player.Gender %>.png" alt="<%= player.Name %>" width="48" height="48" /><br />
						<a href="/Armorial/Character/<%= player.Name %>/Detail" title="<%= player.Name %>"><%= player.Name%></a></td>
						<td valign="top" class="online"><span class="<%= player.IsOnline %>">&#x25C6;</span></td>
					</tr>
						<%
		} %>
				</table>
				<%
		break;
			case ArmorialTab.Households:
				%>
				<table rules="none" cellpadding="4" cellspacing="0" border="0" class="household-list" style="border-collapse:collapse;">
					<tr>
						<th><%= Model.ActionSortLink(Html, "Honor Points")%></th>
						<th><%= Model.ActionSortLink(Html, "Name")%></th>
					</tr>
					<% foreach (Radiance.Contract.HouseholdData household in Model.Results)
		{
					%>
					<tr>
						<td valign="top" class="honor"><%= household.HonorPoints%></td>
						<td valign="top" class="image"><img src="<%= Perenthia.Web.WebUtils.GetHouseholdImageUri(household.ImageUri) %>" alt="<%= household.Name %>" width="48" height="48" /><br />
						<a href="/Armorial/Household/<%= household.Name %>" title="<%= household.Name %>"><%= household.Name%></a></td>
					</tr>
					<%
		} %>
				</table>
				<%
		break;
			case ArmorialTab.Items:
				%><%
		break;
			case ArmorialTab.Creatures:
				%><%
		break;
		}
		%>
		<div class="pager"><%= Html.Pager("Search", "Armorial", Model.PageIndex, Model.MaxRows, Model.TotalRowCount, 5,
			Model.ToRouteValues(), new { }, new { @class = "next-prev" })%></div>
		<%
		}
	else
	{ %>
	<p>No results found</p>
	<%} %>
	</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Perenthia Armorial - Search for Characters, Households, Items and Creatures
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
