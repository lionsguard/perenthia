<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Linq" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">

<div style="margin:0px auto;">
	<h1>Attributes</h1>
		<p>A Character’s attributes provide the base difficulty for all skill tests. What this means for your Character is that when a skill test is performed by the game engine the attribute value is compared to the skill value to determine if the skill was successfully used before the difficulty of the action is applied. </p>
		<br /><br />
		<p>There are eight (8) attributes that are used to perform actions within the game world. These attributes are set upon Character creation and do not change. </p>
		<br /><br />
		<p>Attributes are separated into two categories, physical and mental:</p>
		<table cellpadding="4" cellspacing="0" border="0">
			<tr>
				<th colspan="2"><h2>Physical</h2></th>
				<th colspan="2"><h2>Mental</h2></th>
			</tr>
			<tr>
				<td align="right" valign="top"><h3>Strength</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Strength") %></td>
				<td align="right" valign="top"><h3>Intelligence</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Intelligence")%></td>
			</tr>
			<tr>
				<td align="right" valign="top"><h3>Dexterity</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Dexterity")%></td>
				<td align="right" valign="top"><h3>Perception</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Perception")%></td>
			</tr>
			<tr>
				<td align="right" valign="top"><h3>Stamina</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Stamina")%></td>
				<td align="right" valign="top"><h3>Endurance</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Endurance")%></td>
			</tr>
			<tr>
				<td align="right" valign="top"><h3>Beauty</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Beauty")%></td>
				<td align="right" valign="top"><h3>Affinity</h3></td>
				<td valign="top"><%= Perenthia.Web.Game.GetAttributeDescription("Affinity")%></td>
			</tr>
		</table>
		<br /><br />

		<h1>Skills</h1>
		<p>Perenthia is a skills based game; wherein your Character may train skills in order to perform actions such as ride a horse, wear plate armor, use swords, etc. Attributes of your Character are used as a base difficulty for skill tests. </p>
		<br /><br />
		<p>What this means for you as a Character is that you can train any skill! If you wish to be a spell caster that can wear plate armor all that is required is to spend Skill Points in the Plate Armor skill along with Skill Points in the various magical skills.</p>
		<br /><br />
		<p>Your Character is considered unskilled if the skill value is zero (0). Some items and actions 
		only require that a skill be learned so spending at least one (1) point in the skills that interest 
		you would allow these items and actions to be used or performed.</p>
		<br /><br />
		<%
            if (ViewData["Skills"] != null)
            {
                var skills = (ViewData["Skills"] as List<Radiance.Contract.SkillData>).OrderBy(s => s.GroupName);
                string groupName = String.Empty;
                foreach (var skill in skills)
                {
                    if (skill.GroupName != groupName)
                    {
                        groupName = skill.GroupName;
                        %>
                        <br /><br />
                        <h2><%= groupName %></h2>
                        <%
                    }
                    %>
                    <p class="heading"><%= skill.Name %></p>
                    <p><%= skill.Description %></p>
                    <br />
                    <%
                }
            }
		 %>
</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    Attributes and Skills
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Attributes are stats that impact the actions you can perform. Skills determine the actions that can be performed and the rate of success of those actions." />
</asp:Content>
