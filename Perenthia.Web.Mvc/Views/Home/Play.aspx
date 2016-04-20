<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Clean.Master" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.PlayViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
<% Response.Redirect(WebUtils.GetPlayUri(), true); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Perenthia
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Download and install Silverlight to begin playing Perenthia!" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
