<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
	<h1>Races of Perenthia</h1>
	<p>There are four playable races in Perenthia, each with their own history and qualities that benefit different aspects of game play.</p>
	
	<% 
		if (ViewData["Races"] != null)
		{  %>
	<div style="margin:0px auto;">
		<table cellpadding="0" cellspacing="0" border="0" width="100%">
			<tr>
				<% foreach (var r in (ViewData["Races"] as List<Radiance.Contract.RaceData>)){ %>
				<td>
					<table cellpadding="0" cellspacing="4" border="0">
						<tr>
							<th colspan="2"><h2><%= r.Name %></h2></th>
						</tr>
						<tr>
							<td class="border-simple"><a href="<%= Url.Action("Races", "Lore", new { name = r.Name, gender = "male" }) %>"><img src="/common/images/races/avatar-<%= r.Name %>-male.png" alt="<%= String.Concat(r.Name, " Male") %>" width="64" height="64" /></a></td>
							<td class="border-simple"><a href="<%= Url.Action("Races", "Lore", new { name = r.Name, gender = "female" }) %>"><img src="/common/images/races/avatar-<%= r.Name %>-female.png" alt="<%= String.Concat(r.Name, " Female") %>" width="64" height="64" /></a></td>
						</tr>
					</table>
				</td>
				<td></td>
				<%} %>
			</tr>
		</table>
	</div>
	<%}
	var race = ViewData["Race"] as Radiance.Contract.RaceData;
	var gender = ViewData["Gender"] as string;
	if (race != null && gender != null)
	{
		%>
	<table cellpadding="2" cellspacing="6" border="0">
		<tr>
			<td valign="top" class="border"><img src="/common/images/races/avatar-<%= race.Name %>-<%= gender %>-400.png" alt="<%= race.Name %>" /></td>
			<td valign="top" rowspan="2">
				<img src="/common/images/races/name-<%= race.Name %>.png" alt="<%= race.Name %>" /><br /><br />
				<table cellpadding="2" cellspacing="0" border="0">
					<% foreach (var stat in race.Attributes.Where(a => a.Value != 0))
					{
					%>
					<tr>
						<td align="right" class="stat"><%= stat.Name %>:</td>
						<td class="<%= ((stat.Value > 0) ? "positive" : "negative") %>"><%= stat.Value %></td>
					</tr>
					<%
					} %>
				</table>
				<br /><br />
				<p><%= race.Description %></p>
			</td>
		</tr>
		<tr>
			<td valign="top" align="center">
				<a href="<%= Url.Action("Races", "Lore", new { name = race.Name, gender = "male" }) %>"><img src="/common/images/icon-male.png" alt="Male" width="48" height="48" /></a>
				<a href="<%= Url.Action("Races", "Lore", new { name = race.Name, gender = "female" }) %>"><img src="/common/images/icon-female.png" alt="Female" width="48" height="48" /></a>
			</td>
		</tr>
	</table>
		<%
	}
%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    Races of Perenthia
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Information on the races available for play in the world of Perenthia." />
</asp:Content>
