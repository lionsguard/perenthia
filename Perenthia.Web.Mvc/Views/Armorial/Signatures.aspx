<%@ Page Title="" Language="C#" ValidateRequest="false" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Armorial Signatures</h1>
    
    <p>Add character information to your forum signatures, blogs, personal sites, fan sites, etc.</p>
    
    <p>Enter your character name and choose a signature type to generate the HTML code required to show your character image:</p>
    
    <% using (Html.BeginForm("Signatures", "Armorial", FormMethod.Get)){  %>
		Character Name: <%= Html.TextBox("name", Request.Params["name"], new{ length = 64, width = "150", tabindex = 1}) %>
		&nbsp;&nbsp;
		Signature Type: <%= Html.DropDownList("type", new SelectListItem[]{ new SelectListItem{ Text="Standard", Value="" }, new SelectListItem{ Text="XL", Value="xl"}}, new{ tabindex=2}) %>
		<br />
		<span class="button"><a href="javascript:document.forms[0].submit();" title="Generate HTML" tabindex="3">Generate HTML</a></span>
		<br /><br />
		
	<%
	   
		if (Request.Params["name"] != null)
		{%>
		<h3>HTML Code</h3>
		<%= Html.TextArea("code", (string)ViewData["code"], new { style="width:550px;height:100px;" })  %>
		<br /><br />
		<script type="text/javascript">
			var code = document.getElementById("code");
			if (code != null)
				code.select();
		</script>
		<h3>Sample</h3>
		<img src="/Armorial/Character/<%= Request.Params["name"] %>/<%= Request.Params["type"] %>" alt="Character Signature" />
		   <%
		}
	   } %>
    
    <p>&nbsp;</p>
    <p>&nbsp;</p>
    <p>&nbsp;</p>
    <h2>Examples</h2>
    <h3>Standard Signature</h3>
    <p>This signature returns a PNG image of your character. Use the format (http://www.perenthia.com/Armorial/Character/charactername)</p>
    <img src="/Armorial/Character/Aldarian" alt="Character Signature" />
    <br />
    
    
    <h3>XL Signature</h3>
    <p>This signature returns a PNG image of your character. Use the format (http://www.perenthia.com/Armorial/Character/charactername/xl)</p>
    <img src="/Armorial/Character/Aldarian/xl" alt="Character Signature" />
    <br />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Armorial Signatures
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMeta" runat="server">
<meta name="description" content="Instructions on how to add character information to forums, blogs, personal sites, fan sites, etc." />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
