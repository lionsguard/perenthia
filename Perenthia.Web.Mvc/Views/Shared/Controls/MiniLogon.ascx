<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="mini-login">
<%
    if (Request.IsAuthenticated) {
%>
    <div class="button"><a href="<%= Perenthia.Web.WebUtils.GetPlayUri() %>">Play Now!</a></div><br />
	<%= Html.ActionLink("Change Password", "ChangePassword", "Security", "Change your Lionsguard ID password.")%><br />
	<%= Html.ActionLink("Logout", "Logout", "Security", "Logout")%>
<%
    }
    else {
%> 
<% using (Html.BeginForm("Login", "Security", FormMethod.Post, new { id = "frmLogin" })) { %>
	<div class="login">
		<%= Html.Hidden("rememberMe", true) %>
		<label for="username">Username:</label><br />
		<%= Html.TextBox("username", String.Empty, new { length = "256", width = "100px" })%><br />
        <label for="password">Password:</label><br />
        <%= Html.Password("password", String.Empty, new { length = "256", width = "100px" })%><br />
        
        <div class="button"><a href="javascript:document.forms['frmLogin'].submit();">Login</a></div><br />
		<%= Html.ActionLink("Forgot your password?", "ForgotPassword", "Security", "Forgot your password? Recover it here.")%><br /><br />
        or<br /><a class="rpxnow" onclick="return false;"
           href="<%= Perenthia.Web.WebUtils.GetRpxLoginUrl() %>?token_url=<%= Server.UrlEncode(String.Concat(Lionsguard.Util.GetServerUrl(this.Context), "/AuthVerify")) %>">login with another service</a>
        <% } %>
	</div>
	<div class="signup">
		<div class="button"><%= Html.ActionLink("Sign Up", "SignUp", "Security", "Sign up for a free Lionsguard ID.")%></div>
	</div>
<%
    }
%>
</div>
