<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.PaymentViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
	
    <iframe frameborder="0" scrolling="no" width="425" height="365" src="http://api.jambool.com/payments/v1/<%= Model.OfferId %>/buy_with_socialgold?<%= Model.Parameters %>" ></iframe>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMeta" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
