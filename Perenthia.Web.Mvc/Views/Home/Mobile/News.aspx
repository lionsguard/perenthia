<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Mobile.Master" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.FeedViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Latest News</h1>

	<% foreach (var item in Model.Items)
	{
	%>
	<div class="box">
		<h1><%= item.Title.Text %></h1>
		<div class="heading">
			<%= item.PublishDate %>
		</div>
		<div class="content">
			<%= item.Summary.Text %>
		</div>
	</div>
	<%
	} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
	Latest News
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
