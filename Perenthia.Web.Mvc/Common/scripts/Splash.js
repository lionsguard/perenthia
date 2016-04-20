var PromptFinishInstall = "<div><p>You are now installing Silverlight, refresh your browser when done.</p></div>";
var PromptUpgrade = "<div><p onclick='UpgradeClicked'>This application needs you to upgrade the Silverlight plugin that runs it. An older version is installed. Click here to upgrade it.</p></div>";
var PromptFinishUpgrade = "<div><p>You are now upgrading Silverlight. When this is done, please restart your browser.</p></div>";
var PromptRestart = "<div><p>Please restart your browser.</p></div>";
var PromptNotSupported = "<div><p>This browser doesn't support Silverlight!</p></div>";

function onSilverlightError(sender, args) {
	var appSource = "";
	if (sender != null && sender != 0) {
		appSource = sender.getHost().Source;
	}

	var errorType = args.ErrorType;
	var iErrorCode = args.ErrorCode;

	if (errorType == "ImageError" || errorType == "MediaError") {
		return;
	}

	var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

	errMsg += "Code: " + iErrorCode + "    \n";
	errMsg += "Category: " + errorType + "       \n";
	errMsg += "Message: " + args.ErrorMessage + "     \n";

	if (errorType == "ParserError") {
		errMsg += "File: " + args.xamlFile + "     \n";
		errMsg += "Line: " + args.lineNumber + "     \n";
		errMsg += "Position: " + args.charPosition + "     \n";
	}
	else if (errorType == "RuntimeError") {
		if (args.lineNumber != 0) {
			errMsg += "Line: " + args.lineNumber + "     \n";
			errMsg += "Position: " + args.charPosition + "     \n";
		}
		errMsg += "MethodName: " + args.methodName + "     \n";
	}

	throw new Error(errMsg);
}

function onSilverlightLoad(sender) {
    //Silverlight.IsVersionAvailableOnLoad(sender);
}

Silverlight.onRequiredVersionAvailable = function() {
};

Silverlight.onRestartRequired = function() {
    document.getElementById("silverlightControlHost").innerHTML = PromptRestart;
};

Silverlight.onUpgradeRequired = function() {
    document.getElementById("silverlightControlHost").innerHTML = PromptUpgrade;
};

Silverlight.onInstallRequired = function() {
};

function UpgradeClicked() {
    window.location = "http://go2.microsoft.com/fwlink/?linkid=124807";
    document.getElementById("silverlightControlHost").innerHTML = PromptFinishUpgrade;
}

function InstallClicked() {
    window.location = "http://go2.microsoft.com/fwlink/?linkid=124807";
    document.getElementById("silverlightControlHost").innerHTML = PromptFinishInstall;
}