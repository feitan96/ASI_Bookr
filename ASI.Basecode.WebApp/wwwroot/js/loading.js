// wwwroot/js/loading.js
function showLoading() {
    $('#loading-overlay').css('display', 'flex');
}

function hideLoading() {
    $('#loading-overlay').css('display', 'none');
}

// Global Ajax setup for automatic loading
$(document).ajaxStart(function () {
    showLoading();
});

$(document).ajaxStop(function () {
    hideLoading();
});