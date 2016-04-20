<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Clean.Master" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.RpxViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <div class="info">
        <p>You need to associate this service provider account to your Lionsguard ID account. Simply login within your Lionsguard ID Username and Password. </p>
        <p>You can associated your Lionsguard ID account with multiple services such as Google, Facebook, OpenID and Windows Live. You will only need 
        to perform this association once for each of the services.</p>
        <p>Once you complete this step you may login with your prefered service each time you visit.</p>
    </div>
	<div id="login-container">
		<div id="login-header"><img src="/common/images/logo.png" alt="Perenthia" width="300" height="88" /></div>
		<div id="login-content">
			<fieldset>
				<legend>Login</legend>
					<div class="login">
						<%= Html.ValidationSummary("Login was unsuccessful. Please correct the errors and try again.") %>
						<% using (Html.BeginForm("AuthVerify", "Home", FormMethod.Post, new { id = "frmLogin" })) 
						{
							Response.Write(Html.Hidden("rpxId", Request.Params["rpxId"]));
						%>
						<label for="UserName">User Name: <%= Html.ValidationMessage("username") %></label>
						<div class="field"><%= Html.TextBox("username", Request.Form["username"], new { length = 256, @class = "username", tabindex = 1 })%></div>
						<label for="Password">Password: <%= Html.ValidationMessage("password") %></label>
						<div class="field"><%= Html.Password("password", null, new { length = 256, @class = "password", tabindex = 2 })%></div>
						<div class="button">
							<a href="javascript:document.forms['frmLogin'].submit();" title="Login" tabindex="3">Login</a>
						</div>
						<%} %>
					</div>
			</fieldset>
		</div>
	</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    Associate Service Provider Account to Lionsguard ID Account
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
