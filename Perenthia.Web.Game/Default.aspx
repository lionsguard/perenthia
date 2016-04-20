<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Perenthia.Web._Default" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Perenthia</title>
	<style type="text/css">
		html, body
		{
			height: 100%;
			overflow: hidden;
		}
		body
		{
			padding: 0;
			margin: 0;
		}
		#silverlightControlHost
		{
			height: 100%;
			text-align: center;
		}
	</style>

	<script type="text/javascript" src="/commmon/scripts/Silverlight.js"></script>

	<script type="text/javascript" src="/common/scripts/Silverlight.supportedUserAgent.js"></script>

	<script type="text/javascript" src="/common/scripts/Splash.js"></script>

	<script type="text/javascript">
		function getViewPort() {
			var viewportwidth;
			var viewportheight;

			// the more standards compliant browsers (mozilla/netscape/opera/IE7) use window.innerWidth and window.innerHeight

			if (typeof window.innerWidth != 'undefined') {
				viewportwidth = window.innerWidth,
				viewportheight = window.innerHeight
			}

			// IE6 in standards compliant mode (i.e. with a valid doctype as the first line in the document)

			else if (typeof document.documentElement != 'undefined'
				 && typeof document.documentElement.clientWidth !=
				 'undefined' && document.documentElement.clientWidth != 0) {
				viewportwidth = document.documentElement.clientWidth,
				viewportheight = document.documentElement.clientHeight
			}

			// older versions of IE

			else {
				viewportwidth = document.getElementsByTagName('body')[0].clientWidth,
				viewportheight = document.getElementsByTagName('body')[0].clientHeight
			}
			//alert('Your viewport width is ' + viewportwidth + 'x' + viewportheight);

			var viewPort = new Object();
			viewPort.width = viewportwidth - 20; // scrollbar
			viewPort.height = viewportheight;
			return viewPort;
		} 
	</script>

</head>
<body>
	<form id="form1" runat="server" style="height: 100%">
	<div id='errorLocation' style="font-size: small; color: #ffffff;"></div>
	<div id="silverlightControlHost">
		<object id="silverlightCtl" data="data:application/x-silverlight-2," type="application/x-silverlight-2"
			width="100%" height="100%">
			<param name="source" value="/ClientBin/Perenthia.Loader.xap" />
			<param name="onError" value="onSilverlightError" />
			<param name="isWindowless" value="false" />
			<param name="background" value="black" />
			<param name="minRuntimeVersion" value="3.0.40818.0" />
			<param name="autoUpgrade" value="true" />
			<param name="EnableFrameRateCounter" value="<%= IsDebug %>" />
			<param name="EnableGPUAcceleration" value="true" />
			<param name="EnableCacheVisualization" value="false" />
			<param name="initParams" value="<%= InitParams %>" />
			<param name="SplashScreenSource" value="/common/xaml/splash.xaml" />
			<div id="SLInstallFallback">
				<div>
					<p onclick='UpgradeClicked'>
						<a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40818.0" style="text-decoration: none">
							<img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight"
								style="border-style: none" />
						</a>
					</p>
				</div>
			</div>
		</object>
		<iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px;
			border: 0px"></iframe>
	</div>
	<div id="silverlightExperienceHost" style="visibility: hidden;"></div>

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

	<script type="text/javascript">
//		var viewPort = getViewPort();
//		var obj = document.getElementById('silverlightCtl');
//		if (obj != null && viewPort != null) {
//			obj.width = viewPort.width;
//			obj.height = viewPort.height;
//		}
	</script>

	</form>
</body>
</html>
