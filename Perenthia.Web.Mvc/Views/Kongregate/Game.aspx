<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Perenthia.Web.Models.PlayViewData>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xml:lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Perenthia</title>
	
    <!-- Silverlight specific scripts -->
	<script type="text/javascript" src="/common/scripts/Silverlight.js"></script>
	<script type="text/javascript" src="/common/scripts/Silverlight.supportedUserAgent.js"></script>
	<script type="text/javascript" src="/common/scripts/Splash.js"></script>
	
    <!-- Load the Kongregate Javascript API -->
    <script language="javascript" type="text/javascript" src="http://www.kongregate.com/javascripts/kongregate_api.js"></script>
    
    <!-- Give the shell no border/scroll bars and match the Kongregate background color.
         If your game needs scrollbars, you might need to modify these styles -->
    <style>
      html{border: none; overflow: hidden; background-color: #000;height: 100%;}
      body{border: none; background-color: #000;margin:0; padding:0;}
    </style>
</head>
<body>
    <script language="javascript" type="text/javascript">
    	var kongregate = null;
    	
    	// Called when the API is finished loading
    	function onLoadCompleted() {
    		// Get a global reference to the kongregate API
    		kongregate = kongregateAPI.getAPI();

    		// Embed the game into the "contentdiv" div, which is defined below. You can also
    		// manually create your own iframe, this function is just for convenience.
    		// kongregateAPI.embedFrame("game.html");
    		alert("kongregate loaded");
    	}

    	// Begin the process of loading the Kongregate API:
    	kongregateAPI.loadAPI(onLoadCompleted);
    </script>
	<div id='errorLocation' style="font-size: small; color: #ffffff;">
	</div>
	<div id="silverlightControlHost">
		<object data="data:application/x-silverlight," type="application/x-silverlight" width="700"
			height="500">
			<param name="source" value="/ClientBin/Perenthia.Loader.xap" />
			<param name="onload" value="onSilverlightLoad" />
			<param name="onerror" value="onSilverlightError" />
			<param name="background" value="black" />
			<param name="minRuntimeVersion" value="3.0.40818.0" />
			<param name="autoUpgrade" value="true" />
			<param name="SplashScreenSource" value="/common/xaml/splash.xaml" />
			<param name="initParams" value="<%= String.Format(Game.INIT_PARAMS_FORMAT, Model.AuthKey, Model.ServicesRootUri, Model.GameService, Model.ArmorialService, Model.BuilderService, Model.SecurityService, Model.MediaUri, Model.Version, Model.Mode) %>" />
			<div id="SLInstallFallback">
				<div>
					<p onclick='UpgradeClicked'>
						This application needs you to use the Silverlight plugin to use it.</p>
					<a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40818.0" style="text-decoration: none">
						<img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight"
							style="border-style: none" />
					</a>
				</div>
			</div>
		</object>
		<iframe style='visibility: hidden; height: 0; width: 0; border: 0px'></iframe>
	</div>
	<div id="silverlightExperienceHost" style="visibility: hidden;">
	</div>

	<script type="text/javascript">
		try {
			if (navigator.plugins["Silverlight Plug-In"].description) {
				document.getElementById("SLInstallFallback").innerHTML = PromptRestart;
			}
		}
		catch (e) { }
	</script>

	<script type="text/javascript">
		function CheckSupported() {
			var tst = Silverlight.supportedUserAgent();
			if (tst) {
				// Do nothing
			}
			else {
				document.getElementById("silverlightControlHost").innerHTML = PromptNotSupported;
			}
		}
		CheckSupported();
	</script>

</body>
</html>
