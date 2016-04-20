<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/MobileClean.Master" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.PlayViewData>" %>

<asp:Content ID="Content4" ContentPlaceHolderID="cphMain" runat="server">

<%--AJAX UI--%>
<% using (Html.BeginForm("Play", "Home", FormMethod.Post, new { id ="form1" }))
   { %>
<div>
	<div></div>
	<div><%= Html.TextBox("command", null, new { length = 64, width = 120, height = 24 })%></div>
</div>
<%} %>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
    <link rel="stylesheet" type="text/css" href="/common/css/mobile-game.css" media="screen, projection" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Perenthia
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
