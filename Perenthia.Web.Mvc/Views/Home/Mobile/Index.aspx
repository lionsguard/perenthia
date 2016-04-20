<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Mobile.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <% Html.RenderPartial("/Views/Shared/Controls/HomeContent.ascx"); %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
	Persistent Browser Based Multiplayer Action and Adventure Game
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
