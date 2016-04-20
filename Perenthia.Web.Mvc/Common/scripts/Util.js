function gamePop(url) {
	var win = window.open(url, 'Perenthia', 'status=no,location=no,toolbar=no,menubar=no,directories=no,resizable=no,scrollbars=no,width=995,height=660');
	win.focus();
}

function popThumb(title, url, width, height) {
	openWinWithOptions('/Image?title=' + escape(title) + '&image=' + escape(url), 'image', width, height, 'no', 'no', 'no', 'no', 'no', 'no', 'yes');
}

function openWin(url, name, width, height) {
	var win = window.open(url, name, 'status=no,location=no,toolbar=no,menubar=no,directories=no,resizable=no,scrollbars=no,width=' + width + ',height=' + height);
	win.focus();
}

function openWinWithOptions(url, name, width, height, status, location, toolbar, menubar, directories, resizable, scrollbars) {
	var win = window.open(url, name, 'status=' + status + ',location=' + location +',toolbar=' + toolbar +',menubar=' + menubar +',directories=' + directories +',resizable=' + resizable +',scrollbars=' + scrollbars +',width=' + width + ',height=' + height);
	win.focus();
}

function handleError(url) {
	if (window.opener) {
		window.opener.window.navigate(url);
		window.close();
	}
	else {
		window.navigate(url);
	}
}