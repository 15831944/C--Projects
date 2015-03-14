define(['jQuery'],function ($) {
    function loadFiles() {
        $.ajax({
            url: 'Colo/LoadDxfList',
            type: 'POST',
            dataType: 'json',
            data: {},
            success: function (data) {
            }
        });
    }
    return {
        loadFiles: loadFiles
    };
});