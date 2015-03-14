define(function () {
    function configRouting(app){
        app.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $locationProvider.html5Mode(true);
            $routeProvider.
            when('/', {
                templateUrl: '/scripts/app/templates/step1.html',
                controller: 'step1Ctrl'
            })
            .when('/optimize', {
                templateUrl: '/scripts/app/templates/optimze.html',
                controller: 'optimizerCtrl'
            });
        }]);
    }

    return {
        configRouting: configRouting
    }
});