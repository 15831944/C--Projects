define(['app/renderer', 'app/manualSelection'], function (renderer, manualSelection) {
    var theApp;
    function init(app) {
        theApp = app;
        app.controller('step1Ctrl', function ($scope,$location, coloService) {
            $scope.showGetLayers = 'hidden';
            $scope.showLayersList = 'hidden';
            $scope.obstaclePanel = 'hidden';
            $scope.universePanel = 'hidden';
            $scope.stageIndex = 0;
            $scope.lastObstacleId = 1;
            $scope.obstacles = [];
            $scope.getFile = function() {
                coloService.getServerFiles({
                    success: function (arg) {
                        $scope.$apply(function () { $scope.existingFiles = arg.files });
                    },
                    error: function (err) {

                    }
                });
            }
                
            $scope.$watch('selectedFile', function () {
                if ($scope.selectedFile) {
                    $scope.showGetLayers = 'visible';
                    $scope.GetLayers();
                }
            });

            $scope.$watch('selectedLayer', function () {
                manualSelection.reset();
                if (!$scope.selectedLayer) {
                    return;
                }

                $('#svgid').empty();
                var fileName = $scope.selectedFile.replace('.dxf','');
                var data = {
                    FileName: fileName,
                    StaticLayers: [{ Name: $scope.selectedLayer, IsUniverse: true }],
                    Stage: $scope.stageIndex
                };

                $.ajax({
                    url: 'Colo/GetUniverse',
                    data: JSON.stringify(data),
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    success: function (context) {
                        theApp.factor = context.factor;
                        renderer.Render(context);
                    }
                });
            });

            $scope.GetLayers = function () {
                coloService.getLayers($scope.selectedFile,{
                    success: function (data) {
                        $scope.$apply(function () {
                            $scope.layers = data.layers;
                            $scope.showLayersList = 'visibile';
                        });

                    },
                    error: function (err) {

                    }
                });
            }

            $scope.selectObstacles = function () {
                $scope.obstaclePanel = 'visibile';
            }
            $scope.addObstacle = function (points) {
                var id = 'obst' + $scope.lastObstacleId;
                $scope.$apply(function () {
                    $scope.obstacles.push({ id: id, index: $scope.lastObstacleId, selectedNodes: points.selectedNodes });
                });
                $scope.lastObstacleId++;
                manualSelection.reset();
            }
            $scope.removeObstacle = function (index) {
                var obstArr = [];
                for (var i = 0; i < $scope.obstacles.length; ++i) {
                    if ($scope.obstacles[i] !== this.obst) {
                        obstArr.push($scope.obstacles[i]);
                    } else {
                        for (var j = 0; j < this.obst.selectedNodes.length; ++j) {
                            var points = this.obst.selectedNodes[j].render;
                            d3.selectAll("polyline[endpoint='" + points + "']").remove();
                            d3.selectAll("circle[points='" + points + "']").remove();
                        }
                        
                    }
                }
                $scope.obstacles = obstArr;
            }

            $scope.applyObstacles = function () {
                var obstacles = [];
                for (var i = 0; i < $scope.obstacles.length; ++i) {
                    var selectedNodes = $scope.obstacles[i].selectedNodes;
                    var points = [];
                    for (var j = 0; j < selectedNodes.length; ++j) {
                        var pointArr = selectedNodes[j].render.split(',');
                        points.push({ x: pointArr[0], y: pointArr[1] });
                    }
                    obstacles.push({ id: $scope.obstacles[i].index, points: points });
                }
                var payload = { obstacles: obstacles, factor: theApp.factor }
                coloService.applyObstacles(payload, {
                    success: function (data) {

                    },
                    error: function (err) {

                    }
                });
            }
            $scope.selectUniverse = function () {
                $scope.obstaclePanel = 'hidden';
                $scope.universePanel = 'visible';
            }
            $scope.uploadUniverse = function (points) {
                var payload = {};
                var selectedNodes = [];
                for (var i = 0; i < points.selectedNodes.length; ++i) {
                    var pArr = points.selectedNodes[i].render.split(',');
                    selectedNodes.push({ x: pArr[0], y: pArr[1] });
                }
                payload.universeNodes = selectedNodes;
                payload.factor = theApp.factor;
                coloService.uploadSelectedUniversePoints(payload, {
                    success: function (data) {

                    },
                    error: function (err) {

                    }
                });
            }
            $scope.Next = function () {
                $scope.stageIndex++;
                switch ($scope.stageIndex) {
                    case 1:
                        $scope.selectObstacles();
                        break;
                    case 2:
                        $scope.selectUniverse();
                        break;
                    default:
                        $location.path('/optimize');
                }
                
            }
            $('body').on('PolygonClosed', function (e, points) {
                switch ($scope.stageIndex) {
                    case 1:
                        $scope.addObstacle(points);
                        break
                    case 2:
                        $scope.uploadUniverse(points);
                        break;
                }
                
            });
            $scope.getFile();
        });
    }


    return {
        init: init
    }
});