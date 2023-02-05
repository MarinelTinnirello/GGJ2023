mergeInto(LibraryManager.library, {
	LaunchURL: function (url) {
		url = Pointer_stringify(url);
		
		document.onmouseup = function()
        {
            window.open(url,'_blank');
            document.onmouseup = null;
			console.log('Opening link : ' + url);
        }
	}
});