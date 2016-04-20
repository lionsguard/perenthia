<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Mobile.Master" Inherits="System.Web.Mvc.ViewPage<Radiance.Contract.AvatarData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1><%= Model.Name %></h1>
	<div class="avatar">
		<div class="image"><img src="/common/images/races/avatar-<%= Model.Race %>-<%= Model.Gender %>.png" alt="<%= Model.Name %>" /></div>
		<div class="details">
			<div class="name"><%= Model.Name %> (<%= Model.Level %>)</div>
			<div>
				<span class="zone"><%= Model.Zone %> : </span><span class="location"><%= Model.X %>, <%= Model.Y %>, <%= Model.Z %></span>
				<span class="online"><span class="<%= Model.IsOnline %>">&#x25C6;</span></span>
			</div>
			<div class="household">
				<h3>Household</h3>
				<div>
					<div><%= Html.Image(WebUtils.GetHouseholdImageUri(Model.HouseholdImageUri), Model.HouseholdName, new { width = 64, height = 64 })%></div>
					<div><%= Model.HouseholdName%></div>
				</div>
				<div>
					<div><%= Html.Image(WebUtils.GetRankImageUri(Model.RankImageUri), Model.RankName, new { width = 64, height = 64 })%></div>
					<div><%= Model.RankName%></div>
				</div>
			</div>
		</div>
	</div>
	<br class="clear" />
	
	<div class="box">
		<h1>Skills</h1>
		<% if (Model.Skills != null && Model.Skills.Count() > 0)
		{
			lstSkills.DataSource = Model.Skills.OrderByDescending(s => s.Value);
			lstSkills.DataBind();
		}%>
		<asp:DataList ID="lstSkills" runat="server" RepeatColumns="1" RepeatDirection="Horizontal" CssClass="table" CellPadding="4">
			<ItemTemplate>
				<span class="name"><%# WebUtils.FormatSkillName(Eval("Name"))%> : </span><span class="value"><%# Eval("Value", "{0:#0}") %></span>
			</ItemTemplate>
		</asp:DataList>
	</div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Character Details for <%= Model.Name %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
