<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage<Radiance.Contract.AvatarData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h2>Creature</h2>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
	<%= Model.Name %>
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="<%= Model.Description %>" />
</asp:Content>
