mergeInto(LibraryManager.library, {
    IsDocumentInFullScreen: function() {
        return !!(document.fullscreenElement ||
                 document.webkitFullscreenElement ||
                 document.mozFullScreenElement ||
                 document.msFullscreenElement);
    }
});
