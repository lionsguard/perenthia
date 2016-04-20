<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Lionsguard" %>
<%@ Import Namespace="Perenthia.Web" %>
<div id="menu">
	<div class="left">
		<ul>
			<% if (Page.User.Identity.IsAuthenticated)
			{
			%>
			<li><%= Html.ActionLink("PLAY", "Play", "Home")%></li>
			<%
			}
			else
			{
			%>
			<li><%= Html.ActionLink("LOGIN", "Login", "Security")%></li>
			<%
			} %>
			<li><img src="/common/images/link-divider.gif" alt="" /></li>
			<li><%= Html.ActionLink("ARMORIAL", "Search", "Armorial")%></li>
			<li><img src="/common/images/link-divider.gif" alt="" /></li>
			<li><%= Html.ActionLink("NEWS", "News", "Home")%></li>
			<li><img src="/common/images/link-divider.gif" alt="" /></li>
			<li><%= Html.ActionLink("HOME", "Index", "Home")%></li>
		</ul>
	</div>
	<div class="right">
		<ul>
			<li><%= Html.ActionLink("SCREENSHOTS", "Screenshots", "Media")%></li>
			<li><img src="/common/images/link-divider.gif" alt="" /></li>
			<li><a href="http://blog.perenthia.com" title="BLOG">BLOG</a></li>
			<li><img src="/common/images/link-divider.gif" alt="" /></li>
			<li><a href="<%= Lionsguard.Settings.ForumsUrl %>" title="FORUMS">FORUMS</a></li>
			<%--<li><img src="/common/images/link-divider.gif" alt="" /></li>
			<li><%= Html.ActionLink("MARKET", "Index", "Market")%></li>--%>
		</ul>
	</div>
</div>
