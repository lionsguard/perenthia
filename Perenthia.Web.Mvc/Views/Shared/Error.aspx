<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage<Lionsguard.Models.ErrorInfoViewData>" %>

<asp:Content ID="errorTitle" ContentPlaceHolderID="cphTitle" runat="server">
   Internal Error
</asp:Content>

<asp:Content ID="errorContent" ContentPlaceHolderID="cphMain" runat="server">
    <h1>Internal Error</h1>

	<div><p class="alert-error">The forces are darkness are at work again! An internal error has occured in the application. The guard has been notified and will take the appropriate measures to win back the day!</p></div>

<%
	if (Model.DisplayErrorDetails)
	{
		Response.Write(String.Format("<p>{0}</p>", Model.Exception.ToString()));
	}
	%>
</asp:Content>
