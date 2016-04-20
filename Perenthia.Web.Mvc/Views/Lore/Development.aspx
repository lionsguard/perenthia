<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <h1>Perenthia Development Information</h1>
    <p>Perenthia has been developed using a combination of Microsoft Technologies.</p>

    <ul>
        <li>The backend database is Microsoft SQL Server 2008 and makes heavy use of the XML Data Type.</li>
        <li>The game engine/server is programmed in C# 3.5 and is accessed from the core web site that is ASP.NET C# 3.5.</li>
        <li>The game client or user interface is programmed in Microsoft Silverlight, using C# in the XAML code behind pages.</li>
    </ul>
   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    Perenthia Development Information and Technologies
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Perenthia is a Silverlight game written in C# with an MS SQL 2008 backend." />
</asp:Content>
