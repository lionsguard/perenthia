<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Mobile.Master" Inherits="System.Web.Mvc.ViewPage<Radiance.Contract.ItemData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1><%= Model.Name %></h1>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
<%= Model.Name %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
