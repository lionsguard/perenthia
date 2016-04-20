<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

    <h1>Household Information</h1>

	<p>A Household is a player run group with custom defined ranks and titles of advancement. Households may also form alliances or declare war on other Households, enabling the members to participate in player versus player combat. Household player versus player combat will earn your Household Honor Points and increase the Household's position in the public rankings.</p>
	<p>Membership in a Household determines the charter name and title of the Head of Household. The Head of Household can appoint up to 15 custom titles and grant Household related authority to each. Once a Household gains enough members to become a Manor a land deed may be purchased, allowing the Household to setup and start a city within the Perenthia world.</p>
	<p>&nbsp;</p>
	<h2>Household Charters</h2>
	
	<table cellpadding="4" cellspacing="0" border="0" rules="all" class="table">
		<thead>
			<tr>
				<th>Charter Name</th>
				<th>Required Members</th>
				<th>Head of Household Title</th>
			</tr>
		</thead>
		<tbody>
			<tr>
				<td>Household</td>
				<td>5 members</td>
				<td></td>
			</tr>
			<tr>
				<td>Club</td>
				<td>15 members</td>
				<td></td>
			</tr>
			<tr>
				<td>Clan</td>
				<td>25 members</td>
				<td>Chieftain</td>
			</tr>
			<tr>
				<td>Guild</td>
				<td>50 members</td>
				<td>Master/Mistress</td>
			</tr>
			<tr>
				<td>Manor</td>
				<td>100 members</td>
				<td>Lord/Lady</td>
			</tr>
			<tr>
				<td>Shire</td>
				<td>150 members</td>
				<td>Reeve</td>
			</tr>
			<tr>
				<td>Township</td>
				<td>300 members</td>
				<td>Mayor</td>
			</tr>
			<tr>
				<td>City</td>
				<td>800 members</td>
				<td>Governor</td>
			</tr>
			<tr>
				<td>Barony</td>
				<td>1500 members</td>
				<td>Baron/Baroness</td>
			</tr>
			<tr>
				<td>Viscounty</td>
				<td>2500 members</td>
				<td>Viscount/Viscountess</td>
			</tr>
			<tr>
				<td>County</td>
				<td>5000 members</td>
				<td>Count/Countess</td>
			</tr>
			<tr>
				<td>March</td>
				<td>8000 members</td>
				<td>Marqui/Marchoiness</td>
			</tr>
			<tr>
				<td>Duchy</td>
				<td>12000 members</td>
				<td>Duke/Duchess</td>
			</tr>
			<tr>
				<td>Principality</td>
				<td>25000 members</td>
				<td>Prince/Princess</td>
			</tr>
			<tr>
				<td>Realm</td>
				<td>50000 members</td>
				<td>King/Queen</td>
			</tr>
			<tr>
				<td>Empire</td>
				<td>100000 members</td>
				<td>Emperor/Empress</td>
			</tr>
		</tbody>
	</table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
Household Information
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Households are player run groups where players can form alliances with other Households, declare war on other Households, create custom titles of advancement and gain points of honor when members perform feats and tasks." />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScript" runat="server">
</asp:Content>
