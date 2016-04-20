<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage<Lionsguard.Models.SecurityAnswerViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Forgot Your Password?</h1>

	<%= Html.ValidationSummary("Please correct the errors and try again.", new { @class = "alert-error"})%>
	<% 
		using (Html.BeginForm("ResendPassword", "Security", FormMethod.Post, new { id = "frmResendPwd" }))
		{
		%>
		<%= Html.Hidden("username", Model.UserName) %>
		<%= Html.Hidden("securityQuestion", Model.SecurityQuestion)%>
		<p>You will need to provide the answer to the security question displayed below:</p>
		<p>"<%= HttpUtility.UrlDecode(Model.SecurityQuestion) %>"</p>
		<div>
			<div>
				<%= Html.ValidationMessage("securityAnswer", new { @class = "alert-error" })%>
				<div><%= Html.Password("securityAnswer", null, new { length = 256, width = 200, tabindex = 1 })%></div>
			</div>
			<div class="button">
				<a href="javascript:document.forms['frmResendPwd'].submit();" title="Resend Password" tabindex="2">Resend Password</a>
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

<asp:Content ID="Content3" ContentPlaceHolderID="cphMeta" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
