<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <h1>About Perenthia</h1>
    <p>
        Perenthia is a <a href="http://www.pbbg.org" target="_blank" title="PBBG">persistent
            browser based game</a> set in a fantasy world filled with magic and monsters.
        Perenthia offers progressive game play in an attempt to move away from the level
        treadmill and grind of typical games of the genre.</p>
    <p>
        Perenthia is played in the browser using the <a href="http://www.silverlight.net"
            target="_blank" title="Silverlight">Microsoft Silverlight</a> plugin but does
        send information back and forth over the internet so an internet connection is required.
        Perenthia incorporates an in-game currency called Emblem that can be used to purchase
        special items, participate in events, purchase additional characters, etc.&nbsp;</p>
    <p>
        The general advancement and game play revolve around completing quests and tasks
        centered on the theme of the game. As your character progresses through the various
        levels the game play will change at set intervals. For instance, at your starting
        level of 1 you will have to kill monsters to complete quests and earn experience.
        At level 15 the kill monsters to gain experience to level to get better equipment
        changes to more specific tasks and will continue to change along these lines through
        out the game experience.&nbsp;</p>
    <p>
        &nbsp;</p>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphTitle" runat="server">
    About Perenthia
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="An overall view of Perenthia, what it is, how to play, etc." />
</asp:Content>
