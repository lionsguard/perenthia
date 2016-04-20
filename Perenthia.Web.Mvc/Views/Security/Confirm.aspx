<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Account Verification</h1>
    <p>Enter your email address and the confirmation code from your account details email below to confirm your email address and activate your account.</p>
    
	<%= Html.ValidationSummary("Please correct the following errors and try again.", new { @class = "alert-error"}) %>
    <%using (Html.BeginForm("Confirm", "Security", FormMethod.Post, new { id = "frmConfirm"}))
	{
	%>
	<p>Enter your Lionsguard ID user name or the email address used when the account was created in order to have your account details emailed to you.</p>
	<div>
		<label for="email">Email Address: <%= Html.ValidationMessage("email", new { @class = "alert-error"}) %></label>
		<div><%= Html.TextBox("email", Request.Form["email"], new{ length = 256, width = 200, tabindex = 1}) %></div>
		<label for="code">Activation Code: <%= Html.ValidationMessage("code", new { @class = "alert-error" })%></label>
		<div><%= Html.TextBox("code", Request.Form["code"], new { length = 256, width = 200, tabindex = 2 })%></div>
		<div class="button">
			<a href="javascript:document.forms['frmConfirm'].submit();" title="Activate Account" tabindex="3">Activate Account</a>
		</div>
		<div class="button">
			<%= Html.ActionLink("Cancel", "Index", "Home") %>
		</div>
	</div>
	<%
	}
	%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Account Verification
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Activte your Lonsguard ID account and being playing Perenthia." />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
