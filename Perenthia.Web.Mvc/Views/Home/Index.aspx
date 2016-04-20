<%@ Page Language="C#" MasterPageFile="~/Views/Shared/MasterPages/Public.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cnTitle" runat="server" ContentPlaceHolderID="cphTitle">
    Persistent Browser Based Silverlight Game and Multiplayer Action and Adventure
</asp:Content>
<asp:Content ID="cnMeta" runat="server" ContentPlaceHolderID="cphMeta">
	<meta name="description" content="Perenthia is a fantasy text based adventure game or persistent browser based game created with Microsoft Silverlight where players can immerse themselves in a world filled with adventure, magic and danger. Players can interact with other players by forming player run groups called households, real time chat, private messaging and built in forums. Solo players can adventure into randomly generated dungeons to seek fame and glory." />
</asp:Content>
<asp:Content ID="cnMain" ContentPlaceHolderID="cphMain" runat="server">

    <% Html.RenderPartial("/Views/Shared/Controls/HomeContent.ascx"); %>
	
	<div class="content-left">
		<div class="box">
			<h1>CHARACTER SPOTLIGHT</h1>
			<% 
			if (ViewData["FeaturedCharacter"] != null)
			{
				var player = ViewData["FeaturedCharacter"] as Radiance.Contract.AvatarData;
				if (player != null)
				{ 
			%>
			<div class="avatar">
				<div class="image"><img src="/common/images/races/avatar-<%= player.Race %>-<%= player.Gender %>.png"
									alt="<%= player.Name %>" width="96" height="96" /></div>
				<div class="details">
					<div class="name">
						<%= Html.ActionLink(String.Format("{0} ({1})", player.Name, player.Level), "Character", "Armorial", String.Format("Display details for {0}", player.Name), new { name = player.Name, type = "Detail" }, null)%>
					</div>
					<div class="race">
						<%= player.Race%>
						<%= player.Gender%>
					</div>
					<div class="location">
						<%= player.Zone%>:
						<%= player.X%>,
						<%= player.Y%>,
						<%= player.Z%>
					</div>
					<div class="<%= (player.IsOnline) ? "positive" : "negative" %>">
						<%= (player.IsOnline) ? "ONLINE" : "OFFLINE"%></div>
				</div>
				<br class="clear" />
			</div>
			<%
				}
			}%>
		</div>
	
		<div class="box">
			<h1>RECENT FORUM POSTS</h1>
			<div>
				<% if (ViewData["ForumTopics"] != null)
				   {
					   foreach (var topic in (ViewData["ForumTopics"] as List<Lionsguard.Forums.Topic>))
					   {    
						   %>
				<div class="row">
					<div class="feed">
						<span class="feed-title">
							<a href="<%= String.Format("{0}/Default.aspx?g=posts&t={1}", Lionsguard.Settings.ForumsUrl, topic.ID)%>" title="<%= topic.Title %>"><%= topic.Title %></a>
						<div class="feed-text">
							<%= topic.PostDate %><br />
							<em><%= topic.ReplyCount %> Replies, <%= topic.ViewCount %> Views</em><br />
						</div>
						<span class="feed-author"><%= topic.DisplayName %></span>
						<span class="feed-text">on <%= topic.ForumName %></span><br />
					</div>
				</div>
				<br />
						   <%
					   }
					   %>
				<%} %>
			</div>
		</div>
	</div>
	<div class="content-right">
		<div class="box">
			<h1>LATEST NEWS</h1>
			<div>
				<% 
				var items = ViewData["BlogFeedItems"] as IEnumerable<System.ServiceModel.Syndication.SyndicationItem>;
				if (items != null)
				{
					   foreach (var item in items)
					   {
					%>
					<div class="feed">
						<div class="feed-author">
							<%= item.PublishDate%>
						</div>
						<div class="feed-text">
							<a href="<%= item.Id %>" title="<%= item.Title.Text %>"><%= item.Title.Text%></a>
						</div>
					</div>
					<br />
					<%
					   }
				 } %>
			</div>
		</div>
	</div>
</asp:Content>
