<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Getting Started with Perenthia</h1>
    
    <% Html.RenderPartial("/Views/Shared/Controls/GettingStartedContent.ascx"); %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Getting Started with Perenthia
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMeta" runat="server">
<meta name="description" content="A guide to getting started with Perenthia; creating characters, how the game works, the commands, the user interface and interacting with other players is all covered to make your start in Perenthia a quick and easy journey." />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
