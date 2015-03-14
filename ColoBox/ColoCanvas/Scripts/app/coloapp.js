define(['jquery',
        'app/renderer',
        'app/routing',
        'app/services/dataService',
        'app/directives/serverFiles',
        'app/controllers/step1Ctrl',
        'app/controllers/optimizerCtrl'],
        function ($,
            renderer,
            routing,
            dataService,
            serverFiles,
            step1Ctrl,
            optimizerCtrl) {
    function init() {
        var app = angular.module('coloapp', ['ngResource','ngRoute']);        

        //init services
        dataService.init(app);

        //init directives
        serverFiles.init(app);

        //config the routing
        routing.configRouting(app);

        //init controllers
        step1Ctrl.init(app);
        optimizerCtrl.init(app);

        

        

    } //init
    

    init();
    angular.bootstrap(document, ['coloapp']);
});