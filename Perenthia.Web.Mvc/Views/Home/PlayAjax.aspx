<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Clean.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <% Html.RenderPartial("/Views/Shared/Controls/AjaxClient.ascx"); %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">Perenthia</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMeta" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphHead" runat="server">
    <link rel="stylesheet" type="text/css" href="/common/css/game-ajax.css" media="screen, projection" />
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
