define(function () {
    function init(app) {
        app.factory('coloService', function ($http, $q) {
            function post(url, data) {
                var deferred = $q.defer();

                deferred.promise = $http.post(url, data).success(function (arg) {
                    //deferred.reject(arg.statusMessage);
                    deferred.resolve(arg);
                }).error(function (err) {

                });

                return deferred.promise;
            }

            function getServerFiles(callbacks) {
                ajaxPost('/Colo/GetDxfFiles', null, callbacks);                           
            }

            function getLayers(fileName,callbacks) {
                ajaxPost('/Colo/GetLayers', { fileName: fileName }, callbacks);
            }

            function uploadSelectedUniversePoints(points, callbacks) {
                ajaxPost('/Colo/UploadSelectedUniversePoints', points, callbacks);
            }

            function applyObstacles(obstacles, callbacks) {
                ajaxPost('/Colo/ApplyObstacles', obstacles, callbacks);
            }
            function ajaxPost(url, data, callbacks) {
                //Regular Ajax
                var ajaxOptions = {
                    url: url,
                    
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    success: function (context) {
                        callbacks.success(context);
                    },
                    error: function (err) {
                        callbacks.error(err);
                    }
                }

                if (data) {
                    //data: JSON.stringify(data),
                    ajaxOptions.data = JSON.stringify(data);
                }
                $.ajax(ajaxOptions);
            }
            return {
                getServerFiles: getServerFiles,
                getLayers: getLayers,
                uploadSelectedUniversePoints: uploadSelectedUniversePoints,
                applyObstacles: applyObstacles,
                processDrawing: function () {

                }
            };

            
        }); //coloService definition
    }

    return {
        init: init
    }
});