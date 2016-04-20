<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

	<h1>Change Your Password</h1>
	
	<%= Html.ValidationSummary("Please correct the following errors and try again.", new { @class = "alert-error" })%>
	<% using (Html.BeginForm("ChangePassword", "Security", FormMethod.Post, new { id = "frmChangePwd" }))
	{ %>
	<div>
		<label for="currentPassword">Current Password: <%= Html.ValidationMessage("currentPassword", new { @class = "alert-error" })%></label>
		<div><%= Html.Password("currentPassword", null, new { length = 64, width = 200, tabindex = 1 })%></div>
		<label for="newPassword">New Password: <%= Html.ValidationMessage("newPassword", new { @class = "alert-error" })%></label>
		<div><%= Html.Password("newPassword", null, new { length = 64, width = 200, tabindex = 2 })%></div>
		<label for="confirmPassword">Confirm New Password: <%= Html.ValidationMessage("confirmPassword", new { @class = "alert-error" })%></label>
		<div><%= Html.Password("confirmPassword", null, new { length = 64, width = 200, tabindex = 3 })%></div>
		<div class="button">
			<a href="javascript:document.forms['frmChangePwd'].submit();" title="Change Password" tabindex="4">Change Password</a>
		</div>
		<div class="button">
			<%= Html.ActionLink("Cancel", "Index", "Home") %>
		</div>
	</div>
	<%} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Change Your Password
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Change your password." />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
