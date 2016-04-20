<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <h1>Frequently Asked Questions</h1>
    <ul>
        <li><a href="#what">What is Perenthia?</a></li>
        <li><a href="#plugin">What plugins do I need to play?</a>&nbsp;</li>
    </ul>
    <p>&nbsp;</p>
    <a name="what" title="what"></a>
    <h2>What is Perenthia?</h2>
    <p>Perenthia is a text-based adventure game or persistent browser based game that you
        play over your internet connection through the use of a web browser such as Microsoft
        Internet Explorer or Mozilla Firefox. Perenthia is set in a fantasy world with dragons,
        knights and magic.</p>
    <p>&nbsp;</p>
    <a name="plugin" title="plugin"></a>
    <h2>What plugins do I need to play?</h2>
    <p>Perenthia requires the Microsoft Silverlight plugin.&nbsp;</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    Frequently Asked Questions
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Frequently asked questions and answers related to Perenthia, the game world, required plugins, etc." />
</asp:Content>
