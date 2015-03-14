define(function () {
    function init(app) {
        app.directive('serverFiles', function (coloService) {
            return {
                restrict: 'A',
                link: function (scope, elem, attrs) {
                    $(elem).append('<option value="1">1</option><option value="2">2</option>');
                }
            }
        });
    }

    return {
        init: init
    }
});