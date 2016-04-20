<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Forgot Your Password?</h1>

	<%= Html.ValidationSummary("Please correct the errors and try again.", new { @class = "alert-error"})%>
	<% 
		using (Html.BeginForm("ForgotPassword", "Security", FormMethod.Post, new { id = "frmForgotPwd" }))
		{
		%>
		<p>Enter your Lionsguard ID user name or the email address used when the account was created in order to have your account details emailed to you.</p>
		<div>
			<div>
				<fieldset>
					<legend>Lionsguard ID User Name</legend>
					<%= Html.ValidationMessage("username", new { @class = "alert-error" })%>
					<div><%= Html.TextBox("username", Request.Form["username"], new { length = 256, width = 200, tabindex = 1 })%></div>
				</fieldset>
			</div>
			OR
			<div>
				<fieldset>
					<legend>Email Address</legend>
					<%= Html.ValidationMessage("email", new { @class = "alert-error" })%>
					<div><%= Html.TextBox("email", Request.Form["email"], new { length = 256, width = 200, tabindex = 2 })%></div>
				</fieldset>
			</div>
			<div class="button">
				<a href="javascript:document.forms['frmForgotPwd'].submit();" title="Next" tabindex="3">Next</a>
			</div>
			<div class="button">
				<%= Html.ActionLink("Cancel", "Index", "Home") %>
			</div>
		</div>
		<%
			} %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    Forgot Password
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Forgot your password? You can retrieve it by providing your username or email address and answering your security question." />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
