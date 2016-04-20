<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage<Radiance.Contract.AvatarData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
	<h1><%= Model.Name %></h1>
	<table cellpadding="8" cellspacing="0" border="0" class="character">
		<tr>
			<td valign="top"><img src="/common/images/races/avatar-<%= Model.Race %>-<%= Model.Gender %>.png" alt="<%= Model.Name %>" /></td>
			<td valign="top">
				<div class="name"><%= Model.Name %> (<%= Model.Level %>)</div>
				<div>
					<span class="zone"><%= Model.Zone %> : </span><span class="location"><%= Model.X %>, <%= Model.Y %>, <%= Model.Z %></span>
					<span class="online"><span class="<%= Model.IsOnline %>" title="<%= Model.IsOnline ? "Online" : "Offline" %>">&#x25C6;</span></span>
				</div>
				<div class="household">
					<h3>Household</h3>
					<div>
						<div><%= Html.Image(WebUtils.GetHouseholdImageUri(Model.HouseholdImageUri), Model.HouseholdName, new { width = 64, height = 64 })%></div>
						<div><%= Model.HouseholdName %></div>
					</div>
					<div>
						<div><%= Html.Image(WebUtils.GetRankImageUri(Model.RankImageUri), Model.RankName, new { width = 64, height = 64 })%></div>
						<div><%= Model.RankName %></div>
					</div>
				</div>
			</td>
		</tr>
	</table>
	<h2>Skills</h2>
	<% if (Model.Skills != null && Model.Skills.Length > 0)
	{
		lstSkills.DataSource = Model.Skills.OrderByDescending(s => s.Value);
		lstSkills.DataBind();
	}%>
	<asp:DataList ID="lstSkills" runat="server" RepeatColumns="2" RepeatDirection="Horizontal" CssClass="skills" CellPadding="4">
		<ItemTemplate>
			<span class="name"><%# WebUtils.FormatSkillName(Eval("Name"))%> : </span><span class="value"><%# Eval("Value") %></span>
		</ItemTemplate>
	</asp:DataList>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Character Details for <%= Model.Name %>
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="<%= Model.Description %>" />
</asp:Content>
