<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Screen Shots</h1>
		<table cellpadding="4" cellspacing="4" border="0">
			<tr>
				<td align="center"><a href="javascript:popThumb('Character Creation - Choosing Race', '/Common/images/screenshots/perenthia-ss-ui-cc-race.png', 762, 537);"><img src="/Common/images/screenshots/perenthia-ss-ui-cc-race-thumb.png" alt="Character Creation - Choosing Race" /></a></td>
				<td align="center"><a href="javascript:popThumb('Character Creation - Settting Attributes', '/Common/images/screenshots/perenthia-ss-ui-cc-attributes.png', 762, 537);"><img src="/Common/images/screenshots/perenthia-ss-ui-cc-attributes-thumb.png" alt="Character Creation - Settting Attributes" /></a></td>
				<td align="center"><a href="javascript:popThumb('Character Creation - Settting Skills', '/Common/images/screenshots/perenthia-ss-ui-cc-skills.png', 762, 537);"><img src="/Common/images/screenshots/perenthia-ss-ui-cc-skills-thumb.png" alt="Character Creation - Settting Skills" /></a></td>
			</tr>
			<tr>
				<td align="center">Character Creation - Choosing Race</td>
				<td align="center">Character Creation - Settting Attributes</td>
				<td align="center">Character Creation - Settting Skills</td>
			</tr>
			<tr>
				<td align="center"><a href="javascript:popThumb('Character Sheet', '/Common/images/screenshots/perenthia-ss-ui-charsheet.png', 995, 616);"><img src="/Common/images/screenshots/perenthia-ss-ui-charsheet-thumb.png" alt="Character Sheet" /></a></td>
				<td align="center"><a href="javascript:popThumb('Combat', '/Common/images/screenshots/perenthia-ss-ui-combat1.png', 995, 619);"><img src="/Common/images/screenshots/perenthia-ss-ui-combat1-thumb.png" alt="Combat" /></a></td>
				<td align="center"><a href="javascript:popThumb('Merchant', '/Common/images/screenshots/perenthia-ss-ui-merchant.png', 996, 617);"><img src="/Common/images/screenshots/perenthia-ss-ui-merchant-thumb.png" alt="Merchant" /></a></td>
			</tr>
			<tr>
				<td align="center">Character Sheet</td>
				<td align="center">Combat</td>
				<td align="center">Merchant</td>
			</tr>
			<tr>
				<td align="center"><a href="javascript:popThumb('Looting', '/Common/images/screenshots/perenthia-ss-ui-looting.png', 994, 616);"><img src="/Common/images/screenshots/perenthia-ss-ui-looting-thumb.png" alt="Looting" /></a></td>
				<td align="center"><a href="javascript:popThumb('Combat Stats', '/Common/images/screenshots/perenthia-ss-ui-combat2.png', 996, 613);"><img src="/Common/images/screenshots/perenthia-ss-ui-combat2-thumb.png" alt="Combat Stats" /></a></td>
				<td align="center"></td>
			</tr>
			<tr>
				<td align="center">Looting</td>
				<td align="center">Combat Stats</td>
				<td align="center"></td>
			</tr>
		</table>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Screenshots
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Screen shots of Perenthia, including the character creation process and some in-game actions shots submitted by a player!" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
