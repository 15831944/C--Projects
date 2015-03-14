define(['app/renderer'],function (renderer) {
    function init(app) {
        app.controller('optimizerCtrl', function ($scope, coloService) {
            $scope.originX = 0;
            $scope.originY = 0;
            $scope.delay = 10;
            $scope.drawing = 'Ken-Crazy-Colo';
            $scope.direction = "L2R";
            $scope.coldAisleWidth = 80;
            $scope.spaceAfterObstacle = 10;
            $scope.howManyRacks = 507;
            $scope.indexToStop = 18;
            $scope.rachCountsBeforeOpening = 600;
            $scope.rackOpeningSpace = 10;
            $scope.UniverseLayer = "AB02-0701111$0$AX-WALL";
            $scope.Start = function (arg) {
                $ = jQuery;
                $('#svgid').empty();
                var data = {
                    ColdAisleWidth: $('#ColdWidth').val(),
                    SpaceAfterObstacle: $('#SpaceAfterObstacle').val(),
                    Racks: $('#Racks').val(),
                    RacksBeforeOpenning: $('#RacksBeforeOpening').val(),
                    RacksOpenningSpace: $('#RacksOpenningSpace').val(),
                    IndexToStop: $('#IndexToStop').val(),
                    Direction: $('#direction input[type="radio"]:checked').val(),
                    OriginX: $('#OriginX').val(),
                    OriginY: $('#OriginY').val(),
                    //
                    FileName: $('#drawings input[type="radio"]:checked').val(),
                    OffsetList: $scope.offsets,
                    
                };

                if (data.FileName === 'Ken-Crazy-Colo') {
                    data.StaticLayers = [{ Name: "walls", IsUniverse: true }, { Name: "pillars", IsUniverse: false }];
                } else {
                    data.StaticLayers = [{ Name: "Layer 4", IsUniverse: true }, { Name: "pillars", IsUniverse: false }];
                }

                if ($scope.useExistingData) {
                    data.UseExistingData = $scope.useExistingData = true;
                } else {
                    data.UseExistingData = $scope.useExistingData = false;
                }

                //Regular Ajax
                $.ajax({
                    url: '/Colo/GetResults',
                    data: JSON.stringify(data),
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    success: function (context) {
                        renderer.Render(context);
                    }
                });

            }

            $scope.GetUniverse = function () {
                $('#svgid').empty();
                var data = {
                    FileName: 'ex5',
                    StaticLayers: [{ Name: $scope.UniverseLayer, IsUniverse: true }]
                };

                $.ajax({
                    url: 'Colo/GetUniverse',
                    data: JSON.stringify(data),
                    type: 'POST',
                    contentType: 'application/json; charset=utf-8',
                    success: function (context) {
                        renderer.Render(context);
                    }
                });
            }

            $scope.Refresh = function (arg) {
                renderer.setDrawNumbers($scope.drawNumbers);
                renderer.Refresh();
            }

            $scope.AddOffset = function (arg) {
                var offsets = $scope.offsets || [];
                if (!$scope.LineIndex || !$scope.LineOffset)
                    return;

                var id = offsets.length;
                offsets.push({ lineIndex: $scope.LineIndex, offset: $scope.LineOffset, id: id });


                $scope.offsets = offsets;
                $scope.LineIndex = '';
                $scope.LineOffset = '';

            }

            $scope.RemoveEntry = function (id) {
                var newoffsets = [];
                for (var i = 0; i < $scope.offsets.length; ++i) {
                    if ($scope.offsets[i].id !== this.offset.id) {
                        newoffsets.push($scope.offsets[i]);
                    }
                }

                $scope.offsets = newoffsets;
            }

        });
    }

    return {
        init: init
    }
});