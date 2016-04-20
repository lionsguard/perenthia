<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.FeedViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Latest News</h1>

	<% foreach (var item in Model.Items)
	{
	%>
	<div class="feed-author">
		<%= item.Title.Text %><br />
		<%= item.PublishDate %>
	</div>
	<div>
		<%= item.Summary.Text %>
	</div>
	<br /><br />
	<%
	} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Latest News
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Stay up to date on the latest news and press releases regarding the persistent browser based role playing game Perenthia." />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
