<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

	<h1>Sign Up for a free Lionsguard ID</h1>
	<p class="title">A free Lionsguard ID will enable you to play any of the games offered by Lionsguard Technologies.</p>
	
	<%= Html.ValidationSummary("Please correct the following errors and try again.", new { @class = "alert-error"}) %>
	
    <%using (Html.BeginForm("SignUp", "Security", FormMethod.Post, new { id = "frmSignUp"}))
	{
	%>
	<p>Enter your Lionsguard ID user name or the email address used when the account was created in order to have your account details emailed to you.</p>
	<div class="signup">
		<fieldset>
			<legend>Age Verification</legend>
			<p>In order to create a Lionsguard ID you will need to verify your age. If you are under the age 
			of 18 you must get consent from a parent or guardian in order to proceed. Some games offered by 
			Lionsguard Technologies may have an age requirement.</p>
			<label for="birthDate">Birth Date:</label>
			<div class="field"><%= Html.TextBox("birthDate", Request.Form["birthDate"], new{ @class = "date-pick"}) %><%= Html.ValidationMessage("birthDate", new { @class = "alert-error" })%></div>
		</fieldset>
		
		<fieldset>
			<legend>Details</legend>
			<label for="username">Lionsguard ID:</label>
			<div class="field">
				<%= Html.TextBox("username", Request.Form["username"], new{ length = 256, @class = "username"}) %>
				<%= Html.ValidationMessage("username", new { @class = "alert-error"}) %>
			</div>
			<div class="help">Your Lionsguard ID must be a unique name that you will remember.</div>

			<label for="displayName">Display Name:</label>
			<div class="field">
				<%= Html.TextBox("displayName", Request.Form["displayName"], new { length = 32, @class = "display-name" })%>
				<%= Html.ValidationMessage("displayName", new { @class = "alert-error" })%>
			</div>
			<div class="help">Your display name will be the name that shows on forum posts, etc.</div>
	
			<label for="password">Password:</label>
			<div class="field">
				<%= Html.Password("password", null, new { length = 15, @class = "pwd" })%>
				<%= Html.ValidationMessage("password", new { @class = "alert-error" })%>
			</div>
			
			<label for="confirmPassword">Confirm Password:</label>
			<div class="field">
				<%= Html.Password("confirmPassword", null, new { length = 15, @class = "pwd" })%>
				<%= Html.ValidationMessage("confirmPassword", new { @class = "alert-error" })%>
			</div>
			
			<label for="email">Email Address:</label>
			<div class="field">
				<%= Html.TextBox("email", Request.Form["email"], new { length = 256, @class = "email" })%>
				<%= Html.ValidationMessage("email", new { @class = "alert-error" })%>
			</div>
			<div class="help">A valid email address is required for account activation.</div>
			
			<label for="securityQuestion">Security Question:</label>
			<div class="field">
				<%= Html.TextBox("securityQuestion", Request.Form["securityQuestion"], new { length = 80, @class = "security-question" })%>
				<%= Html.ValidationMessage("securityQuestion", new { @class = "alert-error" })%>
			</div>
			<div class="help">The security question and answer is used for password reset, should you forgot 
					your password you will be asked to answer your security question in order to reset your password.</div>
			
			<label for="securityAnswer">Security Answer:</label>
			<div class="field">
				<%= Html.TextBox("securityAnswer", Request.Form["securityQuestion"], new { length = 128, @class = "security-answer"})%>
				<%= Html.ValidationMessage("securityAnswer", new { @class = "alert-error" })%>
			</div>
		</fieldset>
	
		<div class="button">
			<a href="javascript:document.forms['frmSignUp'].submit();" title="Sign Up" tabindex="3">Sign Up</a>
		</div>
		<div class="button">
			<%= Html.ActionLink("Cancel", "Index", "Home") %>
		</div>
	</div>
	<p class="help">* Some games may require you to create a Character or Characters within the game itself. These games 
	will associate your Characters to your Lionsguard ID.</p>
	<p class="help"><strong>** Note: Creating multiple Lionsguard ID accounts is a violation of 
	the <a href="http://www.lionsguard.com/TermsOfService.aspx">terms of service</a>.</strong></p>
	<%
	}
	%>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Sign Up for a FREE Account
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Sign up for a free Lionsguard ID and begin playing Perenthia right away!" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
    <link rel="stylesheet" type="text/css" href="http://assets.lionsguard.com/jquery/themes/datePicker.css" media="screen, projection" />
    <style type="text/css">
		/* located in demo.css and creates a little calendar icon
		 * instead of a text link for "Choose date"
		 */
		a.dp-choose-date {
			float: left;
			width: 16px;
			height: 16px;
			padding: 0;
			margin: 5px 3px 0;
			display: block;
			text-indent: -2000px;
			overflow: hidden;
			background: url(http://assets.lionsguard.com/jquery/images/calendar.png) no-repeat; 
		}
		a.dp-choose-date.dp-disabled {
			background-position: 0 -20px;
			cursor: default;
		}
		/* makes the input field shorter once the date picker code
		 * has run (to allow space for the calendar icon
		 */
		input.dp-applied {
			width: 140px;
			float: left;
		}
    </style>
	<script type="text/javascript" src="http://assets.lionsguard.com/jquery/dates.js"></script>
	<script type="text/javascript" src="http://assets.lionsguard.com/jquery/jquery.datePicker-2.1.2.js"></script>
	<script type="text/javascript">
		Date.format = 'mm/dd/yyyy';
		$(function() {
			$('.date-pick')
		.datePicker(
			{ 
				startDate: '01/01/1940',
				endDate: (new Date()).asString()
			}
		)
		.bind(
			'focus',
			function() {
				$(this).dpDisplay();
			}
		).bind(
			'blur',
			function(event) {
				// works good in Firefox... But how to get it to work in IE?
				if ($.browser.mozilla) {

					var el = event.explicitOriginalTarget

					var cal = $('#dp-popup')[0];

					while (true) {
						if (el == cal) {
							return false;
						} else if (el == document) {
							$(this).dpClose();
							return true;
						} else {
							el = $(el).parent()[0];
						}
					}
				}
			}
		);
		});
	</script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
