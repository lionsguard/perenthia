<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage<Radiance.Contract.HouseholdData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1><%= Model.Name %></h1>
	<table cellpadding="8" cellspacing="0" border="0" class="household">
		<tr>
			<td valign="top"><%= Html.Image(Model.ImageUri, Model.Name, new{ width = 128, height = 128}) %></td>
			<td valign="top">
				<div class="name"><%= Model.Name %></div>
				<div class="motto">"<%= Model.Motto %>"</div>
				<div class="desc"><%= Model.Description %></div>
			</td>
		</tr>
	</table>
	
	<h2>Members</h2>
	<%--<div class="search">
		<div class="pager"><%= Html.Pager("Household", "Armorial", Model.PageIndex, Model.MaxRows, Model.TotalRowCount, 5,
			new { }, new { @class = "next-prev" })%></div>
	</div>--%>
	<table rules="none" cellpadding="4" cellspacing="0" border="0" class="avatar-list" style="border-collapse:collapse;">
        <tr>
	        <th>Level</th>
	        <th colspan="2">Name</th>
	        <th>Zone</th>
	        <th>Online</th>
        </tr>
        <% if (Model.Members != null)
		   {
			   foreach (Radiance.Contract.AvatarData avatar in Model.Members)
			   {%>
        <tr>
            <td valign="top" class="level"><%= avatar.Level %></td>
            <td valign="top" class="avatarImg"><%= Html.Image(String.Format("/common/images/races/avatar-{0}-{1}.png", avatar.Race, avatar.Gender), avatar.Name, new{ width = 48, height = 48}) %></td>
            <td valign="top" class="name"><a href="/Armorial/Character/<%= avatar.Name %>" title="<%= avatar.Name %>"><%= avatar.Name %></a></td>
            <td valign="top" class="zone"><%= avatar.Zone %></td>
            <td valign="top" class="online"><span class="<%= avatar.IsOnline %>">&#x25C6;</span></td>
        </tr>
				<%}
		   }%>
	</table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
	<h1><%= Model.Name %></h1>
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="<%= Model.Description %>" />
</asp:Content>
