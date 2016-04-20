<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Clean.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

	<% if (ViewData["Message"] != null)
	{
		%>
	<p class="alert-success"><%= ViewData["Message"] %></p>
		<%
	} %>

	<div id="login-container">
		<div id="login-header"><img src="/common/images/logo.png" alt="Perenthia" width="300" height="88" /></div>
		<div id="login-content">
			<fieldset>
				<legend>Login</legend>
					<div class="login">
						<%= Html.ValidationSummary("Login was unsuccessful. Please correct the errors and try again.", new { @class = "alert-error" })%>
						<% using (Html.BeginForm("Login", "Security", FormMethod.Post, new { id = "frmLogin" })) { %>
						<label for="UserName">User Name: <%= Html.ValidationMessage("username", new { @class = "alert-error" }) %></label>
						<div class="field"><%= Html.TextBox("username", Request.Form["username"], new { length = 256, @class = "username", tabindex = 1 })%></div>
						<label for="Password">Password: <%= Html.ValidationMessage("password", new { @class = "alert-error" }) %></label>
						<div class="field"><%= Html.Password("password", null, new { length = 256, @class = "password", tabindex = 2 })%></div>
						<div class="field"><%= Html.CheckBox("rememberMe", true, new { tabindex = 3 })%>&nbsp;<label for="rememberMe">Remember me next time?</label></div>
						<div class="button">
							<a href="javascript:document.forms['frmLogin'].submit();" title="Login" tabindex="4">Login</a>
						</div>
						<div class="button">
							<%= Html.ActionLink("Cancel", "Index", "Home") %>
						</div>
						<div class="links">
							OR<br /> 
							<a class="rpxnow" onclick="return false;"
								href="<%= Perenthia.Web.WebUtils.GetRpxLoginUrl() %>?token_url=<%= Server.UrlEncode(String.Concat(Lionsguard.Util.GetServerUrl(this.Context), "/AuthVerify")) %>">login with another service</a>
						</div>
						<div class="links">
							<%= Html.ActionLink("Need a user name?", "SignUp", "Security", new { tabindex = 5 })%><br />
							<%= Html.ActionLink("Forgot your password?", "ForgotPassword", "Security", new { tabindex = 6 })%><br />
						</div>
						<%} %>
					</div>
			</fieldset>
		</div>
	</div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Login
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Login to Perenthia using your Lionsguard ID account." />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
