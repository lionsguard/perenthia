<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="game-container" style="display:none;">
	<div id="game-header">
		<div><%--PLAYER--%></div>
		<div><%--TARGET--%></div>
	</div>
	<div id="game-content">
		<div><%--ROOM--%></div>
		<div><%--MAP--%></div>
		<div><%--NAV--%></div>
	</div>
	<div id="game-footer">
		<ul>
			<li><a href="javascript:action(ACTION_CHAR_SHEET);"><img src="/common/media/avatar-blank.png" alt="CHARACTER" id="btnCharacter" width="32" height="32" /></a></li>
			<li><a href="javascript:action(ACTION_CHAT);"><img src="/common/media/ui/menu-chat.png" alt="CHAT" width="32" height="32" /></a></li>
			<li><a href="javascript:action(ACTION_SPELLS);"><img src="/common/media/ui/menu-spells.png" alt="SPELLS" width="32" height="32" /></a></li>
			<li><a href="javascript:action(ACTION_INVENTORY);"><img src="/common/media/ui/menu-inventory.png" alt="INVENTORY" width="32" height="32" /></a></li>
			<li><a href="javascript:action(ACTION_CONSOLE);"><img src="/common/media/ui/menu-console.png" alt="CONSOLE" width="32" height="32" /></a></li>
			<li><a href="javascript:action(ACTION_HELP);"><img src="/common/media/ui/menu-help.png" alt="HELP" width="32" height="32" /></a></li>
			<li><a href="javascript:action(ACTION_QUIT);"><img src="/common/media/ui/menu-quit.png" alt="QUIT" width="32" height="32" /></a></li>
		</ul>
	</div>
</div>

<%--CONSOLE WINDOW--%>
<div id="game-console" style="display:none;"></div>

<%--COMBAT WINDOW--%>
<div id="game-combat" style="display:none;"></div>

<%--COMBAT WINDOW--%>
<div id="game-login">
	
</div>



<script type="text/javascript">
	// ACTIONS
	var ACTION_CHAR_SHEET = 0;
	var ACTION_CHAT = 1;
	var ACTION_SPELLS = 2;
	var ACTION_INVENTORY = 3;
	var ACTION_CONSOLE = 4;
	var ACTION_HELP = 5;
	var ACTION_QUIT = 6;

	// COMMANDS
	var CMD_CHARACTER = "CHARACTER";

	// OBJECTS
	var player = null;
	var target = null;


	function action(type) {
		switch (type) {
			case ACTION_CHAR_SHEET:
				if (player == null) {
					execute(CMD_CHARACTER, "");
				}
				break;
			case ACTION_CHAT:
				break;
			case ACTION_CONSOLE:
				break;
			case ACTION_HELP:
				break;
			case ACTION_INVENTORY:
				break;
			case ACTION_QUIT:
				break;
			case ACTION_SPELLS:
				break;
		}
	}
	
	function execute(cmd, args) {
	}


	// SERVICE PROXY
	function serviceProxy(serviceUrl) {
		this.serviceUrl = serviceUrl;

		this.invoke = function(method, data, callback, error) {
			var url = serviceUrl + "/" + method + "?" + data;

			$.ajax({
				url: url,
				data: data,
				type: "GET",
				processData: false,
                contentType: "application/json",
                timeout: 10000,
                dataType: "json",
                success: callback,
                error: error
			});
		}
	}
	var service = serviceProxy("<%= Perenthia.Web.WebUtils.GetPlayUri() %>Services/AjaxGameService.svc");
</script>
