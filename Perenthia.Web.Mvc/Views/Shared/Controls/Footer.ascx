<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="footer">
	<div>This site and all content is copyright &copy; 2003-<%= DateTime.Now.Year %> by Cameron Albert and Lionsguard Technologies, LLC.</div>
	<div>
		<ul>
			<li><%= Html.ActionLink("Armorial", "Search", "Armorial","Search for characters, households, items and creatures.") %></li>
			<li>|</li>
			<li><%= Html.ActionLink("Screenshots", "Screenshots", "Media", "Screenshots of the character creation process including some user submitted game play images.") %></li>
			<li>|</li>
			<li><%= Html.ActionLink("News", "News", "Home", "The latest news and press releases for the browser based game Perenthia.") %></li>
			<li>|</li>
			<li><%= Html.ActionLink("Site Map", "SiteMap", "Home", "A list of the sections of the Perenthia web site for easy navigation.") %></li>
			<li>|</li>
			<li><%= Html.ActionLink("Home", "Index", "Home", "The Perenthia home page.") %></li>
		</ul>
	</div>
	<div>Artwork and Illustrations by <a href="http://www.antongustilo.com" target="_blank" title="Anton Gustilo">Anton Gustilo</a></div>
	<div><a href="http://www.browser-games-hub.org/" target="_blank"><img src="http://www.browser-games-hub.org/images/browsergameshub.png" alt="http://www.browser-games-hub.org/" title="Browser Games Hub" /></a></div>
</div>
