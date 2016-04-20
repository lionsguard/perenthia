<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Mobile.Master" Inherits="System.Web.Mvc.ViewPage<Lionsguard.Models.ErrorInfoViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Page Not Found</h1>
    <p>The page you are looking for could not be found. It may have been removed or had its named changed. Please use the links below to locate the information.</p>

	<%= Html.SiteMap() %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Page Not Found
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
