<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Mobile.Master" Inherits="System.Web.Mvc.ViewPage<Radiance.Contract.HouseholdData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1><%= Model.Name %></h1>
    
    <div>
		<%= Html.Image(Model.ImageUri, Model.Name, new{ width = 128, height = 128}) %>
    </div>
    
    <div class="household">
		<div class="name"><%= Model.Name %></div>
		<div class="motto">"<%= Model.Motto %>"</div>
		<div class="desc"><%= Model.Description %></div>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
<%= Model.Name %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
